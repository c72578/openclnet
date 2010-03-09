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
        public Program Program { get; protected set; }
        public IntPtr KernelID { get; set; }

        internal Kernel( Context context, Program program, IntPtr kernelID )
        {
            Context = context;
            Program = program;
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
                OpenCL.ReleaseKernel( KernelID );
                KernelID = IntPtr.Zero;

                // Note disposing has been done.
                disposed = true;
            }
        }


        #endregion

        public void SetArg( int argIndex, IntPtr argSize, IntPtr argValue )
        {
            ErrorCode result;

            result = (ErrorCode)OpenCL.SetKernelArg( KernelID, (uint)argIndex, argSize, argValue.ToPointer() );
            if( result!=ErrorCode.SUCCESS )
                throw new OpenCLException( "SetArg failed with error code "+result, result);
        }

        #region SetArg functions

        /// <summary>
        /// Set argument argIndex to c
        /// </summary>
        /// <param name="argIndex"></param>
        /// <param name="c"></param>
        public void SetArg(int argIndex, sbyte c)
        {
            ErrorCode result;

            result = (ErrorCode)OpenCL.SetKernelArg(KernelID, (uint)argIndex, new IntPtr(sizeof(sbyte)), &c);
            if (result != ErrorCode.SUCCESS)
                throw new OpenCLException("SetArg failed with error code " + result, result);
        }

        /// <summary>
        /// Set argument argIndex to c
        /// </summary>
        /// <param name="argIndex"></param>
        /// <param name="c"></param>
        public void SetArg(int argIndex, byte c)
        {
            ErrorCode result;

            result = (ErrorCode)OpenCL.SetKernelArg(KernelID, (uint)argIndex, new IntPtr(sizeof(byte)), &c);
            if (result != ErrorCode.SUCCESS)
                throw new OpenCLException("SetArg failed with error code " + result, result);
        }

        /// <summary>
        /// Set argument argIndex to c
        /// </summary>
        /// <param name="argIndex"></param>
        /// <param name="c"></param>
        public void SetArg(int argIndex, short c)
        {
            ErrorCode result;

            result = (ErrorCode)OpenCL.SetKernelArg(KernelID, (uint)argIndex, new IntPtr(sizeof(short)), &c);
            if (result != ErrorCode.SUCCESS)
                throw new OpenCLException("SetArg failed with error code " + result, result);
        }

        /// <summary>
        /// Set argument argIndex to c
        /// </summary>
        /// <param name="argIndex"></param>
        /// <param name="c"></param>
        public void SetArg(int argIndex, ushort c)
        {
            ErrorCode result;

            result = (ErrorCode)OpenCL.SetKernelArg(KernelID, (uint)argIndex, new IntPtr(sizeof(ushort)), &c);
            if (result != ErrorCode.SUCCESS)
                throw new OpenCLException("SetArg failed with error code " + result, result);
        }

        /// <summary>
        /// Set argument argIndex to c
        /// </summary>
        /// <param name="argIndex"></param>
        /// <param name="c"></param>
        public void SetArg(int argIndex, int c)
        {
            ErrorCode result;

            result = (ErrorCode)OpenCL.SetKernelArg(KernelID, (uint)argIndex, new IntPtr(sizeof(int)), &c);
            if (result != ErrorCode.SUCCESS)
                throw new OpenCLException("SetArg failed with error code " + result, result);
        }

        /// <summary>
        /// Set argument argIndex to c
        /// </summary>
        /// <param name="argIndex"></param>
        /// <param name="c"></param>
        public void SetArg(int argIndex, uint c)
        {
            ErrorCode result;

            result = (ErrorCode)OpenCL.SetKernelArg(KernelID, (uint)argIndex, new IntPtr(sizeof(uint)), &c);
            if (result != ErrorCode.SUCCESS)
                throw new OpenCLException("SetArg failed with error code " + result, result);
        }

        /// <summary>
        /// Set argument argIndex to c
        /// </summary>
        /// <param name="argIndex"></param>
        /// <param name="c"></param>
        public void SetArg(int argIndex, long c)
        {
            ErrorCode result;

            result = (ErrorCode)OpenCL.SetKernelArg(KernelID, (uint)argIndex, new IntPtr(sizeof(long)), &c);
            if (result != ErrorCode.SUCCESS)
                throw new OpenCLException("SetArg failed with error code " + result, result);
        }

        /// <summary>
        /// Set argument argIndex to c
        /// </summary>
        /// <param name="argIndex"></param>
        /// <param name="c"></param>
        public void SetArg(int argIndex, ulong c)
        {
            ErrorCode result;

            result = (ErrorCode)OpenCL.SetKernelArg(KernelID, (uint)argIndex, new IntPtr(sizeof(ulong)), &c);
            if (result != ErrorCode.SUCCESS)
                throw new OpenCLException("SetArg failed with error code " + result, result);
        }

        /// <summary>
        /// Set argument argIndex to c
        /// </summary>
        /// <param name="argIndex"></param>
        /// <param name="c"></param>
        public void SetArg( int argIndex, float c )
        {
            ErrorCode result;

            result = (ErrorCode)OpenCL.SetKernelArg(KernelID, (uint)argIndex, new IntPtr(sizeof(float)), &c);
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

            result = (ErrorCode)OpenCL.SetKernelArg(KernelID, (uint)argIndex, new IntPtr(sizeof(double)), &c);
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

            if (Context.Is64BitContext)
            {
                long l = c.ToInt64();
                result = (ErrorCode)OpenCL.SetKernelArg(KernelID, (uint)argIndex, new IntPtr(8), &l);
            }
            else
            {
                int i = c.ToInt32();
                result = (ErrorCode)OpenCL.SetKernelArg(KernelID, (uint)argIndex, new IntPtr(4), &i);
            }
            if( result!=ErrorCode.SUCCESS )
                throw new OpenCLException( "SetArg failed with error code "+result, result);
        }

        /// <summary>
        /// Set argument argIndex to mem
        /// </summary>
        /// <param name="argIndex"></param>
        /// <param name="mem"></param>
        public void SetArg(int argIndex, Mem mem)
        {
            ErrorCode result;
            IntPtr p = mem.MemID;

            result = (ErrorCode)OpenCL.SetKernelArg(KernelID, (uint)argIndex, new IntPtr(sizeof(IntPtr)), &p);
            if (result != ErrorCode.SUCCESS)
                throw new OpenCLException("SetArg failed with error code " + result, result);
        }

        /// <summary>
        /// Set argument argIndex to sampler
        /// </summary>
        /// <param name="argIndex"></param>
        /// <param name="sampler"></param>
        public void SetArg(int argIndex, Sampler sampler)
        {
            ErrorCode result;
            IntPtr p = sampler.SamplerID;

            result = (ErrorCode)OpenCL.SetKernelArg(KernelID, (uint)argIndex, new IntPtr(sizeof(IntPtr)), &p);
            if (result != ErrorCode.SUCCESS)
                throw new OpenCLException("SetArg failed with error code " + result, result);
        }

        #endregion

        #region Setargs with explicit function names(For VB mostly)

        public void SetSByteArg(int argIndex, sbyte c)
        {
            ErrorCode result;

            result = (ErrorCode)OpenCL.SetKernelArg(KernelID, (uint)argIndex, new IntPtr(sizeof(sbyte)), &c);
            if (result != ErrorCode.SUCCESS)
                throw new OpenCLException("SetArg failed with error code " + result, result);
        }

        public void SetByteArg(int argIndex, byte c)
        {
            ErrorCode result;

            result = (ErrorCode)OpenCL.SetKernelArg(KernelID, (uint)argIndex, new IntPtr(sizeof(byte)), &c);
            if (result != ErrorCode.SUCCESS)
                throw new OpenCLException("SetArg failed with error code " + result, result);
        }

        public void SetShortArg(int argIndex, short c)
        {
            ErrorCode result;

            result = (ErrorCode)OpenCL.SetKernelArg(KernelID, (uint)argIndex, new IntPtr(sizeof(short)), &c);
            if (result != ErrorCode.SUCCESS)
                throw new OpenCLException("SetArg failed with error code " + result, result);
        }

        public void SetUShortArg(int argIndex, ushort c)
        {
            ErrorCode result;

            result = (ErrorCode)OpenCL.SetKernelArg(KernelID, (uint)argIndex, new IntPtr(sizeof(ushort)), &c);
            if (result != ErrorCode.SUCCESS)
                throw new OpenCLException("SetArg failed with error code " + result, result);
        }

        public void SetIntArg(int argIndex, int c)
        {
            ErrorCode result;

            result = (ErrorCode)OpenCL.SetKernelArg(KernelID, (uint)argIndex, new IntPtr(sizeof(int)), &c);
            if (result != ErrorCode.SUCCESS)
                throw new OpenCLException("SetArg failed with error code " + result, result);
        }

        public void SetUIntArg(int argIndex, uint c)
        {
            ErrorCode result;

            result = (ErrorCode)OpenCL.SetKernelArg(KernelID, (uint)argIndex, new IntPtr(sizeof(uint)), &c);
            if (result != ErrorCode.SUCCESS)
                throw new OpenCLException("SetArg failed with error code " + result, result);
        }

        public void SetLongArg(int argIndex, long c)
        {
            ErrorCode result;

            result = (ErrorCode)OpenCL.SetKernelArg(KernelID, (uint)argIndex, new IntPtr(sizeof(long)), &c);
            if (result != ErrorCode.SUCCESS)
                throw new OpenCLException("SetArg failed with error code " + result, result);
        }

        public void SetULongArg(int argIndex, ulong c)
        {
            ErrorCode result;

            result = (ErrorCode)OpenCL.SetKernelArg(KernelID, (uint)argIndex, new IntPtr(sizeof(ulong)), &c);
            if (result != ErrorCode.SUCCESS)
                throw new OpenCLException("SetArg failed with error code " + result, result);
        }

        public void SetSingleArg(int argIndex, float c)
        {
            ErrorCode result;

            result = (ErrorCode)OpenCL.SetKernelArg(KernelID, (uint)argIndex, new IntPtr(sizeof(float)), &c);
            if (result != ErrorCode.SUCCESS)
                throw new OpenCLException("SetArg failed with error code " + result, result);
        }

        public void SetDoubleArg(int argIndex, double c)
        {
            ErrorCode result;

            result = (ErrorCode)OpenCL.SetKernelArg(KernelID, (uint)argIndex, new IntPtr(sizeof(double)), &c);
            if (result != ErrorCode.SUCCESS)
                throw new OpenCLException("SetArg failed with error code " + result, result);
        }

        public void SetIntPtrArg(int argIndex, IntPtr c)
        {
            ErrorCode result;

            if (Context.Is64BitContext)
            {
                long l = c.ToInt64();
                result = (ErrorCode)OpenCL.SetKernelArg(KernelID, (uint)argIndex, new IntPtr(8), &l);
            }
            else
            {
                int i = c.ToInt32();
                result = (ErrorCode)OpenCL.SetKernelArg(KernelID, (uint)argIndex, new IntPtr(4), &i);
            }
            if (result != ErrorCode.SUCCESS)
                throw new OpenCLException("SetArg failed with error code " + result, result);
        }

        public void SetMemArg(int argIndex, Mem mem)
        {
            ErrorCode result;
            IntPtr p = mem.MemID;

            result = (ErrorCode)OpenCL.SetKernelArg(KernelID, (uint)argIndex, new IntPtr(sizeof(IntPtr)), &p);
            if (result != ErrorCode.SUCCESS)
                throw new OpenCLException("SetArg failed with error code " + result, result);
        }

        public void SetSamplerArg(int argIndex, Sampler sampler)
        {
            ErrorCode result;
            IntPtr p = sampler.SamplerID;

            result = (ErrorCode)OpenCL.SetKernelArg(KernelID, (uint)argIndex, new IntPtr(sizeof(IntPtr)), &p);
            if (result != ErrorCode.SUCCESS)
                throw new OpenCLException("SetArg failed with error code " + result, result);
        }

        #endregion

#if false
        // Have to add some endian checking before compiling these into the library

        #region Set Char vectors
        
        public void SetChar2Arg(int argIndex, sbyte s0, sbyte s1)
        {
            sbyte* pBuffer = stackalloc sbyte[2];
            pBuffer[0] = s0;
            pBuffer[1] = s1;
            SetArg(argIndex, (IntPtr)(sizeof(sbyte) * 2), (IntPtr)pBuffer);
        }

        public void SetChar4Arg(int argIndex, sbyte s0, sbyte s1, sbyte s2, sbyte s3)
        {
            sbyte* pBuffer = stackalloc sbyte[4];
            pBuffer[0] = s0;
            pBuffer[1] = s1;
            pBuffer[2] = s2;
            pBuffer[3] = s3;
            SetArg(argIndex, (IntPtr)(sizeof(sbyte) * 4), (IntPtr)pBuffer);
        }

        #endregion
        
        #region Set UChar vectors

        public void SetUChar2Arg(int argIndex, byte s0, byte s1)
        {
            byte* pBuffer = stackalloc byte[2];
            pBuffer[0] = s0;
            pBuffer[1] = s1;
            SetArg(argIndex, (IntPtr)(sizeof(byte) * 2), (IntPtr)pBuffer);
        }

        public void SetUChar4Arg(int argIndex, byte s0, byte s1, byte s2, byte s3)
        {
            byte* pBuffer = stackalloc byte[4];
            pBuffer[0] = s0;
            pBuffer[1] = s1;
            pBuffer[2] = s2;
            pBuffer[3] = s3;
            SetArg(argIndex, (IntPtr)(sizeof(byte) * 4), (IntPtr)pBuffer);
        }

        #endregion

        #region Set Int vectors

        public void SetInt2Arg(int argIndex, int s0, int s1)
        {
            int* pBuffer = stackalloc int[2];
            pBuffer[0] = s0;
            pBuffer[1] = s1;
            SetArg(argIndex, (IntPtr)(sizeof(int) * 2), (IntPtr)pBuffer);
        }

        public void SetInt4Arg(int argIndex, int s0, int s1, int s2, int s3)
        {
            int* pBuffer = stackalloc int[4];
            pBuffer[0] = s0;
            pBuffer[1] = s1;
            pBuffer[2] = s2;
            pBuffer[3] = s3;
            SetArg(argIndex, (IntPtr)(sizeof(int) * 4), (IntPtr)pBuffer);
        }

        #endregion

        #region Set UInt vectors

        public void SetUInt2Arg(int argIndex, uint s0, uint s1)
        {
            uint* pBuffer = stackalloc uint[2];
            pBuffer[0] = s0;
            pBuffer[1] = s1;
            SetArg(argIndex, (IntPtr)(sizeof(uint) * 2), (IntPtr)pBuffer);
        }

        public void SetUInt4Arg(int argIndex, uint s0, uint s1, uint s2, uint s3)
        {
            uint* pBuffer = stackalloc uint[4];
            pBuffer[0] = s0;
            pBuffer[1] = s1;
            pBuffer[2] = s2;
            pBuffer[3] = s3;
            SetArg(argIndex, (IntPtr)(sizeof(uint) * 4), (IntPtr)pBuffer);
        }

        #endregion

        #region Set Long vectors

        public void SetLong2Arg(int argIndex, long s0, long s1)
        {
            long* pBuffer = stackalloc long[2];
            pBuffer[0] = s0;
            pBuffer[1] = s1;
            SetArg(argIndex, (IntPtr)(sizeof(long) * 2), (IntPtr)pBuffer);
        }

        public void SetLong4Arg(int argIndex, long s0, long s1, long s2, long s3)
        {
            long* pBuffer = stackalloc long[4];
            pBuffer[0] = s0;
            pBuffer[1] = s1;
            pBuffer[2] = s2;
            pBuffer[3] = s3;
            SetArg(argIndex, (IntPtr)(sizeof(long) * 4), (IntPtr)pBuffer);
        }

        #endregion

        #region Set ULong vectors

        public void SetULong2Arg(int argIndex, ulong s0, ulong s1)
        {
            ulong* pBuffer = stackalloc ulong[2];
            pBuffer[0] = s0;
            pBuffer[1] = s1;
            SetArg(argIndex, (IntPtr)(sizeof(ulong) * 2), (IntPtr)pBuffer);
        }

        public void SetULong4Arg(int argIndex, ulong s0, ulong s1, ulong s2, ulong s3)
        {
            ulong* pBuffer = stackalloc ulong[4];
            pBuffer[0] = s0;
            pBuffer[1] = s1;
            pBuffer[2] = s2;
            pBuffer[3] = s3;
            SetArg(argIndex, (IntPtr)(sizeof(ulong) * 4), (IntPtr)pBuffer);
        }

        #endregion

        #region Set Float vectors

        public void SetFloat2Arg(int argIndex, float s0, float s1)
        {
            float* pBuffer = stackalloc float[2];
            pBuffer[0] = s0;
            pBuffer[1] = s1;
            SetArg(argIndex, (IntPtr)(sizeof(float) * 2), (IntPtr)pBuffer);
        }

        public void SetFloat4Arg(int argIndex, float s0, float s1, float s2, float s3)
        {
            float* pBuffer = stackalloc float[4];
            pBuffer[0] = s0;
            pBuffer[1] = s1;
            pBuffer[2] = s2;
            pBuffer[3] = s3;
            SetArg(argIndex, (IntPtr)(sizeof(float) * 4), (IntPtr)pBuffer);
        }

        #endregion

#endif
        public static implicit operator IntPtr( Kernel k )
        {
            return k.KernelID;
        }

        #region IPropertyContainer Members

        public IntPtr GetPropertySize( uint key )
        {
            IntPtr size;
            ErrorCode result;

            result = (ErrorCode)OpenCL.GetKernelInfo( KernelID, key, IntPtr.Zero, null, out size );
            if( result!=ErrorCode.SUCCESS )
                throw new OpenCLException( "Unable to get kernel info for kernel "+KernelID, result);
            return size;
        }

        public void ReadProperty( uint key, IntPtr keyLength, void* pBuffer )
        {
            IntPtr size;
            ErrorCode result;

            result = (ErrorCode)OpenCL.GetKernelInfo( KernelID, (uint)key, keyLength, pBuffer, out size );
            if( result!=(int)ErrorCode.SUCCESS )
                throw new OpenCLException( "Unable to get kernel info for kernel "+KernelID, result);
        }

        #endregion
    }
}
