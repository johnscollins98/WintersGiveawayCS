namespace WintersGiveaway.Models
{
    public class PrizeAssignmentResult
    {
        public IEnumerable<PrizeAssignment> PrizeAssignments { get; set; }
        public IEnumerable<DiscordGuildMember> UnassignedMembers { get; set; }
    }
}