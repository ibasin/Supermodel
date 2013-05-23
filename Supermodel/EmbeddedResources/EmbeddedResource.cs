using System;
using System.IO;
using System.Reflection;

namespace Supermodel.EmbeddedResources
{
    public static class EmbeddedResource
    {
        public static string ReadTextFile(Assembly assembly, string resourceName)
        {
            using (var stream = assembly.GetManifestResourceStream(resourceName))
            {
                if (stream == null) throw new ArgumentException(resourceName + " is not found.");
                using (var reader = new StreamReader(stream)) { return reader.ReadToEnd(); }
            }
        }
    }
}
