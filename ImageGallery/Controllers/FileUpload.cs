using System;
using System.Drawing;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using ImageGallery.Models;
using System.Drawing.Imaging;

namespace ImageGallery.Controllers
{
    public class FileUploadController : ApiController
    {
        private string ConvertToTextFromPixelFormat(PixelFormat pixelFormat) 
        {
            string bpp = string.Empty;

            switch(pixelFormat)       
              {
                 case PixelFormat.Format8bppIndexed:
                    bpp = "8bpp";
                    break;
                case PixelFormat.Format16bppArgb1555:
                case PixelFormat.Format16bppGrayScale:
                case PixelFormat.Format16bppRgb555:
                case PixelFormat.Format16bppRgb565:
                    bpp = "16bpp";
                    break;
                 case PixelFormat.Format24bppRgb:
                    bpp = "24bpp";
                    break;
                 case PixelFormat.Format32bppArgb:
                 case PixelFormat.Format32bppPArgb:
                    bpp = "32bpp";
                    break;
                 default:
                    bpp = "0";
                    break;      
             }
            
            return bpp;
        }

        public HttpResponseMessage UploadFile()
        {
            List<string> acceptedExtensions = new List<string>() { "jpg", "jpeg", "png", "bmp" };
            Image galleryImage;

            if (HttpContext.Current.Request.Files.AllKeys.Any())
            {
                var uploadedFile = HttpContext.Current.Request.Files["UploadedFile"];
                var extension = uploadedFile.FileName.ToLower().Substring(uploadedFile.FileName.ToLower().LastIndexOf(".") + 1,
                    (uploadedFile.FileName.ToLower().Length - 1) - uploadedFile.FileName.ToLower().LastIndexOf("."));

                if (((List<string>)(from i in acceptedExtensions
                                    where i == extension.ToLower()
                                    select i).ToList<string>()).Count > 0)
                {
                    var fileSavePath = Path.Combine(HttpContext.Current.Server.MapPath("~/Uploads"), uploadedFile.FileName);
                    uploadedFile.SaveAs(fileSavePath);
                    galleryImage = System.Drawing.Image.FromStream(uploadedFile.InputStream);

                    return Request.CreateResponse(HttpStatusCode.OK,
                        new GalleryImage() {
                            ColourDepth = ConvertToTextFromPixelFormat(galleryImage.PixelFormat),
                            FileSize = uploadedFile.ContentLength.ToString(),
                            Format = new ImageFormatConverter().ConvertToString(galleryImage.RawFormat),
                            Height = galleryImage.Height.ToString(),
                            Width = galleryImage.Width.ToString()
                        }
                    );
                }
            }

            return Request.CreateResponse(HttpStatusCode.InternalServerError);
        }
    }
}
