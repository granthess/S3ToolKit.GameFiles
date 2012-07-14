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
    along with Foobar.  If not, see <http://www.gnu.org/licenses/>.
*/
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
