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
