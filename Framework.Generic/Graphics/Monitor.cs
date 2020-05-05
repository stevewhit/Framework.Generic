using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace Framework.Generic.Graphics
{
    public static class Monitor
    {
        /// <summary>
        /// Returns the monitor pixel color at the designated (<paramref name="x"/>, <paramref name="y"/>).
        /// </summary>
        /// <param name="x">The X-Coordinate of the pixel.</param>
        /// <param name="y">The Y-Coordinate of the pixel.</param>
        /// <returns>Returns monitor pixel color at the designated (<paramref name="x"/>, <paramref name="y"/>).</returns>
        public static Color GetPixelColor(int x, int y)
        {
            return Color.FromArgb(GetPixelARGB(x, y));
        }

        /// <summary>
        /// Returns the monitor pixel RGB value at the designated (<paramref name="x"/>, <paramref name="y"/>).
        /// </summary>
        /// <param name="x">The X-Coordinate of the pixel.</param>
        /// <param name="y">The Y-Coordinate of the pixel.</param>
        /// <returns>Returns monitor pixel RGB value at the designated (<paramref name="x"/>, <paramref name="y"/>).</returns>
        public static int GetPixelARGB(int x, int y)
        {
            if (x < 0) throw new ArgumentOutOfRangeException(nameof(x), "Only positive X coordinate values are accepted.");
            if (y < 0) throw new ArgumentOutOfRangeException(nameof(y), "Only positive Y coordinate values are accepted.");

            // Allocate 4 bytes of memory to store the ARGB values.
            var allocatedBitsHandle = GCHandle.Alloc(4, GCHandleType.Pinned);

            // Construct a bitmap with a width and height of 1, and a stride = width * (4 bytes per pixel = 32bppArgb) = 4
            using (var bitmap = new Bitmap(1, 1, 4, PixelFormat.Format32bppArgb, allocatedBitsHandle.AddrOfPinnedObject()))
            {
                using (var graphics = System.Drawing.Graphics.FromImage(bitmap))
                {
                    // Copy the (x, y) pixel from the screen and store it in the bitmap at (0, 0)
                    graphics.CopyFromScreen(x, y, 0, 0, new System.Drawing.Size(1, 1));
                }

                return ExtractLastARGBFromBitmap(bitmap);
            }
        }

        /// <summary>
        /// Extracts and returns the ARGB value from the supplied 1x1 <paramref name="bitmapImage"/>.
        /// </summary>
        /// <param name="bitmapImage">The 1x1 bitmap image that will be read.</param>
        /// <returns>Returns the ARGB value from the supplied 1x1 <paramref name="bitmapImage"/>.</returns>
        private static int ExtractLastARGBFromBitmap(Bitmap bitmapImage)
        {
            if (bitmapImage.Width != 1 || bitmapImage.Height != 1)
                throw new ArgumentOutOfRangeException(nameof(bitmapImage), "Can only perform this operation on bitmaps with a height and width of 1.");

            // Lock the bits that will be read.
            var lockedBitsRect = new Rectangle(0, 0, bitmapImage.Width, bitmapImage.Height);
            var bitmapData = bitmapImage.LockBits(lockedBitsRect, ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

            var imageByteData = new byte[Math.Abs(bitmapData.Stride * bitmapImage.Height)];
            var allocatedMemoryHandle = bitmapData.Scan0;

            // Copy the source data from the allocated memory handle into the lockedBytesData array.
            Marshal.Copy(allocatedMemoryHandle, imageByteData, 0, imageByteData.Length);

            // Unlock the bits that have been read.
            bitmapImage.UnlockBits(bitmapData);

            // Convert and return the bytes array as a 32-bit integer
            // Remember: 8 bits/byte & 1 byte per color chanel (A|R|G|B) --> 32 bits/pixel
            return BitConverter.ToInt32(imageByteData, 0);
        }
    }
}
