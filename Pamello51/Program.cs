using AngleSharp;
using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Pamello51
{
	public class Program
	{
		public static DiscordSocketClient Client;

		public static IConfigurationRoot Configuration;

		public static async Task Main(string[] args) {
			using IHost host = Host.CreateDefaultBuilder()
				.ConfigureServices((_, services) =>
				services.AddSingleton(x => new DiscordSocketClient(new DiscordSocketConfig {
					GatewayIntents = GatewayIntents.All,
					AlwaysDownloadUsers = true
				}))).Build();

			using IServiceScope serviceScope = host.Services.CreateScope();
			IServiceProvider provider = serviceScope.ServiceProvider;

			Client = provider.GetRequiredService<DiscordSocketClient>();
			Configuration = new ConfigurationBuilder()
				.SetBasePath(AppContext.BaseDirectory)
				.AddYamlFile("config.yaml")
				.Build();

			Client.Log += async (message) => Console.WriteLine($"[log] {message}");
			Client.Ready += async () => Console.WriteLine($"{Client.CurrentUser.Username} Ready");

			await Client.LoginAsync(TokenType.Bot, Configuration["token"] ?? throw new Exception("cant find token in configuration file"));
			await Client.StartAsync();

			await Task.Delay(-1);
		}
	}
}
