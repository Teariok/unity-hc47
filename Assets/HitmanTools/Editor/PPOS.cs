using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HitmanTools
{
    public class PPOS : Sector
    {
        private Dictionary<uint, Vector3> m_Entries;

        protected override void ReadBody()
        {
            if( HasMultidata )
            {
                base.ReadBody();
            }
            else
            {
                m_Entries = new Dictionary<uint, Vector3>();

                uint bodySize = SectorSize - BodySize;
                long start = m_Data.Position;

                while( m_Data.Position - start < bodySize )
                {
                    uint index = (uint)(m_Data.Position - start);

                    float x = GetFloat();
                    float y = GetFloat();
                    float z = GetFloat();

                    m_Entries.Add( index, new Vector3( x, y, z ) );
                }
            }
        }

        public Vector3 GetPosition( uint index )
        {
            if( m_Entries.ContainsKey( index ) )
            {
                return m_Entries[index];
            }

            return Vector3.zero;
        }
    }
}