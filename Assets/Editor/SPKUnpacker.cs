using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

public class SPKUnpacker
{
    public Sector m_RootSector;

    public void Unpack( string filePath )
    {
        FileStream fstream = File.Open( filePath, FileMode.Open );

        try
        {
            m_RootSector = new Sector();
            m_RootSector.Unpack( fstream );
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

    public void BuildScene()
    {
        GameObject root = new GameObject( "PROT" );
        PROT prot = (PROT)m_RootSector.GetSubsector( "PROT" );
        CreatePROTTree( root, prot.Subsectors );
    }

    private void CreatePROTTree( GameObject parent, Sector[] collection )
    {
        PHEA phea = (PHEA)m_RootSector.GetSubsector( "PHEA" );
        PNAM pnam = (PNAM)m_RootSector.GetSubsector( "PNAM" );
        PPOS ppos = (PPOS)m_RootSector.GetSubsector( "PPOS" );

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
                Mesh mesh = GenerateMesh( pheaData.Value );
                meshFilter.mesh = mesh;
                root.AddComponent<MeshRenderer>();
            }

            if( entry.HasSubsectors )
            {
                CreatePROTTree( root, entry.Subsectors );
            }
        }
    }

    private Mesh GenerateMesh( PHEA.PHEAEntry pheaData )
    {
        if( pheaData.TriangleCount == 0 && pheaData.QuadCount == 0 )
        {
            return null;
        }

        PVER pver = (PVER)m_RootSector.GetSubsector( "PVER" );
        PFAC pfac = (PFAC)m_RootSector.GetSubsector( "PFAC" );

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