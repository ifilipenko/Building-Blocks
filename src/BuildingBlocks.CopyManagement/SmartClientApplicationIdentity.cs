using System;

namespace BuildingBlocks.CopyManagement
{
    public class SmartClientApplicationIdentity : IApplicationIdentity
    {
        private static readonly Lazy<IApplicationIdentity> _current;

        static SmartClientApplicationIdentity()
        {
            _current = new Lazy<IApplicationIdentity>(() => new SmartClientApplicationIdentity(), true);
        }

        public static IApplicationIdentity Current
        {
            get { return _current.Value; }
        }

        private static readonly Guid _instanceId = Guid.NewGuid();
        private readonly string _applicationUid;
        private readonly string _mashineId;
        private readonly DateTime _instanceStartTime;

        private SmartClientApplicationIdentity()
        {
            _instanceStartTime = System.Diagnostics.Process.GetCurrentProcess().StartTime;
            _applicationUid = ComputeApplicationId();
            _mashineId = ComputerId.Value.ToFingerPrintMd5Hash();
        }

        public Guid InstanceId
        {
            get { return _instanceId; }
        }

        public string ApplicationUid
        {
            get { return _applicationUid; }
        }

        public string MashineId
        {
            get { return _mashineId; }
        }

        public DateTime InstanceStartTime
        {
            get { return _instanceStartTime; }
        }

        public string LicenceKey { get; private set; }

        private static string ComputeApplicationId()
        {
            var exeFileName = System.Reflection.Assembly.GetEntryAssembly().Location;
            var value = "EXE_PATH >> " + exeFileName + "\n" + ComputerId.Value;
            return value.ToFingerPrintMd5Hash();
        }
    }
}