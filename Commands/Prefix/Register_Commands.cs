using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordRegisterManagementBot.Commands.Prefix
{
    public class Register_Commands : BaseCommandModule
    {
        [Command("test")]
        public async Task DropDownList(CommandContext ctx)
        {
            await ctx.Channel.SendMessageAsync("Running bot");
        }
    }
}
