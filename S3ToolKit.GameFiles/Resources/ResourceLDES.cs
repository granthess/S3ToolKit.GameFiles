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
using S3ToolKit.GameFiles.Package;
using System.IO;

namespace S3ToolKit.GameFiles.Resources
{
    public class ResourceLDES
    {
        #region Public Values        
        public string Name { get; private set; }
        public string LotName { get; private set; }
        public string LotDescription { get; private set; }
        public string LotAddress { get; private set; }
        public UInt32 LotWidth { get; private set; }
        public UInt32 LotHeight { get; private set; }

        public UInt32 LotType { get; private set; }
        public UInt32 LotSubType { get; private set; }

        public Single BeautifulVistaBuf { get; private set; }
        public UInt32 LotValue { get; private set; }
        
        #endregion

        #region Constructor
        public ResourceLDES(string Filename)
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
        
        public ResourceLDES(Stream Source)
        {
            Import(Source);
        }

        public ResourceLDES(byte[] buffer)
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

            UInt16 Version = Reader.ReadUInt16();  // expect 0x002b 

            //if (Version == 32)
            //{
            //    LotName = "Borked";
            //    return;
            //}
            
            TS3GUID NameGUID = new TS3GUID(Reader.ReadBytes(8));    // 0 in exported lots
            TS3GUID DescGUID = new TS3GUID(Reader.ReadBytes(8));    //
            TS3GUID LotNameGUID = new TS3GUID(Reader.ReadBytes(8)); // ---

            UInt32 Len = Reader.ReadUInt32();
            Name = Encoding.Unicode.GetString(Reader.ReadBytes((int)Len * 2));

            if (Version >= 32)
            {
                float Unknown1 = Reader.ReadSingle();
                float Unknown2 = Reader.ReadSingle();
            }
            float CornerX = Reader.ReadSingle();
            float CornerY = Reader.ReadSingle();
            float CornerZ = Reader.ReadSingle();
            float Heading = Reader.ReadSingle();

            LotWidth = Reader.ReadUInt32();
            LotHeight = Reader.ReadUInt32();

            UInt32 Unknown3 = Reader.ReadUInt32();
            UInt32 Unknown4 = Reader.ReadUInt32();
            float Unknown5 = Reader.ReadSingle();
            float Unknown6 = Reader.ReadSingle();
            UInt32 Unknown7 = Reader.ReadUInt32();
            UInt32 Unknown8 = Reader.ReadUInt32();

            Len = Reader.ReadUInt32();
            LotName = Encoding.Unicode.GetString(Reader.ReadBytes((int)Len * 2));
            Len = Reader.ReadUInt32();
            LotDescription = Encoding.Unicode.GetString(Reader.ReadBytes((int)Len * 2));
            Len = Reader.ReadUInt32();
            LotAddress = Encoding.Unicode.GetString(Reader.ReadBytes((int)Len * 2));

            LotType = Reader.ReadUInt32();
            if (Version >= 0x2b)
            {
                LotSubType = Reader.ReadUInt32();
            }
            BeautifulVistaBuf = Reader.ReadSingle();
            float LotValueModifier = Reader.ReadSingle();
            LotValue = Reader.ReadUInt32();

            if (Version <= 32)
                return;

            UInt32 Unknown9 = Reader.ReadUInt32();
            byte[] UnknownA = Reader.ReadBytes(5);
        }

        
        #endregion
    }
}
