local sysmenuFrame = ui.GetFrame("sysmenu")

GuiltineSin.Ui.SysMenu.AddButton = function(name, icon, tooltip, onClick)
	local btnCount = #GuiltineSin.Ui.SysMenu.Buttons
	local pos = btnCount + 1
	
	local btnInfo = { ["name"] = name, ["icon"] = icon, ["tooltip"] = tooltip, ["onClick"] = onClick }
	table.insert(GuiltineSin.Ui.SysMenu.Buttons, pos, btnInfo)

	GuiltineSin.Ui.SysMenu.Refresh()
end

GuiltineSin.Ui.SysMenu.InsertButton = function(pos, name, icon, tooltip, onClick)
	local btnCount = #GuiltineSin.Ui.SysMenu.Buttons
	pos = math.max(1, math.min(btnCount + 1, pos))
	
	local btnInfo = { ["name"] = name, ["icon"] = icon, ["tooltip"] = tooltip, ["onClick"] = onClick }
	table.insert(GuiltineSin.Ui.SysMenu.Buttons, pos, btnInfo)

	GuiltineSin.Ui.SysMenu.Refresh()
end

GuiltineSin.Ui.SysMenu.RemoveButton = function(name)
	local buttons = GuiltineSin.Ui.SysMenu.Buttons

	for	i = 1, #buttons do
		local btnInfo = buttons[i]
		if btnInfo.name == name then
			table.remove(buttons, i)
			break
		end
	end

	GuiltineSin.Ui.SysMenu.Refresh()
end
