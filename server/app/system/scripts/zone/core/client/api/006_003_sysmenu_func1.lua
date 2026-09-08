local sysmenuFrame = ui.GetFrame("sysmenu")

GuiltineSin.Ui.SysMenu.Clear = function()
	sysmenuFrame:RemoveChildByType("button")
	
	GuiltineSin.Ui.SysMenu.Buttons = {}
	GuiltineSin.Ui.SysMenu.HideNoticeTexts()

	GuiltineSin.Ui.SysMenu.Refresh()
end

GuiltineSin.Ui.SysMenu.Refresh = function()
	if GuiltineSin.Ui.SysMenu.RefreshSuspended == true then
		return
	end

	sysmenuFrame:RemoveChildByType("button")
	GuiltineSin.Ui.SysMenu.HideNoticeTexts()

	for i = 1, #GuiltineSin.Ui.SysMenu.Buttons do
		local btnInfo = GuiltineSin.Ui.SysMenu.Buttons[i]
		local index = i - 1

		GuiltineSin.Ui.SysMenu.CreateButton(index, btnInfo.name, btnInfo.icon, btnInfo.tooltip, btnInfo.onClick)
	end
end

GuiltineSin.Ui.SysMenu.SuspendRefresh = function()
	GuiltineSin.Ui.SysMenu.RefreshSuspended = true
end

GuiltineSin.Ui.SysMenu.ResumeRefresh = function()
	GuiltineSin.Ui.SysMenu.RefreshSuspended = false
	GuiltineSin.Ui.SysMenu.Refresh()
end
