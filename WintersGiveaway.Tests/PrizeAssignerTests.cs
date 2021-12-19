using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WintersGiveaway.Models;

namespace WintersGiveaway.Tests
{
    internal class MockRandomGenerator : IRandom
    {
        private readonly IList<int> mockValues;
        private int index;

        public MockRandomGenerator(IList<int> mockValues)
        {
            this.mockValues = mockValues;
            index = 0;
        }
        public int Next(int n)
        {
            var value = mockValues[index];
            index++;
            return value < n ? value : n - 1;
        }
    }

    [TestClass]
    public class PrizeAssignerTests
    {
        private List<string> mockPrizes;
        private List<DiscordGuildMember> mockMembers;

        [TestInitialize]
        public void TestInitialize()
        {
            mockPrizes = new List<string>() { "Prize0", "Prize1", "Prize2" };
            mockMembers = new List<DiscordGuildMember>()
            {
                new DiscordGuildMember()
                {
                    Joined = DateTime.Now,
                    User = new DiscordUser() { Id = "0", Username = "User0" }
                },

                new DiscordGuildMember()
                {
                    Joined = DateTime.Now,
                    User = new DiscordUser() { Id = "1", Username = "User1" }
                },

                new DiscordGuildMember()
                {
                    Joined = DateTime.Now,
                    User = new DiscordUser { Id = "2", Username = "User2" }
                }
            };
        }

        [TestMethod]
        public void TestCorrectPrizesAreAssigned()
        {
            var rng = new MockRandomGenerator(new List<int>() { 1, 0, 2 });
            var prizeAssigner = new PrizeAssigner(mockPrizes, mockMembers, rng);

            var result = prizeAssigner.GetPrizeAssignments().ToList();

            Assert.AreEqual(result.Count(), 3);

            Assert.AreEqual(result[0].Prize, mockPrizes[0]);
            Assert.AreEqual(result[1].Prize, mockPrizes[1]);
            Assert.AreEqual(result[2].Prize, mockPrizes[2]);

            Assert.AreEqual(result[0].GuildMember, mockMembers[1]);
            Assert.AreEqual(result[1].GuildMember, mockMembers[0]);
            Assert.AreEqual(result[2].GuildMember, mockMembers[2]);
        }

        [TestMethod]
        public void TestErrorIsThrownIfPrizeCannotBeAssigned()
        {
            var rng = new MockRandomGenerator(new List<int>() { 0, 0, 0, 0 });
            var prizeAssigner = new PrizeAssigner(mockPrizes.Take(2), mockMembers.Take(1).ToList(), rng);

            Assert.ThrowsException<ArgumentException>(() => prizeAssigner.GetPrizeAssignments());
        }

        [TestMethod]
        public void TestMemberCannotWinMultiplePrizes()
        {
            var rng = new MockRandomGenerator(new List<int>() { 0, 0, 0, 2 });
            var prizeAssigner = new PrizeAssigner(mockPrizes.Take(2), mockMembers, rng);
            var res = prizeAssigner.GetPrizeAssignments().ToList();

            Assert.AreEqual(res[0].GuildMember, mockMembers[0]);
            Assert.AreEqual(res[1].GuildMember, mockMembers[2]);
            Assert.AreEqual(res.Count, 2);
        }
    }
}
