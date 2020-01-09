using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PROT : Sector
{
    public struct PROTEntry
    {
        public uint pheaOffset;
        public uint magicNumber;
        public uint sectionSize;
        public uint nodeCount;
        public PROTEntry[] children;
    }

    private PROTEntry[] m_Entries;

    public PROT( string name, byte[] data ) : base( name, data )
    {
        uint offset = 0;
        
        uint size = GetUInt( ref offset );
        uint rootCount = GetUInt( ref offset );

        m_Entries = new PROTEntry[rootCount];

        for( uint rn = 0; rn < rootCount; rn++ )
        {
            Debug.Log( "Root Node " + rn );
            PROTEntry entry = extractPROT( ref offset );
        }
    }

    private PROTEntry extractPROT( ref uint offset )
    {
        PROTEntry entry = new PROTEntry();

        entry.pheaOffset = GetUInt( ref offset );
        entry.magicNumber = GetUInt( ref offset );

        if( entry.magicNumber != 8 )
        {
            entry.sectionSize = GetUInt( ref offset );
            entry.nodeCount = GetUInt( ref offset );

            entry.children = new PROTEntry[entry.nodeCount];
            for( uint i = 0; i < entry.nodeCount; i++ )
            {
                entry.children[i] = extractPROT( ref offset );
            }
        }

        Debug.Log( "PROT-PHEA Offset: " + entry.pheaOffset );
        Debug.Log( "PROT Magic Num: " + entry.magicNumber );
        Debug.Log( "PROT Section Size: " + entry.sectionSize );
        Debug.Log( "PROT Node Count: " + entry.nodeCount );

        return entry;
    }
}
