/*
    Copyright 2012, Grant Hess

    This file is part of S3ToolKit.GameFiles.

    S3ToolKit.Utils is free software: you can redistribute it and/or modify
    it under the terms of the GNU Lesser General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    Foobar is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU Lesser General Public License for more details.

    You should have received a copy of the GNU Lesser General Public License
    along with CC Magic.  If not, see <http://www.gnu.org/licenses/>.
*/
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
