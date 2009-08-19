using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Text;

namespace OpenCLNet
{
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

        public OpenCLAPI CL { get { return Context.CL; } }

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

        public void SetArg( uint argIndex, IntPtr argSize, IntPtr argValue )
        {
            ErrorCode result;

            result = (ErrorCode)CL.SetKernelArg( KernelID, argIndex, argSize, argValue.ToPointer() );
            if( result!=ErrorCode.SUCCESS )
                throw new OpenCLException( "SetArg failed with error code "+result );
        }

        public void SetArg( uint argIndex, int c )
        {
            ErrorCode result;

            result = (ErrorCode)CL.SetKernelArg( KernelID, argIndex, new IntPtr( sizeof( int ) ), &c );
            if( result!=ErrorCode.SUCCESS )
                throw new OpenCLException( "SetArg failed with error code "+result );
        }

        public void SetArg( uint argIndex, long c )
        {
            ErrorCode result;

            result = (ErrorCode)CL.SetKernelArg( KernelID, argIndex, new IntPtr( sizeof( long ) ), &c );
            if( result!=ErrorCode.SUCCESS )
                throw new OpenCLException( "SetArg failed with error code "+result );
        }

        public void SetArg( uint argIndex, float c )
        {
            ErrorCode result;

            result = (ErrorCode)CL.SetKernelArg( KernelID, argIndex, new IntPtr( sizeof( float ) ), &c );
            if( result!=ErrorCode.SUCCESS )
                throw new OpenCLException( "SetArg failed with error code "+result );
        }

        public void SetArg( uint argIndex, double c )
        {
            ErrorCode result;

            result = (ErrorCode)CL.SetKernelArg( KernelID, argIndex, new IntPtr( sizeof( double ) ), &c );
            if( result!=ErrorCode.SUCCESS )
                throw new OpenCLException( "SetArg failed with error code "+result );
        }

        public void SetArg( uint argIndex, IntPtr c )
        {
            ErrorCode result;

            result = (ErrorCode)CL.SetKernelArg( KernelID, argIndex, new IntPtr( sizeof( IntPtr ) ), &c );
            if( result!=ErrorCode.SUCCESS )
                throw new OpenCLException( "SetArg failed with error code "+result );
        }

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
                throw new OpenCLException( "Unable to get kernel info for kernel "+KernelID );
            return size;
        }

        public void ReadProperty( uint key, IntPtr keyLength, void* pBuffer )
        {
            IntPtr size;
            ErrorCode result;

            result = (ErrorCode)CL.GetKernelInfo( KernelID, (uint)key, keyLength, pBuffer, out size );
            if( result!=(int)ErrorCode.SUCCESS )
                throw new OpenCLException( "Unable to get kernel info for kernel "+KernelID );
        }

        #endregion
    }
}
