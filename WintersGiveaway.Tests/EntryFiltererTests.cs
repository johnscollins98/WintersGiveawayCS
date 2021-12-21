using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WintersGiveaway.Interfaces;
using WintersGiveaway.Models;
using WintersGiveaway.Services;

namespace WintersGiveaway.Tests
{
    [TestClass]
    public class EntryFiltererTests
    {
        private List<DiscordUser> mockUsers;
        private List<DiscordGuildMember> mockGuildMembers;
        private DateTime dateInPast;
        private DateTime dateAfterPast;
        private Mock<IDiscordGatherer> mockDiscordGatherer;
        private Mock<IConfigManager> mockConfigManager;

        [TestInitialize]
        public void TestInitialize()
        {
            mockUsers = new List<DiscordUser>()
            {
                new DiscordUser() { Id = "0", Username = "User0" },
                new DiscordUser() { Id = "1", Username = "User1" },
                new DiscordUser() { Id = "2", Username = "User2" },
                new DiscordUser() { Id = "3", Username = "User3" },
            };

            dateInPast = new DateTime(2021, 1, 1);
            dateAfterPast = new DateTime(2022, 1, 1);

            mockGuildMembers = new List<DiscordGuildMember>()
            {
                new DiscordGuildMember() { Joined = dateInPast, User = mockUsers[0] },
                new DiscordGuildMember() { Joined = dateInPast, User = mockUsers[1] },
                new DiscordGuildMember() { Joined = dateInPast, User = mockUsers[2] },
                new DiscordGuildMember() { Joined = dateInPast, User = mockUsers[3] },
            };

            mockDiscordGatherer = new Mock<IDiscordGatherer>();
            mockDiscordGatherer.Setup(p => p.GetDiscordGuildMembersAsync()).ReturnsAsync(mockGuildMembers);
            mockDiscordGatherer.Setup(p => p.GetDiscordMessageReactionsAsync()).ReturnsAsync(mockUsers);

            mockConfigManager = new Mock<IConfigManager>();
            mockConfigManager.Setup(p => p.GetConfg()).Returns(new Config
            {
                CutoffDate = dateAfterPast
            });
        }

        [TestMethod]
        public async Task TestItOnlyIncludesUsersWhoReacted()
        {
            // Arrange
            var users = new List<DiscordUser>()
            {
                mockUsers[0], mockUsers[1]
            };

            var guildMembers = new List<DiscordGuildMember>()
            {
                mockGuildMembers[0], mockGuildMembers[1], mockGuildMembers[2]
            };

            mockDiscordGatherer.Setup(p => p.GetDiscordMessageReactionsAsync()).ReturnsAsync(users);
            mockDiscordGatherer.Setup(p => p.GetDiscordGuildMembersAsync()).ReturnsAsync(guildMembers);
            var entryFilterer = new EntryFilterer(mockDiscordGatherer.Object, mockConfigManager.Object);

            // Act
            var results = await entryFilterer.GetEligibleGuildMembersAsync();

            // Assert
            Assert.AreEqual(results.Count(), 2);
            Assert.IsTrue(results.Contains(guildMembers[0]));
            Assert.IsTrue(results.Contains(guildMembers[1]));
            Assert.IsFalse(results.Contains(guildMembers[2]));
        }

        [TestMethod]
        public async Task TestItFiltersNonGuildMembers()
        {
            // Arrange
            var users = new List<DiscordUser>()
            {
                mockUsers[0], mockUsers[1], mockUsers[2]
            };

            var guildMembers = new List<DiscordGuildMember>()
            {
                mockGuildMembers[0], mockGuildMembers[1]
            };

            mockDiscordGatherer.Setup(p => p.GetDiscordMessageReactionsAsync()).ReturnsAsync(users);
            mockDiscordGatherer.Setup(p => p.GetDiscordGuildMembersAsync()).ReturnsAsync(guildMembers);
            var entryFilterer = new EntryFilterer(mockDiscordGatherer.Object, mockConfigManager.Object);

            // Act
            var result = await entryFilterer.GetEligibleGuildMembersAsync();

            // Assert
            Assert.AreEqual(result.Count(), 2);
            Assert.IsTrue(result.Contains(guildMembers[0]));
            Assert.IsTrue(result.Contains(guildMembers[1]));
            Assert.IsFalse(result.Contains(mockGuildMembers[2]));
        }

        [TestMethod]
        public async Task TestIncludesMatchingDate()
        {
            // Arrange
            var users = new List<DiscordUser>()
            {
                mockUsers[0], mockUsers[1], mockUsers[2]
            };

            var guildMembers = new List<DiscordGuildMember>()
            {
                mockGuildMembers[0], mockGuildMembers[1], mockGuildMembers[2]
            };

            guildMembers[0].Joined = new DateTime(2022, 1, 1); // Same date

            mockDiscordGatherer.Setup(p => p.GetDiscordMessageReactionsAsync()).ReturnsAsync(users);
            mockDiscordGatherer.Setup(p => p.GetDiscordGuildMembersAsync()).ReturnsAsync(guildMembers);
            mockConfigManager.Setup(p => p.GetConfg()).Returns(new Config()
            {
                CutoffDate = new DateTime(2022, 1, 1)
            });

            var entityFilterer = new EntryFilterer(mockDiscordGatherer.Object, mockConfigManager.Object);

            // Act
            var result = await entityFilterer.GetEligibleGuildMembersAsync();

            // Assert
            Assert.AreEqual(result.Count(), 3);
            Assert.IsTrue(result.Contains(guildMembers[0]));
        }

        [TestMethod]
        public async Task TestIsFiltersDatesPastCutoff()
        {
            // Arrange
            var users = new List<DiscordUser>()
            {
                mockUsers[0], mockUsers[1], mockUsers[2]
            };

            var guildMembers = new List<DiscordGuildMember>()
            {
                mockGuildMembers[0], mockGuildMembers[1], mockGuildMembers[2]
            };

            guildMembers[0].Joined = new DateTime(2022, 1, 1, 1, 0, 0); // One hour after

            mockDiscordGatherer.Setup(p => p.GetDiscordMessageReactionsAsync()).ReturnsAsync(users);
            mockDiscordGatherer.Setup(p => p.GetDiscordGuildMembersAsync()).ReturnsAsync(guildMembers);
            mockConfigManager.Setup(p => p.GetConfg()).Returns(new Config()
            {
                CutoffDate = new DateTime(2022, 1, 1)
            });
            var entityFilterer = new EntryFilterer(mockDiscordGatherer.Object, mockConfigManager.Object);

            // Act
            var result = await entityFilterer.GetEligibleGuildMembersAsync();

            // Assert
            Assert.AreEqual(result.Count(), 2);
            Assert.IsFalse(result.Contains(guildMembers[0]));
        }
    }
}