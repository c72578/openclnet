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

    unsafe public class Context : IDisposable, InteropTools.IPropertyContainer
    {
        public IntPtr ContextID { get; protected set; }
        public Platform Platform { get; protected set; }
        public OpenCLAPI CL { get { return Platform.CL; } }
        // Track whether Dispose has been called.
        private bool disposed = false;

        #region Construction / Destruction

        internal Context( Platform platform, IntPtr contextID )
        {
            Platform = platform;
            ContextID = contextID;
        }

        internal Context( Platform platform, ContextProperties[] properties, Device[] devices )
        {
            IntPtr[] intPtrProperties;
            IntPtr[] deviceIDs;
            ErrorCode result;

            Platform = platform;
            deviceIDs = InteropTools.ConvertDevicesToDeviceIDs( devices );

            intPtrProperties = new IntPtr[properties.Length];
            for( int i=0; i<properties.Length; i++ )
                intPtrProperties[i] = new IntPtr( (long)properties[i] );

            ContextID = (IntPtr)CL.CreateContext( intPtrProperties,
                (uint)devices.Length,
                deviceIDs,
                null,
                IntPtr.Zero,
                out result );
            if( result!=ErrorCode.SUCCESS )
                throw new OpenCLException( "CreateContext failed: "+result );
        }

        // Use C# destructor syntax for finalization code.
        // This destructor will run only if the Dispose method
        // does not get called.
        // It gives your base class the opportunity to finalize.
        // Do not provide destructors in types derived from this class.
        ~Context()
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
                CL.ReleaseContext( ContextID );
                ContextID = IntPtr.Zero;

                // Note disposing has been done.
                disposed = true;
            }
        }


        #endregion

        #region Properties

        public uint ContextReferenceCount { get { return InteropTools.ReadUInt( this, (uint)ContextInfo.REFERENCE_COUNT ); } }

        public Device[] ContextDevices
        {
            get
            {
                IntPtr contextDevicesSize;
                ErrorCode result;
                IntPtr[] contextDevices;

                result = (ErrorCode)CL.GetContextInfo( ContextID, (uint)ContextInfo.DEVICES, IntPtr.Zero, null, out contextDevicesSize );
                if( result!=ErrorCode.SUCCESS )
                    throw new OpenCLException( "Unable to get context info for context "+ContextID+" "+result );

                contextDevices = new IntPtr[contextDevicesSize.ToInt64()/sizeof( IntPtr )];
                fixed( IntPtr* pContextDevices = contextDevices )
                {
                    result = (ErrorCode)CL.GetContextInfo( ContextID, (uint)ContextInfo.DEVICES, contextDevicesSize, (void*)pContextDevices, out contextDevicesSize );
                    if( result!=ErrorCode.SUCCESS )
                        throw new OpenCLException( "Unable to get context info for context "+ContextID+" "+result );
                }
                return InteropTools.ConvertDeviceIDsToDevices( Platform, contextDevices );
            }
        }

        public ContextProperties[] ContextProperties
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        #endregion

        public CommandQueue CreateCommandQueue( Device device, CommandQueueProperties properties )
        {
            IntPtr commandQueueID;
            ErrorCode result;

            commandQueueID = (IntPtr)CL.CreateCommandQueue( ContextID, device.DeviceID, (ulong)properties, out result );
            if( result!=ErrorCode.SUCCESS )
                throw new OpenCLException( "CreateCommandQueue failed with error code "+result );
            return new CommandQueue( this, device, commandQueueID );
        }

        public Mem CreateBuffer( MemFlags flags, long size, IntPtr pHost )
        {
            return CreateBuffer( flags, size, pHost.ToPointer() );
        }

        public Mem CreateBuffer( MemFlags flags, long size, void* pHost )
        {
            IntPtr memID;
            ErrorCode result;

            memID = (IntPtr)CL.CreateBuffer( ContextID, (ulong)flags, new IntPtr(size), pHost, out result );
            if( result!=ErrorCode.SUCCESS )
                throw new OpenCLException( "CreateBuffer failed with error code "+result );
            return new Mem( this, memID );
        }

        public Program CreateProgramWithSource( string source )
        {
            return CreateProgramWithSource( new string[] { source } );
        }

        public Program CreateProgramWithSource( string[] source )
        {
            IntPtr programID;
            ErrorCode result;

            programID = (IntPtr)CL.CreateProgramWithSource( ContextID, 1, source, (IntPtr[])null, out result );
            if( result!=ErrorCode.SUCCESS )
                throw new OpenCLException( "CreateProgramWithSource failed with error code "+result );
            return new Program( this, programID );
        }

        public Sampler CreateSampler( bool normalizedCoords, AddressingMode addressingMode, FilterMode filterMode, out ErrorCode result )
        {
            IntPtr samplerID;
            
            samplerID = CL.CreateSampler( ContextID, normalizedCoords, (uint)addressingMode, (uint)filterMode, out result );
            return new Sampler( this, samplerID );
        }

        public static implicit operator IntPtr( Context c )
        {
            return c.ContextID;
        }

        #region IPropertyContainer Members

        unsafe public IntPtr GetPropertySize( uint key )
        {
            IntPtr size;
            ErrorCode result;

            result = (ErrorCode)CL.GetContextInfo( ContextID, key, IntPtr.Zero, null, out size );
            if( result!=ErrorCode.SUCCESS )
                throw new OpenCLException( "GetContextInfo failed: "+result );
            return size;
        }

        unsafe public void ReadProperty( uint key, IntPtr keyLength, void* pBuffer )
        {
            IntPtr size;
            ErrorCode result;

            result = (ErrorCode)CL.GetContextInfo( ContextID, key, keyLength, pBuffer, out size );
            if( result!=ErrorCode.SUCCESS )
                throw new OpenCLException( "GetContextInfo failed: "+result );
        }

        #endregion
    }
}
