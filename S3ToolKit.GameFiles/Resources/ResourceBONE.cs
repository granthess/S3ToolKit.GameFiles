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
    along with Foobar.  If not, see <http://www.gnu.org/licenses/>.
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace S3ToolKit.GameFiles.Resources
{
    public class ResourceBONE
    {
        public List<string> Names { get; private set; }


        public ResourceBONE()
        {
            Names = new List<string>();
        }

        public ResourceBONE(Stream Source)
            : this()
        {
            BinaryReader Reader = new BinaryReader(Source);
            
            UInt32 Version = Reader.ReadUInt32();
            UInt32 Count = Reader.ReadUInt32();


            for (int i = 0; i < Count; i++)
            {
                int len = Reader.ReadSByte();
                Names.Add(Encoding.BigEndianUnicode.GetString(Reader.ReadBytes(len)));
            }
        }
    }
}
