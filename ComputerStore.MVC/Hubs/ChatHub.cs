using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace ComputerStore.MVC.Hubs
{
    public class ChatHub : Hub
    {
        // Hàm này để nhận tin nhắn từ giao diện web và phát lại cho tất cả mọi người
        public async Task SendMessage(string user, string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }
    }
}