using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace WintersGiveaway.Models
{
    public class DiscordGuildMember
    {
        public DiscordUser User { get; set; }
        public string? Nick { get; set; }

        [JsonProperty("joined_at")]
        public DateTime Joined { get; set; }
    }
}