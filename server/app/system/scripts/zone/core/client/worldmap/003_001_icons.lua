GuiltineSin.World = {}
GuiltineSin.World.Icons = {}
GuiltineSin.World.Icons.List = {}

GuiltineSin.World.Icons.Load = function(icons)
	GuiltineSin.World.Icons.Clear();

	for i = 1, #icons do
		GuiltineSin.World.Icons.Add(icons[i])
	end
end

GuiltineSin.World.Icons.Add = function(icon)
	table.insert(GuiltineSin.World.Icons.List, icon)
end

GuiltineSin.World.Icons.GetAll = function()
	return GuiltineSin.World.Icons.List
end

GuiltineSin.World.Icons.Clear = function()
	GuiltineSin.World.Icons.List = {}
end
