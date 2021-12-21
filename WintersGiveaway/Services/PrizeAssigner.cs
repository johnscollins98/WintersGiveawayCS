using WintersGiveaway.Interfaces;
using WintersGiveaway.Models;

namespace WintersGiveaway.Services
{
    public class PrizeAssigner : IPrizeAssigner
    {
        private readonly IEntryFilterer entryFilterer;
        private readonly IRandom random;
        private readonly IDiscordGatherer discordGatherer;

        public PrizeAssigner(IEntryFilterer entryFilterer, IRandom random, IDiscordGatherer discordGatherer)
        {
            this.entryFilterer = entryFilterer;
            this.random = random;
            this.discordGatherer = discordGatherer;
        }

        public async Task<IEnumerable<PrizeAssignment>> GetPrizeAssignmentsAsync()
        {
            var prizes = await discordGatherer.GetPrizesAsync();
            var members = (await entryFilterer.GetEligibleGuildMembersAsync()).ToList();

            if (prizes.Count() > members.Count)
            {
                throw new ArgumentException("Error: There are more prizes than members");
            }

            var assignedIndices = new List<int>();
            var assignments = prizes
                .Select(prize =>
                {
                    bool assigned = false;
                    int memberIndex = -1;
                    while (!assigned)
                    {
                        memberIndex = random.Next(members.Count);
                        if (!assignedIndices.Contains(memberIndex))
                        {
                            assigned = true;
                            assignedIndices.Add(memberIndex);
                        }
                    }

                    return new PrizeAssignment()
                    {
                        Prize = prize,
                        GuildMember = members[memberIndex]
                    };
                });

            return assignments;
        }
    }
}
