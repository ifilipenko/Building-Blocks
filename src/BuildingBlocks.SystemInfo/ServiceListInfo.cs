using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.ServiceProcess;

namespace BuildingBlocks.SystemInfo
{
    public class ServiceListInfo : ReadOnlyCollection<ServiceInfo>
    {
        public ServiceListInfo()
            : base(GetServices())
        {
            
        }

        private static IList<ServiceInfo> GetServices()
        {
            return ServiceController.GetServices()
                .Select(svc => new ServiceInfo
                                   {
                                       ServiceDisplayName = svc.DisplayName,
                                       MachineName = svc.MachineName,
                                       ServiceName = svc.ServiceName,
                                       ServiceType = svc.ServiceType.ToString(),
                                       Status = svc.Status.ToString()
                                   })
                .ToList();
        }
    }
}