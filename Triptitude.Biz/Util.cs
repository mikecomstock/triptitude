using System;
using System.Configuration;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System.Web;
using Triptitude.Biz.Models;

namespace Triptitude.Biz
{
    public static class Util
    {
        public static bool ServerIsProduction
        {
            get { return HttpContext.Current != null && HttpContext.Current.Request.Url.Host.Contains("triptitude.com"); }
        }

        //public static void CreateThumbnails(string url, int websiteId)
        //{
        //    string websiteThumbnailRoot = ConfigurationManager.AppSettings["WebsiteThumbnailRoot"];

        //    string originalOutputPath = Path.Combine(websiteThumbnailRoot, websiteId + "-original.jpg");
        //    string smallOutputPath = Path.Combine(websiteThumbnailRoot, WebsiteActivity.ThumbFilename(websiteId, WebsiteActivity.ThumbSize.Small));
        //    string mediumOutputPath = Path.Combine(websiteThumbnailRoot, WebsiteActivity.ThumbFilename(websiteId, WebsiteActivity.ThumbSize.Medium));
        //    string largeOutputPath = Path.Combine(websiteThumbnailRoot, WebsiteActivity.ThumbFilename(websiteId, WebsiteActivity.ThumbSize.Large));

        //    string cutyCaptPath = ConfigurationManager.AppSettings["CutyCaptPath"];
        //    string userAgent = "Mozilla/5.0 (Windows; U; Windows NT 6.1; en-US) AppleWebKit/534.13 (KHTML, like Gecko) Chrome/9.0.597.84 Safari/534.13";
        //    string arguments = string.Format(@"--url=""{0}"" --out=""{1}"" --user-agent=""{2}""", url, originalOutputPath, userAgent);
        //    var proc = new Process()
        //    {
        //        StartInfo = new ProcessStartInfo(cutyCaptPath, arguments) { UseShellExecute = false, CreateNoWindow = true }
        //    };
        //    proc.Start();
        //    proc.WaitForExit();

        //    if (File.Exists(originalOutputPath))
        //    {
        //        using (Image original = Image.FromFile(originalOutputPath))
        //        {
        //            int cropWidth = original.Width;
        //            int cropHeight = (int)Math.Round((decimal)800 / 800 * original.Width, 0);
        //            using (Image croppedOriginal = new Bitmap(original).Clone(new Rectangle(0, 0, cropWidth, cropHeight), original.PixelFormat))
        //            {
        //                Bitmap small = ResizeImage(croppedOriginal, 70, 70);
        //                small.Save(smallOutputPath, ImageFormat.Jpeg);

        //                Bitmap medium = ResizeImage(croppedOriginal, 200, 200);
        //                medium.Save(mediumOutputPath, ImageFormat.Jpeg);

        //                Bitmap large = ResizeImage(croppedOriginal, 300, 300);
        //                large.Save(largeOutputPath, ImageFormat.Jpeg);
        //            }
        //        }

        //        File.Delete(originalOutputPath);
        //    }
        //}

        //http://stackoverflow.com/questions/249587/high-quality-image-scaling-c
        private static System.Drawing.Bitmap ResizeImage(System.Drawing.Image image, int width, int height)
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

        public static string DateTimeRangeString(int beginDay, TimeSpan? beginTime, int endDay, TimeSpan? endTime)
        {
            string dateTimeString = string.Empty;

            if (beginDay == endDay && !beginTime.HasValue && !endTime.HasValue)
            {
                dateTimeString += string.Format("Day {0}", beginDay);
            }
            else
            {
                if (beginTime.HasValue) dateTimeString += string.Format("{0} ", DateTime.Today.Add(beginTime.Value).ToShortTimeString());
                dateTimeString += string.Format("Day {0}", beginDay);

                dateTimeString += " - ";

                if (endTime.HasValue) dateTimeString += string.Format("{0} ", DateTime.Today.Add(endTime.Value).ToShortTimeString());
                dateTimeString += string.Format("Day {0}", endDay);
            }

            return dateTimeString;
        }

        //public static string GetWebsiteTitle(string url)
        //{
        //    try
        //    {
        //        WebRequest webRequest = WebRequest.Create(url);
        //        webRequest.Timeout = 5000;
        //        using (WebResponse webResponse = webRequest.GetResponse())
        //        using (Stream responseStream = webResponse.GetResponseStream())
        //        using (StreamReader streamReader = new StreamReader(responseStream))
        //        {
        //            string content = streamReader.ReadToEnd();

        //            streamReader.Close();
        //            responseStream.Close();
        //            webResponse.Close();

        //            string title = Regex.Match(content, @"\<title\b[^>]*\>\s*(?<Title>[\s\S]*?)\</title\>", RegexOptions.IgnoreCase).Groups["Title"].Value;
        //            title = HttpUtility.HtmlDecode(title);
        //            return title;
        //        }
        //    }
        //    catch
        //    {
        //        return url;
        //    }
        //}
    }
}
