using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WintersGiveaway.Models
{
    public class PrizeAssignment
    {
        public DiscordGuildMember GuildMember { get; set; }
        public string Prize { get; set; }
    }
}
