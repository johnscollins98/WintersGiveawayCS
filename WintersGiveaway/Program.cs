using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using WintersGiveaway.Interfaces;
using WintersGiveaway.Models;
using WintersGiveaway.Services;

namespace WintersGiveaway
{
    public class Program
    {
        static async Task Main(string[] args)
        {
            IHost host = Host.CreateDefaultBuilder(args)
                .ConfigureServices((_, services) =>
                    services
                        .AddSingleton<IRandom, RandomNumberGenerator>()
                        .AddSingleton<IApiRequester, JsonApiRequester>()
                        .AddSingleton<IConfigManager>(s =>
                            new JsonFileConfigManager(new BasicFile("config.json")))
                        .AddSingleton<IDiscordGatherer, DiscordGatherer>()
                        .AddSingleton<IEntryFilterer, EntryFilterer>()
                        .AddSingleton<IPrizeAssigner, PrizeAssigner>()
                )
                .Build();

            var prizeAssigner = host.Services.GetRequiredService<IPrizeAssigner>();
            var prizeAssignments = await prizeAssigner.GetPrizeAssignmentsAsync();
            foreach (var prizeAssignment in prizeAssignments)
            {
                Console.WriteLine($"Prize: {prizeAssignment.Prize} " +
                    $"- Winner: {prizeAssignment.GuildMember.User.Username} (<@{prizeAssignment.GuildMember.User.Id}>)");
            }
        }
    }
}