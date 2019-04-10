namespace TY.Utility
{
    using System;

    public class MyHelper
    {
        public static string CountSize(long Size)
        {
            double num2;
            string str = "";
            long num = 0L;
            num = Size;
            if (num < 1024.0)
            {
                return (num.ToString("F2") + " Byte");
            }
            if ((num >= 1024.0) && (num < 0x100000L))
            {
                num2 = ((double) num) / 1024.0;
                return (num2.ToString("F2") + " KB");
            }
            if ((num >= 0x100000L) && (num < 0x40000000L))
            {
                num2 = (((double) num) / 1024.0) / 1024.0;
                return (num2.ToString("F2") + " MB");
            }
            if (num >= 0x40000000L)
            {
                str = ((((((double) num) / 1024.0) / 1024.0) / 1024.0)).ToString("F2") + " GB";
            }
            return str;
        }

        public static string FileFormat(string fileExtend)
        {
            switch (fileExtend.ToLower())
            {
                case ".doc":
                case ".docx":
                    return "Word文档";

                case ".pdf":
                    return "PDF文件";

                case ".htm":
                case ".html":
                case ".mht":
                case ".mhtml":
                    return "网页文件";

                case ".xls":
                case ".xlsx":
                case ".xlsm":
                case ".xlsb":
                    return "Excel文档";

                case ".jpg":
                case ".jpeg":
                    return "JPEG图片";

                case ".png":
                    return "PNG图片";

                case ".gif":
                    return "GIF图片";

                case ".bmp":
                    return "BMP图片";

                case ".txt":
                    return "文本文件";

                case ".xml":
                    return "XML文件";

                case ".rar":
                    return "RAR压缩文件";

                case ".zip":
                    return "ZIP压缩文件";
            }
            return (fileExtend.Replace(".", "").ToUpper() + "文件");
        }
    }
}

