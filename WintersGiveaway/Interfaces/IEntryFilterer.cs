using WintersGiveaway.Models;

namespace WintersGiveaway.Interfaces
{
    public interface IEntryFilterer
    {
        Task<IEnumerable<DiscordGuildMember>> GetEligibleGuildMembersAsync();
    }
}