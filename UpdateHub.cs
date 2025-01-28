using Microsoft.AspNetCore.SignalR;

namespace backend_api
{
    public class UpdateHub : Hub
    {
        public async Task SendUpdate(string message)
        {
            await Clients.All.SendAsync("ReceiveUpdate", message);
        }
    }
}
