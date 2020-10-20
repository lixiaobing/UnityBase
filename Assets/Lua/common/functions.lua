--临时容错
local Debug = {}
Debug.Log = print
Debug.LogError = print
Debug.LogWarning = print
function Log(info)
	if type(info) == "table" then
		info = dumpTable(info)
	end
	Debug.Log(string.format("[%s]%s\n%s",Time.frameCount, info, debug.traceback()))
end

function LogError(info)
	if type(info) == "table" then
		info = dumpTable(info)
	end
	Debug.LogError(string.format("[%s]%s\n%s",Time.frameCount, info, debug.traceback()))
end

function LogWarning(info)
	if type(info) == "table" then
		info = dumpTable(info)
	end
	Debug.LogWarning(string.format("[%s]%s\n%s",Time.frameCount, info, debug.traceback()))
end

--[[function BattleLog(info, colorType)
	if type(info) == "table" then
		info = dumpTable(info)
    end
    local colorStr = "white"
    if colorType == 1 then
        colorStr = "blue"
    elseif colorType == 2 then
        colorStr = "green"
    elseif colorType == 3 then
        colorStr = "yellow"
    elseif colorType == 4 then
        colorStr = "red"
    end
	GameLog.BattleLog(string.format("[%s]-- <color=%s>%s</color>", Time.frameCount, colorStr, info), debug.traceback())
end--]]

function dumpTable(value, desciption, nesting)
    if type(nesting) ~= "number" then nesting = 3 end
 
    local lookupTable = {}
    local result = {}
 
    local function _v(v)
        if type(v) == "string" then
            v = "\"" .. v .. "\""
        end
        return tostring(v)
    end
 
    local function _dump(value, desciption, indent, nest, keylen)
        desciption = desciption or "<var>"
        local spc = ""
        if type(keylen) == "number" then
            spc = string.rep(" ", keylen - string.len(_v(desciption)))
        end
        if type(value) ~= "table" then
            result[#result +1 ] = string.format("%s%s%s = %s", indent, _v(desciption), spc, _v(value))
        elseif lookupTable[value] then
            result[#result +1 ] = string.format("%s%s%s = *REF*", indent, desciption, spc)
        else
            lookupTable[value] = true
            if nest > nesting then
                result[#result +1 ] = string.format("%s%s = *MAX NESTING*", indent, desciption)
            else
                result[#result +1 ] = string.format("%s%s = {", indent, _v(desciption))
                local indent2 = indent.."    "
                local keys = {}
                local keylen = 0
                local values = {}
                for k, v in pairs(value) do
                    keys[#keys + 1] = k
                    local vk = _v(k)
                    local vkl = string.len(vk)
                    if vkl > keylen then keylen = vkl end
                    values[k] = v
                end
                table.sort(keys, function(a, b)
                    if type(a) == "number" and type(b) == "number" then
                        return a < b
                    else
                        return tostring(a) < tostring(b)
                    end
                end)
                for i, k in ipairs(keys) do
                    _dump(values[k], k, indent2, nest + 1, keylen)
                end
                result[#result +1] = string.format("%s}", indent)
            end
        end
    end
    _dump(value, desciption, "- ", 1)
 
	local resultStr = ""
    for i, line in ipairs(result) do
        resultStr = resultStr .. line.."\n"
	end
	return resultStr
end

function clone( object )
    local lookup_table = {}
    local function copyObj( object )
        if type( object ) ~= "table" then
            return object
        elseif lookup_table[object] then
            return lookup_table[object]
        end
       
        local new_table = {}
        lookup_table[object] = new_table
        for key, value in pairs( object ) do
            new_table[copyObj( key )] = copyObj( value )
        end
        return setmetatable( new_table, getmetatable( object ) )
    end
    return copyObj( object )
end

function handler(func, self)
    return function(...)  
        if not func then return end
        return func(self, ...) 
    end
end

function isInRange(tar, min, max)
    return tar>=min and tar<=max
end

function string.startWith(content, target)
    local index = string.find(content, target)
    return index == 1
end