using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Pamello51
{
	public class Program
	{
		public static DiscordSocketClient Client;

		public static async Task Main(string[] args) {
			Client = new DiscordSocketClient(new DiscordSocketConfig {
				GatewayIntents = GatewayIntents.All,
				AlwaysDownloadUsers = true
			});

			Client.Log += async (message) => Console.WriteLine($"[log] {message}");
			Client.Ready += async () => Console.WriteLine($"{Client.CurrentUser.Username} Ready");

			await Client.LoginAsync(TokenType.Bot, "");
			await Client.StartAsync();

			await Task.Delay(-1);
		}
	}
}
