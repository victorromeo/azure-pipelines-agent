using System.Net.NetworkInformation;

namespace Microsoft.VisualStudio.Services.Agent.Listener.Diagnostics
{
    class MtuInfo : IDiagnostic
    {
        public bool Execute(ITerminal terminal)
        {
            NetworkInterface[] nics = NetworkInterface.GetAllNetworkInterfaces();
            IPGlobalProperties properties = IPGlobalProperties.GetIPGlobalProperties();
            terminal.WriteLine(string.Format("IP interface information for {0}.{1}", properties.HostName, properties.DomainName));
            terminal.WriteLine();

            foreach (NetworkInterface adapter in nics)
            {
                terminal.WriteLine(adapter.Description);

                if (adapter.Supports(NetworkInterfaceComponent.IPv4))
                {
                    IPInterfaceProperties adapterProperties = adapter.GetIPProperties();
                    IPv4InterfaceProperties p = adapterProperties.GetIPv4Properties();
                    if (p == null)
                    {
                        terminal.WriteLine("No IPv4 information is available for this interface.");
                    }
                    else
                    {
                        terminal.WriteLine(string.Format("  IPv4 MTU ............................... : {0}", p.Mtu));
                    }
                }

                if (adapter.Supports(NetworkInterfaceComponent.IPv6))
                {
                    IPInterfaceProperties adapterProperties = adapter.GetIPProperties();
                    IPv6InterfaceProperties p = adapterProperties.GetIPv6Properties();
                    if (p == null)
                    {
                        terminal.WriteLine("No IPv6 information is available for this interface.");
                    }
                    else
                    {
                        terminal.WriteLine(string.Format("  IPv6 MTU ............................... : {0}", p.Mtu));
                    }
                }
            }

            return true;
        }
    }
}
