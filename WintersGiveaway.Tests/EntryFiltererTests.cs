using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using WintersGiveaway.Models;

namespace WintersGiveaway.Tests
{
    [TestClass]
    public class EntryFiltererTests
    {
        private List<DiscordUser> mockUsers;
        private List<DiscordGuildMember> mockGuildMembers;
        private DateTime dateInPast;
        private DateTime dateAfterPast;

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
        }

        [TestMethod]
        public void TestItOnlyIncludesUsersWhoReacted()
        {
            var users = new List<DiscordUser>()
            {
                mockUsers[0], mockUsers[1]
            };

            var guildMembers = new List<DiscordGuildMember>()
            {
                mockGuildMembers[0], mockGuildMembers[1], mockGuildMembers[2]
            };

            var entryFilterer = new EntryFilterer(users, guildMembers);
            var results = entryFilterer.GetEligibleGuildMembers(dateAfterPast);

            Assert.AreEqual(results.Count(), 2);
            Assert.IsTrue(results.Contains(guildMembers[0]));
            Assert.IsTrue(results.Contains(guildMembers[1]));
            Assert.IsFalse(results.Contains(guildMembers[2]));
        }

        [TestMethod]
        public void TestItFiltersNonGuildMembers()
        {
            var users = new List<DiscordUser>()
            {
                mockUsers[0], mockUsers[1], mockUsers[2]
            };

            var guildMembers = new List<DiscordGuildMember>()
            {
                mockGuildMembers[0], mockGuildMembers[1]
            };

            var entryFilterer = new EntryFilterer(users, guildMembers);
            var result = entryFilterer.GetEligibleGuildMembers(dateAfterPast);

            Assert.AreEqual(result.Count(), 2);
            Assert.IsTrue(result.Contains(guildMembers[0]));
            Assert.IsTrue(result.Contains(guildMembers[1]));
            Assert.IsFalse(result.Contains(mockGuildMembers[2]));
        }

        [TestMethod]
        public void TestIncludesMatchingDate()
        {
            var users = new List<DiscordUser>()
            {
                mockUsers[0], mockUsers[1], mockUsers[2]
            };

            var guildMembers = new List<DiscordGuildMember>()
            {
                mockGuildMembers[0], mockGuildMembers[1], mockGuildMembers[2]
            };

            // One hour after
            guildMembers[0].Joined = new DateTime(2022, 1, 1);

            var entityFilterer = new EntryFilterer(users, guildMembers);
            var result = entityFilterer.GetEligibleGuildMembers(new DateTime(2022, 1, 1));

            Assert.AreEqual(result.Count(), 3);
            Assert.IsTrue(result.Contains(guildMembers[0]));
        }

        [TestMethod]
        public void TestIsFiltersDatesPastCutoff()
        {
            var users = new List<DiscordUser>()
            {
                mockUsers[0], mockUsers[1], mockUsers[2]
            };

            var guildMembers = new List<DiscordGuildMember>()
            {
                mockGuildMembers[0], mockGuildMembers[1], mockGuildMembers[2]
            };

            // One hour after
            guildMembers[0].Joined = new DateTime(2022, 1, 1, 1, 0, 0);

            var entityFilterer = new EntryFilterer(users, guildMembers);
            var result = entityFilterer.GetEligibleGuildMembers(new DateTime(2022, 1, 1));

            Assert.AreEqual(result.Count(), 2);
            Assert.IsFalse(result.Contains(guildMembers[0]));
        }
    }
}