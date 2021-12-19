using System;
using System.Configuration;
using System.Collections.Specialized;

namespace WintersGiveaway
{
    public class Program
    {
        static async Task Main(string[] args)
        {
            string botToken = GetConfigVariable("botToken");
            string channelId = GetConfigVariable("channelId");
            string guildId = GetConfigVariable("guildId");
            string prizeMessageId = GetConfigVariable("prizeMessageId");
            string entryMessageId = GetConfigVariable("entryMessageId");
            string cutoffISODateString = GetConfigVariable("cutoffDate");
            DateTime cutoffDate = DateTime.Parse(cutoffISODateString);

            var discordGatherer = new DiscordGatherer("https://discord.com/api/v9", botToken, guildId, channelId);
            var prizes = await discordGatherer.GetPrizes(prizeMessageId);
            var guildMembers = await discordGatherer.GetDiscordGuildMembers();
            var usersWhoReacted = await discordGatherer.GetDiscordMessageReactions(entryMessageId, "🎁");

            var entryFilterer = new EntryFilterer(usersWhoReacted, guildMembers);
            var eligibleMembers = entryFilterer.GetEligibleGuildMembers(cutoffDate);

            var prizeAssigner = new PrizeAssigner(prizes, eligibleMembers.ToList(), new RandomNumberGenerator());
            var prizeAssignments = prizeAssigner.GetPrizeAssignments();

            foreach (var prizeAssignment in prizeAssignments)
            {
                Console.WriteLine($"Prize: {prizeAssignment.Prize} " +
                    $"- Winner: {prizeAssignment.GuildMember.User.Username} (<@{prizeAssignment.GuildMember.User.Id}>)");
            }
        }

        static string GetConfigVariable(string key)
        {
            string? res = ConfigurationManager.AppSettings.Get(key);
            if (res == null)
            {
                throw new KeyNotFoundException(key);
            }
            return res;
        }
    }
}