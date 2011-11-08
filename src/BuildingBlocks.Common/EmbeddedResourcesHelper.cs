using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;

namespace BuildingBlocks.Common
{
    public class EmbeddedTestsResourcesHelper
    {
        private readonly Assembly _assembly;
        private List<string> _xmlResources;
        private List<string> _allResources;
        private readonly object _lockObject = new object();

        public EmbeddedTestsResourcesHelper(Assembly assembly)
        {
            _assembly = assembly;
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

        void ExtractGeneratorMetadataFilesFromResources()
        {
            EmbeddedResourceLoader loader = new EmbeddedResourceLoader(_assembly);

            _allResources = new List<string>();
            foreach (string resource in loader.ContainedResources)
            {
                string fileName = IsXmlFile(resource)
                                      ? CopyXmlFile(loader, resource)
                                      : CopyFile(loader, resource, ResourceNameToFile);

                if (!string.IsNullOrEmpty(fileName))
                    _allResources.Add(fileName);
            }

            _xmlResources = _allResources.Where(IsXmlFile).ToList();
        }

        private static string ResourceNameToFile(string resource)
        {
            return IsTooLogFileName(resource) ? TrimNamespaces(resource) : resource;
        }

        private static bool IsXmlFile(string resource)
        {
            return resource.ToLower().EndsWith(".xml");
        }

        private static string CopyFile(EmbeddedResourceLoader loader, string resource, System.Func<string, string> resourceGetter)
        {
            string fileName = resourceGetter(resource);
            if (File.Exists(fileName))
            {
                throw new InvalidOperationException(string.Format("File \"{0}\" already exists", fileName));
            }

            Stream fileStream = loader.GetResource(resource);

            byte[] buffer = new byte[fileStream.Length];
            fileStream.Read(buffer, 0, (int) fileStream.Length);

            var writer = new FileStream(fileName, FileMode.Create);
            writer.Write(buffer, 0, buffer.Length);
            fileStream.Close();

            return fileName;
        }

        private static string CopyXmlFile(EmbeddedResourceLoader loader, string resource)
        {
            XmlDocument document = loader.GetResourceAsXml(resource);
            string fileName = IsTooLogFileName(resource) ? TrimNamespaces(resource) : resource;
            document.Save(fileName);

            return fileName;
        }

        private static bool IsTooLogFileName(string resource)
        {
            string fileName = Path.Combine(Directory.GetCurrentDirectory(), resource);
            return fileName.Length > 255;
        }

        private static string TrimNamespaces(string resource)
        {
            int indexOfStartExtension = resource.LastIndexOf('.');
            if (indexOfStartExtension <= 0)
                return resource;

            int indexOfStartFilename = resource.LastIndexOf('.', indexOfStartExtension - 1);
            if (indexOfStartExtension <= 0)
                return resource;

            return resource.Substring(indexOfStartFilename + 1);
        }

        public void CopyMathchedResourceToFile(Predicate<string> resourceSelector, string fileName)
        {
            var loader = new EmbeddedResourceLoader(_assembly);

            string resource = (from r in loader.ContainedResources
                               where resourceSelector(r)
                               select r).FirstOrDefault();

            if (string.IsNullOrEmpty(resource))
            {
                throw new InvalidOperationException("No matched resources");
            }

            CopyFile(loader, resource, r => fileName);
        }

        public string GetFullName(string resurceName)
        {
            return (from r in AllResourceFiles
                    where r.ToLower().EndsWith(resurceName.ToLower())
                    select r).First();
        }
    }
}