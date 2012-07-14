using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using S3ToolKit.Utils.Logging;
using System.IO;
using S3ToolKit.GameFiles.Package;

namespace S3ToolKit.GameFiles.TS3Pack
{
    class Sims3PackBuilder  : IDisposable
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.ToString());

        #region Fields
        Stream DataStore;
        bool _IsEncrypted;
        #endregion

        #region Properties
        public bool IsEncrypted { get { return _IsEncrypted; }}
        public bool IsModified { get; private set; }
        public long PackageSize { get { return GetPackageSize(); } }
        public List<ResourceEntry> Resources { get; private set; }
        #endregion

        #region Constructor
        public Sims3PackBuilder(string FileName, bool IsEncrypted = false)        
        {
            Resources = new List<ResourceEntry>();

            this._IsEncrypted = IsEncrypted;
            DataStore = File.Open(FileName, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None);

            GenerateBlank();
        }


        public Sims3PackBuilder(Stream SourceStream, bool IsEncrypted)
        {
            Resources = new List<ResourceEntry>();

            this._IsEncrypted = IsEncrypted;
            DataStore = SourceStream;

            GenerateBlank();
        }

        #endregion

        #region Lifetime Control
        public void Close()
        {
            Dispose();
        }

        private void CloseState()
        {   
            // Be sure to finish housekeeping
            if (IsModified)
            {
                GenerateIndex();
            }

            foreach (var Resource in Resources)
            {
                Resource.Close();
            }

            Resources.Clear();

            DataStore.Close();
            DataStore = null;
        }

        #endregion

        #region Helper Methods
        private long GetPackageSize()
        {
            if (DataStore == null)
            {
                return 0;
            }
            else
            {
                return DataStore.Length;
            }
        }
        #endregion

        #region Build Methods
        public void AddResource(ResourceEntry Resource)
        {
            if (IsEncrypted != Resource.IsEncrypted)
                throw new InvalidDataException("Encryption State Mismatch");

            byte[] buffer = Resource.ReadRaw();

            long ChunkOffset = DataStore.Length;
            int ChunkLength = buffer.Length;
            int Length = (int)Resource.Length;

            DataStore.Position = DataStore.Length;
            DataStore.Write(buffer, 0, ChunkLength);
            DataStore.Flush();

            ResourceEntry Entry = new ResourceEntry(DataStore, Resource.Key, ChunkOffset, ChunkLength, Length, Resource.IsCompressed, Resource.IsEncrypted);
            Resources.Add(Entry);
            IsModified = true;
        }

        private void GenerateBlank()
        {
            // Generate a Blank Header for the file or stream

            byte[] Header = new byte[0x60];

            // Ensure blank
            for (int i = 0; i < 0x60; i++)
                Header[i] = 0x00;

            BinaryWriter Writer = new BinaryWriter(new MemoryStream(Header, 0, 0x60, true));

            Writer.Write(new char[] { 'D', 'B', 'P', 'F' });    // Magic
            Writer.Write((UInt32)2);

            Writer.BaseStream.Position = 0x3c;
            Writer.Write((UInt32)3);

            if (IsEncrypted)
            {
                for (int i = 0; i < 0x60; i++)
                {
                    Header[i] ^= DBPFPackage.EncryptKey[i];
                }
            }

            DataStore.Write(Header, 0, 0x60);
            DataStore.Flush();
            Writer.Close();
        }

        private void GenerateIndex()
        {
            // Create the Index
            UInt32 IndexOffset = (UInt32)DataStore.Length;
            DataStore.Position = DataStore.Length;
            BinaryWriter Writer = new BinaryWriter(DataStore);

            // We always use Type 0x0000 Indexes.  Space savings is TRIVIAL for others!!!!
            Writer.Write((UInt32)0);

            foreach (var Resource in Resources)
            {
                Resource.Export(Writer, 0);
            }

            UInt32 IndexLength = (UInt32)(DataStore.Position - IndexOffset);

            // Now Backpatch the header values

            Writer.BaseStream.Position = 0x24;
            if (IsEncrypted)
                Writer.Write((UInt32)(Resources.Count ^ 0x5a05adeau));
            else
                Writer.Write((UInt32)Resources.Count);

            Writer.BaseStream.Position = 0x2c;
            if (IsEncrypted)
                Writer.Write((UInt32)(IndexLength ^ 0xd41eda7fu));
            else
                Writer.Write((UInt32)IndexLength);

            Writer.BaseStream.Position = 0x40;
            if (IsEncrypted)
                Writer.Write((UInt32)(IndexOffset ^ 0xbd831f5cu));
            else
                Writer.Write((UInt32)IndexOffset);

            IsModified = false;

            Writer.Flush();
            Writer = null;
        }
        #endregion

        #region IDisposable
        private bool disposed = false;

        //Implement IDisposable.
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    // Free other state (managed objects).
                    CloseState();
                }
                // Free your own state (unmanaged objects).
                // Set large fields to null.
                disposed = true;
            }
        }

        // Use C# destructor syntax for finalization code.
        ~Sims3PackBuilder()
        {
            // Simply call Dispose(false).
            Dispose(false);
        }

        #endregion
    }
}
