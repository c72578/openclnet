/*
 * Copyright (c) 2009 Olav Kalgraf(olav.kalgraf@gmail.com)
 *
 * Permission is hereby granted, free of charge, to any person
 * obtaining a copy of this software and associated documentation
 * files (the "Software"), to deal in the Software without
 * restriction, including without limitation the rights to use,
 * copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the
 * Software is furnished to do so, subject to the following
 * conditions:
 * 
 * The above copyright notice and this permission notice shall be
 * included in all copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
 * EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
 * OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
 * NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
 * HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
 * WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
 * FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
 * OTHER DEALINGS IN THE SOFTWARE.
 */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Text;

namespace OpenCLNet
{

    unsafe public class Platform : InteropTools.IPropertyContainer
    {
        public OpenCLAPI CL { get; protected set; }
        public IntPtr PlatformID { get; protected set; }
        public string Profile { get { return InteropTools.ReadString( this, (uint)PlatformInfo.PROFILE ); } }
        public string Version { get { return InteropTools.ReadString( this, (uint)PlatformInfo.VERSION ); } }
        public string Name { get { return InteropTools.ReadString( this, (uint)PlatformInfo.NAME ); } }
        public string Vendor { get { return InteropTools.ReadString( this, (uint)PlatformInfo.VENDOR ); } }
        public string Extensions { get { return InteropTools.ReadString( this, (uint)PlatformInfo.EXTENSIONS ); } }

        protected Dictionary<IntPtr,Device> _Devices =  new Dictionary<IntPtr, Device>();
        Device[] DeviceList;
        IntPtr[] DeviceIDs;

        protected HashSet<string> ExtensionHashSet = new HashSet<string>();

        public Platform( OpenCLAPI cl, IntPtr platformID )
        {
            CL = cl;
            PlatformID = platformID;

            // Create a local representation of all devices
            DeviceIDs = QueryDeviceIntPtr( DeviceType.ALL );
            for( int i=0; i<DeviceIDs.Length; i++ )
                _Devices[DeviceIDs[i]] = new Device( this, DeviceIDs[i] );
            DeviceList = InteropTools.ConvertDeviceIDsToDevices( this, DeviceIDs );

            InitializeExtensionHashSet();
        }

        public Context CreateDefaultContext()
        {
            return CreateDefaultContext(null, IntPtr.Zero);
        }

        public Context CreateDefaultContext( ContextNotify notify, IntPtr userData )
        {
            IntPtr[] properties = new IntPtr[]
            {
                new IntPtr((long)ContextProperties.PLATFORM), PlatformID,
                IntPtr.Zero,
            };

            IntPtr contextID;
            ErrorCode result;

            contextID = (IntPtr)CL.CreateContext( properties,
                (uint)DeviceIDs.Length,
                DeviceIDs,
                notify,
                userData,
                out result );
            if( result!=ErrorCode.SUCCESS )
                throw new OpenCLException( "CreateContext failed with error code: "+result, result);
            return new Context( this, contextID );
        }

        public Context CreateContext(IntPtr[] contextProperties, Device[] devices, ContextNotify notify, IntPtr userData)
        {
            IntPtr contextID;
            ErrorCode result;

            IntPtr[] deviceIDs = InteropTools.ConvertDevicesToDeviceIDs(devices);
            contextID = (IntPtr)CL.CreateContext(contextProperties,
                (uint)deviceIDs.Length,
                deviceIDs,
                notify,
                userData,
                out result);
            if (result != ErrorCode.SUCCESS)
                throw new OpenCLException("CreateContext failed with error code: " + result, result);
            return new Context(this, contextID);
        }

        public Context CreateContextFromType(IntPtr[] contextProperties, DeviceType deviceType, ContextNotify notify, IntPtr userData)
        {
            IntPtr contextID;
            ErrorCode result;

            contextID = (IntPtr)CL.CreateContextFromType(contextProperties,
                deviceType,
                notify,
                userData,
                out result);
            if (result != ErrorCode.SUCCESS)
                throw new OpenCLException("CreateContextFromType failed with error code: " + result, result);
            return new Context(this, contextID);
        }

        public Device GetDevice( IntPtr index )
        {
            return _Devices[index];
        }

        protected IntPtr[] QueryDeviceIntPtr( DeviceType deviceType )
        {
            ErrorCode result;
            uint numberOfDevices;
            IntPtr[] deviceIDs;

            result = (ErrorCode)CL.GetDeviceIDs( PlatformID, deviceType, 0, null, out numberOfDevices );
            if (result == ErrorCode.DEVICE_NOT_FOUND)
                return new IntPtr[0];

            if( result!=ErrorCode.SUCCESS )
                throw new OpenCLException( "GetDeviceIDs failed: "+((ErrorCode)result).ToString(), result);

            deviceIDs = new IntPtr[numberOfDevices];
            result = (ErrorCode)CL.GetDeviceIDs( PlatformID, deviceType, numberOfDevices, deviceIDs, out numberOfDevices );
            if (result != ErrorCode.SUCCESS)
                throw new OpenCLException( "GetDeviceIDs failed: "+((ErrorCode)result).ToString(), result);

            return deviceIDs;
        }

        public Device[] QueryDevices( DeviceType deviceType )
        {
            IntPtr[] deviceIDs;

            deviceIDs = QueryDeviceIntPtr( deviceType );
            return InteropTools.ConvertDeviceIDsToDevices( this, deviceIDs );
        }

        protected void InitializeExtensionHashSet()
        {
            string[] ext = Extensions.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (string s in ext)
                ExtensionHashSet.Add(s);
        }

        public bool HasExtension(string extension)
        {
            return ExtensionHashSet.Contains(extension);
        }

        public bool HasExtensions(string[] extensions)
        {
            foreach (string s in extensions)
                if (!ExtensionHashSet.Contains(s))
                    return false;
            return true;
        }

        public static implicit operator IntPtr( Platform p )
        {
            return p.PlatformID;
        }

        #region IPropertyContainer Members

        public IntPtr GetPropertySize( uint key )
        {
            IntPtr propertySize;
            ErrorCode result;

            result = (ErrorCode)CL.GetPlatformInfo( PlatformID, key, IntPtr.Zero, null, out propertySize );
            if( result!=ErrorCode.SUCCESS )
                throw new OpenCLException( "Unable to get platform info for platform "+PlatformID+": "+result, result);
            return propertySize;

        }

        public void ReadProperty( uint key, IntPtr keyLength, void* pBuffer )
        {
            IntPtr propertySize;
            ErrorCode result;

            result = (ErrorCode)CL.GetPlatformInfo( PlatformID, key, keyLength, (void*)pBuffer, out propertySize );
            if( result!=ErrorCode.SUCCESS )
                throw new OpenCLException( "Unable to get platform info for platform "+PlatformID+": "+result, result);
        }

        #endregion
    }
}
