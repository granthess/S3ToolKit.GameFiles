using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using S3ToolKit.GameFiles.Package;

namespace S3ToolKit.GameFiles.Resources
{
    public class SimIndexEntry
    {
        public TS3GUID GUID { get; private set; }
        public TGI_Key SIME { get; private set; }
        public TGI_Key Thumbnail { get; private set; }
        public string Name { get; private set; }

        public SimIndexEntry(TS3GUID GUID, TGI_Key SIME)
        {
            this.GUID = GUID;
            this.SIME = SIME;
        }

        public override string ToString()
        {
            if (SIME != null)
                return SIME.ToString();
            else
                return base.ToString();
        }

        public void SetThumbnail (TGI_Key Thumbnail)
        {
            this.Thumbnail = Thumbnail;
        }

        public void SetName (string Name)
        {
            this.Name = Name;
        }
    }
}
