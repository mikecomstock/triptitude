﻿//using System;
//using System.ComponentModel;
//using System.Drawing;
//using System.Windows.Forms;

//namespace Triptitude.Biz
//{
//    public static class Util
//    {
//        //http://pietschsoft.com/post/2008/07/c-generate-webpage-thumbmail-screenshot-image.aspx
//        public static Bitmap GenerateScreenshot(string url, int width, int height)
//        {
//            // Load the webpage into a WebBrowser control
//            WebBrowser wb = new WebBrowser();
//            wb.ScrollBarsEnabled = true;
//            wb.ScriptErrorsSuppressed = true;
//            wb.Navigate(url);
//            while (wb.ReadyState != WebBrowserReadyState.Complete) { Application.DoEvents(); }

//            // Set the size of the WebBrowser control
//            wb.Width = width;
//            wb.Height = height;

//            // Get a Bitmap representation of the webpage as it's rendered in the WebBrowser control
//            Bitmap bitmap = new Bitmap(wb.Width, wb.Height);
//            wb.DrawToBitmap(bitmap, new Rectangle(0, 0, wb.Width, wb.Height));
//            wb.Dispose();

//            return bitmap;
//        }
//    }
//}


using System;
using System.ComponentModel;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using System.IO;

namespace SmallSharpTools.WebPreview
{
    public class PreviewBuilder
    {

        #region "  Variables  "

        private string _url = String.Empty;
        private string _filename = String.Empty;

        #endregion
        
        #region "  Constructor  "

        public PreviewBuilder(string url, string filename)
        {
            this._url = url;
            this._filename = filename;

            // ensure the output directory exists and create it is necessary
            FileInfo outputFile = new FileInfo(filename);
            if (!outputFile.Directory.Exists)
            {
                outputFile.Directory.Create();
            }
        }

        #endregion

        #region "  Methods  "

        public void CreatePreview()
        {
            ThreadStart ts = new ThreadStart(this.DoWork);
            Thread t = new Thread(ts);
            t.SetApartmentState(ApartmentState.STA);
            t.Start();
            t.Join();
        }

        private void DoWork()
        {
            Bitmap bitmap = GetPreviewImage();

            bitmap.Save(_filename);
            bitmap.Dispose();
        }

        private Bitmap GetPreviewImage()
        {
            WebBrowser wb = new WebBrowser();
            wb.ScrollBarsEnabled = false;
            wb.Size = new Size(Width, Height);
            wb.ScriptErrorsSuppressed = true;
            wb.NewWindow += new System.ComponentModel.CancelEventHandler(wb_NewWindow);
            wb.Navigate(_url);
            // wait for it to load
            while (wb.ReadyState != WebBrowserReadyState.Complete)
            {
                Application.DoEvents();
            }
            Bitmap bitmap = new Bitmap(Width, Height);
            Rectangle rect = new Rectangle(0, 0, Width, Height);
            wb.DrawToBitmap(bitmap, rect);
            wb.Dispose();
            return bitmap;
        }

        void wb_NewWindow(object sender, CancelEventArgs e)
        {
            e.Cancel = true;
        }

        #endregion

        #region "  Properties  "

        private int _width = 1024;

        public int Width
        {
            get { return _width; }
            set { _width = value; }
        }

        private int _height = 768;

        public int Height
        {
            get { return _height; }
            set { _height = value; }
        }

        #endregion

    }
}

