using WintersGiveaway.Models;

namespace WintersGiveaway.Interfaces
{
    public interface IDiscordGatherer
    {
        Task<IEnumerable<DiscordGuildMember>> GetDiscordGuildMembersAsync();
        Task<IEnumerable<DiscordUser>> GetDiscordMessageReactionsAsync();
        Task<IEnumerable<string>> GetPrizesAsync();
    }
}