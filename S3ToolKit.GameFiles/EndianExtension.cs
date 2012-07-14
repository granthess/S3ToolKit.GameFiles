using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace S3ToolKit.GameFiles
{
    public static class EndianExtension
    {
        //
        //
        public static UInt16 Swap(this UInt16 inValue)
        {
            return (UInt16)(((inValue & 0xff00) >> 8) |
                             ((inValue & 0x00ff) << 8));
        }

        //
        //
        //public static UInt32 Swap(this UInt32 inValue)
        //{
        //    return (UInt32)(((inValue & 0xff000000) >> 24) |
        //                     ((inValue & 0x00ff0000) >> 8) |
        //                     ((inValue & 0x0000ff00) << 8) |
        //                     ((inValue & 0x000000ff) << 24));
        //}

        public static uint Swap(this uint inValue)
        {
            return (uint)(((inValue & 0xff000000) >> 24) |
                             ((inValue & 0x00ff0000) >> 8) |
                             ((inValue & 0x0000ff00) << 8) |
                             ((inValue & 0x000000ff) << 24));
        }
    }
}



