using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace Imagizer {
    /// <summary>
    /// Spaghetti! Yay! 4/15/2018
    /// </summary>
    public class Program {
        static void Main(string[] args) {
            //Ask user for the directory
            string imgDirectory = GetImageDirectory();

            //Get dat input
            int[] portSize = GetDesiredSize(ImageOrientation.Portrait);
            int[] landSize = GetDesiredSize(ImageOrientation.Landscape);

            bool saveOrig = AskIfSaveOriginals();

            int imageCount = ResizeImagesInDirectory(imgDirectory, portSize, landSize, saveOrig);
            Console.WriteLine(string.Format("Resized {0} Images...", imageCount));
            Console.ReadLine();
        }

        /// <summary>
        /// Obtains the image directory of the images
        /// that we will be resizing.
        /// </summary>
        private static string GetImageDirectory() {
            string directory = null;

            do {
                if (directory != null) {
                    Console.WriteLine("Error! Directory does not exist!");
                }

                Console.Write("Image directory: ");
                directory = Console.ReadLine();
            }
            while (!Directory.Exists(directory));

            return directory;
        }

        /// <summary>
        /// Obtains the desired resolution for the specific
        /// image orientation.
        /// </summary>
        /// <param name="orientation"></param>
        /// <returns>W is in position 0, H is in position 1</returns>
        private static int[] GetDesiredSize(ImageOrientation orientation) {
            int[] pSize = new int[2];

            do {
                Console.WriteLine("Enter " + orientation.ToString() + " resolution 'WxH': ");
                string input = Console.ReadLine();

                if (input != null) {
                    string[] splitInput = input.Split('x');

                    if (splitInput.Count() == 2) {
                        int width = 0;
                        int height = 0;

                        int.TryParse(splitInput[0], out width);
                        int.TryParse(splitInput[1], out height);

                        if(width <= 0 || height <= 0) {
                            Console.WriteLine("Error! Invalid dimensions recieved!");
                        }
                        else {
                            pSize[0] = width;
                            pSize[1] = height;
                        }
                    }
                    else { 
                        Console.WriteLine("Error! Incorrect input recieved!");
                    }
                }
                else {
                    Console.WriteLine("Error! No input recieved!");
                }

            }
            while (pSize[0] <= 0 && pSize[1] <= 0);

            return pSize;
        }

        /// <summary>
        /// Find out if the user wants to save original files.
        /// </summary>
        /// <returns></returns>
        private static bool AskIfSaveOriginals() {
            while (true) {
                Console.Write("Save original images? (y/n) : ");
                string input = Console.ReadLine();

                if(input == "y") {
                    return true;
                }
                else if(input == "n") {
                    return false;
                }
                else {
                    Console.WriteLine("Error! Invalid input recieved!");
                }
            }
        }

        /// <summary>
        /// Resizes every image in the desired directory according to the two image sizes.
        /// </summary>
        /// <param name="dir">The path of the image directory</param>
        /// <param name="portraitSize">The resolution to change portrait mode images to</param>
        /// <param name="landscapeSize">The resolution to change landscape mode images to</param>
        /// <returns>The number of images resized</returns>
        private static int ResizeImagesInDirectory(string dir, int[] portraitSize, int[] landscapeSize, bool saveOrig) {
            string[] fileNames = Directory.GetFiles(dir);
            int count = 0;


            if (saveOrig) {
                string rawDir = dir + "\\raw";

                //Check to see that we haven't already ran here before
                if(Directory.Exists(rawDir)) {
                    ClearFolder(rawDir);
                }
                else {
                    Directory.CreateDirectory(rawDir);
                }
            }

            foreach (string fileName in fileNames) {
                string extension = Path.GetExtension(fileName);

                //Skip non image files.
                if(extension != ".jpg" && extension != ".png") {
                    continue;
                }

                Image newImg = null;
                using (Image currImg = Image.FromFile(fileName)) {
                    ImageOrientation currOrientation = currImg.Width > currImg.Height ? ImageOrientation.Landscape : ImageOrientation.Portrait;

                    switch (currOrientation) {
                        case ImageOrientation.Portrait:
                            newImg = ResizeImage(currImg, portraitSize[0], portraitSize[1]) as Image;
                            break;

                        case ImageOrientation.Landscape:
                            newImg = ResizeImage(currImg, landscapeSize[0], landscapeSize[1]) as Image;
                            break;
                    }
                }

                //Move the image to the sub directory
                if (saveOrig) {
                    string newFileName = Path.GetDirectoryName(fileName) + "\\raw\\" + Path.GetFileName(fileName);
                    File.Move(fileName, newFileName);
                }

                //Save the resized one
                newImg.Save(fileName, newImg.RawFormat);
                count++;
            }

            return count;
        }

        /// <summary>
        /// Resize the image to the specified width and height.
        /// </summary>
        /// <param name="image">The image to resize.</param>
        /// <param name="width">The width to resize to.</param>
        /// <param name="height">The height to resize to.</param>
        /// <returns>The resized image.</returns>
        private static Bitmap ResizeImage(Image image, int width, int height) {
            var destRect = new Rectangle(0, 0, width, height);
            var destImage = new Bitmap(width, height);

            destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (var graphics = Graphics.FromImage(destImage)) {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                using (var wrapMode = new ImageAttributes()) {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }

            return destImage;
        }

        private static void ClearFolder(string FolderName) {
            DirectoryInfo dir = new DirectoryInfo(FolderName);

            foreach (FileInfo fi in dir.GetFiles()) {
                fi.Delete();
            }

            foreach (DirectoryInfo di in dir.GetDirectories()) {
                ClearFolder(di.FullName);
                di.Delete();
            }
        }
    }
}
