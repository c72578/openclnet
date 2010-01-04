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
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;
using System.Drawing.Imaging;
using OpenCLNet;

namespace OpenCLTest
{

    public class Mandelbrot
    {
        public float Left { get; set; }
        public float Top { get; set; }
        public float Right { get; set; }
        public float Bottom { get; set; }

        public int BitmapWidth { get; set; }
        public int BitmapHeight { get; set; }

        public Bitmap Bitmap { get; protected set; }
        OpenCL OpenCL;

        Platform openCLPlatform;
        Device[] openCLDevices;
        Context openCLContext;
        CommandQueue openCLCQ;
        Program mandelBrotProgram;
        Kernel mandelbrotKernel;

        public Mandelbrot( OpenCL openCL, int width, int height )
        {
            OpenCL = openCL;

            openCLPlatform = OpenCL.GetPlatform(0);
            openCLDevices = openCLPlatform.QueryDevices(DeviceType.ALL);
            openCLContext = openCLPlatform.CreateDefaultContext();
            openCLCQ = openCLContext.CreateCommandQueue(openCLDevices[0], (CommandQueueProperties)0);
            mandelBrotProgram = openCLContext.CreateProgramWithSource(File.ReadAllText("Mandelbrot.cl"));
            try
            {
                mandelBrotProgram.Build();
            }
            catch (OpenCLException ocle)
            {
                string buildLog = mandelBrotProgram.GetBuildLog(openCLDevices[0]);
                MessageBox.Show(buildLog,"Build error(64 bit debug sessions in vs2008 always fail like this - debug in 32 bit)");
                Application.Exit();
            }
            mandelbrotKernel = mandelBrotProgram.CreateKernel("Mandelbrot");

            Left = -2.0f;
            Top = 2.0f;
            Right = 2.0f;
            Bottom = -2.0f;
            BitmapWidth = width;
            BitmapHeight = height;
        }

        public void AllocBuffers()
        {
            Bitmap = new Bitmap( BitmapWidth, BitmapHeight, System.Drawing.Imaging.PixelFormat.Format32bppArgb );
        }

        public void Calculate()
        {
            BitmapData bd = Bitmap.LockBits( new Rectangle( 0, 0, Bitmap.Width, Bitmap.Height ), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb );
            int bitmapSize = bd.Stride*bd.Height*4;
            Mem mandelbrotMemBuffer = openCLContext.CreateBuffer( (MemFlags)((long)MemFlags.WRITE_ONLY+(long)MemFlags.USE_HOST_PTR), bitmapSize, bd.Scan0 );

            mandelbrotKernel.SetArg( 0, Left );
            mandelbrotKernel.SetArg( 1, Top );
            mandelbrotKernel.SetArg( 2, Right );
            mandelbrotKernel.SetArg( 3, Bottom );
            mandelbrotKernel.SetArg( 4, bd.Stride );
            mandelbrotKernel.SetArg( 5, mandelbrotMemBuffer );

            Event clEvent;
            IntPtr[] globalWorkSize = new IntPtr[2] { new IntPtr( BitmapWidth ), new IntPtr( BitmapHeight ) };
            IntPtr[] localWorkSize = new IntPtr[2] { new IntPtr( 32 ), new IntPtr( 32 ) };
            openCLCQ.EnqueueNDRangeKernel(mandelbrotKernel, 2u, null, globalWorkSize, null, 0, null, out clEvent);
            openCLContext.WaitForEvent(clEvent);
            clEvent.Dispose();
            Bitmap.UnlockBits(bd);
            mandelbrotMemBuffer.Dispose();
        }
    }
}
