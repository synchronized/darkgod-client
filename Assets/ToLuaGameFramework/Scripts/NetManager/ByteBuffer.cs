using UnityEngine;
using System.Collections;
using System.IO;
using System.Text;
using System;

namespace ToLuaGameFramework
{
    public class ByteBuffer
    {
        #region 变量

        MemoryStream m_Stream = null;
        BinaryWriter m_Writer = null;
        BinaryReader m_Reader = null;

        #endregion

        #region 函数

        public ByteBuffer()
        {
            m_Stream = new MemoryStream();
            m_Writer = new BinaryWriter(m_Stream);
        }

        public ByteBuffer(byte[] data)
        {
            if (data != null)
            {
                m_Stream = new MemoryStream(data);
                m_Reader = new BinaryReader(m_Stream);
            }
            else
            {
                m_Stream = new MemoryStream();
                m_Writer = new BinaryWriter(m_Stream);
            }
        }

        public void Close()
        {
            if (m_Writer != null) m_Writer.Close();
            if (m_Reader != null) m_Reader.Close();

            m_Stream.Close();
            m_Writer = null;
            m_Reader = null;
            m_Stream = null;
        }

        public void WriteBool(Boolean v) { m_Writer.Write(v); }
        public void WriteByte(Byte v) { m_Writer.Write(v); }
        public void WriteBytes(byte[] v) { m_Writer.Write(v); }
        public bool ReadBool() { return m_Reader.ReadBoolean(); }
        public byte ReadByte() { return m_Reader.ReadByte(); }
        public byte[] ReadBytes(int len) { return m_Reader.ReadBytes(len); }

        #region 主机字节序
        public void WriteHostInt16(Int16 v) { m_Writer.Write(v); }
        public void WriteHostInt32(Int32 v) { m_Writer.Write(v); }
        public void WriteHostInt64(Int64 v) { m_Writer.Write(v); }
        public void WriteHostUInt16(UInt16 v) { m_Writer.Write(v); }
        public void WriteHostUInt32(UInt32 v) { m_Writer.Write(v); }
        public void WriteHostUInt64(UInt64 v) { m_Writer.Write(v); }
        public void WriteHostFloat32(Single v) { m_Writer.Write(v); }
        public void WriteHostDouble64(Double v) { m_Writer.Write(v); }
        public void WriteHostStringUInt16(string v)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(v);
            m_Writer.Write((ushort)bytes.Length);
            m_Writer.Write(bytes);
        }

        public Int16 ReadHostInt16() { return m_Reader.ReadInt16(); }
        public Int32 ReadHostInt32() { return m_Reader.ReadInt32(); }
        public Int64 ReadHostInt64() { return m_Reader.ReadInt64(); }
        public UInt16 ReadHostUInt16() { return m_Reader.ReadUInt16(); }
        public UInt32 ReadHostUInt32() { return m_Reader.ReadUInt32(); }
        public UInt64 ReadHostUInt64() { return m_Reader.ReadUInt64(); }
        public float ReadHostFloat() { return m_Reader.ReadSingle(); }
        public double ReadHostDouble() { return m_Reader.ReadDouble(); }
        public string ReadHostStringUInt16()
        {
            ushort len = ReadHostUInt16();
            byte[] buffer = new byte[len];
            buffer = m_Reader.ReadBytes(len);
            return Encoding.UTF8.GetString(buffer);
        }
        #endregion

        #region 网络字节序
        public void WriteNetworkInt16(Int16 v) { m_Writer.Write(Converter.HostToNetworkOrder(v)); }
        public void WriteNetworkInt32(Int32 v) { m_Writer.Write(Converter.HostToNetworkOrder(v)); }
        public void WriteNetworkInt64(Int64 v) { m_Writer.Write(Converter.HostToNetworkOrder(v)); }
        public void WriteNetworkUInt16(UInt16 v) { m_Writer.Write(Converter.HostToNetworkOrder(v)); }
        public void WriteNetworkUInt32(UInt32 v) { m_Writer.Write(Converter.HostToNetworkOrder(v)); }
        public void WriteNetworkUInt64(UInt64 v) { m_Writer.Write(Converter.HostToNetworkOrder(v)); }
        public void WriteNetworkFloat32(Single v) { m_Writer.Write(Converter.HostToNetworkOrder(v)); }
        public void WriteNetworkDouble64(Double v) { m_Writer.Write(Converter.HostToNetworkOrder(v)); }
        public void WriteNetworkStringUInt16(string v)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(v);
            m_Writer.Write(Converter.HostToNetworkOrder((ushort)bytes.Length));
            m_Writer.Write(bytes);
        }

        public Int16 ReadNetworkInt16() { return Converter.HostToNetworkOrder(m_Reader.ReadInt16()); }
        public Int32 ReadNetworkInt32() { return Converter.HostToNetworkOrder(m_Reader.ReadInt32()); }
        public Int64 ReadNetworkInt64() { return Converter.HostToNetworkOrder(m_Reader.ReadInt64()); }
        public UInt16 ReadNetworkUInt16() { return Converter.HostToNetworkOrder(m_Reader.ReadUInt16()); }
        public UInt32 ReadNetworkUInt32() { return Converter.HostToNetworkOrder(m_Reader.ReadUInt32()); }
        public UInt64 ReadNetworkUInt64() { return Converter.HostToNetworkOrder(m_Reader.ReadUInt64()); }
        public float ReadNetworkFloat() { return Converter.HostToNetworkOrder(m_Reader.ReadSingle()); }
        public double ReadNetworkDouble() { return Converter.HostToNetworkOrder(m_Reader.ReadDouble()); }
        public string ReadNetworkStringUInt16()
        {
            ushort len = ReadNetworkUInt16();
            byte[] buffer = new byte[len];
            buffer = m_Reader.ReadBytes(len);
            return Encoding.UTF8.GetString(buffer);
        }
        #endregion

        #region 小端字节序
        public void WriteLittleInt16(Int16 v) { m_Writer.Write(Converter.GetLittleEndian(v)); }
        public void WriteLittleInt32(Int32 v) { m_Writer.Write(Converter.GetLittleEndian(v)); }
        public void WriteLittleInt64(Int64 v) { m_Writer.Write(Converter.GetLittleEndian(v)); }
        public void WriteLittleUInt16(UInt16 v) { m_Writer.Write(Converter.GetLittleEndian(v)); }
        public void WriteLittleUInt32(UInt32 v) { m_Writer.Write(Converter.GetLittleEndian(v)); }
        public void WriteLittleUInt64(UInt64 v) { m_Writer.Write(Converter.GetLittleEndian(v)); }
        public void WriteLittleFloat32(Single v) { m_Writer.Write(Converter.GetLittleEndian(v)); }
        public void WriteLittleDouble64(Double v) { m_Writer.Write(Converter.GetLittleEndian(v)); }
        public void WriteLittleStringUInt16(string v)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(v);
            m_Writer.Write(Converter.GetLittleEndian((ushort)bytes.Length));
            m_Writer.Write(bytes);
        }

        public Int16 ReadLittleInt16() { return Converter.GetLittleEndian(m_Reader.ReadInt16()); }
        public Int32 ReadLittleInt32() { return Converter.GetLittleEndian(m_Reader.ReadInt32()); }
        public Int64 ReadLittleInt64() { return Converter.GetLittleEndian(m_Reader.ReadInt64()); }
        public UInt16 ReadLittleUInt16() { return Converter.GetLittleEndian(m_Reader.ReadUInt16()); }
        public UInt32 ReadLittleUInt32() { return Converter.GetLittleEndian(m_Reader.ReadUInt32()); }
        public UInt64 ReadLittleUInt64() { return Converter.GetLittleEndian(m_Reader.ReadUInt64()); }
        public float ReadLittleFloat() { return Converter.GetLittleEndian(m_Reader.ReadSingle()); }
        public double ReadLittleDouble() { return Converter.GetLittleEndian(m_Reader.ReadDouble()); }
        public string ReadLittleStringUInt16()
        {
            ushort len = ReadLittleUInt16();
            byte[] buffer = new byte[len];
            buffer = m_Reader.ReadBytes(len);
            return Encoding.UTF8.GetString(buffer);
        }
        #endregion

        #region 大端字节序
        public void WriteBigInt16(Int16 v) { m_Writer.Write(Converter.GetBigEndian(v)); }
        public void WriteBigInt32(Int32 v) { m_Writer.Write(Converter.GetBigEndian(v)); }
        public void WriteBigInt64(Int64 v) { m_Writer.Write(Converter.GetBigEndian(v)); }
        public void WriteBigUInt16(UInt16 v) { m_Writer.Write(Converter.GetBigEndian(v)); }
        public void WriteBigUInt32(UInt32 v) { m_Writer.Write(Converter.GetBigEndian(v)); }
        public void WriteBigUInt64(UInt64 v) { m_Writer.Write(Converter.GetBigEndian(v)); }
        public void WriteBigFloat32(Single v) { m_Writer.Write(Converter.GetBigEndian(v)); }
        public void WriteBigDouble64(Double v) { m_Writer.Write(Converter.GetBigEndian(v)); }
        public void WriteBigStringUInt16(string v)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(v);
            m_Writer.Write(Converter.GetBigEndian((ushort)bytes.Length));
            m_Writer.Write(bytes);
        }

        public Int16 ReadBigInt16() { return Converter.GetBigEndian(m_Reader.ReadInt16()); }
        public Int32 ReadBigInt32() { return Converter.GetBigEndian(m_Reader.ReadInt32()); }
        public Int64 ReadBigInt64() { return Converter.GetBigEndian(m_Reader.ReadInt64()); }
        public UInt16 ReadBigUInt16() { return Converter.GetBigEndian(m_Reader.ReadUInt16()); }
        public UInt32 ReadBigUInt32() { return Converter.GetBigEndian(m_Reader.ReadUInt32()); }
        public UInt64 ReadBigUInt64() { return Converter.GetBigEndian(m_Reader.ReadUInt64()); }
        public float ReadBigFloat() { return Converter.GetBigEndian(m_Reader.ReadSingle()); }
        public double ReadBigDouble() { return Converter.GetBigEndian(m_Reader.ReadDouble()); }
        public string ReadBigStringUInt16()
        {
            ushort len = ReadBigUInt16();
            byte[] buffer = new byte[len];
            buffer = m_Reader.ReadBytes(len);
            return Encoding.UTF8.GetString(buffer);
        }
        #endregion

        public byte[] ToBytes()
        {
            m_Writer.Flush();
            return m_Stream.ToArray();
        }

        public void Flush()
        {
            m_Writer.Flush();
        }

        #endregion
    }
}