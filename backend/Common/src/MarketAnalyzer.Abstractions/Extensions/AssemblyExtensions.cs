using System.IO;
using System.Reflection;

namespace System
{
    public static class AssemblyExtensions
    {
        public static byte[] ReadResourceBytes(this Type type, string name)
        {
            return ReadBytes(type.Assembly, $"{type.Namespace}.{name}");
        }

        public static byte[] ReadBytes(this Assembly assembly, string name)
        {
            ArgumentNullException.ThrowIfNull(assembly);
            ArgumentException.ThrowIfNullOrEmpty(nameof(name));

            using Stream stream = assembly.GetManifestResourceStream(name);
            if (stream is null)
            {
                throw new FileNotFoundException($"Resource '{name}' not found in assembly '{assembly.FullName}'.");
            }

            using MemoryStream memoryStream = new();
            stream.CopyTo(memoryStream);
            return memoryStream.ToArray();
        }
    }
}
