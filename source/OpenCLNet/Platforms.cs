using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Text;

namespace OpenCLNet
{

    public class OpenCL
    {
        Dictionary<IntPtr,Platform> _Platforms = new Dictionary<IntPtr, Platform>();
        IntPtr[] PlatformIDs;
        Platform[] Platforms;
        public OpenCLAPI CL;

        public OpenCL( OpenCLAPI cl )
        {
            CL = cl;
            PlatformIDs = GetPlatformIDs();
            if( PlatformIDs==null )
                return;

            Platforms = new Platform[PlatformIDs.Length];
            for( int i=0; i<PlatformIDs.Length; i++ )
            {
                Platform p;

                p = new Platform( CL, PlatformIDs[i] );
                Platforms[i] = p;
                _Platforms[PlatformIDs[i]] = p;
            }
        }

        public int NumberOfPlatforms
        {
            get { return _Platforms.Count; }
        }

        public Platform[] GetPlatforms()
        {
            return (Platform[])Platforms.Clone();
        }

        public Platform GetPlatform( int index )
        {
            return _Platforms[PlatformIDs[index]];
        }

        public Platform GetPlatform( IntPtr platformID )
        {
            return _Platforms[platformID];
        }

        unsafe protected IntPtr[] GetPlatformIDs()
        {
            IntPtr[] platformIDs;
            ErrorCode result;
            uint returnedPlatforms;

            result = (ErrorCode)CL.GetPlatformIDs( 0, null, out returnedPlatforms );
            if( result==ErrorCode.INVALID_VALUE )
                return null;

            platformIDs = new IntPtr[returnedPlatforms];
            result = (ErrorCode)CL.GetPlatformIDs( (uint)platformIDs.Length, platformIDs, out returnedPlatforms );
            if( result==ErrorCode.INVALID_VALUE )
                return null;
            return platformIDs;
        }
    }
}