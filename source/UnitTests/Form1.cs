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
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.IO;
using OpenCLNet;

namespace UnitTests
{
    public partial class Form1 : Form
    {
        OpenCL     OpenCL;
        Platform[] Platforms;
        Regex ParseOpenCLVersion = new Regex(@"OpenCL (?<MajorVersion>\d+)\.(?<MinorVersion>\d+).*");

        public Form1()
        {
            InitializeComponent();
        }

        private void startToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                listBoxErrors.Items.Clear();
                listBoxWarnings.Items.Clear();
                listBoxOutput.Items.Clear();
                RunTests();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Test terminated with a fatal exception.");
            }
        }

        private void RunTests()
        {
            OpenCL = new OpenCL(new ATI());
            TestOpenCLClass();
        }

        private void TestOpenCLClass()
        {
            if (OpenCL.NumberOfPlatforms <= 0)
            {
                listBoxOutput.Items.Add("OpenCL.NumberOfPlatforms=" + OpenCL.NumberOfPlatforms);
                throw new Exception("TestOpenCLClass: NumberOfPlatforms<0. Is the API available at all?");
            }

            Platforms = OpenCL.GetPlatforms();
            if (Platforms.Length != OpenCL.NumberOfPlatforms)
                Error("OpenCL.NumberOfPlatforms!=Length of openCL.GetPlatforms()" + OpenCL.NumberOfPlatforms);

            for (int platformIndex = 0; platformIndex < Platforms.Length; platformIndex++)
            {
                if (OpenCL.GetPlatform(platformIndex) != Platforms[platformIndex])
                    Error("openCL.GetPlatform(platformIndex)!=Platforms[platformIndex]");

                Output("Testing Platform " + platformIndex);
                TestPlatform(Platforms[platformIndex]);
            }
        }

        private void TestPlatform(Platform p)
        {
            Device[] allDevices;
            Device[] cpuDevices;
            Device[] gpuDevices;
            Device[] acceleratorDevices;

            Output("Name: " + p.Name);
            Output("Vendor:" + p.Vendor);
            Output("Version:" + p.Version);

            // Check format of version string
            Match m = ParseOpenCLVersion.Match(p.Version);
            if (!m.Success)
                Warning("Platform " + p.Name + " has an invalid version string");
            else
            {
                if (m.Groups["MajorVersion"].Value != "1" && m.Groups["MinorVersion"].Value != "0")
                    Warning("Platform " + p.Name + " has a version number!=1.0(Not really a problem, but this test is written for 1.0)");
            }

            // Check format of profile
            Output("Profile:" + p.Profile);
            if (p.Profile == "FULL_PROFILE" || p.Profile == "EMBEDDED_PROFILE")
                Output("Profile:" + p.Profile);
            else
                Warning("Platform " + p.Name + " has unknown profile "+p.Profile);
            
            Output("Extensions: " + p.Extensions);

            // Test whether number of devices is consistent
            allDevices = p.QueryDevices(DeviceType.ALL);
            if( allDevices.Length<=0 )
                Warning( "Platform "+p.Name+" has no devices" );

            cpuDevices = p.QueryDevices(DeviceType.CPU);
            gpuDevices = p.QueryDevices(DeviceType.GPU);
            acceleratorDevices = p.QueryDevices(DeviceType.ACCELERATOR);
            if( allDevices.Length!=cpuDevices.Length+gpuDevices.Length+acceleratorDevices.Length )
                Warning( "QueryDevices( DeviceType.ALL ) return length inconsistent with sum of special purpose queries" );

            // Create a few contexts and test them
            Output( "Testing Platform.CreateDefaultContext()" );
            using (Context c = p.CreateDefaultContext())
            {
                Output("Testing context"+c);
                TestContext(c);
            }
            Output("");
            Output("");

            Output("Testing Platform.CreateContext()");
            using (Context c = p.CreateContext(null, cpuDevices, new ContextNotify(ContextNotifyFunc), (IntPtr)0x01234567))
            {
                Output("Testing context" + c);
                TestContext(c);
            }
            Output("");
            Output("");

            Output("Testing Platform.CreateContextFromType()");
            using (Context c = p.CreateContextFromType(null, DeviceType.CPU, new ContextNotify(ContextNotifyFunc), (IntPtr)0x01234567))
            {
                Output("Testing context" + c);
                TestContext(c);
            }
        }

        private void ContextNotifyFunc( string errInfo, byte[] privateInfo, IntPtr cb, IntPtr userData )
        {
            Error(errInfo);
        }

        private void TestContext(Context c)
        {
            Device[] devices = c.Devices;

            for (int deviceIndex = 0; deviceIndex < devices.Length; deviceIndex++)
            {
                Device d;

                d = devices[deviceIndex];
                using (CommandQueue cq = c.CreateCommandQueue(d))
                {
                    TestDevice(d);
                    TestCommandQueue(c, cq);
                }
            }
        }

        private void TestDevice( Device d )
        {
            Output("Testing device: " + d.Name);
            // d.ToString() is overloaded to output all properties as a string, so every property will be tested that way
            Output(d.ToString());
        }

        private void TestCommandQueue(Context c, CommandQueue cq )
        {
            string programName = @"MemoryTests.cl";

            Output("Testing compilation of: " + programName);
            OpenCLNet.Program p0 = c.CreateProgramWithSource(File.ReadAllLines(programName));
            OpenCLNet.Program p = c.CreateProgramWithSource(File.ReadAllText(programName));
            p0.Build();
            p.Build();
            Kernel k = p.CreateKernel(@"LoopAndDoNothing");
            
            TestCommandQueueMemCopy(c, cq);
            TestCommandQueueAsync(c, cq, k );
        }

        #region TestCommandQueue helper functions

        private void TestCommandQueueAsync(Context c, CommandQueue cq, Kernel kernel )
        {
            List<IntPtr> events = new List<IntPtr>();
            IntPtr _event;

            Output("Testing asynchronous task issuing (clEnqueueTask) and waiting for events");

            // Issue a bunch of slow operations
            kernel.SetArg(0, 5000000);
            for (int i = 0; i < 10; i++)
            {
                cq.EnqueueTask(kernel, 0, null, out _event);
                events.Add(_event);
            }

            // Issue a bunch of fast operations
            kernel.SetArg(0, 500);
            for (int i = 0; i < 1000; i++)
            {
                cq.EnqueueTask(kernel, 0, null, out _event);
                events.Add(_event);
            }

            IntPtr[] eventList = events.ToArray();
            cq.EnqueueWaitForEvents(eventList.Length, eventList);
        }

        private void TestCommandQueueMemCopy(Context c, CommandQueue cq)
        {
            AlignedArrayFloat aafSrc = new AlignedArrayFloat(1024 * 1024, 64);
            AlignedArrayFloat aafDst = new AlignedArrayFloat(1024 * 1024, 64);

            SetAAF(aafSrc, 0.0f);
            SetAAF(aafDst, 1.0f);

            /// Test HOST_PTR -> HOST_PTR copy
            /// The call to EnqueueMapBuffer synchronizes caches before testing the result
            Output("Testing synchronous host memory->memory copy(clEnqueueCopyBuffer)");
            using (Mem memSrc = c.CreateBuffer(MemFlags.USE_HOST_PTR, aafSrc.ByteLength, aafSrc))
            {
                using (Mem memDst = c.CreateBuffer(MemFlags.USE_HOST_PTR, aafDst.ByteLength, aafDst))
                {
                    cq.EnqueueCopyBuffer(memSrc, memDst, IntPtr.Zero, IntPtr.Zero, (IntPtr)aafSrc.ByteLength);
                    cq.EnqueueBarrier();
                    IntPtr mappedPtr = cq.EnqueueMapBuffer(memDst, true, MapFlags.READ, (IntPtr)0, (IntPtr)aafDst.ByteLength);
                    if (!TestAAF(aafDst, 0.0f))
                        Error("EnqueueCopyBuffer failed, destination is invalid");
                    cq.EnqueueUnmapMemObject(memDst, mappedPtr);
                    cq.EnqueueBarrier();
                }
            }

            /// Test COPY_HOST_PTR -> COPY_HOST_PTR copy
            /// Verify that original source buffers are intact and that the copy was successful
            Output("Testing synchronous host memory->memory copy(clEnqueueCopyBuffer)");
            SetAAF(aafSrc, 0.0f);
            SetAAF(aafDst, 1.0f);
            using (Mem memSrc = c.CreateBuffer(MemFlags.COPY_HOST_PTR, aafSrc.ByteLength, aafSrc))
            {
                using (Mem memDst = c.CreateBuffer(MemFlags.COPY_HOST_PTR, aafSrc.ByteLength, aafDst))
                {
                    SetAAF(aafSrc, 2.0f);
                    SetAAF(aafDst, 3.0f);

                    cq.EnqueueCopyBuffer(memSrc, memDst, IntPtr.Zero, IntPtr.Zero, (IntPtr)aafSrc.ByteLength);
                    cq.EnqueueBarrier();

                    if (!TestAAF(aafSrc, 2.0f))
                        Error("Memory copy destroyed src buffer");
                    if (!TestAAF(aafDst, 3.0f))
                        Error("Memory copy destroyed dst buffer");

                    cq.EnqueueReadBuffer(memDst, true, IntPtr.Zero, (IntPtr)aafDst.ByteLength, aafDst);
                    if (!TestAAF(aafDst, 0.0f))
                        Error("Memory copy failed");
                }
            }

            /// Test ALLOC_HOST_PTR -> ALLOC_HOST_PTR copy
            Output("Testing synchronous host memory->memory copy(clEnqueueCopyBuffer)");
            SetAAF(aafSrc, 0.0f);
            SetAAF(aafDst, 1.0f);
            using (Mem memSrc = c.CreateBuffer((MemFlags)((ulong)MemFlags.ALLOC_HOST_PTR + (ulong)MemFlags.READ_WRITE), aafSrc.ByteLength, IntPtr.Zero))
            {
                using (Mem memDst = c.CreateBuffer((MemFlags)((ulong)MemFlags.ALLOC_HOST_PTR + (ulong)MemFlags.WRITE_ONLY), aafSrc.ByteLength, IntPtr.Zero))
                {
                    cq.EnqueueWriteBuffer(memSrc, false, (IntPtr)0, (IntPtr)aafSrc.ByteLength, aafSrc);
                    cq.EnqueueWriteBuffer(memDst, false, (IntPtr)0, (IntPtr)aafSrc.ByteLength, aafSrc);
                    cq.EnqueueBarrier();

                    cq.EnqueueCopyBuffer(memSrc, memDst, IntPtr.Zero, IntPtr.Zero, (IntPtr)aafSrc.ByteLength);
                    cq.EnqueueBarrier();

                    cq.EnqueueReadBuffer(memDst, true, IntPtr.Zero, (IntPtr)aafDst.ByteLength, aafDst);
                    if (!TestAAF(aafDst, 0.0f))
                        Error("Memory copy failed");
                }
            }

            /// Test DEFAULT -> DEFAULT copy
            Output("Testing synchronous host memory->memory copy(clEnqueueCopyBuffer)");
            SetAAF(aafSrc, 0.0f);
            SetAAF(aafDst, 1.0f);
            using (Mem memSrc = c.CreateBuffer((MemFlags)((ulong)MemFlags.ALLOC_HOST_PTR + (ulong)MemFlags.READ_ONLY), aafSrc.ByteLength, IntPtr.Zero))
            {
                using (Mem memDst = c.CreateBuffer((MemFlags)((ulong)MemFlags.ALLOC_HOST_PTR + (ulong)MemFlags.WRITE_ONLY), aafSrc.ByteLength, IntPtr.Zero))
                {
                    cq.EnqueueWriteBuffer(memSrc, false, (IntPtr)0, (IntPtr)aafSrc.ByteLength, aafSrc);
                    cq.EnqueueWriteBuffer(memDst, false, (IntPtr)0, (IntPtr)aafSrc.ByteLength, aafSrc);
                    cq.EnqueueBarrier();

                    cq.EnqueueCopyBuffer(memSrc, memDst, IntPtr.Zero, IntPtr.Zero, (IntPtr)aafSrc.ByteLength);
                    cq.EnqueueBarrier();

                    cq.EnqueueReadBuffer(memDst, true, IntPtr.Zero, (IntPtr)aafDst.ByteLength, aafDst);
                    if (!TestAAF(aafDst, 0.0f))
                        Error("Memory copy failed");
                }
            }
        }

        private bool TestAAF(AlignedArrayFloat aaf, float c)
        {
            for (int i = 0; i < aaf.Length; i++)
                if (aaf[i] != c)
                    return false;
            return true;
        }

        private void SetAAF(AlignedArrayFloat aaf, float c)
        {
            for (int i = 0; i < aaf.Length; i++)
                aaf[i] = c;
        }

        private void SetAAFLinear(AlignedArrayFloat aaf)
        {
            for (int i = 0; i < aaf.Length; i++)
                aaf[i] = (float)i;
        }

        #endregion

        private void Output(string s)
        {
            listBoxOutput.Items.Add(s);
        }

        private void Warning(string s)
        {
            listBoxWarnings.Items.Add(s);
        }

        private void Error(string s)
        {
            listBoxErrors.Items.Add(s);
        }
    }
}
