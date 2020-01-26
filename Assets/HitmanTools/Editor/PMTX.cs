using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HitmanTools
{
    public class PMTX : Sector
    {
        private Dictionary<uint, int> m_KeyLookup;
        private List<ushort> m_Entries;
        private byte[] m_Body;

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

                m_Body = GetBytes( (int)bodySize );
                /*while( m_Data.Position - start < bodySize )
                {
                    uint index = (uint)(m_Data.Position - start);
                    m_Body = GetBytes( (int)bodySize );

                    ushort value = GetUInt16();

                    m_Entries.Add( value );
                    m_KeyLookup.Add( index, m_Entries.Count - 1 );
                }*/
            }
        }

        public float GetIndex( uint index )
        {
            int val = GetIntIndex( index );
            return val / 1073741824.0f;
        }

        public int GetIntIndex( uint index )
        {
            index *= 4;
            byte[] data = new byte[] { m_Body[index], m_Body[index + 1], m_Body[index + 2], m_Body[index + 3] };

            return System.BitConverter.ToInt32( data, 0 );
        }

        public float GetFloatIndex( uint index )
        {
            index *= 4;
            byte[] data = new byte[] { m_Body[index], m_Body[index + 1], m_Body[index + 2], m_Body[index + 3] };

            return System.BitConverter.ToSingle( data, 0 );
        }
    }
}