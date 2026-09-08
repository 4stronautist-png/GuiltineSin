GuiltineSin.DoString = function(code)
	local chunk, err = load(code)
	if chunk then
		chunk(func)
		return true
	else
		GuiltineSin.Log.Error('Execution failed. Error: ' .. err)
		return false
	end
end
