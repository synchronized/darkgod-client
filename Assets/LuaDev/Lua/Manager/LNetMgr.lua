local protobuf = require "pb"
local cjsonutil = require "cjson.util"
local crypt = require "crypt"

LNetMgr = LNetMgr or Class("LNetMgr")

local m_SessionId = 0;
local m_SessionMap = {};

function LNetMgr.CheckConnect()
    Log(string.format("LNetMgr.CheckConnect(%s, %d)", ServerConfig.Address, ServerConfig.Port))
    NetManager.Instance:CheckConnect(ServerConfig.Address, ServerConfig.Port)
end

function LNetMgr.CloseConnect()
    NetManager.Instance:CloseConnect()
end

function LNetMgr.OnReceveServerData(msgName, bytesBody)
    --LogError("-------------------pb.load isOk:", isOk, ", n:", n)
    Log(string.format("<== RESPONSE %s data: %s", msgName, crypt.base64encode(bytesBody)))
    local resp = assert(protobuf.decode('proto.'..msgName, bytesBody))
    Log(string.format("<== RESPONSE %s data: %s", msgName, cjsonutil.serialise_value(resp)))

    if "res_msgresult" == msgName then
        LNetMgr.OnResMsgresult(resp)
        return
    end

    local nFunc = Proto[msgName]
    if nFunc == nil then
        LogError(string.format("函数找不到 %s", msgName))
    else
        nFunc(resp)
    end
end

function LNetMgr.SendMessage(msgName, args, cb)
    Log(string.format("==> REQUEST %s data: %s", msgName, cjsonutil.serialise_value(args)))

    local client_session_id = 0;
    if cb ~= nil then
        m_SessionId = m_SessionId+1;
        client_session_id = m_SessionId;
        m_SessionMap[client_session_id] = { name = msgName, req = args, callback = cb};
    end
    --local bytesBody = assert(protobuf.encode('proto.'..msgName, args))
    --MsgSender.Instance:SendMessage(msgName, bytesBody)
    --local buff = string.pack(">s2>I4>s2", msgName, client_session_id, bytesBody)
	local bytesBody = ""
	if args then
		bytesBody = assert(protobuf.encode('proto.'..msgName, args))
	end
	local msg = string.pack(">s2>I4>s2", msgName, client_session_id, bytesBody)
    Log("buff:", crypt.base64encode(msg))
    NetManager.Instance:SendMessage(msg)

end

function LNetMgr.OnResMsgresult(msgResult)
    local client_session_id = msgResult.session
    local session = m_SessionMap[client_session_id]
    if session ~= nil then
        local ok, err_msg = pcall(session.callback, session.req, msgResult.result, msgResult.error_code)
        if not ok then
            print(string.format("    session %s[%d] for msgresult error : %s", session.name, client_session_id, tostring(err_msg)))
        end
    end
end