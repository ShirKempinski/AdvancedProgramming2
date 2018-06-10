using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Web_App_for_Image_Service.Models;

namespace Web_App_for_Image_Service.Controllers
{
    public class HomeController : Controller
    {
        public HomePageModel homeModel;
        public PicturesModel picturesModel;
        public LogPageModel logModel;

        public HomeController()
        {
            picturesModel = new PicturesModel();
            homeModel = new HomePageModel(picturesModel.pictures.Count);
            picturesModel.PictureDeleted += homeModel.UpdatePicCounter;
            logModel = new LogPageModel();
        }
        public IActionResult Home()
        {
            return View(homeModel);
        }

        public IActionResult Photos()
        {
            return View(picturesModel);
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Logs()
        {
            return View(logModel);
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult DeleteConfirmation(string srcPath)
        {
            Picture picture = picturesModel.pictures.Find(photo => photo.originalImagePath == srcPath);
            return View(picture);
            
        }

        public IActionResult DeletePicture(string originalPath)
        {

            Picture picture = picturesModel.pictures.Find(photo => photo.originalImagePath == originalPath);
            if (picture != null) picturesModel.DeletePicture(picture);
            return RedirectToAction("Photos");
        }

        public IActionResult FullPhoto(string srcPath)
        {
            Picture picture = picturesModel.pictures.Find(photo => photo.originalImagePath == srcPath);
            if (picture != null) ViewBag.PhotoPathToDelete = picture.name;
            return View(picture);
        }

        public IActionResult SearchResults(string status)
        {
            SearchResultModel sr = new SearchResultModel(logModel.SearchLogs(status));
           return View(sr);
        }
    }
}
