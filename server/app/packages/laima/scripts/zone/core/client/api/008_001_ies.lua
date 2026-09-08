GuiltineSin.Ies = {}

GUILTINESIN_IES_DB = {}

GuiltineSin.Ies.AddClass = function(idSpace, cls)
	local list = GUILTINESIN_IES_DB[idSpace]

	if not list then
		list = {}
		list["Entries"] = {}
		list["ById"] = {}
		list["ByName"] = {}

		GUILTINESIN_IES_DB[idSpace] = list
	end

	table.insert(list["Entries"], cls)
	list["ById"][cls.ClassID] = cls
	list["ByName"][cls.ClassName] = cls
end

GuiltineSin.Ies.GetClassById = function(idSpace, clsId)
	local list = GUILTINESIN_IES_DB[idSpace]

	if list then
		local cls = list["ById"][clsId]
		if cls then
			return cls
		end
	end

	return nil
end

GuiltineSin.Ies.GetClassByName = function(idSpace, clsName)
	local list = GUILTINESIN_IES_DB[idSpace]

	if list then
		local cls = list["ByName"][clsName]
		if cls then
			return cls
		end
	end

	return nil
end

GuiltineSin.Ies.GetIdSpace = function(clsList)
	local first = GetClassByIndexFromList(clsList, 0)
	return GetIDSpace(first)
end
