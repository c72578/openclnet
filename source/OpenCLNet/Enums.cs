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
using System.Runtime.InteropServices;


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

    #endregion

    [StructLayout(LayoutKind.Sequential,Pack=1)]
    public struct OCLImageFormat
    {
        internal cl_channel_order image_channel_order;
        internal cl_channel_type image_channel_data_type;

        public static readonly OCLImageFormat RGB8U = new OCLImageFormat(ChannelOrder.RGB, ChannelType.UNSIGNED_INT8);
        public static readonly OCLImageFormat RGB8S = new OCLImageFormat(ChannelOrder.RGB, ChannelType.SIGNED_INT8);
        public static readonly OCLImageFormat RGB16U = new OCLImageFormat(ChannelOrder.RGB, ChannelType.UNSIGNED_INT16);
        public static readonly OCLImageFormat RGB16S = new OCLImageFormat(ChannelOrder.RGB, ChannelType.SIGNED_INT16);
        public static readonly OCLImageFormat RGB32U = new OCLImageFormat(ChannelOrder.RGB, ChannelType.UNSIGNED_INT32);
        public static readonly OCLImageFormat RGB32S = new OCLImageFormat(ChannelOrder.RGB, ChannelType.SIGNED_INT32);
        public static readonly OCLImageFormat RGBFloat = new OCLImageFormat(ChannelOrder.RGB, ChannelType.FLOAT);
        public static readonly OCLImageFormat RGBHalf = new OCLImageFormat(ChannelOrder.RGB, ChannelType.HALF_FLOAT);

        public static readonly OCLImageFormat RG8U = new OCLImageFormat(ChannelOrder.RG, ChannelType.UNSIGNED_INT8);
        public static readonly OCLImageFormat RG8S = new OCLImageFormat(ChannelOrder.RG, ChannelType.SIGNED_INT8);
        public static readonly OCLImageFormat RG16U = new OCLImageFormat(ChannelOrder.RG, ChannelType.UNSIGNED_INT16);
        public static readonly OCLImageFormat RG16S = new OCLImageFormat(ChannelOrder.RG, ChannelType.SIGNED_INT16);
        public static readonly OCLImageFormat RG32U = new OCLImageFormat(ChannelOrder.RG, ChannelType.UNSIGNED_INT32);
        public static readonly OCLImageFormat RG32S = new OCLImageFormat(ChannelOrder.RG, ChannelType.SIGNED_INT32);
        public static readonly OCLImageFormat RGFloat = new OCLImageFormat(ChannelOrder.RG, ChannelType.FLOAT);
        public static readonly OCLImageFormat RGHalf = new OCLImageFormat(ChannelOrder.RG, ChannelType.HALF_FLOAT);

        public static readonly OCLImageFormat R8U = new OCLImageFormat(ChannelOrder.R, ChannelType.UNSIGNED_INT8);
        public static readonly OCLImageFormat R8S = new OCLImageFormat(ChannelOrder.R, ChannelType.SIGNED_INT8);
        public static readonly OCLImageFormat R16U = new OCLImageFormat(ChannelOrder.R, ChannelType.UNSIGNED_INT16);
        public static readonly OCLImageFormat R16S = new OCLImageFormat(ChannelOrder.R, ChannelType.SIGNED_INT16);
        public static readonly OCLImageFormat R32U = new OCLImageFormat(ChannelOrder.R, ChannelType.UNSIGNED_INT32);
        public static readonly OCLImageFormat R32S = new OCLImageFormat(ChannelOrder.R, ChannelType.SIGNED_INT32);
        public static readonly OCLImageFormat RFloat = new OCLImageFormat(ChannelOrder.R, ChannelType.FLOAT);
        public static readonly OCLImageFormat RHalf = new OCLImageFormat(ChannelOrder.R, ChannelType.HALF_FLOAT);

        public static readonly OCLImageFormat RA8U = new OCLImageFormat(ChannelOrder.RA, ChannelType.UNSIGNED_INT8);
        public static readonly OCLImageFormat RA8S = new OCLImageFormat(ChannelOrder.RA, ChannelType.SIGNED_INT8);
        public static readonly OCLImageFormat RA16U = new OCLImageFormat(ChannelOrder.RA, ChannelType.UNSIGNED_INT16);
        public static readonly OCLImageFormat RA16S = new OCLImageFormat(ChannelOrder.RA, ChannelType.SIGNED_INT16);
        public static readonly OCLImageFormat RA32U = new OCLImageFormat(ChannelOrder.RA, ChannelType.UNSIGNED_INT32);
        public static readonly OCLImageFormat RA32S = new OCLImageFormat(ChannelOrder.RA, ChannelType.SIGNED_INT32);
        public static readonly OCLImageFormat RAFloat = new OCLImageFormat(ChannelOrder.RA, ChannelType.FLOAT);
        public static readonly OCLImageFormat RAHalf = new OCLImageFormat(ChannelOrder.RA, ChannelType.HALF_FLOAT);

        public static readonly OCLImageFormat RGBA8U = new OCLImageFormat(ChannelOrder.RGBA, ChannelType.UNSIGNED_INT8);
        public static readonly OCLImageFormat RGBA8S = new OCLImageFormat(ChannelOrder.RGBA, ChannelType.SIGNED_INT8);
        public static readonly OCLImageFormat RGBA16U = new OCLImageFormat(ChannelOrder.RGBA, ChannelType.UNSIGNED_INT16);
        public static readonly OCLImageFormat RGBA16S = new OCLImageFormat(ChannelOrder.RGBA, ChannelType.SIGNED_INT16);
        public static readonly OCLImageFormat RGBA32U = new OCLImageFormat(ChannelOrder.RGBA, ChannelType.UNSIGNED_INT32);
        public static readonly OCLImageFormat RGBA32S = new OCLImageFormat(ChannelOrder.RGBA, ChannelType.SIGNED_INT32);
        public static readonly OCLImageFormat RGBAFloat = new OCLImageFormat(ChannelOrder.RGBA, ChannelType.FLOAT);
        public static readonly OCLImageFormat RGBAHalf = new OCLImageFormat(ChannelOrder.RGBA, ChannelType.HALF_FLOAT);

        public static readonly OCLImageFormat BGRA8U = new OCLImageFormat(ChannelOrder.BGRA, ChannelType.UNSIGNED_INT8);
        public static readonly OCLImageFormat BGRA8S = new OCLImageFormat(ChannelOrder.BGRA, ChannelType.SIGNED_INT8);
        public static readonly OCLImageFormat BGRA16U = new OCLImageFormat(ChannelOrder.BGRA, ChannelType.UNSIGNED_INT16);
        public static readonly OCLImageFormat BGRA16S = new OCLImageFormat(ChannelOrder.BGRA, ChannelType.SIGNED_INT16);
        public static readonly OCLImageFormat BGRA32U = new OCLImageFormat(ChannelOrder.BGRA, ChannelType.UNSIGNED_INT32);
        public static readonly OCLImageFormat BGRA32S = new OCLImageFormat(ChannelOrder.BGRA, ChannelType.SIGNED_INT32);
        public static readonly OCLImageFormat BGRAFloat = new OCLImageFormat(ChannelOrder.BGRA, ChannelType.FLOAT);
        public static readonly OCLImageFormat BGRAHalf = new OCLImageFormat(ChannelOrder.BGRA, ChannelType.HALF_FLOAT);

        public static readonly OCLImageFormat ARGB8U = new OCLImageFormat(ChannelOrder.ARGB, ChannelType.UNSIGNED_INT8);
        public static readonly OCLImageFormat ARGB8S = new OCLImageFormat(ChannelOrder.ARGB, ChannelType.SIGNED_INT8);
        public static readonly OCLImageFormat ARGB16U = new OCLImageFormat(ChannelOrder.ARGB, ChannelType.UNSIGNED_INT16);
        public static readonly OCLImageFormat ARGB16S = new OCLImageFormat(ChannelOrder.ARGB, ChannelType.SIGNED_INT16);
        public static readonly OCLImageFormat ARGB32U = new OCLImageFormat(ChannelOrder.ARGB, ChannelType.UNSIGNED_INT32);
        public static readonly OCLImageFormat ARGB32S = new OCLImageFormat(ChannelOrder.ARGB, ChannelType.SIGNED_INT32);
        public static readonly OCLImageFormat ARGBFloat = new OCLImageFormat(ChannelOrder.ARGB, ChannelType.FLOAT);
        public static readonly OCLImageFormat ARGBHalf = new OCLImageFormat(ChannelOrder.ARGB, ChannelType.HALF_FLOAT);

        public static readonly OCLImageFormat A8U = new OCLImageFormat(ChannelOrder.A, ChannelType.UNSIGNED_INT8);
        public static readonly OCLImageFormat A8S = new OCLImageFormat(ChannelOrder.A, ChannelType.SIGNED_INT8);
        public static readonly OCLImageFormat A16U = new OCLImageFormat(ChannelOrder.A, ChannelType.UNSIGNED_INT16);
        public static readonly OCLImageFormat A16S = new OCLImageFormat(ChannelOrder.A, ChannelType.SIGNED_INT16);
        public static readonly OCLImageFormat A32U = new OCLImageFormat(ChannelOrder.A, ChannelType.UNSIGNED_INT32);
        public static readonly OCLImageFormat A32S = new OCLImageFormat(ChannelOrder.A, ChannelType.SIGNED_INT32);
        public static readonly OCLImageFormat AFloat = new OCLImageFormat(ChannelOrder.A, ChannelType.FLOAT);
        public static readonly OCLImageFormat AHalf = new OCLImageFormat(ChannelOrder.A, ChannelType.HALF_FLOAT);

        public static readonly OCLImageFormat INTENSITY8U = new OCLImageFormat(ChannelOrder.INTENSITY, ChannelType.UNSIGNED_INT8);
        public static readonly OCLImageFormat INTENSITY8S = new OCLImageFormat(ChannelOrder.INTENSITY, ChannelType.SIGNED_INT8);
        public static readonly OCLImageFormat INTENSITY16U = new OCLImageFormat(ChannelOrder.INTENSITY, ChannelType.UNSIGNED_INT16);
        public static readonly OCLImageFormat INTENSITY16S = new OCLImageFormat(ChannelOrder.INTENSITY, ChannelType.SIGNED_INT16);
        public static readonly OCLImageFormat INTENSITY32U = new OCLImageFormat(ChannelOrder.INTENSITY, ChannelType.UNSIGNED_INT32);
        public static readonly OCLImageFormat INTENSITY32S = new OCLImageFormat(ChannelOrder.INTENSITY, ChannelType.SIGNED_INT32);
        public static readonly OCLImageFormat INTENSITYFloat = new OCLImageFormat(ChannelOrder.INTENSITY, ChannelType.FLOAT);
        public static readonly OCLImageFormat INTENSITYHalf = new OCLImageFormat(ChannelOrder.INTENSITY, ChannelType.HALF_FLOAT);

        public static readonly OCLImageFormat LUMINANCE8U = new OCLImageFormat(ChannelOrder.LUMINANCE, ChannelType.UNSIGNED_INT8);
        public static readonly OCLImageFormat LUMINANCE8S = new OCLImageFormat(ChannelOrder.LUMINANCE, ChannelType.SIGNED_INT8);
        public static readonly OCLImageFormat LUMINANCE16U = new OCLImageFormat(ChannelOrder.LUMINANCE, ChannelType.UNSIGNED_INT16);
        public static readonly OCLImageFormat LUMINANCE16S = new OCLImageFormat(ChannelOrder.LUMINANCE, ChannelType.SIGNED_INT16);
        public static readonly OCLImageFormat LUMINANCE32U = new OCLImageFormat(ChannelOrder.LUMINANCE, ChannelType.UNSIGNED_INT32);
        public static readonly OCLImageFormat LUMINANCE32S = new OCLImageFormat(ChannelOrder.LUMINANCE, ChannelType.SIGNED_INT32);
        public static readonly OCLImageFormat LUMINANCEFloat = new OCLImageFormat(ChannelOrder.LUMINANCE, ChannelType.FLOAT);
        public static readonly OCLImageFormat LUMINANCEHalf = new OCLImageFormat(ChannelOrder.LUMINANCE, ChannelType.HALF_FLOAT);

        public OCLImageFormat(ChannelOrder channelOrder, ChannelType channelType)
        {
            image_channel_order = (cl_channel_order)channelOrder;
            image_channel_data_type = (cl_channel_type)channelType;
        }

        public ChannelOrder ChannelOrder
        {
            get
            {
                return (ChannelOrder)image_channel_order;
            }
            set
            {
                image_channel_order = (cl_channel_order)value;
            }
        }
        public ChannelType ChannelType
        {
            get
            {
                return (ChannelType)image_channel_data_type;
            }
            set
            {
                image_channel_data_type = (cl_channel_type)value;
            }
        }
    }

    public enum ErrorCode: int
    {
        SUCCESS                            =      0,
        DEVICE_NOT_FOUND                   =      -1,
        DEVICE_NOT_AVAILABLE               =      -2,
        COMPILER_NOT_AVAILABLE             =      -3,
        MEM_OBJECT_ALLOCATION_FAILURE      =      -4,
        OUT_OF_RESOURCES                   =      -5,
        OUT_OF_HOST_MEMORY                 =      -6,
        PROFILING_INFO_NOT_AVAILABLE       =      -7,
        MEM_COPY_OVERLAP                   =      -8,
        IMAGE_FORMAT_MISMATCH              =      -9,
        IMAGE_FORMAT_NOT_SUPPORTED         =      -10,
        BUILD_PROGRAM_FAILURE              =      -11,
        MAP_FAILURE                        =      -12,

        INVALID_VALUE                      =      -30,
        INVALID_DEVICE_TYPE                =      -31,
        INVALID_PLATFORM                   =      -32,
        INVALID_DEVICE                     =      -33,
        INVALID_CONTEXT                    =      -34,
        INVALID_QUEUE_PROPERTIES           =      -35,
        INVALID_COMMAND_QUEUE              =      -36,
        INVALID_HOST_PTR                   =      -37,
        INVALID_MEM_OBJECT                 =      -38,
        INVALID_IMAGE_FORMAT_DESCRIPTOR    =      -39,
        INVALID_IMAGE_SIZE                 =      -40,
        INVALID_SAMPLER                    =      -41,
        INVALID_BINARY                     =      -42,
        INVALID_BUILD_OPTIONS              =      -43,
        INVALID_PROGRAM                    =      -44,
        INVALID_PROGRAM_EXECUTABLE         =      -45,
        INVALID_KERNEL_NAME                =      -46,
        INVALID_KERNEL_DEFINITION          =      -47,
        INVALID_KERNEL                     =      -48,
        INVALID_ARG_INDEX                  =      -49,
        INVALID_ARG_VALUE                  =      -50,
        INVALID_ARG_SIZE                   =      -51,
        INVALID_KERNEL_ARGS                =      -52,
        INVALID_WORK_DIMENSION             =      -53,
        INVALID_WORK_GROUP_SIZE            =      -54,
        INVALID_WORK_ITEM_SIZE             =      -55,
        INVALID_GLOBAL_OFFSET              =      -56,
        INVALID_EVENT_WAIT_LIST            =      -57,
        INVALID_EVENT                      =      -58,
        INVALID_OPERATION                  =      -59,
        INVALID_GL_OBJECT                  =      -60,
        INVALID_BUFFER_SIZE                =      -61,
        INVALID_MIP_LEVEL                  =      -62,
    };

    public enum Bool
    {
        FALSE                               =     0,
        TRUE                                =     1
    };

    public enum PlatformInfo
    {
        PROFILE                        = 0x0900,
        VERSION                        = 0x0901,
        NAME                           = 0x0902,
        VENDOR                         = 0x0903,
        EXTENSIONS                     = 0x0904,
    };

    // cl_device_type - bitfield
    public enum DeviceType : ulong
    {
        DEFAULT                      = (1 << 0),
        CPU                          = (1 << 1),
        GPU                          = (1 << 2),
        ACCELERATOR                  = (1 << 3),
        ALL                          = 0xFFFFFFFF,
    };

    // cl_device_info
    public enum DeviceInfo
    {
        TYPE                             = 0x1000,
        VENDOR_ID                        = 0x1001,
        MAX_COMPUTE_UNITS                = 0x1002,
        MAX_WORK_ITEM_DIMENSIONS         = 0x1003,
        MAX_WORK_GROUP_SIZE              = 0x1004,
        MAX_WORK_ITEM_SIZES              = 0x1005,
        PREFERRED_VECTOR_WIDTH_CHAR      = 0x1006,
        PREFERRED_VECTOR_WIDTH_SHORT     = 0x1007,
        PREFERRED_VECTOR_WIDTH_INT       = 0x1008,
        PREFERRED_VECTOR_WIDTH_LONG      = 0x1009,
        PREFERRED_VECTOR_WIDTH_FLOAT     = 0x100A,
        PREFERRED_VECTOR_WIDTH_DOUBLE    = 0x100B,
        MAX_CLOCK_FREQUENCY              = 0x100C,
        ADDRESS_BITS                     = 0x100D,
        MAX_READ_IMAGE_ARGS              = 0x100E,
        MAX_WRITE_IMAGE_ARGS             = 0x100F,
        MAX_MEM_ALLOC_SIZE               = 0x1010,
        IMAGE2D_MAX_WIDTH                = 0x1011,
        IMAGE2D_MAX_HEIGHT               = 0x1012,
        IMAGE3D_MAX_WIDTH                = 0x1013,
        IMAGE3D_MAX_HEIGHT               = 0x1014,
        IMAGE3D_MAX_DEPTH                = 0x1015,
        IMAGE_SUPPORT                    = 0x1016,
        MAX_PARAMETER_SIZE               = 0x1017,
        MAX_SAMPLERS                     = 0x1018,
        MEM_BASE_ADDR_ALIGN              = 0x1019,
        MIN_DATA_TYPE_ALIGN_SIZE         = 0x101A,
        SINGLE_FP_CONFIG                 = 0x101B,
        GLOBAL_MEM_CACHE_TYPE            = 0x101C,
        GLOBAL_MEM_CACHELINE_SIZE        = 0x101D,
        GLOBAL_MEM_CACHE_SIZE            = 0x101E,
        GLOBAL_MEM_SIZE                  = 0x101F,
        MAX_CONSTANT_BUFFER_SIZE         = 0x1020,
        MAX_CONSTANT_ARGS                = 0x1021,
        LOCAL_MEM_TYPE                   = 0x1022,
        LOCAL_MEM_SIZE                   = 0x1023,
        ERROR_CORRECTION_SUPPORT         = 0x1024,
        PROFILING_TIMER_RESOLUTION       = 0x1025,
        ENDIAN_LITTLE                    = 0x1026,
        AVAILABLE                        = 0x1027,
        COMPILER_AVAILABLE               = 0x1028,
        EXECUTION_CAPABILITIES           = 0x1029,
        QUEUE_PROPERTIES                 = 0x102A,
        NAME                             = 0x102B,
        VENDOR                           = 0x102C,
        DRIVER_VERSION                   = 0x102D,
        PROFILE                          = 0x102E,
        VERSION                          = 0x102F,
        EXTENSIONS                       = 0x1030,
        PLATFORM                         = 0x1031,
    };	

    // cl_device_fp_config - bitfield
    public enum FpConfig : ulong
    {
        DENORM                               = (1 << 0),
        INF_NAN                              = (1 << 1),
        ROUND_TO_NEAREST                     = (1 << 2),
        ROUND_TO_ZERO                        = (1 << 3),
        ROUND_TO_INF                         = (1 << 4),
        FMA                                  = (1 << 5),
    };

    // cl_device_mem_cache_type
    public enum DeviceMemCacheType
    {
        NONE                                    = 0x0,
        READ_ONLY_CACHE                         = 0x1,
        READ_WRITE_CACHE                        = 0x2,
    };

    // cl_device_local_mem_type
    public enum DeviceLocalMemType
    {
        LOCAL                                   = 0x1,
        GLOBAL                                  = 0x2,
    };

// cl_device_exec_capabilities - bitfield
    public enum DeviceExecCapabilities : ulong
    {
        KERNEL                             = (1 << 0),
        NATIVE_KERNEL                      = (1 << 1),
    };

// cl_command_queue_properties - bitfield
    public enum CommandQueueProperties : ulong
    {
        NONE                              = 0,
        OUT_OF_ORDER_EXEC_MODE_ENABLE     = (1 << 0),
        PROFILING_ENABLE                  = (1 << 1),
    };

// cl_context_info
    public enum ContextInfo
    {
        REFERENCE_COUNT                 = 0x1080,
        DEVICES                         = 0x1081,
        PROPERTIES                      = 0x1082,
    };

// cl_context_properties
    public enum ContextProperties : ulong
    {
        PLATFORM                        = 0x1084,
    };

// cl_command_queue_info
    public enum CommandQueueInfo
    {
        CONTEXT                           = 0x1090,
        DEVICE                            = 0x1091,
        REFERENCE_COUNT                   = 0x1092,
        PROPERTIES                        = 0x1093,
    };

// cl_mem_flags - bitfield
    public enum MemFlags : ulong
    {
        READ_WRITE                          = (1 << 0),
        WRITE_ONLY                          = (1 << 1),
        READ_ONLY                           = (1 << 2),
        USE_HOST_PTR                        = (1 << 3),
        ALLOC_HOST_PTR                      = (1 << 4),
        COPY_HOST_PTR                       = (1 << 5),
    };
// cl_channel_order
    public enum ChannelOrder
    {
        R                                       = 0x10B0,
        A                                       = 0x10B1,
        RG                                      = 0x10B2,
        RA                                      = 0x10B3,
        RGB                                     = 0x10B4,
        RGBA                                    = 0x10B5,
        BGRA                                    = 0x10B6,
        ARGB                                    = 0x10B7,
        INTENSITY                               = 0x10B8,
        LUMINANCE                               = 0x10B9,
    };

// cl_channel_type
    public enum ChannelType
    {
        SNORM_INT8                              = 0x10D0,
        SNORM_INT16                             = 0x10D1,
        UNORM_INT8                              = 0x10D2,
        UNORM_INT16                             = 0x10D3,
        UNORM_SHORT_565                         = 0x10D4,
        UNORM_SHORT_555                         = 0x10D5,
        UNORM_INT_101010                        = 0x10D6,
        SIGNED_INT8                             = 0x10D7,
        SIGNED_INT16                            = 0x10D8,
        SIGNED_INT32                            = 0x10D9,
        UNSIGNED_INT8                           = 0x10DA,
        UNSIGNED_INT16                          = 0x10DB,
        UNSIGNED_INT32                          = 0x10DC,
        HALF_FLOAT                              = 0x10DD,
        FLOAT                                   = 0x10DE,
    };

// cl_mem_object_type
    public enum MemObjectType
    {
        BUFFER                       = 0x10F0,
        IMAGE2D                      = 0x10F1,
        IMAGE3D                      = 0x10F2,
    };

// cl_mem_info
    public enum MemInfo
    {
        TYPE                                = 0x1100,
        FLAGS                               = 0x1101,
        SIZE                                = 0x1102,
        HOST_PTR                            = 0x1103,
        MAP_COUNT                           = 0x1104,
        REFERENCE_COUNT                     = 0x1105,
        CONTEXT                             = 0x1106,
    };

// cl_image_info
    public enum ImageInfo
    {
        FORMAT                            = 0x1110,
        ELEMENT_SIZE                      = 0x1111,
        ROW_PITCH                         = 0x1112,
        SLICE_PITCH                       = 0x1113,
        WIDTH                             = 0x1114,
        HEIGHT                            = 0x1115,
        DEPTH                             = 0x1116,
    };

// cl_addressing_mode
    public enum AddressingMode : uint
    {
        NONE                            = 0x1130,
        CLAMP_TO_EDGE                   = 0x1131,
        CLAMP                           = 0x1132,
        REPEAT                          = 0x1133,
    };

// cl_filter_mode
    public enum FilterMode : uint
    {
        NEAREST                          = 0x1140,
        LINEAR                           = 0x1141,
    };

// cl_sampler_info
    public enum SamplerInfo : uint
    {
        REFERENCE_COUNT                 = 0x1150,
        CONTEXT                         = 0x1151,
        NORMALIZED_COORDS               = 0x1152,
        ADDRESSING_MODE                 = 0x1153,
        FILTER_MODE                     = 0x1154,
    };

// cl_map_flags - bitfield
    public enum MapFlags : ulong
    {
        READ                                = (1 << 0),
        WRITE                               = (1 << 1),
    };

// cl_program_info
    public enum ProgramInfo
    {
        REFERENCE_COUNT                 = 0x1160,
        CONTEXT                         = 0x1161,
        NUM_DEVICES                     = 0x1162,
        DEVICES                         = 0x1163,
        SOURCE                          = 0x1164,
        BINARY_SIZES                    = 0x1165,
        BINARIES                        = 0x1166,
    };

// cl_program_build_info
    public enum ProgramBuildInfo
    {
        STATUS                    = 0x1181,
        OPTIONS                   = 0x1182,
        LOG                       = 0x1183,
    };

// cl_build_status
    public enum BuildStatus
    {
        SUCCESS                           = 0,
        NONE                              = -1,
        ERROR                             = -2,
        IN_PROGRESS                       = -3,
    };

    // cl_kernel_info
    public enum KernelInfo
    {
        FUNCTION_NAME                    = 0x1190,
        NUM_ARGS                         = 0x1191,
        REFERENCE_COUNT                  = 0x1192,
        CONTEXT                          = 0x1193,
        PROGRAM                          = 0x1194,
    };
// cl_kernel_work_group_info
    public enum KernelWorkGroupInfo
    {
        WORK_GROUP_SIZE                  = 0x11B0,
        COMPILE_WORK_GROUP_SIZE          = 0x11B1,
        LOCAL_MEM_SIZE                   = 0x11B2,
    };

// cl_event_info
    public enum EventInfo
    {
        COMMAND_QUEUE                     = 0x11D0,
        COMMAND_TYPE                      = 0x11D1,
        REFERENCE_COUNT                   = 0x11D2,
        COMMAND_EXECUTION_STATUS          = 0x11D3,
    };

// cl_command_type
    public enum CommandType
    {
        NDRANGE_KERNEL                  = 0x11F0,
        TASK                            = 0x11F1,
        NATIVE_KERNEL                   = 0x11F2,
        READ_BUFFER                     = 0x11F3,
        WRITE_BUFFER                    = 0x11F4,
        COPY_BUFFER                     = 0x11F5,
        READ_IMAGE                      = 0x11F6,
        WRITE_IMAGE                     = 0x11F7,
        COPY_IMAGE                      = 0x11F8,
        COPY_IMAGE_TO_BUFFER            = 0x11F9,
        COPY_BUFFER_TO_IMAGE            = 0x11FA,
        MAP_BUFFER                      = 0x11FB,
        MAP_IMAGE                       = 0x11FC,
        UNMAP_MEM_OBJECT                = 0x11FD,
        MARKER                          = 0x11FE,
        ACQUIRE_GL_OBJECTS              = 0x11FF,
        RELEASE_GL_OBJECTS              = 0x1200,
    };

// command execution status
    public enum ExecutionStatus
    {
        COMPLETE                                = 0x0,
        RUNNING                                 = 0x1,
        SUBMITTED                               = 0x2,
        QUEUED                                  = 0x3,
    };

// cl_profiling_info
    public enum ProfilingInfo
    {
        QUEUED                = 0x1280,
        SUBMIT                = 0x1281,
        START                 = 0x1282,
        END                   = 0x1283,
    };



    // ********************************************
    // * CLGL enums
    // ********************************************
    public enum CLGLObjectType
    {
        BUFFER            = 0x2000,
        TEXTURE2D         = 0x2001,
        TEXTURE3D         = 0x2002,
        RENDERBUFFER      = 0x2003,
    };

    public enum CLGLTextureInfo
    {
        TEXTURE_TARGET           = 0x2004,
        MIPMAP_LEVEL             = 0x2005,
    };
}
