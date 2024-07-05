using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.Exceptions;
using Newtonsoft.Json;
using Newtonsoft.Json.Bson;
using DiscordRegisterManagementBot.config;
using DiscordRegisterManagementBot.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JsonReader = DiscordRegisterManagementBot.config.JsonReader;

namespace DiscordRegisterManagementBot.Events
{
    public class Client_events
    {

        public static Task Client_Ready(DiscordClient sender, DSharpPlus.EventArgs.ReadyEventArgs args)
        {
            /* foreach (var guild in Program.Client.Guilds)
             {
                 bool status = false;
                 Console.WriteLine($"Bağlı olunan sunucu: {guild.Value.Name} (ID: {guild.Value.Id})");

                 dynamic data = await jsonReader.ReadJson<GuildSettings>("data.json");

                 if (data.AsyncState != null) foreach (Settings guildVal in data.settings) if (guildVal.guildId == guild.Value.Id) status = true;

                 if (!status)
                 {
                     Settings guildSettings = new Settings();
                     guildSettings.guildId = guild.Value.Id;
                     jsonReader.EkleVeKaydetAsync(guildSettings);
                 }

             }*/


            return Task.CompletedTask;
        }
        public static async Task Client_ModalSubmitted(DiscordClient sender, DSharpPlus.EventArgs.ModalSubmitEventArgs args)
        {

        }

        public static async Task Client_GuildMemberAdded(DiscordClient sender, DSharpPlus.EventArgs.GuildMemberAddEventArgs args)
        {
            var jsonReader = new JsonReader();
            dynamic data = await jsonReader.ReadJson<GuildSettings>("data.json");

            foreach (Settings guildVal in data.settings)

                if ((guildVal.guildId == args.Guild.Id))
                {
                    if ((guildVal.unregistered_role_id != null || guildVal.unregistered_role_id != ""))
                    {
                        var memberRole = args.Guild.Roles.Values.FirstOrDefault(r => r.Id == Convert.ToUInt64(guildVal.unregistered_role_id));
                        if (memberRole != null)
                        {
                            try
                            {
                                await args.Member.GrantRoleAsync(memberRole);
                            }
                            catch (NotFoundException)
                            {

                            }

                        }
                    }


                    if ((guildVal.register_channel_id != null || guildVal.register_channel_id != "") && (guildVal.modaretor_role_id != null || guildVal.modaretor_role_id != ""))
                    {
                        try
                        {
                            var btn_Register = new DiscordButtonComponent(ButtonStyle.Success, "RegisterToServer"  + args.Member.Id.ToString(), "Kayıt Et");
                            var btn_kick     = new DiscordButtonComponent(ButtonStyle.Danger, "kickFromServer"     + args.Member.Id.ToString(), "Sunucudan Tekmele!");

                            string accountStatus = args.Member.Verified != null ? "Güvenilir" : "Doğrulama Yapılmadı";


                            var message = new DiscordMessageBuilder()
                              .AddEmbed(new DiscordEmbedBuilder()
                              .WithColor(DiscordColor.Aquamarine)
                              .WithTitle("Yeni Kullanıcı Giriş Yaptı").WithDescription($"{args.Guild.GetRole(Convert.ToUInt64(guildVal.modaretor_role_id)).Mention} Buraya Bakmalısın \n\n{args.Member.Username} Sunucuya Katıldı \n\n{args.Member.Mention} Sunucumuza Hoşgeldin! \n\n{args.Guild.MemberCount}. üyemiz olarak saflarımızda yer almaya hazır \nDoğrulama Durumu: {accountStatus}")).AddComponents(btn_Register, btn_kick);

                            await args.Guild.GetChannel(Convert.ToUInt64(guildVal.register_channel_id)).SendMessageAsync(message);

                        }
                        catch (NotFoundException)
                        {

                            throw;
                        }
                    }
                }

        }

        public static async Task Client_ComponentInteractionCreated(DiscordClient sender, DSharpPlus.EventArgs.ComponentInteractionCreateEventArgs args)
        {
            var jsonReader = new JsonReader();
            dynamic data = await jsonReader.ReadJson<GuildSettings>("data.json");
          

            if (args.Interaction.Data.CustomId.Contains("kickFromServer"))
            {
                bool isAdmin = args.Guild.GetMemberAsync(args.User.Id).Result.Roles.Any(role => role.Permissions.HasPermission(Permissions.Administrator));
                if (!isAdmin) return;

                ulong id = Convert.ToUInt64(args.Interaction.Data.CustomId.ToString().Substring(14, args.Interaction.Data.CustomId.Length - 14));
                try
                {
                    var member = await args.Guild.GetMemberAsync(id);

                    var informationMessage = new DiscordEmbedBuilder
                    {
                        Color = DiscordColor.Green,
                        Title = "Üye Banlandı",
                        Description = $"{member.DisplayName} Üyesi {args.User.Mention} tarafından Banlandı"
                    };

                    await args.Interaction.Guild.BanMemberAsync(member);
                    await args.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().AddEmbed(informationMessage));
                }
                catch (NotFoundException)
                {
                    var failMessage = new DiscordEmbedBuilder
                    {
                        Color = DiscordColor.Green,
                        Title = "Üye Bulunamadı",
                        Description = $"Zaten bu üye banlandı veya sunucudan çıktı."
                    };
                    await args.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().AddEmbed(failMessage));

                }

            }

            if (args.Interaction.Data.CustomId.Contains("RegisterToServer"))
            {
                Settings dataGuild;

                foreach (var obj in data.settings)
                {
                    if (obj.guildId == args.Guild.Id) dataGuild = obj;
                }

                data = await jsonReader.ReadJson<GuildSettings>("data.json");
                ulong id = Convert.ToUInt64(args.Interaction.Data.CustomId.ToString().Substring(16, args.Interaction.Data.CustomId.Length - 16));
                var newMember = args.Guild.GetMemberAsync(id);
                var roleCheck = newMember.Result.Roles.FirstOrDefault(x => x.Id == data.member_role_id);
                if (roleCheck != null)
                {
                    var failedMessage = new DiscordEmbedBuilder
                    {
                        Color = DiscordColor.Red,
                        Title = "Başarısız",
                        Description = $"Bu kullanıcı zaten kayıt edildi!"
                    };

                    await args.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().AddEmbed(failedMessage));
                }
                else
                {
                  
                 
                    if ( ( data.registration_quesitons!= null ) || (data.registration_quesitons != "") )
                    {

                        var modal = new DiscordInteractionResponseBuilder()
                                         .WithTitle("Oyuncu Kayıt Etme")
                                         .WithCustomId(id.ToString());

                        foreach ( var item in data.registration_quesitons)
                        {
                            modal.AddComponents(new TextInputComponent(label : item.label, customId: item.CustomId, placeholder: item.PlaceHolder));
                        }

                        await args.Interaction.CreateResponseAsync(DSharpPlus.InteractionResponseType.Modal, modal);

                    }
                }


            }



        }
    }
}

