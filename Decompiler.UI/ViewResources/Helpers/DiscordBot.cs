using Decompiler.UI.ViewResources.Data;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Decompiler.UI.ViewResources.Helpers
{
    public class DiscordBot
    {
        public DiscordSocketClient? Client { get; private set; }
        public async Task RunAsync(Func<Task> onReady)
        {
            Client = new DiscordSocketClient();

            Client.Log += ClientLog;
            await Client.LoginAsync(Discord.TokenType.Bot, User.DiscordBot);
            await Client.StartAsync();
            Client.Ready += onReady;
            await Task.Delay(5000);
        }

        private async Task ClientLog(Discord.LogMessage ex)
        {
            Directory.CreateDirectory($"{Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)}\\Temp");
            await File.AppendAllTextAsync($"{Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)}\\Temp\\discord.log.txt", $"[{DateTime.Now}] {ex.ToString()}\n");
        }
    }
}
