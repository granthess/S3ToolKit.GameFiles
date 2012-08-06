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
yusing System;
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
