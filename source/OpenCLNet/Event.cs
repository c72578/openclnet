﻿/*
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
using System.Linq;
using System.Text;

namespace OpenCLNet
{
    public class Event : IDisposable, InteropTools.IPropertyContainer
    {
        public IntPtr EventID { get; protected set; }
        public Context Context { get; protected set; }
        OpenCLAPI CL { get { return Context.CL; } }

        // Track whether Dispose has been called.
        private bool disposed = false;

        #region Construction / Destruction

        internal Event( Context context, IntPtr eventID )
        {
            Context = context;
            EventID = eventID;
        }

        // Use C# destructor syntax for finalization code.
        // This destructor will run only if the Dispose method
        // does not get called.
        // It gives your base class the opportunity to finalize.
        // Do not provide destructors in types derived from this class.
        ~Event()
        {
            // Do not re-create Dispose clean-up code here.
            // Calling Dispose(false) is optimal in terms of
            // readability and maintainability.
            Dispose( false );
        }

        #endregion

        #region IDisposable Members

        // Implement IDisposable.
        // Do not make this method virtual.
        // A derived class should not be able to override this method.
        public void Dispose()
        {
            Dispose( true );
            // This object will be cleaned up by the Dispose method.
            // Therefore, you should call GC.SupressFinalize to
            // take this object off the finalization queue
            // and prevent finalization code for this object
            // from executing a second time.
            GC.SuppressFinalize( this );
        }

        // Dispose(bool disposing) executes in two distinct scenarios.
        // If disposing equals true, the method has been called directly
        // or indirectly by a user's code. Managed and unmanaged resources
        // can be disposed.
        // If disposing equals false, the method has been called by the
        // runtime from inside the finalizer and you should not reference
        // other objects. Only unmanaged resources can be disposed.
        private void Dispose( bool disposing )
        {
            // Check to see if Dispose has already been called.
            if( !this.disposed )
            {
                // If disposing equals true, dispose all managed
                // and unmanaged resources.
                if( disposing )
                {
                    // Dispose managed resources.
                }

                // Call the appropriate methods to clean up
                // unmanaged resources here.
                // If disposing is false,
                // only the following code is executed.
                CL.ReleaseEvent( EventID );
                EventID = IntPtr.Zero;

                // Note disposing has been done.
                disposed = true;
            }
        }

        #endregion

        #region IPropertyContainer Members

        unsafe public IntPtr GetPropertySize( uint key )
        {
            IntPtr size;
            ErrorCode result;

            result = (ErrorCode)CL.GetEventInfo( EventID, key, IntPtr.Zero, null, out size );
            if( result!=ErrorCode.SUCCESS )
                throw new OpenCLException( "GetEventInfo failed; "+result, result);
            return size;
        }

        unsafe public void ReadProperty( uint key, IntPtr keyLength, void* pBuffer )
        {
            IntPtr size;
            ErrorCode result;

            result = (ErrorCode)CL.GetEventInfo( EventID, key, keyLength, pBuffer, out size );
            if( result!=ErrorCode.SUCCESS )
                throw new OpenCLException( "GetEventInfo failed; "+result, result);
        }

        #endregion

        public unsafe void GetEventProfilingInfo(ProfilingInfo paramName, out ulong paramValue)
        {
            IntPtr paramValueSizeRet;
            ulong v;
            ErrorCode errorCode;

            errorCode = CL.GetEventProfilingInfo(EventID, paramName, (IntPtr)sizeof(ulong), &v, out paramValueSizeRet);
            if (errorCode != ErrorCode.SUCCESS)
                throw new OpenCLException("GetEventProfilingInfo failed with error code "+errorCode, errorCode );
            paramValue = v;
        }

        public static implicit operator IntPtr( Event _event )
        {
            return _event.EventID;
        }
    }
}
