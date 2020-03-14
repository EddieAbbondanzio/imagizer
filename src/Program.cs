using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace BulkImageResizer {
    /// <summary>
    /// Spaghetti! Yay! 4/15/2018
    /// </summary>
    public class Program {
        static void Main(string[] args) {
            BulkImageResizer bir = new BulkImageResizer(new ConsoleInputAdapter());
            bir.Run();
        }
    }
}
