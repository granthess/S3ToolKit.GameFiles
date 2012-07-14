using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using S3ToolKit.GameFiles;  // to include Endian Tools
using S3ToolKit.GameFiles.Package;

namespace S3ToolKit.GameFiles.Resources
{
    /// <summary>
    /// Encapsulates a single entry from a 0x7672F0C5 resource
    /// </summary>
    // Known Values:
    // Package ID 
    // Package Name
    // Thumbnail TGI_Key
    // Dependency List
    // Resource List
    public class DepListEntry
    {
        public TS3GUID PackageID { get; private set; }
        public UInt32 PackageType { get; private set; }
        public UInt32 PackageSubType { get; private set; }

        public List<TS3GUID> Dependencies { get; private set; }
        public List<TGI_Key> Resources { get; private set; }
        public List<string> ExtraData { get; private set; }
        
        
        public String Name { get; set; }
        public TGI_Key Thumbnail { get; set; }


        public DepListEntry(TS3GUID PackageID)
        {
            this.PackageID = PackageID;
            Dependencies = new List<TS3GUID>();
            Resources = new List<TGI_Key>();
            ExtraData = new List<string>();
        }

        public void SetPackageType(UInt32 Type, UInt32 SubType)
        {
            this.PackageType = Type;
            this.PackageSubType = SubType;

        }
        public void AddDependency(TS3GUID GUID)
        {
            Dependencies.Add(GUID);
        }

        public void AddResource(TGI_Key Key1, TGI_Key Key2)
        {
            //if (Key1 != Key2)
            //{
            //    Console.WriteLine("{0} <--> {1}", Key1, Key2);
            //}
            Resources.Add(Key1);
        }

        public void AddExtraData(string Value)
        {
            ExtraData.Add(Value);
        }
    }
}
