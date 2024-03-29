﻿namespace WintersGiveaway.Models
{
    public class Config
    {
        public string BotToken { get; set; }
        public string GuildId { get; set; }
        public string EntryChannelId { get; set; }
        public string PrizeChannelId { get; set; }
        public string EntryMessageId { get; set; }
        public string PrizeMessageId { get; set; }
        public string EntryEmoji { get; set; }
        public DateTime CutoffDate { get; set; }
    }
}
