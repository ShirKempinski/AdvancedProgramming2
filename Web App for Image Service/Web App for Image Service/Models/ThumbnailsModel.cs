using System;
using System.Collections.Generic;
using System.IO;

namespace Web_App_for_Image_Service.Models
{
    public class ThumbnailsModel
    {
        #region Members
        private string outputDir;
        private string thumbnailsDir;
        public List<FormattedThumbnail> thumbnailsFormats;
        #endregion

        public ThumbnailsModel()
        {
            outputDir = @"OutputDir";
            thumbnailsDir = outputDir + @"\Thumbnails";
            foreach (string path in Directory.GetFiles(thumbnailsDir, "*", SearchOption.AllDirectories))
            {
                thumbnailsFormats.Add(new FormattedThumbnail(path));
            }
        }
    }
}
