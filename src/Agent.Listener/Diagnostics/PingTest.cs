using System.Globalization;
using System.Net.NetworkInformation;

namespace Microsoft.VisualStudio.Services.Agent.Listener.Diagnostics
{
    class PingTest : IDiagnosticTest
    {
        public bool Execute(ITerminal terminal)
        {
            using (Ping ping = new Ping())
            {
                try
                {
                    terminal.WriteLine(string.Format(CultureInfo.CurrentCulture, "Attempt to Ping: {0} with timeout {1}", c_hostname, c_timeout));
                    PingReply pingreply = ping.Send(c_hostname, c_timeout);
                    terminal.WriteLine(string.Format(CultureInfo.CurrentCulture, "Address: {0}", pingreply.Address));
                    terminal.WriteLine(string.Format(CultureInfo.CurrentCulture, "Status: {0}", pingreply.Status));
                    terminal.WriteLine(string.Format(CultureInfo.CurrentCulture, "Round trip time: {0}", pingreply.RoundtripTime));

                    if (pingreply.Status != IPStatus.Success)
                    {
                        terminal.WriteError(string.Format(CultureInfo.CurrentCulture, "Unsuccessful status response from {0}.  Verify internet connection is working", c_hostname));
                        return false;
                    }
                }
                catch (PingException ex)
                {
                    terminal.WriteError(ex);
                    return false;
                }
            }

            return true;
        }

        private const string c_hostname = "www.bing.com";
        private const int c_timeout = 10000;
    }
}
