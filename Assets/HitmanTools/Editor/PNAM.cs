using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HitmanTools
{
    public class PNAM : Sector
    {
        private Dictionary<uint, string> m_Entries;

        protected override void ReadBody()
        {
            if( HasMultidata )
            {
                base.ReadBody();
            }
            else
            {
                m_Entries = new Dictionary<uint, string>();

                uint bodySize = SectorSize - BodySize;
                uint index = 0;

                while( index < bodySize )
                {
                    string entry = GetString();
                    m_Entries.Add( index, entry );

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
}