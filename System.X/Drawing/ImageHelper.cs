using SkiaSharp;

namespace System.X.Drawing;

public sealed class ImageHelper
{
    public static readonly ImageHelper Instance = new ImageHelper();

    ImageHelper()
    {

    }

    public byte[] ThumbW(byte[] imgData, int width, int quality = 80)
    {
        if (imgData == null)
            throw new ArgumentNullException("imgData");
        if (width <= 0)
            throw new ArgumentOutOfRangeException("width");

        using (var ori = SKBitmap.Decode(imgData))
        {
            using (var res = ori.Resize(new SKImageInfo(width, ori.Height * width / ori.Width), SKFilterQuality.Medium))
            using (var img = SKImage.FromBitmap(res))
                return img.Encode(SKEncodedImageFormat.Jpeg, quality).ToArray();
        }
    }
    public byte[] ThumbH(byte[] imgData, int height, int quality = 80)
    {
        if (imgData == null)
            throw new ArgumentNullException("imgData");
        if (height <= 0)
            throw new ArgumentOutOfRangeException("height");

        using (var ori = SKBitmap.Decode(imgData))
        {
            using (var res = ori.Resize(new SKImageInfo(ori.Width * height / ori.Height, height), SKFilterQuality.Medium))
            using (var img = SKImage.FromBitmap(res))
                return img.Encode(SKEncodedImageFormat.Jpeg, quality).ToArray();
        }
    }
    public byte[] Thumb(byte[] imgData, int width, int height, int quality = 80)
    {
        if (imgData == null)
            throw new ArgumentNullException("imgData");
        if (width <= 0)
            throw new ArgumentOutOfRangeException("width");
        if (height <= 0)
            throw new ArgumentOutOfRangeException("height");

        using (var ori = SKBitmap.Decode(imgData))
        {
            using (var res = ori.Resize(new SKImageInfo(width, height), SKFilterQuality.Medium))
            using (var img = SKImage.FromBitmap(res))
                return img.Encode(SKEncodedImageFormat.Jpeg, quality).ToArray();
        }
    }
    public byte[] Cut(byte[] imgData, int width, int height, int xoffset, int yoffset, int quality = 80)
    {
        if (imgData == null)
            throw new ArgumentNullException("imgData");

        if (width <= 0)
            throw new ArgumentOutOfRangeException("width");
        if (height <= 0)
            throw new ArgumentOutOfRangeException("height");
        if (xoffset <= 0)
            throw new ArgumentOutOfRangeException("xoffset");
        if (yoffset <= 0)
            throw new ArgumentOutOfRangeException("yoffset");

        using (var ori = SKBitmap.Decode(imgData))
        using (var img = new SKBitmap(new SKImageInfo(width, height)))
        {
            int right = xoffset + width;
            int bottom = yoffset + height;
            if (right > ori.Width)
                right = ori.Width;
            if (bottom > ori.Height)
                bottom = ori.Height;

            if (ori.ExtractSubset(img, new SKRectI(xoffset, yoffset, right, bottom)))
                using (var data = img.Encode(SKEncodedImageFormat.Jpeg, quality))
                    return data.ToArray();

            return null;
        }
    }
    public byte[] ThumbQ(byte[] imgData, int quality = 75)
    {
        using (var ori = SKBitmap.Decode(imgData))
        using (var res = ori.Resize(new SKImageInfo(ori.Width, ori.Height), SKFilterQuality.Medium))
        using (var img = SKImage.FromBitmap(res))
            return img.Encode(SKEncodedImageFormat.Jpeg, quality).ToArray();
    }

    public byte[] DrawRect(byte[] imgData, int x1, int y1, int x2, int y2, SKColor color, int brushSize)
    {
        using (var ori = SKBitmap.Decode(imgData))
        using (var canvas = new SKCanvas(ori))
        using (var paint = new SKPaint())
        {
            paint.Color = color;
            paint.StrokeWidth = brushSize;
            paint.Style = SKPaintStyle.Stroke;
            canvas.DrawRect(new SKRect(x1, y1, x2, y2), paint);
            canvas.Save();
            using (var res = ori.Resize(new SKImageInfo(ori.Width, ori.Height), SKFilterQuality.Medium))
            using (var img = SKImage.FromBitmap(res))
                return img.Encode(SKEncodedImageFormat.Jpeg, 80).ToArray();
        }
    }

    public byte[] BW(byte[] imgData)
    {
        int x, y;
        byte val;
        SKColor pixel;
        using (var ori = SKBitmap.Decode(imgData))
        {
            int width = ori.Width;
            int height = ori.Height;
            for (x = 0; x < width; x++)
            {
                for (y = 0; y < height; y++)
                {
                    pixel = ori.GetPixel(x, y);
                    val = (byte)((pixel.Red + pixel.Green + pixel.Blue) / 3);
                    ori.SetPixel(x, y, new SKColor(val, val, val));
                }
            }
            using (var res = ori.Resize(new SKImageInfo(width, height), SKFilterQuality.Medium))
            using (var img = SKImage.FromBitmap(res))
            {
                return img.Encode(SKEncodedImageFormat.Jpeg, 80).ToArray();
            }
        }
    }
    byte[] LD(byte[] imgData, byte value)
    {
        int x, y;
        byte r, g, b;
        SKColor pixel;
        using (var ori = SKBitmap.Decode(imgData))
        {
            int width = ori.Width;
            int height = ori.Height;
            for (x = 0; x < width; x++)
            {
                for (y = 0; y < height; y++)
                {
                    pixel = ori.GetPixel(x, y);
                    r = RGBFloor(pixel.Red + value);
                    g = RGBFloor(pixel.Green + value);
                    b = RGBFloor(pixel.Blue + value);
                    ori.SetPixel(x, y, new SKColor(r, g, b));
                }
            }
            using (var res = ori.Resize(new SKImageInfo(width, height), SKFilterQuality.Medium))
            using (var img = SKImage.FromBitmap(res))
            {
                return img.Encode(SKEncodedImageFormat.Jpeg, 80).ToArray();
            }
        }
    }
    byte RGBFloor(int value)
    {
        if (value < 0) return 0;
        if (value > 255) return 255;
        return (byte)value;
    }
    public byte[] RevColor(byte[] imgData)
    {
        int x, y;
        byte r, g, b;
        SKColor pixel;
        using (var ori = SKBitmap.Decode(imgData))
        {
            int width = ori.Width;
            int height = ori.Height;
            for (x = 0; x < width; x++)
            {
                for (y = 0; y < height; y++)
                {
                    pixel = ori.GetPixel(x, y);
                    r = (byte)(255 - pixel.Red);
                    g = (byte)(255 - pixel.Green);
                    b = (byte)(255 - pixel.Blue);
                    ori.SetPixel(x, y, new SKColor(r, g, b));
                }
            }
            using (var res = ori.Resize(new SKImageInfo(width, height), SKFilterQuality.Medium))
            using (var img = SKImage.FromBitmap(res))
            {
                return img.Encode(SKEncodedImageFormat.Jpeg, 80).ToArray();
            }
        }
    }


    byte[] FilterAlpha(byte[] imgData)
    {
        return FilterColor(imgData, 0);
    }
    byte[] FilterRed(byte[] imgData)
    {
        return FilterColor(imgData, 1);
    }
    byte[] FilterGreen(byte[] imgData)
    {
        return FilterColor(imgData, 2);
    }
    byte[] FilterBlue(byte[] imgData)
    {
        return FilterColor(imgData, 3);
    }
    byte[] FilterColor(byte[] imgData, int channel)
    {
        int x, y;
        SKColor pixel;
        using (var ori = SKBitmap.Decode(imgData))
        {
            int width = ori.Width;
            int height = ori.Height;
            for (x = 0; x < width; x++)
            {
                for (y = 0; y < height; y++)
                {
                    pixel = ori.GetPixel(x, y);
                    switch (channel)
                    {
                        case 0: ori.SetPixel(x, y, new SKColor(pixel.Red, pixel.Green, pixel.Blue, 0)); break;
                        case 1: ori.SetPixel(x, y, new SKColor(0, pixel.Green, pixel.Blue, pixel.Alpha)); break;
                        case 2: ori.SetPixel(x, y, new SKColor(pixel.Red, 0, pixel.Blue, pixel.Alpha)); break;
                        case 3: ori.SetPixel(x, y, new SKColor(pixel.Red, pixel.Green, 0, pixel.Alpha)); break;
                    }
                }
            }
            using (var res = ori.Resize(new SKImageInfo(width, height), SKFilterQuality.Medium))
            using (var img = SKImage.FromBitmap(res))
            {
                return img.Encode(SKEncodedImageFormat.Jpeg, 80).ToArray();
            }
        }
    }

    byte[] FlipV(byte[] imgData)
    {
        int x, y, z;
        SKColor pixel;
        using (var ori = SKBitmap.Decode(imgData))
        {
            int width = ori.Width;
            int height = ori.Height;
            for (y = height - 1; y >= 0; y--)
            {
                for (x = width - 1, z = 0; x >= 0; x--)
                {
                    pixel = ori.GetPixel(x, y);
                    ori.SetPixel(z++, y, new SKColor(pixel.Red, pixel.Green, pixel.Blue));
                }
            }
            using (var res = ori.Resize(new SKImageInfo(width, height), SKFilterQuality.Medium))
            using (var img = SKImage.FromBitmap(res))
            {
                return img.Encode(SKEncodedImageFormat.Jpeg, 80).ToArray();
            }
        }
    }
    byte[] FlipH(byte[] imgData)
    {
        int x, y, z;
        SKColor pixel;
        using (var ori = SKBitmap.Decode(imgData))
        {
            int width = ori.Width;
            int height = ori.Height;
            for (x = 0; x < width; x++)
            {
                for (y = height - 1, z = 0; y >= 0; y--)
                {
                    pixel = ori.GetPixel(x, y);
                    ori.SetPixel(x, z++, new SKColor(pixel.Red, pixel.Green, pixel.Blue));
                }
            }
            using (var res = ori.Resize(new SKImageInfo(width, height), SKFilterQuality.Medium))
            using (var img = SKImage.FromBitmap(res))
            {
                return img.Encode(SKEncodedImageFormat.Jpeg, 80).ToArray();
            }
        }
    }

    public byte[] QRCode(string content, int pixel = 11, int version = -1)
    {
        using (var gene = new QRCoder.QRCodeGenerator())
        using (var data = gene.CreateQrCode(content, QRCoder.QRCodeGenerator.ECCLevel.M, true, true, QRCoder.QRCodeGenerator.EciMode.Utf8, version))
        using (var code = new QRCoder.PngByteQRCode(data))
            return code.GetGraphic(pixel);
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
                img.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                return ms.GetBuffer();
            }
        }
    }
}