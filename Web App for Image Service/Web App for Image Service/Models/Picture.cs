using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;

namespace Web_App_for_Image_Service.Models
{
    public class Picture
    {
        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "thumbnailPath")]
        public string thumbnailPath;

        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "name")]
        public string name;

        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "date")]
        public string date;

        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "originalImagePath")]
        public string originalImagePath;

        public Picture(string thumbnailPath)
        {
            this.thumbnailPath = thumbnailPath;
            originalImagePath = thumbnailPath.Replace(@"Thumbnails\", "");
            string[] splittedPath = thumbnailPath.Split(@"\".ToCharArray());
            name = Path.GetFileName(thumbnailPath);
            int lastIndex = splittedPath.Length - 1;
            if (splittedPath[lastIndex - 1] == "UndefinedDate") date = "Undefined";
            else date = splittedPath[lastIndex - 1] + "." + splittedPath[lastIndex - 2];
        }


    }
}

