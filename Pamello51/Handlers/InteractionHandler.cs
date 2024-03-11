using Discord.Interactions;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Pamello51.Handlers
{
	public class InteractionHandler
	{
		private readonly IServiceProvider _services;

		public InteractionHandler(IServiceProvider services) {
			_services = services;
		}

		public async Task InitializeAsync() {
			await Program.Commands.AddModulesAsync(Assembly.GetEntryAssembly(), _services);

			Program.Client.InteractionCreated += HandleInteracion;
		}

		public async Task HandleInteracion(SocketInteraction interaction) {
			try {
				SocketInteractionContext context = new SocketInteractionContext(Program.Client, interaction);
				await Program.Commands.ExecuteCommandAsync(context, _services);
			}
			catch (Exception x) {
				Console.WriteLine(x);
			}
		}
	}
}
