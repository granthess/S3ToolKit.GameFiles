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
