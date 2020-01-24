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
            GameObject root = new GameObject( entry.SectorId );
            root.transform.SetParent( parent.transform );

            PHEA.PHEAEntry? pheaData = phea.GetEntry( entry.PheaOffset );
            if( pheaData.HasValue )
            {
                root.name = pnam.GetName( pheaData.Value.PnamIndex );
                root.transform.localPosition = ppos.GetPosition( pheaData.Value.PosIndex );
                
                MeshFilter meshFilter = root.AddComponent<MeshFilter>();
                meshFilter.mesh = GenerateMesh( rootSector, pheaData.Value );
                root.AddComponent<MeshRenderer>();
            }

            if( entry.HasSubsectors )
            {
                CreatePROTTree( rootSector, root, entry.Subsectors );
            }
        }
    }

    private static Mesh GenerateMesh( Sector rootSector, PHEA.PHEAEntry pheaData )
    {
        if( pheaData.TriangleCount == 0 && pheaData.QuadCount == 0 )
        {
            return null;
        }

        PVER pver = (PVER)rootSector.GetSubsector( "PVER" );
        PFAC pfac = (PFAC)rootSector.GetSubsector( "PFAC" );

        Mesh mesh = new Mesh();

        List<Vector3> verts = new List<Vector3>();
        List<int> indices = new List<int>();
        int t = 0;

        for( int i = 0; i < pheaData.QuadCount; i++ )
        {
            for( int j = 0; j < 4; j++ )
            {
                uint offs = (uint)(pheaData.QuadIndex + i * 4 + j);
                ushort offsVal = pfac.GetIndex( offs );
                ushort vertOP = (ushort)(offsVal * 3);
                ushort vertDP = (ushort)(vertOP / 2);

                verts.Add( pver.GetVertex( pheaData.VertexIndex + vertDP ) );
            }

            indices.Add( t );
            indices.Add( t + 2 );
            indices.Add( t + 1 );

            indices.Add( t );
            indices.Add( t + 3 );
            indices.Add( t + 2 );

            t += 4;
        }

        int g = 0;
        for( int i = 0; i < pheaData.TriangleCount; i++ )
        {
            for( int j = 0; j < 3; j++ )
            {
                uint offs = (uint)(pheaData.TriangleIndex + i * 3 + j);
                ushort offsVal = pfac.GetIndex( offs );
                ushort vertOP = (ushort)(offsVal * 3);
                ushort vertDP = (ushort)(vertOP / 2);

                verts.Add( pver.GetVertex( pheaData.VertexIndex + vertDP ) );
            }

            indices.Add( t + g );
            indices.Add( t + g + 2 );
            indices.Add( t + g + 1 );
            g += 3;
        }

        mesh.vertices = verts.ToArray();
        mesh.triangles = indices.ToArray();
        mesh.RecalculateNormals();

        return mesh;
    }
}