using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace OpenCLNet
{
    public unsafe class OCLImage : Mem
    {
        internal OCLImage(Context context, IntPtr memID)
            : base(context,memID)
        {
        }

        #region Properties

        public OCLImageFormat ImageFormat
        {
            get
            {
                IntPtr size = GetPropertySize((uint)ImageInfo.FORMAT);
                byte* pBuffer = stackalloc byte[(int)size];

                ReadProperty((uint)ImageInfo.FORMAT, size, pBuffer);
                return (OCLImageFormat)Marshal.PtrToStructure((IntPtr)pBuffer, typeof(OCLImageFormat));
            }
        }
        public IntPtr ElementSize { get { return InteropTools.ReadIntPtr(this, (uint)ImageInfo.ELEMENT_SIZE); } }
        public IntPtr RowPitch { get { return InteropTools.ReadIntPtr(this, (uint)ImageInfo.ROW_PITCH); } }
        public IntPtr SlicePitch { get { return InteropTools.ReadIntPtr(this, (uint)ImageInfo.SLICE_PITCH); } }
        public IntPtr Width { get { return InteropTools.ReadIntPtr(this, (uint)ImageInfo.WIDTH); } }
        public IntPtr Height { get { return InteropTools.ReadIntPtr(this, (uint)ImageInfo.HEIGHT); } }
        public IntPtr Depth { get { return InteropTools.ReadIntPtr(this, (uint)ImageInfo.DEPTH); } }

        #endregion

        // Override the IPropertyContainer interface of the Mem class.
        #region IPropertyContainer Members

        public override unsafe IntPtr GetPropertySize(uint key)
        {
            IntPtr size;
            ErrorCode result;

            result = (ErrorCode)OpenCL.GetImageInfo(MemID, key, IntPtr.Zero, null, out size);
            if (result != ErrorCode.SUCCESS)
            {
                size = base.GetPropertySize(key);
            }
            return size;
        }

        public override unsafe void ReadProperty(uint key, IntPtr keyLength, void* pBuffer)
        {
            IntPtr size;
            ErrorCode result;

            result = (ErrorCode)OpenCL.GetImageInfo(MemID, key, keyLength, pBuffer, out size);
            if (result != ErrorCode.SUCCESS)
            {
                base.ReadProperty(key, keyLength, pBuffer);
            }
        }

        #endregion
    }
}
