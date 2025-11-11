using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine.Profiling;

namespace UnityEditor.PackageManager.ValidationSuite
{
    internal class PackageCIUtils : CIUtils
    {
        internal static List<string> Pack(string path, string destinationPath)
        {
#if UNITY_2019_3_OR_NEWER
            Profiler.BeginSample("Pack");
            var packRequest = Client.Pack(path, destinationPath);
            while (!packRequest.IsCompleted)
            {
                Thread.Sleep(100);
            }

            if (packRequest.Status != StatusCode.Success)
                throw new Exception("Failed to properly pack package.  Error = " + packRequest.Error.message);

            var generatedPackages = new List<string>();
            generatedPackages.Add(packRequest.Result.tarballPath);
            Profiler.EndSample();
            return generatedPackages;
#else
            return _Pack("package", path, destinationPath);
#endif
        }
    }
}
