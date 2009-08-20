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
    unsafe public class Device : InteropTools.IPropertyContainer
    {
        internal Device( Platform platform, IntPtr deviceID )
        {
            Platform = platform;
            DeviceID = deviceID;
        }

        //  User-defined conversion from double to Digit
        public static implicit operator IntPtr( Device d )
        {
            return d.DeviceID;
        }

        #region Properties

        public OpenCLAPI CL { get { return Platform.CL; } }
        public IntPtr DeviceID { get; protected set; }

        public DeviceType DeviceType { get { return (DeviceType) InteropTools.ReadULong( this, (uint)DeviceInfo.TYPE ); } }
        public uint VendorID { get { return InteropTools.ReadUInt( this, (uint)DeviceInfo.VENDOR_ID ); } }
        public uint MaxComputeUnits { get { return InteropTools.ReadUInt( this, (uint)DeviceInfo.MAX_COMPUTE_UNITS ); } }
        public uint MaxWorkItemDimensions { get { return InteropTools.ReadUInt( this, (uint)DeviceInfo.MAX_WORK_ITEM_DIMENSIONS ); } }
        public IntPtr[] MaxWorkItemSizes { get { return InteropTools.ReadIntPtrArray( this, (uint)DeviceInfo.MAX_WORK_ITEM_SIZES ); } }
        public long MaxWorkGroupSize { get { return InteropTools.ReadIntPtr( this, (uint)DeviceInfo.MAX_WORK_GROUP_SIZE ).ToInt64(); } }
        public uint PreferredVectorWidthChar { get { return InteropTools.ReadUInt( this, (uint)DeviceInfo.PREFERRED_VECTOR_WIDTH_CHAR ); } }
        public uint PreferredVectorWidthShort { get { return InteropTools.ReadUInt( this, (uint)DeviceInfo.PREFERRED_VECTOR_WIDTH_SHORT ); } }
        public uint PreferredVectorWidthInt { get { return InteropTools.ReadUInt( this, (uint)DeviceInfo.PREFERRED_VECTOR_WIDTH_INT ); } }
        public uint PreferredVectorWidthLong { get { return InteropTools.ReadUInt( this, (uint)DeviceInfo.PREFERRED_VECTOR_WIDTH_LONG ); } }
        public uint PreferredVectorWidthFloat { get { return InteropTools.ReadUInt( this, (uint)DeviceInfo.PREFERRED_VECTOR_WIDTH_FLOAT ); } }
        public uint PreferredVectorWidthDouble { get { return InteropTools.ReadUInt( this, (uint)DeviceInfo.PREFERRED_VECTOR_WIDTH_DOUBLE ); } }
        public uint MaxClockFrequency { get { return InteropTools.ReadUInt( this, (uint)DeviceInfo.MAX_CLOCK_FREQUENCY ); } }
        public uint AddressBits { get { return InteropTools.ReadUInt( this, (uint)DeviceInfo.ADDRESS_BITS ); } }
        public ulong MaxMemAllocSize { get { return InteropTools.ReadULong( this, (uint)DeviceInfo.MAX_MEM_ALLOC_SIZE ); } }
        public bool ImageSupport { get { return InteropTools.ReadBool( this, (uint)DeviceInfo.IMAGE_SUPPORT ); } }
        public uint MaxReadImageArgs { get { return InteropTools.ReadUInt( this, (uint)DeviceInfo.MAX_READ_IMAGE_ARGS ); } }
        public uint MaxWriteImageArgs { get { return InteropTools.ReadUInt( this, (uint)DeviceInfo.MAX_WRITE_IMAGE_ARGS ); } }
        public long Image2DMaxWidth { get { return InteropTools.ReadIntPtr( this, (uint)DeviceInfo.IMAGE2D_MAX_WIDTH ).ToInt64(); } }
        public long Image2DMaxHeight { get { return InteropTools.ReadIntPtr( this, (uint)DeviceInfo.IMAGE2D_MAX_HEIGHT ).ToInt64(); } }
        public long Image3DMaxWidth { get { return InteropTools.ReadIntPtr( this, (uint)DeviceInfo.IMAGE3D_MAX_WIDTH ).ToInt64(); } }
        public long Image3DMaxHeight { get { return InteropTools.ReadIntPtr( this, (uint)DeviceInfo.IMAGE3D_MAX_HEIGHT ).ToInt64(); } }
        public long Image3DMaxDepth { get { return InteropTools.ReadIntPtr( this, (uint)DeviceInfo.IMAGE3D_MAX_DEPTH ).ToInt64(); } }
        public uint MaxSamplers { get { return InteropTools.ReadUInt( this, (uint)DeviceInfo.MAX_SAMPLERS ); } }
        public long MaxParameterSize { get { return InteropTools.ReadIntPtr( this, (uint)DeviceInfo.MAX_PARAMETER_SIZE ).ToInt64(); } }
        public uint MemBaseAddrAlign { get { return InteropTools.ReadUInt( this, (uint)DeviceInfo.MEM_BASE_ADDR_ALIGN ); } }
        public uint MinDataTypeAlignSize { get { return InteropTools.ReadUInt( this, (uint)DeviceInfo.MIN_DATA_TYPE_ALIGN_SIZE ); } }
        public ulong SingleFPConfig { get { return InteropTools.ReadULong( this, (uint)DeviceInfo.SINGLE_FP_CONFIG ); } }
        public DeviceMemCacheType GlobalMemCacheType { get { return (DeviceMemCacheType)InteropTools.ReadUInt( this, (uint)DeviceInfo.GLOBAL_MEM_CACHE_TYPE ); } }
        public uint GlobalMemCacheLineSize { get { return InteropTools.ReadUInt( this, (uint)DeviceInfo.GLOBAL_MEM_CACHELINE_SIZE ); } }
        public ulong GlobalMemCacheSize { get { return InteropTools.ReadULong( this, (uint)DeviceInfo.GLOBAL_MEM_CACHE_SIZE ); } }
        public ulong GlobalMemSize { get { return InteropTools.ReadULong( this, (uint)DeviceInfo.GLOBAL_MEM_SIZE ); } }
        public ulong MaxConstantBufferSize { get { return InteropTools.ReadULong( this, (uint)DeviceInfo.MAX_CONSTANT_BUFFER_SIZE ); } }
        public uint MaxConstantArgs { get { return InteropTools.ReadUInt( this, (uint)DeviceInfo.MAX_CONSTANT_ARGS ); } }
        public DeviceLocalMemType LocalMemType { get { return (DeviceLocalMemType)InteropTools.ReadUInt( this, (uint)DeviceInfo.LOCAL_MEM_TYPE ); } }
        public ulong LocalMemSize { get { return InteropTools.ReadULong( this, (uint)DeviceInfo.LOCAL_MEM_SIZE ); } }
        public bool ErrorCorrectionSupport { get { return InteropTools.ReadBool( this, (uint)DeviceInfo.ERROR_CORRECTION_SUPPORT ); } }
        public ulong ProfilingTimerResolution { get { return (ulong)InteropTools.ReadIntPtr( this, (uint)DeviceInfo.PROFILING_TIMER_RESOLUTION ).ToInt64(); } }
        public bool EndianLittle { get { return InteropTools.ReadBool( this, (uint)DeviceInfo.ENDIAN_LITTLE ); } }
        public bool Available { get { return InteropTools.ReadBool( this, (uint)DeviceInfo.AVAILABLE ); } }
        public bool CompilerAvailable { get { return InteropTools.ReadBool( this, (uint)DeviceInfo.COMPILER_AVAILABLE ); } }
        public ulong ExecutionCapabilities { get { return InteropTools.ReadULong( this, (uint)DeviceInfo.EXECUTION_CAPABILITIES ); } }
        public ulong QueueProperties { get { return InteropTools.ReadULong( this, (uint)DeviceInfo.QUEUE_PROPERTIES ); } }
        public Platform Platform { get; protected set; }
        public string Name { get { return InteropTools.ReadString( this, (uint)DeviceInfo.NAME ); } }
        public string Vendor { get { return InteropTools.ReadString( this, (uint)DeviceInfo.VENDOR ); } }
        public string DriverVersion { get { return InteropTools.ReadString( this, (uint)DeviceInfo.DRIVER_VERSION ); } }
        public string Profile { get { return InteropTools.ReadString( this, (uint)DeviceInfo.PROFILE ); } }
        public string Version { get { return InteropTools.ReadString( this, (uint)DeviceInfo.VERSION ); } }
        public string Extensions { get { return InteropTools.ReadString( this, (uint)DeviceInfo.EXTENSIONS ); } }

        #endregion

        #region IPropertyContainer Members

        public IntPtr GetPropertySize( uint key )
        {
            IntPtr size;
            ErrorCode result;

            result = (ErrorCode)CL.GetDeviceInfo( DeviceID, key, IntPtr.Zero, null, out size );
            if( result!=ErrorCode.SUCCESS )
                throw new OpenCLException( "Unable to get device info for device "+DeviceID );
            return size;
        }

        public void ReadProperty( uint key, IntPtr keyLength, void* pBuffer )
        {
            IntPtr size;
            ErrorCode result;

            result = (ErrorCode)CL.GetDeviceInfo( DeviceID, (uint)key, keyLength, pBuffer, out size );
            if( result!=(int)ErrorCode.SUCCESS )
                throw new OpenCLException( "Unable to get device info for device "+DeviceID );
        }

        #endregion

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine( "Name: "+Name );
            sb.AppendLine( "Vendor: "+Vendor );
            sb.AppendLine( "VendorID: "+VendorID );
            sb.AppendLine( "DriverVersion: "+DriverVersion );
            sb.AppendLine( "Profile: "+Profile );
            sb.AppendLine( "Version: "+Version );
            sb.AppendLine( "Extensions: "+Extensions );
            sb.AppendLine( "DeviceType: "+DeviceType );
            sb.AppendLine( "MaxComputeUnits: "+MaxComputeUnits );
            sb.AppendLine( "MaxWorkItemDimensions: "+MaxWorkItemDimensions );
            sb.Append( "MaxWorkItemSizes:" );
            for( int i=0; i<MaxWorkItemSizes.Length; i++ )
                sb.Append( " "+i+"="+(int)MaxWorkItemSizes[i] );
            sb.AppendLine( "" );
            sb.AppendLine( "MaxWorkGroupSize: "+MaxWorkGroupSize );
            sb.AppendLine( "PreferredVectorWidthChar: "+PreferredVectorWidthChar );
            sb.AppendLine( "PreferredVectorWidthShort: "+PreferredVectorWidthShort );
            sb.AppendLine( "PreferredVectorWidthInt: "+PreferredVectorWidthInt );
            sb.AppendLine( "PreferredVectorWidthLong: "+PreferredVectorWidthLong );
            sb.AppendLine( "PreferredVectorWidthFloat: "+PreferredVectorWidthFloat );
            sb.AppendLine( "PreferredVectorWidthDouble: "+PreferredVectorWidthDouble );
            sb.AppendLine( "MaxClockFrequency: "+MaxClockFrequency );
            sb.AppendLine( "AddressBits: "+AddressBits );
            sb.AppendLine( "MaxMemAllocSize: "+MaxMemAllocSize );
            sb.AppendLine( "ImageSupport: "+ImageSupport );
            sb.AppendLine( "MaxReadImageArgs: "+MaxReadImageArgs );
            sb.AppendLine( "MaxWriteImageArgs: "+MaxWriteImageArgs );
            sb.AppendLine( "Image2DMaxWidth: "+Image2DMaxWidth );
            sb.AppendLine( "Image2DMaxHeight: "+Image2DMaxHeight );
            sb.AppendLine( "Image3DMaxWidth: "+Image3DMaxWidth );
            sb.AppendLine( "Image3DMaxHeight: "+Image3DMaxHeight );
            sb.AppendLine( "Image3DMaxDepth: "+Image3DMaxDepth );
            sb.AppendLine( "MaxSamplers: "+MaxSamplers );
            sb.AppendLine( "MaxParameterSize: "+MaxParameterSize );
            sb.AppendLine( "MemBaseAddrAlign: "+MemBaseAddrAlign );
            sb.AppendLine( "MinDataTypeAlignSize: "+MinDataTypeAlignSize );
            sb.AppendLine( "SingleFPConfig: "+SingleFPConfig );
            sb.AppendLine( "GlobalMemCacheType: "+GlobalMemCacheType );
            sb.AppendLine( "GlobalMemCacheLineSize: "+GlobalMemCacheLineSize );
            sb.AppendLine( "GlobalMemCacheSize: "+GlobalMemCacheSize );
            sb.AppendLine( "GlobalMemSize: "+GlobalMemSize );
            sb.AppendLine( "MaxConstantBufferSize: "+MaxConstantBufferSize );
            sb.AppendLine( "MaxConstantArgs: "+MaxConstantArgs );
            sb.AppendLine( "LocalMemType: "+LocalMemType );
            sb.AppendLine( "LocalMemSize: "+LocalMemSize );
            sb.AppendLine( "ErrorCorrectionSupport: "+ErrorCorrectionSupport );
            sb.AppendLine( "ProfilingTimerResolution: "+ProfilingTimerResolution );
            sb.AppendLine( "EndianLittle: "+EndianLittle );
            sb.AppendLine( "Available: "+Available );
            sb.AppendLine( "CompilerAvailable: "+CompilerAvailable );
            sb.AppendLine( "ExecutionCapabilities: "+ExecutionCapabilities );
            sb.AppendLine( "QueueProperties: "+QueueProperties );
            return sb.ToString();
        }
    }
}