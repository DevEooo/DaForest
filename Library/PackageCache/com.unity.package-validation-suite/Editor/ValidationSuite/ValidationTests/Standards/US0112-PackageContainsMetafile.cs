using System;
using System.IO;

namespace UnityEditor.PackageManager.ValidationSuite.ValidationTests.Standards
{
    internal class PackageContainsMetafileUS0112 : BaseStandardChecker
    {
        public override string StandardCode => "US-0112";
        public override StandardVersion Version => new StandardVersion(1, 0, 0);

        public void Check(string folder)
        {
            CheckMetaInFolderRecursively(folder);
        }

        bool ShouldIgnore(string name)
        {
            //Names starting with a "." are being ignored by AssetDB.
            //Names finishing with ".meta" are considered meta files in Editor Code.
            if (Path.GetFileName(name).StartsWith(".") || name.EndsWith(".meta"))
                return true;

            // Honor the Unity tilde skipping of import
            if (Path.GetDirectoryName(name).EndsWith("~") || name.EndsWith("~"))
                return true;

            // Ignore node_modules folder as it is created inside the tested directory when production dependencies exist
            if (Path.GetDirectoryName(name).EndsWith("node_modules") || name.Contains("node_modules"))
                return true;

            return false;
        }

        void CheckMeta(string toCheck)
        {
            if (ShouldIgnore(toCheck))
                return;

            if (System.IO.File.Exists(toCheck + ".meta"))
                return;

            AddError("Did not find meta file for " + toCheck);
        }

        void CheckMetaInFolderRecursively(string folder)
        {
            try
            {
                foreach (string file in LongPathUtils.Directory.GetFiles(folder))
                {
                    CheckMeta(file);
                }

                foreach (string dir in LongPathUtils.Directory.GetDirectories(folder))
                {
                    if (ShouldIgnore(dir))
                        continue;

                    CheckMeta(dir);
                    CheckMetaInFolderRecursively(dir);
                }
            }
            catch (Exception e)
            {
                AddError("Exception " + e.Message);
            }
        }
    }
}
