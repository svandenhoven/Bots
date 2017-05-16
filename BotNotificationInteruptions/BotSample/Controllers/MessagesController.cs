using Microsoft.Bot.Connector;
using BotSample.Dialogs;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.Bot.Builder.Dialogs;
using BotSample.Notifications;
using BotSample.MessageRouting;
using BotSample.DialogManager;
using System;

namespace BotSample
{
    [BotAuthentication]
    public class MessagesController : ApiController
    {
        /// <summary>
        /// POST: api/Messages
        /// Receive a message from a user and reply to it
        /// </summary>
        public async Task<HttpResponseMessage> Post([FromBody]Activity activity)
        {
            if (activity.Type == ActivityTypes.Message)
            {
                string notificationData = string.Empty;
                // Notitication is sent
                if (NotificationsManager.TryGetNotificationData(activity, out notificationData))
                {
                    // A notification related backchannel message was detected
                    Notification notification = Notification.FromJsonString(notificationData);

                    var dialogManager = DialogManager.Dialogs.GetInstance();

                    //notification is already the active dialog
                    if (dialogManager.IsActiveDialog(activity.Conversation.Id))
                    {
                        await Notifications.NotificationsManager.SendNotificationAsync(notification);
                    }
                    else
                    {
                        //if there is not active dialog
                        if (dialogManager.IsFirstDialog(activity.Conversation.Id) && !dialogManager.ActiveDialogExist())
                        {
                            await Notifications.NotificationsManager.SendNotificationAsync(notification);
                        }
                        else
                        {
                            if (notification.Priority < NotificationPriorityOptions.High)
                            {
                                dialogManager.StackDialog(activity, notification);
                                var responseMessage = new HttpResponseMessage(HttpStatusCode.OK);
                                return responseMessage;
                            }
                            else
                            {
                                var pausedDialog = dialogManager.PauseActiveDialog();
                                notification.Message = $"High Prio Message: {notification.Message}";
                                await Notifications.NotificationsManager.SendNotificationAsync(notification);
                                dialogManager.UnPauseActiveDialog(pausedDialog);
                            }
                        }
                    }
                }
                // Normal message is sent
                else
                {
                    // Normal Message, just sent it.
                    //Save sending party to Azure Table
                    MessageRouting.MessageRouterManager.Instance.StoreParties(activity);
                    await Conversation.SendAsync(activity, () => new RootDialog());
                }
            }
            else
            {
                HandleSystemMessage(activity);
            }
            var response = Request.CreateResponse(HttpStatusCode.OK);
            return response;
        }

        private Activity HandleSystemMessage(Activity message)
        {
            if (message.Type == ActivityTypes.DeleteUserData)
            {
                // Implement user deletion here
                // If we handle user deletion, return a real message
            }
            else if (message.Type == ActivityTypes.ConversationUpdate)
            {
                // Handle conversation state changes, like members being added and removed
                // Use Activity.MembersAdded and Activity.MembersRemoved and Activity.Action for info
                // Not available in all channels
            }
            else if (message.Type == ActivityTypes.ContactRelationUpdate)
            {
                // Handle add/remove from contact lists
                // Activity.From + Activity.Action represent what happened
            }
            else if (message.Type == ActivityTypes.Typing)
            {
                // Handle knowing tha the user is typing
            }
            else if (message.Type == ActivityTypes.Ping)
            {
            }


            return null;
        }
    }
}