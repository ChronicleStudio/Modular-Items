using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Modular_Items.Enums
{
    public enum EnumComponentTypeFlag
    {
        Handle = 1,         // Starting point for Non-hafted tools, if secondary is set, this will be in another position used for alternate hand placement
                            // If tool is hafted and contains a handle, the handle position will be attached based on points dictated by the haft.

        Haft = 2,           // Starting point for Hafted weapons
        Head = 4,           // Attachment to the handle or haft. IE: Sword blade, Axe Head (with or without multiple functions, also includes pommels and butt spikes, etc...

        Guard = 8,          // Attached to the handle at the same place as the head or haft

        // Secondary Components
        Handle2 = 16,
        Haft2 = 32,
        Head2 = 64,
        Guard2 = 128,

        // Tertiery Components
        Handle3 = 256,
        Haft3 = 512,
        Head3 = 1024,
        Guard3 = 2048,

        // Custom Components
        Custom7 = 4096,
        Custom8 = 8192,
        Custom9 = 16384,
        Custom10 = 32768,
        Custom11 = 65536
    }
}
