using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;

namespace Web_App_for_Image_Service.Models
{
    public class FormattedThumbnail
    {
        public string fullPath;

        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "name")]
        public string name;

        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "date")]
        public string date;

        public FormattedThumbnail(string path)
        {
            fullPath = path;
            string[] splittedPath = path.Split(@"\".ToCharArray());
            name = Path.GetFileName(path);
            int lastIndex = splittedPath.Length - 1;
            date = splittedPath[lastIndex - 1] + "." + splittedPath[lastIndex - 2];
        }
    }
}

