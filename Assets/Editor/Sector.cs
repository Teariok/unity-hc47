using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Sector
{
    protected string m_Name;
    protected byte[] m_Data;

    public Sector(string name, byte[] data)
    {
        m_Name = name;
        m_Data = data;
    }

    public uint GetDataSize()
    {
        return (uint)m_Data.Length;
    }

    public uint GetUInt(ref uint offset)
    {
        const uint SIZE = 4;

        byte[] extract = new byte[SIZE];
        System.Array.Copy(m_Data, offset, extract, 0, SIZE);

        offset += SIZE;

        return System.BitConverter.ToUInt32(extract, 0);
    }

    public ushort GetUInt16(ref uint offset)
    {
        const uint SIZE = 2;

        byte[] extract = new byte[SIZE];
        System.Array.Copy(m_Data, offset, extract, 0, SIZE);

        offset += SIZE;

        return System.BitConverter.ToUInt16(extract, 0);
    }

    public string GetString(ref uint offset)
    {
        uint end = offset;
        for (end = offset; end < m_Data.Length; end++)
        {
            if (m_Data[end] == 0x00)
            {
                break;
            }
        }

        uint size = end - offset;

        byte[] extract = new byte[size];
        System.Array.Copy(m_Data, offset, extract, 0, size);

        offset += size+1;

        return System.Text.Encoding.UTF8.GetString(extract);
    }
}
