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
        public static HomePageModel homeModel;
        public static PicturesModel picturesModel;
        public static LogPageModel logModel;
        public static ConfigModel configModel;

        public HomeController()
        {
            if (picturesModel == null) picturesModel = new PicturesModel();
            if (homeModel == null) homeModel = new HomePageModel(picturesModel.pictures.Count);
            picturesModel.PictureDeleted += homeModel.UpdatePicCounter;
            if (logModel == null) logModel = new LogPageModel();
            if (configModel == null) configModel = new ConfigModel();

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

        public IActionResult Config()
        {
            return View(configModel);
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
            return View("Photos", picturesModel);
        }

        public IActionResult FullPhoto(string srcPath)
        {
            Picture picture = picturesModel.pictures.Find(photo => photo.originalImagePath == srcPath);
            if (picture != null) ViewBag.PhotoPathToDelete = picture.name;
            return View(picture);
        }

        public IActionResult SearchResults(string status)
        {
           logModel.SearchLogs(status);
           return View("Logs", logModel);
        }

        public IActionResult HandlerConfirmation(string handlerPath)
        {
            configModel.selectedHandler = handlerPath;
            return View("HandlerConfirmation",configModel);
        }

        public IActionResult RemoveHandler(string handlerPath)
        {
            if(!string.IsNullOrEmpty(handlerPath)) configModel.RemoveHandler(handlerPath);
            return View("Config", configModel);
        }
    }
}
