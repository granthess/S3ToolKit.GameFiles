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
