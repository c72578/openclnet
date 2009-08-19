using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Text;

namespace OpenCLNet
{
    public class InteropTools
    {
        public unsafe interface IPropertyContainer
        {
            IntPtr GetPropertySize( uint key );
            void ReadProperty( uint key, IntPtr keyLength, void* pBuffer );
        }

        public static byte[] CreateNullTerminatedString( string s )
        {
            int len;
            byte[] data;

            len = Encoding.UTF8.GetByteCount( s );
            data = new byte[len+1];
            Encoding.UTF8.GetBytes( s, 0, s.Length, data, 0 );
            data[len] = 0;
            return data;
        }

        public static IntPtr[] ConvertDevicesToDeviceIDs( Device[] devices )
        {
            IntPtr[] deviceIDs;

            if( devices==null )
                return null;

            deviceIDs = new IntPtr[devices.Length];
            for( int i=0; i<devices.Length; i++ )
                deviceIDs[i] = devices[i];
            return deviceIDs;
        }

        public static Device[] ConvertDeviceIDsToDevices( Platform platform, IntPtr[] deviceIDs )
        {
            Device[] devices;

            if( deviceIDs==null )
                return null;

            devices = new Device[deviceIDs.Length];
            for( int i=0; i<deviceIDs.Length; i++ )
                devices[i] = platform.GetDevice( deviceIDs[i] );
            return devices;
        }

        #region Helper functions to read properties

        unsafe public static bool ReadBool( IPropertyContainer propertyContainer, uint key )
        {
            return ReadUInt( propertyContainer, key )==(uint)Bool.TRUE?true:false;
        }

        unsafe public static byte[] ReadBytes( IPropertyContainer propertyContainer, uint key )
        {
            IntPtr   size;

            size = propertyContainer.GetPropertySize( key );
            byte[]  data = new byte[size.ToInt64()];
            fixed( byte* pData=data )
            {
                propertyContainer.ReadProperty( key, size, pData );
            }
            return data;
        }

        unsafe public static string ReadString( IPropertyContainer propertyContainer, uint key )
        {
            IntPtr   size;
            string s;

            size = propertyContainer.GetPropertySize( (uint)key );
            byte[] stringData = new byte[size.ToInt64()];
            fixed( byte* pStringData=stringData )
            {
                propertyContainer.ReadProperty( (uint)key, size, pStringData );
            }

            s = Encoding.UTF8.GetString( stringData );
            int nullIndex = s.IndexOf( '\0' );
            if( nullIndex>=0 )
                return s.Substring( 0, nullIndex );
            else
                return s;
        }

        unsafe public static int ReadInt( IPropertyContainer propertyContainer, uint key )
        {
            int output;

            propertyContainer.ReadProperty( key, new IntPtr(sizeof(int)), &output );
            return output;
        }

        unsafe public static uint ReadUInt( IPropertyContainer propertyContainer, uint key )
        {
            uint output;

            propertyContainer.ReadProperty( key, new IntPtr(sizeof( uint )), &output );
            return output;
        }

        unsafe public static long ReadLong( IPropertyContainer propertyContainer, uint key )
        {
            long output;

            propertyContainer.ReadProperty( key, new IntPtr(sizeof( long )), &output );
            return output;
        }

        unsafe public static ulong ReadULong( IPropertyContainer propertyContainer, uint key )
        {
            ulong output;

            propertyContainer.ReadProperty( key, new IntPtr(sizeof( ulong )), &output );
            return output;
        }

        unsafe public static IntPtr ReadIntPtr( IPropertyContainer propertyContainer, uint key )
        {
            IntPtr output;

            propertyContainer.ReadProperty( key, new IntPtr(sizeof( IntPtr )), &output );
            return output;
        }

        unsafe public static IntPtr[] ReadIntPtrArray( IPropertyContainer propertyContainer, uint key )
        {
            IntPtr size = propertyContainer.GetPropertySize( key );
            long numElements = (long)size/sizeof(IntPtr);
            IntPtr[] ptrs = new IntPtr[numElements];
            byte[] data = InteropTools.ReadBytes( propertyContainer, key );

            fixed( byte* pData = data )
            {
                void** pBS = (void**)pData;
                for( int i=0; i<numElements; i++ )
                    ptrs[i] = new IntPtr( pBS[i] );
            }
            return ptrs;
        }

        #endregion

    }
}
