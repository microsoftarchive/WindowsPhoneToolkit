using System;
using System.Collections.Generic;
using System.Windows.Media.Imaging;
using System.Xml.Serialization;

namespace PictureViewer
{
    public class Picture
    {
        [XmlElement("Name")]
        public string Name { get; set; }

        [XmlElement("Date")]
        public string Date { get; set; }

        [XmlElement("Desc")]
        public string Desc { get; set; }

        [XmlElement("Url")]
        public string Url { get; set; }

        [XmlIgnore]
        public BitmapSource Bitmap { get; set; }
    }
}