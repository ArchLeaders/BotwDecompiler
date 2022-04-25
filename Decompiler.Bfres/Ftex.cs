using BfresLibrary.GX2;
using DirectXTexNet;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace Decompiler.Bfres
{
    public static class Ftex
    {
        public static Bitmap GetBitmap(byte[] Buffer, int Width, int Height, PixelFormat pixelFormat = PixelFormat.Format32bppArgb)
        {
            Rectangle Rect = new Rectangle(0, 0, Width, Height);

            Bitmap Img = new Bitmap(Width, Height, pixelFormat);

            BitmapData ImgData = Img.LockBits(Rect, ImageLockMode.WriteOnly, Img.PixelFormat);

            if (Buffer.Length > ImgData.Stride * Img.Height)
                throw new Exception($"Invalid Buffer Length ({Buffer.Length})!!!");

            Marshal.Copy(Buffer, 0, ImgData.Scan0, Buffer.Length);

            Img.UnlockBits(ImgData);

            return Img;
        }

        public static byte[] ConvertBgraToRgba(byte[] bytes)
        {
            if (bytes == null)
                throw new Exception("Data block returned null. Make sure the parameters and image properties are correct!");

            for (int i = 0; i < bytes.Length; i += 4)
            {
                var temp = bytes[i];
                bytes[i] = bytes[i + 2];
                bytes[i + 2] = temp;
            }
            return bytes;
        }

        public static DXGI_FORMAT GetDXGI(this GX2SurfaceFormat format)
        {
            return format switch
            {
                GX2SurfaceFormat.Invalid => DXGI_FORMAT.UNKNOWN,
                GX2SurfaceFormat.TC_R8_UNorm => DXGI_FORMAT.R8_UNORM,
                GX2SurfaceFormat.TC_R8_UInt => DXGI_FORMAT.R8_UINT,
                GX2SurfaceFormat.TC_R8_SNorm => DXGI_FORMAT.R8_SNORM,
                GX2SurfaceFormat.TC_R8_SInt => DXGI_FORMAT.R8_SINT,
                GX2SurfaceFormat.TCS_R8_G8_B8_A8_SRGB => DXGI_FORMAT.R8G8B8A8_UNORM_SRGB,
                GX2SurfaceFormat.T_BC1_UNorm => DXGI_FORMAT.BC1_UNORM,
                GX2SurfaceFormat.T_BC1_SRGB => DXGI_FORMAT.BC1_UNORM_SRGB,
                GX2SurfaceFormat.T_BC2_UNorm => DXGI_FORMAT.BC2_UNORM,
                GX2SurfaceFormat.T_BC2_SRGB => DXGI_FORMAT.BC2_UNORM_SRGB,
                GX2SurfaceFormat.T_BC3_UNorm => DXGI_FORMAT.BC3_UNORM,
                GX2SurfaceFormat.T_BC3_SRGB => DXGI_FORMAT.BC3_UNORM_SRGB,
                GX2SurfaceFormat.T_BC4_UNorm => DXGI_FORMAT.BC4_UNORM,
                GX2SurfaceFormat.T_BC4_SNorm => DXGI_FORMAT.BC4_SNORM,
                GX2SurfaceFormat.T_BC5_UNorm => DXGI_FORMAT.BC5_UNORM,
                GX2SurfaceFormat.T_BC5_SNorm => DXGI_FORMAT.BC5_SNORM
            };
        }
    }
}
