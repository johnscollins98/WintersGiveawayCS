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
            foreach (var prizeAssignment in prizeAssignments.PrizeAssignments)
            {
                Console.WriteLine($"Prize: {prizeAssignment.Prize.Replace("*", "").Trim()} " +
                    $"- Winner: {prizeAssignment.GuildMember.User.Username} (<@{prizeAssignment.GuildMember.User.Id}>)");
            }

            Console.WriteLine("");
            Console.WriteLine("People who didn't win:");
            Console.WriteLine("");

            foreach (var remaining in prizeAssignments.UnassignedMembers)
            {
                Console.WriteLine($"{remaining.User.Username} (<@{remaining.User.Id}>)");
            }
        }
    }
}