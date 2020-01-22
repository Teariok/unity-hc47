using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PROT : Sector
{
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
