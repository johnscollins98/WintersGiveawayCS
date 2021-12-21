using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WintersGiveaway.Interfaces;
using WintersGiveaway.Models;
using WintersGiveaway.Services;

namespace WintersGiveaway.Tests
{
    [TestClass]
    public class PrizeAssignerTests
    {
        private List<string> mockPrizes;
        private List<DiscordGuildMember> mockMembers;
        private Mock<IDiscordGatherer> mockDiscordGatherer;
        private Mock<IRandom> mockRandomGenerator;
        private Mock<IEntryFilterer> mockEntryFilterer;

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

            mockDiscordGatherer = new Mock<IDiscordGatherer>();
            mockDiscordGatherer.Setup(p => p.GetPrizesAsync()).ReturnsAsync(mockPrizes);

            mockEntryFilterer = new Mock<IEntryFilterer>();
            mockEntryFilterer.Setup(p => p.GetEligibleGuildMembersAsync()).ReturnsAsync(mockMembers);

            mockRandomGenerator = new Mock<IRandom>();
        }

        [TestMethod]
        public async Task TestCorrectPrizesAreAssigned()
        {
            // Arrange
            mockRandomGenerator.SetupSequence(p => p.Next(3))
                .Returns(1)
                .Returns(0)
                .Returns(2);

            var prizeAssigner = new PrizeAssigner(
                mockEntryFilterer.Object, mockRandomGenerator.Object, mockDiscordGatherer.Object);

            // Act
            var result = (await prizeAssigner.GetPrizeAssignmentsAsync()).ToList();

            // Assert
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
            // Arrange
            mockDiscordGatherer.Setup(p => p.GetPrizesAsync()).ReturnsAsync(mockPrizes.Take(2));
            mockEntryFilterer.Setup(p => p.GetEligibleGuildMembersAsync()).ReturnsAsync(mockMembers.Take(1));
            var prizeAssigner = new PrizeAssigner(
                mockEntryFilterer.Object, mockRandomGenerator.Object, mockDiscordGatherer.Object);

            // Act/Assert
            Assert.ThrowsExceptionAsync<ArgumentException>(() => prizeAssigner.GetPrizeAssignmentsAsync());
        }

        [TestMethod]
        public async Task TestMemberCannotWinMultiplePrizes()
        {
            // Arrange
            mockRandomGenerator.SetupSequence(p => p.Next(3))
                .Returns(0)
                .Returns(0)
                .Returns(0)
                .Returns(2);
            mockDiscordGatherer.Setup(p => p.GetPrizesAsync()).ReturnsAsync(mockPrizes.Take(2));

            var prizeAssigner = new PrizeAssigner(
                mockEntryFilterer.Object, mockRandomGenerator.Object, mockDiscordGatherer.Object);

            // Act
            var res = (await prizeAssigner.GetPrizeAssignmentsAsync()).ToList();

            // Assert
            Assert.AreEqual(res[0].GuildMember, mockMembers[0]);
            Assert.AreEqual(res[1].GuildMember, mockMembers[2]);
            Assert.AreEqual(res.Count, 2);
        }
    }
}
