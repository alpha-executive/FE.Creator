using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FE.Creator.FileStorage
{
    interface IThumbinalGenerator
    {
       bool IsMatchFormat { get; }

        Image GetThumbinal(int width, int height);
    }
    class GeneralFileThumbinalGenerator : IThumbinalGenerator
    {
        private string mFullFileName = string.Empty;
        private static Dictionary<string, Bitmap> thumbinalMappings = new Dictionary<string, Bitmap>();
        static GeneralFileThumbinalGenerator() {
            thumbinalMappings.Add(".docx", Properties.Resources.docx);
            thumbinalMappings.Add(".doc", Properties.Resources.docx);
            thumbinalMappings.Add(".xlsx", Properties.Resources.xlsx);
            thumbinalMappings.Add(".xls", Properties.Resources.xlsx);
            thumbinalMappings.Add(".pptx", Properties.Resources.pptx);
            thumbinalMappings.Add(".ppt", Properties.Resources.pptx);
            thumbinalMappings.Add(".pdf", Properties.Resources.pdf);
            thumbinalMappings.Add(".iso", Properties.Resources.iso);
            thumbinalMappings.Add(".html", Properties.Resources.html);
            thumbinalMappings.Add(".htm", Properties.Resources.html);

            //Zip, rar, 7z, gz, tar.
            thumbinalMappings.Add(".zip", Properties.Resources.zip);
            thumbinalMappings.Add(".rar", Properties.Resources.zip);
            thumbinalMappings.Add(".7z", Properties.Resources.zip);
            thumbinalMappings.Add(".gz", Properties.Resources.zip);
            thumbinalMappings.Add(".tar", Properties.Resources.zip);
       }

        public string LowerCaseExtension
        {
            get
            {
                if (!File.Exists(mFullFileName))
                {
                    return string.Empty;
                }

                return (new FileInfo(mFullFileName))
                    .Extension
                    .ToLower();
            }
        }
        public bool IsMatchFormat
        {
            get
            {
                return true;
            }
        }

        public GeneralFileThumbinalGenerator(string fullFileName)
        {
            this.mFullFileName = fullFileName;
        }

        public Image GetThumbinal(int width, int height)
        {
            if (thumbinalMappings
                    .ContainsKey(this.LowerCaseExtension))
                return thumbinalMappings[this.LowerCaseExtension];

            return Properties.Resources.doc;
        }
    }

    class ImageFileThumbinalGenerator : IThumbinalGenerator
    {
        Image.GetThumbnailImageAbort imageAbortHandler = new Image.GetThumbnailImageAbort(ThumbinalAbort);

        private string mFullFileName = string.Empty;
        private static string[] SUPPORTED_FILE_TYPES = new string[]
        {
            ".jpg",".png", ".bmp", ".gif", ".jpeg"
        };
        public bool IsMatchFormat
        {
            get
            {
                if (!File.Exists(mFullFileName))
                    return false;

                return SUPPORTED_FILE_TYPES.Contains((new FileInfo(mFullFileName))
                    .Extension
                    .ToLower());
            }
        }
        public ImageFileThumbinalGenerator(string fullFileName)
        {
            mFullFileName = fullFileName;
        }

        private static bool ThumbinalAbort()
        {
            return false;
        }
        public Image GetThumbinal(int width, int height)
        {
            if (!File.Exists(mFullFileName))
            {
                return null;
            }

            Image orignialImage = Image
                .FromFile(mFullFileName);
           
            //get the thumbinal compress ratio.
            double ratio = width * 1.0 / orignialImage.Width <= height * 1.0 / orignialImage.Height?
                width * 1.0 / orignialImage.Width : height * 1.0 / orignialImage.Height;

            return
                orignialImage.GetThumbnailImage((int)(orignialImage.Width * ratio), (int)(orignialImage.Height * ratio), imageAbortHandler, IntPtr.Zero);
        }
    }
    public class SimpleFileThumbinalGenerator
    {
        public static Image GetThumbnail(string fileName, int width, int height) {
            ImageFileThumbinalGenerator imageGenerator = new ImageFileThumbinalGenerator(fileName);
            if (imageGenerator.IsMatchFormat)
                return imageGenerator.GetThumbinal(width, height);

            GeneralFileThumbinalGenerator generalFileGenrator = new GeneralFileThumbinalGenerator(fileName);
            if (generalFileGenrator.IsMatchFormat)
                return generalFileGenrator.GetThumbinal(width, height);

            return null;
        }
    }
}
