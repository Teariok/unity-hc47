using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PVER : Sector
{
    private Dictionary<uint, uint> m_KeyLookup;
    private List<Vector3> m_Entries;

    protected override void ReadBody()
    {
        if( HasMultidata )
        {
            base.ReadBody();
        }
        else
        {
            m_KeyLookup = new Dictionary<uint, uint>();
            m_Entries = new List<Vector3>();

            uint bodySize = SectorSize - BodySize;
            long start = m_Data.Position;

            while( m_Data.Position - start < bodySize )
            {
                uint index = (uint)(m_Data.Position - start);

                float x = GetFloat();
                float y = GetFloat();
                float z = GetFloat();

                m_Entries.Add( new Vector3(x,y,z) );
                m_KeyLookup.Add( index, (uint)m_Entries.Count - 1 );
                Debug.Log( new Vector3( x, y, z ) );
            }
        }
    }

    public Vector3[] GetVertices( uint index, int count )
    {
        if( m_KeyLookup.ContainsKey( index ) )
        {
            int start = (int)m_KeyLookup[index];
            Vector3[] values = new Vector3[count];
            m_Entries.CopyTo( start, values, 0, count );

            return values;
        }

        return null;
    }
}
