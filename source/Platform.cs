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
using System.Text.RegularExpressions;

namespace OpenCLNet
{

    unsafe public class Platform : InteropTools.IPropertyContainer
    {
        #region Properties

        public IntPtr PlatformID { get; protected set; }
        /// <summary>
        /// Equal to "FULL_PROFILE" if the implementation supports the OpenCL specification or
        /// "EMBEDDED_PROFILE" if the implementation supports the OpenCL embedded profile.
        /// </summary>
        public string Profile { get { return InteropTools.ReadString( this, (uint)PlatformInfo.PROFILE ); } }
        /// <summary>
        /// OpenCL version string. Returns the OpenCL version supported by the implementation. This version string
        /// has the following format: OpenCL&lt;space&gt;&lt;major_version.minor_version&gt;&lt;space&gt;&lt;platform specific information&gt;
        /// </summary>
        public string Version { get { return InteropTools.ReadString( this, (uint)PlatformInfo.VERSION ); } }
        /// <summary>
        /// Platform name string
        /// </summary>
        public string Name { get { return InteropTools.ReadString( this, (uint)PlatformInfo.NAME ); } }
        /// <summary>
        /// Platform Vendor string
        /// </summary>
        public string Vendor { get { return InteropTools.ReadString( this, (uint)PlatformInfo.VENDOR ); } }
        /// <summary>
        /// Space separated string of extension names.
        /// Note that this class has some support functions to help query extension capbilities.
        /// This property is only present for completeness.
        /// </summary>
        public string Extensions { get { return InteropTools.ReadString( this, (uint)PlatformInfo.EXTENSIONS ); } }
        /// <summary>
        /// Convenience method to get at the major_version field in the Version string
        /// </summary>
        public int OpenCLMajorVersion { get; protected set; }
        /// <summary>
        /// Convenience method to get at the minor_version field in the Version string
        /// </summary>
        public int OpenCLMinorVersion { get; protected set; }

        #endregion

        #region Private variables

        Regex VersionStringRegex = new Regex("OpenCL (?<Major>[0-9]+)\\.(?<Minor>[0-9]+)");
        protected Dictionary<IntPtr, Device> _Devices = new Dictionary<IntPtr, Device>();
        Device[] DeviceList;
        IntPtr[] DeviceIDs;

        protected HashSet<string> ExtensionHashSet = new HashSet<string>();

        #endregion

        DirextX10Extension DirectX10Extension;
        DirextX11Extension DirectX11Extension;

        #region Constructors

        public Platform( IntPtr platformID )
        {
            PlatformID = platformID;

            // Create a local representation of all devices
            DeviceIDs = QueryDeviceIntPtr( DeviceType.ALL );
            for( int i=0; i<DeviceIDs.Length; i++ )
                _Devices[DeviceIDs[i]] = new Device( this, DeviceIDs[i] );
            DeviceList = InteropTools.ConvertDeviceIDsToDevices( this, DeviceIDs );

            InitializeExtensionHashSet();

            Match m = VersionStringRegex.Match(Version);
            if (m.Success)
            {
                OpenCLMajorVersion = int.Parse(m.Groups["Major"].Value);
                OpenCLMinorVersion = int.Parse(m.Groups["Minor"].Value);
            }
            else
            {
                OpenCLMajorVersion = 1;
                OpenCLMinorVersion = 0;
            }

            if (OpenCLMajorVersion == 1 && OpenCLMajorVersion == 2)
            {
                if (HasExtension(DirextX10Extension.ExtensionName))
                    DirectX10Extension = new DirextX10Extension(this);

                if (HasExtension(DirextX11Extension.ExtensionName))
                    DirectX11Extension = new DirextX11Extension(this);
            }
        }

        #endregion

        public Context CreateDefaultContext()
        {
            return CreateDefaultContext(null, IntPtr.Zero);
        }

        public Context CreateDefaultContext( ContextNotify notify, IntPtr userData )
        {
            IntPtr[] properties = new IntPtr[]
            {
                new IntPtr((long)ContextProperties.PLATFORM), PlatformID,
                IntPtr.Zero,
            };

            IntPtr contextID;
            ErrorCode result;

            contextID = (IntPtr)OpenCL.CreateContext( properties,
                (uint)DeviceIDs.Length,
                DeviceIDs,
                notify,
                userData,
                out result );
            if( result!=ErrorCode.SUCCESS )
                throw new OpenCLException( "CreateContext failed with error code: "+result, result);
            return new Context( this, contextID );
        }

        public Context CreateContext(IntPtr[] contextProperties, Device[] devices, ContextNotify notify, IntPtr userData)
        {
            IntPtr contextID;
            ErrorCode result;

            IntPtr[] deviceIDs = InteropTools.ConvertDevicesToDeviceIDs(devices);
            contextID = (IntPtr)OpenCL.CreateContext(contextProperties,
                (uint)deviceIDs.Length,
                deviceIDs,
                notify,
                userData,
                out result);
            if (result != ErrorCode.SUCCESS)
                throw new OpenCLException("CreateContext failed with error code: " + result, result);
            return new Context(this, contextID);
        }

        public Context CreateContextFromType(IntPtr[] contextProperties, DeviceType deviceType, ContextNotify notify, IntPtr userData)
        {
            IntPtr contextID;
            ErrorCode result;

            contextID = (IntPtr)OpenCL.CreateContextFromType(contextProperties,
                deviceType,
                notify,
                userData,
                out result);
            if (result != ErrorCode.SUCCESS)
                throw new OpenCLException("CreateContextFromType failed with error code: " + result, result);
            return new Context(this, contextID);
        }

        /// <summary>
        /// Get a Device structure, given an OpenCL device handle
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public Device GetDevice(IntPtr index)
        {
            if (_Devices.ContainsKey(index))
                return _Devices[index];
            else
                return null;
        }

        protected IntPtr[] QueryDeviceIntPtr( DeviceType deviceType )
        {
            ErrorCode result;
            uint numberOfDevices;
            IntPtr[] deviceIDs;

            result = (ErrorCode)OpenCL.GetDeviceIDs( PlatformID, deviceType, 0, null, out numberOfDevices );
            if (result == ErrorCode.DEVICE_NOT_FOUND || (result == ErrorCode.SUCCESS && numberOfDevices==0))
                return new IntPtr[0];

            if( result!=ErrorCode.SUCCESS )
                throw new OpenCLException( "GetDeviceIDs failed: "+((ErrorCode)result).ToString(), result);

            deviceIDs = new IntPtr[numberOfDevices];
            result = (ErrorCode)OpenCL.GetDeviceIDs(PlatformID, deviceType, numberOfDevices, deviceIDs, out numberOfDevices);
            if (result != ErrorCode.SUCCESS)
                throw new OpenCLException("GetDeviceIDs failed: " + ((ErrorCode)result).ToString(), result);
            return deviceIDs;
        }

        /// <summary>
        /// Find all devices of a specififc type
        /// </summary>
        /// <param name="deviceType"></param>
        /// <returns>Array containing the devices</returns>
        public Device[] QueryDevices( DeviceType deviceType )
        {
            IntPtr[] deviceIDs;

            deviceIDs = QueryDeviceIntPtr( deviceType );
            return InteropTools.ConvertDeviceIDsToDevices( this, deviceIDs );
        }

        /// <summary>
        /// Returns true if this OpenCL platform has a version number greater
        /// than or equal to the version specified in the parameters
        /// </summary>
        /// <param name="majorVersion"></param>
        /// <param name="minorVersion"></param>
        /// <returns></returns>
        public bool VersionCheck(int majorVersion, int minorVersion)
        {
            return OpenCLMajorVersion>majorVersion || (OpenCLMajorVersion==majorVersion && OpenCLMinorVersion>=minorVersion);
        }

        #region Extension management

        /// <summary>
        /// OpenCL 1.2
        /// </summary>
        /// <param name="func_name"></param>
        /// <returns></returns>
        public IntPtr GetExtensionFunctionAddress(string func_name)
        {
            return OpenCL.GetExtensionFunctionAddressForPlatform(this,func_name);
        }

        protected void InitializeExtensionHashSet()
        {
            string[] ext = Extensions.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (string s in ext)
                ExtensionHashSet.Add(s);
        }

        /// <summary>
        /// Test if this platform supports a specific extension
        /// </summary>
        /// <param name="extension"></param>
        /// <returns>Returns true if the extension is supported</returns>
        public bool HasExtension(string extension)
        {
            return ExtensionHashSet.Contains(extension);
        }

        /// <summary>
        /// Test if this platform supports a set of exentions
        /// </summary>
        /// <param name="extensions"></param>
        /// <returns>Returns true if all the extensions are supported</returns>
        public bool HasExtensions( IEnumerable<string> extensions)
        {
            foreach (string s in extensions)
                if (!ExtensionHashSet.Contains(s))
                    return false;
            return true;
        }

        #endregion

        public static implicit operator IntPtr( Platform p )
        {
            return p.PlatformID;
        }

        #region IPropertyContainer Members

        public IntPtr GetPropertySize( uint key )
        {
            IntPtr propertySize;
            ErrorCode result;

            result = (ErrorCode)OpenCL.GetPlatformInfo( PlatformID, key, IntPtr.Zero, null, out propertySize );
            if( result!=ErrorCode.SUCCESS )
                throw new OpenCLException( "Unable to get platform info for platform "+PlatformID+": "+result, result);
            return propertySize;

        }

        public void ReadProperty( uint key, IntPtr keyLength, void* pBuffer )
        {
            IntPtr propertySize;
            ErrorCode result;

            result = (ErrorCode)OpenCL.GetPlatformInfo( PlatformID, key, keyLength, (void*)pBuffer, out propertySize );
            if( result!=ErrorCode.SUCCESS )
                throw new OpenCLException( "Unable to get platform info for platform "+PlatformID+": "+result, result);
        }

        #endregion
    }

    public class OpenCLExtension
    {
        /// <summary>
        /// IsAvailable is true if the API was imported correctly, false otherwise
        /// </summary>
        public bool IsAvailable { get; internal set; }

        public Platform Platform { get; internal set; }

        internal OpenCLExtension(Platform platform)
        {
            Platform = platform;
        }

        #region Helpers

        internal IntPtr[] RepackEvents(IList<Event> events)
        {
            if (events == null)
                return null;

            IntPtr[] repackedEvents = new IntPtr[events.Count];

            for (int i = 0; i < events.Count; i++)
                repackedEvents[i] = events[i];
            return repackedEvents;
        }

        internal IntPtr[] RepackMems(IList<Mem> objects)
        {
            if (objects == null)
                return null;

            IntPtr[] repackedObjects = new IntPtr[objects.Count];

            for (int i = 0; i < objects.Count; i++)
                repackedObjects[i] = objects[i];
            return repackedObjects;
        }

        #endregion

    }

    public class DirextX10Extension : OpenCLExtension
    {
        #region Delegates

        // D3D10 Delegates
        internal unsafe delegate ErrorCode clGetDeviceIDsFromD3D10KHRDelegate(
            IntPtr platform,
            uint d3d_device_source,
            IntPtr d3d_object,
            uint d3d_device_set,
            uint num_entries,
            [In][Out][MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 4)]IntPtr[] devices,
            uint* num_devices);
        internal unsafe delegate IntPtr clCreateFromD3D10BufferKHRDelegate(IntPtr context, ulong flags, IntPtr resource, out ErrorCode errcode_ret);
        internal unsafe delegate IntPtr clCreateFromD3D10Texture2DKHRDelegate(IntPtr context, ulong flags, IntPtr resource, uint subresource, out ErrorCode errcode_ret);
        internal unsafe delegate IntPtr clCreateFromD3D10Texture3DKHRDelegate(IntPtr context, ulong flags, IntPtr resource, uint subresource, out ErrorCode errcode_ret);
        internal unsafe delegate ErrorCode clEnqueueAcquireD3D10ObjectsKHRDelegate(
            IntPtr command_queue,
            uint num_objects,
            [In][MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 1)] IntPtr[] mem_objects,
            uint num_events_in_wait_list,
            [In][MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 3)] IntPtr[] event_wait_list,
            IntPtr* _event);
        internal unsafe delegate ErrorCode clEnqueueReleaseD3D10ObjectsKHRDelegate(
            IntPtr command_queue,
            uint num_objects,
            [In][MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 1)] IntPtr[] mem_objects,
            uint num_events_in_wait_list,
            [In][MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 3)] IntPtr[] event_wait_list,
            IntPtr* _event);

        #endregion

        public static readonly string ExtensionName = "cl_khr_d3d10_sharing";

        internal clGetDeviceIDsFromD3D10KHRDelegate clGetDeviceIDsFromD3D10KHR;
        internal clCreateFromD3D10BufferKHRDelegate clCreateFromD3D10BufferKHR;
        internal clCreateFromD3D10Texture2DKHRDelegate clCreateFromD3D10Texture2DKHR;
        internal clCreateFromD3D10Texture3DKHRDelegate clCreateFromD3D10Texture3DKHR;
        internal clEnqueueAcquireD3D10ObjectsKHRDelegate clEnqueueAcquireD3D10ObjectsKHR;
        internal clEnqueueReleaseD3D10ObjectsKHRDelegate clEnqueueReleaseD3D10ObjectsKHR;

        #region Constructors

        internal DirextX10Extension(Platform platform)
            : base(platform)
        {
            IntPtr entryPoint;

            if (!platform.HasExtension(ExtensionName))
                return;

            if (!platform.VersionCheck(1, 2))
                return;
            
            entryPoint = OpenCL.GetExtensionFunctionAddressForPlatform(platform, "clGetDeviceIDsFromD3D10KHR_fn");
            if (entryPoint == IntPtr.Zero)
                throw new OpenCLException( "Error when importing cl_khr_d3d10_sharing extension" );
            clGetDeviceIDsFromD3D10KHR = (clGetDeviceIDsFromD3D10KHRDelegate)Marshal.GetDelegateForFunctionPointer(entryPoint, typeof(clGetDeviceIDsFromD3D10KHRDelegate));

            entryPoint = OpenCL.GetExtensionFunctionAddressForPlatform(platform, "clCreateFromD3D10BufferKHR_fn");
            if (entryPoint == IntPtr.Zero)
                throw new OpenCLException( "Error when importing cl_khr_d3d10_sharing extension" );
            clCreateFromD3D10BufferKHR = (clCreateFromD3D10BufferKHRDelegate)Marshal.GetDelegateForFunctionPointer(entryPoint, typeof(clCreateFromD3D10BufferKHRDelegate));
            
            entryPoint = OpenCL.GetExtensionFunctionAddressForPlatform(platform, "clCreateFromD3D10Texture2DKHR_fn");
            if (entryPoint == IntPtr.Zero)
                throw new OpenCLException( "Error when importing cl_khr_d3d10_sharing extension" );
            clCreateFromD3D10Texture2DKHR = (clCreateFromD3D10Texture2DKHRDelegate)Marshal.GetDelegateForFunctionPointer(entryPoint, typeof(clCreateFromD3D10Texture2DKHRDelegate));
            
            entryPoint = OpenCL.GetExtensionFunctionAddressForPlatform(platform, "clCreateFromD3D10Texture3DKHR_fn");
            if (entryPoint == IntPtr.Zero)
                throw new OpenCLException( "Error when importing cl_khr_d3d10_sharing extension" );
            clCreateFromD3D10Texture3DKHR = (clCreateFromD3D10Texture3DKHRDelegate)Marshal.GetDelegateForFunctionPointer(entryPoint, typeof(clCreateFromD3D10Texture3DKHRDelegate));
            
            entryPoint = OpenCL.GetExtensionFunctionAddressForPlatform(platform, "clEnqueueAcquireD3D10ObjectsKHR_fn");
            if (entryPoint == IntPtr.Zero)
                throw new OpenCLException( "Error when importing cl_khr_d3d10_sharing extension" );
            clEnqueueAcquireD3D10ObjectsKHR = (clEnqueueAcquireD3D10ObjectsKHRDelegate)Marshal.GetDelegateForFunctionPointer(entryPoint, typeof(clEnqueueAcquireD3D10ObjectsKHRDelegate));
            
            entryPoint = OpenCL.GetExtensionFunctionAddressForPlatform(platform, "clEnqueueReleaseD3D10ObjectsKHR_fn");
            if (entryPoint == IntPtr.Zero)
                throw new OpenCLException( "Error when importing cl_khr_d3d10_sharing extension" );
            clEnqueueReleaseD3D10ObjectsKHR = (clEnqueueReleaseD3D10ObjectsKHRDelegate)Marshal.GetDelegateForFunctionPointer(entryPoint, typeof(clEnqueueReleaseD3D10ObjectsKHRDelegate));

            IsAvailable = true;
        }

        #endregion


        public unsafe Device[] GetDeviceIDsFromD3D10(Platform platform, D3D10DeviceSource d3d_device_source, IntPtr d3d_object, D3D10DeviceSet d3d_device_set)
        {
            ErrorCode status;
            uint numDevices;

            status = GetDeviceIDsFromD3D10(platform, d3d_device_source, d3d_object, d3d_device_set, 0, null, &numDevices);
            if (status != ErrorCode.SUCCESS)
                return null;

            IntPtr[] deviceArray = new IntPtr[numDevices];
            status = GetDeviceIDsFromD3D10(platform, d3d_device_source, d3d_object, d3d_device_set, numDevices, deviceArray, &numDevices);
            if (status != ErrorCode.SUCCESS)
                return null;

            return InteropTools.ConvertDeviceIDsToDevices(platform, deviceArray);
        }

        internal unsafe ErrorCode GetDeviceIDsFromD3D10(Platform platform, D3D10DeviceSource d3d_device_source, IntPtr d3d_object, D3D10DeviceSet d3d_device_set, uint num_entries, IntPtr[] devices, uint* num_devices)
        {
            ErrorCode status;

            status = clGetDeviceIDsFromD3D10KHR(platform, (uint)d3d_device_source, d3d_object, (uint)d3d_device_set, num_entries, devices, num_devices);
            return status;
        }

        public Mem CreateFromD3D10Buffer(Context context, ulong flags, IntPtr resource, out ErrorCode status)
        {
            IntPtr memID;
            
            memID = clCreateFromD3D10BufferKHR(context,flags, resource, out status);
            if (status != ErrorCode.SUCCESS)
                throw new OpenCLException("CreateFromD3D10Buffer: Failed with ErrorCode=" + status);
            return new Mem(context, memID);
        }

        public Mem CreateFromD3D10Texture2D(Context context, ulong flags, IntPtr resource, uint subresource)
        {
            ErrorCode status;
            IntPtr memID;

            memID = clCreateFromD3D10Texture2DKHR(context, flags, resource, subresource, out status);
            if (status != ErrorCode.SUCCESS)
                throw new OpenCLException("clCreateFromD3D10Texture2DKHR: Failed with ErrorCode=" + status);
            return new Mem(context, memID);
        }

        public Mem CreateFromD3D10Texture3D(Context context, ulong flags, IntPtr resource, uint subresource)
        {
            IntPtr memID;
            ErrorCode status;

            memID = clCreateFromD3D10Texture3DKHR(context, flags, resource, subresource, out status);
            if (status != ErrorCode.SUCCESS)
                throw new OpenCLException("CreateFromD3D10Texture3D: Failed with ErrorCode=" + status);
            return new Mem(context, memID);
        }

        public unsafe ErrorCode EnqueueAcquireD3D10Objects(CommandQueue command_queue, IList<Mem> objects)
        {
            return EnqueueAcquireD3D10Objects(command_queue, objects, null, null);
        }

        public unsafe ErrorCode EnqueueAcquireD3D10Objects(CommandQueue command_queue, IList<Mem> objects, IList<Event> event_wait_list)
        {
            return EnqueueAcquireD3D10Objects(command_queue, objects, event_wait_list, null);
        }

        public unsafe ErrorCode EnqueueAcquireD3D10Objects(CommandQueue command_queue, IList<Mem> objects, IList<Event> event_wait_list, out Event _event)
        {
            ErrorCode status;
            IntPtr tmpEvent;

            status = EnqueueAcquireD3D10Objects(command_queue, objects, event_wait_list, &tmpEvent);
            _event = new Event(command_queue.Context, command_queue, tmpEvent);
            return status;
        }

        internal unsafe ErrorCode EnqueueAcquireD3D10Objects(CommandQueue command_queue, IList<Mem> objects, IList<Event> event_wait_list, IntPtr* _event)
        {
            ErrorCode status;
            IntPtr[] repackedObjects;
            IntPtr[] repackedEvents;
            IntPtr tmpEvent;

            if (objects == null)
                throw new ArgumentNullException("EnqueueAcquireD3D10Objects: object list was null");

            repackedObjects = RepackMems(objects);
            repackedEvents = RepackEvents(event_wait_list);

            status = clEnqueueAcquireD3D10ObjectsKHR(command_queue, (uint)objects.Count, repackedObjects, (uint)event_wait_list.Count, repackedEvents, &tmpEvent);
            if (status != ErrorCode.SUCCESS)
                throw new OpenCLException("EnqueueAcquireD3D10Objects: Failed with ErrorCode=" + status);

            *_event = tmpEvent;
            return status;
        }

        public unsafe ErrorCode EnqueueReleaseD3D10Objects(CommandQueue command_queue, List<Mem> objects)
        {
            return EnqueueReleaseD3D10Objects(command_queue, objects, null, null);
        }

        public unsafe ErrorCode EnqueueReleaseD3D10Objects(CommandQueue command_queue, List<Mem> objects, List<Event> event_wait_list)
        {
            return EnqueueReleaseD3D10Objects(command_queue, objects, event_wait_list, null);
        }

        public unsafe ErrorCode EnqueueReleaseD3D10Objects(CommandQueue command_queue, List<Mem> objects, List<Event> event_wait_list, out Event _event)
        {
            ErrorCode status;
            IntPtr tmpEvent;

            status = EnqueueAcquireD3D10Objects(command_queue, objects, event_wait_list, &tmpEvent);
            _event = new Event(command_queue.Context, command_queue, tmpEvent);
            return status;
        }

        internal unsafe ErrorCode EnqueueReleaseD3D10Objects(CommandQueue command_queue, IList<Mem> objects, IList<Event> event_wait_list, IntPtr* _event)
        {
            ErrorCode status;
            IntPtr[] repackedObjects;
            IntPtr[] repackedEvents;
            IntPtr tmpEvent;

            if (objects == null)
                throw new ArgumentNullException("EnqueueReleaseD3D10Objects: object list was null");

            repackedObjects = RepackMems(objects);
            repackedEvents = RepackEvents(event_wait_list);

            status = clEnqueueReleaseD3D10ObjectsKHR(command_queue, (uint)objects.Count, repackedObjects, (uint)event_wait_list.Count, repackedEvents, &tmpEvent);
            if (status != ErrorCode.SUCCESS)
                throw new OpenCLException("EnqueueReleaseD3D10Objects: Failed with ErrorCode=" + status);

            *_event = tmpEvent;
            return status;
        }


    }



    public class DirextX11Extension : OpenCLExtension
    {
        #region Delegates

        // D3D11 Delegates
        internal unsafe delegate ErrorCode clGetDeviceIDsFromD3D11KHRDelegate(
            IntPtr platform,
            uint d3d_device_source,
            IntPtr d3d_object,
            uint d3d_device_set,
            uint num_entries,
            [In][Out][MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 4)]IntPtr[] devices,
            uint* num_devices);
        internal unsafe delegate IntPtr clCreateFromD3D11BufferKHRDelegate(IntPtr context, ulong flags, IntPtr resource, out ErrorCode errcode_ret);
        internal unsafe delegate IntPtr clCreateFromD3D11Texture2DKHRDelegate(IntPtr context, ulong flags, IntPtr resource, uint subresource, out ErrorCode errcode_ret);
        internal unsafe delegate IntPtr clCreateFromD3D11Texture3DKHRDelegate(IntPtr context, ulong flags, IntPtr resource, uint subresource, out ErrorCode errcode_ret);
        internal unsafe delegate ErrorCode clEnqueueAcquireD3D11ObjectsKHRDelegate(
            IntPtr command_queue,
            uint num_objects,
            [In][MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 1)] IntPtr[] mem_objects,
            uint num_events_in_wait_list,
            [In][MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 3)] IntPtr[] event_wait_list,
            IntPtr* _event);
        internal unsafe delegate ErrorCode clEnqueueReleaseD3D11ObjectsKHRDelegate(
            IntPtr command_queue,
            uint num_objects,
            [In][MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 1)] IntPtr[] mem_objects,
            uint num_events_in_wait_list,
            [In][MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 3)] IntPtr[] event_wait_list,
            IntPtr* _event);

        #endregion

        public static readonly string ExtensionName = "cl_khr_d3d11_sharing";

        internal clGetDeviceIDsFromD3D11KHRDelegate clGetDeviceIDsFromD3D11KHR;
        internal clCreateFromD3D11BufferKHRDelegate clCreateFromD3D11BufferKHR;
        internal clCreateFromD3D11Texture2DKHRDelegate clCreateFromD3D11Texture2DKHR;
        internal clCreateFromD3D11Texture3DKHRDelegate clCreateFromD3D11Texture3DKHR;
        internal clEnqueueAcquireD3D11ObjectsKHRDelegate clEnqueueAcquireD3D11ObjectsKHR;
        internal clEnqueueReleaseD3D11ObjectsKHRDelegate clEnqueueReleaseD3D11ObjectsKHR;

        #region Constructors

        internal DirextX11Extension(Platform platform)
            : base(platform)
        {
            IntPtr entryPoint;

            if (!platform.HasExtension(ExtensionName))
                return;

            if (!platform.VersionCheck(1, 2))
                return;

            entryPoint = OpenCL.GetExtensionFunctionAddressForPlatform(platform, "clGetDeviceIDsFromD3D10KHR_fn");
            if (entryPoint == IntPtr.Zero)
                throw new OpenCLException("Error when importing cl_khr_d3d10_sharing extension");
            clGetDeviceIDsFromD3D11KHR = (clGetDeviceIDsFromD3D11KHRDelegate)Marshal.GetDelegateForFunctionPointer(entryPoint, typeof(clGetDeviceIDsFromD3D11KHRDelegate));

            entryPoint = OpenCL.GetExtensionFunctionAddressForPlatform(platform, "clCreateFromD3D11BufferKHR_fn");
            if (entryPoint == IntPtr.Zero)
                throw new OpenCLException("Error when importing cl_khr_D3D11_sharing extension");
            clCreateFromD3D11BufferKHR = (clCreateFromD3D11BufferKHRDelegate)Marshal.GetDelegateForFunctionPointer(entryPoint, typeof(clCreateFromD3D11BufferKHRDelegate));

            entryPoint = OpenCL.GetExtensionFunctionAddressForPlatform(platform, "clCreateFromD3D11Texture2DKHR_fn");
            if (entryPoint == IntPtr.Zero)
                throw new OpenCLException("Error when importing cl_khr_D3D11_sharing extension");
            clCreateFromD3D11Texture2DKHR = (clCreateFromD3D11Texture2DKHRDelegate)Marshal.GetDelegateForFunctionPointer(entryPoint, typeof(clCreateFromD3D11Texture2DKHRDelegate));

            entryPoint = OpenCL.GetExtensionFunctionAddressForPlatform(platform, "clCreateFromD3D11Texture3DKHR_fn");
            if (entryPoint == IntPtr.Zero)
                throw new OpenCLException("Error when importing cl_khr_D3D11_sharing extension");
            clCreateFromD3D11Texture3DKHR = (clCreateFromD3D11Texture3DKHRDelegate)Marshal.GetDelegateForFunctionPointer(entryPoint, typeof(clCreateFromD3D11Texture3DKHRDelegate));

            entryPoint = OpenCL.GetExtensionFunctionAddressForPlatform(platform, "clEnqueueAcquireD3D11ObjectsKHR_fn");
            if (entryPoint == IntPtr.Zero)
                throw new OpenCLException("Error when importing cl_khr_D3D11_sharing extension");
            clEnqueueAcquireD3D11ObjectsKHR = (clEnqueueAcquireD3D11ObjectsKHRDelegate)Marshal.GetDelegateForFunctionPointer(entryPoint, typeof(clEnqueueAcquireD3D11ObjectsKHRDelegate));

            entryPoint = OpenCL.GetExtensionFunctionAddressForPlatform(platform, "clEnqueueReleaseD3D11ObjectsKHR_fn");
            if (entryPoint == IntPtr.Zero)
                throw new OpenCLException("Error when importing cl_khr_D3D11_sharing extension");
            clEnqueueReleaseD3D11ObjectsKHR = (clEnqueueReleaseD3D11ObjectsKHRDelegate)Marshal.GetDelegateForFunctionPointer(entryPoint, typeof(clEnqueueReleaseD3D11ObjectsKHRDelegate));

            IsAvailable = true;
        }

        #endregion


        public unsafe Device[] GetDeviceIDsFromD3D11(Platform platform, D3D11DeviceSource d3d_device_source, IntPtr d3d_object, D3D11DeviceSet d3d_device_set)
        {
            ErrorCode status;
            uint numDevices;

            status = GetDeviceIDsFromD3D11(platform, d3d_device_source, d3d_object, d3d_device_set, 0, null, &numDevices);
            if (status != ErrorCode.SUCCESS)
                return null;

            IntPtr[] deviceArray = new IntPtr[numDevices];
            status = GetDeviceIDsFromD3D11(platform, d3d_device_source, d3d_object, d3d_device_set, numDevices, deviceArray, &numDevices);
            if (status != ErrorCode.SUCCESS)
                return null;

            return InteropTools.ConvertDeviceIDsToDevices(platform, deviceArray);
        }

        internal unsafe ErrorCode GetDeviceIDsFromD3D11(Platform platform, D3D11DeviceSource d3d_device_source, IntPtr d3d_object, D3D11DeviceSet d3d_device_set, uint num_entries, IntPtr[] devices, uint* num_devices)
        {
            ErrorCode status;

            status = clGetDeviceIDsFromD3D11KHR(platform, (uint)d3d_device_source, d3d_object, (uint)d3d_device_set, num_entries, devices, num_devices);
            return status;
        }

        public Mem CreateFromD3D11Buffer(Context context, ulong flags, IntPtr resource, out ErrorCode status)
        {
            IntPtr memID;

            memID = clCreateFromD3D11BufferKHR(context, flags, resource, out status);
            if (status != ErrorCode.SUCCESS)
                throw new OpenCLException("CreateFromD3D11Buffer: Failed with ErrorCode=" + status);
            return new Mem(context, memID);
        }

        public Mem CreateFromD3D11Texture2D(Context context, ulong flags, IntPtr resource, uint subresource)
        {
            ErrorCode status;
            IntPtr memID;

            memID = clCreateFromD3D11Texture2DKHR(context, flags, resource, subresource, out status);
            if (status != ErrorCode.SUCCESS)
                throw new OpenCLException("clCreateFromD3D11Texture2DKHR: Failed with ErrorCode=" + status);
            return new Mem(context, memID);
        }

        public Mem CreateFromD3D11Texture3D(Context context, ulong flags, IntPtr resource, uint subresource)
        {
            IntPtr memID;
            ErrorCode status;

            memID = clCreateFromD3D11Texture3DKHR(context, flags, resource, subresource, out status);
            if (status != ErrorCode.SUCCESS)
                throw new OpenCLException("CreateFromD3D11Texture3D: Failed with ErrorCode=" + status);
            return new Mem(context, memID);
        }

        public unsafe ErrorCode EnqueueAcquireD3D11Objects(CommandQueue command_queue, IList<Mem> objects)
        {
            return EnqueueAcquireD3D11Objects(command_queue, objects, null, null);
        }

        public unsafe ErrorCode EnqueueAcquireD3D11Objects(CommandQueue command_queue, IList<Mem> objects, IList<Event> event_wait_list)
        {
            return EnqueueAcquireD3D11Objects(command_queue, objects, event_wait_list, null);
        }

        public unsafe ErrorCode EnqueueAcquireD3D11Objects(CommandQueue command_queue, IList<Mem> objects, IList<Event> event_wait_list, out Event _event)
        {
            ErrorCode status;
            IntPtr tmpEvent;

            status = EnqueueAcquireD3D11Objects(command_queue, objects, event_wait_list, &tmpEvent);
            _event = new Event(command_queue.Context, command_queue, tmpEvent);
            return status;
        }

        internal unsafe ErrorCode EnqueueAcquireD3D11Objects(CommandQueue command_queue, IList<Mem> objects, IList<Event> event_wait_list, IntPtr* _event)
        {
            ErrorCode status;
            IntPtr[] repackedObjects;
            IntPtr[] repackedEvents;
            IntPtr tmpEvent;

            if (objects == null)
                throw new ArgumentNullException("EnqueueAcquireD3D11Objects: object list was null");

            repackedObjects = RepackMems(objects);
            repackedEvents = RepackEvents(event_wait_list);

            status = clEnqueueAcquireD3D11ObjectsKHR(command_queue, (uint)objects.Count, repackedObjects, (uint)event_wait_list.Count, repackedEvents, &tmpEvent);
            if (status != ErrorCode.SUCCESS)
                throw new OpenCLException("EnqueueAcquireD3D11Objects: Failed with ErrorCode=" + status);

            *_event = tmpEvent;
            return status;
        }

        public unsafe ErrorCode EnqueueReleaseD3D11Objects(CommandQueue command_queue, List<Mem> objects)
        {
            return EnqueueReleaseD3D11Objects(command_queue, objects, null, null);
        }

        public unsafe ErrorCode EnqueueReleaseD3D11Objects(CommandQueue command_queue, List<Mem> objects, List<Event> event_wait_list)
        {
            return EnqueueReleaseD3D11Objects(command_queue, objects, event_wait_list, null);
        }

        public unsafe ErrorCode EnqueueReleaseD3D11Objects(CommandQueue command_queue, List<Mem> objects, List<Event> event_wait_list, out Event _event)
        {
            ErrorCode status;
            IntPtr tmpEvent;

            status = EnqueueAcquireD3D11Objects(command_queue, objects, event_wait_list, &tmpEvent);
            _event = new Event(command_queue.Context, command_queue, tmpEvent);
            return status;
        }

        internal unsafe ErrorCode EnqueueReleaseD3D11Objects(CommandQueue command_queue, IList<Mem> objects, IList<Event> event_wait_list, IntPtr* _event)
        {
            ErrorCode status;
            IntPtr[] repackedObjects;
            IntPtr[] repackedEvents;
            IntPtr tmpEvent;

            if (objects == null)
                throw new ArgumentNullException("EnqueueReleaseD3D11Objects: object list was null");

            repackedObjects = RepackMems(objects);
            repackedEvents = RepackEvents(event_wait_list);

            status = clEnqueueReleaseD3D11ObjectsKHR(command_queue, (uint)objects.Count, repackedObjects, (uint)event_wait_list.Count, repackedEvents, &tmpEvent);
            if (status != ErrorCode.SUCCESS)
                throw new OpenCLException("EnqueueReleaseD3D11Objects: Failed with ErrorCode=" + status);

            *_event = tmpEvent;
            return status;
        }


    }
}
