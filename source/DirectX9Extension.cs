using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Text.RegularExpressions;

namespace OpenCLNet
{
    public class DirectX9Extension : Extension
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

        public static readonly string ExtensionName = "cl_dx9_media_sharing";

        internal clGetDeviceIDsFromD3D11KHRDelegate clGetDeviceIDsFromD3D11KHR;
        internal clCreateFromD3D11BufferKHRDelegate clCreateFromD3D11BufferKHR;
        internal clCreateFromD3D11Texture2DKHRDelegate clCreateFromD3D11Texture2DKHR;
        internal clCreateFromD3D11Texture3DKHRDelegate clCreateFromD3D11Texture3DKHR;
        internal clEnqueueAcquireD3D11ObjectsKHRDelegate clEnqueueAcquireD3D11ObjectsKHR;
        internal clEnqueueReleaseD3D11ObjectsKHRDelegate clEnqueueReleaseD3D11ObjectsKHR;

        #region Constructors

        internal DirectX9Extension(Platform platform)
            : base(platform)
        {
            IntPtr entryPoint;

            if (!platform.HasExtension(ExtensionName))
                return;

            if (!platform.VersionCheck(1, 2))
                return;

            entryPoint = OpenCL.GetExtensionFunctionAddressForPlatform(platform, "clGetDeviceIDsFromDX9MediaAdapterKHR_fn");
            if (entryPoint == IntPtr.Zero)
                throw new OpenCLException("Error when importing cl_dx9_media_sharing extension");
            clGetDeviceIDsFromD3D11KHR = (clGetDeviceIDsFromD3D11KHRDelegate)Marshal.GetDelegateForFunctionPointer(entryPoint, typeof(clGetDeviceIDsFromD3D11KHRDelegate));

            entryPoint = OpenCL.GetExtensionFunctionAddressForPlatform(platform, "clCreateFromDX9MediaSurfaceKHR_fn");
            if (entryPoint == IntPtr.Zero)
                throw new OpenCLException("Error when importing cl_dx9_media_sharing extension");
            clCreateFromD3D11BufferKHR = (clCreateFromD3D11BufferKHRDelegate)Marshal.GetDelegateForFunctionPointer(entryPoint, typeof(clCreateFromD3D11BufferKHRDelegate));

            entryPoint = OpenCL.GetExtensionFunctionAddressForPlatform(platform, "clEnqueueAcquireDX9MediaSurfacesKHR_fn");
            if (entryPoint == IntPtr.Zero)
                throw new OpenCLException("Error when importing cl_dx9_media_sharing extension");
            clCreateFromD3D11Texture2DKHR = (clCreateFromD3D11Texture2DKHRDelegate)Marshal.GetDelegateForFunctionPointer(entryPoint, typeof(clCreateFromD3D11Texture2DKHRDelegate));

            entryPoint = OpenCL.GetExtensionFunctionAddressForPlatform(platform, "clEnqueueReleaseDX9MediaSurfacesKHR_fn");
            if (entryPoint == IntPtr.Zero)
                throw new OpenCLException("Error when importing cl_dx9_media_sharing extension");
            clCreateFromD3D11Texture3DKHR = (clCreateFromD3D11Texture3DKHRDelegate)Marshal.GetDelegateForFunctionPointer(entryPoint, typeof(clCreateFromD3D11Texture3DKHRDelegate));

            IsAvailable = true;
        }

        #endregion


        internal override string GetName()
        {
            return ExtensionName;
        }

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