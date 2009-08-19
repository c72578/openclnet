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

            openCLPlatform = OpenCL.GetPlatform( 0 );
            openCLDevices = openCLPlatform.QueryDevices( DeviceType.ALL );
            openCLContext = openCLPlatform.CreateDefaultContext( null, IntPtr.Zero );
            openCLCQ = openCLContext.CreateCommandQueue( openCLDevices[0], (CommandQueueProperties)0 );
            mandelBrotProgram = openCLContext.CreateProgramWithSource( File.ReadAllText( "Mandelbrot.cl" ) );
            mandelBrotProgram.Build();
            mandelbrotKernel = mandelBrotProgram.CreateKernel( "Mandelbrot" );

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

            IntPtr _event;
            IntPtr[] globalWorkSize = new IntPtr[2] { new IntPtr( BitmapWidth ), new IntPtr( BitmapHeight ) };
            IntPtr[] localWorkSize = new IntPtr[2] { new IntPtr( 1 ), new IntPtr( 1 ) };
            openCLCQ.EnqueueNDRangeKernel( mandelbrotKernel, 2u, null, globalWorkSize, null, 0, null, out _event );
            OpenCL.CL.WaitForEvents( 1, new IntPtr[] { _event } );

            OpenCL.CL.ReleaseEvent( _event );
            Bitmap.UnlockBits( bd );
            mandelbrotMemBuffer.Dispose();
        }
    }
}
