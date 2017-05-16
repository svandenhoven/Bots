using Microsoft.Bot.Connector;
using System;

namespace BotSample
{
    /// <summary>
    /// The singleton instance of this class.
    /// </summary>

    [Serializable]
    internal class BotConversation
    {
        internal enum ConverstationStatusOptions
        {
            Active,
            Paused,
            OnHold
        }

        internal string ServiceUrl
        {
            get;
            private set;
        }

        internal string ChannelId
        {
            get;
            private set;
        }

        internal ChannelAccount ChannelAccount
        {
            get;
            private set;
        }

        internal ConversationAccount ConversationAccount
        {
            get;
            private set;
        }
        
        internal ConverstationStatusOptions ConversationStatus
        {
            get;
            set;
        }

        internal DateTime LastUpdated
        {
            get;
            set;
        }

        internal BotConversation(Activity activity)
        {
            ServiceUrl = activity.ServiceUrl;
            ChannelId = activity.ChannelId;
            ChannelAccount = activity.From;
            ConversationAccount = activity.Conversation;
            ConversationStatus = ConverstationStatusOptions.Active;
            LastUpdated = DateTime.Now;
        }

        internal BotConversation(string serviceUrl, string channelId, ChannelAccount channelAccount, ConversationAccount conversationAccount)
        {
            ServiceUrl = serviceUrl;
            ChannelId = channelId;
            ChannelAccount = channelAccount;
            ConversationAccount = conversationAccount;
            ConversationStatus = ConverstationStatusOptions.Active;
            LastUpdated = DateTime.Now;
        }
    }
}