using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ImageGallery.Models
{
    public class GalleryImage
    {
        public string FileName { get; set; }
        public string FileSize { get; set; }
        public string ColourDepth { get; set; }
        public string Width { get; set; }
        public string Height { get; set; }
        public string Format { get; set; }
    }
}