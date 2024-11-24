using Microsoft.AspNetCore.SignalR;

namespace MessagingWebApp
{
    public class ChatHub : Hub<IChatClient>
    {
        // These methods are for the client to send info to the server.

        public override async Task OnConnectedAsync()
        {
            //this.Clients.
            //this.Groups.
            //this.Context.
            await Clients.All.ReceiveMessage($"{Context.ConnectionId} has joined!");
        }

        public async Task SendMessage(string message)
        {
            await Clients.All.ReceiveMessage($"{Context.ConnectionId}: {message}");
        }
    }
}
