using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordRegisterManagementBot.Entities
{
    public class Question
    {
        public string Label { get; set; }

        public string CustomId { get; set; }
        public string PlaceHolder { get; set; }
    }
}
