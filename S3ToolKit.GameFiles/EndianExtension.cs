/*
    Copyright 2012, Grant Hess

    This file is part of S3ToolKit.GameFiles.

    S3ToolKit.Utils is free software: you can redistribute it and/or modify
    it under the terms of the GNU Lesser General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    Foobar is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU Lesser General Public License for more details.

    You should have received a copy of the GNU Lesser General Public License
    along with CC Magic.  If not, see <http://www.gnu.org/licenses/>.
*/
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



