using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using OpenCLNet;

namespace ImageCrossFade
{
    public partial class Form1 : Form
    {
        bool Initialized = false;

        Bitmap InputBitmap0;
        Bitmap InputBitmap1;
        BitmapData InputBitmapData0;
        BitmapData InputBitmapData1;
        Bitmap OutputBitmap;

        SimpleOCLHelper OCLHelper;
        Mem InputBuffer0;
        Mem InputBuffer1;
        Kernel CrossFadeKernel;
        IntPtr[] CrossFadeGlobalWorkSize = new IntPtr[2];

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                InitializeOpenCL();
                SetupBitmaps();
                Initialized = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show( ex.Message, "Initiation failed...");
                Application.Exit();
            }
        }

        /// <summary>
        /// Loads two bitmaps and locks them for the duration of the program.
        /// Also creates two OpenCL buffers that map to the locked images
        /// </summary>
        private void SetupBitmaps()
        {
            InputBitmap0 = (Bitmap)Bitmap.FromFile(@"Input0.png");
            InputBitmap1 = (Bitmap)Bitmap.FromFile(@"Input1.png");
            if (InputBitmap1.Size != InputBitmap0.Size)
                InputBitmap1 = new Bitmap(InputBitmap1, InputBitmap0.Size);
            OutputBitmap = new Bitmap(InputBitmap0);

            InputBitmapData0 = InputBitmap0.LockBits(new Rectangle(0, 0, InputBitmap0.Width, InputBitmap0.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            InputBitmapData1 = InputBitmap1.LockBits(new Rectangle(0, 0, InputBitmap1.Width, InputBitmap1.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

            InputBuffer0 = OCLHelper.Context.CreateBuffer(MemFlags.USE_HOST_PTR, InputBitmapData0.Stride * InputBitmapData0.Height, InputBitmapData0.Scan0);
            InputBuffer1 = OCLHelper.Context.CreateBuffer(MemFlags.USE_HOST_PTR, InputBitmapData1.Stride * InputBitmapData1.Height, InputBitmapData1.Scan0);
        }

        /// <summary>
        /// Create a SimpleOCLHelper, using platform 0, default device type and a source containing test functions.
        /// The Helper automatically compiles and creates kernels.
        /// We can then extract named kernels using the GetKernel method.
        /// 
        /// For more advanced scenarios, one might use the functions in the Platform class
        /// to query devices, create contexts etc. Platforms can be enumerated using
        /// for( int i=0; i<OpenCL.NumberofPlatforms; i++ )
        ///     Platform p = OpenCL.GetPlatform(i);
        /// </summary>
        private void InitializeOpenCL()
        {
            if (OpenCL.NumberOfPlatforms == 0)
            {
                MessageBox.Show("OpenCL not available");
                Application.Exit();
            }

            string source = File.ReadAllText(@"OpenCLFunctions.cl");

            OCLHelper = new SimpleOCLHelper(OpenCL.GetPlatform(0), DeviceType.ALL, source );
            
            for (int i = 0; i < OCLHelper.Devices.Length; i++)
                comboBoxDeviceSelector.Items.Add(OCLHelper.Devices[i].Vendor+":"+OCLHelper.Devices[i].Name);
            comboBoxDeviceSelector.SelectedIndex = 0;

            CrossFadeKernel = OCLHelper.GetKernel("CrossFade");
        }

        /// <summary>
        /// Launch the CrossFadeKernel
        /// 
        /// First we set its arguments, 
        /// then we enqueue the kernel using EnqueueNDRangeKernel,
        /// and finally, map the buffer for reading to make sure
        /// there aren't any cache issues when OpenCL completes.
        /// </summary>
        /// <param name="ratio"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="input0"></param>
        /// <param name="inputStride0"></param>
        /// <param name="input1"></param>
        /// <param name="inputStride1"></param>
        /// <param name="output"></param>
        private void DoCrossFade( float ratio,
                                  int width, int height,
                                  Mem input0, int inputStride0,
                                  Mem input1, int inputStride1,
                                  BitmapData output )
        {
            Mem outputBuffer;

            int deviceIndex = comboBoxDeviceSelector.SelectedIndex;

            outputBuffer = OCLHelper.Context.CreateBuffer(MemFlags.USE_HOST_PTR, output.Stride * output.Height, output.Scan0);

            CrossFadeGlobalWorkSize[0] = (IntPtr)width;
            CrossFadeGlobalWorkSize[1] = (IntPtr)height;
            CrossFadeKernel.SetArg(0, ratio);
            CrossFadeKernel.SetArg(1, width);
            CrossFadeKernel.SetArg(2, height);
            CrossFadeKernel.SetArg(3, input0);
            CrossFadeKernel.SetArg(4, inputStride0);
            CrossFadeKernel.SetArg(5, input1);
            CrossFadeKernel.SetArg(6, inputStride1);
            CrossFadeKernel.SetArg(7, outputBuffer);
            CrossFadeKernel.SetArg(8, output.Stride);
            
            OCLHelper.CQs[deviceIndex].EnqueueNDRangeKernel(CrossFadeKernel, 2, null, CrossFadeGlobalWorkSize, null);
            OCLHelper.CQs[deviceIndex].EnqueueBarrier();
            IntPtr p = OCLHelper.CQs[deviceIndex].EnqueueMapBuffer(outputBuffer, true, MapFlags.READ, IntPtr.Zero, (IntPtr)(output.Stride * output.Height));
            OCLHelper.CQs[deviceIndex].EnqueueUnmapMemObject(outputBuffer, p);
            OCLHelper.CQs[deviceIndex].Finish();
            outputBuffer.Dispose();
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            if (!Initialized)
            {
                g.FillRectangle(new SolidBrush(BackColor), 0, 0, Width, Height);
                return;
            }

            g.FillRectangle(new SolidBrush(BackColor), 0, 0, Width, Height);
            g.DrawImageUnscaled(OutputBitmap, 0, 50);
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
        }

        private void hScrollBarRatio_ValueChanged(object sender, EventArgs e)
        {
            if (!Initialized)
                return;

            BitmapData bd = OutputBitmap.LockBits(new Rectangle(0, 0, OutputBitmap.Width, OutputBitmap.Height), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
            DoCrossFade(hScrollBarRatio.Value*0.01f,
                InputBitmapData0.Width, InputBitmapData0.Height,
                InputBuffer0, InputBitmapData0.Stride,
                InputBuffer1, InputBitmapData1.Stride,
                bd);
            OutputBitmap.UnlockBits(bd);
            Refresh();
        }

        private void comboBoxDeviceSelector_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
