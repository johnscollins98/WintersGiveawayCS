using WintersGiveaway.Models;

namespace WintersGiveaway.Interfaces
{
    public interface IPrizeAssigner
    {
        Task<IEnumerable<PrizeAssignment>> GetPrizeAssignmentsAsync();
    }
}