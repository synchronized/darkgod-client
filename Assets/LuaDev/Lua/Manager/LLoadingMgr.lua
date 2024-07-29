-- region *.lua
-- Date
-- 此文件由[BabeLua]插件自动生成

local Timer = require "System.Timer"

---@class LLoadingCtrl @ ---------------------------------------------------------------
LLoadingCtrl = LLoadingCtrl or {
    NeedShowCongLian = false,
}

local function fnOpenLoadingUI()
    LLoadingCtrl.NeedShowCongLian = true
    CommandManager.Execute(CommandID.OpenUI, "CommonMgr", "LoadingUI")
end

local function fnCheckCloseLoading()
    if LLoadingMgr.Num > 0 then
        return
    end

    LLoadingCtrl.NeedShowCongLian = false
    CommandManager.Execute(CommandID.CloseUI, "CommonMgr", "LoadingUI")
end

-- =======================================================================

local function LLoadingMgrCreate()
    ---@class LLoadingMgr @ ---------------------------------------------------------------
    local selfClass = LLoadingMgr or {
        Num = 0,
    }
    selfClass.timerSet = selfClass.timerSet or { }

    -- 锁定屏幕，超时后回调并解锁，返回定时器
    ---@param waitTime number
    ---@param callBack function
    function selfClass.Lock(waitTime, callBack)
        fnOpenLoadingUI()

        selfClass.Num = selfClass.Num + 1
        local timer
        timer = Timer.New(function()
            selfClass.UnLock(timer)

            if callBack ~= nil then
                callBack()
            end
        end, waitTime)
        timer:Start()
        selfClass.timerSet[timer] = 1
        return timer
    end

    -- 主动解锁 传入定时器
    ---@param timer table
    function selfClass.UnLock(timer)
        if timer == nil or selfClass.timerSet[timer] == nil then
            return
        end

        timer.Stop()
        selfClass.Num = selfClass.Num - 1
        fnCheckCloseLoading()
    end
    return selfClass
end

-- 超时等待
---@class LLoadingMgr @ ---------------------------------------------------------------
LLoadingMgr = LLoadingMgrCreate()

-- =======================================================================

local function LReconnectingMgrCreate()
    -- 网络重连用
    ---@class LReconnectingMgr @ ---------------------------------------------------------------
    local selfClass = LReconnectingMgr or {
        -- 延迟转圈等待定时器
        timerLoading = nil,
        -- 重连定时器
        timerReconnecting = nil,
        -- 锁定中
        LockIng = false,
    }

    ---@param showLoadingTime number
    ---@param reconnectTime number
    function selfClass.Lock(showLoadingTime, reconnectTime)
        selfClass.LockIng = true
        selfClass.RemoveTime()

        -- 延迟转圈等待
        local fnShowLoading = function()
            LLoadingMgr.UnLock(selfClass.timerLoading)
            fnOpenLoadingUI()
        end
        if showLoadingTime > 0 then
            selfClass.timerLoading = LLoadingMgr.Lock(showLoadingTime, fnShowLoading)
        else
            fnShowLoading()
        end

        -- 太久了断开连接，重新调用登陆
        selfClass.timerReconnecting = LLoadingMgr.Lock(reconnectTime, function()
            LLoadingMgr.UnLock(selfClass.timerReconnecting)
            LogWarning("GlobalPart.ChongLianView ReConnect")

            --尝试再次登陆
            --LoginPart.NeedTryLogin(3)
            CommandManager.Execute(CommandID.TryLogin)
        end)
    end

    function selfClass.UnLock()
        if not selfClass.LockIng then
            return
        end

        selfClass.LockIng = false
        selfClass.RemoveTime()
        fnCheckCloseLoading()
    end

    function selfClass.IsLock()
        return selfClass.LockIng
    end

    function selfClass.RemoveTime()
        LLoadingMgr.UnLock(selfClass.timerLoading)
        LLoadingMgr.UnLock(selfClass.timerReconnecting)
    end
    return selfClass
end

-- 网络重连用
---@class LReconnectingMgr @ ---------------------------------------------------------------
LReconnectingMgr = LReconnectingMgrCreate()
