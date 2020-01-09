using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.IO;

public class SPKProcessor
{
    private class Sector
    {
        private string m_Name;
        private byte[] m_Data;

        public Sector(string name, byte[] data)
        {
            m_Name = name;
            m_Data = data;

            // Debug.Log(name + " is size " + data.Length);
        }

        public uint GetDataSize()
        {
            return (uint)m_Data.Length;
        }

        public uint GetUInt(uint offset)
        {
            byte[] extract = new byte[4];
            System.Array.Copy(m_Data, offset, extract, 0, 4);
            return System.BitConverter.ToUInt32(extract, 0);
        }

        public ushort GetUInt16(uint offset)
        {
            byte[] extract = new byte[2];
            System.Array.Copy(m_Data, offset, extract, 0, 2);
            return System.BitConverter.ToUInt16(extract, 0);
        }

        public string GetString(uint offset)
        {
            Debug.Log("Get String at " + offset);
            uint end = offset;
            for(end = offset; end < m_Data.Length; end++)
            {
                if(m_Data[end] == 0x00)
                {
                    break;
                }
            }

            byte[] extract = new byte[end - offset];
            System.Array.Copy(m_Data, offset, extract, 0, end - offset);
            return System.Text.Encoding.UTF8.GetString(extract);
        }
    }
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

            Sector sector = new Sector(sectorId, sectorData);
            m_Sectors.Add(sectorId, sector);
        }

        Debug.Log(fstream.Length - fstream.Position);

        fstream.Close();




        /*uint offset = 0;
        Sector prot = m_Sectors["PROT"];

        uint size = prot.GetUInt(offset);
        offset += 4;
        Debug.Log("PROT Size: " + size);

        uint rootCount = prot.GetUInt(offset);
        offset += 4;
        Debug.Log("PROT Root Count: " + rootCount);

        for (uint rn = 0; rn < rootCount; rn++)
        {
            Debug.Log("Root Node " + rn);
            ExtractProt(ref offset);
        }*/


        
        uint offset = 0;
        Sector phea = m_Sectors["PHEA"];

        while (offset < phea.GetDataSize())
        {
            uint unknown1 = phea.GetUInt(offset);
            offset += 4;

            uint exc = phea.GetUInt(offset);
            offset += 4;

            uint nameOffset = phea.GetUInt(offset);
            offset += 4;
            Debug.Log("Name Offset: " + nameOffset);

            uint unknown2 = phea.GetUInt(offset);
            offset += 4;

            uint unknown3 = phea.GetUInt(offset);
            offset += 4;

            ushort unknown4 = phea.GetUInt16(offset);
            offset += 2;

            ushort size = phea.GetUInt16(offset);
            offset += 2;
            Debug.Log("Size: " + size);

            if( size == 32 )
            {
                uint vertOffset = phea.GetUInt(offset);
                offset += 4;

                uint quatOffset = phea.GetUInt(offset);
                offset += 4;

                uint textureOffset = phea.GetUInt(offset);
                offset += 4;

                uint faceOffset = phea.GetUInt(offset);
                offset += 4;

                uint vertCount = phea.GetUInt(offset);
                offset += 4;

                uint quatCount = phea.GetUInt(offset);
                offset += 4;

                uint textureCount = phea.GetUInt(offset);
                offset += 4;

                uint unknown5 = phea.GetUInt(offset);
                offset += 4;

                uint flags = phea.GetUInt(offset);
                offset += 4;
            }
        }
    }

    private static void ExtractProt(ref uint offset, GameObject parent = null)
    {
        Sector prot = m_Sectors["PROT"];
        Sector phea = m_Sectors["PHEA"];

        uint pheaOffset = prot.GetUInt(offset);
        offset += 4;
        Debug.Log("PROT-PHEA Offset: " + pheaOffset);

        uint pnamOffset = phea.GetUInt((pheaOffset + 8) & 0x00FFFFFF);
        Debug.Log("PHEA-PHNAM Offset: " + pnamOffset);

        Sector pnam = m_Sectors["PNAM"];
        string name = pnam.GetString(pnamOffset);
        Debug.Log("NAME: " + name);

        uint magicNumber = prot.GetUInt(offset);
        offset += 4;
        Debug.Log("PROT Magic Num: " + magicNumber);

        GameObject node = new GameObject(name);
        if( parent != null )
        {
            node.transform.SetParent(parent.transform);
        }

        if (magicNumber != 8)
        {
            uint sectionSize = prot.GetUInt(offset);
            offset += 4;
            Debug.Log("PROT Section Size: " + sectionSize);

            uint nodeCount = prot.GetUInt(offset);
            offset += 4;
            Debug.Log("PROT Node Count: " + nodeCount);

            for (uint i = 0; i < nodeCount; i++)
            {
                ExtractProt(ref offset, node);
            }
        }
    }
}