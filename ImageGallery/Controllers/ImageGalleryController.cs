using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using ImageGallery.Models;
using System.IO;
using System.Web;
using System.Drawing;
using System.Drawing.Imaging;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;

namespace ImageGallery.Controllers
{
    public class ImageGalleryController : ApiController
    {
        private string ConvertToTextFromPixelFormat(PixelFormat pixelFormat)
        {
            string bpp = string.Empty;

            switch (pixelFormat)
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

        [HttpGet, ActionName("getalluploadedimages")]
        public List<GalleryImage> GetAllUploadedImages()
        {
            DirectoryInfo uploadsDirectory = new DirectoryInfo(HttpContext.Current.Server.MapPath("~/Uploads"));
            List<GalleryImage> uploadedImages = new List<GalleryImage>();

            foreach (FileInfo file in uploadsDirectory.GetFiles())
            {
                using (Image image = Image.FromFile(file.FullName))
                {
                    uploadedImages.Add(new GalleryImage()
                    {
                        FileName = file.Name,
                        ColourDepth = ConvertToTextFromPixelFormat(image.PixelFormat),
                        FileSize = file.Length.ToString(),
                        Format = new ImageFormatConverter().ConvertToString(image.RawFormat),
                        Height = image.Height.ToString(),
                        Width = image.Width.ToString()
                    });
                }
            }

            return uploadedImages;
        }

        public HttpResponseMessage ImageDetailsRedirect([FromBody]FormDataCollection data)
        {
            string filename = data["filename"];
            HttpCookie cookie = new HttpCookie("galleryimage");
            cookie["filename"] = filename;
            var response = Request.CreateResponse(HttpStatusCode.OK, "more_details");
            response.Headers.AddCookies(new CookieHeaderValue[] { new CookieHeaderValue("filenameToView", filename) });

            return response;
        }

        [HttpGet, ActionName("getimagedetails")]
        public GalleryImage GetImageDetails()
        {
            var cookie = Request.Headers.GetCookies("filenameToView").FirstOrDefault();

            if (cookie != null)
            {
                var fileToView = Path.Combine(HttpContext.Current.Server.MapPath("~/Uploads"), cookie["filenameToView"].Value);
                using (FileStream fileStream = new FileStream(fileToView, FileMode.Open))
                {
                    using (Image image = System.Drawing.Image.FromStream(fileStream))
                    {
                        return new GalleryImage()
                        {
                            FileName = cookie["filenameToView"].Value,
                            ColourDepth = ConvertToTextFromPixelFormat(image.PixelFormat),
                            FileSize = fileStream.Length.ToString(),
                            Format = new ImageFormatConverter().ConvertToString(image.RawFormat),
                            Height = image.Height.ToString(),
                            Width = image.Width.ToString()
                        };
                    }
                }
            }
            else
            {
                return new GalleryImage();
            }
        }
    }
}
