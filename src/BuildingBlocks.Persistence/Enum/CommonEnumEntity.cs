namespace BuildingBlocks.Persistence.Enum
{
    public class CommonEnumEntity<TEnum>
        where TEnum : struct
    {
        public object Id { get; set; }
        public object ResolveKey { get; set; }
        public TEnum Enum { get; set; }
        public string Title { get; set; }
        public object Code { get; set; }
    }
}