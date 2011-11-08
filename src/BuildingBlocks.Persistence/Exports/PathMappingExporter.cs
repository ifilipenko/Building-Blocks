using System;
using System.IO;
using FluentNHibernate.Cfg;

namespace BuildingBlocks.Persistence.Exports
{
    public class PathMappingExporter : IMappingExporter
    {
        private readonly string _path;

        public PathMappingExporter(string path)
        {
            if (!Directory.Exists(path))
                throw new ArgumentException("Path not exists", "path");

            _path = path;
        }

        public void Export(FluentMappingsContainer fluentMappings)
        {
            fluentMappings.ExportTo(_path);
        }

        public void Export(AutoMappingsContainer autoMappings)
        {
            autoMappings.ExportTo(_path);
        }

        public IMappingExporter Clone()
        {
            return new PathMappingExporter(_path);
        }
    }
}