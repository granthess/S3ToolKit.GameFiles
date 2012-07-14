using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace S3ToolKit.GameFiles.Resources
{
    public class ResourceBONE
    {
        public List<string> Names { get; private set; }


        public ResourceBONE()
        {
            Names = new List<string>();
        }

        public ResourceBONE(Stream Source)
            : this()
        {
            BinaryReader Reader = new BinaryReader(Source);
            
            UInt32 Version = Reader.ReadUInt32();
            UInt32 Count = Reader.ReadUInt32();


            for (int i = 0; i < Count; i++)
            {
                int len = Reader.ReadSByte();
                Names.Add(Encoding.BigEndianUnicode.GetString(Reader.ReadBytes(len)));
            }
        }
    }
}
