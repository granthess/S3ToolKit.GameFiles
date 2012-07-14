using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using S3ToolKit.GameFiles.Package;

namespace S3ToolKit.GameFiles.Library
{
    /// <summary>
    /// Encapsulates a list of ResourceEntry 's in priority order
    /// </summary>
    public class ResourceList
    {
        public ResourceEntry Value { get { return GetValue(); } }
        public TGI_Key Key {get;private set;}
        private List<ResourceEntry> Duplicates;

        private Dictionary<int,ResourceEntry> List;

        private ResourceEntry GetValue()
        {
            var x = List.Keys.Max<int>();

            return List[x];            
        }

        public ResourceList(TGI_Key Key)
        {
            List = new Dictionary<int, ResourceEntry>();
            Duplicates = new List<ResourceEntry>();
            this.Key = Key;
        }

        public void AddResource(ResourceEntry Entry, int Priority)
        {
            if (Entry.Key != Key)
            {
                throw new ArgumentException();
            }

            if (List.ContainsKey(Priority))
            {
                Duplicates.Add(Entry);
            }
            else
            {
                List.Add(Priority, Entry);
            }
   
        }

    }
}
