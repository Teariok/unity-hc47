using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PHEA : Sector
{
    public struct PHEAEntry
    {
        public uint unknown1;
        public uint exc;
        public uint pnamIndex;
        public uint unknown2;
        public uint unknown3;
        public ushort unknown4;
        public ushort size;
        public uint vertexOffset;
        public uint quaternionOffset;
        public uint textureOffset;
        public uint faceOffset;
        public uint vertexCount;
        public uint quaternionCount;
        public uint textureCount;
        public uint unknown5;
        public uint flags;
    }

    private Dictionary<uint, PHEAEntry> m_Entries;

    public PHEA( string name, byte[] data ) : base( name, data )
    {
        uint offset = 0;
        m_Entries = new Dictionary<uint, PHEAEntry>();

        while( offset < GetDataSize() )
        {
            uint index = offset;
            PHEAEntry entry = new PHEAEntry()
            {
                unknown1 = GetUInt(ref offset),
                exc = GetUInt(ref offset),
                pnamIndex = GetUInt(ref offset),
                unknown2 = GetUInt(ref offset),
                unknown3 = GetUInt(ref offset),
                unknown4 = GetUInt16(ref offset),
                size = GetUInt16(ref offset)
            };

            Debug.Log("Name Offset: " + entry.pnamIndex);
            Debug.Log("Size: " + entry.size);

            if( entry.size == 32 )
            {
                entry.vertexOffset = GetUInt(ref offset);
                entry.quaternionOffset = GetUInt(ref offset);
                entry.textureOffset = GetUInt(ref offset);
                entry.faceOffset = GetUInt(ref offset);
                entry.vertexCount = GetUInt(ref offset);
                entry.quaternionCount = GetUInt(ref offset);
                entry.textureCount = GetUInt(ref offset);
                entry.unknown5 = GetUInt(ref offset);
                entry.flags = GetUInt(ref offset);
            }

            m_Entries.Add(index, entry);
        }
    }
}
