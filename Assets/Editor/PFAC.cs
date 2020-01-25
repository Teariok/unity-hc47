using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PFAC : Sector
{
    private Dictionary<uint, int> m_KeyLookup;
    private List<ushort> m_Entries;

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

            while( m_Data.Position - start < bodySize )
            {
                uint index = (uint)(m_Data.Position - start);

                ushort value = GetUInt16();

                m_Entries.Add( value );
                m_KeyLookup.Add( index, m_Entries.Count - 1 );
            }
        }
    }

    public ushort GetIndex( uint index )
    {
        index *= 2;
        if( m_KeyLookup.ContainsKey( index ) )
        {
            int i = m_KeyLookup[index];
            return m_Entries[i];
        }

        throw new System.Exception( string.Format( "Not Found: Face index at position {0} ({1})", index / 4, index ) );
    }
}
