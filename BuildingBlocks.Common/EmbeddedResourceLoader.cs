using System.IO;
using System.Reflection;
using System.Xml;

namespace BuildingBlocks.Common
{
    public class EmbeddedResourceLoader
    {
        readonly Assembly _assembly;

        public EmbeddedResourceLoader()
        {
            _assembly = Assembly.GetCallingAssembly();
        }

        public EmbeddedResourceLoader(Assembly resurceAssembly)
        {
            _assembly = resurceAssembly;
        }

        public string[] ContainedResources
        {
            get { return _assembly.GetManifestResourceNames(); }
        }

        public Stream GetResource(string resourceName)
        {
            return _assembly.GetManifestResourceStream(resourceName);
        }

        public XmlDocument GetResourceAsXml(string resourceName)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(GetResource(resourceName));
            return doc;
        }

        public void ExtractResourceToXmlFile(string fileName)
        {
            if (!File.Exists(fileName))
            {
                GetResourceAsXml(fileName).Save(fileName);
            }
        }
    }
}