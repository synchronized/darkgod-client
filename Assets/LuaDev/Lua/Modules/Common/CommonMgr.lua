local BaseMgr = require "Core.BaseMgr"
local CommonMgr = Class("CommonMgr", BaseMgr)

function CommonMgr:Ctor()
    self.super.Ctor(self)
    self:AddUI("Alert", require "Modules.Common.Alert")
    self:AddUI("LoadingUI", require "Modules.Common.LoadingUI")
end

return CommonMgr
