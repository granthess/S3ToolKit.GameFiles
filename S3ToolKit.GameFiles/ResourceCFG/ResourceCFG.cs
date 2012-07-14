using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using S3ToolKit.Utils.Logging;
using System.IO;

namespace S3ToolKit.GameFiles.ResourceCFG
{
    public class ResourceCFG
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.ToString());


        #region fields
        #endregion

        #region Properties
        public string FileName { get; private set; }
        public List<ResourceCFGEntry> Entries { get; private set; }
        #endregion

        public override string ToString()
        {
            return string.Format("{0} entries in {1}", Entries.Count, FileName);
        }

        public ResourceCFG(string FileName)
        {
            this.FileName = FileName;
            Entries = new List<ResourceCFGEntry>();
            Parse();
        }

        private void ExpandAndAdd(int Priority, string VarPart)
        {
            string temp = VarPart.Replace('/', '\\');
            if (temp.Contains ("\\*\\"))
            {

            }
            
            string left = temp.Substring(0,temp.IndexOf("\\"));
            if (left[1] == ':')
                left += "\\";
            string right = temp.Substring(temp.IndexOf("\\")+1);

            ExpandAndAdd(Priority, left, right);
        }

        private void ExpandAndAdd(int Priority, string FixedPart, string VarPart)
        {
            if (VarPart.Contains("\\"))
            {
                string left = VarPart.Substring(0, VarPart.IndexOf("\\"));
                string right = VarPart.Substring(VarPart.IndexOf("\\") + 1);


                var DirList = Directory.GetDirectories(FixedPart, left);

                foreach (string DirName in DirList)
                {
                    ExpandAndAdd(Priority, DirName, right);
                }

            }
            else
            {
                var FileList = Directory.GetFiles(FixedPart, VarPart);

                foreach (string FileName in FileList)
                {
                    Entries.Add(new ResourceCFGEntry(Priority, FileName));
                }
            }
     
   
        }

        private void Parse()
        {
            if (!File.Exists(FileName))
            {
                return;
            }
            List<string> Lines = new List<string> (File.ReadLines(FileName));  // isn't this a cool new feature of .net 4.0 :)

            int Priority = 0;
            
            foreach (string Line in Lines)
            {             
                if (Line.ToLower().StartsWith("priority") )
                {
                    string value = Line.Substring(9).Trim();
                    try
                    {
                        Priority = int.Parse(value);
                    }
                    catch (FormatException)
                    {

                    }
                }
                else if (Line.ToLower().StartsWith("packedfile"))
                {
                    string name = Path.Combine(Path.GetDirectoryName(FileName), Line.Substring(11).Trim());
                    if (name.Contains("*"))
                    {
                        ExpandAndAdd(Priority, name);
                    }
                    else
                    {
                        Entries.Add(new ResourceCFGEntry(Priority, name));
                    }
                }
                else if (Line.ToLower().StartsWith("filetype"))
                {
                }
                else if (Line.ToLower().StartsWith("directoryfiles"))
                {
                }
                else if (Line.ToLower().StartsWith("group"))
                {
                }
                else if (Line.ToLower().StartsWith("scan"))
                {
                }
                else
                {
                }

            }
        }
    }
}
