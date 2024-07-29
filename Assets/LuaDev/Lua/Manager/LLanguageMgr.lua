require "Define.Language"

LLanguageMgr = LLanguageMgr or {
}

local langMap = {
    [LangID.ConnectionRest] = "连接已重置，请重新登陆",
    [LangID.Disconnection] = "网络已断开，将重新连接。",
    [LangID.ReLogin] = "需重新登陆服务器",
}

function LLanguageMgr.GetLang(langId)
    return langMap[langId]
end