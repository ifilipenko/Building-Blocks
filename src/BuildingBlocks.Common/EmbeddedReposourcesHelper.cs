using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;

namespace BuildingBlocks.Common
{
    public class EmbeddedTestsReposourcesHelper
    {
        private readonly Assembly _assembly;
        private List<string> _xmlResources;
        private List<string> _allResources;
        private readonly object _lockObject = new object();

        public EmbeddedTestsReposourcesHelper(Assembly assembly)
        {
            _assembly = assembly;
        }

        void ExtractGeneratorMetadataFilesFromResources()
        {
            EmbeddedResourceLoader loader = new EmbeddedResourceLoader(_assembly);

            _xmlResources = new List<string>();
            _allResources = new List<string>();
            foreach (string resource in loader.ContainedResources)
            {
                if (resource.ToLower().EndsWith(".xml"))
                {
                    CopyXmlFile(loader, resource);
                    _xmlResources.Add(resource);
                }
                else
                {
                    CopyFile(loader, resource);
                }
                _allResources.Add(resource);
            }
        }

        private static void CopyFile(EmbeddedResourceLoader loader, string resource)
        {
            if (File.Exists(resource))
                return;

            Stream fileStream = loader.GetResource(resource);

            byte[] buffer = new byte[fileStream.Length];
            fileStream.Read(buffer, 0, (int) fileStream.Length);

            FileStream writer = new FileStream(resource, FileMode.CreateNew);
            writer.Write(buffer, 0, buffer.Length);
            fileStream.Close();
        }

        private static void CopyXmlFile(EmbeddedResourceLoader loader, string resource)
        {
            XmlDocument document = loader.GetResourceAsXml(resource);
            document.Save(resource);
        }

        public string[] AllResourceFiles
        {
            get
            {
                lock (_lockObject)
                {
                    if (_allResources == null)
                    {
                        ExtractGeneratorMetadataFilesFromResources();
                    }
                }
                return _allResources.ToArray();
            }
        }

        public string[] XmlFiles
        {
            get
            {
                lock (_lockObject)
                {
                    if (_xmlResources == null)
                    {
                        ExtractGeneratorMetadataFilesFromResources();
                    }
                }
                return _xmlResources.ToArray();
            }
        }

        public bool ContainsEndsWith(string resurceName)
        {
            return (from r in AllResourceFiles
                    where r.ToLower().EndsWith(resurceName.ToLower())
                    select true).DefaultIfEmpty(false).First();
        }

        public string GetFullName(string resurceName)
        {
            return (from r in AllResourceFiles
                    where r.ToLower().EndsWith(resurceName.ToLower())
                    select r).First();
        }
    }
}