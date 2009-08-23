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





    /// <summary>
    /// ATI specific implementation of the OpenCLAPI
    /// </summary>
    unsafe public class ATI : OpenCLAPI
    {
        internal static class Configuration
        {
            public const string Library = "opencl.dll";
        }

        #region Platform API

        // Platform API
        [DllImport(Configuration.Library)]
        public extern static cl_int clGetPlatformIDs(cl_uint num_entries, [Out] [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 0)] cl_platform_id[] platforms, out cl_uint num_platforms);

        [DllImport( Configuration.Library )]
        public extern static cl_int clGetPlatformInfo( cl_platform_id platform, cl_platform_info param_name, IntPtr param_value_size, void* param_value, out IntPtr param_value_size_ret );

        public override ErrorCode GetPlatformIDs( uint num_entries, IntPtr[] platforms, out uint num_platforms )
        {
            return (ErrorCode)clGetPlatformIDs( num_entries, platforms, out num_platforms );
        }

        public override ErrorCode GetPlatformInfo( IntPtr platform, uint param_name, IntPtr param_value_size, void* param_value, out IntPtr param_value_size_ret )
        {
            return (ErrorCode)clGetPlatformInfo( platform, param_name, param_value_size, param_value, out param_value_size_ret );
        }

        #endregion

        #region Device API

        // Device APIs
        [DllImport(Configuration.Library)]
        public extern static cl_int clGetDeviceIDs(cl_platform_id platform, [MarshalAs(UnmanagedType.U8)]DeviceType device_type, cl_uint num_entries, [Out][MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 2)]cl_device_id[] devices, out cl_uint num_devices);

        [DllImport( Configuration.Library)]
        public extern static cl_int clGetDeviceInfo( cl_device_id device, cl_device_info param_name, IntPtr param_value_size, void* param_value, out IntPtr param_value_size_ret );

        public override ErrorCode GetDeviceIDs( IntPtr platform, DeviceType device_type, uint num_entries, IntPtr[] devices, out uint num_devices )
        {
            return (ErrorCode)clGetDeviceIDs( platform, device_type, num_entries, devices, out num_devices );
        }

        public override ErrorCode GetDeviceInfo( IntPtr device, uint param_name, IntPtr param_value_size, void* param_value, out IntPtr param_value_size_ret )
        {
            return (ErrorCode)clGetDeviceInfo( device, param_name, param_value_size, param_value, out param_value_size_ret );
        }

        #endregion

        #region Context API

        // Context APIs 
        [DllImport( Configuration.Library )]
        public extern static cl_context clCreateContext( [In] cl_context_properties[] properties, cl_uint num_devices, [In]cl_device_id[] devices, ContextNotify pfn_notify, IntPtr user_data, [MarshalAs( UnmanagedType.I4 )] out ErrorCode errcode_ret );
        [DllImport( Configuration.Library )]
        public extern static cl_context clCreateContextFromType( [In] cl_context_properties[] properties, [MarshalAs( UnmanagedType.U8 )]DeviceType device_type, ContextNotify pfn_notify, IntPtr user_data, [MarshalAs( UnmanagedType.I4 )] out ErrorCode errcode_ret );
        [DllImport( Configuration.Library )]
        public extern static cl_int clRetainContext( cl_context context );
        [DllImport( Configuration.Library )]
        public extern static cl_int clReleaseContext( cl_context context );
        [DllImport( Configuration.Library )]
        public extern static cl_int clGetContextInfo( cl_context context, cl_context_info param_name, IntPtr param_value_size, void* param_value, out IntPtr param_value_size_ret );

        public override IntPtr CreateContext( IntPtr[] properties, uint num_devices, IntPtr[] devices, ContextNotify pfn_notify,IntPtr user_data, out ErrorCode errcode_ret )
        {
            return clCreateContext( properties, num_devices, devices, pfn_notify, user_data, out errcode_ret );
        }

        public override IntPtr CreateContextFromType( IntPtr[] properties, DeviceType device_type, ContextNotify pfn_notify, IntPtr user_data, out ErrorCode errcode_ret )
        {
            return clCreateContextFromType( properties, device_type, pfn_notify, user_data, out errcode_ret );
        }

        public override ErrorCode RetainContext( IntPtr context )
        {
            return (ErrorCode)clRetainContext( context );
        }

        public override ErrorCode ReleaseContext( IntPtr context )
        {
            return (ErrorCode)clReleaseContext( context );
        }

        public override ErrorCode GetContextInfo( IntPtr context, uint param_name, IntPtr param_value_size, void* param_value, out IntPtr param_value_size_ret )
        {
            return (ErrorCode)clGetContextInfo( context, param_name, param_value_size, param_value, out param_value_size_ret );
        }

        #endregion

        #region Program Object API

        // Program Object APIs
        [DllImport( Configuration.Library )]
        public extern static cl_program clCreateProgramWithSource( cl_context context,
            cl_uint count,
            [In] string[] strings,
            [In] IntPtr[] lengths,
            [MarshalAs( UnmanagedType.I4 )] out ErrorCode errcode_ret );
        [DllImport( Configuration.Library )]
        public extern static cl_program clCreateProgramWithBinary( cl_context context,
            cl_uint num_devices,
            [In] cl_device_id[] device_list,
            [In] IntPtr[] lengths,
            [In] byte[][] binaries,
            [Out][MarshalAs( UnmanagedType.LPArray, SizeParamIndex=1 )] cl_int[] binary_status,
            [MarshalAs( UnmanagedType.I4 )] out ErrorCode errcode_ret );
        [DllImport( Configuration.Library )]
        public extern static cl_int clRetainProgram( cl_program program );
        [DllImport( Configuration.Library )]
        public extern static cl_int clReleaseProgram( cl_program program );
        [DllImport( Configuration.Library )]
        public extern static cl_int clBuildProgram( cl_program program,
            cl_uint num_devices,
            [In] cl_device_id[] device_list,
            string options,
            ProgramNotify pfn_notify,
            IntPtr user_data );
        [DllImport( Configuration.Library )]
        public extern static cl_int clUnloadCompiler();
        [DllImport( Configuration.Library )]
        public extern static cl_int clGetProgramInfo( cl_program program, cl_program_info param_name, IntPtr param_value_size, void* param_value, out IntPtr param_value_size_ret );
        [DllImport( Configuration.Library )]
        public extern static cl_int clGetProgramBuildInfo( cl_program program, cl_device_id device, cl_program_build_info param_name, IntPtr param_value_size, void* param_value, out IntPtr param_value_size_ret );


        public override IntPtr CreateProgramWithSource( IntPtr context, uint count, string[] strings, IntPtr[] lengths, out ErrorCode errcode_ret )
        {
            return clCreateProgramWithSource( context, count, strings, lengths, out errcode_ret );
        }

        public override IntPtr CreateProgramWithBinary( IntPtr context, uint num_devices, IntPtr[] device_list, IntPtr[] lengths, byte[][] binaries, int[] binary_status, out ErrorCode errcode_ret )
        {
            IntPtr program;

            program = clCreateProgramWithBinary( context, num_devices, device_list, lengths, binaries, binary_status, out errcode_ret );
            return program;
        }

        public override ErrorCode RetainProgram( IntPtr program )
        {
            return (ErrorCode)clRetainProgram( program );
        }

        public override ErrorCode ReleaseProgram( IntPtr program )
        {
            return (ErrorCode)clReleaseProgram( program );
        }

        public override ErrorCode BuildProgram( IntPtr program, uint num_devices, IntPtr[] device_list, string options, ProgramNotify pfn_notify, IntPtr user_data )
        {
            return (ErrorCode)clBuildProgram( program, num_devices, device_list, options, pfn_notify, user_data );
        }

        public override ErrorCode UnloadCompiler()
        {
            return (ErrorCode)clUnloadCompiler();
        }

        public override ErrorCode GetProgramInfo( IntPtr program, uint param_name, IntPtr param_value_size, void* param_value, out IntPtr param_value_size_ret )
        {
            return (ErrorCode)clGetProgramInfo( program, param_name, param_value_size, param_value, out param_value_size_ret );
        }

        public override ErrorCode GetProgramBuildInfo( IntPtr program, IntPtr device, uint param_name, IntPtr param_value_size, void* param_value, out IntPtr param_value_size_ret )
        {
            return (ErrorCode)clGetProgramBuildInfo( program, device, param_name, param_value_size, param_value, out param_value_size_ret );
        }

        #endregion

        #region Command Queue API

        // Command Queue APIs
        [DllImport( Configuration.Library )]
        public extern static IntPtr clCreateCommandQueue( cl_context context, cl_device_id device, cl_command_queue_properties properties, [MarshalAs( UnmanagedType.I4 )] out ErrorCode errcode_ret );
        [DllImport( Configuration.Library )]
        public extern static ErrorCode clRetainCommandQueue( cl_command_queue command_queue );
        [DllImport( Configuration.Library )]
        public extern static ErrorCode clReleaseCommandQueue( cl_command_queue command_queue );
        [DllImport( Configuration.Library )]
        public extern static ErrorCode clGetCommandQueueInfo( cl_command_queue command_queue, cl_command_queue_info param_name, IntPtr param_value_size, void* param_value, out IntPtr param_value_size_ret );
        [DllImport( Configuration.Library )]
        public extern static ErrorCode clSetCommandQueueProperty( cl_command_queue command_queue, cl_command_queue_properties properties, [MarshalAs( UnmanagedType.I4 )]bool enable, out cl_command_queue_properties old_properties );

        public override IntPtr CreateCommandQueue( IntPtr context, IntPtr device, ulong properties, out ErrorCode errcode_ret )
        {
            return clCreateCommandQueue( context, device, properties, out errcode_ret );
        }

        public override ErrorCode RetainCommandQueue( IntPtr command_queue )
        {
            return clRetainCommandQueue( command_queue );
        }

        public override ErrorCode ReleaseCommandQueue( IntPtr command_queue )
        {
            return clReleaseCommandQueue( command_queue );
        }

        public override ErrorCode GetCommandQueueInfo( IntPtr command_queue, uint param_name, IntPtr param_value_size, void* param_value, out IntPtr param_value_size_ret )
        {
            return clGetCommandQueueInfo( command_queue, param_name, param_value_size, param_value, out param_value_size_ret );
        }

        public override ErrorCode SetCommandQueueProperty( IntPtr command_queue, ulong properties, bool enable, out ulong old_properties )
        {
            return clSetCommandQueueProperty( command_queue, properties, enable, out old_properties );
        }

        #endregion

        #region Memory Object API

        // Memory Object APIs
        [DllImport( Configuration.Library )]
        public extern static cl_mem clCreateBuffer( cl_context context, cl_mem_flags flags, IntPtr size, void* host_ptr, out ErrorCode errcode_ret );
        [DllImport( Configuration.Library )]
        public extern static cl_mem clCreateImage2D( cl_context context, cl_mem_flags flags, cl_image_format* image_format, IntPtr image_width, IntPtr image_height, IntPtr image_row_pitch, void* host_ptr, out ErrorCode errcode_ret );
        [DllImport( Configuration.Library )]
        public extern static cl_mem clCreateImage3D( cl_context context, cl_mem_flags flags, cl_image_format* image_format, IntPtr image_width, IntPtr image_height, IntPtr image_depth, IntPtr image_row_pitch, IntPtr image_slice_pitch, void* host_ptr, out ErrorCode errcode_ret );
        [DllImport( Configuration.Library )]
        public extern static ErrorCode clRetainMemObject( cl_mem memobj );
        [DllImport( Configuration.Library )]
        public extern static ErrorCode clReleaseMemObject( cl_mem memobj );
        [DllImport( Configuration.Library )]
        public extern static ErrorCode clGetSupportedImageFormats( cl_context context,
            cl_mem_flags flags,
            cl_mem_object_type image_type,
            cl_uint num_entries,
            [Out][MarshalAs( UnmanagedType.LPArray, SizeParamIndex=3 )] cl_image_format[] image_formats,
            out cl_uint num_image_formats );
        [DllImport( Configuration.Library )]
        public extern static ErrorCode clGetMemObjectInfo( cl_mem memobj, cl_mem_info param_name, IntPtr param_value_size, void* param_value, out IntPtr param_value_size_ret );
        [DllImport( Configuration.Library )]
        public extern static ErrorCode clGetImageInfo( cl_mem image, cl_image_info param_name, IntPtr param_value_size, void* param_value, out IntPtr param_value_size_ret );

        public override IntPtr CreateBuffer( IntPtr context, ulong flags, IntPtr size, void* host_ptr, out ErrorCode errcode_ret )
        {
            return clCreateBuffer( context, flags, size, host_ptr, out errcode_ret );
        }

        public override IntPtr CreateImage2D( IntPtr context, ulong flags, cl_image_format image_format, IntPtr image_width, IntPtr image_height, IntPtr image_row_pitch, void* host_ptr, out ErrorCode errcode_ret )
        {
            return clCreateImage2D( context, flags, &image_format, image_width, image_height, image_row_pitch, host_ptr, out errcode_ret );
        }

        public override IntPtr CreateImage3D( IntPtr context, ulong flags, cl_image_format image_format, IntPtr image_width, IntPtr image_height, IntPtr image_depth, IntPtr image_row_pitch, IntPtr image_slice_pitch, void* host_ptr, out ErrorCode errcode_ret )
        {
            return clCreateImage3D( context, flags, &image_format, image_width, image_height, image_depth, image_row_pitch, image_slice_pitch, host_ptr, out errcode_ret );
        }

        public override ErrorCode RetainMemObject( IntPtr memobj )
        {
            return clRetainMemObject( memobj );
        }

        public override ErrorCode ReleaseMemObject( IntPtr memobj )
        {
            return clReleaseMemObject( memobj );
        }

        public override ErrorCode GetSupportedImageFormats( IntPtr context, ulong flags, uint image_type, uint num_entries, cl_image_format[] image_formats, out uint num_image_formats )
        {
            return clGetSupportedImageFormats( context, flags, image_type, num_entries, image_formats, out num_image_formats );
        }

        public override ErrorCode GetMemObjectInfo( IntPtr memobj, uint param_name, IntPtr param_value_size, void* param_value, out IntPtr param_value_size_ret )
        {
            return clGetMemObjectInfo( memobj, param_name, param_value_size, param_value, out param_value_size_ret );
        }

        public override ErrorCode GetImageInfo( IntPtr image, uint param_name, IntPtr param_value_size, void* param_value, out IntPtr param_value_size_ret )
        {
            return clGetImageInfo( image, param_name, param_value_size, param_value, out param_value_size_ret );
        }

        #endregion

        #region Kernel Object API

        // Kernel Object APIs
        [DllImport( Configuration.Library )]
        public extern static cl_kernel clCreateKernel(cl_program program, string kernel_name, out ErrorCode errcode_ret);
        [DllImport( Configuration.Library )]
        public extern static ErrorCode clCreateKernelsInProgram(cl_program program,
            cl_uint num_kernels,
            [Out][MarshalAs(UnmanagedType.LPArray,SizeParamIndex=1)]cl_kernel[] kernels,
            out cl_uint num_kernels_ret);
        [DllImport( Configuration.Library )]
        public extern static ErrorCode clRetainKernel( cl_kernel kernel );
        [DllImport( Configuration.Library )]
        public extern static ErrorCode clReleaseKernel( cl_kernel kernel );
        [DllImport( Configuration.Library )]
        public extern static ErrorCode clSetKernelArg( cl_kernel kernel, cl_uint arg_index, IntPtr arg_size, void* arg_value );
        [DllImport( Configuration.Library )]
        public extern static ErrorCode clGetKernelInfo( cl_kernel kernel, cl_kernel_info param_name, IntPtr param_value_size, void* param_value, out IntPtr param_value_size_ret );
        [DllImport( Configuration.Library )]
        public extern static ErrorCode clGetKernelWorkGroupInfo( cl_kernel kernel, cl_device_id device, cl_kernel_work_group_info param_name, IntPtr param_value_size, void* param_value, out IntPtr param_value_size_ret );

        public override cl_kernel CreateKernel( IntPtr program, string kernel_name, out ErrorCode errcode_ret )
        {
            return clCreateKernel( program, kernel_name, out errcode_ret );
        }

        public override ErrorCode CreateKernelsInProgram( IntPtr program, uint num_kernels, IntPtr[] kernels, out uint num_kernels_ret )
        {
            return clCreateKernelsInProgram( program, num_kernels, kernels, out num_kernels_ret );
        }

        public override ErrorCode RetainKernel( IntPtr kernel )
        {
            return clRetainKernel( kernel );
        }

        public override ErrorCode ReleaseKernel( IntPtr kernel )
        {
            return clReleaseKernel( kernel );
        }

        public override ErrorCode SetKernelArg( IntPtr kernel, uint arg_index, IntPtr arg_size, void* arg_value )
        {
            return clSetKernelArg( kernel, arg_index, arg_size, arg_value );
        }

        public override ErrorCode GetKernelInfo( IntPtr kernel, uint param_name, IntPtr param_value_size, void* param_value, out IntPtr param_value_size_ret )
        {
            return clGetKernelInfo( kernel, param_name, param_value_size, param_value, out param_value_size_ret );
        }

        public override ErrorCode GetKernelWorkGroupInfo( IntPtr kernel, IntPtr device, uint param_name, IntPtr param_value_size, void* param_value, out IntPtr param_value_size_ret )
        {
            return clGetKernelWorkGroupInfo( kernel, device, param_name, param_value_size, param_value, out param_value_size_ret );
        }

        #endregion

        #region Enqueued Commands API

        // Enqueued Commands APIs
        [DllImport( Configuration.Library )]
        public extern static ErrorCode clEnqueueReadBuffer( cl_command_queue command_queue,
            cl_mem buffer,
            cl_bool blocking_read,
            IntPtr offset,
            IntPtr cb,
            void* ptr,
            cl_uint num_events_in_wait_list,
            [In] [MarshalAs(UnmanagedType.LPArray,SizeParamIndex=6)] cl_event[] event_wait_list,
            cl_event* _event);
        [DllImport( Configuration.Library )]
        public extern static ErrorCode clEnqueueWriteBuffer( cl_command_queue command_queue,
            cl_mem buffer,
            cl_bool blocking_write,
            IntPtr offset,
            IntPtr cb,
            void* ptr,
            cl_uint num_events_in_wait_list,
            [In] [MarshalAs(UnmanagedType.LPArray,SizeParamIndex=6)] cl_event[] event_wait_list,
            cl_event* _event);
        [DllImport(Configuration.Library)]
        public extern static ErrorCode clEnqueueCopyBuffer( cl_command_queue command_queue,
            cl_mem src_buffer,
            cl_mem dst_buffer,
            IntPtr src_offset,
            IntPtr dst_offset,
            IntPtr cb,
            cl_uint num_events_in_wait_list,
            [In] [MarshalAs(UnmanagedType.LPArray,SizeParamIndex=6)] cl_event[] event_wait_list,
            cl_event* _event);
        [DllImport(Configuration.Library)]
        public extern static ErrorCode clEnqueueReadImage( cl_command_queue command_queue,
            cl_mem image,
            cl_bool blocking_read,
            IntPtr[] origin,
            IntPtr[] region,
            IntPtr row_pitch,
            IntPtr slice_pitch,
            void* ptr,
            cl_uint num_events_in_wait_list,
            [In] [MarshalAs(UnmanagedType.LPArray,SizeParamIndex=8)] cl_event[] event_wait_list,
            cl_event* _event);
        [DllImport(Configuration.Library)]
        public extern static ErrorCode clEnqueueWriteImage( cl_command_queue command_queue,
            cl_mem image,
            cl_bool blocking_write,
            IntPtr[] origin,
            IntPtr[] region,
            IntPtr input_row_pitch,
            IntPtr input_slice_pitch,
            void* ptr,
            cl_uint num_events_in_wait_list,
            [In] [MarshalAs(UnmanagedType.LPArray,SizeParamIndex=8)] cl_event[] event_wait_list,
            cl_event* _event);
        [DllImport(Configuration.Library)]
        public extern static ErrorCode clEnqueueCopyImage( cl_command_queue command_queue,
            cl_mem src_image,
            cl_mem dst_image,
            IntPtr[] src_origin,
            IntPtr[] dst_origin,
            IntPtr[] region,
            cl_uint num_events_in_wait_list,
            [In] [MarshalAs(UnmanagedType.LPArray,SizeParamIndex=6)] cl_event[] event_wait_list,
            cl_event* _event);
        [DllImport(Configuration.Library)]
        public extern static ErrorCode clEnqueueCopyImageToBuffer( cl_command_queue command_queue,
            cl_mem src_image,
            cl_mem dst_buffer,
            IntPtr[] src_origin,
            IntPtr[] region,
            IntPtr dst_offset,
            cl_uint num_events_in_wait_list,
            [In] [MarshalAs(UnmanagedType.LPArray,SizeParamIndex=6)] cl_event[] event_wait_list,
            cl_event* _event);
        [DllImport(Configuration.Library)]
        public extern static ErrorCode clEnqueueCopyBufferToImage( cl_command_queue command_queue,
            cl_mem src_buffer,
            cl_mem dst_image,
            IntPtr src_offset,
            IntPtr[] dst_origin,
            IntPtr[] region,
            cl_uint num_events_in_wait_list,
            [In] [MarshalAs(UnmanagedType.LPArray,SizeParamIndex=6)] cl_event[] event_wait_list,
            cl_event* _event);
        [DllImport(Configuration.Library)]
        public extern static void* clEnqueueMapBuffer( cl_command_queue command_queue,
            cl_mem buffer,
            cl_bool blocking_map,
            cl_map_flags map_flags,
            IntPtr offset,
            IntPtr cb,
            cl_uint num_events_in_wait_list,
            [In] [MarshalAs(UnmanagedType.LPArray,SizeParamIndex=6)] cl_event[] event_wait_list,
            cl_event* _event,
            out ErrorCode errcode_ret);
        [DllImport( Configuration.Library )]
        public extern static void* clEnqueueMapImage( cl_command_queue command_queue,
            cl_mem image,
            cl_bool blocking_map,
            cl_map_flags map_flags,
            IntPtr[] origin,
            IntPtr[] region,
            out IntPtr image_row_pitch,
            out IntPtr image_slice_pitch,
            cl_uint num_events_in_wait_list,
            [In] [MarshalAs(UnmanagedType.LPArray,SizeParamIndex=6)] cl_event[] event_wait_list,
            cl_event* _event,
            out ErrorCode errcode_ret);
        [DllImport( Configuration.Library )]
        public extern static ErrorCode clEnqueueUnmapMemObject( cl_command_queue command_queue,
            cl_mem memobj,
            void* mapped_ptr,
            cl_uint num_events_in_wait_list,
            [In] [MarshalAs(UnmanagedType.LPArray,SizeParamIndex=6)] cl_event[] event_wait_list,
            cl_event* _event);
        [DllImport(Configuration.Library)]
        public extern static ErrorCode clEnqueueNDRangeKernel( cl_command_queue command_queue,
            cl_kernel kernel,
            cl_uint work_dim,
            [In] [MarshalAs( UnmanagedType.LPArray, SizeParamIndex=2 )] IntPtr[] global_work_offset,
            [In] [MarshalAs( UnmanagedType.LPArray, SizeParamIndex=2 )] IntPtr[] global_work_size,
            [In] [MarshalAs( UnmanagedType.LPArray, SizeParamIndex=2 )] IntPtr[] local_work_size,
            cl_uint num_events_in_wait_list,
            [In] [MarshalAs(UnmanagedType.LPArray,SizeParamIndex=6)] cl_event[] event_wait_list,
            cl_event* _event);
        [DllImport(Configuration.Library)]
        public extern static ErrorCode clEnqueueTask( cl_command_queue command_queue,
            cl_kernel kernel,
            cl_uint num_events_in_wait_list,
            [In] [MarshalAs(UnmanagedType.LPArray,SizeParamIndex=2)] cl_event[] event_wait_list,
            cl_event* _event);
        [DllImport( Configuration.Library )]
        public extern static ErrorCode clEnqueueNativeKernel(cl_command_queue command_queue,
            NativeKernel user_func,
            void* args,
            IntPtr cb_args,
            cl_uint num_mem_objects,
            [In] cl_mem[] mem_list,
            [Out] [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 2)] IntPtr[] args_mem_loc,
            cl_uint num_events_in_wait_list,
            [In]cl_event[] event_wait_list,
            cl_event* _event);
        [DllImport( Configuration.Library )]
        public extern static ErrorCode clEnqueueMarker( cl_command_queue command_queue, cl_event* _event );
        [DllImport( Configuration.Library )]
        public extern static ErrorCode clEnqueueWaitForEvents( cl_command_queue command_queue,
            cl_uint num_events,
            [In] [MarshalAs( UnmanagedType.LPArray, SizeParamIndex=1 )] cl_event[] _event_list );
        [DllImport( Configuration.Library )]
        public extern static ErrorCode clEnqueueBarrier( cl_command_queue command_queue );


        public override ErrorCode EnqueueReadBuffer( IntPtr command_queue, IntPtr buffer, uint blocking_read, IntPtr offset, IntPtr cb, void* ptr, uint num_events_in_wait_list, IntPtr[] event_wait_list, IntPtr* _event )
        {
            return clEnqueueReadBuffer( command_queue, buffer, blocking_read, offset, cb, ptr, num_events_in_wait_list, event_wait_list, _event );
        }

        public override ErrorCode EnqueueWriteBuffer( IntPtr command_queue, IntPtr buffer, uint blocking_write, IntPtr offset, IntPtr cb, void* ptr, uint num_events_in_wait_list, IntPtr[] event_wait_list, IntPtr* _event )
        {
            return clEnqueueWriteBuffer( command_queue, buffer, blocking_write, offset, cb, ptr, num_events_in_wait_list, event_wait_list, _event );
        }

        public override ErrorCode EnqueueCopyBuffer( IntPtr command_queue, IntPtr src_buffer, IntPtr dst_buffer, IntPtr src_offset, IntPtr dst_offset, IntPtr cb, uint num_events_in_wait_list, IntPtr[] event_wait_list, IntPtr* _event )
        {
            return clEnqueueCopyBuffer( command_queue, src_buffer, dst_buffer, src_offset, dst_offset, cb, num_events_in_wait_list, event_wait_list, _event );
        }

        public override ErrorCode EnqueueReadImage( IntPtr command_queue, IntPtr image, uint blocking_read, IntPtr[] origin, IntPtr[] region, IntPtr row_pitch, IntPtr slice_pitch, void* ptr, uint num_events_in_wait_list, IntPtr[] event_wait_list, IntPtr* _event )
        {
            return clEnqueueReadImage( command_queue, image, blocking_read, origin, region, row_pitch, slice_pitch, ptr, num_events_in_wait_list, event_wait_list, _event );
        }

        public override ErrorCode EnqueueWriteImage( IntPtr command_queue, IntPtr image, uint blocking_write, IntPtr[] origin, IntPtr[] region, IntPtr input_row_pitch, IntPtr input_slice_pitch, void* ptr, uint num_events_in_wait_list, IntPtr[] event_wait_list, IntPtr* _event )
        {
            return clEnqueueWriteImage( command_queue, image, blocking_write, origin, region, input_row_pitch, input_slice_pitch, ptr, num_events_in_wait_list, event_wait_list, _event );
        }

        public override ErrorCode EnqueueCopyImage( IntPtr command_queue, IntPtr src_image, IntPtr dst_image, IntPtr[] src_origin, IntPtr[] dst_origin, IntPtr[] region, uint num_events_in_wait_list, IntPtr[] event_wait_list, IntPtr* _event )
        {
            return clEnqueueCopyImage( command_queue, src_image, dst_image, src_origin, dst_origin, region, num_events_in_wait_list, event_wait_list, _event );
        }

        public override ErrorCode EnqueueCopyImageToBuffer( IntPtr command_queue, IntPtr src_image, IntPtr dst_buffer, IntPtr[] src_origin, IntPtr[] region, IntPtr dst_offset, uint num_events_in_wait_list, IntPtr[] event_wait_list, IntPtr* _event )
        {
            return clEnqueueCopyImageToBuffer( command_queue, src_image, dst_buffer, src_origin, region, dst_offset, num_events_in_wait_list, event_wait_list, _event );
        }

        public override ErrorCode EnqueueCopyBufferToImage( IntPtr command_queue, IntPtr src_buffer, IntPtr dst_image, IntPtr src_offset, IntPtr[] dst_origin, IntPtr[] region, uint num_events_in_wait_list, IntPtr[] event_wait_list, IntPtr* _event )
        {
            return clEnqueueCopyBufferToImage( command_queue, src_buffer, dst_image, src_offset, dst_origin, region, num_events_in_wait_list, event_wait_list, _event );
        }

        public override void* EnqueueMapBuffer( IntPtr command_queue, IntPtr buffer, uint blocking_map, ulong map_flags, IntPtr offset, IntPtr cb, uint num_events_in_wait_list, IntPtr[] event_wait_list, IntPtr* _event, out ErrorCode errcode_ret )
        {
            return clEnqueueMapBuffer( command_queue, buffer, blocking_map, map_flags, offset, cb, num_events_in_wait_list, event_wait_list, _event, out errcode_ret );
        }

        public override void* EnqueueMapImage( IntPtr command_queue, IntPtr image, uint blocking_map, ulong map_flags, IntPtr[] origin, IntPtr[] region, out IntPtr image_row_pitch, out IntPtr image_slice_pitch, uint num_events_in_wait_list, IntPtr[] event_wait_list, IntPtr* _event, out ErrorCode errcode_ret )
        {
            return clEnqueueMapImage( command_queue, image, blocking_map, map_flags, origin, region, out image_row_pitch, out image_slice_pitch, num_events_in_wait_list, event_wait_list, _event, out errcode_ret );
        }

        public override ErrorCode EnqueueUnmapMemObject( IntPtr command_queue, IntPtr memobj, void* mapped_ptr, uint num_events_in_wait_list, IntPtr[] event_wait_list, IntPtr* _event )
        {
            return clEnqueueUnmapMemObject( command_queue, memobj, mapped_ptr, num_events_in_wait_list, event_wait_list, _event );
        }

        public override ErrorCode EnqueueNDRangeKernel( IntPtr command_queue, IntPtr kernel, uint work_dim, IntPtr[] global_work_offset, IntPtr[] global_work_size, IntPtr[] local_work_size, uint num_events_in_wait_list, IntPtr[] event_wait_list, IntPtr* _event )
        {
            return clEnqueueNDRangeKernel( command_queue, kernel, work_dim, global_work_offset, global_work_size, local_work_size, num_events_in_wait_list, event_wait_list, _event );
        }

        public override ErrorCode EnqueueTask( IntPtr command_queue, IntPtr kernel, uint num_events_in_wait_list, IntPtr[] event_wait_list, IntPtr* _event )
        {
            return clEnqueueTask( command_queue, kernel, num_events_in_wait_list, event_wait_list, _event );
        }

        public override ErrorCode EnqueueNativeKernel(cl_command_queue command_queue,
            NativeKernel user_func,
            void* args,
            IntPtr cb_args,
            cl_uint num_mem_objects,
            cl_mem[] mem_list,
            IntPtr[] args_mem_loc,
            cl_uint num_events_in_wait_list,
            cl_event[] event_wait_list,
            cl_event* _event)
        {
            return clEnqueueNativeKernel( command_queue,
                user_func,
                args,
                cb_args,
                num_mem_objects,
                mem_list,
                args_mem_loc,
                num_events_in_wait_list,
                event_wait_list,
                _event );
        }

        public override ErrorCode EnqueueMarker( IntPtr command_queue, IntPtr* _event )
        {
            return clEnqueueMarker( command_queue, _event );
        }

        public override ErrorCode EnqueueWaitForEvents( IntPtr command_queue, uint num_events, IntPtr[] _event_list )
        {
            return clEnqueueWaitForEvents( command_queue, num_events, _event_list );
        }

        public override ErrorCode EnqueueBarrier( IntPtr command_queue )
        {
            return clEnqueueBarrier( command_queue );
        }

        #endregion

        #region Flush and Finish API

        [DllImport( Configuration.Library )]
        public extern static ErrorCode clFlush( cl_command_queue command_queue );
        [DllImport( Configuration.Library )]
        public extern static ErrorCode clFinish( cl_command_queue command_queue );

        // Flush and Finish APIs
        public override ErrorCode Flush( cl_command_queue command_queue )
        {
            return clFlush( command_queue );
        }

        public override ErrorCode Finish( cl_command_queue command_queue )
        {
            return clFinish( command_queue );
        }

        #endregion

        #region Event Object API

        [DllImport( Configuration.Library )]
        public extern static ErrorCode clWaitForEvents( cl_uint num_events,
            [In] [MarshalAs(UnmanagedType.LPArray,SizeParamIndex=0)] cl_event[] _event_list );
        [DllImport( Configuration.Library )]
        public extern static ErrorCode clGetEventInfo( cl_event _event,
            cl_event_info param_name,
            IntPtr param_value_size,
            void* param_value,
            out IntPtr param_value_size_ret );
        [DllImport( Configuration.Library )]
        public extern static ErrorCode clRetainEvent( cl_event _event );
        [DllImport( Configuration.Library )]
        public extern static ErrorCode clReleaseEvent( cl_event _event );

        // Event Object APIs
        public override ErrorCode WaitForEvents( cl_uint num_events, cl_event[] _event_list )
        {
            return clWaitForEvents( num_events, _event_list );
        }

        public override ErrorCode GetEventInfo( cl_event _event, cl_event_info param_name, IntPtr param_value_size, void* param_value, out IntPtr param_value_size_ret )
        {
            return clGetEventInfo( _event, param_name, param_value_size, param_value, out param_value_size_ret );
        }

        public override ErrorCode RetainEvent( cl_event _event )
        {
            return clRetainEvent( _event );
        }

        public override ErrorCode ReleaseEvent( cl_event _event )
        {
            return clReleaseEvent( _event );
        }

        #endregion

        #region Sampler API

        // Sampler APIs
        [DllImport( Configuration.Library )]
        public extern static cl_sampler clCreateSampler( cl_context context, cl_bool normalized_coords, cl_addressing_mode addressing_mode, cl_filter_mode filter_mode, out ErrorCode errcode_ret );
        [DllImport( Configuration.Library )]
        public extern static ErrorCode clRetainSampler( cl_sampler sampler );
        [DllImport( Configuration.Library )]
        public extern static ErrorCode clReleaseSampler( cl_sampler sampler );
        [DllImport( Configuration.Library )]
        public extern static ErrorCode clGetSamplerInfo( cl_sampler sampler, cl_sampler_info param_name, IntPtr param_value_size, void* param_value, out IntPtr param_value_size_ret );

        public override cl_sampler CreateSampler( cl_context context, bool normalized_coords, cl_addressing_mode addressing_mode, cl_filter_mode filter_mode, out ErrorCode errcode_ret )
        {
            return clCreateSampler( context, normalized_coords?(cl_bool)Bool.TRUE:(cl_bool)Bool.FALSE, addressing_mode, filter_mode, out errcode_ret );
        }

        public override ErrorCode RetainSampler( cl_sampler sampler )
        {
            return clRetainSampler( sampler );
        }

        public override ErrorCode ReleaseSampler( cl_sampler sampler )
        {
            return clReleaseSampler( sampler );
        }

        public override ErrorCode GetSamplerInfo( cl_sampler sampler, cl_sampler_info param_name, IntPtr param_value_size, void* param_value, out IntPtr param_value_size_ret )
        {
            return clGetSamplerInfo( sampler, param_name, param_value_size, param_value, out param_value_size_ret );
        }

        #endregion

        #region GLObject API

        [DllImport( Configuration.Library )]
        public extern static cl_mem clCreateFromGLBuffer( cl_context context,
            cl_mem_flags flags,
            GLuint bufobj,
            out ErrorCode errcode_ret );
        [DllImport( Configuration.Library )]
        public extern static cl_mem clCreateFromGLTexture2D( cl_context context,
            cl_mem_flags flags,
            GLenum target,
            GLint mipLevel,
            GLuint texture,
            out ErrorCode errcode_ret );
        [DllImport( Configuration.Library )]
        public extern static cl_mem clCreateFromGLTexture3D( cl_context context,
            cl_mem_flags flags,
            GLenum target,
            GLint mipLevel,
            GLuint texture,
            out ErrorCode errcode_ret );
        [DllImport( Configuration.Library )]
        public extern static cl_mem clCreateFromGLRenderbuffer( cl_context context,
            cl_mem_flags flags,
            GLuint renderBuffer,
            out ErrorCode errcode_ret );
        [DllImport( Configuration.Library )]
        public extern static ErrorCode clGetGLObjectInfo( cl_mem memobj,
            out cl_gl_object_type gl_object_type,
            out GLuint gl_object_name );
        [DllImport( Configuration.Library )]
        public extern static ErrorCode clGetGLTextureInfo( cl_mem memobj,
            cl_gl_texture_info param_name,
            IntPtr param_value_size,
            void* param_value,
            out IntPtr param_value_size_ret );
        [DllImport( Configuration.Library )]
        public extern static ErrorCode clEnqueueAcquireGLObjects( cl_command_queue command_queue,
            cl_uint num_objects,
            [In] cl_mem[] mem_objects,
            cl_uint num_events_in_wait_list,
            [In] cl_event[] event_wait_list,
            cl_event* _event );
        [DllImport( Configuration.Library )]
        public extern static ErrorCode clEnqueueReleaseGLObjects( cl_command_queue command_queue,
            cl_uint num_objects,
            [In] cl_mem[] mem_objects,
            cl_uint num_events_in_wait_list,
            [In] cl_event[] event_wait_list,
            cl_event* _event );

        public override cl_mem CreateFromGLBuffer(cl_context context, cl_mem_flags flags, GLuint bufobj, out ErrorCode errcode_ret)
        {
            return clCreateFromGLBuffer(context, flags, bufobj, out errcode_ret);
        }
        public override cl_mem CreateFromGLTexture2D(cl_context context, cl_mem_flags flags, GLenum target, GLint mipLevel, GLuint texture, out ErrorCode errcode_ret)
        {
            return clCreateFromGLTexture2D(context, flags, target, mipLevel, texture, out errcode_ret);
        }
        public override cl_mem CreateFromGLTexture3D(cl_context context, cl_mem_flags flags, GLenum target, GLint mipLevel, GLuint texture, out ErrorCode errcode_ret)
        {
            return clCreateFromGLTexture3D(context, flags, target, mipLevel, texture, out errcode_ret);
        }
        public override cl_mem CreateFromGLRenderbuffer(cl_context context, cl_mem_flags flags, GLuint renderbuffer, out ErrorCode errcode_ret)
        {
            return clCreateFromGLRenderbuffer(context, flags, renderbuffer, out errcode_ret);
        }
        public override ErrorCode GetGLObjectInfo(cl_mem memobj, out cl_gl_object_type gl_object_type, out GLuint gl_object_name)
        {
            return clGetGLObjectInfo(memobj, out gl_object_type, out gl_object_name);
        }
        public override ErrorCode GetGLTextureInfo(cl_mem memobj, cl_gl_texture_info param_name, IntPtr param_value_size, void* param_value, out IntPtr param_value_size_ret)
        {
            return clGetGLTextureInfo(memobj, param_name, param_value_size, param_value, out param_value_size_ret);
        }
        public override ErrorCode EnqueueAcquireGLObjects(cl_command_queue command_queue, cl_uint num_objects, cl_mem[] mem_objects, cl_uint num_events_in_wait_list, cl_event[] event_wait_list, cl_event* _event)
        {
            return clEnqueueAcquireGLObjects(command_queue, num_objects, mem_objects, num_events_in_wait_list, event_wait_list, _event);
        }
        public override ErrorCode EnqueueReleaseGLObjects(cl_command_queue command_queue, cl_uint num_objects, cl_mem[] mem_objects, cl_uint num_events_in_wait_list, cl_event[] event_wait_list, cl_event* _event)
        {
            return clEnqueueAcquireGLObjects(command_queue, num_objects, mem_objects, num_events_in_wait_list, event_wait_list, _event);
        }

        #endregion


        // Extension function access
        [DllImport(Configuration.Library)]
        public extern static IntPtr clGetExtensionFunctionAddress(string func_name);

        public override IntPtr GetExtensionFunctionAddress( string func_name )
        {
            return clGetExtensionFunctionAddress(func_name);
        }

#if false
    //    // Profiling APIs
    //    public extern static cl_int clGetEventProfilingInfo(cl_event _event, cl_profiling_info param_name, size_t param_value_size, void* param_value, size_t* param_value_size_ret);

        _clCreateFromD3D10Buffer@20
        _clCreateFromD3D9Buffer@24
        _clCreateImageFromD3D10Resource@20
        _clCreateImageFromD3D9Resource@24
#endif
    }
}

//int __stdcall clEnqueueAcquireExternalObjects(struct _cl_command_queue *,unsigned int,struct _cl_mem * const *,unsigned int,struct _cl_event * const *,struct _cl_event * *)	0x10001890	0x00001890	1 (0x1)	OpenCL.dll	C:\src\Vision\Vision\bin\x86\Debug\OpenCL.dll	Exported Function	
//int __stdcall clEnqueueReleaseExternalObjects(struct _cl_command_queue *,unsigned int,struct _cl_mem * const *,unsigned int,struct _cl_event * const *,struct _cl_event * *)	0x10001ad0	0x00001ad0	2 (0x2)	OpenCL.dll	C:\src\Vision\Vision\bin\x86\Debug\OpenCL.dll	Exported Function	
//_clCreatePerfCounterAMD@12	0x10003bb0	0x00003bb0	30 (0x1e)	OpenCL.dll	C:\src\Vision\Vision\bin\x86\Debug\OpenCL.dll	Exported Function	
//_clEnqueueBeginPerfCounterAMD@24	0x10003620	0x00003620	36 (0x24)	OpenCL.dll	C:\src\Vision\Vision\bin\x86\Debug\OpenCL.dll	Exported Function	
//_clEnqueueEndPerfCounterAMD@24	0x10003870	0x00003870	41 (0x29)	OpenCL.dll	C:\src\Vision\Vision\bin\x86\Debug\OpenCL.dll	Exported Function	
//_clGetEventProfilingInfo@20	0x10007cb0	0x00007cb0	62 (0x3e)	OpenCL.dll	C:\src\Vision\Vision\bin\x86\Debug\OpenCL.dll	Exported Function	
//_clGetExtensionFunctionAddress@4	0x10004e40	0x00004e40	63 (0x3f)	OpenCL.dll	C:\src\Vision\Vision\bin\x86\Debug\OpenCL.dll	Exported Function	
//_clGetPerfCounterInfoAMD@20	0x100029e0	0x000029e0	70 (0x46)	OpenCL.dll	C:\src\Vision\Vision\bin\x86\Debug\OpenCL.dll	Exported Function	
//_clReleasePerfCounterAMD@4	0x10002840	0x00002840	82 (0x52)	OpenCL.dll	C:\src\Vision\Vision\bin\x86\Debug\OpenCL.dll	Exported Function	
//_clRetainPerfCounterAMD@4	0x10002910	0x00002910	90 (0x5a)	OpenCL.dll	C:\src\Vision\Vision\bin\x86\Debug\OpenCL.dll	Exported Function	
