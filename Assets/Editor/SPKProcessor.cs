using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.IO;
using System.Reflection;

public class SPKProcessor
{
    [MenuItem("Teario/Open SPK")]
    private static void ProcessSPK()
    {
        /*System.Type type = System.Type.GetType( "PSCR" );
        ConstructorInfo method = type.GetConstructor(new System.Type[] { } );
        PSCR instance = method.Invoke( null ) as PSCR;*/

        string filePath = EditorUtility.OpenFilePanel("Open SPK File", null, "spk");

        if(string.IsNullOrEmpty(filePath))
        {
            return;
        }

        FileStream fstream = File.Open( filePath, FileMode.Open );

        try
        {
            Sector sector = new Sector();
            sector.Unpack( fstream );
        }
        catch( System.Exception ex )
        {
            throw new System.Exception( "Unpack Failed", ex );
        }
        finally
        {
            fstream.Close();
        }

        /*FileStream fstream = File.Open(filePath, FileMode.Open);
        byte[] buff = new byte[4];

        fstream.Read( buff, 0, 4);
        string sectorId = System.Text.Encoding.UTF8.GetString( buff );

        fstream.Read( buff, 0, 4);
        uint sectorInfo = System.BitConverter.ToUInt32( buff, 0 );

        uint sectorSize = (sectorInfo & 0x3FFFFFFF) - 8;
        bool hasSubdata = (sectorInfo & (1 << 31)) != 0;
        bool hasMultidata = (sectorInfo & (1 << 30)) != 0;

        fstream.Read(buff, 0, 4);
        uint dataSize = System.BitConverter.ToUInt32( buff, 0 );

        fstream.Read(buff, 0, 4);
        uint subsectorCount = System.BitConverter.ToUInt32( buff, 0 );

        Debug.Log( "Sector Id: " + sectorId );
        Debug.Log( "Sector Info: " + sectorInfo );
        Debug.Log( " - Size: " + sectorSize );
        Debug.Log( " - Has Subsections: " + hasSubdata );
        Debug.Log( " - Has Multidata: " + hasMultidata );
        Debug.Log( "Data Size: " + dataSize );
        Debug.Log( "Subsector Count: " + subsectorCount );

        uint count = subsectorCount;
        for(int i = 0; i < count; i++)
        {
            fstream.Read( buff, 0, 4 );
            sectorId = System.Text.Encoding.UTF8.GetString( buff );

            fstream.Read( buff, 0, 4 );
            sectorInfo = System.BitConverter.ToUInt32( buff, 0 );

            sectorSize = (sectorInfo & 0x3FFFFFFF) - 8;
            hasSubdata = (sectorInfo & (1 << 31)) != 0;
            hasMultidata = (sectorInfo & (1 << 30)) != 0;

            byte[] sectorData = new byte[sectorSize];
            fstream.Read(sectorData, 0, (int)sectorSize);

            buff[0] = sectorData[0];
            buff[1] = sectorData[1];
            buff[2] = sectorData[2];
            buff[3] = sectorData[3];
            dataSize = System.BitConverter.ToUInt32( buff, 0 );

            subsectorCount = 0;
            if( hasSubdata )
            {
                buff[0] = sectorData[4];
                buff[1] = sectorData[5];
                buff[2] = sectorData[6];
                buff[3] = sectorData[7];
                subsectorCount = System.BitConverter.ToUInt32( buff, 0 );
            }

            Debug.Log( "Sector Id: " + sectorId );
            Debug.Log( "Sector Info: " + sectorInfo );
            Debug.Log( " - Size: " + sectorSize );
            Debug.Log( " - Has Subsections: " + hasSubdata );
            Debug.Log( " - Has Multidata: " + hasMultidata );
            Debug.Log( "Data Size: " + dataSize );
            Debug.Log( "Subsector Count: " + subsectorCount );

            continue;

            Sector sector = null;
            if( sectorId == "PROT" )
            {
                sector = new PROT( sectorId, sectorData );
            }
            else if( sectorId == "PHEA" )
            {
                sector = new PHEA( sectorId, sectorData );
            }
            else if( sectorId == "PNAM" )
            {
                sector = new PNAM( sectorId, sectorData );
            }
            else if( sectorId == "PSCR" )
            {
                sector = new PSCR( sectorId, sectorData );
            }

            if( sector != null )
            {
                m_Sectors.Add( sectorId, sector );
            }
        }

        Debug.Log(fstream.Length - fstream.Position);

        fstream.Close();*/
    }
}