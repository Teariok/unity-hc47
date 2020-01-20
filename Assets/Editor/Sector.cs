using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;

public class Sector
{
    protected FileStream m_Data;

    protected string m_SectorId;
    protected uint m_SectorSize;
    protected bool m_HasSubsectors;
    protected bool m_HasMultidata;
    protected uint m_BodySize;
    protected uint m_SubsectorCount;
    protected uint m_MultidataCount;
    protected uint[] m_MultidataSizes;
    protected Sector[] m_Subsectors;

    public virtual void Unpack( FileStream data )
    {
        m_Data = data;

        ReadHeader();
        ReadMeta();
        ExtractSubsectors();
        ReadBody();
    }

    protected void ReadHeader()
    {
        m_SectorId = GetString( 4 );
        uint sectorInfo = GetUInt();

        m_SectorSize = (sectorInfo & 0x3FFFFFFF);
        m_HasSubsectors = (sectorInfo & (1 << 31)) != 0;
        m_HasMultidata = (sectorInfo & (1 << 30)) != 0;
    }

    protected void ReadMeta()
    {
        m_BodySize = m_HasSubsectors || m_HasMultidata ? GetUInt() : 8;
        m_SubsectorCount = m_HasSubsectors ? GetUInt() : 8;
        m_MultidataCount = m_HasMultidata ? GetUInt() : 0;

        if( m_HasMultidata )
        {
            m_MultidataSizes = new uint[m_MultidataCount];
            for( uint i = 0; i < m_MultidataCount; i++ )
            {
                m_MultidataSizes[i] = GetUInt();
            }
        }
    }

    protected void ExtractSubsectors()
    {
        if( m_HasSubsectors )
        {
            m_Subsectors = new Sector[m_SubsectorCount];
            for( uint i = 0; i < m_SubsectorCount; i++ )
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
        if( m_HasMultidata )
        {
            for( uint i = 0; i < m_MultidataCount; i++ )
            {
                GetBytes( (int)m_MultidataSizes[i] );
            }
        }
        else
        {
            uint bodySize = m_SectorSize - m_BodySize;
            GetBytes( (int)bodySize );
        }
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
