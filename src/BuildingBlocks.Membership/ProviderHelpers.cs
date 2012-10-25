using System;
using System.Diagnostics;
using System.Web.Hosting;

namespace BuildingBlocks.Membership
{
    internal class ProviderHelpers
    {
        public static string GetDefaultAppName()
        {
            try
            {
                var virtualPath = HostingEnvironment.ApplicationVirtualPath;

                if (string.IsNullOrEmpty(virtualPath))
                {
                    virtualPath = Process.GetCurrentProcess().MainModule.ModuleName;
                    var startIndex = virtualPath.IndexOf('.');
                    if (startIndex > -1)
                    {
                        virtualPath = virtualPath.Remove(startIndex);
                    }
                }

                return string.IsNullOrEmpty(virtualPath) ? "/" : virtualPath;
            }
            catch (Exception)
            {
                return "/";
            }
        }
    }
}