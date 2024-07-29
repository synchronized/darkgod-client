using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using LuaInterface;

namespace ToLuaGameFramework
{	
    public class MsgDispatcher 
    {

        private static MsgDispatcher m_Instance;

        public static MsgDispatcher Instance {
            get {
                if (m_Instance == null) {
                    m_Instance = new MsgDispatcher();
                }
                return m_Instance;
            }
        }

        private Dictionary<string, EasyEvent<byte[]>> mEvents = new Dictionary<string, EasyEvent<byte[]>>();

        private MsgDispatcher() {
            NetManager.Instance.RegisterReceiveEvent(Dispatcher);
        }

        public void OnInit() {}

        public void Register(string key, Action<byte[]> onEvent)
        {
            if (mEvents.TryGetValue(key, out var easyEvent))
            {
                easyEvent.Register(onEvent);
            }
            else
            {
                easyEvent = new EasyEvent<byte[]>();
                mEvents.Add(key,easyEvent);
                easyEvent.Register(onEvent);
            }
        }

        public void UnRegister(string key, Action<byte[]> onEvent)
        {
            if (mEvents.TryGetValue(key, out var easyEvent))
            {
                easyEvent?.UnRegister(onEvent);
            }
        }

        public bool Send(string key, byte[] data)
        {
            if (mEvents.TryGetValue(key, out var easyEvent))
            {
                easyEvent?.Trigger(data);
                return easyEvent != null;
            }
            return false;
        }

        public void Dispatcher(byte[] bytearray) 
        {
            ByteBuffer buff = new ByteBuffer(bytearray);
            string msgName = buff.ReadNetworkStringUInt16();
            ushort bodySize = buff.ReadNetworkUInt16();
            byte[] bodyBytes = buff.ReadBytes(bodySize);

            Debug.Log(string.Format("[Network]Dispatcher msgname: {0} data: {1}", 
                msgName, Convert.ToBase64String(bodyBytes)));
            if (this.Send(msgName, bodyBytes) == false) {
                //通知lua
                string bodyStr = System.Text.Encoding.Default.GetString(bodyBytes);
                var lfunc = LuaManager.instance.GetFunction("LNetMgr.OnReceveServerData");
                lfunc.Call(msgName, new LuaByteBuffer(bodyBytes));
            }
        }
    }
}
