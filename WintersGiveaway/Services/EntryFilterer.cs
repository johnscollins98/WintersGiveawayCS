using WintersGiveaway.Interfaces;
using WintersGiveaway.Models;

namespace WintersGiveaway.Services
{
    public class EntryFilterer : IEntryFilterer
    {
        private readonly IDiscordGatherer discordGatherer;
        private readonly IConfigManager configManager;

        public EntryFilterer(IDiscordGatherer discordGatherer, IConfigManager configManager)
        {
            this.discordGatherer = discordGatherer;
            this.configManager = configManager;
        }

        public async Task<IEnumerable<DiscordGuildMember>> GetEligibleGuildMembersAsync()
        {
            var guildMembers = await discordGatherer.GetDiscordGuildMembersAsync();
            var usersWhoReacted = await discordGatherer.GetDiscordMessageReactionsAsync();
            return usersWhoReacted
                .Where(user => guildMembers.Any(g => g.User.Id == user.Id))
                .Select(user => guildMembers.First(g => g.User.Id == user.Id))
                .Where(guildMember => guildMember.Joined <= configManager.GetConfg().CutoffDate);
        }
    }
}
