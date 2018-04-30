using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageService
{
    /// <summary>
    /// IImageModal interface has an AddFile function.
    /// </summary>
    public interface IImageModal
    {
        string AddFile(List<String> path, out bool result);
    }
}
