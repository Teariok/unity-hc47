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

            GameObject root = new GameObject( "PROT" );
            PROT prot = (PROT)sector.GetSubsector( "PROT" );
            CreatePROTTree( sector, root, prot.Subsectors );
        }
        catch( System.Exception ex )
        {
            throw new System.Exception( "Unpack Failed", ex );
        }
        finally
        {
            fstream.Close();
        }
    }

    private static void CreatePROTTree( Sector rootSector, GameObject parent, Sector[] collection )
    {
        PHEA phea = (PHEA)rootSector.GetSubsector( "PHEA" );
        PNAM pnam = (PNAM)rootSector.GetSubsector( "PNAM" );
        PPOS ppos = (PPOS)rootSector.GetSubsector( "PPOS" );

        foreach( PROTEntry entry in collection )
        {
            GameObject root = GameObject.CreatePrimitive( PrimitiveType.Sphere ); //new GameObject( entry.SectorId );
            root.name = entry.SectorId;
            
            PHEA.PHEAEntry? pheaData = phea.GetEntry( entry.PheaOffset );
            if( pheaData.HasValue )
            {
                root.name = pnam.GetName( pheaData.Value.PnamIndex );
                root.transform.position = ppos.GetPosition( pheaData.Value.PosIndex );
            }

            
            root.transform.SetParent( parent.transform );

            if( entry.HasSubsectors )
            {
                CreatePROTTree( rootSector, root, entry.Subsectors );
            }
        }
    }
}