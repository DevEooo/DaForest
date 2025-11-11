using System.IO;

namespace UnityEditor.PackageManager.ValidationSuite
{
    internal static class LongPathUtils
    {
        // https://docs.microsoft.com/en-us/windows/win32/fileio/maximum-file-path-limitation
        const string k_LongPathPrefix = @"\\?\";

        static string AddLongPathPrefix(string path)
        {
            if (!path.StartsWith(k_LongPathPrefix) && Path.IsPathRooted(path))
            {
                return k_LongPathPrefix + Path.GetFullPath(path);
            }

            return path;
        }

        static string RemoveLongPathPrefix(string path)
        {
            if (path.StartsWith(k_LongPathPrefix))
            {
                return path.Substring(k_LongPathPrefix.Length);
            }

            return path;
        }

        static void RemoveLongPathPrefix(string[] paths)
        {
            for (var i = 0; i < paths.Length; i++)
            {
                paths[i] = RemoveLongPathPrefix(paths[i]);
            }
        }

        public static class Directory
        {
            public static bool Exists(string path)
            {
#if UNITY_EDITOR_WIN
                return System.IO.Directory.Exists(AddLongPathPrefix(path));
#else
                return System.IO.Directory.Exists(path);
#endif
            }

            public static string[] GetDirectories(string path)
            {
#if UNITY_EDITOR_WIN
                var directories = System.IO.Directory.GetDirectories(AddLongPathPrefix(path));
                RemoveLongPathPrefix(directories);
                return directories;
#else
                return System.IO.Directory.GetDirectories(path);
#endif
            }

            public static string[] GetDirectories(string path, string searchPattern)
            {
#if UNITY_EDITOR_WIN
                var directories = System.IO.Directory.GetDirectories(AddLongPathPrefix(path), searchPattern);
                RemoveLongPathPrefix(directories);
                return directories;
#else
                return System.IO.Directory.GetDirectories(path, searchPattern);
#endif
            }

            public static string[] GetDirectories(string path, string searchPattern, SearchOption searchOption)
            {
#if UNITY_EDITOR_WIN
                var directories = System.IO.Directory.GetDirectories(AddLongPathPrefix(path), searchPattern, searchOption);
                RemoveLongPathPrefix(directories);
                return directories;
#else
                return System.IO.Directory.GetDirectories(path, searchPattern, searchOption);
#endif
            }

            public static string[] GetFiles(string path)
            {
#if UNITY_EDITOR_WIN
                var files = System.IO.Directory.GetFiles(AddLongPathPrefix(path));
                RemoveLongPathPrefix(files);
                return files;
#else
                return System.IO.Directory.GetFiles(path);
#endif
            }

            public static string[] GetFiles(string path, string searchPattern)
            {
#if UNITY_EDITOR_WIN
                var files = System.IO.Directory.GetFiles(AddLongPathPrefix(path), searchPattern);
                RemoveLongPathPrefix(files);
                return files;
#else
                return System.IO.Directory.GetFiles(path, searchPattern);
#endif
            }

            public static string[] GetFiles(string path, string searchPattern, SearchOption searchOption)
            {
#if UNITY_EDITOR_WIN
                var files = System.IO.Directory.GetFiles(AddLongPathPrefix(path), searchPattern, searchOption);
                RemoveLongPathPrefix(files);
                return files;
#else
                return System.IO.Directory.GetFiles(path, searchPattern, searchOption);
#endif
            }
        }

        public static class File
        {
            public static bool Exists(string path)
            {
#if UNITY_EDITOR_WIN
                return System.IO.File.Exists(AddLongPathPrefix(path));
#else
                return System.IO.File.Exists(path);
#endif
            }
        }
    }
}
