using System;
using System.Windows.Media.Imaging;
using System.Collections.Generic;

namespace PictureViewer
{
    public class Picture
    {
        public string Name { get; set; }
        public string Date { get; set; }
        public string Desc { get; set; }
        public string Url { get; set; }
        public BitmapSource Bitmap { get; set; }
    }
}