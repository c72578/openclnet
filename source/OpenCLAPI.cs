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
using System.Security;

namespace OpenCLNet
{
    #region Using statements

#warning Todo: map these things to native types if they aren't IntPtrs
    using cl_platform_id = IntPtr;
    using cl_device_id=IntPtr;
    using cl_context=IntPtr;
    using cl_command_queue=IntPtr;
    using cl_mem=IntPtr;
    using cl_program=IntPtr;
    using cl_kernel=IntPtr;
    using cl_event=IntPtr;
    using cl_sampler=IntPtr;

    using cl_bool=UInt32;
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
    using cl_gl_context_info=UInt32;
    using GLuint=UInt32;
    using GLint=Int32;
    using GLenum=Int32;

    using cl_device_partition_property_ext=UInt64;
    
    using cl_d3d10_device_source_khr = UInt32;
    using cl_d3d10_device_set_khr = UInt32;
    using UINT = UInt32;
    using ID3D10Buffer = IntPtr;
    using ID3D10Texture2D = IntPtr;
    using ID3D10Texture3D = IntPtr;
    using cl_device_partition_property = IntPtr;
    using cl_device_affinity_domain = UInt64;
    using cl_buffer_create_type = UInt32;

    #endregion

    /// <summary>
    /// OpenCLAPI - native bindings
    /// </summary>
    [SuppressUnmanagedCodeSecurity()]
    unsafe public static class OpenCLAPI
    {
        internal static class Configuration
        {
            public const string Library = "opencl.dll";
        }
    
        static OpenCLAPI()
        {
        }

        #region Platform API

        // Platform API
        [DllImport(Configuration.Library)]
        public extern static int clGetPlatformIDs(uint num_entries, [Out] [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 0)] cl_platform_id[] platforms, out uint num_platforms);
        [DllImport(Configuration.Library)]
        public extern static int clGetPlatformIDs(uint num_entries, IntPtr* pPlatforms, out uint num_platforms);

        [DllImport(Configuration.Library)]
        public extern static int clGetPlatformInfo(cl_platform_id platform, cl_platform_info param_name, IntPtr param_value_size, void* param_value, out IntPtr param_value_size_ret);

        #endregion

        #region Device API

        // Device APIs
        [DllImport(Configuration.Library)]
        public extern static int clGetDeviceIDs(cl_platform_id platform, DeviceType device_type, uint num_entries, [Out][MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 2)]cl_device_id[] devices, out uint num_devices);
        [DllImport(Configuration.Library)]
        public extern static int clGetDeviceIDs(IntPtr platform, ulong device_type, uint num_entries, IntPtr* pDevices, out uint num_devices);

        [DllImport(Configuration.Library)]
        public extern static int clGetDeviceInfo(cl_device_id device, cl_device_info param_name, IntPtr param_value_size, void* param_value, out IntPtr param_value_size_ret);

        /// <summary>
        /// OpenCL 1.2
        /// </summary>
        /// <param name="device"></param>
        /// <param name="properties"></param>
        /// <param name="num_devices"></param>
        /// <param name="out_devices"></param>
        /// <param name="num_devices_ret"></param>
        /// <returns></returns>
        [DllImport(Configuration.Library)]
        public extern static ErrorCode clCreateSubDevices(cl_device_id in_device,
            [In] byte[] properties,
            uint num_entries,
            [In] [Out] [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 4)] cl_device_id[] out_devices,
            out uint num_devices);
        [DllImport(Configuration.Library)]
        public extern static ErrorCode clCreateSubDevices(cl_device_id in_device,
            byte* pProperties,
            uint num_entries,
            IntPtr* pOutDevices,
            out uint num_devices);

        /// <summary>
        /// OpenCL 1.2
        /// </summary>
        /// <param name="device"></param>
        /// <returns></returns>
        [DllImport(Configuration.Library)]
        public extern static ErrorCode clRetainDevice(cl_device_id device);

        /// <summary>
        /// OpenCL 1.2
        /// </summary>
        /// <param name="device"></param>
        /// <returns></returns>
        [DllImport(Configuration.Library)]
        public extern static ErrorCode clReleaseDevice(cl_device_id device);
    
        #endregion

        #region Context API

        // Context APIs 
        [DllImport(Configuration.Library)]
        public extern static cl_context clCreateContext([In] cl_context_properties[] properties, uint num_devices, [In]cl_device_id[] devices, ContextNotify pfn_notify, IntPtr user_data, [MarshalAs(UnmanagedType.I4)] out ErrorCode errcode_ret);
        [DllImport(Configuration.Library)]
        public extern static cl_context clCreateContext(IntPtr* pProperties, uint num_devices, IntPtr* pDevices, ContextNotify pfn_notify, IntPtr user_data, [MarshalAs(UnmanagedType.I4)]out ErrorCode errcode_ret);
        [DllImport(Configuration.Library)]
        public extern static cl_context clCreateContextFromType([In] cl_context_properties[] properties, DeviceType device_type, ContextNotify pfn_notify, IntPtr user_data, [MarshalAs(UnmanagedType.I4)] out ErrorCode errcode_ret);
        [DllImport(Configuration.Library)]
        public extern static cl_context clCreateContextFromType(IntPtr* properties, DeviceType device_type, ContextNotify pfn_notify, IntPtr user_data, [MarshalAs(UnmanagedType.I4)] out ErrorCode errcode_ret);
        [DllImport(Configuration.Library)]
        public extern static int clRetainContext(cl_context context);
        [DllImport(Configuration.Library)]
        public extern static int clReleaseContext(cl_context context);
        [DllImport(Configuration.Library)]
        public extern static int clGetContextInfo(cl_context context, cl_context_info param_name, IntPtr param_value_size, void* param_value, out IntPtr param_value_size_ret);

        #endregion

        #region Command Queue API

        // Command Queue APIs
        [DllImport(Configuration.Library)]
        public extern static IntPtr clCreateCommandQueue(cl_context context, cl_device_id device, cl_command_queue_properties properties, [MarshalAs(UnmanagedType.I4)] out ErrorCode errcode_ret);
        [DllImport(Configuration.Library)]
        public extern static ErrorCode clRetainCommandQueue(cl_command_queue command_queue);
        [DllImport(Configuration.Library)]
        public extern static ErrorCode clReleaseCommandQueue(cl_command_queue command_queue);
        [DllImport(Configuration.Library)]
        public extern static ErrorCode clGetCommandQueueInfo(cl_command_queue command_queue, cl_command_queue_info param_name, IntPtr param_value_size, void* param_value, out IntPtr param_value_size_ret);
        [DllImport(Configuration.Library)]
        [Obsolete("Function deprecated in OpenCL 1.1 due to being inherently unsafe", false)]
        public extern static ErrorCode clSetCommandQueueProperty(cl_command_queue command_queue, cl_command_queue_properties properties, [MarshalAs(UnmanagedType.I4)]bool enable, out cl_command_queue_properties old_properties);

        #endregion

        #region Memory Object API

        // Memory Object APIs
        [DllImport(Configuration.Library)]
        public extern static cl_mem clCreateBuffer(cl_context context, cl_mem_flags flags, IntPtr size, void* host_ptr, out ErrorCode errcode_ret);
        [DllImport(Configuration.Library)]
        [Obsolete("Deprecated in OpenCL 1.2. See clCreateImage",false)]
        public extern static cl_mem clCreateImage2D(cl_context context, cl_mem_flags flags, ImageFormat* image_format, IntPtr image_width, IntPtr image_height, IntPtr image_row_pitch, void* host_ptr, out ErrorCode errcode_ret);
        [DllImport(Configuration.Library)]
        [Obsolete("Deprecated in OpenCL 1.2. See clCreateImage",false)]
        public extern static cl_mem clCreateImage3D(cl_context context, cl_mem_flags flags, ImageFormat* image_format, IntPtr image_width, IntPtr image_height, IntPtr image_depth, IntPtr image_row_pitch, IntPtr image_slice_pitch, void* host_ptr, out ErrorCode errcode_ret);
        [DllImport(Configuration.Library)]
        public extern static ErrorCode clRetainMemObject(cl_mem memobj);
        [DllImport(Configuration.Library)]
        public extern static ErrorCode clReleaseMemObject(cl_mem memobj);
        [DllImport(Configuration.Library)]
        public extern static ErrorCode clGetSupportedImageFormats(cl_context context,
            cl_mem_flags flags,
            cl_mem_object_type image_type,
            uint num_entries,
            [Out][MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 3)] ImageFormat[] image_formats,
            out uint num_image_formats);
        [DllImport(Configuration.Library)]
        public extern static ErrorCode clGetMemObjectInfo(cl_mem memobj, cl_mem_info param_name, IntPtr param_value_size, void* param_value, out IntPtr param_value_size_ret);
        [DllImport(Configuration.Library)]
        public extern static ErrorCode clGetImageInfo(cl_mem image, cl_image_info param_name, IntPtr param_value_size, void* param_value, out IntPtr param_value_size_ret);

        // OpenCL 1.1
        [DllImport(Configuration.Library)]
        public extern static cl_mem clCreateSubBuffer(cl_mem buffer, cl_mem_flags flags, BufferCreateType buffer_create_type, void* buffer_create_info, out ErrorCode errcode_ret );
        [DllImport(Configuration.Library)]
        public extern static ErrorCode clSetMemObjectDestructorCallback(cl_mem memobj, void* pfn_notify, void* user_data);
        
        // OpenCL 1.2
        [DllImport(Configuration.Library)]
        public extern static cl_mem clCreateSubBuffer(cl_mem buffer, cl_mem_flags flags, cl_buffer_create_type buffer_create_type, void* buffer_create_info, int* errcode_ret);
        #endregion

        #region Sampler API

        // Sampler APIs
        [DllImport(Configuration.Library)]
        public extern static cl_sampler clCreateSampler(cl_context context, cl_bool normalized_coords, cl_addressing_mode addressing_mode, cl_filter_mode filter_mode, out ErrorCode errcode_ret);
        [DllImport(Configuration.Library)]
        public extern static ErrorCode clRetainSampler(cl_sampler sampler);
        [DllImport(Configuration.Library)]
        public extern static ErrorCode clReleaseSampler(cl_sampler sampler);
        [DllImport(Configuration.Library)]
        public extern static ErrorCode clGetSamplerInfo(cl_sampler sampler, cl_sampler_info param_name, IntPtr param_value_size, void* param_value, out IntPtr param_value_size_ret);

        #endregion

        #region Program Object API

        // Program Object APIs
        [DllImport(Configuration.Library)]
        public extern static cl_program clCreateProgramWithSource(cl_context context,
            uint count,
            [In] string[] strings,
            [In] IntPtr[] lengths,
            [MarshalAs(UnmanagedType.I4)] out ErrorCode errcode_ret);
        [DllImport(Configuration.Library)]
        public extern static cl_program clCreateProgramWithBinary(cl_context context,
            uint num_devices,
            [In] cl_device_id[] device_list,
            [In] IntPtr[] lengths,
            [In] IntPtr[] pBinaries,
            [Out][MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 1)] int[] binary_status,
            [MarshalAs(UnmanagedType.I4)] out ErrorCode errcode_ret);
        [DllImport(Configuration.Library)]
        public extern static int clRetainProgram(cl_program program);
        [DllImport(Configuration.Library)]
        public extern static int clReleaseProgram(cl_program program);
        [DllImport(Configuration.Library)]
        public extern static int clBuildProgram(cl_program program,
            uint num_devices,
            [In] cl_device_id[] device_list,
            string options,
            ProgramNotify pfn_notify,
            IntPtr user_data);
        [DllImport(Configuration.Library)]
        [Obsolete("Deprecated in OpenCL 1.2. Use clUnloadPlatformCompiler.")]
        public extern static int clUnloadCompiler();
        [DllImport(Configuration.Library)]
        public extern static int clGetProgramInfo(cl_program program, cl_program_info param_name, IntPtr param_value_size, void* param_value, out IntPtr param_value_size_ret);
        [DllImport(Configuration.Library)]
        public extern static int clGetProgramBuildInfo(cl_program program, cl_device_id device, cl_program_build_info param_name, IntPtr param_value_size, void* param_value, out IntPtr param_value_size_ret);

        // OpenCL 1.2
        /// <summary>
        /// OpenCL 1.2
        /// </summary>
        /// <param name="context"></param>
        /// <param name="num_devices"></param>
        /// <param name="device_list"></param>
        /// <param name="kernel_names"></param>
        /// <param name="errcode_ret"></param>
        /// <returns></returns>
        [DllImport(Configuration.Library)]
        public extern static cl_program clCreateProgramWithBuiltInKernels(cl_context context, uint num_devices, cl_device_id* device_list, char* kernel_names, int* errcode_ret );
        // public extern static int clCompileProgram(cl_program program,uint num_devices,const cl_device_id * device_list,const char * options, uint num_input_headers,const cl_program * input_headers,const char ** header_include_names,void (CL_CALLBACK * pfn_notify )(cl_program program, void * user_data ),void * user_data ) ;
        //public extern static cl_program clLinkProgram(cl_context context,uint num_devices,const cl_device_id * device_list,const char * options, uint num_input_programs,const cl_program * input_programs,void (CL_CALLBACK * pfn_notify)(cl_program program, void * user_data),void * user_data,int * errcode_ret ) ;
        
        /// <summary>
        /// OpenCL 1.2
        /// </summary>
        /// <param name="platform"></param>
        /// <returns></returns>
        [DllImport(Configuration.Library)]
        public extern static int clUnloadPlatformCompiler(cl_platform_id platform);

        #endregion

        #region Kernel Object API

        // Kernel Object APIs
        [DllImport(Configuration.Library)]
        public extern static cl_kernel clCreateKernel(cl_program program, string kernel_name, out ErrorCode errcode_ret);
        [DllImport(Configuration.Library)]
        public extern static ErrorCode clCreateKernelsInProgram(cl_program program,
            uint num_kernels,
            [Out][MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 1)]cl_kernel[] kernels,
            out uint num_kernels_ret);
        [DllImport(Configuration.Library)]
        public extern static ErrorCode clRetainKernel(cl_kernel kernel);
        [DllImport(Configuration.Library)]
        public extern static ErrorCode clReleaseKernel(cl_kernel kernel);
        [DllImport(Configuration.Library)]
        public extern static ErrorCode clSetKernelArg(cl_kernel kernel, uint arg_index, IntPtr arg_size, void* arg_value);
        [DllImport(Configuration.Library)]
        public extern static ErrorCode clGetKernelInfo(cl_kernel kernel, cl_kernel_info param_name, IntPtr param_value_size, void* param_value, out IntPtr param_value_size_ret);
        [DllImport(Configuration.Library)]
        public extern static ErrorCode clGetKernelWorkGroupInfo(cl_kernel kernel, cl_device_id device, cl_kernel_work_group_info param_name, IntPtr param_value_size, void* param_value, out IntPtr param_value_size_ret);

        // OpenCL 1.2
        //public extern static int clGetKernelArgInfo(cl_kernel kernel,uint arg_indx,cl_kernel_arg_info param_name,size_t param_value_size,void * param_value,size_t * param_value_size_ret);

        #endregion

        #region Enqueued Commands API

        // Enqueued Commands APIs
        [DllImport(Configuration.Library)]
        public extern static ErrorCode clEnqueueReadBuffer(cl_command_queue command_queue,
            cl_mem buffer,
            cl_bool blocking_read,
            IntPtr offset,
            IntPtr cb,
            void* ptr,
            uint num_events_in_wait_list,
            [In] [MarshalAs(UnmanagedType.LPArray)] cl_event[] event_wait_list,
            cl_event* _event);
        [DllImport(Configuration.Library)]
        public extern static ErrorCode clEnqueueReadBuffer(cl_command_queue command_queue,
            cl_mem buffer,
            cl_bool blocking_read,
            IntPtr offset,
            IntPtr cb,
            void* ptr,
            uint num_events_in_wait_list,
            IntPtr* event_wait_list,
            cl_event* _event);


        [DllImport(Configuration.Library)]
        public extern static ErrorCode clEnqueueWriteBuffer(cl_command_queue command_queue,
            cl_mem buffer,
            cl_bool blocking_write,
            IntPtr offset,
            IntPtr cb,
            void* ptr,
            uint num_events_in_wait_list,
            [In] [MarshalAs(UnmanagedType.LPArray)] cl_event[] event_wait_list,
            cl_event* _event);
        [DllImport(Configuration.Library)]
        public extern static ErrorCode clEnqueueWriteBuffer(cl_command_queue command_queue,
            cl_mem buffer,
            cl_bool blocking_write,
            IntPtr offset,
            IntPtr cb,
            void* ptr,
            uint num_events_in_wait_list,
            IntPtr* event_wait_list,
            cl_event* _event);

        [DllImport(Configuration.Library)]
        public extern static ErrorCode clEnqueueCopyBuffer(cl_command_queue command_queue,
            cl_mem src_buffer,
            cl_mem dst_buffer,
            IntPtr src_offset,
            IntPtr dst_offset,
            IntPtr cb,
            uint num_events_in_wait_list,
            [In] [MarshalAs(UnmanagedType.LPArray)] cl_event[] event_wait_list,
            cl_event* _event);
        [DllImport(Configuration.Library)]
        public extern static ErrorCode clEnqueueCopyBuffer(cl_command_queue command_queue,
            cl_mem src_buffer,
            cl_mem dst_buffer,
            IntPtr src_offset,
            IntPtr dst_offset,
            IntPtr cb,
            uint num_events_in_wait_list,
            IntPtr* event_wait_list,
            cl_event* _event);

        [DllImport(Configuration.Library)]
        public extern static ErrorCode clEnqueueReadImage(cl_command_queue command_queue,
            cl_mem image,
            cl_bool blocking_read,
            [In] [MarshalAs(UnmanagedType.LPArray, SizeConst = 3)]IntPtr[] origin,
            [In] [MarshalAs(UnmanagedType.LPArray, SizeConst = 3)]IntPtr[] region,
            IntPtr row_pitch,
            IntPtr slice_pitch,
            void* ptr,
            uint num_events_in_wait_list,
            [In] [MarshalAs(UnmanagedType.LPArray)] cl_event[] event_wait_list,
            cl_event* _event);
        [DllImport(Configuration.Library)]
        public extern static ErrorCode clEnqueueReadImage(cl_command_queue command_queue,
            cl_mem image,
            cl_bool blocking_read,
            IntPtr* origin,
            IntPtr* region,
            IntPtr row_pitch,
            IntPtr slice_pitch,
            void* ptr,
            uint num_events_in_wait_list,
            IntPtr* event_wait_list,
            cl_event* _event);

        [DllImport(Configuration.Library)]
        public extern static ErrorCode clEnqueueWriteImage(cl_command_queue command_queue,
            cl_mem image,
            cl_bool blocking_write,
            [In] [MarshalAs(UnmanagedType.LPArray, SizeConst = 3)]IntPtr[] origin,
            [In] [MarshalAs(UnmanagedType.LPArray, SizeConst = 3)]IntPtr[] region,
            IntPtr input_row_pitch,
            IntPtr input_slice_pitch,
            void* ptr,
            uint num_events_in_wait_list,
            [In] [MarshalAs(UnmanagedType.LPArray)] cl_event[] event_wait_list,
            cl_event* _event);
        [DllImport(Configuration.Library)]
        public extern static ErrorCode clEnqueueWriteImage(cl_command_queue command_queue,
            cl_mem image,
            cl_bool blocking_write,
            IntPtr* origin,
            IntPtr* region,
            IntPtr input_row_pitch,
            IntPtr input_slice_pitch,
            void* ptr,
            uint num_events_in_wait_list,
            IntPtr* event_wait_list,
            cl_event* _event);

        [DllImport(Configuration.Library)]
        public extern static ErrorCode clEnqueueCopyImage(cl_command_queue command_queue,
            cl_mem src_image,
            cl_mem dst_image,
            [In] [MarshalAs(UnmanagedType.LPArray, SizeConst = 3)]IntPtr[] src_origin,
            [In] [MarshalAs(UnmanagedType.LPArray, SizeConst = 3)]IntPtr[] dst_origin,
            [In] [MarshalAs(UnmanagedType.LPArray, SizeConst = 3)]IntPtr[] region,
            uint num_events_in_wait_list,
            [In] [MarshalAs(UnmanagedType.LPArray)] cl_event[] event_wait_list,
            cl_event* _event);
        [DllImport(Configuration.Library)]
        public extern static ErrorCode clEnqueueCopyImage(cl_command_queue command_queue,
            cl_mem src_image,
            cl_mem dst_image,
            IntPtr* src_origin,
            IntPtr* dst_origin,
            IntPtr* region,
            uint num_events_in_wait_list,
            IntPtr* event_wait_list,
            cl_event* _event);


        [DllImport(Configuration.Library)]
        public extern static ErrorCode clEnqueueCopyImageToBuffer(cl_command_queue command_queue,
            cl_mem src_image,
            cl_mem dst_buffer,
            [In] [MarshalAs(UnmanagedType.LPArray, SizeConst = 3)]IntPtr[] src_origin,
            [In] [MarshalAs(UnmanagedType.LPArray, SizeConst = 3)]IntPtr[] region,
            IntPtr dst_offset,
            uint num_events_in_wait_list,
            [In] [MarshalAs(UnmanagedType.LPArray)] cl_event[] event_wait_list,
            cl_event* _event);
        [DllImport(Configuration.Library)]
        public extern static ErrorCode clEnqueueCopyImageToBuffer(cl_command_queue command_queue,
            cl_mem src_image,
            cl_mem dst_buffer,
            IntPtr* src_origin,
            IntPtr* region,
            IntPtr dst_offset,
            uint num_events_in_wait_list,
            IntPtr* event_wait_list,
            cl_event* _event);

        [DllImport(Configuration.Library)]
        public extern static ErrorCode clEnqueueCopyBufferToImage(cl_command_queue command_queue,
            cl_mem src_buffer,
            cl_mem dst_image,
            IntPtr src_offset,
            [In] [MarshalAs(UnmanagedType.LPArray, SizeConst = 3)]IntPtr[] dst_origin,
            [In] [MarshalAs(UnmanagedType.LPArray, SizeConst = 3)]IntPtr[] region,
            uint num_events_in_wait_list,
            [In] [MarshalAs(UnmanagedType.LPArray)] cl_event[] event_wait_list,
            cl_event* _event);
        [DllImport(Configuration.Library)]
        public extern static ErrorCode clEnqueueCopyBufferToImage(cl_command_queue command_queue,
            cl_mem src_buffer,
            cl_mem dst_image,
            IntPtr src_offset,
            IntPtr* dst_origin,
            IntPtr* region,
            uint num_events_in_wait_list,
            IntPtr* event_wait_list,
            cl_event* _event);


        [DllImport(Configuration.Library)]
        public extern static void* clEnqueueMapBuffer(cl_command_queue command_queue,
            cl_mem buffer,
            cl_bool blocking_map,
            cl_map_flags map_flags,
            IntPtr offset,
            IntPtr cb,
            uint num_events_in_wait_list,
            [In] [MarshalAs(UnmanagedType.LPArray)] cl_event[] event_wait_list,
            cl_event* _event,
            out ErrorCode errcode_ret);
        [DllImport(Configuration.Library)]
        public extern static void* clEnqueueMapBuffer(cl_command_queue command_queue,
            cl_mem buffer,
            cl_bool blocking_map,
            cl_map_flags map_flags,
            IntPtr offset,
            IntPtr cb,
            uint num_events_in_wait_list,
            IntPtr* event_wait_list,
            cl_event* _event,
            out ErrorCode errcode_ret);

        [DllImport(Configuration.Library)]
        public extern static void* clEnqueueMapImage(cl_command_queue command_queue,
            cl_mem image,
            cl_bool blocking_map,
            cl_map_flags map_flags,
            [In] [MarshalAs(UnmanagedType.LPArray, SizeConst = 3)]IntPtr[] origin,
            [In] [MarshalAs(UnmanagedType.LPArray, SizeConst = 3)]IntPtr[] region,
            out IntPtr image_row_pitch,
            out IntPtr image_slice_pitch,
            uint num_events_in_wait_list,
            [In] [MarshalAs(UnmanagedType.LPArray)] cl_event[] event_wait_list,
            cl_event* _event,
            out ErrorCode errcode_ret);
        [DllImport(Configuration.Library)]
        public extern static void* clEnqueueMapImage(cl_command_queue command_queue,
            cl_mem image,
            cl_bool blocking_map,
            cl_map_flags map_flags,
            IntPtr* origin,
            IntPtr* region,
            out IntPtr image_row_pitch,
            out IntPtr image_slice_pitch,
            uint num_events_in_wait_list,
            IntPtr* event_wait_list,
            cl_event* _event,
            out ErrorCode errcode_ret);

        [DllImport(Configuration.Library)]
        public extern static ErrorCode clEnqueueUnmapMemObject(cl_command_queue command_queue,
            cl_mem memobj,
            void* mapped_ptr,
            uint num_events_in_wait_list,
            [In] [MarshalAs(UnmanagedType.LPArray)] cl_event[] event_wait_list,
            cl_event* _event);
        [DllImport(Configuration.Library)]
        public extern static ErrorCode clEnqueueUnmapMemObject(cl_command_queue command_queue,
            cl_mem memobj,
            void* mapped_ptr,
            uint num_events_in_wait_list,
            IntPtr* event_wait_list,
            cl_event* _event);

        [DllImport(Configuration.Library)]
        public extern static ErrorCode clEnqueueNDRangeKernel(cl_command_queue command_queue,
            cl_kernel kernel,
            uint work_dim,
            [In] [MarshalAs(UnmanagedType.LPArray)] IntPtr[] global_work_offset,
            [In] [MarshalAs(UnmanagedType.LPArray)] IntPtr[] global_work_size,
            [In] [MarshalAs(UnmanagedType.LPArray)] IntPtr[] local_work_size,
            uint num_events_in_wait_list,
            [In] [MarshalAs(UnmanagedType.LPArray)] cl_event[] event_wait_list,
            cl_event* _event);
        [DllImport(Configuration.Library)]
        public extern static ErrorCode clEnqueueNDRangeKernel(cl_command_queue command_queue,
            cl_kernel kernel,
            uint work_dim,
            IntPtr* global_work_offset,
            IntPtr* global_work_size,
            IntPtr* local_work_size,
            uint num_events_in_wait_list,
            IntPtr* event_wait_list,
            cl_event* _event);

        [DllImport(Configuration.Library)]
        public extern static ErrorCode clEnqueueTask(cl_command_queue command_queue,
            cl_kernel kernel,
            uint num_events_in_wait_list,
            [In] [MarshalAs(UnmanagedType.LPArray)] cl_event[] event_wait_list,
            cl_event* _event);
        [DllImport(Configuration.Library)]
        public extern static ErrorCode clEnqueueTask(cl_command_queue command_queue,
            cl_kernel kernel,
            uint num_events_in_wait_list,
            IntPtr* event_wait_list,
            cl_event* _event);

        [DllImport(Configuration.Library)]
        public extern static ErrorCode clEnqueueNativeKernel(cl_command_queue command_queue,
            NativeKernelInternal user_func,
            void* args,
            IntPtr cb_args,
            uint num_mem_objects,
            [In] cl_mem[] mem_list,
            [In] [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 4)] IntPtr[] args_mem_loc,
            uint num_events_in_wait_list,
            [In] [MarshalAs(UnmanagedType.LPArray)] cl_event[] event_wait_list,
            cl_event* _event);
        [DllImport(Configuration.Library)]
        public extern static ErrorCode clEnqueueNativeKernel(cl_command_queue command_queue,
            NativeKernelInternal user_func,
            void* args,
            IntPtr cb_args,
            uint num_mem_objects,
            IntPtr* mem_list,
            IntPtr* args_mem_loc,
            uint num_events_in_wait_list,
            IntPtr* event_wait_list,
            cl_event* _event);

        [DllImport(Configuration.Library)]
        [Obsolete("Deprecated in OpenCL 1.2. Use clEnqueueMarkerWithWaitList.",false)]
        public extern static ErrorCode clEnqueueMarker(cl_command_queue command_queue, cl_event* _event);

        [DllImport(Configuration.Library)]
        [Obsolete("Deprecated in OpenCL 1.2. Use clEnqueueMarkerWithWaitList.", false)]
        public extern static ErrorCode clEnqueueWaitForEvents(cl_command_queue command_queue,
            uint num_events,
            [In] [MarshalAs(UnmanagedType.LPArray)] cl_event[] _event_list);
        [DllImport(Configuration.Library)]
        [Obsolete("Deprecated in OpenCL 1.2 Use clEnqueueMarkerWithWaitList.", false)]
        public extern static ErrorCode clEnqueueWaitForEvents(cl_command_queue command_queue,
            uint num_events,
            IntPtr* _event_list);
        
        [DllImport(Configuration.Library)]
        [Obsolete("Deprecated in OpenCL 1.2. Use clEnqueueBarrierWithWaitList.", false)]
        public extern static ErrorCode clEnqueueBarrier(cl_command_queue command_queue);

        // OpenCL 1.1
        [DllImport(Configuration.Library)]
        public extern static ErrorCode clEnqueueReadBufferRect(cl_command_queue command_queue,
                                cl_mem buffer,
                                cl_bool blocking_read,
                                [In] IntPtr[] buffer_offset,
                                [In] IntPtr[] host_offset,
                                [In] IntPtr[] region,
                                IntPtr buffer_row_pitch,
                                IntPtr buffer_slice_pitch,
                                IntPtr host_row_pitch,
                                IntPtr host_slice_pitch,
                                void* ptr,
                                uint num_events_in_wait_list,
                                [In] [MarshalAs(UnmanagedType.LPArray)] cl_event[] event_wait_list,
                                cl_event* _event);
        [DllImport(Configuration.Library)]
        public extern static ErrorCode clEnqueueReadBufferRect(cl_command_queue command_queue,
                                cl_mem buffer,
                                cl_bool blocking_read,
                                IntPtr* buffer_offset,
                                IntPtr* host_offset,
                                IntPtr* region,
                                IntPtr buffer_row_pitch,
                                IntPtr buffer_slice_pitch,
                                IntPtr host_row_pitch,
                                IntPtr host_slice_pitch,
                                void* ptr,
                                uint num_events_in_wait_list,
                                cl_event* event_wait_list,
                                cl_event* _event);

        [DllImport(Configuration.Library)]
        public extern static ErrorCode clEnqueueWriteBufferRect(cl_command_queue command_queue,
                                 cl_mem buffer,
                                 cl_bool blocking_write,
                                 [In] IntPtr[] buffer_offset,
                                 [In] IntPtr[] host_offset,
                                 [In] IntPtr[] region,
                                 IntPtr buffer_row_pitch,
                                 IntPtr buffer_slice_pitch,
                                 IntPtr host_row_pitch,
                                 IntPtr host_slice_pitch,
                                 void* ptr,
                                 uint num_events_in_wait_list,
                                 [In] [MarshalAs(UnmanagedType.LPArray)] cl_event[] _event_wait_list,
                                 cl_event* _event);
        [DllImport(Configuration.Library)]
        public extern static ErrorCode clEnqueueWriteBufferRect(cl_command_queue command_queue,
                                 cl_mem buffer,
                                 cl_bool blocking_write,
                                 IntPtr* buffer_offset,
                                 IntPtr* host_offset,
                                 IntPtr* region,
                                 IntPtr buffer_row_pitch,
                                 IntPtr buffer_slice_pitch,
                                 IntPtr host_row_pitch,
                                 IntPtr host_slice_pitch,
                                 void* ptr,
                                 uint num_events_in_wait_list,
                                 cl_event* _event_wait_list,
                                 cl_event* _event);

        [DllImport(Configuration.Library)]
        public extern static ErrorCode clEnqueueCopyBufferRect(cl_command_queue command_queue,
                                cl_mem src_buffer,
                                cl_mem dst_buffer,
                                [In] IntPtr[] src_origin,
                                [In] IntPtr[] dst_origin,
                                [In] IntPtr[] region,
                                IntPtr src_row_pitch,
                                IntPtr src_slice_pitch,
                                IntPtr dst_row_pitch,
                                IntPtr dst_slice_pitch,
                                uint num_events_in_wait_list,
                                [In] [MarshalAs(UnmanagedType.LPArray)] cl_event[] _event_wait_list,
                                cl_event* _event);
        [DllImport(Configuration.Library)]
        public extern static ErrorCode clEnqueueCopyBufferRect(cl_command_queue command_queue,
                                cl_mem src_buffer,
                                cl_mem dst_buffer,
                                IntPtr* src_origin,
                                IntPtr* dst_origin,
                                IntPtr* region,
                                IntPtr src_row_pitch,
                                IntPtr src_slice_pitch,
                                IntPtr dst_row_pitch,
                                IntPtr dst_slice_pitch,
                                uint num_events_in_wait_list,
                                cl_event* _event_wait_list,
                                cl_event* _event);

        // OpenCL 1.2

        //public extern static int clEnqueueFillBuffer(cl_command_queue command_queue,cl_mem buffer, const void * pattern, size_t pattern_size, size_t offset, size_t size, uint num_events_in_wait_list, const cl_event * event_wait_list, cl_event * _event );
        //public extern static int clEnqueueFillImage(cl_command_queue command_queue,cl_mem image, const void * fill_color, const size_t * origin[3], const size_t * region[3], uint num_events_in_wait_list, const cl_event * event_wait_list, cl_event * _event) ;
        // extern  int clEnqueueMigrateMemObjects(cl_command_queue command_queue,uint num_mem_objects,const cl_mem * mem_objects,cl_mem_migration_flags flags ,uint  num_events_in_wait_list,const cl_event * event_wait_list,cl_event * event ) ;
        // extern  int clEnqueueMarkerWithWaitList(cl_command_queue command_queue,uint num_events_in_wait_list,const cl_event * event_wait_list,cl_event * event);
        // extern  int clEnqueueBarrierWithWaitList(cl_command_queue command_queue,uint num_events_in_wait_list,const cl_event * event_wait_list,cl_event * event);
        // extern  int clSetPrintfCallback(cl_context context,void (CL_CALLBACK * pfn_notify)(cl_context program, uint printf_data_len, char * printf_data_ptr, void * user_data),void * user_data);

        #endregion

        #region Flush and Finish API

        [DllImport(Configuration.Library)]
        public extern static ErrorCode clFlush(cl_command_queue command_queue);
        [DllImport(Configuration.Library)]
        public extern static ErrorCode clFinish(cl_command_queue command_queue);

        #endregion

        #region Event Object API

        [DllImport(Configuration.Library)]
        public extern static ErrorCode clWaitForEvents(uint num_events,
            [In] [MarshalAs(UnmanagedType.LPArray)] cl_event[] event_wait_list);
        [DllImport(Configuration.Library)]
        public extern static ErrorCode clWaitForEvents(uint num_events,
            IntPtr* event_wait_list);
        
        [DllImport(Configuration.Library)]
        public extern static ErrorCode clGetEventInfo(cl_event _event,
            cl_event_info param_name,
            IntPtr param_value_size,
            void* param_value,
            out IntPtr param_value_size_ret);
        
        [DllImport(Configuration.Library)]
        public extern static ErrorCode clRetainEvent(cl_event _event);
        
        [DllImport(Configuration.Library)]
        public extern static ErrorCode clReleaseEvent(cl_event _event);

        // OpenCL 1.1
        [DllImport(Configuration.Library)]
        public extern static cl_event clCreateUserEvent(cl_context context, out ErrorCode errcode_ret);
        [DllImport(Configuration.Library)]
        public extern static ErrorCode clSetUserEventStatus(cl_event _event, ExecutionStatus execution_status);
        [DllImport(Configuration.Library)]
        public extern static ErrorCode clSetEventCallback(cl_event _event, int command_exec_callback_type, EventNotifyInternal pfn_notify, IntPtr user_data);
        #endregion

        #region GLObject API

        [DllImport(Configuration.Library)]
        public extern static cl_mem clCreateFromGLBuffer(cl_context context,
            cl_mem_flags flags,
            GLuint bufobj,
            out ErrorCode errcode_ret);
        [DllImport(Configuration.Library)]
        public extern static cl_mem clCreateFromGLTexture2D(cl_context context,
            cl_mem_flags flags,
            GLenum target,
            GLint mipLevel,
            GLuint texture,
            out ErrorCode errcode_ret);
        [DllImport(Configuration.Library)]
        public extern static cl_mem clCreateFromGLTexture3D(cl_context context,
            cl_mem_flags flags,
            GLenum target,
            GLint mipLevel,
            GLuint texture,
            out ErrorCode errcode_ret);
        [DllImport(Configuration.Library)]
        public extern static cl_mem clCreateFromGLRenderbuffer(cl_context context,
            cl_mem_flags flags,
            GLuint renderBuffer,
            out ErrorCode errcode_ret);
        [DllImport(Configuration.Library)]
        public extern static ErrorCode clGetGLObjectInfo(cl_mem memobj,
            out cl_gl_object_type gl_object_type,
            out GLuint gl_object_name);
        [DllImport(Configuration.Library)]
        public extern static ErrorCode clGetGLTextureInfo(cl_mem memobj,
            cl_gl_texture_info param_name,
            IntPtr param_value_size,
            void* param_value,
            out IntPtr param_value_size_ret);
        [DllImport(Configuration.Library)]
        public extern static ErrorCode clEnqueueAcquireGLObjects(cl_command_queue command_queue,
            uint num_objects,
            [In] cl_mem[] mem_objects,
            uint num_events_in_wait_list,
            [In] cl_event[] event_wait_list,
            cl_event* _event);
        [DllImport(Configuration.Library)]
        public extern static ErrorCode clEnqueueReleaseGLObjects(cl_command_queue command_queue,
            uint num_objects,
            [In] cl_mem[] mem_objects,
            uint num_events_in_wait_list,
            [In] cl_event[] event_wait_list,
            cl_event* _event);

        #endregion

        // Extension function access
        [DllImport(Configuration.Library)]
        [Obsolete("Deprecated in OpenCL 1.2. Use clGetExtensionFunctionAddressForPlatform", false)]
        public extern static IntPtr clGetExtensionFunctionAddress(string func_name);

        [DllImport(Configuration.Library)]
        public extern static IntPtr clGetExtensionFunctionAddressForPlatform(cl_platform_id platform, string func_name);

        [DllImport(Configuration.Library)]
        public extern static int clGetEventProfilingInfo(cl_event _event, cl_profiling_info param_name, IntPtr param_value_size, void* param_value, out IntPtr param_value_size_ret);
    }

#if false
    unsafe public static class GLSharing
    {
        public delegate int clGetGLContextInfoKHRDelegate(cl_context_properties* properties,
        cl_gl_context_info param_name,
        IntPtr param_value_size,
        void* param_value,
        IntPtr* param_value_size_ret);

        public static readonly clGetGLContextInfoKHRDelegate clGetGLContextInfoKHR;
        static GLSharing()
        {
            IntPtr func = OpenCLAPI.clGetExtensionFunctionAddress("clGetGLContextInfoKHR");
            clGetGLContextInfoKHR = (clGetGLContextInfoKHRDelegate)Marshal.GetDelegateForFunctionPointer(func, typeof(clGetGLContextInfoKHRDelegate));
        }
    }

    unsafe public static class GLEvent
    {
        static GLEvent()
        {
        }
    }

#endif
}
