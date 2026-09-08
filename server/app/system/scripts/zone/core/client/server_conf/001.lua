GuiltineSin.Conf = {}
GuiltineSin.Conf.Options = {}

GuiltineSin.Conf.Init = function(optionTable)
	for k, v in pairs(optionTable) do
		GuiltineSin.Conf.Options[k] = v
	end
end

GuiltineSin.Conf.Get = function(optionName)
	return GuiltineSin.Conf.Options[optionName]
end

GuiltineSin.Conf.GetInt = function(optionName)
	return tonumber(GuiltineSin.Conf.Options[optionName])
end

GuiltineSin.Conf.GetBool = function(optionName)
	local val = GuiltineSin.Conf.Options[optionName]
	return val == "true" or val == "1" or val == "yes"
end
