using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using S3ToolKit.Utils.Logging;
using S3ToolKit.GameFiles.Package;

namespace S3ToolKit.GameFiles.Library
{
    /// <summary>
    /// Provides a searchable priority enabled list of resources with a back-link to the DBPFPackage containing the resource
    /// and automatically allows adding multiple duplicate resources with priority management.
    /// </summary>
    public class ResourceIndex : IDisposable
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.ToString());

        public IList<ResourceEntry> Entries { get { return GetResourceList(); } }

        private Dictionary<TGI_Key, ResourceList> ResourceData;

        public ResourceIndex()
        {
            ResourceData = new Dictionary<TGI_Key, ResourceList>();
        }

        #region Helpers
        IList<ResourceEntry> GetResourceList()
        {
            List<ResourceEntry> TempList = new List<ResourceEntry>();

            foreach (ResourceList Entry in ResourceData.Values)
            {
                TempList.Add(Entry.Value);
            }

            return TempList;
        }
        #endregion

        #region Lifetime Control
        public void Close()
        {
            Dispose();
        }
        public void Clear()
        {
          
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
        ~ResourceIndex()
        {
            // Simply call Dispose(false).
            Dispose(false);
        }

        #endregion

        public void AddResources(List<ResourceEntry> Resources, int Priority)
        {            
            foreach (ResourceEntry Entry in Resources)
            {
                if (ResourceData.ContainsKey(Entry.Key))
                {
                    ResourceData[Entry.Key].AddResource(Entry, Priority);
                }
                else
                {
                    ResourceList List = new ResourceList(Entry.Key);
                    List.AddResource(Entry, Priority);
                    ResourceData.Add(Entry.Key, List);
                }
            }
        }

        public ResourceEntry GetResource(TGI_Key Key)
        {
            if (!ResourceData.ContainsKey (Key))
                throw new ArgumentException ();

            return ResourceData[Key].Value;
        }
    }
}
