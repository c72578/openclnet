using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenCLNet
{
    public class SimpleOCLHelper
    {
        public Platform Platform;
        /// <summary>
        /// The devices bound to the Helper
        /// </summary>
        public Device[] Devices;
        /// <summary>
        /// The Context associated with this Helper
        /// </summary>
        public Context Context;
        /// <summary>
        /// CommandQueue for device with same index
        /// </summary>
        public CommandQueue[] CQs;
        /// <summary>
        /// Alias for CQs[0]
        /// </summary>
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
            CQs = new CommandQueue[Devices.Length];
            for( int i=0; i<CQs.Length; i++ )
                CQs[i] = Context.CreateCommandQueue(Devices[i], CommandQueueProperties.PROFILING_ENABLE);
            CQ = CQs[0];
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