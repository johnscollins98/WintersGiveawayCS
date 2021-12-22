using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WintersGiveaway.Interfaces;
using WintersGiveaway.Services;

namespace WintersGiveaway.Tests
{
    [TestClass]
    public class JsonFileConfigManagerTests
    {
        private Mock<IFile> mockFile;

        [TestInitialize]
        public void TestInitialize()
        {
            mockFile = new Mock<IFile>();
            mockFile.Setup(p => p.GetText()).Returns(
                @"{
                    ""botToken"": ""1"",
                    ""channelId"": ""2"",
                    ""guildId"": ""3"",
                    ""prizeMessageId"": ""4"",
                    ""entryMessageId"":  ""5"",
                    ""cutoffDate"": ""2021-12-12T00:00:00+0000"",
                    ""entryEmoji"":  ""🎁""
                }"
            );
        }

        [TestMethod]
        public void TestItSuccessfullyParsesJsonString()
        {
            // Arrange
            var jsonFileConfigManager = new JsonFileConfigManager(mockFile.Object);

            // Act
            var config = jsonFileConfigManager.GetConfg();

            // Assert
            Assert.AreEqual(config.BotToken, "1");
            Assert.AreEqual(config.ChannelId, "2");
            Assert.AreEqual(config.GuildId, "3");
            Assert.AreEqual(config.PrizeMessageId, "4");
            Assert.AreEqual(config.EntryMessageId, "5");
            Assert.AreEqual(config.CutoffDate, new DateTime(2021, 12, 12));
            Assert.AreEqual(config.EntryEmoji, "🎁");
        }

        [TestMethod]
        public void TestItUsesCache()
        {
            // Arrange
            var jsonConfigManager = new JsonFileConfigManager(mockFile.Object);

            // Act
            jsonConfigManager.GetConfg();
            jsonConfigManager.GetConfg();

            // Assert
            mockFile.Verify(p => p.GetText(), Times.Exactly(1));
        }

        [TestMethod]
        public void TestHowItHandlesIncompleteConfig()
        {
            // Arrange
            mockFile.Setup(p => p.GetText()).Returns(
                @"{
                }"
            );
            var jsonConfigManager = new JsonFileConfigManager(mockFile.Object);

            // Act
            var config = jsonConfigManager.GetConfg();

            // Assert
            Assert.AreEqual(config.BotToken, null);
            Assert.AreEqual(config.ChannelId, null);
            Assert.AreEqual(config.GuildId, null);
            Assert.AreEqual(config.PrizeMessageId, null);
            Assert.AreEqual(config.EntryMessageId, null);
            Assert.AreEqual(config.CutoffDate, new DateTime());
            Assert.AreEqual(config.EntryEmoji, null);
        }
}
}
