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

    public class OpenCL : IDisposable
    {
        Dictionary<IntPtr,Platform> _Platforms = new Dictionary<IntPtr, Platform>();
        IntPtr[] PlatformIDs;
        Platform[] Platforms;
        internal OpenCLAPI CL;
        bool disposed = false;

        public OpenCL()
        {
            CL = new ATI();
            Initialize();
        }

        public OpenCL( OpenCLAPI cl )
        {
            CL = cl;
            Initialize();
        }

        ~OpenCL()
        {
            Dispose( false );
        }


        #region IDisposable Members

        // Implement IDisposable.
        // Do not make this method virtual.
        // A derived class should not be able to override this method.
        public void Dispose()
        {
            Dispose(true);
            // This object will be cleaned up by the Dispose method.
            // Therefore, you should call GC.SupressFinalize to
            // take this object off the finalization queue
            // and prevent finalization code for this object
            // from executing a second time.
            GC.SuppressFinalize(this);
        }



        // Dispose(bool disposing) executes in two distinct scenarios.
        // If disposing equals true, the method has been called directly
        // or indirectly by a user's code. Managed and unmanaged resources
        // can be disposed.
        // If disposing equals false, the method has been called by the
        // runtime from inside the finalizer and you should not reference
        // other objects. Only unmanaged resources can be disposed.
        private void Dispose(bool disposing)
        {
            // Check to see if Dispose has already been called.
            if (!this.disposed)
            {
                // If disposing equals true, dispose all managed
                // and unmanaged resources.
                if (disposing)
                {
                }

                // Call the appropriate methods to clean up
                // unmanaged resources here.
                // If disposing is false,
                // only the following code is executed.


                // Note disposing has been done.
                disposed = true;
            }
        }

        #endregion

        private void Initialize()
        {
            PlatformIDs = GetPlatformIDs();
            if (PlatformIDs == null)
                return;

            Platforms = new Platform[PlatformIDs.Length];
            for (int i = 0; i < PlatformIDs.Length; i++)
            {
                Platform p;

                p = new Platform(CL, PlatformIDs[i]);
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