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
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace OpenCLNet
{
    #region Using statements

    using cl_char=SByte;
    using cl_uchar=Byte;
    using cl_short=Byte;
    using cl_ushort=Byte;
    using cl_int=Int32;
    using cl_uint=UInt32;
    using cl_long=Int64;
    using cl_ulong=UInt64;
    using cl_half=UInt16;
    using cl_float=Single;
    using cl_double=Double;

    using cl_platform_id=IntPtr;
    using cl_device_id=IntPtr;
    using cl_context=IntPtr;
    using cl_command_queue=IntPtr;
    using cl_mem=IntPtr;
    using cl_program=IntPtr;
    using cl_kernel=IntPtr;
    using cl_event=IntPtr;
    using cl_sampler=IntPtr;

    using cl_bool=UInt32;
    using cl_bitfield=UInt64;
    using cl_device_type=UInt64;
    using cl_platform_info=UInt32;
    using cl_device_info=UInt32;
    using cl_device_address_info=UInt64;
    using cl_device_fp_config=UInt64;
    using cl_device_mem_cache_type=UInt32;
    using cl_device_local_mem_type=UInt32;
    using cl_device_exec_capabilities=UInt64;
    using cl_command_queue_properties=UInt64;

    using cl_context_properties=IntPtr;
    using cl_context_info=UInt32;
    using cl_command_queue_info=UInt32;
    using cl_channel_order=UInt32;
    using cl_channel_type=UInt32;
    using cl_mem_flags=UInt64;
    using cl_mem_object_type=UInt32;
    using cl_mem_info=UInt32;
    using cl_image_info=UInt32;
    using cl_addressing_mode=UInt32;
    using cl_filter_mode=UInt32;
    using cl_sampler_info=UInt32;
    using cl_map_flags=UInt64;
    using cl_program_info=UInt32;
    using cl_program_build_info=UInt32;
    using cl_build_status=Int32;
    using cl_kernel_info=UInt32;
    using cl_kernel_work_group_info=UInt32;
    using cl_event_info=UInt32;
    using cl_command_type=UInt32;
    using cl_profiling_info=UInt32;

    using cl_gl_object_type=UInt32;
    using cl_gl_texture_info=UInt32;
    using cl_gl_platform_info=UInt32;
    using GLuint=UInt32;
    using GLint=Int32;
    using GLenum=Int32;
    #endregion

    public delegate void ContextNotify( string errInfo, byte[] data, IntPtr cb, IntPtr userData );
    public delegate void ProgramNotify( cl_program program, IntPtr userData );
    public delegate void NativeKernel( IntPtr args );

    public class OpenCLException : Exception
    {
        public ErrorCode ErrorCode=ErrorCode.SUCCESS;

        public OpenCLException()
        {
        }

        public OpenCLException( ErrorCode errorCode )
        {
            ErrorCode = errorCode;
        }

        public OpenCLException( string errorMessage )
            : base( errorMessage )
        {
        }

        public OpenCLException( string errorMessage, ErrorCode errorCode )
            : base( errorMessage )
        {
            ErrorCode = errorCode;
        }

        public OpenCLException( string message, Exception innerException )
            : base( message, innerException )
        {
        }

        public OpenCLException( string message, ErrorCode errorCode, Exception innerException )
            : base( message, innerException )
        {
            ErrorCode = errorCode;
        }
    }

    public unsafe abstract class OpenCLAPI
    {
        #region Platform API
        // Platform API
        public abstract ErrorCode GetPlatformIDs( cl_uint num_entries, cl_platform_id[] platforms, out cl_uint numPlatforms );
        public abstract ErrorCode GetPlatformInfo( cl_platform_id platform, cl_platform_info param_name, IntPtr param_value_size, void* param_value, out IntPtr param_value_size_ret );
        #endregion

        #region Device API

        // Device APIs
        public abstract ErrorCode GetDeviceIDs( cl_platform_id platform, DeviceType device_type, cl_uint num_entries, cl_device_id[] devices, out cl_uint num_devices );
        public abstract ErrorCode GetDeviceInfo( cl_device_id device, cl_device_info param_name, IntPtr param_value_size, void* param_value, out IntPtr param_value_size_ret );

        #endregion

        #region Context API

        // Context APIs
        public abstract cl_context CreateContext( cl_context_properties[] properties, cl_uint num_devices, cl_device_id[] devices, ContextNotify pfn_notify, IntPtr user_data, out ErrorCode errcode_ret );
        public abstract cl_context CreateContextFromType( cl_context_properties[] properties, DeviceType device_type, ContextNotify pfn_notify, IntPtr user_data, out ErrorCode errcode_ret );
        public abstract ErrorCode RetainContext( cl_context context );
        public abstract ErrorCode ReleaseContext( cl_context context );
        public abstract ErrorCode GetContextInfo( cl_context context, cl_context_info param_name, IntPtr param_value_size, void* param_value, out IntPtr param_value_size_ret );

        #endregion

        #region Program Object API

        // Program Object APIs
        public abstract cl_program CreateProgramWithSource( cl_context context, cl_uint count, string[] strings, IntPtr[] lengths, out ErrorCode errcode_ret );
        public abstract cl_program CreateProgramWithBinary( cl_context context, cl_uint num_devices, cl_device_id[] device_list, IntPtr[] lengths, byte[][] binaries, cl_int[] binary_status, out ErrorCode errcode_ret );
        public abstract ErrorCode RetainProgram( cl_program program );
        public abstract ErrorCode ReleaseProgram( cl_program program );
        public abstract ErrorCode BuildProgram( cl_program program, cl_uint num_devices, cl_device_id[] device_list, string options, ProgramNotify pfn_notify, IntPtr user_data );
        public abstract ErrorCode UnloadCompiler();
        public abstract ErrorCode GetProgramInfo( cl_program program, cl_program_info param_name, IntPtr param_value_size, void* param_value, out IntPtr param_value_size_ret );
        public abstract ErrorCode GetProgramBuildInfo( cl_program program, cl_device_id device, cl_program_build_info param_name, IntPtr param_value_size, void* param_value, out IntPtr param_value_size_ret );

        #endregion

        #region Command Queue API

        // Command Queue APIs
        public abstract cl_command_queue CreateCommandQueue( cl_context context, cl_device_id device, cl_command_queue_properties properties, out ErrorCode errcode_ret );
        public abstract ErrorCode RetainCommandQueue( cl_context command_queue );
        public abstract ErrorCode ReleaseCommandQueue( cl_context command_queue );
        public abstract ErrorCode GetCommandQueueInfo( cl_context command_queue, cl_command_queue_info param_name, IntPtr param_value_size, void* param_value, out IntPtr param_value_size_ret );
        public abstract ErrorCode SetCommandQueueProperty( cl_context command_queue, cl_command_queue_properties properties, bool enable, out cl_command_queue_properties old_properties );

        #endregion

        #region Memory Object API

        // Memory Object APIs
        public abstract cl_mem CreateBuffer( cl_context context, cl_mem_flags flags, IntPtr size, void* host_ptr, out ErrorCode errcode_ret );
        public abstract cl_mem CreateImage2D( cl_context context, cl_mem_flags flags, cl_image_format image_format, IntPtr image_width, IntPtr image_height, IntPtr image_row_pitch, void* host_ptr, out ErrorCode errcode_ret );
        public abstract cl_mem CreateImage3D( cl_context context, cl_mem_flags flags, cl_image_format image_format, IntPtr image_width, IntPtr image_height, IntPtr image_depth, IntPtr image_row_pitch, IntPtr image_slice_pitch, void* host_ptr, out ErrorCode errcode_ret );
        public abstract ErrorCode RetainMemObject( cl_mem memobj );
        public abstract ErrorCode ReleaseMemObject( cl_mem memobj );
        public abstract ErrorCode GetSupportedImageFormats( cl_context context, cl_mem_flags flags, cl_mem_object_type image_type, cl_uint num_entries, cl_image_format[] image_formats, out cl_uint num_image_formats );
        public abstract ErrorCode GetMemObjectInfo( cl_mem memobj, cl_mem_info param_name, IntPtr param_value_size, void* param_value, out IntPtr param_value_size_ret );
        public abstract ErrorCode GetImageInfo( cl_mem image, cl_image_info param_name, IntPtr param_value_size, void* param_value, out IntPtr param_value_size_ret );

        #endregion

        #region Kernel Object API

        // Kernel Object APIs
        public abstract cl_kernel CreateKernel( cl_program program, string kernel_name, out ErrorCode errcode_ret );
        public abstract ErrorCode CreateKernelsInProgram( cl_program program, cl_uint num_kernels, cl_kernel[] kernels, out cl_uint num_kernels_ret );
        public abstract ErrorCode RetainKernel( cl_kernel kernel );
        public abstract ErrorCode ReleaseKernel( cl_kernel kernel );
        public abstract ErrorCode SetKernelArg( cl_kernel kernel, cl_uint arg_index, IntPtr arg_size, void* arg_value );
        public abstract ErrorCode GetKernelInfo( cl_kernel kernel, cl_kernel_info param_name, IntPtr param_value_size, void* param_value, out IntPtr param_value_size_ret );
        public abstract ErrorCode GetKernelWorkGroupInfo( cl_kernel kernel, cl_device_id device, cl_kernel_work_group_info param_name, IntPtr param_value_size, void* param_value, out IntPtr param_value_size_ret );

        #endregion

        #region Enqueued Commands API

        // Enqueued Commands APIs
        public abstract ErrorCode EnqueueReadBuffer( cl_command_queue command_queue,
            cl_mem buffer,
            cl_bool blocking_read,
            IntPtr offset,
            IntPtr cb,
            void* ptr,
            cl_uint num_events_in_wait_list,
            cl_event[] event_wait_list,
            cl_event* _event );
        public abstract ErrorCode EnqueueWriteBuffer( cl_command_queue command_queue,
            cl_mem buffer,
            cl_bool blocking_write,
            IntPtr offset,
            IntPtr cb,
            void* ptr,
            cl_uint num_events_in_wait_list,
            cl_event[] event_wait_list,
            cl_event* _event);
        public abstract ErrorCode EnqueueCopyBuffer( cl_command_queue command_queue,
            cl_mem src_buffer,
            cl_mem dst_buffer,
            IntPtr src_offset,
            IntPtr dst_offset,
            IntPtr cb,
            cl_uint num_events_in_wait_list,
            cl_event[] event_wait_list,
            cl_event* _event);
        public abstract ErrorCode EnqueueReadImage( cl_command_queue command_queue,
            cl_mem image,
            cl_bool blocking_read,
            IntPtr[] origin,
            IntPtr[] region,
            IntPtr row_pitch,
            IntPtr slice_pitch,
            void* ptr,
            cl_uint num_events_in_wait_list,
            cl_event[] event_wait_list,
            cl_event* _event);
        public abstract ErrorCode EnqueueWriteImage( cl_command_queue command_queue,
            cl_mem image,
            cl_bool blocking_write,
            IntPtr[] origin,
            IntPtr[] region,
            IntPtr input_row_pitch,
            IntPtr input_slice_pitch,
            void* ptr,
            cl_uint num_events_in_wait_list,
            cl_event[] event_wait_list,
            cl_event* _event);
        public abstract ErrorCode EnqueueCopyImage( cl_command_queue command_queue,
            cl_mem src_image,
            cl_mem dst_image,
            IntPtr[] src_origin,
            IntPtr[] dst_origin,
            IntPtr[] region,
            cl_uint num_events_in_wait_list,
            cl_event[] event_wait_list,
            cl_event* _event );
        public abstract ErrorCode EnqueueCopyImageToBuffer( cl_command_queue command_queue,
            cl_mem src_image,
            cl_mem dst_buffer,
            IntPtr[] src_origin,
            IntPtr[] region,
            IntPtr dst_offset,
            cl_uint num_events_in_wait_list,
            cl_event[] event_wait_list,
            cl_event* _event );
        public abstract ErrorCode EnqueueCopyBufferToImage( cl_command_queue command_queue,
            cl_mem src_buffer,
            cl_mem dst_image,
            IntPtr src_offset,
            IntPtr[] dst_origin,
            IntPtr[] region,
            cl_uint num_events_in_wait_list,
            cl_event[] event_wait_list,
            cl_event* _event);
        public abstract void* EnqueueMapBuffer( cl_command_queue command_queue,
            cl_mem buffer,
            cl_bool blocking_map,
            cl_map_flags map_flags,
            IntPtr offset,
            IntPtr cb,
            cl_uint num_events_in_wait_list,
            cl_event[] event_wait_list,
            cl_event* _event,
            out ErrorCode errcode_ret );
        public abstract void* EnqueueMapImage( cl_command_queue command_queue,
            cl_mem image,
            cl_bool blocking_map,
            cl_map_flags map_flags,
            IntPtr[] origin,
            IntPtr[] region,
            out IntPtr image_row_pitch,
            out IntPtr image_slice_pitch,
            cl_uint num_events_in_wait_list,
            cl_event[] event_wait_list,
            cl_event* _event,
            out ErrorCode errcode_ret );
        public abstract ErrorCode EnqueueUnmapMemObject( cl_command_queue command_queue,
            cl_mem memobj,
            void* mapped_ptr,
            cl_uint num_events_in_wait_list,
            cl_event[] event_wait_list,
            cl_event* _event);
        public abstract ErrorCode EnqueueNDRangeKernel( cl_command_queue command_queue,
            cl_kernel kernel,
            cl_uint work_dim,
            IntPtr[] global_work_offset,
            IntPtr[] global_work_size,
            IntPtr[] local_work_size,
            cl_uint num_events_in_wait_list,
            cl_event[] event_wait_list,
            cl_event* _event);
        public abstract ErrorCode EnqueueTask( cl_command_queue command_queue,
            cl_kernel kernel,
            cl_uint num_events_in_wait_list,
            cl_event[] event_wait_list,
            cl_event* _event);
        public abstract ErrorCode EnqueueNativeKernel(cl_command_queue command_queue,
            NativeKernel user_func,
            void* args,
            IntPtr cb_args,
            cl_uint num_mem_objects,
            cl_mem[] mem_list,
            IntPtr[] args_mem_loc,
            cl_uint num_events_in_wait_list,
            cl_event[] event_wait_list,
            cl_event* _event);
        public abstract ErrorCode EnqueueMarker(cl_command_queue command_queue, cl_event* _event);
        public abstract ErrorCode EnqueueWaitForEvents( cl_command_queue command_queue, cl_uint num_events, cl_event[] _event_list );
        public abstract ErrorCode EnqueueBarrier( cl_command_queue command_queue );

        #endregion

        #region Flush and Finish API

        // Flush and Finish APIs
        public abstract ErrorCode Flush( cl_command_queue command_queue );
        public abstract ErrorCode Finish( cl_command_queue command_queue );

        #endregion

        #region Event Object API

        // Event Object APIs
        public abstract ErrorCode WaitForEvents( cl_uint num_events, cl_event[] _event_list );
        public abstract ErrorCode GetEventInfo( cl_event _event, cl_event_info param_name, IntPtr param_value_size, void* param_value, out IntPtr param_value_size_ret );
        public abstract ErrorCode RetainEvent( cl_event _event );
        public abstract ErrorCode ReleaseEvent( cl_event _event );

        #endregion

        #region Sampler API
        
        // Sampler APIs
        public abstract cl_sampler CreateSampler( cl_context context, bool normalized_coords, cl_addressing_mode addressing_mode, cl_filter_mode filter_mode, out ErrorCode errcode_ret );
        public abstract ErrorCode RetainSampler( cl_sampler sampler );
        public abstract ErrorCode ReleaseSampler( cl_sampler sampler );
        public abstract ErrorCode GetSamplerInfo( cl_sampler sampler, cl_sampler_info param_name, IntPtr param_value_size, void* param_value, out IntPtr param_value_size_ret );

        #endregion            

        #region GLObject API

        // There's no actual GLObject class. These functions are located in Context, Mem and CommandQueue
        public abstract cl_mem CreateFromGLBuffer(cl_context context, cl_mem_flags flags, GLuint bufobj, out ErrorCode errcode_ret);
        public abstract cl_mem CreateFromGLTexture2D(cl_context context, cl_mem_flags flags, GLenum target, GLint mipLevel, GLuint texture, out ErrorCode errcode_ret);
        public abstract cl_mem CreateFromGLTexture3D(cl_context context, cl_mem_flags flags, GLenum target, GLint mipLevel, GLuint texture, out ErrorCode errcode_ret);
        public abstract cl_mem CreateFromGLRenderbuffer(cl_context context, cl_mem_flags flags, GLuint renderBuffer, out ErrorCode errcode_ret);
        public abstract ErrorCode GetGLObjectInfo(cl_mem memobj, out cl_gl_object_type gl_object_type, out GLuint gl_object_name);
        public abstract ErrorCode GetGLTextureInfo(cl_mem memobj, cl_gl_texture_info param_name, IntPtr param_value_size, void* param_value, out IntPtr param_value_size_ret);
        public abstract ErrorCode EnqueueAcquireGLObjects(cl_command_queue command_queue, cl_uint num_objects, cl_mem[] mem_objects, cl_uint num_events_in_wait_list, cl_event[] event_wait_list, cl_event* _event);
        public abstract ErrorCode EnqueueReleaseGLObjects(cl_command_queue command_queue, cl_uint num_objects, cl_mem[] mem_objects, cl_uint num_events_in_wait_list, cl_event[] event_wait_list, cl_event* _event);

        #endregion


        // Extension function access
        public abstract IntPtr GetExtensionFunctionAddress(string func_name);

        // Profiling APIs
        public abstract ErrorCode GetEventProfilingInfo(cl_event _event, ProfilingInfo param_name, IntPtr param_value_size, void* param_value, out IntPtr param_value_size_ret);
    }
}
