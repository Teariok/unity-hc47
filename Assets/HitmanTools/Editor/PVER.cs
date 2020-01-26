using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HitmanTools
{
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
                    m_Entries.Add( new Vector3( GetFloat(), GetFloat(), GetFloat() ) );
                    m_KeyLookup.Add( index, (uint)m_Entries.Count - 1 );
                }
            }
        }

        public Vector3 GetVertex( uint index )
        {
            index *= 4;
            if( m_KeyLookup.ContainsKey( index ) )
            {
                uint i = m_KeyLookup[index];
                return m_Entries[(int)i];
            }

            throw new System.Exception( string.Format( "Not Found: Vertex at position {0} ({1})", index / 4, index ) );
        }
    }
}