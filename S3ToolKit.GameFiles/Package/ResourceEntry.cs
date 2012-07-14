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

namespace S3ToolKit.GameFiles.Package
{
    /// <summary>
    /// Encapsulates a Read Only Resource Entry in a DBPF Package
    /// </summary>
    // Stores the TGI Key for the resource
    // and a reference to the data in the package file (or memory) (or temp file)
    public class ResourceEntry : IDisposable
    {
        #region Fields
        long _ChunkOffset;
        int _ChunkLength;
        int _ResourceLength;
        Stream DataStore;
        bool _IsEncrypted;
        #endregion

        #region Properties
        public TGI_Key Key { get; private set; }
        public bool IsCompressed { get; private set; }
        public bool IsEncrypted { get { return _IsEncrypted; } }
        public long Length { get { return _ResourceLength; } }
        public int Priority { get; set; }  // used by GameLibrary code
        #endregion
        
        #region Constructor
        public ResourceEntry(Stream Datastore, TGI_Key Key, long ChunkOffset, int ChunkLength, int ResourceLength, bool IsCompressed, bool IsEncrypted)
        {
            this.DataStore = Datastore;
            this.Key = Key;
            _ChunkOffset = ChunkOffset;
            _ChunkLength = ChunkLength;
            _ResourceLength = ResourceLength;
            this.IsCompressed = IsCompressed;
            _IsEncrypted = IsEncrypted;
        }

        public ResourceEntry(Stream DataStore, TGI_Key Key)
        {
            this.DataStore = DataStore;
            this.Key = Key;
            _ChunkOffset = 0;
            _ChunkLength = (int)DataStore.Length;
            _ResourceLength = (int)DataStore.Length;
            this.IsCompressed = false;
            _IsEncrypted = false;
        }
        #endregion

        #region Lifetime Management
        public void Close()
        {
            Dispose();
        }
        public void Clear()
        {           
            throw new NotImplementedException();
        }
        #endregion

        #region Public Read Methods
        public byte[] Read()
        {
            //if (_IsEncrypted)
            //    throw new InvalidDataException("Cannot read Encrypted Resource");

            if (!IsCompressed)
            {
                return ReadRaw();
            }
            else
            {

                byte[] buffer = Compression.UncompressStream(new MemoryStream(ReadRaw(), false), _ChunkLength, _ResourceLength);
                if (buffer.Length != _ResourceLength)
                    throw new InvalidDataException("Decompression Failure");

                return buffer;
            }

        }

        public byte[] ReadRaw()
        {
            byte[] buffer = new byte[_ChunkLength];
            DataStore.Position = _ChunkOffset;

            if (DataStore.Read(buffer, 0, _ChunkLength) != _ChunkLength)
                throw new InvalidDataException("Unexpected End of Data");

            return buffer;            
        }

        public Stream GetStream(bool WantRaw = false)
        {
            if (WantRaw)
            {
                return new MemoryStream(ReadRaw(), false);
            }
            else
            {
                return new MemoryStream(Read(), false);
            }
        }

        #endregion

        #region Import Methods
        #endregion

        #region Export Methods
        public void Export(BinaryWriter Writer, UInt32 IndexType)
        {
            if (IndexType != 0)
                throw new ArgumentException();
            // Just Dump the information out
            // T G I _ Chunk Offset, Chunk Length, File Length, Comp+Flag
            Writer.Write((UInt32)Key.Type);
            Writer.Write((UInt32)Key.Group);
            UInt32 Lo = (UInt32)(Key.Instance & 0x00000000ffffffffu);
            UInt32 Hi = (UInt32)((Key.Instance & 0xffffffff00000000u) >> 32);
            Writer.Write((UInt32)Hi);
            Writer.Write((UInt32)Lo);
            Writer.Write((UInt32)_ChunkOffset);
            Writer.Write((UInt32)((UInt32)_ChunkLength | 0x80000000u));
            Writer.Write((UInt32)Length);
            if (IsCompressed)
                Writer.Write((UInt16)0xffffu);
            else
                Writer.Write((UInt16)0x0000u);
            Writer.Write((UInt16)0x0001);
        }
        #endregion

        #region Compression Support
        public bool Compress()
        {
            return ChangeCompression(true);
        }

        public bool Decompress()
        {
            return ChangeCompression(false);
        }

        public void ChangeStream(Stream NewStream)
        {
            DataStore = NewStream;
            IsCompressed = false;
            _ResourceLength = (int)DataStore.Length;
            _ChunkLength = (int)DataStore.Length;
            _ChunkOffset = 0;
        }

        private bool ChangeCompression(bool Compress)
        {
            // Don't do anything if it is already the mode we want...
            if (Compress == IsCompressed)
                return false;

            if (Compress)
            {
                byte[] Buffer = Read();

                byte[] NewBuffer = Compression.CompressStream(Buffer);

                DataStore = new MemoryStream(NewBuffer, false);
                IsCompressed = true;
                _ChunkOffset = 0;
                _ChunkLength = (int)DataStore.Length;
                _ResourceLength = (int)Buffer.Length;
                return true;
            }
            else
            {
                // uncompress the resource
                Stream temp = GetStream();

                DataStore = temp;
                IsCompressed = false;
                _ChunkOffset = 0;
                _ChunkLength = (int)DataStore.Length;
                _ResourceLength = (int)DataStore.Length;
                return true;
            }
        }
        #endregion

        #region Helpers
        public override string ToString()
        {
            if (Key != null)
                return Key.ToString();
            else return base.ToString();
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
                }
                // Free your own state (unmanaged objects).
                // Set large fields to null.
                disposed = true;
            }
        }

        // Use C# destructor syntax for finalization code.
        ~ResourceEntry()
        {
            // Simply call Dispose(false).
            Dispose(false);
        }

        #endregion

    }
}
