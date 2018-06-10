using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using Microsoft.AspNetCore.Hosting;

namespace Web_App_for_Image_Service.Models
{
    public class PicturesModel
    {
        #region Members
        private string outputDir;
        private string thumbnailsDir;
        public List<Picture> pictures;

        public event EventHandler<int> PictureDeleted;
        #endregion

        public PicturesModel()
        {
            pictures = new List<Picture>();
            thumbnailsDir = @"wwwroot/OutputDir/Thumbnails";
            foreach (string path in Directory.GetFiles(thumbnailsDir, "*", SearchOption.AllDirectories))
            {
                string pathRel =  path.Replace("wwwroot", "~");
                pictures.Add(new Picture(pathRel));
                Console.Out.WriteLine(pathRel);
            }
        }

        public void DeletePicture(Picture pic)
        {
            pictures.Remove(pic);
            string path1 = pic.originalImagePath.Replace("~", "wwwroot").Replace(@"/", @"\");
            string path2 = pic.thumbnailPath.Replace("~", "wwwroot").Replace(@"/", @"\");
            System.IO.File.Delete(path1);
            System.IO.File.Delete(path2);
            PictureDeleted?.Invoke(this,pictures.Count);
            Thread.Sleep(2000);
        }
    }
}
