param(
    [string]$Root = (Resolve-Path (Join-Path $PSScriptRoot '..')).ProviderPath,
    [int]$TargetLevel = 500,
    [switch]$ShowMainWarnings,
    [switch]$ShowKnowledgeWarnings,
    [switch]$SkipLogScan
)

$ErrorActionPreference = 'Stop'

function Invoke-Step {
    param(
        [string]$Name,
        [scriptblock]$Action
    )

    Write-Host ""
    Write-Host "==> $Name" -ForegroundColor Cyan
    & $Action
    Write-Host "OK  $Name" -ForegroundColor Green
}

function Invoke-Tool {
    param(
        [string]$Path,
        [string[]]$Arguments = @()
    )

    if (-not (Test-Path -LiteralPath $Path)) {
        throw "Missing tool: $Path"
    }

    & $Path @Arguments
    if ($LASTEXITCODE -ne 0) {
        throw "Tool failed with exit code ${LASTEXITCODE}: $Path"
    }
}

function Invoke-ToolCapture {
    param(
        [string]$Path,
        [string[]]$Arguments = @()
    )

    if (-not (Test-Path -LiteralPath $Path)) {
        throw "Missing tool: $Path"
    }

    $output = & $Path @Arguments 2>&1
    if ($LASTEXITCODE -ne 0) {
        $output | ForEach-Object { Write-Host $_ }
        throw "Tool failed with exit code ${LASTEXITCODE}: $Path"
    }

    return @($output)
}

function Assert-LastExitCode {
    param(
        [string]$Name
    )

    if ($null -ne $LASTEXITCODE -and $LASTEXITCODE -ne 0) {
        throw "Step failed with exit code ${LASTEXITCODE}: $Name"
    }
}

function Test-LogForQuestRuntimeErrors {
    $patterns = @(
        'Exception',
        '\[Error\]',
        'unknown monster',
        'non-existent target',
        'stale handle',
        'loaded 0 scripts',
        'dialog.*fail',
        'Quest.*failed'
    )

    $roots = @(
        (Join-Path $Root 'logs'),
        'C:\GuiltineSin\release\log_Client',
        'C:\GuiltineSin\release\DisconnectLog'
    )

    $files = foreach ($logRoot in $roots) {
        if (Test-Path -LiteralPath $logRoot) {
            Get-ChildItem -LiteralPath $logRoot -File -Recurse -ErrorAction SilentlyContinue |
                Where-Object { $_.LastWriteTime -ge (Get-Date).AddDays(-2) }
        }
    }

    if (-not $files) {
        Write-Host "No recent log files found for scan."
        return
    }

    $matches = foreach ($file in $files) {
        Select-String -LiteralPath $file.FullName -Pattern $patterns -CaseSensitive:$false -ErrorAction SilentlyContinue
    }

    $filtered = @($matches | Where-Object {
        $_.Line -notmatch 'recommended maximum length' -and
        $_.Line -notmatch 'default password for inter-server communication' -and
        $_.Line -notmatch 'Lost connection to coordinator, will try to reconnect' -and
        -not ($_.Path -match 'BarracksServer' -and $_.Line -match 'loaded 0 scripts')
    })

    if ($filtered.Count -gt 0) {
        $filtered |
            Select-Object -First 80 |
            ForEach-Object {
                $relative = $_.Path
                if ($relative.StartsWith($Root, [System.StringComparison]::OrdinalIgnoreCase)) {
                    $relative = $relative.Substring($Root.Length).TrimStart('\', '/')
                }
                Write-Host "LOG: ${relative}:$($_.LineNumber): $($_.Line)"
            }

        throw "Recent quest/client runtime log scan found $($filtered.Count) suspicious line(s)."
    }

    Write-Host "Recent quest/client runtime log scan clean."
}

$papayaFlow = Join-Path $PSScriptRoot 'Validate-PapayaMainQuestFlow.ps1'
$mainChain = Join-Path $PSScriptRoot 'Validate-MainQuestChain.ps1'
$simulation = Join-Path $PSScriptRoot 'Simulate-QuestProgressionToLevel.ps1'
$knowledge = Join-Path $PSScriptRoot 'Validate-TosKnowledge.ps1'

Write-Host "=========================================="
Write-Host "   GUILTINESIN QUEST REGRESSION"
Write-Host "=========================================="
$Root = (Resolve-Path -LiteralPath $Root).ProviderPath
Write-Host "Root: $Root"

Invoke-Step 'Papaya captured main quest flow' {
    $global:LASTEXITCODE = 0
    & $papayaFlow -Root $Root
    Assert-LastExitCode 'Papaya captured main quest flow'
}

Invoke-Step 'Main quest chain runtime coverage' {
    $global:LASTEXITCODE = 0
    if ($ShowMainWarnings) {
        & $mainChain -Root $Root -ShowWarnings
    }
    else {
        & $mainChain -Root $Root
    }
    Assert-LastExitCode 'Main quest chain runtime coverage'
}

Invoke-Step "Player-flow simulation to level $TargetLevel" {
    $global:LASTEXITCODE = 0
    & $simulation -Root $Root -TargetLevel $TargetLevel
    Assert-LastExitCode "Player-flow simulation to level $TargetLevel"
}

Invoke-Step 'TOS knowledge consistency' {
    $knowledgeOutput = & $knowledge -AppRoot $Root 2>&1
    $knowledgeText = ($knowledgeOutput -join [Environment]::NewLine)
    $result = $knowledgeText | ConvertFrom-Json

    Write-Host "Knowledge errors: $($result.errors)"
    Write-Host "Knowledge issues: $($result.issue_count)"
    Write-Host "Knowledge warnings: $($result.warnings)"

    if ($ShowKnowledgeWarnings -and $result.issues) {
        $result.issues |
            Select-Object -First 120 |
            ForEach-Object {
                Write-Host "WARN: [$($_.category)] $($_.quest_name): $($_.message)"
            }
    }

    if ([int]$result.errors -gt 0) {
        throw "TOS knowledge consistency reported $($result.errors) error(s)."
    }
}

if (-not $SkipLogScan) {
    Invoke-Step 'Recent quest/client runtime log scan' {
        Test-LogForQuestRuntimeErrors
    }
}

Write-Host ""
Write-Host "Quest regression passed." -ForegroundColor Green
