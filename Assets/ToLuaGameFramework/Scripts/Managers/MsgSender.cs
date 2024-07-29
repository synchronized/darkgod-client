using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

namespace ToLuaGameFramework
{
    public class MsgSender 
    {
        private static MsgSender m_Instance;

        public static MsgSender Instance {
            get {
                if (m_Instance == null) {
                    m_Instance = new MsgSender();
                }
                return m_Instance;
            }
        }

        private uint m_SessionId = 0;

        private Dictionary<uint, Action<bool, int>> m_SessionMap = new();

        private MsgSender() {
            //MsgDispatcher.Instance.Register("res_msgresult", OnResMsgresult);
        }

        public void OnInit() {}

        public void SendMessage(string name, byte[] body_data) {
            this.SendMessage(name, body_data, null);
        }

        public void SendMessage(string name, byte[] body_data, Action<bool, int> cb) {
            uint client_session_id = 0;
            if (cb != null) {
                m_SessionId++;
                client_session_id = m_SessionId;
                m_SessionMap[client_session_id] = cb;
            }

            ByteBuffer buff = new();
            buff.WriteNetworkStringUInt16(name);
            buff.WriteNetworkUInt32(client_session_id);
            buff.WriteNetworkUInt16((ushort)body_data.Length);
            buff.WriteBytes(body_data);

            Debug.Log(string.Format("[Network]SendMessage msgname: {0}", name));
            NetManager.Instance.SendMessage(buff.ToBytes());
        }

        private void OnResMsgresult(byte[] bytearray) {
            ByteBuffer buff = new ByteBuffer(bytearray);
            uint client_session_id = buff.ReadNetworkUInt32();
            bool result = buff.ReadBool();
            int errCode = buff.ReadNetworkInt32();
            OnCallback(client_session_id, result, errCode);
        }

        public void OnCallback(uint sessionId, bool result, int errCode) {
            m_SessionMap[sessionId]?.Invoke(result, errCode);
        }
    }
}
