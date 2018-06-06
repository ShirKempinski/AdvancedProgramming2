using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Web_App_for_Image_Service.Models;

namespace Web_App_for_Image_Service.Controllers
{
    public class HomeController : Controller
    {
        public HomePageModel homeModel;
        public ThumbnailsModel thumbnailsModel;

        public HomeController()
        {
            homeModel = new HomePageModel();
            thumbnailsModel = new ThumbnailsModel();
        }
        public IActionResult Index()
        {
            return View(homeModel);
        }

        public IActionResult Photos()
        {
            ViewData["Message"] = "Your application Photos page.";

            return View(thumbnailsModel);
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
