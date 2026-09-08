local sysmenuFrame = ui.GetFrame("sysmenu")

GuiltineSin.Ui.RestMenu.Buttons = {}

GuiltineSin.Ui.RestMenu.Clear = function()
	GuiltineSin.Ui.RestMenu.Buttons = {}
end

GuiltineSin.Ui.RestMenu.AddButton = function(cls)
	table.insert(GuiltineSin.Ui.RestMenu.Buttons, cls)
end

GuiltineSin.Ui.RestMenu.RemoveButton = function(name)
	for i, cls in ipairs(GuiltineSin.Ui.RestMenu.Buttons) do
		if cls.Script == name then
			table.remove(GuiltineSin.Ui.RestMenu.Buttons, i)
			return
		end
	end
end

local list, count = GetClassList("restquickslotinfo")
for i = 0, count - 1 do
	local cls = GetClassByIndexFromList(list, i)
	if cls ~= nil and (cls.VisibleScript == "None" or _G[cls.VisibleScript]() == 1) then
		-- icon_rest_fire -> RestFire
		--local name = cls.Icon:gsub("^icon_", ""):gsub("^(%a)", string.upper):gsub("_([%w])", string.upper):gsub("_", "")

		GuiltineSin.Ui.RestMenu.AddButton(cls)
	end
end

GuiltineSin.Override("ON_RESTQUICKSLOT_OPEN", function(original, frame, msg, argStr, argNum)
	for i, cls in ipairs(GuiltineSin.Ui.RestMenu.Buttons) do
		local slot = GET_CHILD(frame, "slot"..i, "ui::CSlot")
		if slot ~= nil then
			slot:ReleaseBlink()
			slot:ClearIcon()
			SET_REST_QUICK_SLOT(slot, cls)
		end
	end

	frame:ShowWindow(1);

	if IsJoyStickMode() == 0 then
		local quickFrame = ui.GetFrame("quickslotnexpbar")
		quickFrame:ShowWindow(0);
	elseif IsJoyStickMode(pc) == 1 then
		local joystickQuickFrame = ui.GetFrame("joystickquickslot")
		joystickQuickFrame:ShowWindow(0);
	end

	OPEN_REST_QUICKSLOT(frame);
end)
