using Newtonsoft.Json;
using WintersGiveaway.Interfaces;
using WintersGiveaway.Models;

namespace WintersGiveaway.Services
{
    public class DiscordGatherer : IDiscordGatherer
    {
        private readonly string rootUrl = "https://discord.com/api/v9";

        private readonly Config config;
        private readonly IApiRequester apiRequester;

        public DiscordGatherer(IApiRequester apiRequester, IConfigManager configManager)
        {
            this.config = configManager.GetConfg();
            this.apiRequester = apiRequester;
        }

        public async Task<IEnumerable<DiscordUser>> GetDiscordMessageReactionsAsync()
        {
            bool finishedGathering = false;
            IEnumerable<DiscordUser> res = new List<DiscordUser>();
            string? lastId = null;

            while (!finishedGathering)
            {
                var endpoint = $"/channels/{config.ChannelId}/messages/{config.EntryMessageId}/reactions/{config.EntryEmoji}?limit=100";
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

        public async Task<IEnumerable<string>> GetPrizesAsync()
        {
            var message = await GetDiscordMessage(config.PrizeMessageId);
            var prizes = message.Content.Split("\n").Skip(1);
            return prizes;
        }

        public async Task<IEnumerable<DiscordGuildMember>> GetDiscordGuildMembersAsync()
        {
            var endpoint = $"/guilds/{config.GuildId}/members?limit=1000";
            var response = await MakeDiscordRequest<IEnumerable<DiscordGuildMember>>(endpoint);
            return response;
        }

        private async Task<DiscordMessage> GetDiscordMessage(string messageId)
        {
            var endpoint = $"/channels/{config.ChannelId}/messages/{messageId}";
            var response = await MakeDiscordRequest<DiscordMessage>(endpoint);
            return response;
        }

        private async Task<T> MakeDiscordRequest<T>(string endpoint) where T : class
        {
            var message = new HttpRequestMessage(HttpMethod.Get, $"{rootUrl}{endpoint}");
            message.Headers.Add("Authorization", $"Bot {config.BotToken}");
            return await apiRequester.MakeRequestAsync<T>(message);
        }
    }
}
