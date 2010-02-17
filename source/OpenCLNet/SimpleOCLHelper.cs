using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenCLNet
{
    public class SimpleOCLHelper
    {
        public Platform Platform;
        public Device[] Devices;
        public Context Context;
        public CommandQueue CQ;
        protected Program Program;
        protected Dictionary<string, Kernel> Kernels;

        public SimpleOCLHelper(Platform platform, DeviceType deviceType, string source)
        {
            Platform = platform;
            Initialize(deviceType, source);
        }

        protected virtual void Initialize(DeviceType deviceType, string source)
        {
            Devices = Platform.QueryDevices(deviceType);
            if (Devices.Length == 0)
                throw new OpenCLException("No devices of type "+deviceType+" present");

            Context = Platform.CreateContext(null,Devices,null, IntPtr.Zero);
            CQ = Context.CreateCommandQueue(Devices[0], CommandQueueProperties.PROFILING_ENABLE);
            Program = Context.CreateProgramWithSource(source);
            Program.Build();
            Kernels = Program.CreateKernelDictionary();
        }

        public Kernel GetKernel(string kernelName)
        {
            return Kernels[kernelName];
        }
    }
}