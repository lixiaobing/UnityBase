-- @brief: UI消息中心
-- @version: 1.0.0
-- @author lanqibo
-- All rights reserved
MsgCenter = {}
local this = MsgCenter

--消息载体池
local _Messagers = {}

--添加消息关联
function MsgCenter.addMessager(msg_name, msgr)
	local msgs = _Messagers[msg_name]
	if not msgs then
		_Messagers[msg_name] = {}
		table.insert(_Messagers[msg_name],msgr)
	else
		for i = #msgs, 1, -1 do
			if msgs[i].id_ == msgr.id_ then
				return
			end
		end
		table.insert(_Messagers[msg_name], msgr)
	end
end

--移除组件全部消息关联
function MsgCenter.removeAllMessager(msgr)
	if msgr and msgr.listeners_ then
		for k,v in pairs(msgr.listeners_) do
			local msgs = _Messagers[k]

			for i = #msgs, 1, -1 do
				if _Messagers[k][i].id_ == msgr.id_ then
					table.remove(_Messagers[k], i)
					break
				end
			end
		end
	end
end

--移除组件指定消息关联
function MsgCenter.removeMessager(msgr, msg_name)

	local msgs = _Messagers[msg_name]

	if msgs then
		for i = #msgs, 1, -1 do
			if msgs[i].id_ == msgr.id_ then
				table.remove(msgs, i)
				break
			end
		end
	end
end

--发送消息
function MsgCenter.sendMessage(msg_name, ...)
	if _Messagers[msg_name] then
		local msgrs = _Messagers[msg_name]
		local count = #msgrs
		for i = #msgrs, 1, -1 do
			if msgrs[i] and msgrs[i].owner_ then
                msgrs[i]:receiveMessage(msg_name, ...)
			end
		end
	end
end

