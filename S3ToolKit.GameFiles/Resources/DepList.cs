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

using S3ToolKit.GameFiles;
using System.IO;
using S3ToolKit.GameFiles.Package;  // to include Endian Tools

namespace S3ToolKit.GameFiles.Resources
{
    /// <summary>
    /// Encapsulates a 0x7672f0c5 dependency list resource
    /// </summary>
    // need to be able to build this from either an existing resource or a TS3Pack Manifest
    //
    // Because their is no way to know the size of an entry, it must be parsed by the DepList and 
    // then a DepListEntry created.
    //
    public class DepList
    {
        private Dictionary<TS3GUID, DepListEntry> Entries;

        public DepList()
        {
            Entries = new Dictionary<TS3GUID, DepListEntry>();
        }

        public DepList(byte[] Source)
            : this()
        {
            MemoryStream SourceStream = new MemoryStream(Source, false);
            try
            {
                Import(SourceStream);
            }
            finally
            {
                SourceStream.Close();
            }
        }

        public DepList(Stream Source)
            : this()
        {
            Import(Source);
        }

        private void Import(Stream Source)
        {
            BinaryReader Reader = new BinaryReader(Source);
            // Read the DepList from the Stream and create the appropriate DepListEntries...

            // Get the Count
            UInt32 Count = Reader.ReadUInt32().Swap();

            DepListEntry Entry;

            for (int i = 0; i < Count; i++)
            {
                // Read the Signature

                byte[] sig = Reader.ReadBytes(5);

                // Next get DependencyCount and ResourceCount
                UInt32 DependencyCount = Reader.ReadUInt32().Swap();
                UInt32 ResourceCount = Reader.ReadUInt32().Swap();

                TS3GUID PackageID = new TS3GUID(Reader.ReadBytes(16));

                Entry = new DepListEntry(PackageID);

                // Now read the Dependencies
                for (int j = 0; j < DependencyCount; j++)
                {
                    Entry.AddDependency(new TS3GUID(Reader.ReadBytes(16)));
                }

                // Now read the Entry Type and Sub Type
                Entry.SetPackageType(Reader.ReadUInt32().Swap(), Reader.ReadUInt32().Swap());
                UInt16 unknown01 = Reader.ReadUInt16();


                UInt32 Type;
                UInt32 Group;
                UInt32 InstLo;
                UInt32 InstHi;
                UInt64 Inst;

                // Now read the Resource Keys
                for (int j = 0; j < ResourceCount; j++)
                {
                    Type = Reader.ReadUInt32().Swap();
                    Group = Reader.ReadUInt32().Swap();
                    InstLo = Reader.ReadUInt32().Swap();
                    InstHi = Reader.ReadUInt32().Swap();
                    Inst = (UInt64)(InstLo + ((UInt64)InstHi << 32));

                    TGI_Key Key1 = new TGI_Key(Type, Group, Inst);


                    Type = Reader.ReadUInt32().Swap();
                    Group = Reader.ReadUInt32().Swap();
                    InstLo = Reader.ReadUInt32().Swap();
                    InstHi = Reader.ReadUInt32().Swap();
                    Inst = (UInt64)(InstLo + ((UInt64)InstHi << 32));
                    TGI_Key Key2 = new TGI_Key(Type, Group, Inst);

                    Entry.AddResource(Key1, Key2);
                }

                // Next, read the String Length and the String
                int NameCount = (int)Reader.ReadUInt32().Swap();
                                
                Entry.Name = Encoding.BigEndianUnicode.GetString(Reader.ReadBytes(NameCount * 2));

                Reader.ReadBytes(4);  // skip the gap at the end of the String

                Type = Reader.ReadUInt32().Swap();
                Group = Reader.ReadUInt32().Swap();
                InstLo = Reader.ReadUInt32().Swap();
                InstHi = Reader.ReadUInt32().Swap();
                Inst = (UInt64)(InstLo + ((UInt64)InstHi << 32));
                Entry.Thumbnail = new TGI_Key(Type, Group, Inst);
                Console.WriteLine(Entry.Thumbnail);

                UInt32 ExtraLen;
                byte Flags = Reader.ReadByte();
                // Get the flag

                do
                {
                    
                    ExtraLen = Reader.ReadUInt32().Swap();

                    if (ExtraLen != 0)
                    {
                        Entry.AddExtraData (Encoding.ASCII.GetString(Reader.ReadBytes((int)ExtraLen)));
                    }
                }
                while (ExtraLen != 0);
                Entries.Add(Entry.PackageID, Entry);
            }

        }

        public bool ContainsID(TS3GUID PackageID)
        {
            return Entries.ContainsKey(PackageID);
        }
    }
}
