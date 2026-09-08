GuiltineSin.Comm = {}

local cache = {}

GuiltineSin.Comm.BeginRecv = function(key)
	cache[key] = { ['key'] = key, ['data'] = {} }
end

GuiltineSin.Comm.Recv = function(key, dataset)
	local obj = cache[key]
	
	if obj then
		if type(data) ~= "table" then
			data = {data}
		end

		for	_, v in ipairs(dataset) do
			table.insert(obj.data, v)
		end
	end
end

GuiltineSin.Comm.Exec = function(key, callback)
	local obj = cache[key]
	
	if obj then
		callback(obj)
	end
end

GuiltineSin.Comm.ExecData = function(key, callback)
	local obj = cache[key]
	
	if obj then
		callback(obj.data)
	end
end

GuiltineSin.Comm.EndRecv = function(key)
	cache[key] = nil
end
