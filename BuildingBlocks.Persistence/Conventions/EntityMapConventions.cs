using System;
using FluentNHibernate.Mapping;

namespace BuildingBlocks.Persistence.Conventions
{
    public delegate void IdConvention(IdentityPart identityPart, string tableName);

    public class EntityMapConventions : IEntityMapConventions
    {
        public EntityMapConventions()
        {
            IdConvention = ((identityPart, tableName) =>
            {
                try
                {
                  identityPart
                      .GeneratedBy
                      .Native(b => b.AddParam("sequence", "seq_" + tableName));
                }
                catch (InvalidOperationException)
                {
                    identityPart
                        .GeneratedBy.Assigned();
                }
            });
        }

        public IdConvention IdConvention { get; set; }
        public bool DefaultCacheable { get; set; }

        public IConvention Clone()
        {
            return new EntityMapConventions
                       {
                           IdConvention = IdConvention,
                           DefaultCacheable = DefaultCacheable
                       };
        }

        object ICloneable.Clone()
        {
            return Clone();
        }
    }
}