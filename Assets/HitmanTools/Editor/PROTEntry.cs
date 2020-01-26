using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HitmanTools
{
    public class PROTEntry : Sector
    {
        public uint PheaOffset { get; protected set; }

        protected override void ReadHeader()
        {
            PheaOffset = GetUInt() & 0xFFFFFF;
            SectorId = PheaOffset.ToString();

            uint sectorInfo = GetUInt();

            SectorSize = (sectorInfo & 0x3FFFFFFF);
            HasSubsectors = (sectorInfo & (1 << 31)) != 0;
            HasMultidata = (sectorInfo & (1 << 30)) != 0;
        }

        protected override void ExtractSubsectors()
        {
            if( HasSubsectors )
            {
                Subsectors = new Sector[SubsectorCount];
                for( uint i = 0; i < SubsectorCount; i++ )
                {
                    PROTEntry subsector = new PROTEntry();
                    subsector.Unpack( m_Data );

                    Subsectors[i] = subsector;
                }
            }
        }
    }
}