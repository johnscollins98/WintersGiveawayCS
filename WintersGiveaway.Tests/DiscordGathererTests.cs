using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using WintersGiveaway.Interfaces;
using WintersGiveaway.Models;
using WintersGiveaway.Services;

namespace WintersGiveaway.Tests
{
    [TestClass]
    public class DiscordGathererTests
    {
        private Mock<IApiRequester> apiRequester;
        private Mock<IConfigManager> configManager;

        [TestInitialize]
        public void TestInitialize()
        {
            apiRequester = new Mock<IApiRequester>();
            configManager = new Mock<IConfigManager>();
            configManager.Setup(p => p.GetConfg()).Returns(new Config()
            {
                GuildId = "1",
                BotToken = "2",
                ChannelId = "3",
                CutoffDate = DateTime.UtcNow,
                EntryEmoji = "4",
                EntryMessageId = "5",
                PrizeMessageId = "6"
            });
        }

        [TestMethod]
        public async Task TestItGetsReactionsLessThan100()
        {
            // Arrange
            var usersWhoReacted = new List<DiscordUser>();
            for (int i = 0; i < 50; i++)
                usersWhoReacted.Add(new DiscordUser() { Id = i.ToString(), Username = $"User {i}" });

            apiRequester.Setup(p => p.MakeRequestAsync<IEnumerable<DiscordUser>>(It.IsAny<HttpRequestMessage>()))
                .ReturnsAsync(usersWhoReacted);

            var discordGatherer = new DiscordGatherer(apiRequester.Object, configManager.Object);

            // Act
            var result = await discordGatherer.GetDiscordMessageReactionsAsync();

            // Assert
            Assert.AreEqual(result.Count(), 50);
        }

        [TestMethod]
        public async Task TestItGetsReactionsMoreThan100()
        {
            // Arrange
            var usersWhoReacted = new List<DiscordUser>();
            for (int i = 0; i < 125; i++)
                usersWhoReacted.Add(new DiscordUser() { Id = i.ToString(), Username = $"User {i}" });

            apiRequester.SetupSequence(p => p.MakeRequestAsync<IEnumerable<DiscordUser>>(It.IsAny<HttpRequestMessage>()))
                .ReturnsAsync(usersWhoReacted.Take(100))
                .ReturnsAsync(usersWhoReacted.TakeLast(25));

            var discordGatherer = new DiscordGatherer(apiRequester.Object, configManager.Object);
            var config = configManager.Object.GetConfg();

            // Act
            var result = await discordGatherer.GetDiscordMessageReactionsAsync();

            // Assert
            Assert.AreEqual(result.Count(), 125);
            apiRequester.Verify(
                p => p.MakeRequestAsync<IEnumerable<DiscordUser>>(It.IsAny<HttpRequestMessage>()),
                Times.Exactly(2));

            var endpoint = $"https://discord.com/api/v9/channels/{config.ChannelId}/messages/{config.EntryMessageId}/reactions/{config.EntryEmoji}?limit=100&after=99";
            var msg = new HttpRequestMessage(HttpMethod.Get, endpoint);
            msg.Headers.Add("Authorization", $"Bot {config.BotToken}");
            apiRequester.Verify(
                p => p.MakeRequestAsync<IEnumerable<DiscordUser>>(It.Is<HttpRequestMessage>(p => p.RequestUri.ToString() == endpoint)), Times.Exactly(1));

        }

        [TestMethod]
        public async Task TestItCanGetPrizes()
        {
            // Arrange
            apiRequester.Setup(p => p.MakeRequestAsync<DiscordMessage>(It.IsAny<HttpRequestMessage>()))
                .ReturnsAsync(new DiscordMessage()
                {
                    Content = "Test\nPrize 1\nPrize 2\nPrize 3"
                });
            var discordGatherer = new DiscordGatherer(apiRequester.Object, configManager.Object);

            // Act
            var prizes = await discordGatherer.GetPrizesAsync();

            // Assert
            Assert.AreEqual(prizes.Count(), 3);
            Assert.AreEqual(prizes.ToList()[0], "Prize 1");

            var config = configManager.Object.GetConfg();
            var endpoint = $"https://discord.com/api/v9/channels/{config.ChannelId}/messages/{config.PrizeMessageId}";
            apiRequester.Verify(p => p.MakeRequestAsync<DiscordMessage>(It.Is<HttpRequestMessage>(p => p.RequestUri.ToString() == endpoint)), Times.Exactly(1));
        }

        [TestMethod]
        public async Task TestItGetsGuildMembers()
        {
            // Arrange
            var guildMembers = new List<DiscordGuildMember>()
            {
                new DiscordGuildMember() { Joined = DateTime.Now, User = new DiscordUser() { Id = "0", Username = "0" }},
                new DiscordGuildMember() { Joined = DateTime.Now, User = new DiscordUser() { Id = "1", Username = "1" }},
                new DiscordGuildMember() { Joined = DateTime.Now, User = new DiscordUser() { Id = "2", Username = "2" }}
            };
            apiRequester.Setup(p => p.MakeRequestAsync<IEnumerable<DiscordGuildMember>>(It.IsAny<HttpRequestMessage>()))
                .ReturnsAsync(guildMembers);
            var discordGatherer = new DiscordGatherer(apiRequester.Object, configManager.Object);

            // Act
            var results = await discordGatherer.GetDiscordGuildMembersAsync();

            // Assert
            Assert.AreEqual(results.Count(), guildMembers.Count());
            Assert.AreEqual(results.ToList()[0], guildMembers[0]);
            var config = configManager.Object.GetConfg();
            var endpoint = $"https://discord.com/api/v9/guilds/{config.GuildId}/members?limit=1000";
            apiRequester.Verify(p => p.MakeRequestAsync<IEnumerable<DiscordGuildMember>>(It.Is<HttpRequestMessage>(p => p.RequestUri.ToString() == endpoint)), Times.Exactly(1));
        }
    }
}
