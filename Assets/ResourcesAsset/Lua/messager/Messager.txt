
local Messager = class("Messager")
local id = 0

function Messager:ctor(owner)
    id = id + 1
    self.id_ = id
    self.owner_ = owner
    self.listeners_ = {}
end

--添加一个监听器
function Messager:addListener(msg_name, callback)
    if msg_name and msg_name ~= "" and callback then
        self.listeners_[msg_name] = self.listeners_[msg_name] or {}

        local include = false
        for i=#self.listeners_[msg_name],1,-1 do
            if self.listeners_[msg_name][i] == callback then
                include = true
                break
            end
        end
        if not include then
            table.insert(self.listeners_[msg_name], callback)
            EventCenter.addMessager(msg_name, self)
        end
    end
end

--移除一个监听器
function Messager:removeListener(msg_name, callback)
    if self.listeners_[msg_name] then
        if not callback then
            self.listeners_[msg_name] = nil
            EventCenter.removeMessager(self, msg_name)
        else
            for i=#self.listeners_[msg_name],1,-1 do
                if self.listeners_[msg_name][i] == callback then
                    table.remove(self.listeners_[msg_name], i)
                    break
                end
            end
            if not next(self.listeners_[msg_name]) then
                EventCenter.removeMessager(self, msg_name)
            end
        end
    end
end

function Messager:sendMessage(msg_name, data)
    EventCenter.sendMessage(msg_name, data)
end

function Messager:receiveMessage(msg_name, ...)
    if self.listeners_[msg_name] then
        local calls = {}
        for _, callback in ipairs(self.listeners_[msg_name]) do
            if self.owner_ then
                table.insert(calls, callback)
            end
        end
        for i,callback in ipairs(calls) do
            callback(self.owner_, ...)
        end
    end
end

function Messager:dispose()
    EventCenter.removeAllMessager(self)
    self.owner_ = nil
    self.listeners_ = {}
end
