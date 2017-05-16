using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using BotSample.MessageRouting;

namespace BotSample.Notifications
{
    public enum NotificationPriorityOptions
    {
        Low,
        Normal,
        High
    }

    [Serializable]
    public class Notification
    {
        public List<Party> PartiesToNotify
        {
            get;
            set;
        }

        public string Message
        {
            get;
            set;
        }

        public NotificationPriorityOptions Priority
        {
            get;
            set;
        }

        public Notification()
        {
            PartiesToNotify = new List<Party>();
            Message = string.Empty;
        }

        public string ToJsonString()
        {
            return JsonConvert.SerializeObject(this);
        }

        public static Notification FromJsonString(string jsonString)
        {
            return JsonConvert.DeserializeObject<Notification>(jsonString);
        }
    }
}