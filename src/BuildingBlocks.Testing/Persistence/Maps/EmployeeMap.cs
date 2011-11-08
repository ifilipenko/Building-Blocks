using BuildingBlocks.Testing.Persistence.Model;
using FluentNHibernate.Mapping;

namespace BuildingBlocks.Testing.Persistence.Maps
{
    public class EmployeeMap : ClassMap<Employee>
    {
        public EmployeeMap()
        {
            Table("Employee");

            Id(e => e.ID, "ID").GeneratedBy.Native();
            Map(e => e.Name, "Name");
            Map(e => e.CategoryName, "CategoryName");
            References(e => e.Department, "Department_ID")
                .LazyLoad();
        }
    }
}