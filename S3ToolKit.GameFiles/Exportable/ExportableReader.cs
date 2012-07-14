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
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace S3ToolKit.GameFiles.Exportable
{
    public class ExportableReader
    {        
        private Dictionary<UInt32, UInt32> LookupTable;
        private BinaryReader Reader;

        public ExportableReader(byte[] Buffer)
        {
            //Stream temps = File.Create(@"C:\temp\xyz.SIMEdescription");
            //Stream tempm = new MemoryStream(Buffer, false);
            //tempm.CopyTo(temps);
            //tempm.Close();
            //temps.Close();

            LookupTable = new Dictionary<uint, uint>();
            Reader = new BinaryReader(new MemoryStream(Buffer, false));
            UInt16 Version = Reader.ReadUInt16();
            UInt32 Offset = Reader.ReadUInt32();

            Reader.BaseStream.Position = Offset;

            UInt16 KeyCount = Reader.ReadUInt16();

            for (int i = 0; i < KeyCount; i++)
            {
                LookupTable.Add(Reader.ReadUInt32(), Reader.ReadUInt32());
            }

        }

        public UInt64 ReadUint64(UInt32 Name)
        {
            UInt32 KeyID = Name ^ 0xee28814f;
            UInt32 Offset = LookupTable[KeyID];

            Reader.BaseStream.Position = Offset;
            return Reader.ReadUInt64();
        }

        public UInt32 ReadUint32(UInt32 Name)
        {
            UInt32 KeyID = Name ^ 0xf1288606;
            UInt32 Offset = LookupTable[KeyID];

            Reader.BaseStream.Position = Offset;
            return Reader.ReadUInt32();
        }

        public UInt16 ReadUint16(UInt32 Name)
        {
            UInt32 KeyID = Name ^ 0xf328896c;
            UInt32 Offset = LookupTable[KeyID];

            Reader.BaseStream.Position = Offset;
            return Reader.ReadUInt16();
        }

        public Int64 ReadInt64(UInt32 Name)
        {
            UInt32 KeyID = Name ^ 0x71568e6;
            UInt32 Offset = LookupTable[KeyID];

            Reader.BaseStream.Position = Offset;
            return Reader.ReadInt64();
        }

        public Int32 ReadInt32(UInt32 Name)
        {
            UInt32 KeyID = Name ^ 0x415642b;
            UInt32 Offset = LookupTable[KeyID];

            Reader.BaseStream.Position = Offset;
            return Reader.ReadInt32();
        }

        public Int16 ReadInt16(UInt32 Name)
        {
            UInt32 KeyID = Name ^ 0x21560c5;
            UInt32 Offset = LookupTable[KeyID];

            Reader.BaseStream.Position = Offset;
            return Reader.ReadInt16();
        }

        public Single ReadFloat(UInt32 Name)
        {
            UInt32 KeyID = Name ^ 0x4edcd7a9;
            UInt32 Offset = LookupTable[KeyID];

            Reader.BaseStream.Position = Offset;
            return Reader.ReadSingle();
        }

        public string ReadString(UInt32 Name)
        {
            UInt32 KeyID = Name ^ 0x15196597;
            UInt32 Offset = LookupTable[KeyID];

            Reader.BaseStream.Position = Offset;

            UInt32 StrLen = Reader.ReadUInt32();

            return Encoding.Unicode.GetString(Reader.ReadBytes((int)StrLen*2));
        }

        public Int32[] ReadInt32List(UInt32 Name)
        {
            UInt32 KeyID = Name ^ 0xa4744bf2;
            UInt32 Offset = LookupTable[KeyID];

            Reader.BaseStream.Position = Offset;
            UInt32 Count = Reader.ReadUInt32();

            Int32[] temp = new Int32[Count];

            for (int i = 0; i < Count; i++)
            {
                temp[i] = Reader.ReadInt32();
            }

            return temp;
        }

        public Int64[] ReadInt64List(UInt32 Name)
        {
            UInt32 KeyID = Name ^ 0xa4744bf2;
            UInt32 Expected = 0xf461bc16;

            UInt32 Dif = Expected ^ KeyID;

            UInt32 Offset = LookupTable[KeyID];

            Reader.BaseStream.Position = Offset;
            UInt32 Count = Reader.ReadUInt32();

            Int64[] temp = new Int64[Count];

            for (int i = 0; i < Count; i++)
            {
                temp[i] = Reader.ReadInt64();
            }

            return temp;
        }

        public UInt32[] ReadUInt32List(UInt32 Name)
        {
            UInt32 KeyID = Name ^ 0xa4744bf2;
            UInt32 Offset = LookupTable[KeyID];

            Reader.BaseStream.Position = Offset;
            UInt32 Count = Reader.ReadUInt32();

            UInt32[] temp = new UInt32[Count];

            for (int i = 0; i < Count; i++)
            {
                temp[i] = Reader.ReadUInt32();
            }

            return temp;
        }

        public UInt64[] ReadUInt64List(UInt32 Name)
        {
            UInt32 KeyID = Name ^ 0xbb744cbb;           
            UInt32 Offset = LookupTable[KeyID];

            Reader.BaseStream.Position = Offset;
            UInt32 Count = Reader.ReadUInt32();

            UInt64[] temp = new UInt64[Count];

            for (int i = 0; i < Count; i++)
            {
                temp[i] = Reader.ReadUInt64();
            }

            return temp;
        }
    }

}
