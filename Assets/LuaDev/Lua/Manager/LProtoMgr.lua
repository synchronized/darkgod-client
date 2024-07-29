local pb = require "pb"

LProtoMgr = LProtoMgr or {}

function LProtoMgr.OnInit()
    local protoBytes = ResManager.LLoadBinaryAssetSyn("Proto/Protobuf/Protocol")
    assert(pb.load(protoBytes))
end

return LProtoMgr