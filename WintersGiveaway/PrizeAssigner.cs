using WintersGiveaway.Models;

namespace WintersGiveaway
{
    public class PrizeAssigner
    {
        private readonly IEnumerable<string> prizes;
        private readonly IList<DiscordGuildMember> members;
        private readonly IRandom random;

        public PrizeAssigner(IEnumerable<string> prizes, IList<DiscordGuildMember> members, IRandom random)
        {
            this.prizes = prizes;
            this.members = members;
            this.random = random;
        }

        public IEnumerable<PrizeAssignment> GetPrizeAssignments()
        {
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
