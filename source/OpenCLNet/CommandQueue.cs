using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Text;

namespace OpenCLNet
{
    unsafe public class CommandQueue
    {
        public IntPtr CommandQueueID { get; private set; }
        public Context Context { get; private set; }
        public Device Device { get; private set; }
        public uint ReferenceCount { get { return 0; } }
        public CommandQueueProperties Properties { get { return (CommandQueueProperties)0; } }
        public OpenCLAPI CL { get { return Context.CL; } }

        // Track whether Dispose has been called.
        private bool disposed = false;

        #region Construction / Destruction

        internal CommandQueue( Context context, Device device, IntPtr commandQueueID )
        {
            Context = context;
            Device = device;
            CommandQueueID = commandQueueID;
        }

        // Use C# destructor syntax for finalization code.
        // This destructor will run only if the Dispose method
        // does not get called.
        // It gives your base class the opportunity to finalize.
        // Do not provide destructors in types derived from this class.
        ~CommandQueue()
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
                CL.ReleaseCommandQueue( CommandQueueID );
                CommandQueueID = IntPtr.Zero;

                // Note disposing has been done.
                disposed = true;
            }
        }

        #endregion

        public void EnqueueWriteBuffer( Mem buffer, bool blockingWrite, IntPtr offset, IntPtr cb, IntPtr ptr, int numEventsInWaitList, IntPtr[] eventWaitList, out IntPtr _event )
        {
            ErrorCode result;

            result = CL.EnqueueWriteBuffer( CommandQueueID,
                buffer.MemID,
                (uint)(blockingWrite?Bool.TRUE:Bool.FALSE),
                offset,
                cb,
                ptr.ToPointer(),
                (uint)numEventsInWaitList,
                eventWaitList,
                out _event );
            if( result!=ErrorCode.SUCCESS )
                throw new OpenCLException( "EnqueueReadBuffer failed with error code "+result );
        }

        public void EnqueueReadBuffer( Mem buffer, bool blockingRead, IntPtr offset, IntPtr cb, IntPtr ptr, int numEventsInWaitList, IntPtr[] eventWaitList, out IntPtr _event )
        {
            ErrorCode result;

            result = CL.EnqueueReadBuffer( CommandQueueID,
                buffer.MemID,
                (uint)(blockingRead?Bool.TRUE:Bool.FALSE),
                offset,
                cb,
                ptr.ToPointer(),
                (uint)numEventsInWaitList,
                eventWaitList,
                out _event );
            if( result!=ErrorCode.SUCCESS )
                throw new OpenCLException( "EnqueueReadBuffer failed with error code "+result );
        }

        public void EnqueueCopyBuffer( Mem src_buffer, Mem dst_buffer, IntPtr src_offset, IntPtr dst_offset, IntPtr cb, int num_events_in_wait_list, IntPtr[] eventWaitList, out IntPtr _event )
        {
            ErrorCode result;

            result = CL.EnqueueCopyBuffer( CommandQueueID,
                src_buffer.MemID,
                dst_buffer.MemID,
                src_offset,
                dst_offset,
                cb,
                (uint)num_events_in_wait_list,
                eventWaitList,
                out _event );
            if( result!=ErrorCode.SUCCESS )
                throw new OpenCLException( "EnqueueCopyBuffer failed with error code "+result );
        }

        public void EnqueueReadImage( Mem image, bool blockingRead, IntPtr[] origin, IntPtr[] region, IntPtr row_pitch, IntPtr slice_pitch, void* ptr, int num_events_in_wait_list, IntPtr[] eventWaitList, out IntPtr _event )
        {
            ErrorCode result;

            result = CL.EnqueueReadImage( CommandQueueID,
                image.MemID,
                (uint)(blockingRead?Bool.TRUE:Bool.FALSE),
                origin,
                region,
                row_pitch,
                slice_pitch,
                ptr,
                (uint)num_events_in_wait_list,
                eventWaitList,
                out _event );
            if( result!=ErrorCode.SUCCESS )
                throw new OpenCLException( "EnqueueReadImage failed with error code "+result );
        }

        public void EnqueueWriteImage( Mem image, bool blockingWrite, IntPtr[] origin, IntPtr[] region, IntPtr input_row_pitch, IntPtr input_slice_pitch, void* ptr, int num_events_in_wait_list, IntPtr[] eventWaitList, out IntPtr _event )
        {
            ErrorCode result;

            result = CL.EnqueueWriteImage( CommandQueueID,
                image.MemID,
                (uint)(blockingWrite?Bool.TRUE:Bool.FALSE),
                origin,
                region,
                input_row_pitch,
                input_slice_pitch,
                ptr,
                (uint)num_events_in_wait_list,
                eventWaitList,
                out _event );
            if( result!=ErrorCode.SUCCESS )
                throw new OpenCLException( "EnqueueWriteImage failed with error code "+result );
        }

        public void EnqueueCopyImage( Mem src_image, Mem dst_image, IntPtr[] src_origin, IntPtr[] dst_origin, IntPtr[] region, int num_events_in_wait_list, IntPtr[] eventWaitList, out IntPtr _event )
        {
            ErrorCode result;

            result = CL.EnqueueCopyImage( CommandQueueID,
                src_image,
                dst_image,
                src_origin,
                dst_origin,
                region,
                (uint)num_events_in_wait_list,
                eventWaitList,
                out _event );
            if( result!=ErrorCode.SUCCESS )
                throw new OpenCLException( "EnqueueCopyImage failed with error code "+result );
        }

        public void EnqueueCopyImageToBuffer( Mem src_image, Mem dst_buffer, IntPtr[] src_origin, IntPtr[] region, IntPtr dst_offset, int num_events_in_wait_list, IntPtr[] eventWaitList, out IntPtr _event )
        {
            ErrorCode result;

            result = CL.EnqueueCopyImageToBuffer( CommandQueueID,
                src_image,
                dst_buffer,
                src_origin,
                region,
                dst_offset,
                (uint)num_events_in_wait_list,
                eventWaitList,
                out _event );
            if( result!=ErrorCode.SUCCESS )
                throw new OpenCLException( "EnqueueCopyImageToBuffer failed with error code "+result );
        }

        public void EnqueueCopyBufferToImage( Mem src_buffer, Mem dst_image, IntPtr src_offset, IntPtr[] dst_origin, IntPtr[] region, int num_events_in_wait_list, IntPtr[] eventWaitList, out IntPtr _event )
        {
            ErrorCode result;

            result = CL.EnqueueCopyBufferToImage( CommandQueueID,
                src_buffer,
                dst_image,
                src_offset,
                dst_origin,
                region,
                (uint) num_events_in_wait_list,
                eventWaitList,
                out _event );
            if( result!=ErrorCode.SUCCESS )
                throw new OpenCLException( "EnqueueCopyBufferToImage failed with error code "+result );
        }

        public IntPtr EnqueueMapBuffer( Mem buffer, bool blockingMap, MapFlags map_flags, IntPtr offset, IntPtr cb, int num_events_in_wait_list, IntPtr[] eventWaitList, out IntPtr _event, out ErrorCode result )
        {
            IntPtr ptr;

            ptr = (IntPtr)CL.EnqueueMapBuffer( CommandQueueID,
                buffer,
                (uint)(blockingMap?Bool.TRUE:Bool.FALSE),
                (ulong)map_flags,
                offset,
                cb,
                (uint)num_events_in_wait_list,
                eventWaitList,
                out _event,
                out result );
            if( result!=ErrorCode.SUCCESS )
                throw new OpenCLException( "EnqueueMapBuffer failed with error code "+result );
            return ptr;
        }

        public IntPtr EnqueueMapImage( Mem image, bool blockingMap, MapFlags map_flags, IntPtr[] origin, IntPtr[] region, IntPtr image_row_pitch, IntPtr image_slice_pitch, int num_events_in_wait_list, IntPtr[] eventWaitList, out IntPtr _event, out ErrorCode result )
        {
            IntPtr ptr;

            ptr = (IntPtr)CL.EnqueueMapImage( CommandQueueID,
                image,
                (uint)(blockingMap?Bool.TRUE:Bool.FALSE),
                (ulong)map_flags,
                origin,
                region,
                image_row_pitch,
                image_slice_pitch,
                (uint)num_events_in_wait_list,
                eventWaitList,
                out _event,
                out result );
            if( result!=ErrorCode.SUCCESS )
                throw new OpenCLException( "EnqueueMapImage failed with error code "+result );
            return ptr;
        }

        public void EnqueueUnmapMemObject( Mem memobj, IntPtr mapped_ptr, int num_events_in_wait_list, IntPtr[] eventWaitList, out IntPtr _event )
        {
            ErrorCode result;

            result = CL.EnqueueUnmapMemObject( CommandQueueID,
                memobj,
                mapped_ptr.ToPointer(),
                (uint)num_events_in_wait_list,
                eventWaitList,
                out _event );
            if( result!=ErrorCode.SUCCESS )
                throw new OpenCLException( "EnqueueUnmapMemObject failed with error code "+result );
        }

        public void EnqueueNDRangeKernel( Kernel kernel, uint workDim, IntPtr[] globalWorkOffset, IntPtr[] globalWorkSize, IntPtr[] localWorkSize, uint numEventsInWaitList, IntPtr[] eventWaitList, out IntPtr _event )
        {
            ErrorCode result;

            result = CL.EnqueueNDRangeKernel( CommandQueueID, kernel.KernelID, workDim, globalWorkOffset, globalWorkSize, localWorkSize, numEventsInWaitList, eventWaitList, out _event );
            if( result!=ErrorCode.SUCCESS )
                throw new OpenCLException( "EnqueueNDRangeKernel failed with error code "+result );
        }

        public void EnqueueTask( Kernel kernel, int numEventsInWaitList, IntPtr[] eventWaitList, ref IntPtr _event )
        {
            ErrorCode result;

            result = (ErrorCode)CL.EnqueueTask( CommandQueueID,
                kernel.KernelID,
                (uint)numEventsInWaitList,
                eventWaitList,
                out _event );
            if( result!=ErrorCode.SUCCESS )
                throw new OpenCLException( "EnqueueTask failed with error code "+result );
        }

        public void SetProperty( CommandQueueProperties properties, bool enable, out CommandQueueProperties oldProperties )
        {
            ErrorCode result;
            ulong returnedProperties = 0;

            result = (ErrorCode)CL.SetCommandQueueProperty( CommandQueueID,
                (ulong)properties,
                enable,
                out returnedProperties );
            if( result!=ErrorCode.SUCCESS )
                throw new OpenCLException( "SetCommandQueueProperty failed with error code "+result );
            oldProperties = (CommandQueueProperties)returnedProperties;
        }

        public static implicit operator IntPtr( CommandQueue cq )
        {
            return cq.CommandQueueID;
        }
    }
}
