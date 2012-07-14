using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using S3ToolKit.Utils.Logging;

namespace S3ToolKit.GameFiles.ResourceCFG
{
    public class ResourceCFGEntry
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.ToString());

        #region Properties
        public int Priority { get; private set; }
        public string PackageFileName { get; private set; }
        #endregion

        public override string ToString()
        {
            return string.Format("[{0}] {1}", Priority, PackageFileName);
        }

        #region Lifetime
        public ResourceCFGEntry(int Priority, string FileName)
        {
            this.Priority = Priority;
            this.PackageFileName = FileName;
        }
        #endregion
    }
}
