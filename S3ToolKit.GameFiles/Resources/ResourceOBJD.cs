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
using System.IO;
using S3ToolKit.GameFiles.Package;

namespace S3ToolKit.GameFiles.Resources
{
    public class ResourceOBJD
    {
        #region Properties
        public string InstanceName { get; private set; }
        public string Name { get; private set; }
        public string Description { get; private set; }
        public Single Price { get; private set; }
        public TGI_Key ThumbKey { get; private set; }
        #endregion



        #region Constructor
        

        public ResourceOBJD(string Filename)
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
        
        public ResourceOBJD(Stream Source)
        {
            Import(Source);
        }

        public ResourceOBJD(byte[] buffer)
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
            BinaryReader Reader = new BinaryReader(Source,Encoding.BigEndianUnicode);

            UInt32 Version = Reader.ReadUInt32();
            Reader.ReadUInt32();
            Reader.ReadUInt32();
                       
            ReadMatBlock(Reader);

            if (Version >= 0x16)
            {
                InstanceName = Reader.ReadString();
            }

            ReadCommonBlock(Reader);
        }

        private void ReadMatBlock(BinaryReader Reader, bool IsWallFloor=false)
        {

            // read the Material List Stuff
            UInt32 MatCount = Reader.ReadUInt32();

            for (int i = 0; i < MatCount; i++)
            {
                byte MatType = Reader.ReadByte();
                if (MatType != 1)
                    Reader.ReadUInt32();

                UInt32 Offset = Reader.ReadUInt32();
                Reader.BaseStream.Seek(Offset, SeekOrigin.Current);
                Reader.ReadUInt32();
                if (IsWallFloor)
                {
                    Reader.ReadUInt32();
                    Reader.ReadUInt32();
                    Reader.ReadUInt32();
                }

            }

        }

        private void ReadCommonBlock(BinaryReader Reader)
        {
            UInt32 Version = Reader.ReadUInt32();
            Reader.ReadUInt64();
            Reader.ReadUInt64();

            Name = Reader.ReadString();
            Description = Reader.ReadString();

            Name = Name.Replace("CatalogObjects/Name:", "");
            Description = Description.Replace("CatalogObjects/Description:", "");

            Price = Reader.ReadSingle();
            Reader.ReadSingle();
            Reader.ReadSingle();
            Reader.ReadByte();

            //ThumbKey = new TGI_Key(Reader.ReadUInt32(), Reader.ReadUInt32(), Reader.ReadUInt64());

        }
        #endregion
    }
}
