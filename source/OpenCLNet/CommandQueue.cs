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
    /// The CommandQueue class wraps an OpenCL command queue reference.
    /// 
    /// This class contains methods that correspond to all OpenCL functions that take
    /// a command queue as their first parameter. Most notably, all the Enqueue() functions.
    /// In effect, it makes this class into the workhorse of most OpenCL applications.
    /// </summary>
    unsafe public class CommandQueue : IDisposable
    {
        public IntPtr CommandQueueID { get; private set; }
        public Context Context { get; private set; }
        public Device Device { get; private set; }
        public uint ReferenceCount { get { return 0; } }
        public CommandQueueProperties Properties { get { return (CommandQueueProperties)0; } }

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
                OpenCL.ReleaseCommandQueue( CommandQueueID );
                CommandQueueID = IntPtr.Zero;

                // Note disposing has been done.
                disposed = true;
            }
        }

        #endregion

        // Enqueue methods follow. Typically, each will have 3 versions.
        // One which takes an event wait list and and event output
        // One which takes an event wait list
        // and one which takes neither

        #region EnqueueWriteBuffer

        /// <summary>
        /// Enqueues a command to write data to a buffer object identified by buffer from host memory identified by ptr.
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="blockingWrite"></param>
        /// <param name="offset"></param>
        /// <param name="cb"></param>
        /// <param name="ptr"></param>
        /// <param name="event_wait_list"></param>
        /// <param name="InteropTools.ConvertEventsToEventIDs(event_wait_list)"></param>
        /// <param name="_event"></param>
        public void EnqueueWriteBuffer(Mem buffer, bool blockingWrite, IntPtr offset, IntPtr cb, IntPtr ptr, int numEventsInWaitList, Event[] event_wait_list, out Event _event)
        {
            ErrorCode result;
            IntPtr tmpEvent;

            result = OpenCL.EnqueueWriteBuffer(CommandQueueID,
                buffer.MemID,
                (uint)(blockingWrite ? Bool.TRUE : Bool.FALSE),
                offset,
                cb,
                ptr.ToPointer(),
                (uint)numEventsInWaitList,
                InteropTools.ConvertEventsToEventIDs(event_wait_list),
                &tmpEvent);
            _event = new Event( Context, tmpEvent );
            if (result != ErrorCode.SUCCESS)
                throw new OpenCLException("EnqueueReadBuffer failed with error code " + result, result);
        }
        public void EnqueueWriteBuffer(Mem buffer, bool blockingWrite, IntPtr offset, IntPtr cb, IntPtr ptr, int numEventsInWaitList, Event[] event_wait_list)
        {
            ErrorCode result;

            result = OpenCL.EnqueueWriteBuffer(CommandQueueID,
                buffer.MemID,
                (uint)(blockingWrite ? Bool.TRUE : Bool.FALSE),
                offset,
                cb,
                ptr.ToPointer(),
                (uint)numEventsInWaitList,
                InteropTools.ConvertEventsToEventIDs(event_wait_list),
                null);
            if (result != ErrorCode.SUCCESS)
                throw new OpenCLException("EnqueueReadBuffer failed with error code " + result, result);
        }
        public void EnqueueWriteBuffer(Mem buffer, bool blockingWrite, IntPtr offset, IntPtr cb, IntPtr ptr)
        {
            ErrorCode result;

            result = OpenCL.EnqueueWriteBuffer(CommandQueueID,
                buffer.MemID,
                (uint)(blockingWrite ? Bool.TRUE : Bool.FALSE),
                offset,
                cb,
                ptr.ToPointer(),
                (uint)0,
                null,
                null);
            if (result != ErrorCode.SUCCESS)
                throw new OpenCLException("EnqueueReadBuffer failed with error code " + result, result);
        }
        #endregion

        #region EnqueueReadBuffer

        /// <summary>
        /// Enqueues a command to read data from a buffer object identified by buffer to host memory identified by ptr.
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="blockingRead"></param>
        /// <param name="offset"></param>
        /// <param name="cb"></param>
        /// <param name="ptr"></param>
        /// <param name="numEventsInWaitList"></param>
        /// <param name="event_wait_list"></param>
        /// <param name="_event"></param>
        public void EnqueueReadBuffer(Mem buffer, bool blockingRead, IntPtr offset, IntPtr cb, IntPtr ptr, int numEventsInWaitList, Event[] event_wait_list, out Event _event)
        {
            ErrorCode result;
            IntPtr tmpEvent;

            result = OpenCL.EnqueueReadBuffer(CommandQueueID,
                buffer.MemID,
                (uint)(blockingRead ? Bool.TRUE : Bool.FALSE),
                offset,
                cb,
                ptr.ToPointer(),
                (uint)numEventsInWaitList,
                InteropTools.ConvertEventsToEventIDs(event_wait_list),
                &tmpEvent);
            _event = new Event(Context, tmpEvent); 
            if (result != ErrorCode.SUCCESS)
                throw new OpenCLException("EnqueueReadBuffer failed with error code " + result, result);
        }

        public void EnqueueReadBuffer(Mem buffer, bool blockingRead, IntPtr offset, IntPtr cb, IntPtr ptr, int numEventsInWaitList, Event[] event_wait_list )
        {
            ErrorCode result;

            result = OpenCL.EnqueueReadBuffer(CommandQueueID,
                buffer.MemID,
                (uint)(blockingRead ? Bool.TRUE : Bool.FALSE),
                offset,
                cb,
                ptr.ToPointer(),
                (uint)numEventsInWaitList,
                InteropTools.ConvertEventsToEventIDs(event_wait_list),
                null);
            if (result != ErrorCode.SUCCESS)
                throw new OpenCLException("EnqueueReadBuffer failed with error code " + result, result);
        }

        public void EnqueueReadBuffer(Mem buffer, bool blockingRead, IntPtr offset, IntPtr cb, IntPtr ptr)
        {
            ErrorCode result;

            result = OpenCL.EnqueueReadBuffer(CommandQueueID,
                buffer.MemID,
                (uint)(blockingRead ? Bool.TRUE : Bool.FALSE),
                offset,
                cb,
                ptr.ToPointer(),
                (uint)0,
                null,
                null);
            if (result != ErrorCode.SUCCESS)
                throw new OpenCLException("EnqueueReadBuffer failed with error code " + result, result);
        }

        #endregion

        #region EnqueueCopyBuffer

        /// <summary>
        /// Enqueues a command to copy a buffer object identified by src_buffer to another buffer object identified by dst_buffer.
        /// </summary>
        /// <param name="src_buffer"></param>
        /// <param name="dst_buffer"></param>
        /// <param name="src_offset"></param>
        /// <param name="dst_offset"></param>
        /// <param name="cb"></param>
        /// <param name="num_events_in_wait_list"></param>
        /// <param name="event_wait_list"></param>
        /// <param name="_event"></param>
        public void EnqueueCopyBuffer(Mem src_buffer, Mem dst_buffer, IntPtr src_offset, IntPtr dst_offset, IntPtr cb, int num_events_in_wait_list, Event[] event_wait_list, out Event _event)
        {
            ErrorCode result;
            IntPtr tmpEvent;

            result = OpenCL.EnqueueCopyBuffer(CommandQueueID,
                src_buffer.MemID,
                dst_buffer.MemID,
                src_offset,
                dst_offset,
                cb,
                (uint)num_events_in_wait_list,
                InteropTools.ConvertEventsToEventIDs(event_wait_list),
                &tmpEvent);
            _event = new Event(Context, tmpEvent);
            if (result != ErrorCode.SUCCESS)
                throw new OpenCLException("EnqueueCopyBuffer failed with error code " + result, result);
        }

        public void EnqueueCopyBuffer(Mem src_buffer, Mem dst_buffer, IntPtr src_offset, IntPtr dst_offset, IntPtr cb, int num_events_in_wait_list, Event[] event_wait_list)
        {
            ErrorCode result;

            result = OpenCL.EnqueueCopyBuffer(CommandQueueID,
                src_buffer.MemID,
                dst_buffer.MemID,
                src_offset,
                dst_offset,
                cb,
                (uint)num_events_in_wait_list,
                InteropTools.ConvertEventsToEventIDs(event_wait_list),
                null);
            if (result != ErrorCode.SUCCESS)
                throw new OpenCLException("EnqueueCopyBuffer failed with error code " + result, result);
        }

        public void EnqueueCopyBuffer(Mem src_buffer, Mem dst_buffer, IntPtr src_offset, IntPtr dst_offset, IntPtr cb)
        {
            ErrorCode result;

            result = OpenCL.EnqueueCopyBuffer(CommandQueueID,
                src_buffer.MemID,
                dst_buffer.MemID,
                src_offset,
                dst_offset,
                cb,
                (uint)0,
                null,
                null);
            if (result != ErrorCode.SUCCESS)
                throw new OpenCLException("EnqueueCopyBuffer failed with error code " + result, result);
        }

        #endregion

        #region EnqueueReadImage

        public void EnqueueReadImage(Mem image, bool blockingRead, IntPtr[] origin, IntPtr[] region, IntPtr row_pitch, IntPtr slice_pitch, void* ptr, int num_events_in_wait_list, Event[] event_wait_list, out Event _event)
        {
            ErrorCode result;
            IntPtr tmpEvent;

            result = OpenCL.EnqueueReadImage(CommandQueueID,
                image.MemID,
                (uint)(blockingRead ? Bool.TRUE : Bool.FALSE),
                origin,
                region,
                row_pitch,
                slice_pitch,
                ptr,
                (uint)num_events_in_wait_list,
                InteropTools.ConvertEventsToEventIDs(event_wait_list),
                &tmpEvent);
            _event = new Event(Context, tmpEvent);
            if (result != ErrorCode.SUCCESS)
                throw new OpenCLException("EnqueueReadImage failed with error code " + result, result);
        }

        public void EnqueueReadImage(Mem image, bool blockingRead, IntPtr[] origin, IntPtr[] region, IntPtr row_pitch, IntPtr slice_pitch, void* ptr, int num_events_in_wait_list, Event[] event_wait_list)
        {
            ErrorCode result;

            result = OpenCL.EnqueueReadImage(CommandQueueID,
                image.MemID,
                (uint)(blockingRead ? Bool.TRUE : Bool.FALSE),
                origin,
                region,
                row_pitch,
                slice_pitch,
                ptr,
                (uint)num_events_in_wait_list,
                InteropTools.ConvertEventsToEventIDs(event_wait_list),
                null);
            if (result != ErrorCode.SUCCESS)
                throw new OpenCLException("EnqueueReadImage failed with error code " + result, result);
        }

        public void EnqueueReadImage(Mem image, bool blockingRead, IntPtr[] origin, IntPtr[] region, IntPtr row_pitch, IntPtr slice_pitch, void* ptr)
        {
            ErrorCode result;

            result = OpenCL.EnqueueReadImage(CommandQueueID,
                image.MemID,
                (uint)(blockingRead ? Bool.TRUE : Bool.FALSE),
                origin,
                region,
                row_pitch,
                slice_pitch,
                ptr,
                (uint)0,
                null,
                null);
            if (result != ErrorCode.SUCCESS)
                throw new OpenCLException("EnqueueReadImage failed with error code " + result, result);
        }

        #endregion

        #region EnqueueWriteImage

        public void EnqueueWriteImage(Mem image, bool blockingWrite, IntPtr[] origin, IntPtr[] region, IntPtr input_row_pitch, IntPtr input_slice_pitch, void* ptr, int num_events_in_wait_list, Event[] event_wait_list, out Event _event)
        {
            ErrorCode result;
            IntPtr tmpEvent;

            result = OpenCL.EnqueueWriteImage(CommandQueueID,
                image.MemID,
                (uint)(blockingWrite ? Bool.TRUE : Bool.FALSE),
                origin,
                region,
                input_row_pitch,
                input_slice_pitch,
                ptr,
                (uint)num_events_in_wait_list,
                InteropTools.ConvertEventsToEventIDs(event_wait_list),
                &tmpEvent);
            _event = new Event(Context, tmpEvent);
            if (result != ErrorCode.SUCCESS)
                throw new OpenCLException("EnqueueWriteImage failed with error code " + result, result);
        }

        public void EnqueueWriteImage(Mem image, bool blockingWrite, IntPtr[] origin, IntPtr[] region, IntPtr input_row_pitch, IntPtr input_slice_pitch, void* ptr, int num_events_in_wait_list, Event[] event_wait_list)
        {
            ErrorCode result;

            result = OpenCL.EnqueueWriteImage(CommandQueueID,
                image.MemID,
                (uint)(blockingWrite ? Bool.TRUE : Bool.FALSE),
                origin,
                region,
                input_row_pitch,
                input_slice_pitch,
                ptr,
                (uint)num_events_in_wait_list,
                InteropTools.ConvertEventsToEventIDs(event_wait_list),
                null);
            if (result != ErrorCode.SUCCESS)
                throw new OpenCLException("EnqueueWriteImage failed with error code " + result, result);
        }

        public void EnqueueWriteImage(Mem image, bool blockingWrite, IntPtr[] origin, IntPtr[] region, IntPtr input_row_pitch, IntPtr input_slice_pitch, void* ptr)
        {
            ErrorCode result;

            result = OpenCL.EnqueueWriteImage(CommandQueueID,
                image.MemID,
                (uint)(blockingWrite ? Bool.TRUE : Bool.FALSE),
                origin,
                region,
                input_row_pitch,
                input_slice_pitch,
                ptr,
                (uint)0,
                null,
                null);
            if (result != ErrorCode.SUCCESS)
                throw new OpenCLException("EnqueueWriteImage failed with error code " + result, result);
        }

        #endregion

        #region EnqueueCopyImage

        public void EnqueueCopyImage(Mem src_image, Mem dst_image, IntPtr[] src_origin, IntPtr[] dst_origin, IntPtr[] region, int num_events_in_wait_list, Event[] event_wait_list, out Event _event)
        {
            ErrorCode result;
            IntPtr tmpEvent;

            result = OpenCL.EnqueueCopyImage(CommandQueueID,
                src_image.MemID,
                dst_image.MemID,
                src_origin,
                dst_origin,
                region,
                (uint)num_events_in_wait_list,
                InteropTools.ConvertEventsToEventIDs(event_wait_list),
                &tmpEvent);
            _event = new Event(Context, tmpEvent);
            if (result != ErrorCode.SUCCESS)
                throw new OpenCLException("EnqueueCopyImage failed with error code " + result, result);
        }

        public void EnqueueCopyImage(Mem src_image, Mem dst_image, IntPtr[] src_origin, IntPtr[] dst_origin, IntPtr[] region, int num_events_in_wait_list, Event[] event_wait_list)
        {
            ErrorCode result;

            result = OpenCL.EnqueueCopyImage(CommandQueueID,
                src_image.MemID,
                dst_image.MemID,
                src_origin,
                dst_origin,
                region,
                (uint)num_events_in_wait_list,
                InteropTools.ConvertEventsToEventIDs(event_wait_list),
                null);
            if (result != ErrorCode.SUCCESS)
                throw new OpenCLException("EnqueueCopyImage failed with error code " + result, result);
        }

        public void EnqueueCopyImage(Mem src_image, Mem dst_image, IntPtr[] src_origin, IntPtr[] dst_origin, IntPtr[] region)
        {
            ErrorCode result;

            result = OpenCL.EnqueueCopyImage(CommandQueueID,
                src_image.MemID,
                dst_image.MemID,
                src_origin,
                dst_origin,
                region,
                (uint)0,
                null,
                null);
            if (result != ErrorCode.SUCCESS)
                throw new OpenCLException("EnqueueCopyImage failed with error code " + result, result);
        }

        #endregion

        #region EnqueueCopyImageToBuffer

        public void EnqueueCopyImageToBuffer(Mem src_image, Mem dst_buffer, IntPtr[] src_origin, IntPtr[] region, IntPtr dst_offset, int num_events_in_wait_list, Event[] event_wait_list, out Event _event)
        {
            ErrorCode result;
            IntPtr tmpEvent;

            result = OpenCL.EnqueueCopyImageToBuffer(CommandQueueID,
                src_image.MemID,
                dst_buffer.MemID,
                src_origin,
                region,
                dst_offset,
                (uint)num_events_in_wait_list,
                InteropTools.ConvertEventsToEventIDs(event_wait_list),
                &tmpEvent);
            _event = new Event(Context, tmpEvent);
            if (result != ErrorCode.SUCCESS)
                throw new OpenCLException("EnqueueCopyImageToBuffer failed with error code " + result, result);
        }

        public void EnqueueCopyImageToBuffer(Mem src_image, Mem dst_buffer, IntPtr[] src_origin, IntPtr[] region, IntPtr dst_offset, int num_events_in_wait_list, Event[] event_wait_list)
        {
            ErrorCode result;

            result = OpenCL.EnqueueCopyImageToBuffer(CommandQueueID,
                src_image.MemID,
                dst_buffer.MemID,
                src_origin,
                region,
                dst_offset,
                (uint)num_events_in_wait_list,
                InteropTools.ConvertEventsToEventIDs(event_wait_list),
                null);
            if (result != ErrorCode.SUCCESS)
                throw new OpenCLException("EnqueueCopyImageToBuffer failed with error code " + result, result);
        }

        public void EnqueueCopyImageToBuffer(Mem src_image, Mem dst_buffer, IntPtr[] src_origin, IntPtr[] region, IntPtr dst_offset)
        {
            ErrorCode result;

            result = OpenCL.EnqueueCopyImageToBuffer(CommandQueueID,
                src_image.MemID,
                dst_buffer.MemID,
                src_origin,
                region,
                dst_offset,
                (uint)0,
                null,
                null);
            if (result != ErrorCode.SUCCESS)
                throw new OpenCLException("EnqueueCopyImageToBuffer failed with error code " + result, result);
        }

        #endregion

        #region EnqueueCopyBufferToImage

        public void EnqueueCopyBufferToImage(Mem src_buffer, Mem dst_image, IntPtr src_offset, IntPtr[] dst_origin, IntPtr[] region, int num_events_in_wait_list, Event[] event_wait_list, out Event _event)
        {
            ErrorCode result;
            IntPtr tmpEvent;

            result = OpenCL.EnqueueCopyBufferToImage(CommandQueueID,
                src_buffer.MemID,
                dst_image.MemID,
                src_offset,
                dst_origin,
                region,
                (uint)num_events_in_wait_list,
                InteropTools.ConvertEventsToEventIDs(event_wait_list),
                &tmpEvent);
            _event = new Event(Context, tmpEvent);
            if (result != ErrorCode.SUCCESS)
                throw new OpenCLException("EnqueueCopyBufferToImage failed with error code " + result, result);
        }

        public void EnqueueCopyBufferToImage(Mem src_buffer, Mem dst_image, IntPtr src_offset, IntPtr[] dst_origin, IntPtr[] region, int num_events_in_wait_list, Event[] event_wait_list)
        {
            ErrorCode result;

            result = OpenCL.EnqueueCopyBufferToImage(CommandQueueID,
                src_buffer.MemID,
                dst_image.MemID,
                src_offset,
                dst_origin,
                region,
                (uint)num_events_in_wait_list,
                InteropTools.ConvertEventsToEventIDs(event_wait_list),
                null);
            if (result != ErrorCode.SUCCESS)
                throw new OpenCLException("EnqueueCopyBufferToImage failed with error code " + result, result);
        }

        public void EnqueueCopyBufferToImage(Mem src_buffer, Mem dst_image, IntPtr src_offset, IntPtr[] dst_origin, IntPtr[] region)
        {
            ErrorCode result;

            result = OpenCL.EnqueueCopyBufferToImage(CommandQueueID,
                src_buffer.MemID,
                dst_image.MemID,
                src_offset,
                dst_origin,
                region,
                (uint)0,
                null,
                null);
            if (result != ErrorCode.SUCCESS)
                throw new OpenCLException("EnqueueCopyBufferToImage failed with error code " + result, result);
        }

        #endregion

        #region EnqueueMapBuffer

        public IntPtr EnqueueMapBuffer(Mem buffer, bool blockingMap, MapFlags map_flags, IntPtr offset, IntPtr cb, int num_events_in_wait_list, Event[] event_wait_list, out Event _event)
        {
            ErrorCode result;
            IntPtr ptr;
            IntPtr tmpEvent;

            ptr = (IntPtr)OpenCL.EnqueueMapBuffer(CommandQueueID,
                buffer.MemID,
                (uint)(blockingMap ? Bool.TRUE : Bool.FALSE),
                (ulong)map_flags,
                offset,
                cb,
                (uint)num_events_in_wait_list,
                InteropTools.ConvertEventsToEventIDs(event_wait_list),
                &tmpEvent,
                out result);
            _event = new Event(Context, tmpEvent); 

            if (result != ErrorCode.SUCCESS)
                throw new OpenCLException("EnqueueMapBuffer failed with error code " + result, result);
            return ptr;
        }

        public IntPtr EnqueueMapBuffer(Mem buffer, bool blockingMap, MapFlags map_flags, IntPtr offset, IntPtr cb, int num_events_in_wait_list, Event[] event_wait_list)
        {
            ErrorCode result;
            IntPtr ptr;

            ptr = (IntPtr)OpenCL.EnqueueMapBuffer(CommandQueueID,
                buffer.MemID,
                (uint)(blockingMap ? Bool.TRUE : Bool.FALSE),
                (ulong)map_flags,
                offset,
                cb,
                (uint)num_events_in_wait_list,
                InteropTools.ConvertEventsToEventIDs(event_wait_list),
                null,
                out result);

            if (result != ErrorCode.SUCCESS)
                throw new OpenCLException("EnqueueMapBuffer failed with error code " + result, result);
            return ptr;
        }

        public IntPtr EnqueueMapBuffer(Mem buffer, bool blockingMap, MapFlags map_flags, IntPtr offset, IntPtr cb)
        {
            ErrorCode result;
            IntPtr ptr;

            ptr = (IntPtr)OpenCL.EnqueueMapBuffer(CommandQueueID,
                buffer.MemID,
                (uint)(blockingMap ? Bool.TRUE : Bool.FALSE),
                (ulong)map_flags,
                offset,
                cb,
                (uint)0,
                null,
                null,
                out result);

            if (result != ErrorCode.SUCCESS)
                throw new OpenCLException("EnqueueMapBuffer failed with error code " + result, result);
            return ptr;
        }

        #endregion

        #region EnqueueMapImage

        /// <summary>
        /// Map the memory of a Mem object into host memory.
        /// This function must be used before native code accesses an area of memory that's under the control
        /// of OpenCL. This includes Mem objects allocated with MemFlags.USE_HOST_PTR, as results may be cached
        /// in another location. Mapping will ensure caches are synchronizatized.
        /// </summary>
        /// <param name="image">Mem object to map</param>
        /// <param name="blockingMap">Flag that indicates if the operation is synchronous or not</param>
        /// <param name="map_flags">Read/Write flags</param>
        /// <param name="origin">origin contains the x,y,z coordinates indicating the starting point to map</param>
        /// <param name="region">origin contains the width,height,depth coordinates indicating the size of the area to map</param>
        /// <param name="image_row_pitch"></param>
        /// <param name="image_slice_pitch"></param>
        /// <param name="num_events_in_wait_list"></param>
        /// <param name="event_wait_list"></param>
        /// <param name="_event"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public IntPtr EnqueueMapImage(Mem image, bool blockingMap, MapFlags map_flags, IntPtr[] origin, IntPtr[] region, out IntPtr image_row_pitch, out IntPtr image_slice_pitch, int num_events_in_wait_list, Event[] event_wait_list, out Event _event, out ErrorCode result)
        {
            IntPtr ptr;
            IntPtr tmpEvent;
            
            ptr = (IntPtr)OpenCL.EnqueueMapImage(CommandQueueID,
                image.MemID,
                (uint)(blockingMap ? Bool.TRUE : Bool.FALSE),
                (ulong)map_flags,
                origin,
                region,
                out image_row_pitch,
                out image_slice_pitch,
                (uint)num_events_in_wait_list,
                InteropTools.ConvertEventsToEventIDs(event_wait_list),
                &tmpEvent,
                out result);
            _event = new Event(Context, tmpEvent); 
            if (result != ErrorCode.SUCCESS)
                throw new OpenCLException("EnqueueMapImage failed with error code " + result, result);
            return ptr;
        }

        public IntPtr EnqueueMapImage(Mem image, bool blockingMap, MapFlags map_flags, IntPtr[] origin, IntPtr[] region, out IntPtr image_row_pitch, out IntPtr image_slice_pitch, int num_events_in_wait_list, Event[] event_wait_list, out ErrorCode result)
        {
            IntPtr ptr;

            ptr = (IntPtr)OpenCL.EnqueueMapImage(CommandQueueID,
                image.MemID,
                (uint)(blockingMap ? Bool.TRUE : Bool.FALSE),
                (ulong)map_flags,
                origin,
                region,
                out image_row_pitch,
                out image_slice_pitch,
                (uint)num_events_in_wait_list,
                InteropTools.ConvertEventsToEventIDs(event_wait_list),
                null,
                out result);
            if (result != ErrorCode.SUCCESS)
                throw new OpenCLException("EnqueueMapImage failed with error code " + result, result);
            return ptr;
        }

        public IntPtr EnqueueMapImage(Mem image, bool blockingMap, MapFlags map_flags, IntPtr[] origin, IntPtr[] region, out IntPtr image_row_pitch, out IntPtr image_slice_pitch, out ErrorCode result)
        {
            IntPtr ptr;

            ptr = (IntPtr)OpenCL.EnqueueMapImage(CommandQueueID,
                image.MemID,
                (uint)(blockingMap ? Bool.TRUE : Bool.FALSE),
                (ulong)map_flags,
                origin,
                region,
                out image_row_pitch,
                out image_slice_pitch,
                (uint)0,
                null,
                null,
                out result);
            if (result != ErrorCode.SUCCESS)
                throw new OpenCLException("EnqueueMapImage failed with error code " + result, result);
            return ptr;
        }

        #endregion

        #region EnqueueUnmapMemObject

        /// <summary>
        /// Unmap a previously mapped Mem object
        /// </summary>
        /// <param name="memobj"></param>
        /// <param name="mapped_ptr"></param>
        /// <param name="num_events_in_wait_list"></param>
        /// <param name="event_wait_list"></param>
        /// <param name="_event"></param>
        public void EnqueueUnmapMemObject(Mem memobj, IntPtr mapped_ptr, int num_events_in_wait_list, Event[] event_wait_list, out Event _event)
        {
            ErrorCode result;
            IntPtr tmpEvent;

            result = OpenCL.EnqueueUnmapMemObject(CommandQueueID,
                memobj.MemID,
                mapped_ptr.ToPointer(),
                (uint)num_events_in_wait_list,
                InteropTools.ConvertEventsToEventIDs(event_wait_list),
                &tmpEvent);
            _event = new Event(Context, tmpEvent);
            if (result != ErrorCode.SUCCESS)
                throw new OpenCLException("EnqueueUnmapMemObject failed with error code " + result, result);
        }

        public void EnqueueUnmapMemObject(Mem memobj, IntPtr mapped_ptr, int num_events_in_wait_list, Event[] event_wait_list)
        {
            ErrorCode result;

            result = OpenCL.EnqueueUnmapMemObject(CommandQueueID,
                memobj.MemID,
                mapped_ptr.ToPointer(),
                (uint)num_events_in_wait_list,
                InteropTools.ConvertEventsToEventIDs(event_wait_list),
                null);
            if (result != ErrorCode.SUCCESS)
                throw new OpenCLException("EnqueueUnmapMemObject failed with error code " + result, result);
        }

        public void EnqueueUnmapMemObject(Mem memobj, IntPtr mapped_ptr)
        {
            ErrorCode result;

            result = OpenCL.EnqueueUnmapMemObject(CommandQueueID,
                memobj.MemID,
                mapped_ptr.ToPointer(),
                (uint)0,
                null,
                null);
            if (result != ErrorCode.SUCCESS)
                throw new OpenCLException("EnqueueUnmapMemObject failed with error code " + result, result);
        }

        #endregion

        #region EnqueueNDRangeKernel

        /// <summary>
        /// Execute a parallel kernel.
        /// </summary>
        /// <param name="kernel"></param>
        /// <param name="workDim">The number of dimensions in the workspace(0-2), must correspond to the number of dimensions in the following arrays</param>
        /// <param name="globalWorkOffset">null in OpenCL 1.0, but will allow indices to start at non-0 locations</param>
        /// <param name="globalWorkSize">Index n of this array=the length of the n'th dimension of global work space</param>
        /// <param name="localWorkSize">Index n of this array=the length of the n'th dimension of local work space</param>
        /// <param name="numEventsInWaitList"></param>
        /// <param name="event_wait_list"></param>
        /// <param name="_event"></param>
        public void EnqueueNDRangeKernel(Kernel kernel, uint workDim, IntPtr[] globalWorkOffset, IntPtr[] globalWorkSize, IntPtr[] localWorkSize, uint numEventsInWaitList, Event[] event_wait_list, out Event _event)
        {
            ErrorCode result;
            IntPtr tmpEvent;

            result = OpenCL.EnqueueNDRangeKernel(CommandQueueID,
                kernel.KernelID,
                workDim,
                globalWorkOffset,
                globalWorkSize,
                localWorkSize,
                numEventsInWaitList,
                InteropTools.ConvertEventsToEventIDs(event_wait_list),
                &tmpEvent);
            _event = new Event(Context, tmpEvent);
            if (result != ErrorCode.SUCCESS)
                throw new OpenCLException("EnqueueNDRangeKernel failed with error code " + result, result);
        }

        public void EnqueueNDRangeKernel(Kernel kernel, uint workDim, IntPtr[] globalWorkOffset, IntPtr[] globalWorkSize, IntPtr[] localWorkSize, uint numEventsInWaitList, Event[] event_wait_list)
        {
            ErrorCode result;

            result = OpenCL.EnqueueNDRangeKernel(CommandQueueID,
                kernel.KernelID,
                workDim,
                globalWorkOffset,
                globalWorkSize,
                localWorkSize,
                numEventsInWaitList,
                InteropTools.ConvertEventsToEventIDs(event_wait_list),
                null);
            if (result != ErrorCode.SUCCESS)
                throw new OpenCLException("EnqueueNDRangeKernel failed with error code " + result, result);
        }

        public void EnqueueNDRangeKernel(Kernel kernel, uint workDim, IntPtr[] globalWorkOffset, IntPtr[] globalWorkSize, IntPtr[] localWorkSize)
        {
            ErrorCode result;

            result = OpenCL.EnqueueNDRangeKernel(CommandQueueID,
                kernel.KernelID,
                workDim,
                globalWorkOffset,
                globalWorkSize,
                localWorkSize,
                0,
                null,
                null);
            if (result != ErrorCode.SUCCESS)
                throw new OpenCLException("EnqueueNDRangeKernel failed with error code " + result, result);
        }

        #endregion

        #region EnqueueTask

        /// <summary>
        /// Execute a simple kernel
        /// </summary>
        /// <param name="kernel"></param>
        /// <param name="numEventsInWaitList"></param>
        /// <param name="event_wait_list"></param>
        /// <param name="_event"></param>
        public void EnqueueTask(Kernel kernel, int numEventsInWaitList, Event[] event_wait_list, out Event _event)
        {
            ErrorCode result;
            IntPtr tmpEvent;

            result = (ErrorCode)OpenCL.EnqueueTask(CommandQueueID,
                kernel.KernelID,
                (uint)numEventsInWaitList,
                InteropTools.ConvertEventsToEventIDs(event_wait_list),
                &tmpEvent);
            _event = new Event(Context, tmpEvent);
            if (result != ErrorCode.SUCCESS)
                throw new OpenCLException("EnqueueTask failed with error code " + result, result);
        }

        /// <summary>
        /// Execute a simple kernel
        /// </summary>
        /// <param name="kernel"></param>
        /// <param name="numEventsInWaitList"></param>
        /// <param name="event_wait_list"></param>
        public void EnqueueTask(Kernel kernel, int numEventsInWaitList, Event[] event_wait_list)
        {
            ErrorCode result;

            result = (ErrorCode)OpenCL.EnqueueTask(CommandQueueID,
                kernel.KernelID,
                (uint)numEventsInWaitList,
                InteropTools.ConvertEventsToEventIDs(event_wait_list),
                null);
            if (result != ErrorCode.SUCCESS)
                throw new OpenCLException("EnqueueTask failed with error code " + result, result);
        }

        /// <summary>
        /// Execute a simple kernel
        /// </summary>
        /// <param name="kernel"></param>
        public void EnqueueTask(Kernel kernel)
        {
            ErrorCode result;

            result = (ErrorCode)OpenCL.EnqueueTask(CommandQueueID,
                kernel.KernelID,
                (uint)0,
                null,
                null);
            if (result != ErrorCode.SUCCESS)
                throw new OpenCLException("EnqueueTask failed with error code " + result, result);
        }

        #endregion

        #region EnqueueNativeKernel

        /// <summary>
        /// Enquque a user function. This function is only supported if
        /// DeviceExecCapabilities.NATIVE_KERNEL set in Device.ExecutionCapabilities
        /// 
        /// NOTE: As the delegate is being passed to an unmanaged function,
        /// it is the caller's responsibility to maintain a reference to the
        /// delegate so it does not get garbage collected before the callback happens.
        /// Failure to do this will cause crashes.
        /// </summary>
        /// <param name="nativeKernel"></param>
        /// <param name="numEventsInWaitList"></param>
        /// <param name="event_wait_list"></param>
        /// <param name="_event"></param>
        public void EnqueueNativeKernel(NativeKernel nativeKernel, int numEventsInWaitList, Event[] event_wait_list, out Event _event)
        {
            ErrorCode result;
            IntPtr tmpEvent;
            
            result = OpenCL.EnqueueNativeKernel(CommandQueueID,
                nativeKernel,
                null,
                IntPtr.Zero,
                0,
                null,
                null,
                (uint)numEventsInWaitList,
                InteropTools.ConvertEventsToEventIDs(event_wait_list),
                &tmpEvent);
            if (result != ErrorCode.SUCCESS)
                throw new OpenCLException("EnqueueNativeKernel failed with error code " + result, result);
            _event = new Event(Context, tmpEvent);
        }

        /// <summary>
        /// Enquque a user function. This function is only supported if
        /// DeviceExecCapabilities.NATIVE_KERNEL set in Device.ExecutionCapabilities
        /// 
        /// NOTE: As the delegate is being passed to an unmanaged function,
        /// it is the caller's responsibility to maintain a reference to the
        /// delegate so it does not get garbage collected before the callback happens.
        /// Failure to do this will cause crashes.
        /// </summary>
        /// <param name="nativeKernel"></param>
        /// <param name="numEventsInWaitList"></param>
        /// <param name="event_wait_list"></param>
        public void EnqueueNativeKernel(NativeKernel nativeKernel, int numEventsInWaitList, Event[] event_wait_list)
        {
            ErrorCode result;

            result = OpenCL.EnqueueNativeKernel(CommandQueueID,
                nativeKernel,
                null,
                IntPtr.Zero,
                0,
                null,
                null,
                (uint)numEventsInWaitList,
                InteropTools.ConvertEventsToEventIDs( event_wait_list ),
                null);
            if (result != ErrorCode.SUCCESS)
                throw new OpenCLException("EnqueueNativeKernel failed with error code " + result, result);
        }

        /// <summary>
        /// Enquque a user function. This function is only supported if
        /// DeviceExecCapabilities.NATIVE_KERNEL set in Device.ExecutionCapabilities
        /// 
        /// NOTE: As the delegate is being passed to an unmanaged function,
        /// it is the caller's responsibility to maintain a reference to the
        /// delegate so it does not get garbage collected before the callback happens.
        /// Failure to do this will cause crashes.
        /// </summary>
        /// <param name="nativeKernel"></param>
        public void EnqueueNativeKernel(NativeKernel nativeKernel)
        {
            ErrorCode result;

            result = OpenCL.EnqueueNativeKernel(CommandQueueID,
                nativeKernel,
                null,
                IntPtr.Zero,
                0,
                null,
                null,
                0,
                null,
                null);
            if (result != ErrorCode.SUCCESS)
                throw new OpenCLException("EnqueueNativeKernel failed with error code " + result, result);
        }

        #endregion

        #region EnqueueAcquireGLObjects

        public void EnqueueAcquireGLObjects(int numObjects, Mem[] memObjects, int numEventsInWaitList, Event[] event_wait_list, out Event _event)
        {
            IntPtr tmpEvent;
            ErrorCode result;

            result = OpenCL.EnqueueAcquireGLObjects(CommandQueueID,
                (uint)numObjects,
                InteropTools.ConvertMemToMemIDs(memObjects),
                (uint)numEventsInWaitList,
                InteropTools.ConvertEventsToEventIDs(event_wait_list),
                &tmpEvent);
            if (result != ErrorCode.SUCCESS)
                throw new OpenCLException("EnqueueAcquireGLObjects failed with error code " + result, result);
            _event = new Event(Context, tmpEvent);
        }

        public void EnqueueAcquireGLObjects(int numObjects, Mem[] memObjects, int numEventsInWaitList, Event[] event_wait_list)
        {
            ErrorCode result;

            result = OpenCL.EnqueueAcquireGLObjects(CommandQueueID,
                (uint)numObjects,
                InteropTools.ConvertMemToMemIDs(memObjects),
                (uint)numEventsInWaitList,
                InteropTools.ConvertEventsToEventIDs(event_wait_list),
                null);
            if (result != ErrorCode.SUCCESS)
                throw new OpenCLException("EnqueueAcquireGLObjects failed with error code " + result, result);
        }

        public void EnqueueAcquireGLObjects(int numObjects, Mem[] memObjects)
        {
            ErrorCode result;

            result = OpenCL.EnqueueAcquireGLObjects(CommandQueueID,
                (uint)numObjects,
                InteropTools.ConvertMemToMemIDs(memObjects),
                (uint)0,
                null,
                null);
            if (result != ErrorCode.SUCCESS)
                throw new OpenCLException("EnqueueAcquireGLObjects failed with error code " + result, result);
        }

        #endregion

        #region EnqueueReleaseGLObjects

        public void EnqueueReleaseGLObjects(int numObjects, Mem[] memObjects, int numEventsInWaitList, Event[] event_wait_list, out Event _event)
        {
            IntPtr tmpEvent;
            ErrorCode result;

            result = OpenCL.EnqueueReleaseGLObjects(CommandQueueID,
                (uint)numObjects,
                InteropTools.ConvertMemToMemIDs(memObjects),
                (uint)numEventsInWaitList,
                InteropTools.ConvertEventsToEventIDs(event_wait_list),
                &tmpEvent);
            if (result != ErrorCode.SUCCESS)
                throw new OpenCLException("EnqueueReleaseGLObjects failed with error code " + result, result);
            _event = new Event(Context, tmpEvent);
        }

        public void EnqueueReleaseGLObjects(int numObjects, Mem[] memObjects, int numEventsInWaitList, Event[] event_wait_list)
        {
            ErrorCode result;

            result = OpenCL.EnqueueReleaseGLObjects(CommandQueueID,
                (uint)numObjects,
                InteropTools.ConvertMemToMemIDs(memObjects),
                (uint)numEventsInWaitList,
                InteropTools.ConvertEventsToEventIDs(event_wait_list),
                null);
            if (result != ErrorCode.SUCCESS)
                throw new OpenCLException("EnqueueReleaseGLObjects failed with error code " + result, result);
        }

        public void EnqueueReleaseGLObjects(int numObjects, Mem[] memObjects)
        {
            ErrorCode result;

            result = OpenCL.EnqueueReleaseGLObjects(CommandQueueID,
                (uint)numObjects,
                InteropTools.ConvertMemToMemIDs(memObjects),
                (uint)0,
                null,
                null);
            if (result != ErrorCode.SUCCESS)
                throw new OpenCLException("EnqueueReleaseGLObjects failed with error code " + result, result);
        }

        #endregion

        #region EnqueueMarker

        public void EnqueueMarker( out Event _event )
        {
            IntPtr tmpEvent;

            OpenCL.EnqueueMarker(CommandQueueID, &tmpEvent);
            _event = new Event(Context, tmpEvent);
        }

        #endregion

        #region EnqueueWaitForEvents

        public void EnqueueWaitForEvents( int num_events, Event[] _event_list )
        {
            OpenCL.EnqueueWaitForEvents( CommandQueueID, (uint)num_events, InteropTools.ConvertEventsToEventIDs(_event_list) );
        }

        #endregion

        #region EnqueueBarrier

        public void EnqueueBarrier()
        {
            OpenCL.EnqueueBarrier(CommandQueueID);
        }

        #endregion

        #region Flush

        public void Flush()
        {
            OpenCL.Flush(CommandQueueID);
        }

        #endregion

        #region Finish

        public void Finish()
        {
            OpenCL.Finish(CommandQueueID);
        }

        #endregion

        [Obsolete("Function deprecated in OpenCL 1.1 due to being inherently unsafe", false)]
        public void SetProperty(CommandQueueProperties properties, bool enable, out CommandQueueProperties oldProperties)
        {
            ErrorCode result;
            ulong returnedProperties = 0;
#pragma warning disable 618
            result = (ErrorCode)OpenCL.SetCommandQueueProperty( CommandQueueID,
                (ulong)properties,
                enable,
                out returnedProperties );
            if( result!=ErrorCode.SUCCESS )
                throw new OpenCLException( "SetCommandQueueProperty failed with error code "+result , result);
            oldProperties = (CommandQueueProperties)returnedProperties;
#pragma warning restore 618
        }

        public static implicit operator IntPtr( CommandQueue cq )
        {
            return cq.CommandQueueID;
        }
    }
}
