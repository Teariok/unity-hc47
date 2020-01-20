using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PNAM : Sector
{
    private Dictionary<uint, string> m_Entries;

    protected override void ReadBody()
    {
        if( m_HasMultidata )
        {
            base.ReadBody();
        }
        else
        {
            m_Entries = new Dictionary<uint, string>();

            uint bodySize = m_SectorSize - m_BodySize;
            uint index = 0;

            while( index < bodySize )
            {
                string entry = GetString();
                m_Entries.Add( index, entry );
                Debug.Log( index + " - " + entry );

                index += (uint)entry.Length + 1;
            }
        }
    }

    public string GetName( uint index )
    {
        if( m_Entries.ContainsKey( index ) )
        {
            return m_Entries[index];
        }

        return string.Empty;
    }
}
