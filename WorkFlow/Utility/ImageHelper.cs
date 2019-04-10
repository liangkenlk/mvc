namespace TY.Utility
{
    using System;
    using System.Drawing;
    using System.Drawing.Drawing2D;
    using System.Drawing.Imaging;
    using System.IO;

    public static class ImageHelper
    {
        public static void GenThumbnail(Stream fileStream, string pathImageTo, int thumbWidth, int thumbHeight)
        {
            Image image = null;
            try
            {
                image = Image.FromStream(fileStream);
            }
            catch
            {
            }
            if (image != null)
            {
                int width = image.Width;
                int height = image.Height;
                int num3 = thumbWidth;
                int num4 = thumbHeight;
                int x = 0;
                int y = 0;
                if ((num4 * width) > (num3 * height))
                {
                    num4 = (height * thumbWidth) / width;
                    y = (thumbWidth - num4) / 2;
                }
                else
                {
                    num3 = (width * thumbHeight) / height;
                    x = (thumbWidth - num3) / 2;
                }
                Bitmap bitmap = new Bitmap(thumbWidth, thumbHeight);
                Graphics graphics = Graphics.FromImage(bitmap);
                graphics.Clear(Color.White);
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.DrawImage(image, new Rectangle(x, y, num3, num4), new Rectangle(0, 0, width, height), GraphicsUnit.Pixel);
                try
                {
                    bitmap.Save(pathImageTo, ImageFormat.Jpeg);
                }
                catch (Exception exception)
                {
                    throw exception;
                }
                finally
                {
                    image.Dispose();
                    bitmap.Dispose();
                    graphics.Dispose();
                }
            }
        }

        public static void GenThumbnail(string fromPath, string pathImageTo, int thumbWidth, int thumbHeight)
        {
            Image image = null;
            try
            {
                image = Image.FromFile(fromPath);
            }
            catch
            {
            }
            if (image != null)
            {
                int width = image.Width;
                int height = image.Height;
                int num3 = thumbWidth;
                int num4 = thumbHeight;
                int x = 0;
                int y = 0;
                if ((num4 * width) > (num3 * height))
                {
                    num4 = (height * thumbWidth) / width;
                    y = (thumbWidth - num4) / 2;
                }
                else
                {
                    num3 = (width * thumbHeight) / height;
                    x = (thumbWidth - num3) / 2;
                }
                Bitmap bitmap = new Bitmap(thumbWidth, thumbHeight);
                Graphics graphics = Graphics.FromImage(bitmap);
                graphics.Clear(Color.White);
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.DrawImage(image, new Rectangle(x, y, num3, num4), new Rectangle(0, 0, width, height), GraphicsUnit.Pixel);
                try
                {
                    bitmap.Save(pathImageTo, ImageFormat.Jpeg);
                }
                catch (Exception exception)
                {
                    throw exception;
                }
                finally
                {
                    image.Dispose();
                    bitmap.Dispose();
                    graphics.Dispose();
                }
            }
        }

        public static bool GetFixedRatioImage(string srcFile, string destFile, int maxWidth, int maxHeight)
        {
            if (!File.Exists(srcFile))
            {
                return false;
            }
            int width = 0;
            int height = 0;
            using (Image image = Image.FromFile(srcFile))
            {
                if ((image.Width > maxWidth) && (image.Height > maxHeight))
                {
                    width = maxWidth;
                    height = (maxWidth * image.Height) / image.Width;
                    if (height > maxHeight)
                    {
                        width = (maxHeight * image.Width) / image.Height;
                        height = maxHeight;
                    }
                }
                else
                {
                    width = image.Width;
                    height = image.Height;
                }
                using (Image image2 = new Bitmap(width, height))
                {
                    using (Graphics graphics = Graphics.FromImage(image2))
                    {
                        graphics.InterpolationMode = InterpolationMode.High;
                        graphics.SmoothingMode = SmoothingMode.HighQuality;
                        graphics.Clear(Color.Transparent);
                        graphics.DrawImage(image, new Rectangle(0, 0, width, height), 0, 0, image.Width, image.Height, GraphicsUnit.Pixel);
                        image2.Save(destFile, ImageFormat.Jpeg);
                    }
                }
            }
            return true;
        }

        public static bool GetFixedSizeImage(string srcFile, string destFile, int destWidth, int destHeight)
        {
            if (!File.Exists(srcFile))
            {
                return false;
            }
            int srcWidth = 0;
            int srcHeight = 0;
            using (Image image = Image.FromFile(srcFile))
            {
                if ((image.Width > destWidth) && (image.Height > destHeight))
                {
                    if ((((float) image.Width) / ((float) destWidth)) > (((float) image.Height) / ((float) destHeight)))
                    {
                        srcWidth = (image.Height * destWidth) / destHeight;
                        srcHeight = image.Height;
                    }
                    else
                    {
                        srcWidth = image.Width;
                        srcHeight = (image.Width * destHeight) / destWidth;
                    }
                }
                else
                {
                    srcWidth = image.Width;
                    srcHeight = image.Height;
                    destWidth = image.Width;
                    destHeight = image.Height;
                }
                using (Image image2 = new Bitmap(destWidth, destHeight))
                {
                    using (Graphics graphics = Graphics.FromImage(image2))
                    {
                        graphics.InterpolationMode = InterpolationMode.High;
                        graphics.SmoothingMode = SmoothingMode.HighQuality;
                        graphics.Clear(Color.Transparent);
                        graphics.DrawImage(image, new Rectangle(0, 0, destWidth, destHeight), 0, 0, srcWidth, srcHeight, GraphicsUnit.Pixel);
                        image2.Save(destFile, ImageFormat.Jpeg);
                    }
                }
            }
            return true;
        }

        public static void MakeThumbnail(string originalImagePath, string thumbnailPath, int width, int height, string mode)
        {
            Image image = Image.FromFile(originalImagePath);
            int num = width;
            int num2 = height;
            int x = 0;
            int y = 0;
            int num5 = image.Width;
            int num6 = image.Height;
            string str = mode;
            if ((str != null) && (str != "HW"))
            {
                if (!(str == "W"))
                {
                    if (str == "H")
                    {
                        num = (image.Width * height) / image.Height;
                    }
                    else if (str == "Cut")
                    {
                        if ((((double) image.Width) / ((double) image.Height)) > (((double) num) / ((double) num2)))
                        {
                            num6 = image.Height;
                            num5 = (image.Height * num) / num2;
                            y = 0;
                            x = (image.Width - num5) / 2;
                        }
                        else
                        {
                            num5 = image.Width;
                            num6 = (image.Width * height) / num;
                            x = 0;
                            y = (image.Height - num6) / 2;
                        }
                    }
                }
                else
                {
                    num2 = (image.Height * width) / image.Width;
                }
            }
            Image image2 = new Bitmap(num, num2);
            Graphics graphics = Graphics.FromImage(image2);
            graphics.InterpolationMode = InterpolationMode.High;
            graphics.SmoothingMode = SmoothingMode.HighQuality;
            graphics.Clear(Color.Transparent);
            graphics.DrawImage(image, new Rectangle(0, 0, num, num2), new Rectangle(x, y, num5, num6), GraphicsUnit.Pixel);
            try
            {
                image2.Save(thumbnailPath, ImageFormat.Jpeg);
            }
            catch (Exception exception)
            {
                throw exception;
            }
            finally
            {
                image.Dispose();
                image2.Dispose();
                graphics.Dispose();
            }
        }
    }
}

