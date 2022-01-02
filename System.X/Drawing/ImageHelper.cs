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
        ImageCodecInfo GetImageEncoder(ImageFormat format)
        {
            if (format == null) return null;

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

        public byte[] Thumbnail(byte[] bytes, float scale, System.Drawing.Imaging.ImageFormat saveFormat = null)
        {
            using (var input = new MemoryStream(bytes))
            using (var output = new MemoryStream())
            {
                Thumbnail(input, output, scale, saveFormat);
                return output.GetBuffer();
            }
        }
        public byte[] Thumbnail(byte[] bytes, float scale, int quality, System.Drawing.Imaging.ImageFormat saveFormat = null)
        {
            using (var input = new MemoryStream(bytes))
            using (var output = new MemoryStream())
            {
                Thumbnail(input, output, scale, quality, saveFormat);
                return output.GetBuffer();
            }
        }
        public byte[] Thumbnail(byte[] bytes, int width, int height, System.Drawing.Imaging.ImageFormat saveFormat = null)
        {
            using (var input = new MemoryStream(bytes))
            using (var output = new MemoryStream())
            {
                Thumbnail(input, output, width, height, saveFormat);
                return output.GetBuffer();
            }
        }
        public byte[] Thumbnail(byte[] bytes, int width, int height, int quality, System.Drawing.Imaging.ImageFormat saveFormat = null)
        {
            using (var input = new MemoryStream(bytes))
            using (var output = new MemoryStream())
            {
                Thumbnail(input, output, width, height, quality, saveFormat);
                return output.GetBuffer();
            }
        }

        public void Thumbnail(Stream inputStream, Stream outputStream, float scale, System.Drawing.Imaging.ImageFormat saveFormat = null)
        {
            using (var image = Image.FromStream(inputStream))
            using (var thumb = InternalThumbnail(image, Convert.ToInt32(image.Width * scale), Convert.ToInt32(image.Height * scale), 0, 0, image.Width, image.Height))
                thumb.Save(outputStream, saveFormat ?? image.RawFormat);
        }
        public void Thumbnail(Stream inputStream, Stream outputStream, float scale, int quality, System.Drawing.Imaging.ImageFormat saveFormat = null)
        {
            using (var image = Image.FromStream(inputStream))
            using (var thumb = InternalThumbnail(image, Convert.ToInt32(image.Width * scale), Convert.ToInt32(image.Height * scale), 0, 0, image.Width, image.Height))
                Compress(thumb, outputStream, quality, saveFormat);
        }
        public void Thumbnail(Stream inputStream, Stream outputStream, int width, int height, System.Drawing.Imaging.ImageFormat saveFormat = null)
        {
            using (var image = Image.FromStream(inputStream))
            using (var thumb = InternalThumbnail(image, width, height, 0, 0, image.Width, image.Height))
                thumb.Save(outputStream, saveFormat ?? image.RawFormat);
        }
        public void Thumbnail(Stream inputStream, Stream outputStream, int width, int height, int quality, System.Drawing.Imaging.ImageFormat saveFormat = null)
        {
            using (var image = Image.FromStream(inputStream))
            using (var thumb = InternalThumbnail(image, width, height, 0, 0, image.Width, image.Height))
                Compress(thumb, outputStream, quality, saveFormat);
        }
        public Bitmap Thumbnail(Image image, int width, int height)
        {
            int x = 0;
            int y = 0;
            int ow = image.Width;
            int oh = image.Height;

            var k1 = image.Width * height;
            var k2 = width * image.Height;

            if (k1 > k2)
            {
                ow = k2 / height;
                x = (image.Width - ow) / 2;
            }
            else if (k1 < k2)
            {
                oh = k1 / width;
                y = (image.Height - oh) / 2;
            }
            //return img.GetThumbnailImage(width, height, null, IntPtr.Zero);
            return InternalThumbnail(image, width, height, x, y, ow, oh);
        }


        public byte[] Compress(byte[] bytes, int quality = 80, System.Drawing.Imaging.ImageFormat saveFormat = null)
        {
            using (var input = new MemoryStream(bytes))
            using (var output = new MemoryStream())
            {
                Compress(input, output, quality, saveFormat);
                return output.GetBuffer();
            }
        }
        public void Compress(byte[] bytes, Stream outputStream, int quality = 80, System.Drawing.Imaging.ImageFormat saveFormat = null)
        {
            using (var input = new MemoryStream(bytes))
                Compress(input, outputStream, quality, saveFormat);
        }
        public void Compress(Stream inputStream, Stream outputStream, int quality = 80, System.Drawing.Imaging.ImageFormat saveFormat = null)
        {
            using (var image = Image.FromStream(inputStream))
                Compress(image, outputStream, quality, saveFormat);
        }
        public byte[] Compress(System.Drawing.Image image, int quality = 80, System.Drawing.Imaging.ImageFormat saveFormat = null)
        {
            using (var ms = new System.IO.MemoryStream())
            {
                Compress(image, ms, quality, saveFormat);
                return ms.GetBuffer();
            }
        }
        void Compress(System.Drawing.Image image, Stream outputStream, int quality = 80, System.Drawing.Imaging.ImageFormat saveFormat = null)
        {
            var coder = GetImageEncoder(saveFormat ?? image.RawFormat);
            if (coder != null)
                image.Save(outputStream, coder, GenQualityEncoderParameters(quality));
            else
                image.Save(outputStream, saveFormat ?? image.RawFormat);
        }

        public Bitmap LD(Bitmap image, byte value)
        {
            Bitmap bm = new Bitmap(image.Width, image.Height);
            int x, y, resultR, resultG, resultB;
            Color pixel;
            for (x = 0; x < image.Width; x++)
            {
                for (y = 0; y < image.Height; y++)
                {
                    pixel = image.GetPixel(x, y);
                    resultR = RGBFloor(pixel.R + value);
                    resultG = RGBFloor(pixel.G + value);
                    resultB = RGBFloor(pixel.B + value);
                    bm.SetPixel(x, y, Color.FromArgb(resultR, resultG, resultB));
                }
            }
            return bm;
        }
        int RGBFloor(int value)
        {
            if (value < 0) return 0;
            if (value > 255) return 255;
            return value;
        }
        public Bitmap ReverseColor(Bitmap image)
        {
            Bitmap bm = new Bitmap(image.Width, image.Height);
            int x, y, r, g, b;
            Color pixel;
            for (x = 0; x < image.Width; x++)
            {
                for (y = 0; y < image.Height; y++)
                {
                    pixel = image.GetPixel(x, y);
                    r = 255 - pixel.R;
                    g = 255 - pixel.G;
                    b = 255 - pixel.B;
                    bm.SetPixel(x, y, Color.FromArgb(r, g, b));
                }
            }
            return bm;
        }

        public Bitmap FilterColor(Bitmap image, X.Drawing.ColorChannel channel)
        {
            int width = image.Width;
            int height = image.Height;
            Bitmap bm = new Bitmap(width, height);
            int x, y;
            Color pixel;

            for (x = 0; x < width; x++)
            {
                for (y = 0; y < height; y++)
                {
                    pixel = image.GetPixel(x, y);
                    switch (channel)
                    {
                        case ColorChannel.Red: bm.SetPixel(x, y, Color.FromArgb(0, pixel.G, pixel.B)); break;
                        case ColorChannel.Green: bm.SetPixel(x, y, Color.FromArgb(pixel.R, 0, pixel.B)); break;
                        case ColorChannel.Blue: bm.SetPixel(x, y, Color.FromArgb(pixel.R, pixel.G, 0)); break;
                        case ColorChannel.Red | ColorChannel.Green: bm.SetPixel(x, y, Color.FromArgb(0, 0, pixel.B)); break;
                        case ColorChannel.Red | ColorChannel.Blue: bm.SetPixel(x, y, Color.FromArgb(0, 0, pixel.B)); break;
                        case ColorChannel.Green | ColorChannel.Blue: bm.SetPixel(x, y, Color.FromArgb(0, 0, pixel.B)); break;
                        case ColorChannel.Red | ColorChannel.Green | ColorChannel.Blue: bm.SetPixel(x, y, Color.FromArgb(0, 0, 0)); break;
                    }
                }
            }
            return bm;
        }

        public Bitmap BW(Bitmap image)
        {
            int width = image.Width;
            int height = image.Height;
            Bitmap bm = new Bitmap(width, height);
            int x, y, result;
            Color pixel;
            for (x = 0; x < width; x++)
            {
                for (y = 0; y < height; y++)
                {
                    pixel = image.GetPixel(x, y);
                    result = (pixel.R + pixel.G + pixel.B) / 3;
                    bm.SetPixel(x, y, Color.FromArgb(result, result, result));
                }
            }
            return bm;
        }
        public Bitmap Flip(Bitmap image, X.Drawing.FlipMode mode)
        {
            switch (mode)
            {
                case FlipMode.Horizontal: return FlipHorizontal(image);
                case FlipMode.Vertical: return FlipVertical(image);
                case FlipMode.Both: return FlipBoth(image);
                default: return image;
            }
        }
        Bitmap FlipBoth(Bitmap image)
        {
            int width = image.Width;
            int height = image.Height;
            Bitmap bm = new Bitmap(width, height);
            int x, y;
            Color pixel;
            for (x = 0; x < width; x++)
            {
                for (y = 0; y < height; y++)
                {
                    pixel = image.GetPixel(x, y);
                    bm.SetPixel(width - x - 1, height - y - 1, Color.FromArgb(pixel.R, pixel.G, pixel.B));
                }
            }
            return bm;
        }
        Bitmap FlipVertical(Bitmap image)
        {
            int width = image.Width;
            int height = image.Height;
            Bitmap bm = new Bitmap(width, height);
            int x, y, z;
            Color pixel;
            for (y = height - 1; y >= 0; y--)
            {
                for (x = width - 1, z = 0; x >= 0; x--)
                {
                    pixel = image.GetPixel(x, y);
                    bm.SetPixel(z++, y, Color.FromArgb(pixel.R, pixel.G, pixel.B));
                }
            }
            return bm;
        }
        Bitmap FlipHorizontal(Bitmap image)
        {
            int width = image.Width;
            int height = image.Height;
            Bitmap bm = new Bitmap(width, height);
            int x, y, z;
            Color pixel;
            for (x = 0; x < width; x++)
            {
                for (y = height - 1, z = 0; y >= 0; y--)
                {
                    pixel = image.GetPixel(x, y);
                    bm.SetPixel(x, z++, Color.FromArgb(pixel.R, pixel.G, pixel.B));
                }
            }
            return bm;
        }

        public byte[] QRCode(string content, int pixel = 11, int version = -1)
        {
            using (var gene = new QRCoder.QRCodeGenerator())
            using (var data = gene.CreateQrCode(content, QRCoder.QRCodeGenerator.ECCLevel.M, true, true, QRCoder.QRCodeGenerator.EciMode.Utf8, version))
            using (var code = new QRCoder.PngByteQRCode(data))
                return code.GetGraphic(pixel);
        }
        public void QRCode(string content, string destFileName, int pixel = 11, int version = -1)
        {
            using (var gene = new QRCoder.QRCodeGenerator())
            using (var data = gene.CreateQrCode(content, QRCoder.QRCodeGenerator.ECCLevel.M, true, true, QRCoder.QRCodeGenerator.EciMode.Utf8, version))
            using (var code = new QRCoder.PngByteQRCode(data))
                File.WriteAllBytes(destFileName, code.GetGraphic(pixel));
        }

        public byte[] BarCode128(string content, int width = 250, int heigth = 50)
        {
            using (var ms = new MemoryStream())
            using (var bc = new BarcodeLib.Barcode())
            {
                bc.IncludeLabel = true;
                bc.Alignment = BarcodeLib.AlignmentPositions.CENTER;
                bc.LabelFont = new System.Drawing.Font(System.Drawing.FontFamily.GenericMonospace, heigth * 0.3f, System.Drawing.FontStyle.Regular);
                using (var img = bc.Encode(BarcodeLib.TYPE.CODE128, content, System.Drawing.Color.Black, System.Drawing.Color.White, width, heigth))
                {
                    img.Save(ms, ImageFormat.Jpeg);
                    return ms.GetBuffer();
                }
            }
        }
        public void BarCode128(string content, string srcFileName, int width = 250, int heigth = 50)
        {
            using (var ms = new MemoryStream())
            using (var bc = new BarcodeLib.Barcode())
            {
                bc.IncludeLabel = true;
                bc.Alignment = BarcodeLib.AlignmentPositions.CENTER;
                bc.LabelFont = new System.Drawing.Font(System.Drawing.FontFamily.GenericMonospace, heigth * 0.3f, System.Drawing.FontStyle.Regular);
                using (var img = bc.Encode(BarcodeLib.TYPE.CODE128, content, System.Drawing.Color.Black, System.Drawing.Color.White, width, heigth))
                {
                    img.Save(ms, ImageFormat.Jpeg);
                    System.IO.File.WriteAllBytes(srcFileName, ms.GetBuffer());
                }
            }
        }
        public byte[] GetBytes(System.Drawing.Image image)
        {
            using (var ms = new System.IO.MemoryStream())
            {
                image.Save(ms, image.RawFormat);
                return ms.GetBuffer();
            }
        }
    }
}