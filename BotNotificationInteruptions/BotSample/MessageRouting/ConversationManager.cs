using System;
using System.Threading.Tasks;
using BotSample.Notifications;
using System.Collections.Generic;
using Microsoft.Bot.Connector;
using BotSample.Notifications;

namespace BotSample
{
    internal class ConversationManager
    {
        private static List<BotConversation> _botConversations;

        internal static List<BotConversation> BotConversations
        {
            get
            {
                if (_botConversations == null)
                {
                    _botConversations = new List<BotConversation>();
                }

                return _botConversations;
            }
        }


        internal static Task StackNotification(Notification notification)
        {
            return null;
        }
    }
}