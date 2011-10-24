namespace BuildingBlocks.Testing.Persistence.Model
{
    public class ServiceType 
    {
        public virtual long ID { get; set; }
        public virtual string LocalCode { get; set; }
        public virtual string FederalCode { get; set; }
        public virtual string Name { get; set; }
        public virtual decimal RedundantMember { get; set; }
        public virtual long Number { get; set; }
        public virtual Currency Currency { get; set; }
    }

    public class Currency
    {
        public Country Country { get; set; }
        public string FullName { get; set; }
        public string ShortName { get; set; }
        public string Sign { get; set; }
    }

    public class Country 
    {
        public virtual long ID { get; set; }
        public virtual string Name { get; set; }
    }

    public class PatientCase 
    {
        public virtual long ID { get; set; }
        public virtual string PatientName { get; set; }
    }

    public class Service
    {
        public virtual long ID { get; set; }
        public virtual decimal Quantity { get; set; }

        public virtual ServiceType ServiceType { get; set; }
        public virtual PatientCase Case { get; set; }
    }

    public class NotMappedEntity
    {
        public string Foo { get; set; }
        public string Bar { get; set; }
    }
}
