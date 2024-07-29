local protobuf = require "pb"
local cjsonutil = require "cjson.util"
local crypt = require "crypt"
local errcode = require "Error.errcode"

LoginPart = Class("LoginPart")

LoginPart.LoginFailNum = 0

function LoginPart.NeedTryLogin(tryType)
    -- 有登陆成功过并且非重连需要重新登陆
    if LoginPart.ReConnect_Token ~= 0 and not LoginPart.CanReConnect then
        -- txt 连接已重置，请重新登陆
        local alert = Modules.Common:OpenUI("Alert")
        alert:SetContent(LLanguageMgr.GetLang(LangID.ConnectionRest))

        LNetMgr.CloseConnect()
        return
    end

    if tryType == TryLoginType.ConnectFail then
        -- 1:连接服务器失败
        LoginPart.LoginFailNum = LoginPart.LoginFailNum + 1
        if LoginPart.LoginFailNum < 3 or (LoginPart.LoginFailNum < 5 and not LoginPart.CanReConnect) then
            -- 提示：网络已断开，将重新连接。
            local alert = Modules.Common:OpenUI("Alert")
            alert:SetContent(LLanguageMgr.GetLang(LangID.Disconnection))
        else
            -- 提示：需重新登陆服务器
            local alert = Modules.Common:OpenUI("Alert")
            alert:SetContent(LLanguageMgr.GetLang(LangID.ReLogin))
        end
    elseif tryType == TryLoginType.Disconnect then
        -- 2:断开连接
        if LoginPart.Logining then
            -- 提示：网络已断开，点确定重连。
            local alert = Modules.Common:OpenUI("Alert")
            alert:SetContent(LLanguageMgr.GetLang(LangID.Disconnection))
        else
            -- 直接重连
            CommandManager.Execute(CommandID.DoLogin)
            return
        end
    elseif tryType == TryLoginType.RequestTimeout then
        -- 3:请求超时
        if LoginPart.Logining then
            -- 提示：连接超时，即将重连。
            local alert = Modules.Common:OpenUI("Alert")
            alert:SetContent(LLanguageMgr.GetLang(LangID.Disconnection))
        else
            -- 直接重连
            CommandManager.Execute(CommandID.DoLogin)
            return
        end
    elseif tryType == TryLoginType.PingTimeout then
        -- 4:ping超时
        -- 直接重连
        CommandManager.Execute(CommandID.DoLogin)
        return

    else
        LogError("错误登陆请求，tryType:" .. tostring(tryType))
    end

    -- 关闭连接
    LNetMgr.CloseConnect()
end

function LoginPart.DoLogin()
    LNetMgr.CloseConnect()
    LNetMgr.CheckConnect()
end

Proto = Proto or {}

local playerInfo = require "Entity.PlayerInfo"

function Proto.res_acknowledgment(args)
	playerInfo.acknumber = crypt.base64decode(args.acknumber)
	playerInfo.clientkey = crypt.randomkey()
	LNetMgr.SendMessage ("req_handshake", {
		client_pub = crypt.base64encode(crypt.dhexchange(playerInfo.clientkey)),
	})
end

local cb_handshake = function(req, opflag, error_code)
	if not opflag then
		print(string.format("<error> RESPONSE.handshake errcode:%d(%s)", error_code, errcode.error_msg(error_code)))
		return
	end
	LNetMgr.SendMessage ("req_auth", { 
		username = crypt.base64encode(crypt.desencode(playerInfo.secret, playerInfo.username)),
		password = crypt.base64encode(crypt.desencode(playerInfo.secret, playerInfo.password)),
	})
end

function Proto.res_handshake(resp)
	if not resp then
		print(string.format("<error> RESPONSE.handshake resp is nil:"))
		return
	end
	playerInfo.secret = crypt.dhsecret(crypt.base64decode(resp.secret), playerInfo.clientkey)
	print("sceret is ", crypt.hexencode(playerInfo.secret))

	local hmac = crypt.base64encode(crypt.hmac64(playerInfo.acknumber, playerInfo.secret))
	LNetMgr.SendMessage ("req_challenge", { hmac = hmac }, cb_handshake)
end

function Proto.res_auth(resp)
	if not resp then
		print(string.format("<error> RESPONSE.auth resp is nil:"))
		return
	end

	playerInfo.login_session = resp.login_session
	playerInfo.login_session_expire = resp.expire
	playerInfo.token = crypt.base64decode(resp.token)

	-- 跳转到游戏服务器
	LNetMgr.SendMessage ("req_switchgame", nil)

	--message.register(protoloader.GAME)
	playerInfo.authed = true

	-- 请求角色列表
	LNetMgr.SendMessage ("req_character_list", nil)
end

function Proto.res_character_list (resp)
	resp = resp or {}
	local character = resp.character or {}

	local character_id
	if type(character) == "table" then
		character_id = next(character)
	end
	if not character_id then
	    Log(string.format("create character"))
		LNetMgr.SendMessage("req_character_create", {
			character = {
				name = string.format("%s-%s", playerInfo.username, "hello"),
				race = "human",
				class = "warrior" ,
			},
		})
	else
	    Log(string.format("choose characterId: %s", tostring(character_id)))
		LNetMgr.SendMessage("req_character_pick", {
            id = character_id,
		})
	end
end

function Proto.res_character_create ()
	LNetMgr.SendMessage ("req_character_list")
end

function Proto.res_character_pick (resp)
	print(string.format("<== RESPONSE res_character_pick character: %s",
						cjsonutil.serialise_value(resp)))


    --进入主界面
    CommandManager.Execute(CommandID.OpenUI, "LobbyMainMgr")
end