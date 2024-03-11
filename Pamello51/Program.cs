using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Pamello51
{
	public class Program
	{
		public static Task Main() => new Program().MainAsync();

		public async Task MainAsync() {
			using IHost host = Host.CreateDefaultBuilder()
				.ConfigureServices((_, services) =>
				services.AddSingleton(x => new DiscordSocketClient(new DiscordSocketConfig {
					GatewayIntents = GatewayIntents.All,
					AlwaysDownloadUsers = true
				}))).Build();

			await RunAsync(host);
		}

		public async Task RunAsync(IHost host) {
			using IServiceScope serviceScope = host.Services.CreateScope();
			IServiceProvider provider = serviceScope.ServiceProvider;

			DiscordSocketClient client = provider.GetRequiredService<DiscordSocketClient>();

			client.Log += async (message) => Console.WriteLine($"[log] {message}");
			client.Ready += async () => Console.WriteLine($"{client.CurrentUser.Username} Ready");

			await client.LoginAsync(TokenType.Bot, "MTIxNjY4MjEwMjU3NDI4NDg4MQ.GHxEGe.G4Fx0OyiFytvog1GRKU5l-2LRT7ai9blMKI6aY");
			await client.StartAsync();

			await Task.Delay(-1);
		}
	}
}
