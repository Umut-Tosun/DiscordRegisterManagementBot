using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using DSharpPlus.Exceptions;
using DiscordRegisterManagementBot.Commands.Prefix;
using DiscordRegisterManagementBot.config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DiscordRegisterManagementBot.Events;
using DiscordRegisterManagementBot.Entities;

namespace DiscordRegisterManagementBot
{
    public class Program
    {
        public static DiscordClient Client { get; set; }
        public static CommandsNextExtension Commands { get; set; }
        static async Task  Main(string[] args)
        {
            var jsonReader = new JsonReader();
            dynamic obj = await jsonReader.ReadJson<ReadToken>("config.json");


            var discordConfig = new DiscordConfiguration()
            {
                Intents = DiscordIntents.All,
                Token = obj.token,
                TokenType = TokenType.Bot,
                AutoReconnect = true
            };

            Client = new DiscordClient(discordConfig);

            Client.Ready += Client_events.Client_Ready;
            Client.ComponentInteractionCreated += Client_events.Client_ComponentInteractionCreated;
            Client.GuildMemberAdded += Client_events.Client_GuildMemberAdded;
            Client.ModalSubmitted += Client_events.Client_ModalSubmitted;

            var commandsConfig = new CommandsNextConfiguration()
            {
                StringPrefixes = new string[] { obj.prefix },
                EnableMentionPrefix = true,
                EnableDms = true,
                EnableDefaultHelp = false,
            };

            Commands = Client.UseCommandsNext(commandsConfig);
            Commands.RegisterCommands<Register_Commands>();

        

            await Client.ConnectAsync();
            await Task.Delay(-1);

            

        }
      

    }
}
