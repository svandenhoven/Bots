using Microsoft.Bot.Connector;
using BotSample.MessageRouting;
using BotSample.Notifications;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BotSample.DialogManager
{
    public enum DialogStatusStatusOptions
    {
        Active,
        Paused,
        OnHold
    }

    public class Dialog
    {
        public Dialog()
        { }

        public Dialog(string ConversationId, Party party)
        {
            DialogId = ConversationId;
            DialogStatus = DialogStatusStatusOptions.Active;
            Party = party;
            MessageCount = 1;
            LastUpdated = DateTime.Now;
        }

        public string DialogId { get; set; }
        public DialogStatusStatusOptions DialogStatus { get; set; }
        public Party Party { get; set; }
        public int MessageCount { get; set; }
        public DateTime LastUpdated { get; set; }
        public string LastMessage {get; set;}
    }

    public class Dialogs
    {
        private static Dialogs _dialogs = null;
        public static List<Dialog> _list = null;
        
        private Dialogs()
        {
            _list = new List<Dialog>();
        }

        //This is the factory method
        public static Dialogs GetInstance()
        {
            return _dialogs ?? (_dialogs = new Dialogs());
        }

        public void Add(Dialog item)
        {
            _list.Add(item);
        }

        public void StackDialog(Activity activity, Notification notification)
        {
            var party = new Party(activity.ServiceUrl, activity.ChannelId, activity.From, activity.Conversation);
            var dialog = new Dialog();
            dialog.DialogId = activity.Conversation.Id;
            dialog.DialogStatus = DialogStatusStatusOptions.Paused;
            dialog.Party = party;
            dialog.MessageCount = 1;
            dialog.LastMessage = notification.Message;
            dialog.LastUpdated = DateTime.Now;
            _list.Add(dialog);
        }

        public void Remove(string Id)
        {
            if (_list.Where(d => d.DialogId == Id).Count() > 0)
            {
                var dialog = _list.Where(d => d.DialogId == Id).First();
                _list.Remove(dialog);
            }
        }

        public void Remove(Dialog dialog)
        {
            _list.Remove(dialog);
        }

        public void Reset()
        {
            _list.Clear();
        }

        public Dialog ReActivateNextDialog()
        {
            if (_list.OrderBy(d => d.LastUpdated).Count() > 0)
            {
                var dialog = _list.OrderBy(d => d.LastUpdated).First();
                Dialog newDialog = CloneDialog(dialog);
                _list.Remove(dialog);
                newDialog.DialogStatus = DialogStatusStatusOptions.Active;
                _list.Add(newDialog);
                return newDialog;
            }
            return null;
        }

        internal bool ActiveDialogExist()
        {
            var result = _list.Where(d => d.DialogStatus == DialogStatusStatusOptions.Active).Count() > 0;
            return result;
        }

        public bool IsFirstDialog(string Id)
        {
            var result = _list.Where(d => d.DialogId == Id).Count() == 0;
            return result;
        }

        public bool IsActiveDialog(string Id)
        {
            var result = _list.Where(d => d.DialogId == Id && d.DialogStatus == DialogStatusStatusOptions.Active).Count() > 0;
            return result;
        }

        internal Dialog UpdateDialog(IMessageActivity messageActivity)
        {
            if (_list.Where(d => d.DialogId == messageActivity.Conversation.Id).Count() > 0)
            {
                var dialog = _list.Where(d => d.DialogId == messageActivity.Conversation.Id).First();
                Dialog newDialog = CloneDialog(dialog);
                _list.Remove(dialog);
                newDialog.MessageCount++;
                newDialog.LastMessage = messageActivity.Text;
                _list.Add(newDialog);
                return newDialog;
            }
            else
            {
                throw new Exception($"Dialog Id {messageActivity.Conversation.Id} Not Found");
            }

        }

        internal Dialog PauseActiveDialog()
        {
            var dialog = _list.Where(d => d.DialogStatus == DialogStatusStatusOptions.Active).First();
            Dialog newDialog = CloneDialog(dialog);
            _list.Remove(dialog);
            newDialog.DialogStatus = DialogStatusStatusOptions.Paused;
            _list.Add(newDialog);
            return newDialog;
        }

        internal Dialog UnPauseActiveDialog(Dialog dialog)
        {
            var index = _list.IndexOf(dialog);
            Dialog newDialog = _list[index];
            _list.Remove(dialog);
            newDialog.DialogStatus = DialogStatusStatusOptions.Active;
            _list.Add(newDialog);
            return newDialog;
        }

        private static Dialog CloneDialog(Dialog dialog)
        {
            return new Dialog { DialogId = dialog.DialogId, DialogStatus = dialog.DialogStatus, MessageCount = dialog.MessageCount, Party = dialog.Party, LastUpdated = DateTime.Now, LastMessage = dialog.LastMessage };
        }
    }


}

