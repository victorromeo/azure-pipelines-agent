using System.Threading.Tasks;
using Microsoft.VisualStudio.Services.Agent;
using Microsoft.VisualStudio.Services.Location;
using Microsoft.VisualStudio.Services.WebApi;

namespace Agent.Cli.Fakes
{
    public class FakeLocationServer : AgentService, ILocationServer
    {
        public Task ConnectAsync(VssConnection jobConnection)
        {
            // Connect immediately
            return Task.CompletedTask;
        }

        public Task<ConnectionData> GetConnectionDataAsync()
        {
            return new Task<ConnectionData>(() =>
            {
                return new ConnectionData()
                {
                    
                };
            });
        }
    }
}