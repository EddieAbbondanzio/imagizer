using System.IO;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

namespace BulkImageResizer {
    public class BulkImageResizer {
        private IInputAdapter inputAdapter;

        public BulkImageResizer(IInputAdapter inputAdapter) {
            this.inputAdapter = inputAdapter;
        }

        public void Run() {
            string directory = GetDirectory();
            ImageDimensions dimensions = GetDimensions();
            bool saveOriginals = GetSaveOriginals();

            ProcessDirectory(directory, dimensions, saveOriginals);
        }

        public string GetDirectory() {
            string directory = null;

            do {
                if (directory != null) {
                    inputAdapter.WriteLine("Directory not found. Please enter a valid directory:");
                } else {
                    inputAdapter.WriteLine("Please specify a directory: ");
                }

                directory = inputAdapter.ReadLine();
            } while (!Directory.Exists(directory));

            return directory;
        }

        public ImageDimensions GetDimensions() {
            while (true) {
                inputAdapter.WriteLine("Please enter desired dimensions ex \"400 200\": ");
                string input = inputAdapter.ReadLine();

                string[] splitInput = input.Split(" ");

                int width = 0, height = 0;

                if (int.TryParse(splitInput[0], out width) && int.TryParse(splitInput[1], out height)) {
                    return new ImageDimensions(width, height);
                } else {
                    inputAdapter.WriteLine("Error: Wrong format. Please enter dimensions in pixels seperated by a space.");
                }
            }
        }

        public bool GetSaveOriginals() {
            inputAdapter.WriteLine("Save originals? (y/n): ");
            string input = inputAdapter.ReadLine();

            // Assume user will want to save them by default. 
            return input != "n";
        }

        public void ProcessDirectory(string path, ImageDimensions dimensions, bool saveOriginals) {
            string[] fileNames = Directory.GetFiles(path);
            int count = 0;

            if (saveOriginals) {
                string rawDir = Path.Join(path, "original");

                //Check to see that we haven't already ran here before
                if (Directory.Exists(rawDir)) {
                    ClearFolder(rawDir);
                } else {
                    Directory.CreateDirectory(rawDir);
                }
            }

            foreach (string fileName in fileNames) {
                string extension = Path.GetExtension(fileName);

                //Skip non image files.
                if (extension != ".jpg" && extension != ".png") {
                    continue;
                }

                using (Image image = Image.Load(fileName)) {
                    //Move the original image to the sub directory
                    if (saveOriginals) {
                        string newFileName = Path.Join(Path.GetDirectoryName(fileName), "original", Path.GetFileName(fileName));
                        File.Move(fileName, newFileName);
                    }

                    image.Mutate(i => i.Resize(dimensions.Width, dimensions.Height));
                    image.Save(fileName);
                }

                count++;
                inputAdapter.WriteLine($"Processed {fileName}");
            }

            inputAdapter.WriteLine($"Done! Processed {count} image(s). Goodbye.");
        }

        private void ClearFolder(string FolderName) {
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