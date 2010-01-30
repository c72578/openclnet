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
}
