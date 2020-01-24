using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PFAC : Sector
{
    private Dictionary<uint, int> m_KeyLookup;
    private List<ushort> m_Entries;
    private byte[] m_RawData;

    protected override void ReadBody()
    {
        if( HasMultidata )
        {
            base.ReadBody();
        }
        else
        {
            m_KeyLookup = new Dictionary<uint, int>();
            m_Entries = new List<ushort>();

            uint bodySize = SectorSize - BodySize;
            long start = m_Data.Position;

            m_RawData = GetBytes( (int)bodySize );

            /*while( m_Data.Position - start < bodySize )
            {
                uint index = (uint)(m_Data.Position - start);

                ushort value = GetUInt16();

                m_Entries.Add( value );
                m_KeyLookup.Add( index, m_Entries.Count - 1 );
            }*/
        }
    }

    public ushort GetIndex( uint index )
    {
        index *= 2;
        byte[] data = new byte[] { m_RawData[index], m_RawData[index + 1] };
        return System.BitConverter.ToUInt16( data, 0 );
    }

    public int[] GetTriangles( uint index, int count )
    {
        if( m_KeyLookup.ContainsKey( index ) )
        {
            int[] values = new int[count];
            int start = m_KeyLookup[index];

            for( uint i = 0; i < count; i++ )
            {
                values[i] = m_Entries[start + (int)i];
            }

            return values;
        }

        return null;
    }
}
