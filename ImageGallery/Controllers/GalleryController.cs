using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ImageGallery.Controllers
{
    public class GalleryController : Controller
    {
        //
        // GET: /Gallery/

        public ActionResult Upload()
        {
            return View();
        }

        public ActionResult ImageDetails()
        {
            return View();
        }

    }
}
