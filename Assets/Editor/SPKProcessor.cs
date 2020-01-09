using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.IO;

public class SPKProcessor
{
    private static Dictionary<string, Sector> m_Sectors;

    [MenuItem("Teario/Open SPK")]
    private static void ProcessSPK()
    {
        string filePath = EditorUtility.OpenFilePanel("Open SPK File", null, "spk");

        if(string.IsNullOrEmpty(filePath))
        {
            return;
        }

        m_Sectors = new Dictionary<string, Sector>();

        FileStream fstream = File.Open(filePath, FileMode.Open);

        byte[] fourCCRaw = new byte[4];
        fstream.Read(fourCCRaw, 0, 4);

        byte[] magicNumberRaw = new byte[4];
        fstream.Read(magicNumberRaw, 0, 4);

        byte[] dataSizeRaw = new byte[4];
        fstream.Read(dataSizeRaw, 0, 4);

        byte[] sectorCountRaw = new byte[4];
        fstream.Read(sectorCountRaw, 0, 4);

        int sectorCount = (int)System.BitConverter.ToUInt32(sectorCountRaw, 0);
        for(int i = 0; i < sectorCount; i++)
        {
            byte[] sectorIdRaw = new byte[4];
            fstream.Read(sectorIdRaw, 0, 4);
            string sectorId = System.Text.Encoding.UTF8.GetString(sectorIdRaw);

            byte[] sectorSizeRaw = new byte[4];
            fstream.Read(sectorSizeRaw, 0, 4);

            uint sectorSize = System.BitConverter.ToUInt32(sectorSizeRaw, 0);
            Debug.Log(sectorId + " should be size " + sectorSize);
            sectorSize = (sectorSize & 0x00FFFFFF)-8;
            Debug.Log(sectorId + " masked size " + sectorSize);

            byte[] sectorData = new byte[sectorSize];
            fstream.Read(sectorData, 0, (int)sectorSize);

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

            if( sector != null )
            {
                m_Sectors.Add( sectorId, sector );
            }
        }

        Debug.Log(fstream.Length - fstream.Position);

        fstream.Close();
    }
}