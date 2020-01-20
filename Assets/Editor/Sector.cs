using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;

public class Sector
{
    protected FileStream m_Data;

    public string SectorId { get; protected set; }
    public uint SectorSize { get; protected set; }
    public bool HasSubsectors { get; protected set; }
    public bool HasMultidata { get; protected set; }
    public uint BodySize { get; protected set; }
    public uint SubsectorCount { get; protected set; }
    public uint MultidataCount { get; protected set; }
    public uint[] MultidataSizes { get; protected set; }
    public Sector[] Subsectors { get; protected set; }

    public virtual void Unpack( FileStream data )
    {
        m_Data = data;

        ReadHeader();
        ReadMeta();
        ExtractSubsectors();
        ReadBody();
    }

    protected virtual void ReadHeader()
    {
        SectorId = GetString( 4 );
        uint sectorInfo = GetUInt();

        SectorSize = (sectorInfo & 0x3FFFFFFF);
        HasSubsectors = (sectorInfo & (1 << 31)) != 0;
        HasMultidata = (sectorInfo & (1 << 30)) != 0;
    }

    protected void ReadMeta()
    {
        BodySize = HasSubsectors || HasMultidata ? GetUInt() : 8;
        SubsectorCount = HasSubsectors ? GetUInt() : 8;
        MultidataCount = HasMultidata ? GetUInt() : 0;

        if( HasMultidata )
        {
            MultidataSizes = new uint[MultidataCount];
            for( uint i = 0; i < MultidataCount; i++ )
            {
                MultidataSizes[i] = GetUInt();
            }
        }
    }

    protected  virtual void ExtractSubsectors()
    {
        if( HasSubsectors )
        {
            Subsectors = new Sector[SubsectorCount];
            for( uint i = 0; i < SubsectorCount; i++ )
            {
                // Need to peek at the next id in order to construct
                // the correct type of container for it
                string nextId = GetString( 4 );
                m_Data.Position = m_Data.Position - 4;

                Sector subsector = GetSectorContainer( nextId );
                subsector.Unpack( m_Data );
            }
        }
    }

    protected virtual void ReadBody()
    {
        if( HasMultidata )
        {
            for( uint i = 0; i < MultidataCount; i++ )
            {
                GetBytes( (int)MultidataSizes[i] );
            }
        }
        else
        {
            uint bodySize = SectorSize - BodySize;
            GetBytes( (int)bodySize );
        }
    }

    public Sector GetSubsector( string sectorId )
    {
        if( HasSubsectors )
        {
            foreach( Sector subsector in Subsectors )
            {
                Sector foundSector = subsector.SectorId.Equals( sectorId ) ? subsector : subsector.GetSubsector( SectorId );
                if( foundSector != null )
                {
                    return foundSector;
                }
            }
        }

        return null;
    }

    protected Sector GetSectorContainer( string sectorId )
    {
        System.Type type = System.Type.GetType( sectorId );
        
        if( type != null )
        {
            ConstructorInfo method = type.GetConstructor( new System.Type[] { } );
            return method.Invoke( null ) as Sector;
        }

        return new Sector();
    }

    public uint GetUInt()
    {
        const int SIZE = 4;
        return System.BitConverter.ToUInt32( GetBytes( SIZE ), 0);
    }

    public ushort GetUInt16()
    {
        const int SIZE = 2;
        return System.BitConverter.ToUInt16( GetBytes( SIZE ), 0);
    }

    public string GetString( uint size = 0 )
    {
        if( size > 0 )
        {
            return System.Text.Encoding.UTF8.GetString( GetBytes( 4 ) );
        }

        byte[] buffer = new byte[40];
        int len = 0;
        byte data;
        while((data = (byte)m_Data.ReadByte()) != 0x00)
        {
            if( len >= buffer.Length )
            {
                byte[] tmp = new byte[buffer.Length + 10];
                System.Array.Copy( buffer, tmp, buffer.Length );
                buffer = tmp;
            }

            buffer[len] = data;
            len++;
        }

        return System.Text.Encoding.UTF8.GetString( buffer, 0, len );
    }

    public byte GetByte()
    {
        return (byte)m_Data.ReadByte();
    }

    public byte[] GetBytes( int count )
    {
        byte[] value = new byte[count];
        m_Data.Read( value, 0, count );

        return value;
    }
}
