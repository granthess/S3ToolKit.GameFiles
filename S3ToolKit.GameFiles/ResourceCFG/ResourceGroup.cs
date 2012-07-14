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
