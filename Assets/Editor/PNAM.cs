using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PNAM : Sector
{
    private Dictionary<uint, string> m_Entries;

    /* override void Unpack( ref uint offset, byte[] data )
    {
        base.Unpack( ref offset, data );

        m_Entries = new Dictionary<uint, string>();

        while( offset < m_BodySize )
        {
            uint start = offset;
            string entry = GetString( ref offset );
            
            m_Entries.Add( start, entry );

            Debug.Log( "NAME: " + start + " - " + entry );
        }
    }*/
}
