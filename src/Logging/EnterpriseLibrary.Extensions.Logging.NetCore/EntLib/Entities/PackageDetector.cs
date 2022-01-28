using System;
using System.IO;
using System.Linq;
using System.Reflection;


namespace MorganStanley.ComposeUI.Logging.Entity
{
    public static class PackageDetector
    {
        public static bool IsPackageInstalled(string packageId)
        {
            var assemblies = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory, "*.dll")
                .Select(x => Assembly.Load(AssemblyName.GetAssemblyName(x)).GetName().Name)
                .ToArray();
            return assemblies.Contains(packageId);
        }
    }
}