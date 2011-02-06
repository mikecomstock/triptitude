using System;
using System.Configuration;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using Triptitude.Biz.Models;

namespace Triptitude.Biz.Services
{
    public class WebsiteService
    {
        public void AddWebsite(string url)
        {
            Website website = new Website();
            website.URL = url;
            website.Title = "New website";

            var repo = new Repo<Website>();
            repo.Add(website);
            repo.Save();

            CreateThumbnail(url, website.Id);
        }

        private static void CreateThumbnail(string url, int websiteId)
        {
            string websiteThumbnailRoot = ConfigurationManager.AppSettings["WebsiteThumbnailRoot"];

            string originalOutputPath = Path.Combine(websiteThumbnailRoot, "Original", websiteId + ".jpg");
            string smallOutputPath = Path.Combine(websiteThumbnailRoot, "Small", websiteId + ".jpg");
            string mediumOutputPath = Path.Combine(websiteThumbnailRoot, "Medium", websiteId + ".jpg");
            string largeOutputPath = Path.Combine(websiteThumbnailRoot, "Large", websiteId + ".jpg");

            string cutyCaptPath = ConfigurationManager.AppSettings["CutyCaptPath"];

            string arguments = string.Format(@"--url=""{0}"" --out=""{1}""", url, originalOutputPath);
            var proc = new Process()
            {
                StartInfo = new ProcessStartInfo(cutyCaptPath, arguments) { UseShellExecute = false, CreateNoWindow = true }
            };
            proc.Start();
            proc.WaitForExit();

            if (File.Exists(originalOutputPath))
            {
                using (Image original = Image.FromFile(originalOutputPath))
                {
                    int cropWidth = original.Width;
                    int cropHeight = (int)Math.Round((decimal)600 / 800 * original.Width, 0);
                    using (Image croppedOriginal = new Bitmap(original).Clone(new Rectangle(0, 0, cropWidth, cropHeight), original.PixelFormat))
                    {
                        Bitmap small = ResizeImage(croppedOriginal, 100, 750);
                        small.Save(smallOutputPath, ImageFormat.Jpeg);

                        Bitmap medium = ResizeImage(croppedOriginal, 200, 150);
                        medium.Save(mediumOutputPath, ImageFormat.Jpeg);
                        
                        Bitmap large = ResizeImage(croppedOriginal, 300, 225);
                        large.Save(largeOutputPath, ImageFormat.Jpeg);
                    }
                }

                File.Delete(originalOutputPath);
            }
        }

        //http://stackoverflow.com/questions/249587/high-quality-image-scaling-c
        public static System.Drawing.Bitmap ResizeImage(System.Drawing.Image image, int width, int height)
        {
            //a holder for the result
            Bitmap result = new Bitmap(width, height);

            //use a graphics object to draw the resized image into the bitmap
            using (Graphics graphics = Graphics.FromImage(result))
            {
                //set the resize quality modes to high quality
                graphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                //draw the image into the target bitmap
                graphics.DrawImage(image, 0, 0, result.Width, result.Height);
            }

            //return the resulting bitmap
            return result;
        }
    }
}
