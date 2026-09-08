GuiltineSin = {}

GuiltineSin.Backup = function(original)
	return GuiltineSin.BackupIn(_G, original)
end

GuiltineSin.BackupIn = function(parent, original)
	local backupName = "GuiltineSinBackup__" .. original;
	local backup = parent[backupName]
	
	if not backup then
		backup = parent[original]
		parent[backupName] = backup
	end
	
	return backup
end

GuiltineSin.Override = function (original, override)
	return GuiltineSin.OverrideIn(_G, original, override)
end


GuiltineSin.OverrideIn = function (parent, original, override)
	local backup = GuiltineSin.BackupIn(parent, original)
	
	parent[original] = function(...)
		return override(backup, ...)
	end
	
	return backup
end

GUILTINESIN_HOOKS = {}

GuiltineSin.Hook = function(originalName, hook)
	local listName = "_G_." .. originalName

	local list = GUILTINESIN_HOOKS[listName]
	local listExisted = list ~= nil

	if list == nil then
		list = {}
		GUILTINESIN_HOOKS[listName] = list
	end

	table.insert(list, hook)

	if not listExisted then
		GuiltineSin.Override(originalName, function(original, ...)
			local list = GUILTINESIN_HOOKS[listName]
		
			local result = original(...)
		
			if list then
				for _, hook in pairs(list) do
					result = hook(original, result, ...)
				end
			end

			return result
		end)
	end
end
