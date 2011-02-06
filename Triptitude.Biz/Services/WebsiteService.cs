using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Threading;
using SmallSharpTools.WebPreview;

namespace Triptitude.Biz.Services
{
    public class WebsiteService
    {
        public void CreateThumbnailAsync(string url)
        {
             _GenerateScreenshot(url);
            //var t = new Thread(_GenerateScreenshot);
            //t.SetApartmentState(ApartmentState.STA);
            //t.Start();
        }

        public void _GenerateScreenshot(string url)
        {
            string filename = "C:\\Users\\Mike\\Desktop\\Temp\\screenshots\\" + Guid.NewGuid() + ".bmp";
            var previewBuilder = new PreviewBuilder(url, filename);
            previewBuilder.CreatePreview();
            
        }
    }
}
