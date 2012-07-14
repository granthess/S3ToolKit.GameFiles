using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using S3ToolKit.GameFiles.Package;
using S3ToolKit.Utils.Logging;
using S3ToolKit.GameFiles.ResourceCFG;

namespace S3ToolKit.GameFiles.Library
{
    /// <summary>
    /// Provides a searchable, priority encoded, group of DBPF Package files
    /// </summary>
    public class GameLibrary : IDisposable
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.ToString());

        public ResourceIndex Index { get; private set; }
        private List<DBPFPackage> Packages;
        public List<string> PackageFilenames { get; private set; }

        public GameLibrary()
        {
            Index = new ResourceIndex();
            Packages = new List<DBPFPackage>();
            PackageFilenames = new List<string>();
        }

        public DBPFPackage Add(ResourceCFGEntry Entry)
        {
            return Add(Entry.PackageFileName, Entry.Priority);
        }

        public DBPFPackage Add(string FileName, int Prioirty = 0)
        {
            if (File.Exists(FileName))
            {
                try
                {
                    DBPFPackage Package = new DBPFPackage(FileName);
                    Add(Package, Prioirty);
                    return Package;
                }
                catch (InvalidDataException)
                {
                    return null;
                }
            }
            else return null;

        }

        public void Add(DBPFPackage Package, int Priority = 0)
        {
            Index.AddResources(Package.Resources, Priority);
            if (Package.FileName != null)
                PackageFilenames.Add(Package.FileName);
            Packages.Add(Package);
        }

        public bool ContainsPackage(string FileName)
        {
            return PackageFilenames.Contains(FileName);
        }

        #region Lifetime Control
        public void Close()
        {
            Dispose();
        }
        public void Clear()
        {
            Index.Close();
            foreach (DBPFPackage Package in Packages)
            {
                Package.Close();
            }
            Packages.Clear();
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
        ~GameLibrary()
        {
            // Simply call Dispose(false).
            Dispose(false);
        }

        #endregion

        public ResourceEntry GetResource(TGI_Key Key)
        {
            return Index.GetResource(Key);
        }
    }
}
