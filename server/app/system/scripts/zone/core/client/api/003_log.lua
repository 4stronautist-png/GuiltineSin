GuiltineSin.Log = {}

GuiltineSin.Log.Info = function(format, ...)
	GuiltineSin.Log.Write("INFO_NORMAL", format, ...)
end

GuiltineSin.Log.Error = function(format, ...)
	GuiltineSin.Log.Write("ERROR_LOGIC", format, ...)
end

GuiltineSin.Log.Warning = function(format, ...)
	GuiltineSin.Log.Write("WARNING_DEBUG", format, ...)
end

GuiltineSin.Log.Write = function(errorCode, format, ...)
	local argCount = select("#", ...)
	
	if argCount == 0 then
		IMC_LOG(errorCode, GuiltineSin.Util.Serialize(format))
		return
	end

	local text = format;

	for i = 1, argCount do
		local indexStr = "{" .. (i - 1) .. "}"
		local replacement = GuiltineSin.Util.Serialize(select(i, ...))

		text = string.gsub(text, indexStr, replacement)
	end

	IMC_LOG(errorCode, text)
end
