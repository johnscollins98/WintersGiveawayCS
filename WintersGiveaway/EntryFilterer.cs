using WintersGiveaway.Models;

namespace WintersGiveaway
{
    public class EntryFilterer
    {
        private readonly IEnumerable<DiscordUser> usersWhoReacted;
        private readonly IEnumerable<DiscordGuildMember> guildMembers;

        public EntryFilterer(IEnumerable<DiscordUser> usersWhoReacted, IEnumerable<DiscordGuildMember> guildMembers)
        {
            this.usersWhoReacted = usersWhoReacted;
            this.guildMembers = guildMembers;
        }

        public IEnumerable<DiscordGuildMember> GetEligibleGuildMembers(DateTime cutoff)
        {
            return usersWhoReacted
                .Where(user => guildMembers.Any(g => g.User.Id == user.Id))
                .Select(user => guildMembers.First(g => g.User.Id == user.Id))
                .Where(guildMember => guildMember.Joined <= cutoff);
        }
    }
}
