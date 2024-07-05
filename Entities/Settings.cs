using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordRegisterManagementBot.Entities
{
    public class Settings
    {
        public ulong guildId { get; set; }
        public bool status { get; set; }
        public string unregistered_role_id { get; set; }
        public string member_role_id { get; set; }
        public string modaretor_role_id { get; set; }
        public string register_channel_id { get; set; }
        public string log_channel_id { get; set; }
        public string register_tag { get; set; }
    }
}
