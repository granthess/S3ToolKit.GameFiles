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
using S3ToolKit.Utils.Registry;
using S3ToolKit.GameFiles.ResourceCFG;
using System.IO;

namespace S3ToolKit.GameFiles.Library
{
    public static class LibraryBuilder
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.ToString());

        public static GameLibrary GetLibrary(bool BasegameOnly = false)
        {
            log.Debug("GetLibrary");

            ResourceGroup Group = new ResourceGroup();


            foreach (var Entry in InstallationInfo.Instance.Packs)
            {                
                if (Entry.IsGame)
                {
                    if ((Entry.IsBaseGame) | (!BasegameOnly))
                    {
                        log.Debug(string.Format("Added Game/Pack {0}", Entry.DisplayName));
                        Group.AddResourceCFG(Path.Combine(Entry.InstallDir, "game", "bin", "resource.cfg"));
                        Group.AddResourceCFG(Path.Combine(Entry.InstallDir, "gamedata", "win32", "resource.cfg"));
                        Group.AddResourceCFG(Path.Combine(Entry.InstallDir, "gamedata", "shared", "resource.cfg"));
                    }
                }
            }

           GameLibrary Library = new GameLibrary();

            foreach (var Entry in Group.Entries)
            {
                Library.Add(Entry);
            }


            Library.Add(Path.Combine(InstallationInfo.Instance.BaseGame.InstallDir, "Gameplay", "GameplayData.package"));

            log.Debug(string.Format("Added {0} Packages containing {1} resources", Library.PackageFilenames.Count, Library.Index.Entries.Count));

            return Library;
        }
    }
}
