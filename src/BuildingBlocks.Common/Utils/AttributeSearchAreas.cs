using System;

namespace BuildingBlocks.Common.Utils
{
    [Flags]
    public enum AttributeSearchAreas
    {
        TargetClass = 0x0,
        Interfaces = 0x1,
        BaseClasses = 0x4,
        All = Interfaces | BaseClasses | TargetClass
    }
}