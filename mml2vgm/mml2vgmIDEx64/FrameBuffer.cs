﻿using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace mml2vgmIDEx64
{
    //public class FrameBuffer
    //{
    //    public PictureBox pbScreen;
    //    public Bitmap bmpPlane;
    //    public int bmpPlaneW = 0;
    //    public int bmpPlaneH = 0;
    //    public byte[] baPlaneBuffer;
    //    public BufferedGraphics bgPlane;
    //    public int zoom = 1;
    //    public Size imageSize = new Size(0, 0);

    //    public void Add(PictureBox pbScreen, Image initialImage, Action<object, PaintEventArgs> p, int zoom)
    //    {
    //        this.zoom = zoom;
    //        this.pbScreen = pbScreen;
    //        System.Drawing.BufferedGraphicsContext currentContext;
    //        currentContext = BufferedGraphicsManager.Current;
    //        imageSize = new Size(initialImage.Size.Width, initialImage.Size.Height);

    //        pbScreen.Size = new Size(imageSize.Width * zoom, imageSize.Height * zoom);

    //        bgPlane = currentContext.Allocate(pbScreen.CreateGraphics(), pbScreen.DisplayRectangle);
    //        if (p != null) pbScreen.Paint += new System.Windows.Forms.PaintEventHandler(p);
    //        bmpPlane = new Bitmap(imageSize.Width, imageSize.Height, PixelFormat.Format32bppArgb);
    //        bmpPlaneW = imageSize.Width;
    //        bmpPlaneH = imageSize.Height;
    //        BitmapData bdPlane = bmpPlane.LockBits(new Rectangle(0, 0, bmpPlane.Width, bmpPlane.Height), ImageLockMode.ReadOnly, bmpPlane.PixelFormat);
    //        baPlaneBuffer = new byte[bdPlane.Stride * bmpPlane.Height];
    //        System.Runtime.InteropServices.Marshal.Copy(bdPlane.Scan0, baPlaneBuffer, 0, baPlaneBuffer.Length);
    //        bmpPlane.UnlockBits(bdPlane);
    //        bgPlane.Graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
    //        bgPlane.Graphics.DrawImage(initialImage, 0, 0, imageSize.Width * zoom, imageSize.Height * zoom);
    //    }

    //    public void Remove(Action<object, PaintEventArgs> p)
    //    {
    //        if (bmpPlane != null)
    //        {
    //            bmpPlane.Dispose();
    //            bmpPlane = null;
    //        }
    //        if (bgPlane != null)
    //        {
    //            bgPlane.Dispose();
    //            bgPlane = null;
    //        }
    //        try
    //        {
    //            if (pbScreen != null) pbScreen.Paint -= new System.Windows.Forms.PaintEventHandler(p);
    //        }
    //        catch (Exception ex)
    //        {
    //            log.ForcedWrite(ex);
    //        }
    //        pbScreen = null;

    //        baPlaneBuffer = null;
    //    }

    //    private void drawScreen()
    //    {
    //        if (bmpPlane == null) return;

    //        BitmapData bdPlane = bmpPlane.LockBits(new Rectangle(0, 0, bmpPlane.Width, bmpPlane.Height), ImageLockMode.WriteOnly, bmpPlane.PixelFormat);
    //        unsafe
    //        {
    //            byte* bdP = (byte*)bdPlane.Scan0;
    //            int adr;
    //            for (int y = 0; y < bdPlane.Height; y++)
    //            {
    //                adr = bdPlane.Stride * y;
    //                for (int x = 0; x < bdPlane.Stride; x++)
    //                {
    //                    bdP[adr + x] = baPlaneBuffer[bdPlane.Stride * y + x];
    //                }
    //            }
    //        }
    //        bmpPlane.UnlockBits(bdPlane);

    //        bgPlane.Graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
    //        bgPlane.Graphics.DrawImage(bmpPlane, 0, 0, bmpPlane.Width * zoom, bmpPlane.Height * zoom);

    //        //IntPtr hBmp = bmpPlane.GetHbitmap();
    //        //IntPtr hFormDC = bgPlane.Graphics.GetHdc(), hDC = CreateCompatibleDC(hFormDC);
    //        //IntPtr hPrevBmp = SelectObject(hDC, hBmp);
    //        //BitBlt(hFormDC, 0, 0, bmpPlane.Width, bmpPlane.Height, hDC, 0, 0, SRCCOPY);
    //        //bgPlane.Graphics.ReleaseHdc(hFormDC);
    //        //SelectObject(hDC, hPrevBmp);
    //        //DeleteDC(hDC);
    //        //DeleteObject(hBmp);
    //    }

    //    public static uint SRCINVERT = 0x00660046;
    //    public static uint SRCCOPY = 0x00CC0020;
    //    [DllImport("gdi32.dll")]
    //    public static extern bool BitBlt(
    //     IntPtr hdcDest, int nXDest, int nYDest, int nWidth, int nHeight, IntPtr hdcSrc,
    //            int nXSrc, int nYSrc, uint dwRop);
    //    [DllImport("gdi32.dll", EntryPoint = "SelectObject")]
    //    public static extern IntPtr SelectObject(IntPtr hdc, IntPtr h);
    //    [DllImport("gdi32.dll", ExactSpelling = true, SetLastError = true)]
    //    static extern IntPtr CreateCompatibleDC(IntPtr hdc);

    //    [DllImport("gdi32.dll", ExactSpelling = true, SetLastError = true)]
    //    static extern bool DeleteDC(IntPtr hdc);

    //    [DllImport("gdi32.dll")]
    //    public static extern bool DeleteObject(IntPtr hObject);

    //    public void Refresh(Action<object, PaintEventArgs> p)
    //    {
    //        Action act;

    //        if (pbScreen == null) return;
    //        if (pbScreen.IsDisposed) return;
    //        if (!pbScreen.Created) return;
    //        if (pbScreen.Parent == null) return;
    //        //if (pbScreen.Parent is form.frmChipBase && !((form.frmChipBase)pbScreen.Parent).Opened) return;

    //        try
    //        {
    //            pbScreen.Invoke(act = () =>
    //            {
    //                try
    //                {
    //                    drawScreen();
    //                }
    //                catch (Exception ex)
    //                {
    //                    log.ForcedWrite(ex);
    //                    Remove(p);
    //                }
    //                if (bgPlane != null) bgPlane.Render();
    //            });
    //        }
    //        catch (ObjectDisposedException)
    //        {
    //            ;//握りつぶす
    //        }
    //    }

    //    public void drawByteArray(int x, int y, byte[] src, int srcWidth, int imgX, int imgY, int imgWidth, int imgHeight)
    //    {
    //        if (bmpPlane == null)
    //        {
    //            return;
    //        }

    //        try
    //        {
    //            int adr1;
    //            int adr2;
    //            int wid = bmpPlaneW * 4;
    //            adr1 = wid * y + x * 4;
    //            adr2 = srcWidth * 4 * imgY + imgX * 4;
    //            for (int i = 0; i < imgHeight; i++)
    //            {
    //                if (adr1 >= 0 && adr2 >= 0)
    //                {
    //                    for (int j = 0; j < imgWidth * 4; j++)
    //                    {
    //                        if (baPlaneBuffer == null)
    //                        {
    //                            continue;
    //                        }

    //                        if (adr1 + j >= baPlaneBuffer.Length)
    //                        {
    //                            continue;
    //                        }
    //                        if (adr2 + j >= src.Length)
    //                        {
    //                            continue;
    //                        }
    //                        baPlaneBuffer[adr1 + j] = src[adr2 + j];
    //                    }
    //                }

    //                adr1 += wid;
    //                adr2 += srcWidth * 4;

    //            }
    //        }
    //        catch (Exception ex)
    //        {
    //            log.ForcedWrite(ex);
    //        }
    //    }

    //    public void drawByteArrayTransp(int x, int y, byte[] src, int srcWidth, int imgX, int imgY, int imgWidth, int imgHeight)
    //    {
    //        if (bmpPlane == null)
    //        {
    //            return;
    //        }

    //        try
    //        {
    //            int adr1;
    //            int adr2;
    //            int wid = bmpPlaneW * 4;
    //            adr1 = wid * y + x * 4;
    //            adr2 = srcWidth * 4 * imgY + imgX * 4;
    //            for (int i = 0; i < imgHeight; i++)
    //            {
    //                if (adr1 >= 0 && adr2 >= 0)
    //                {
    //                    for (int j = 0; j < imgWidth * 4; j += 4)
    //                    {
    //                        if (baPlaneBuffer == null)
    //                        {
    //                            continue;
    //                        }

    //                        if (adr1 + j >= baPlaneBuffer.Length)
    //                        {
    //                            continue;
    //                        }
    //                        if (adr2 + j >= src.Length)
    //                        {
    //                            continue;
    //                        }

    //                        if (src[adr2 + j + 0] == 0x00 && src[adr2 + j + 1] == 0xff && src[adr2 + j + 2] == 0x00) continue;

    //                        baPlaneBuffer[adr1 + j + 0] = src[adr2 + j + 0];
    //                        baPlaneBuffer[adr1 + j + 1] = src[adr2 + j + 1];
    //                        baPlaneBuffer[adr1 + j + 2] = src[adr2 + j + 2];
    //                        baPlaneBuffer[adr1 + j + 3] = src[adr2 + j + 3];
    //                    }
    //                }

    //                adr1 += wid;
    //                adr2 += srcWidth * 4;

    //            }
    //        }
    //        catch (Exception ex)
    //        {
    //            log.ForcedWrite(ex);
    //        }
    //    }
    //}

    public class FrameBuffer
    {
        public PictureBox pbScreen;
        public Bitmap bmpPlane;
        public int bmpPlaneW = 0;
        public int bmpPlaneH = 0;
        public int[] baPlaneBuffer;
        public BufferedGraphics bgPlane;
        public int zoom = 1;
        public Size imageSize = new Size(0, 0);

        public void Add(PictureBox pbScreen, Image initialImage, Action<object, PaintEventArgs> p, int zoom)
        {
            this.zoom = zoom;
            this.pbScreen = pbScreen;
            System.Drawing.BufferedGraphicsContext currentContext;
            currentContext = BufferedGraphicsManager.Current;
            imageSize = new Size(initialImage.Size.Width, initialImage.Size.Height);

            pbScreen.Size = new Size(imageSize.Width * zoom, imageSize.Height * zoom);

            bgPlane = currentContext.Allocate(pbScreen.CreateGraphics(), pbScreen.DisplayRectangle);
            if (p != null) pbScreen.Paint += new System.Windows.Forms.PaintEventHandler(p);
            bmpPlane = new Bitmap(imageSize.Width, imageSize.Height, PixelFormat.Format32bppArgb);
            bmpPlaneW = imageSize.Width;
            bmpPlaneH = imageSize.Height;
            BitmapData bdPlane = bmpPlane.LockBits(new Rectangle(0, 0, bmpPlane.Width, bmpPlane.Height), ImageLockMode.ReadOnly, bmpPlane.PixelFormat);
            baPlaneBuffer = new int[bdPlane.Stride * bmpPlane.Height / 4];
            System.Runtime.InteropServices.Marshal.Copy(bdPlane.Scan0, baPlaneBuffer, 0, baPlaneBuffer.Length);
            bmpPlane.UnlockBits(bdPlane);
            bgPlane.Graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
            bgPlane.Graphics.DrawImage(initialImage, 0, 0, imageSize.Width * zoom, imageSize.Height * zoom);
        }

        public void Remove(Action<object, PaintEventArgs> p)
        {
            if (bmpPlane != null)
            {
                bmpPlane.Dispose();
                bmpPlane = null;
            }
            if (bgPlane != null)
            {
                bgPlane.Dispose();
                bgPlane = null;
            }
            try
            {
                if (pbScreen != null) pbScreen.Paint -= new System.Windows.Forms.PaintEventHandler(p);
            }
            catch (Exception ex)
            {
                log.ForcedWrite(ex);
            }
            pbScreen = null;

            baPlaneBuffer = null;
        }

        private void drawScreen()
        {
            if (bmpPlane == null) return;

            BitmapData bdPlane = bmpPlane.LockBits(new Rectangle(0, 0, bmpPlane.Width, bmpPlane.Height), ImageLockMode.WriteOnly, bmpPlane.PixelFormat);
            unsafe
            {
                int* bdP = (int*)bdPlane.Scan0;
                int adr = 0;
                for (int y = 0; y < bdPlane.Height; y++)
                {
                    for (int x = 0; x < bdPlane.Stride / 4; x++)
                    {
                        bdP[adr] = baPlaneBuffer[adr];
                        adr++;
                    }
                }
            }
            bmpPlane.UnlockBits(bdPlane);

            bgPlane.Graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
            bgPlane.Graphics.DrawImage(bmpPlane, 0, 0, bmpPlane.Width * zoom, bmpPlane.Height * zoom);

        }

        public unsafe void clearScreen()
        {
            for (int i = 0; i < baPlaneBuffer.Length; i += 4)
            {
                baPlaneBuffer[i] = unchecked((int)0xFF00_0000); // ABGR
            }
        }


        //public static uint SRCINVERT = 0x00660046;
        //public static uint SRCCOPY = 0x00CC0020;
        //[DllImport("gdi32.dll")]
        //public static extern bool BitBlt(
        // IntPtr hdcDest, int nXDest, int nYDest, int nWidth, int nHeight, IntPtr hdcSrc,
        //        int nXSrc, int nYSrc, uint dwRop);
        //[DllImport("gdi32.dll", EntryPoint = "SelectObject")]
        //public static extern IntPtr SelectObject(IntPtr hdc, IntPtr h);
        //[DllImport("gdi32.dll", ExactSpelling = true, SetLastError = true)]
        //static extern IntPtr CreateCompatibleDC(IntPtr hdc);

        //[DllImport("gdi32.dll", ExactSpelling = true, SetLastError = true)]
        //static extern bool DeleteDC(IntPtr hdc);

        //[DllImport("gdi32.dll")]
        //public static extern bool DeleteObject(IntPtr hObject);

        public void Refresh(Action<object, PaintEventArgs> p)
        {
            Action act;

            if (pbScreen == null) return;
            if (pbScreen.IsDisposed) return;

            try
            {
                pbScreen.Invoke(act = () =>
                {
                    try
                    {
                        drawScreen();
                    }
                    catch (Exception ex)
                    {
                        log.ForcedWrite(ex);
                        Remove(p);
                    }
                    if (bgPlane != null) bgPlane.Render();
                });
            }
            catch (ObjectDisposedException)
            {
                ;//握りつぶす
            }
        }

        public void drawIntArray(int x, int y, int[] src, int srcWidth, int imgX, int imgY, int imgWidth, int imgHeight)
        {
            if (bmpPlane == null)
            {
                return;
            }
            if (baPlaneBuffer == null)
            {
                return;
            }

            int adr1;
            int adr2;
            int wid = bmpPlaneW;
            adr1 = wid * y + x;
            adr2 = srcWidth * imgY + imgX;
            int imgWidth4 = imgWidth;
            int srcWidth4 = srcWidth;

            for (int i = 0; i < imgHeight; i++)
            {
                if (adr1 < 0 || adr2 < 0) continue;
                for (int j = 0; j < imgWidth4; j++)
                {
                    if (adr1 >= baPlaneBuffer.Length) continue;
                    if (adr2 >= src.Length) continue;

                    baPlaneBuffer[adr1] = src[adr2];
                    adr1++;
                    adr2++;
                }

                adr1 += wid - imgWidth4;
                adr2 += srcWidth4 - imgWidth4;
            }
        }

        public void drawByteArrayTransp(int x, int y, int[] src, int srcWidth, int imgX, int imgY, int imgWidth, int imgHeight)
        {
            if (bmpPlane == null)
            {
                return;
            }
            if (baPlaneBuffer == null)
            {
                return;
            }

            try
            {
                int adr1;
                int adr2;
                int wid = bmpPlaneW;
                adr1 = wid * y + x;
                adr2 = srcWidth * imgY + imgX;
                for (int i = 0; i < imgHeight; i++)
                {
                    if (adr1 >= 0 && adr2 >= 0)
                    {
                        for (int j = 0; j < imgWidth; j += 4)
                        {

                            if (adr1 + j >= baPlaneBuffer.Length)
                            {
                                continue;
                            }
                            if (adr2 + j >= src.Length)
                            {
                                continue;
                            }

                            if (src[adr2 + j] == unchecked((int)0xff00ff00)) continue;

                            baPlaneBuffer[adr1 + j] = src[adr2 + j];
                        }
                    }

                    adr1 += wid;
                    adr2 += srcWidth * 4;

                }
            }
            catch (Exception ex)
            {
                log.ForcedWrite(ex);
            }
        }

        public void drawBoxArray(int x, int y, byte src, int thin, int width, int height)
        {
            if (bmpPlane == null) return;

            int adr1;
            int wid = bmpPlaneW;
            adr1 = wid * y + x;
            for (int i = 0; i < height; i++)
            {
                if (adr1 < 0)
                {
                    adr1 += wid;
                    continue;
                }

                if (i >= thin && i < height - thin)
                {
                    for (int j = 0; j < width; j++)
                    {
                        if (baPlaneBuffer == null) continue;
                        if (adr1 + j >= baPlaneBuffer.Length) continue;

                        if (j >= thin && j < width - thin) continue;

                        baPlaneBuffer[adr1 + j] = src;
                    }

                    adr1 += wid;
                    continue;
                }

                for (int j = 0; j < width; j++)
                {
                    if (baPlaneBuffer == null) continue;
                    if (adr1 + j >= baPlaneBuffer.Length) continue;

                    baPlaneBuffer[adr1 + j] = src;
                }

                adr1 += wid;

            }
        }

        public void drawFillBox(int x, int y, int width, int height, byte b, byte g, byte r)
        {
            if (bmpPlane == null) return;

            int adr1;
            int wid = bmpPlaneW;
            adr1 = wid * y + x;
            int argb = (0xff << 24) | (r << 16) | (g << 8) | b;
            for (int i = 0; i < height; i++)
            {
                if (y + i < 0) continue;
                if (y + i >= bmpPlaneH) continue;

                if (adr1 < 0)
                {
                    adr1 += wid;
                    continue;
                }

                if (baPlaneBuffer != null)
                {
                    for (int j = 0; j < width; j++)
                    {
                        if (x + j < 0) continue;
                        if (x + j >= bmpPlaneW) continue;

                        if (adr1 + j >= baPlaneBuffer.Length) continue;

                        baPlaneBuffer[adr1 + j] = argb;
                    }
                }

                adr1 += wid;

            }

        }

        public void drawFillBox(int x, int y, int width, int height, byte b1, byte g1, byte r1, byte b2, byte g2, byte r2)
        {
            if (bmpPlane == null) return;

            int adr1;
            int wid = bmpPlaneW;
            adr1 = wid * y + x;
            int argb;
            int argb1 = (0xff << 24) | (r1 << 16) | (g1 << 8) | b1;
            int argb2 = (0xff << 24) | (r2 << 16) | (g2 << 8) | b2;

            for (int i = 0; i < height; i++)
            {
                if (y + i < 0) continue;
                if (y + i >= bmpPlaneH) continue;

                if (adr1 < 0)
                {
                    adr1 += wid;
                    continue;
                }

                if (baPlaneBuffer != null)
                {
                    for (int j = 0; j < width; j++)
                    {
                        if (x + j < 0) continue;
                        if (x + j >= bmpPlaneW) continue;

                        if (adr1 + j >= baPlaneBuffer.Length) continue;

                        argb = argb1;
                        if (j == 0 || j == width - 1 || i == 0 || i == height - 1)
                        {
                            argb = argb2;
                        }
                        baPlaneBuffer[adr1 + j] = argb;
                    }
                }

                adr1 += wid;

            }
        }
    }

}
