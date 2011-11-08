using System;
using System.IO;
using System.Linq;
using System.Reflection;
using CuttingEdge.Conditions;

namespace BuildingBlocks.Configuration.AssemblyLoad
{
    public class FileSystemAssembliesLoader : IAssembliesLoader
    {
        public static FileSystemAssembliesLoader LoaderForDllsContained(string assemblyNameSubstring, string filePath)
        {
            Condition.Requires(assemblyNameSubstring, "assemblyNameSubstring").IsNotNullOrEmpty();

            return LoaderForDllsWhereNameMathed(n => n.Contains(assemblyNameSubstring), filePath);
        }

        public static FileSystemAssembliesLoader LoaderForDllsWhereNameMathed(Func<string, bool> assemblyNameCondition, string filePath)
        {
            Condition.Requires(assemblyNameCondition, "assemblyNameCondition").IsNotNull();

            return new FileSystemAssembliesLoader(
                filePath,
                name =>
                    {
                        var assemblyName = Path.GetFileName(name) ?? string.Empty;
                        return assemblyName.EndsWith(".dll") && assemblyNameCondition(Path.GetFileNameWithoutExtension(assemblyName));
                    });
        }

        private readonly string _path;
        private readonly Func<string, bool> _fileNameCondition;

        public FileSystemAssembliesLoader(string path, Func<string, bool> fileNameCondition)
        {
            Condition.Requires(path, "path").IsNotNullOrEmpty();
            Condition.Requires(fileNameCondition, "fileNameCondition").IsNotNull();

            _path = path;
            _fileNameCondition = fileNameCondition;
        }

        public Assembly[] LoadAssemblies()
        {
            var allFiles = Directory.GetFiles(_path);
            var files = allFiles.Where(_fileNameCondition);
            return files.Select(Assembly.LoadFrom).ToArray();
        }
    }
}