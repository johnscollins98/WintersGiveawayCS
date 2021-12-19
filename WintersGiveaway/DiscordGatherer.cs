using Newtonsoft.Json;
using WintersGiveaway.Models;

namespace WintersGiveaway
{
    public class DiscordGatherer
    {
        private readonly HttpClient client = new HttpClient();

        private readonly string rootUrl;
        private readonly string botToken;
        private readonly string guildId;
        private readonly string channelId;

        public DiscordGatherer(string rootUrl, string botToken, string guildId, string channelId)
        {
            this.rootUrl = rootUrl;
            this.botToken = botToken;
            this.guildId = guildId;
            this.channelId = channelId;
        }

        public async Task<IEnumerable<DiscordUser>> GetDiscordMessageReactions(string messageId, string emoji)
        {
            bool finishedGathering = false;
            IEnumerable<DiscordUser> res = new List<DiscordUser>();
            string? lastId = null;

            while (!finishedGathering)
            {
                var endpoint = $"/channels/{channelId}/messages/{messageId}/reactions/{emoji}?limit=100";
                if (lastId != null)
                {
                    endpoint += $"&after={lastId}";
                }

                var response = await MakeDiscordRequest<IEnumerable<DiscordUser>>(endpoint);
                finishedGathering = response.Count() < 100;
                lastId = response.Last().Id;
                res = res.Concat(response);
            }

            return res;
        }

        public async Task<IEnumerable<string>> GetPrizes(string messageId)
        {
            var message = await GetDiscordMessage(messageId);
            var prizes = message.Content.Split("\n").Skip(1);
            return prizes;
        }

        public async Task<IEnumerable<DiscordGuildMember>> GetDiscordGuildMembers()
        {
            var endpoint = $"/guilds/{guildId}/members?limit=1000";
            var response = await MakeDiscordRequest<IEnumerable<DiscordGuildMember>>(endpoint);
            return response;
        }

        private async Task<DiscordMessage> GetDiscordMessage(string messageId)
        {
            var endpoint = $"/channels/{channelId}/messages/{messageId}";
            var response = await MakeDiscordRequest<DiscordMessage>(endpoint);
            return response;
        }

        private async Task<T> MakeDiscordRequest<T>(string endpoint)
        {
            var message = new HttpRequestMessage(HttpMethod.Get, $"{rootUrl}{endpoint}");
            message.Headers.Add("Authorization", $"Bot {botToken}");
            var response = await client.SendAsync(message);
            var jsonString = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<T>(jsonString);
        }
    }
}
