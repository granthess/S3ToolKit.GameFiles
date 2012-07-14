using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using S3ToolKit.Utils.Logging;

namespace S3ToolKit.GameFiles.ResourceCFG
{
    public class ResourceGroup
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.ToString());

        #region Properties
        public List<ResourceCFG> ConfigFiles;
        public List<ResourceCFGEntry> Entries;
        #endregion


        #region Lifetime
        public ResourceGroup()
        {
            log.Debug(".ctor");

            ConfigFiles = new List<ResourceCFG>();
            Entries = new List<ResourceCFGEntry>();
        }

        #endregion

        public void AddResourceCFG(ResourceCFG ResCFG)
        {
            ConfigFiles.Add(ResCFG);

            // now merge in the packages to the entries
            foreach (ResourceCFGEntry Entry in ResCFG.Entries)
            {
                Entries.Add(Entry);
            }
        }

        public ResourceCFG AddResourceCFG(string FileName)
        {
            ResourceCFG ResCFG = new ResourceCFG(FileName);
            AddResourceCFG(ResCFG);
            return ResCFG;
        }
    }
}
