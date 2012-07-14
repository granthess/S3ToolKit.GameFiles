using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using S3ToolKit.GameFiles;
using S3ToolKit.GameFiles.Package;
using System.Dynamic;

namespace S3ToolKit.GameFiles.TS3Pack
{
    /// <summary>
    /// Encapsulates a TS3Pack (a.k.a. Sims3Pack) file
    /// </summary>
    // Just like the Package class, this should contain ***NO PUFFERY***
    // Need the XML Manifest and a list of contained items
    // Can contain multiple PNG files and multiple packages (DBPF or DBPP)
    public class Sims3Pack : IDisposable
    {
        #region Fields
        private Stream DataStore;
        #endregion

        #region Properties
        public List<DBPFPackage> Packages { get; private set; }
        public Stream Thumbnail { get; private set; }
        public List<Stream> Thumbnails { get; private set; }
        public XmlDocument Manifest { get; private set; }
        public bool IsCorrupt { get; private set; }
        public bool IsEncrypted { get; private set; }
        public string Type { get; private set; }
        public string SubType { get; private set; }
        #endregion 
        
        #region Constructor
        public Sims3Pack ()
        {
            Packages = new List<DBPFPackage>();
            Thumbnails = new List<Stream>();
        }

        public Sims3Pack(Stream SourceStream)
            : this()
        {
            DataStore = SourceStream;
            Import();
        }

        public Sims3Pack(string FileName)
            : this()
        {
            IsEncrypted = false;
            DataStore = File.Open(FileName, FileMode.Open, FileAccess.Read,FileShare.Read);
            try
            {
                Import();
            }
            catch (InvalidDataException e)
            {
                DataStore.Close();
                if (e.Message == "DBPP Not Supported") IsEncrypted = true;
                //throw e;
            }
            catch (XmlException)
            {
                DataStore.Close();
                IsCorrupt = true;
                // throw e;
            }
            catch (EndOfStreamException)
            {
                DataStore.Close();
                IsCorrupt = true;
                //throw e;
            }
        }
        #endregion

        #region Import Methods
        private void Import()
        {
            BinaryReader Reader = new BinaryReader(DataStore);

            // Start by reading the magic 
            int MagicLength = Reader.ReadInt32();

            string Magic = new string(Reader.ReadChars(MagicLength));

            if (Magic != "TS3Pack")
                throw new InvalidDataException(string.Format("Invalid Magic: Expected [TS3Pack], Found [{0}]", Magic));

            UInt16 Version = Reader.ReadUInt16();
            Int32 XMLLength = Reader.ReadInt32();

            Manifest = new XmlDocument();

            string Temp = Encoding.UTF8.GetString(Reader.ReadBytes(XMLLength));
            // string Temp = new string(Reader.ReadChars(XMLLength));

            MemoryStream test = new MemoryStream(Encoding.UTF8.GetBytes(Temp),false);

            //Manifest.LoadXml(Temp);
            Manifest.Load(test);

            test.Close();
            
            long BaseOffset = DataStore.Position;

            // Now parse out the XML
            XmlNode Decl = Manifest.FirstChild;
            XmlElement Sims3Package = (XmlElement) Decl.NextSibling;

            dynamic Base = new ExpandoObject();

            Base.Type = Sims3Package.GetAttribute("Type");
            Base.SubType = Sims3Package.GetAttribute("SubType");

            Type = Base.Type;
            SubType = Base.SubType;

            Base.Localized = new Dictionary<string, dynamic>();
            Base.Packages = new List<dynamic>();

            XmlElement Node = Sims3Package.FirstChild as XmlElement;
            while (Node != null)
            {
                if (Node.Name == "LocalizedNames")
                {
                    XmlElement SubNode = Node.FirstChild as XmlElement;

                    while (SubNode != null)
                    {
                        string Locale = SubNode.GetAttribute("Language");
                        if (!Base.Localized.ContainsKey(Locale))
                        {
                            Base.Localized.Add(Locale, new ExpandoObject());
                        }

                        Base.Localized[Locale].Name = SubNode.InnerText;

                        SubNode = SubNode.NextSibling as XmlElement;
                    }
                }
                else if (Node.Name == "LocalizedDescriptions")
                {
                    XmlElement SubNode = Node.FirstChild as XmlElement;

                    while (SubNode != null)
                    {
                        string Locale = SubNode.GetAttribute("Language");
                        if (!Base.Localized.ContainsKey(Locale))
                        {
                            Base.Localized.Add(Locale, new ExpandoObject());
                        }

                        Base.Localized[Locale].Description = SubNode.InnerText;

                        SubNode = SubNode.NextSibling as XmlElement;
                    }
                }
                else if (Node.Name == "PackagedFile")
                {
                    XmlElement SubNode = Node.FirstChild as XmlElement;
                    dynamic package = new ExpandoObject();
                    while (SubNode != null)
                    {
                        if (SubNode.Name == "Length")
                        {
                            package.Length = long.Parse(SubNode.InnerText);
                        }
                        else if (SubNode.Name == "Offset")
                        {
                            package.Offset = long.Parse(SubNode.InnerText);
                        }
                        else if (SubNode.Name == "Name")
                        {
                            package.Name = SubNode.InnerText;
                            if ((package.Name as string).EndsWith(".package"))
                            {
                                package.IsPackage = true;
                            }
                            else
                            {
                                package.IsPackage = false;
                            }
                        }
                        else
                        {
                            (package as IDictionary<string, Object>).Add(SubNode.Name, SubNode.InnerText);
                        }

                        SubNode = SubNode.NextSibling as XmlElement;
        
                    }
                    Base.Packages.Add(package);
                }
                else
                {
                    // Add the node to the Base object dynamically.   !! HOW COOL IS THAT !! :)
                    (Base as IDictionary<string, Object>).Add(Node.Name, Node.InnerText);
                }


                Node = Node.NextSibling as XmlElement;
            }


            // Now we need to generate the actual DBPFPackage entries for these files
            IsCorrupt = false;
            DBPFPackage Pack = null;
            foreach (var entry in Base.Packages)
            {
                if (entry.IsPackage)
                {
                    try
                    {
                        Pack = new DBPFPackage();
                        Pack.Import(new SubStream(DataStore, BaseOffset + entry.Offset, entry.Length), false);
                        Pack.GUID = Path.GetFileNameWithoutExtension(entry.Name);
                        if (BaseOffset + entry.Offset + entry.Length > DataStore.Length)
                        {
                            IsCorrupt = true; 
                        }
                    }
                    catch (InvalidDataException)
                    {
                        Pack = null;
                        IsEncrypted = true;
                        // what's up here?
                    }
                    if (Pack != null)
                    {                        
                        Packages.Add(Pack);
                    }
                }
                else
                {
                    Thumbnails.Add( new SubStream(DataStore, BaseOffset + entry.Offset, entry.Length));
                }
            }
        }
        #endregion

        #region Lifetime Control
        public void Close()
        {
            Dispose();
        }

        public void Clear()
        {
            foreach (DBPFPackage Package in Packages)
            {
                Package.Close();
            }

            Packages.Clear();

            DataStore.Close();
            DataStore = null;
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
        ~Sims3Pack()
        {
            // Simply call Dispose(false).
            Dispose(false);
        }

        #endregion

    }
}
