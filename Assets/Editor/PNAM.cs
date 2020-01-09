using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PNAM : Sector
{
    private Dictionary<uint, string> m_Entries;

    public PNAM( string name, byte[] data ) : base( name, data )
    {
        uint offset = 0;
        m_Entries = new Dictionary<uint, string>();

        while( offset < GetDataSize() )
        {
            uint start = offset;
            string entry = GetString( ref offset );
            
            m_Entries.Add( start, entry );

            Debug.Log( "NAME: " + start + " - " + entry );
        }
    }
}
