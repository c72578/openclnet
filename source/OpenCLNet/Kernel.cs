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
    /// <summary>
    /// The Kernel class wraps an OpenCL kernel handle
    /// 
    /// The main purposes of this class is to serve as a handle to
    /// a compiled OpenCL function and to set arguments on the function
    /// before enqueueing calls.
    /// 
    /// Arguments are set using either the overloaded SetArg functions or
    /// explicit Set*Arg functions where * is a type. The most usual types
    /// are supported, but no vectors. If you need to set a parameter that's
    /// more advanced than what's supported here, use the version of SetArg
    /// that takes a pointer and size.
    /// 
    /// Note that pointer arguments are set by passing their OpenCL memory object,
    /// not native pointers.
    /// </summary>
    unsafe public class Kernel : InteropTools.IPropertyContainer
    {
        // Track whether Dispose has been called.
        private bool disposed = false;

        public string FunctionName { get { return InteropTools.ReadString( this, (uint)KernelInfo.FUNCTION_NAME ); } }
        public uint NumArgs { get { return InteropTools.ReadUInt( this, (uint)KernelInfo.NUM_ARGS ); } }
        public uint ReferenceCount { get { return InteropTools.ReadUInt( this, (uint)KernelInfo.REFERENCE_COUNT ); } }
        public Context Context { get; protected set; }
        public Program Program { get { throw new NotImplementedException(); } }
        public IntPtr KernelID { get; set; }

        internal OpenCLAPI CL { get { return Context.CL; } }

        internal Kernel( Context context, IntPtr kernelID )
        {
            Context = context;
            KernelID = kernelID;
        }

        ~Kernel()
        {
            Dispose( false );
        }

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
                CL.ReleaseKernel( KernelID );
                KernelID = IntPtr.Zero;

                // Note disposing has been done.
                disposed = true;
            }
        }


        #endregion

        public void SetArg( int argIndex, IntPtr argSize, IntPtr argValue )
        {
            ErrorCode result;

            result = (ErrorCode)CL.SetKernelArg( KernelID, (uint)argIndex, argSize, argValue.ToPointer() );
            if( result!=ErrorCode.SUCCESS )
                throw new OpenCLException( "SetArg failed with error code "+result, result);
        }

        /// <summary>
        /// Set argument argIndex to c
        /// </summary>
        /// <param name="argIndex"></param>
        /// <param name="c"></param>
        public void SetArg( int argIndex, int c )
        {
            ErrorCode result;

            result = (ErrorCode)CL.SetKernelArg(KernelID, (uint)argIndex, new IntPtr(sizeof(int)), &c);
            if( result!=ErrorCode.SUCCESS )
                throw new OpenCLException( "SetArg failed with error code "+result, result);
        }

        /// <summary>
        /// Set argument argIndex to c
        /// </summary>
        /// <param name="argIndex"></param>
        /// <param name="c"></param>
        public void SetArg( int argIndex, long c )
        {
            ErrorCode result;

            result = (ErrorCode)CL.SetKernelArg(KernelID, (uint)argIndex, new IntPtr(sizeof(long)), &c);
            if( result!=ErrorCode.SUCCESS )
                throw new OpenCLException( "SetArg failed with error code "+result, result);
        }

        /// <summary>
        /// Set argument argIndex to c
        /// </summary>
        /// <param name="argIndex"></param>
        /// <param name="c"></param>
        public void SetArg( int argIndex, float c )
        {
            ErrorCode result;

            result = (ErrorCode)CL.SetKernelArg(KernelID, (uint)argIndex, new IntPtr(sizeof(float)), &c);
            if( result!=ErrorCode.SUCCESS )
                throw new OpenCLException( "SetArg failed with error code "+result, result);
        }

        /// <summary>
        /// Set argument argIndex to c
        /// </summary>
        /// <param name="argIndex"></param>
        /// <param name="c"></param>
        public void SetArg(int argIndex, double c)
        {
            ErrorCode result;

            result = (ErrorCode)CL.SetKernelArg(KernelID, (uint)argIndex, new IntPtr(sizeof(double)), &c);
            if( result!=ErrorCode.SUCCESS )
                throw new OpenCLException( "SetArg failed with error code "+result, result);
        }

        /// <summary>
        /// Set argument argIndex to c
        /// </summary>
        /// <param name="argIndex"></param>
        /// <param name="c"></param>
        public void SetArg( int argIndex, IntPtr c )
        {
            ErrorCode result;

            result = (ErrorCode)CL.SetKernelArg(KernelID, (uint)argIndex, new IntPtr(sizeof(IntPtr)), &c);
            if( result!=ErrorCode.SUCCESS )
                throw new OpenCLException( "SetArg failed with error code "+result, result);
        }

        #region Setargs with explicit function names(For VB mostly)

        public void SetIntArg(int argIndex, int c)
        {
            ErrorCode result;

            result = (ErrorCode)CL.SetKernelArg(KernelID, (uint)argIndex, new IntPtr(sizeof(int)), &c);
            if (result != ErrorCode.SUCCESS)
                throw new OpenCLException("SetArg failed with error code " + result, result);
        }

        public void SetLongArg(int argIndex, long c)
        {
            ErrorCode result;

            result = (ErrorCode)CL.SetKernelArg(KernelID, (uint)argIndex, new IntPtr(sizeof(long)), &c);
            if (result != ErrorCode.SUCCESS)
                throw new OpenCLException("SetArg failed with error code " + result, result);
        }

        public void SetSingleArg(int argIndex, float c)
        {
            ErrorCode result;

            result = (ErrorCode)CL.SetKernelArg(KernelID, (uint)argIndex, new IntPtr(sizeof(float)), &c);
            if (result != ErrorCode.SUCCESS)
                throw new OpenCLException("SetArg failed with error code " + result, result);
        }

        public void SetDoubleArg(int argIndex, double c)
        {
            ErrorCode result;

            result = (ErrorCode)CL.SetKernelArg(KernelID, (uint)argIndex, new IntPtr(sizeof(double)), &c);
            if (result != ErrorCode.SUCCESS)
                throw new OpenCLException("SetArg failed with error code " + result, result);
        }

        public void SetIntPtrArg(int argIndex, IntPtr c)
        {
            ErrorCode result;

            result = (ErrorCode)CL.SetKernelArg(KernelID, (uint)argIndex, new IntPtr(sizeof(IntPtr)), &c);
            if (result != ErrorCode.SUCCESS)
                throw new OpenCLException("SetArg failed with error code " + result, result);
        }

        #endregion

        public static implicit operator IntPtr( Kernel k )
        {
            return k.KernelID;
        }

        #region IPropertyContainer Members

        public IntPtr GetPropertySize( uint key )
        {
            IntPtr size;
            ErrorCode result;

            result = (ErrorCode)CL.GetKernelInfo( KernelID, key, IntPtr.Zero, null, out size );
            if( result!=ErrorCode.SUCCESS )
                throw new OpenCLException( "Unable to get kernel info for kernel "+KernelID, result);
            return size;
        }

        public void ReadProperty( uint key, IntPtr keyLength, void* pBuffer )
        {
            IntPtr size;
            ErrorCode result;

            result = (ErrorCode)CL.GetKernelInfo( KernelID, (uint)key, keyLength, pBuffer, out size );
            if( result!=(int)ErrorCode.SUCCESS )
                throw new OpenCLException( "Unable to get kernel info for kernel "+KernelID, result);
        }

        #endregion
    }
}
