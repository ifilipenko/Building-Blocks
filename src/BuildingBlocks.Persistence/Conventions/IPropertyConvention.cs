using System;
using System.Reflection;

namespace BuildingBlocks.Persistence.Conventions
{
    public interface IPropertyConvention : IConvention<Type, PropertyInfo>
    {
    }
}