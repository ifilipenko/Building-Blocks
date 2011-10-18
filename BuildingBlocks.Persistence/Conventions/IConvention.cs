using BuildingBlocks.Common;

namespace BuildingBlocks.Persistence.Conventions
{
    public interface IConvention : ICloneable<IConvention>
    {
    }

    public interface IConvention<in TApplied, out TResult> : IConvention
    {
        TResult ApplyTo(TApplied instance);
    }
}