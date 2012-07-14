using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using S3ToolKit.GameFiles.Package;

namespace S3ToolKit.GameFiles.Resources
{
    public class SimIndex
    {
        public List<SimIndexEntry> Entries { get; private set; }
        private List<TGI_Key> Keys;

        public SimIndex()
        {
            Entries = new List<SimIndexEntry>();
            Keys = new List<TGI_Key>();
        }

        public SimIndex(byte[] Source)
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

        public SimIndex(Stream Source)
            : this()
        {
            Import(Source);
        }

        private void Import(Stream Source)
        {
            BinaryReader Reader = new BinaryReader(Source);

            UInt16 version = Reader.ReadUInt16();
            UInt32 Count = Reader.ReadUInt32();

            for (int i = 0; i < Count; i++)
            {
                TGI_Key SimKey = ReadKey(Reader);
                TS3GUID GUID = ReadGUID(Reader); // 0x 6108b9b753ddff83 472e29bece2fd7ac
                Reader.ReadUInt32();  
                TGI_Key Thumbnail = ReadKey(Reader);
                UInt32 NameLen = Reader.ReadUInt32();
                string Name = Encoding.Unicode.GetString (Reader.ReadBytes((int)NameLen*2));
                SimIndexEntry Entry = new SimIndexEntry(GUID, SimKey);
                Entry.SetThumbnail(Thumbnail);
                Entry.SetName(Name);

                Entries.Add(Entry);
                Keys.Add(Entry.SIME);
            }
        }

        public void Export(Stream Destination)
        {
            BinaryWriter Writer = new BinaryWriter(Destination);

            Writer.Write((UInt16)0x0003);
            Writer.Write((UInt32)Entries.Count);

            foreach (SimIndexEntry Entry in Entries)
            {
                WriteKey(Entry.SIME, Writer);
                WriteGUID(Entry.GUID, Writer);

                Writer.Write((UInt32)0x0003);

                WriteKey(Entry.Thumbnail, Writer);

                Writer.Write((UInt32)Entry.Name.Length);
                Writer.Write(Encoding.Unicode.GetBytes(Entry.Name));
            }
        }

        private TGI_Key ReadKey(BinaryReader Reader)
        {
            UInt32 Type = Reader.ReadUInt32();
            UInt32 Group = Reader.ReadUInt32();
            UInt64 Instance = Reader.ReadUInt64();

            return new TGI_Key(Type, Group, Instance);
        }

        private void WriteKey(TGI_Key Key, BinaryWriter Writer)
        {
            Writer.Write(Key.Type);
            Writer.Write(Key.Group);
            Writer.Write(Key.Instance);
        }

        private TS3GUID ReadGUID(BinaryReader Reader)
        {
            byte[] buf1 = Reader.ReadBytes(16);
            byte[] buffer = new byte[16];

            for (int i = 7; i >= 0; i--)
            {
                buffer[7 - i] = buf1[i];
                buffer[15 - i] = buf1[i + 8];
            }

            return new TS3GUID(buffer); 
        }

        private void WriteGUID(TS3GUID GUID, BinaryWriter Writer)
        {
            byte[] buf1  = GUID.Value;
            byte[] buffer = new byte[16];

            for (int i = 7; i >= 0; i--)
            {
                buffer[7 - i] = buf1[i];
                buffer[15 - i] = buf1[i + 8];
            }

            Writer.Write(buf1);
        }

        public bool ContainsKey(TGI_Key Key)
        {
            return Keys.Contains(Key);

            //foreach (SimIndexEntry Entry in Entries)
            //{
            //    if (Entry.SIME == Key)
            //    {
            //        return true;
            //    }
            //}
            //return false;
        }
    }
}
