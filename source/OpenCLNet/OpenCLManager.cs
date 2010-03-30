using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace OpenCLNet
{
    public partial class OpenCLManager : Component
    {
        public List<string> RequireExtensions = new List<string>();
        public bool RequireImageSupport { get; set; }
        public bool UseMultiplePlatforms { get; set; }
        public List<DeviceType> DeviceTypes = new List<DeviceType>();

        public OpenCLManager()
        {
            InitializeComponent();
        }

        public OpenCLManager(IContainer container)
        {
            container.Add(this);

            InitializeComponent();
        }

        public void InitializeOpenCL()
        {
            if (OpenCL.NumberOfPlatforms == 0)
                return;

            foreach (Platform p in OpenCL.GetPlatforms())
            {
                //p.QueryDevices(DeviceType DeviceType.
            }
        }
    }
}
