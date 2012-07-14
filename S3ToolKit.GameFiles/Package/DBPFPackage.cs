using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using S3ToolKit.Utils.Logging;

namespace S3ToolKit.GameFiles.Package
{
    /// <summary>
    /// Encapsulates the information required for a Read Only DBPF/DBPP Package file
    /// </summary>
    /// <remarks>
    /// A DBPF File contains several structures that are housekeeping information and don't need
    /// to be kept in memory as they can be re-generated as needed.  These structures aren't 
    /// included in this class.
    /// </remarks>
    
    // Required information:
    // DBPF vs DBPP (Encrypted)
    // Index of Resources
    // -- Everything else is puffery and we want to minimize puffery.
    //
    // Since we don't read everything into memory, we need to keep access to the stream/file 
    // where we import from.  This is done with the private Stream DataStream
    public class DBPFPackage : IDisposable
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.ToString());

        #region Fields
        Stream DataStore;
        bool _IsEncrypted;        
        #endregion

        #region Properties
        public bool IsEncrypted { get { return _IsEncrypted; }}
        public List<ResourceEntry> Resources { get; private set; }
        public string FileName { get; private set; }
        public string GUID { get; set; }
        #endregion

        #region Constructor
        public DBPFPackage()
        {
            Resources = new List<ResourceEntry>();
        }

        public DBPFPackage(Stream SourceStream)
            : this()
        {
            Import(SourceStream);
        }

        public DBPFPackage(string FileName)
            : this()
        {
            Import(FileName);
        }

        #endregion

        #region Lifetime Control
        public void Close()
        {
            Dispose();
        }
        public void Clear()
        {
            if (DataStore != null)
            {
                DataStore.Close();
                DataStore = null;
            }

            foreach (var Resource in Resources)
            {
                Resource.Close();
            }
        }
        #endregion

        #region Export Methods
        public void Export(string FileName)
        {
            Stream OutStream = File.Create(FileName);
            try
            {
                Export(OutStream);
            }
            finally
            {
                OutStream.Close();
            }
        }

        public void Export(Stream OutStream)
        {
            DataStore.Seek(0, SeekOrigin.Begin);
            DataStore.CopyTo(OutStream);
        }

        #endregion 

        #region Import Methods
        public void Import(String FileName)
        {
            DataStore = File.Open(FileName, FileMode.Open, FileAccess.Read, FileShare.Read);
            try
            {
                this.FileName = FileName;
                Import();
            }
            catch (InvalidDataException e)
            {
                DataStore.Close();
                throw e;
            }
        }

        public void Import(Stream SourceStream)
        {
            this.FileName = "<Stream>";
            Import(SourceStream, true);
        }
        
        public void Import(Stream SourceStream, bool UseBuffer)
        {
            if (UseBuffer)
            {
                DataStore = new MemoryStream();
                SourceStream.CopyTo(DataStore);
            }
            else
            {
                DataStore = SourceStream;
            }
            Import();
        }

        private void Import()
        {
            BinaryReader Reader;
            byte[] buffer;            

            // Start by getting the Header :  0x60 bytes at the beginning of the file
            DataStore.Seek (0,SeekOrigin .Begin);

            buffer = new byte[0x60];
            DataStore.Read (buffer,0,0x60);

            
            // Check the Magic
            string Magic = Encoding.ASCII.GetString(buffer, 0, 4);

            if ((Magic != "DBPF") & (Magic != "DBPP"))
            {
                throw new InvalidDataException("Unknown Magic Number: " + Magic);
            }

            if (Magic == "DBPP")
            {
                _IsEncrypted = true;
                throw new InvalidDataException("DBPP Not Supported");
            }

            if (IsEncrypted)
            {
                // Decrypt the header
                for (int i = 0; i < 0x60; i++)
                {
                    buffer[i] ^= EncryptKey[i];
                }
            }

            Reader = new BinaryReader(new MemoryStream(buffer, false));
            Reader.BaseStream.Position = 4;
            
            // Next verify the Major, Minor Versions
            // Major, Minor == 2,0

            uint Major = Reader.ReadUInt32();
            uint Minor = Reader.ReadUInt32();
            uint UMajor = Reader.ReadUInt32();
            uint UMinor = Reader.ReadUInt32();
            uint Flags = Reader.ReadUInt32();
            uint CDate = Reader.ReadUInt32();
            uint MDate = Reader.ReadUInt32();
            uint IMajor = Reader.ReadUInt32();
            uint IndexCount = Reader.ReadUInt32();
            uint IOffset = Reader.ReadUInt32();
            uint IndexLength = Reader.ReadUInt32();
            uint HCount = Reader.ReadUInt32();
            uint HOffset = Reader.ReadUInt32();
            uint HLength = Reader.ReadUInt32();
            uint IMinor = Reader.ReadUInt32();
            uint IndexOffset = Reader.ReadUInt32();

            // Verify Header Values
            if ((Major != 2) | (Minor != 0))
            {
                log.Warn(string.Format("Unknown DBPF Version {0}.{1}, Expected 2.0 -- {2}", Major, Minor,this.FileName));
                throw new InvalidDataException(string.Format("Unknown File Version: {0}.{1}, Expected 2.0", Major, Minor));
            }

            if (((IMajor != 0) & (IMajor != 7)) | (IMinor != 3))
            {
                log.Warn(string.Format("Unknown Index Version {0}.{1}, Expected 0.3 -- {2}", IMajor, IMinor,this.FileName));
                //throw new InvalidDataException(string.Format("Unknown Index Version: {0}.{1}, Expected 0.3", IMajor, IMinor));
            }

            if ((UMajor != 0) | (UMinor != 0) | (Flags != 0) | (CDate != 0) | (MDate != 0) | (HCount != 0) | (HLength != 0))
            {
                log.Warn(string.Format("Unused Header Value not set to 0 -- {0}", this.FileName));
            //    throw new InvalidDataException("Unused header value not 0");
            }

            if ((IOffset != 0) & (IOffset != IndexOffset))
            {
                log.Warn(string.Format("Header Offset mismatch -- Not loadable by Game! -- {0}", this.FileName));
            }
            
            // Now we need to get a new buffer, BinaryReader, etc and read in the Index
            buffer = new byte[IndexLength];
            DataStore.Position = IndexOffset;

            DataStore.Read(buffer, 0, (int)IndexLength);
            Reader = new BinaryReader(new MemoryStream(buffer, false));

            // Vars to hold the info
            UInt32 Type =0;
            UInt32 Group=0;
            UInt64 Instance=0;

            TGI_Key Key;
            ResourceEntry Entry;

            UInt32 ILow = 0xffffffffu;
            UInt32 IHigh = 0xffffffffu;

            UInt32 ChunkOffset;
            UInt32 ChunkLength;
            UInt32 ResourceLength;
            UInt16 ResourceCompression;
            UInt16 ResourceFlag;

            // if we don't have any index entries, just exit
            if (IndexCount == 0)
            {
                log.Info("Empty Package -- No Index Entries");
                return;
            }

            // Now we get the index type
            uint IndexBits = Reader.ReadUInt32();

            // Test Package Index Type :::
            //
            int CommonCount = 0;
            if ((IndexBits & 0x0001) == 0x0001)
            {
                CommonCount += 4;
            }
            if ((IndexBits & 0x0002) == 0x0002)
            {
                CommonCount += 4;
            }
            if ((IndexBits & 0x0004) == 0x0004)
            {
                CommonCount += 4;
            }
            if ((IndexBits & 0x0008) == 0x0008)
            {
                CommonCount += 4;
            }

            

            int BlockLength = CommonCount + 4;
            BlockLength += (32 - CommonCount) * (int)IndexCount;

            if (BlockLength != IndexLength)
            {
                log.Warn(string.Format("DBPF Format Error, IndexType vs IndexSize mismatch: Actual Size {0}, Calculated Size {1}, IndexType {2} -- {3}", IndexLength, BlockLength,IndexBits,this.FileName));
                throw new InvalidDataException("DBPF Format Error, IndexType vs IndexSize mismatch");          
            }



            if ((IndexBits | 0x000f) != 0x000f )
            {
                log.Fatal("Import not implemented");
                throw new NotImplementedException();           
            }

            if ((IndexBits & 0x00000001) == 0x00000001)
            {
                Type = Reader.ReadUInt32();
            }

            if ((IndexBits & 0x00000002) == 0x00000002)
            {
                Group = Reader.ReadUInt32();
            }

            if ((IndexBits & 0x00000004) == 0x00000004)
            {
                ILow = Reader.ReadUInt32();
            }

            if ((IndexBits & 0x00000008) == 0x00000008)
            {
                IHigh = Reader.ReadUInt32();             


            }

            // now we cycle through each of the entries
            for (int i = 0; i < IndexCount; i++)
            {
                // Get the Variable Portion
                if ((IndexBits & 0x00000001) == 0x00000000)
                {
                    Type = Reader.ReadUInt32();
                }

                if ((IndexBits & 0x00000002) == 0x00000000)
                {
                    Group = Reader.ReadUInt32();
                }

                if ((IndexBits & 0x00000004) == 0x00000000)
                {
                    ILow = Reader.ReadUInt32();
                }
                if ((IndexBits & 0x000000008) == 0x00000000)
                {
                    IHigh = Reader.ReadUInt32();
                }

                Instance = (UInt64)IHigh | ((UInt64)ILow << 32);

                // Get the static part
                ChunkOffset = Reader.ReadUInt32();
                ChunkLength = Reader.ReadUInt32() & 0x7fffffff;
                ResourceLength = Reader.ReadUInt32();
                ResourceCompression = Reader.ReadUInt16();
                ResourceFlag = Reader.ReadUInt16();

                // Build the Key and the Entry
                Key = new TGI_Key(Type, Group, Instance);

                Entry = new ResourceEntry(DataStore, Key, (long)ChunkOffset, (int)ChunkLength, (int)ResourceLength, ResourceCompression == 0xffff, IsEncrypted);

                Resources.Add(Entry);
            }
        }
        
        #endregion

        #region Export Helper
        public void CopyTo(Stream OutputStream)
        {
            DataStore.Position = 0;
            DataStore.CopyTo(OutputStream);
        }

        public void CopyTo(string FileName)
        {
            Stream OutputStream = File.Open(FileName, FileMode.OpenOrCreate, FileAccess.Write);

            try
            {
                CopyTo(OutputStream);
            }
            finally
            {
                OutputStream.Close();
            }
        }
        #endregion

        #region Helpers
        public static readonly byte[] EncryptKey = new byte[] { 
            0x00, 0x00, 0x00, 0x16, 0xad, 0x20, 0xa4, 0x7a,     // DBPP, Major 
            0x2a, 0xee, 0xdc, 0xb7, 0x4d, 0x5e, 0xa9, 0x35,     // Minor, UMaj
            0x5b, 0x75, 0x26, 0x22, 0x33, 0xba, 0x60, 0x58,     // UMin, Flags
            0x99, 0xa2, 0xc2, 0x88, 0x70, 0x3b, 0x59, 0x05,     // CDate, MDate
            0x8a, 0xb4, 0xf7, 0x87, 0xea, 0xad, 0x05, 0x5a,     // IMaj, IndexCount
            0xaf, 0x8c, 0x3e, 0x18, 0x7f, 0xda, 0x1e, 0xd4,     // IOffset, ILength
            0xa2, 0x85, 0xac, 0x6c, 0x5f, 0x71, 0x64, 0xb0,     // HoleCount, HoleLoc
            0xcd, 0x03, 0x72, 0x50, 0x28, 0x58, 0xc3, 0x63,     // HoleLength, IMinor
            0x5c, 0x1f, 0x83, 0xbd, 0x74, 0xf6, 0x6b, 0xee,     // ILocation, fill
            0xbf, 0x54, 0x46, 0xa8, 0xdc, 0xec, 0xdf, 0x96,     // fill, fill
            0x24, 0xe1, 0xa2, 0x63, 0xc1, 0xfb, 0x1d, 0xff,
            0x6b, 0x05, 0xba, 0xbf, 0x40, 0x88, 0x5f, 0xfb
        };

        public ResourceEntry GetResource(TGI_Key Key)
        {
            var Result = from Item in Resources
                         where Item.Key == Key
                         select Item;

            foreach (ResourceEntry Entry in Result)
            {
                return Entry;  // get the first match only
            }

            return null;
        }

        #endregion

        #region Classification Helpers
        /// <summary>
        /// returns the first composition resource for the package
        /// </summary>
        /// <returns></returns>
        public TGI_Key GetCompositionResource()
        {
            var Keys = GetCompositionResources();

            TGI_Key Result = new TGI_Key(0,0,0);
            int Priority = -1;

            foreach (var Item in Keys)
            {
                if (Item.Value > Priority)
                {
                    Priority = Item.Value;
                    Result = Item.Key;
                }
            }

            return Result;
        }

        public Dictionary<TGI_Key, int> GetCompositionResources()
        {       
            Dictionary<TGI_Key, int> CompositionResources = new Dictionary<TGI_Key, int>();

            foreach (ResourceEntry Entry in Resources)
            {
                try
                {
                    if (
                        (Entry.Key.Type == 0x0668f628) | // World Description                    
                        (Entry.Key.Type == 0x04F88964) | // SIME -- SIM
                        (Entry.Key.Type == 0x062853a8)   // FAMD -- Family Data
                        )
                    {
                        CompositionResources.Add(Entry.Key, 750);
                    }
                    else if (
                        (Entry.Key.Type == 0xD063545b) | // LDES -- Lot
                        (Entry.Key.Type == 0x319e4f1d) | // OBJD -- Object                
                        (Entry.Key.Type == 0xD4D9FBE5) | // PTRN -- Pattern
                        (Entry.Key.Type == 0x034aeecb)   // CASP -- Clothing / Accessory                                        
                        )
                    {
                        CompositionResources.Add(Entry.Key, 500);
                    }
                    else if (
                        (Entry.Key.Type == 0x0333406c) | // xml
                        (Entry.Key.Type == 0x03b33ddf) | // xml Tuning
                        (Entry.Key.Type == 0x073faa07)   // S3SA -- Script / Core Mod
                        )
                    {
                        CompositionResources.Add(Entry.Key, 250);
                    }
                }
                catch (ArgumentException)
                {
                    // Just ignore any duplicates
                }
            }

            return CompositionResources;
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
                    Clear();
                }
                // Free your own state (unmanaged objects).
                // Set large fields to null.
                disposed = true;
            }
        }

        // Use C# destructor syntax for finalization code.
        ~DBPFPackage()
        {
            // Simply call Dispose(false).
            Dispose(false);
        }

        #endregion
    }
}
