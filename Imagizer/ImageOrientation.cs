using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Imagizer {
    /// <summary>
    /// The two possible ways that an image can be oriented.
    /// </summary>
    public enum ImageOrientation : byte {
        Landscape = 0,
        Portrait = 1,
    }
}
