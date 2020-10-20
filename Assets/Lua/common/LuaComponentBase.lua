
LuaComponentBase = class("LuaComponentBase")

function LuaComponentBase:AddLuaComponent(obj)
    self.gameObject = obj
    self.transform = obj.transform
   
    local comp = LuaComponent.Bind(obj, self)
    self.mono = comp
    
    if comp and self.Awake then
        self:Awake()
    end
 
	return comp
end

function LuaComponentBase:OnDestroy()

end

function LuaComponentBase:clearMembers()
    for k,v in pairs(self) do
        local t = type(v)
        if t == "userdata" then
            self[k] = nil
        elseif t == "table" and not getmetatable(v) then
            self:clearTable(v, 0)
        end
    end
end

function LuaComponentBase:clearTable(tab, layers)
    local layer_count = layers + 1
    if layer_count > 2 then return end

    for k,v in pairs(tab) do
        if type(v) == "table" and not getmetatable(v) then
            self:clearTable(v, layer_count)
        elseif type(v) == "userdata" then
            tab[k] = nil
        end
    end
end

function LuaComponentBase:addUpdate()
    self:removeUpdate()
    if self.onUpdate then
        UpdateBeat:Add(self.onUpdate, self)
    end
    if self.onLateUpdate then
        LateUpdateBeat:Add(self.onLateUpdate, self)
    end
    if self.onFixedUpdate then
        FixedUpdateBeat:Add(self.onFixedUpdate, self)
    end
end

function LuaComponentBase:removeUpdate()
    if self.onUpdate then
        UpdateBeat:Remove(self.onUpdate, self)
    end
    if self.onLateUpdate then
        LateUpdateBeat:Remove(self.onLateUpdate, self)
    end
    if self.onFixedUpdate then
        FixedUpdateBeat:Remove(self.onFixedUpdate, self)
    end
end

return LuaComponentBase