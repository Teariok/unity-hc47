using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PVER : Sector
{
    private Dictionary<uint, uint> m_KeyLookup;
    private List<float> m_Entries;
    private byte[] m_RawData;
    private List<Vector3> m_Verts;

    protected override void ReadBody()
    {
        if( HasMultidata )
        {
            base.ReadBody();
        }
        else
        {
            m_KeyLookup = new Dictionary<uint, uint>();
            m_Entries = new List<float>();

            uint bodySize = SectorSize - BodySize;
            long start = m_Data.Position;

            //m_RawData = GetBytes( (int)bodySize );
            m_Verts = new List<Vector3>();
            while( m_Data.Position - start < bodySize )
            {
                m_RawData = GetBytes( (int)bodySize );
                //m_Verts.Add( new Vector3( GetFloat(), GetFloat(), GetFloat() ) );
                //uint index = (uint)(m_Data.Position - start);

                //m_Entries.Add( GetFloat() );
                //m_KeyLookup.Add( index, (uint)m_Entries.Count - 1 );
            }
        }
    }

    public float GetFloat( int index )
    {
        index *= 4;
        byte[] data = new byte[] { m_RawData[index], m_RawData[index + 1], m_RawData[index + 2], m_RawData[index + 3] };
        return System.BitConverter.ToSingle( data, 0 );
    }
    
    public Vector3[] GetVertices( uint index, int count )
    {

        //if( m_KeyLookup.ContainsKey( index ) )
        {
            //int start = (int)m_KeyLookup[index];
            int start = 0;
            Vector3[] values = new Vector3[count];

            for( int i = 0; i < count; i++ )
            {
                values[i] = new Vector3( m_Entries[start], m_Entries[start+1], m_Entries[start+2]);
                start += 6;
            }

            return values;
        }

        //return null;
    }
}
