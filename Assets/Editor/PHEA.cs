using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PHEA : Sector
{
    public struct PHEAEntry
    {
        public uint PdblIndex;
        public uint PexcIndex;
        public uint PnamIndex;
        public uint MtxIndex; // ( * 4 ? )
        public uint PosIndex;
        public ushort unknown1;
        public ushort Info;
        public uint VertexIndex;
        public uint QuadIndex;
        public uint TextureIndex;
        public uint FtxIndex;
        public uint VertexCount;
        public uint QuadCount;
        public uint FtxCount;
        public uint unknown2;
        public uint flags;
    }

    private Dictionary<uint, PHEAEntry> m_Entries;

    protected override void ReadBody()
    {
        if( HasMultidata )
        {
            base.ReadBody();
        }
        else
        {
            m_Entries = new Dictionary<uint, PHEAEntry>();

            uint bodySize = SectorSize - BodySize;
            long start = m_Data.Position;

            while( m_Data.Position - start < bodySize )
            {
                long lindex = m_Data.Position - start;
                uint index = (uint)(m_Data.Position - start);

                PHEAEntry entry = new PHEAEntry()
                {
                    PdblIndex = GetUInt(),
                    PexcIndex = GetUInt(),
                    PnamIndex = GetUInt(),
                    MtxIndex = GetUInt(),
                    PosIndex = GetUInt(),
                    unknown1 = GetUInt16(),
                    Info = GetUInt16()
                };

                if( (entry.Info & (0x20|0x400)) != 0 )
                {
                    entry.VertexIndex = GetUInt();
                    entry.QuadIndex = GetUInt();
                    entry.TextureIndex = GetUInt();
                    entry.FtxIndex = GetUInt();
                    entry.VertexCount = GetUInt();
                    entry.QuadCount = GetUInt();
                    entry.FtxCount = GetUInt();
                    entry.unknown2 = GetUInt();
                    entry.flags = GetUInt();
                }

                if( (entry.Info & 0x80) != 0 )
                {
                    GetUInt();
                    GetUInt();
                    GetUInt();
                    GetUInt();
                    GetUInt();
                    GetUInt();
                    GetUInt();
                }

                m_Entries.Add( index, entry );
            }
        }
    }

    public PHEAEntry? GetEntry( uint index )
    {
        if( m_Entries.ContainsKey( index ) )
        {
            return m_Entries[index];
        }

        return null;
    }
}