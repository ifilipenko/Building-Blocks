using System;
using System.IO;

namespace BuildingBlocks.Common
{
    public static class DirectoryInfoExtensions
    {
        public static void CopyTo(this DirectoryInfo source, string destDirectory, bool recursive = true)
        {
            if (source == null)
                throw new ArgumentNullException("source");
            if (destDirectory == null)
                throw new ArgumentNullException("destDirectory");

            if (!source.Exists)
                throw new DirectoryNotFoundException("Source directory not found: " + source.FullName);
            var target = new DirectoryInfo(destDirectory);
            if (!target.Exists)
                target.Create();

            foreach (var file in source.GetFiles())
            {
                file.CopyTo(Path.Combine(target.FullName, file.Name), true);
            }

            if (!recursive)
                return;

            foreach (var directory in source.GetDirectories())
            {
                CopyTo(directory, Path.Combine(target.FullName, directory.Name));
            }
        }
    }
}
