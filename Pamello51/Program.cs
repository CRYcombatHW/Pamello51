using AngleSharp;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Pamello51.Handlers;

namespace Pamello51
{
	public class Program
	{
		public static DiscordSocketClient Client;
		public static InteractionService Commands;

		public static IConfigurationRoot Configuration;

		public static async Task Main(string[] args) {
			using IHost host = Host.CreateDefaultBuilder()
				.ConfigureServices((_, services) =>
					services.AddSingleton<InteractionHandler>()
				).Build();

			using IServiceScope serviceScope = host.Services.CreateScope();
			IServiceProvider provider = serviceScope.ServiceProvider;

			Configuration = new ConfigurationBuilder()
				.SetBasePath(AppContext.BaseDirectory)
				.AddYamlFile("config.yaml")
				.Build();

			Client = new DiscordSocketClient(new DiscordSocketConfig {
				GatewayIntents = GatewayIntents.All,
				AlwaysDownloadUsers = true,
			});
			Commands = new InteractionService(Client);

			await provider.GetRequiredService<InteractionHandler>().InitializeAsync();

			Client.Log += async (message) => Console.WriteLine($"[client log] {message}");
			Client.Ready += async () => {
				await Commands.RegisterCommandsToGuildAsync(ulong.Parse(Configuration["guild"] ?? throw new Exception("cant find token in configuration file")));
				Console.WriteLine($"{Client.CurrentUser.Username} Ready");
			};

			Commands.Log += async (message) => Console.WriteLine($"[commands log] {message}");

			await Client.LoginAsync(TokenType.Bot, Configuration["token"] ?? throw new Exception("cant find token in configuration file"));
			await Client.StartAsync();

			await Task.Delay(-1);
		}
	}
}
