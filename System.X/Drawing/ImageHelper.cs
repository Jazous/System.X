using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Text;
using System.Linq;

namespace System.X.Drawing
{
    public sealed class ImageHelper
    {
        public static readonly ImageHelper Instance = new ImageHelper();

        ImageHelper()
        {
        }

        Bitmap InternalThumbnail(Image img, int descWidth, int descHeight, int srcX, int srcY, int srcWidth, int srcHeight)
        {
            var bm = new Bitmap(descWidth, descHeight);
            using (var g = Graphics.FromImage(bm))
            {
                g.CompositingQuality = CompositingQuality.HighQuality;
                g.SmoothingMode = SmoothingMode.HighQuality;
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.Clear(Color.Transparent);
                g.DrawImage(img, new Rectangle(0, 0, descWidth, descHeight), new Rectangle(srcX, srcY, srcWidth, srcHeight), GraphicsUnit.Pixel);
                g.Dispose();
                return bm;
            }
        }

        public Bitmap Thumbnail(Image img, int width, int height)
        {
            int x = 0;
            int y = 0;
            int ow = img.Width;
            int oh = img.Height;

            var k1 = img.Width * height;
            var k2 = width * img.Height;

            if (k1 > k2)
            {
                ow = k2 / height;
                x = (img.Width - ow) / 2;
            }
            else if (k1 < k2)
            {
                oh = k1 / width;
                y = (img.Height - oh) / 2;
            }
            return InternalThumbnail(img, width, height, x, y, ow, oh);
            //return img.GetThumbnailImage(width, height, null, IntPtr.Zero);
        }


        public byte[] Thumbnail(byte[] bytes, float scale)
        {
            return Thumbnail(bytes, scale, 80);
        }
        public byte[] Thumbnail(byte[] bytes, float scale, int quality)
        {
            using (var input = new MemoryStream(bytes))
            using (var output = new MemoryStream())
            {
                Thumbnail(input, output, scale, quality);
                return output.ToArray();
            }
        }
        public byte[] Thumbnail(byte[] bytes, int width, int height)
        {
            return Thumbnail(bytes, width, height, 80);
        }
        public byte[] Thumbnail(byte[] bytes, int width, int height, int quality)
        {
            using (var input = new MemoryStream(bytes))
            using (var output = new MemoryStream())
            {
                Thumbnail(input, output, width, height, quality);
                return output.ToArray();
            }
        }
        public void Thumbnail(Stream inputStream, Stream outputStream, int width, int height)
        {
            Thumbnail(inputStream, outputStream, width, height, 80);
        }
        public void Thumbnail(Stream inputStream, Stream outputStream, float scale)
        {
            Thumbnail(inputStream, outputStream, scale, 80);
        }
        public void Thumbnail(Stream inputStream, Stream outputStream, int width, int height, int quality)
        {
            using (var img = Image.FromStream(inputStream))
            using (var thumb = Thumbnail(img, width, height))
            {
                var coder = GetImageEncoder(img.RawFormat);
                if (coder != null)
                    thumb.Save(outputStream, coder, GenQualityEncoderParameters(quality));
                else
                    thumb.Save(outputStream, img.RawFormat);
            }
        }
        public void Thumbnail(Stream inputStream, Stream outputStream, float scale, int quality)
        {
            using (var img = Image.FromStream(inputStream))
            using (var thumb = InternalThumbnail(img, Convert.ToInt32(img.Width * scale), Convert.ToInt32(img.Height * scale), 0, 0, img.Width, img.Height))
            {
                var coder = GetImageEncoder(img.RawFormat);
                if (coder != null)
                    thumb.Save(outputStream, coder, GenQualityEncoderParameters(quality));
                else
                    thumb.Save(outputStream, img.RawFormat);
            }
        }


        public byte[] Compress(byte[] bytes)
        {
            return Compress(bytes, 80);
        }
        public byte[] Compress(byte[] bytes, int quality)
        {
            using (var input = new MemoryStream(bytes))
            using (var output = new MemoryStream())
            {
                Compress(input, output, quality);
                return output.ToArray();
            }
        }
        public void Compress(Stream inputStream, Stream outputStream)
        {
            Compress(inputStream, outputStream, 80);
        }
        public void Compress(Stream inputStream, Stream outputStream, int quality)
        {
            using (var thum = Image.FromStream(inputStream))
            {
                var coder = GetImageEncoder(thum.RawFormat);
                if (coder != null)
                    thum.Save(outputStream, coder, GenQualityEncoderParameters(quality));
                else
                    thum.Save(outputStream, thum.RawFormat);
            }
        }

        ImageCodecInfo GetImageEncoder(ImageFormat format)
        {
            var codecs = ImageCodecInfo.GetImageDecoders();
            foreach (var codec in codecs)
            {
                if (codec.FormatID == format.Guid)
                    return codec;
            }
            return null;
        }
        EncoderParameters GenQualityEncoderParameters(int quality)
        {
            var ep = new EncoderParameters(1);
            ep.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, new long[1] { quality });
            return ep;
        }
    }
}