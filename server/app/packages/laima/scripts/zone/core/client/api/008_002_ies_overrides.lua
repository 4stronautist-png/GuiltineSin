GuiltineSin.Hook("GetClassByNameFromList", function(original, result, clsList, name)

	local idSpace = GuiltineSin.Ies.GetIdSpace(clsList)
	if idSpace then
		local dbCls = GuiltineSin.Ies.GetClassByName(idSpace, name)
		if dbCls ~= nil then
			return dbCls
		end
	end

	return result

end)

GuiltineSin.Hook("GetClass", function(original, result, clsListName, clsName)

	local dbCls = GuiltineSin.Ies.GetClassByName(clsListName, clsName)
	if dbCls ~= nil then
		return dbCls
	end

	return result

end)

GuiltineSin.Hook("GetClassByType", function(original, result, clsListName, clsId)

	local dbCls = GuiltineSin.Ies.GetClassById(clsListName, clsId)
	if dbCls ~= nil then
		return dbCls
	end

	return result

end)
