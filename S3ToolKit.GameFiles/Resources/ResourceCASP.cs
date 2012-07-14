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
    public class ResourceCASP
    {
        #region Properties
        public string Name { get; private set; }
        public UInt32 ClothingType { get; private set; }        
        public UInt32 TypeFlags { get; private set; }        
        public UInt32 AgeGender { get; private set; }        
        public UInt32 Category { get; private set; }
        #endregion



        #region Constructor
        

        public ResourceCASP(string Filename)
        {
            Stream Source = File.OpenRead(Filename);
            try
            {
                Import(Source);
            }
            finally
            {
                Source.Close();
            }
        }
        
        public ResourceCASP(Stream Source)
        {
            Import(Source);
        }

        public ResourceCASP(byte[] buffer)
        {
            Stream MS = new MemoryStream(buffer, false);
            try
            {
                Import(MS);
            }
            finally
            {
                MS.Close();
            }
        }
        #endregion

        #region Helpers
        void Import(Stream Source)
        {     
            BinaryReader Reader = new BinaryReader(Source);

            UInt32 Version = Reader.ReadUInt32();
            Reader.ReadUInt32();

            UInt32 PresetCount = Reader.ReadUInt32();

            for (int i = 0; i < PresetCount; i++)
            {
                UInt32 StrLen = Reader.ReadUInt32();
                string Bob = Encoding.Unicode.GetString(Reader.ReadBytes((int)StrLen * 2));
                Reader.ReadUInt32();
            }

            UInt16 StrLen2 = Reader.ReadUInt16();
            Name = Encoding.Unicode.GetString(Reader.ReadBytes(StrLen2));

            Reader.ReadSingle();
            // Reader.ReadByte();
            ClothingType = Reader.ReadUInt32();
            TypeFlags = Reader.ReadUInt32();
            AgeGender = Reader.ReadUInt32();
            Category = Reader.ReadUInt32();

        }

        
        #endregion

        
    }
}
