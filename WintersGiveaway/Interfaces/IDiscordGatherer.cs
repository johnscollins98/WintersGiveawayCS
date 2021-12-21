using WintersGiveaway.Models;

namespace WintersGiveaway.Interfaces
{
    public interface IDiscordGatherer
    {
        Task<IEnumerable<DiscordGuildMember>> GetDiscordGuildMembers();
        Task<IEnumerable<DiscordUser>> GetDiscordMessageReactions();
        Task<IEnumerable<string>> GetPrizes();
    }
}