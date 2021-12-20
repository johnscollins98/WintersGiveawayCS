using Newtonsoft.Json;
using WintersGiveaway.Models;

namespace WintersGiveaway
{
    public class Program
    {
        static async Task Main(string[] args)
        {
            var config = JsonConvert.DeserializeObject<Config>(File.ReadAllText("config.json"));

            var discordGatherer = new DiscordGatherer("https://discord.com/api/v9", config.BotToken, config.GuildId, config.ChannelId);
            var prizes = await discordGatherer.GetPrizes(config.PrizeMessageId);
            var guildMembers = await discordGatherer.GetDiscordGuildMembers();
            var usersWhoReacted = await discordGatherer.GetDiscordMessageReactions(config.EntryMessageId, "🎁");

            var entryFilterer = new EntryFilterer(usersWhoReacted, guildMembers);
            var eligibleMembers = entryFilterer.GetEligibleGuildMembers(config.CutoffDate);

            var prizeAssigner = new PrizeAssigner(prizes, eligibleMembers.ToList(), new RandomNumberGenerator());
            var prizeAssignments = prizeAssigner.GetPrizeAssignments();

            foreach (var prizeAssignment in prizeAssignments)
            {
                Console.WriteLine($"Prize: {prizeAssignment.Prize} " +
                    $"- Winner: {prizeAssignment.GuildMember.User.Username} (<@{prizeAssignment.GuildMember.User.Id}>)");
            }
        }
    }
}