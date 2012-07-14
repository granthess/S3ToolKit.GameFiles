using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using S3ToolKit.GameFiles.Package;
using S3ToolKit.GameFiles.Exportable;
using System.IO;


namespace S3ToolKit.GameFiles.Resources
{
    public class ResourceSIME
    {
        #region Public Values        
        public List<TGI_Key> TGITable { get; private set; }

        public int DefaultOutfitKey { get; private set; }
        public UInt32 SimFlags { get; private set; }

        public string FirstName { get; private set; }
        public string LastName { get; private set; }
        public string Biography { get; private set; }

        public UInt32 FavoriteMusic { get; private set; }
        public UInt32  FavoriteFood { get; private set; }
        public UInt32 FavoriteColor { get; private set; }
        public UInt32 ZodiacSign { get; private set; }

        public List<UInt64> Traits { get; private set; }
        public UInt32 LifeTimeWish { get; private set; }

        
        #endregion

        #region Lifetime Control
        public ResourceSIME(string Filename)
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
        
        public ResourceSIME(Stream Source)
        {
            Import(Source);
        }

        public ResourceSIME(byte[] buffer)
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
            Traits = new List<UInt64>();
            // Setup the exportable reader
            TGITable = new List<TGI_Key>();
            BinaryReader Reader = new BinaryReader(Source);

            UInt16 Version = Reader.ReadUInt16();
            UInt16 TGICount = Reader.ReadUInt16();

            for (int i = 0; i < TGICount; i++)
            {
                UInt64 Instance = Reader.ReadUInt64();
                UInt32 Group = Reader.ReadUInt32();
                UInt32 Type = Reader.ReadUInt32();
                TGI_Key temp = new TGI_Key(Type, Group, Instance);

                TGITable.Add(temp);
            }

            UInt16 Unknown1 = Reader.ReadUInt16();
            UInt32 ExportableSize = Reader.ReadUInt32();

            ExportableReader EReader = new ExportableReader(Reader.ReadBytes((int)ExportableSize));

            byte[] Unknown2 = Reader.ReadBytes(5);

            // now get the values
            SimFlags = EReader.ReadUint32(0x68cdf632);
            DefaultOutfitKey = EReader.ReadInt32(0xf93356ee);
            FirstName = EReader.ReadString(0xeb5173a0);
            LastName = EReader.ReadString(0x7047cb14);
            FavoriteColor = EReader.ReadUint32(0x90254f2f);
            FavoriteFood = EReader.ReadUint32(0x35f0b67e);
            FavoriteMusic = EReader.ReadUint32(0x1672cf33);
            try { ZodiacSign = EReader.ReadUint32(0xa84edf4); }
            catch (KeyNotFoundException) { ZodiacSign = 0xff; }

            LifeTimeWish = EReader.ReadUint32(0x84205551);

            UInt32 TraitCount;
            try
            {
                TraitCount = EReader.ReadUint32(0x5b6a3bbd);
                UInt64[] TraitTemp = EReader.ReadUInt64List(0x6979b0fb);

                foreach (UInt64 Trait in TraitTemp)
                {
                    Traits.Add(Trait);
                }
            }
            catch (KeyNotFoundException) { TraitCount = 0; }


            if (Biography == "")
            {
                Biography = "Test Bio";
            }
       
        }

        
        #endregion
    }
}




