CommandID = {
    OpenUI = "OpenUI",
    CloseUI = "CloseUI",

    TryLogin = "TryLogin",
    DoLogin = "DoLogin",
}

TryLoginType = {
    ConnectFail = "ConnectFail", --连接服务器失败
    Disconnect = "Disconnect", --断开连接
    RequestTimeout = "RequestTimeout", --请求超时
    PingTimeout = "PingTimeout", --ping超时
}

--[[
EventID = {
    StorageItemUpdate = 1,
    --仓库物品变化
    BagItemUpdate = 2, --背包物品变化
    PlayerJinBiUpdate = 3, --玩家金币变化
    PlayerZuanShiUpdate = 4, --玩家钻石变化
    PlayerMoLiUpdate = 5, --玩家魔力币变化
    TcpBroadcast = 6, --收到游戏服推送消息
    DDZCustomOpenCards = 7, --斗地主自定义关卡打开选牌
    DDZPokerCustomClickCard = 8, --斗地主自定义关卡点牌
    DDZCustomDeskCardChange = 9, --斗地主自定义关卡牌已创建在桌面或从桌面删除
    DDZCustomRefreshAllSeelctPoker = 10, --斗地主自定义关卡刷新所有牌按钮上的数字
}

EmErrorCode = Abx.Common.Define.EmErrorCode

]]