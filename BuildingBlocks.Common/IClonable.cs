using System;

namespace BuildingBlocks.Common
{
    public interface ICloneable<T> : ICloneable
    {
        new T Clone();
    }
}