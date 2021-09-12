using System;
using System.Collections.Generic;
using System.Text;

namespace System.X.Drawing
{
    [Flags]
    public enum ColorChannel
    {
        Alpha = 0x1,
        Red = 0x10,
        Green = 0x100,
        Blue = 0x1000
    }
}