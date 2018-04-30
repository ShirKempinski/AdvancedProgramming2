using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text.RegularExpressions;
using System.Threading;

namespace ImageService
{
    /// <summary>
    /// ImageModal class. Responsible for moving Image files between folders.
    /// </summary>
    /// <remarks>
    /// Implements the IImageModal interface.
    /// </remarks>
    public class ImageModal : IImageModal
    {
        #region Members
        private int thumbnailSize { get; set; }
        private string thumbnailsDir;
        private string outputDir;
        #endregion

        /// <summary>
        /// ImageModal constructor.
        /// </summary>
        /// <param name="thumbnailSize">The size of the Thumbnail to be generated from the added files</param>
        /// <param name="outputDir">The directory to which the files must be moved</param>
        /// <returns>
        /// A new ImageModal object
        /// </returns>
        public ImageModal(int thumbnailSize, string outputDir)
        {
            this.thumbnailSize = thumbnailSize;
            this.outputDir = outputDir;
            thumbnailsDir = outputDir + @"\Thumbnails";
        }

        /// <summary>
        /// IImageModal method.
        /// Adds the designated file to the assigned output directory.
        /// </summary>
        /// <param name="path">List<String> fileName and filePath></param>
        /// <param name="result">True if success, false otherwise</param>
        /// <returns>
        /// A string containing the destination filename.
        /// </returns>
        public string AddFile(List<String> path, out bool result)
        {
            try
            {   
                String sourcePath = path[0];
                String fileName = path[1];
                string sourceFile = Path.Combine(sourcePath, fileName);
                while (IsFileLocked(new FileInfo(sourceFile)))
                {
                    Thread.Sleep(100);
                }
                string targetPath = outputDir + pathFromDate(sourceFile);
                string destFile = GenerateDestName(fileName, targetPath);
                CreateHiddenDirectory(targetPath);
                AddThumbnail(path);
                MoveFile(sourceFile, destFile);
                result = true;
                return destFile;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                result = false;
                return "Error: File" + path[1] + " coudn't be added to " + outputDir + "\n";
            }
        }

        /// <summary>
        /// Creates a destination path composed of the fileName and targetPath.
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="targetPath"> Destination directory.</param>
        /// <returns>
        /// The generated destination path string.
        /// </returns>
        /// <remarks> A number is appended if a file of the same name already exists.</remarks>
        private string GenerateDestName(string fileName, string targetPath)
        {
            string destFile = Path.Combine(targetPath, fileName);
            while (File.Exists(destFile))
            {
                int i = 1;
                string extension = Path.GetExtension(fileName);
                fileName = fileName.TrimEnd(extension.ToCharArray());
                if (fileName.EndsWith("(" + i + ")"))
                {
                    fileName = fileName.TrimEnd(("(" + i + ")").ToCharArray());
                    i++;
                }
                fileName = fileName + "(" + i + ")" + extension;
                destFile = Path.Combine(targetPath, fileName);
            }
            return destFile;
        }

        /// <summary>
        /// Creates a directory with the Hidden FileAttribute.
        /// </summary>
        /// <param name="targetPath">The directories name and path</param>
        private void CreateHiddenDirectory(string targetPath)
        {
            if (!Directory.Exists(targetPath))
            {
                DirectoryInfo di = Directory.CreateDirectory(targetPath);
                di.Attributes = FileAttributes.Directory | FileAttributes.Hidden;
            }
        }

        /// <summary>
        /// Copies a file from the given sourceFile path to the destFile path.
        /// </summary>
        /// <param name="sourceFile">Source file path.</param>
        /// <param name="destFile">Destination file path.</param>
        private void CopyFile(string sourceFile, string destFile)
        {
            File.Copy(sourceFile, destFile, true);
        }

        /// <summary>
        /// Moves a file from the given sourceFile path to the destFile path.
        /// </summary>
        /// <param name="sourceFile">Source file path.</param>
        /// <param name="destFile">Destination file path.</param>
        private void MoveFile(string sourceFile, string destFile)
        {
            File.Move(sourceFile, destFile);
        }

        /// <summary>
        /// Extracts the DateTime information from the specified Image.
        /// </summary>
        /// <param name="path">The target Image path.</param>
        /// <returns>
        /// DateTime object with the target Image information.
        /// </returns>
        private DateTime GetImageDateTime(String path)
        {
            using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read))
                try
                {
                    using (Image myImage = Image.FromStream(fs, false, false))
                    {
                        if (!myImage.PropertyIdList.Any(p => p == 36867))
                        {
                            return new DateTime(1, 1, 1);
                        }
                        Regex r = new Regex(":");
                        PropertyItem propItem = myImage.GetPropertyItem(36867);
                        string dateTaken = r.Replace(Encoding.UTF8.GetString(propItem.Value), "-", 2);
                        return DateTime.Parse(dateTaken);
                    }
                } catch (IOException e)
                {
                    return new DateTime(1, 1, 1);
                } finally
                {
                    if (fs != null)
                        fs.Close();
                }
        }

        /// <summary>
        /// Creates a Thumbail version of the target Image and adds it to the relevant Thumbnail folder.
        /// </summary>
        /// <param name="path">String<List> Target filename and file path</param>
        /// <returns>
        /// True if succeeded, false otherwise.
        /// </returns>
        private bool AddThumbnail(List<String> path)
        {
            try
            {
                String sourcePath = path[0];
                String fileName = path[1];
                string sourceFile = Path.Combine(sourcePath, fileName);
                string targetPath = thumbnailsDir + pathFromDate(sourceFile);
                string destFile = GenerateDestName(fileName, targetPath);
                CreateHiddenDirectory(targetPath);
                Image thumbnail = CreateThumbnail(sourceFile);
                thumbnail.Save(destFile);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
            return true;
        }

        /// <summary>
        /// Creates a Thumbnail Image out of an Image.
        /// </summary>
        /// <param name="sourcePath">The original Image path</param>
        /// <returns>
        /// An image with size proportional to the App.config ThumbailSize.
        /// </returns>
        private Image CreateThumbnail(string sourcePath)
        {
            using (FileStream fs = new FileStream(sourcePath, FileMode.Open, FileAccess.Read))
            using (Image targetImage = Image.FromStream(fs, false, false))
            {
                Size thumbnailSize = GetThumbnailSize(targetImage);
                return targetImage.GetThumbnailImage(thumbnailSize.Width, thumbnailSize.Height, null, IntPtr.Zero);
            }
        }

        /// <summary>
        /// Converts the original Image Size to the porportion defined in the App.config.
        /// </summary>
        /// <param name="original">Image to be resized</param>
        /// <returns>
        /// The proportional Size.
        /// </returns>
        private Size GetThumbnailSize(Image original)
        {
            // Width and height.
            int originalWidth = original.Width;
            int originalHeight = original.Height;
            // Compute best factor to scale entire image based on larger dimension.
            double factor;
            if (originalWidth > originalHeight)
            {
                factor = (double)thumbnailSize / originalWidth;
            }
            else
            {
                factor = (double)thumbnailSize / originalHeight;
            }
            // Return thumbnail size.
            return new Size((int)(originalWidth * factor), (int)(originalHeight * factor));
        }

        /// <summary>
        /// Converts the Date information to a string.
        /// </summary>
        /// <param name="sourceFile">Source file path</param>
        /// <returns>
        /// The date information as a string year/month/date
        /// </returns>
        private string pathFromDate(string sourceFile)
        {
            DateTime dt = GetImageDateTime(sourceFile);
            if (dt.Year == 1)
            {
                return @"\" + "UndefinedDate";
            }
            return @"\" + dt.Year + @"\" + dt.Month;
        }

        /// <summary>
        /// check if the file is locked or ready for use
        /// </summary>
        /// <param name="file"> the FileInfo to be checked </param>
        /// <returns>
        /// true if the file is unavailable (hasn't finished loading), false otherwise 
        /// <returns>
        private bool IsFileLocked(FileInfo file)
        {
            FileStream stream = null;
            try
            {
                stream = file.Open(FileMode.Open, FileAccess.Read, FileShare.None);
            }
            // file is locked
            catch (IOException)
            {
                return true;
            }
            finally
            {
                if (stream != null)
                    stream.Close();
            }
            //file is not locked
            return false;
        }
    }
}
