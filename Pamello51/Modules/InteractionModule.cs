using Discord.Interactions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pamello51.Modules
{
	public class InteractionModule : InteractionModuleBase<SocketInteractionContext>
	{
		[SlashCommand("hello", "great da bot")]
		public async Task HelloCommand() {
			await RespondAsync("hellow!");
		}
	}
}
