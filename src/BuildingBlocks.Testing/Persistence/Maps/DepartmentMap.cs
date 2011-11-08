using BuildingBlocks.Testing.Persistence.Model;
using FluentNHibernate.Mapping;

namespace BuildingBlocks.Testing.Persistence.Maps
{
    public class DepartmentMap : ClassMap<Department>
    {
        public DepartmentMap()
        {
            Table("Department");

            Id(e => e.ID, "ID").GeneratedBy.Native();
            Map(e => e.Name, "Name");
        }
    }
}