using System;

namespace BuildingBlocks.CopyManagement
{
    public interface IApplicationIdentity
    {
        Guid InstanceId { get; }
        DateTime InstanceStartTime { get; }
        string ApplicationUid { get; }
        string MashineId { get; }
        string LicenceKey { get; }
    }
}