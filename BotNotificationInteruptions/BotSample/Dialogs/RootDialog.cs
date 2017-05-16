using Microsoft.Bot.Builder.Dialogs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Bot.Connector;
using BotSample.MessageRouting;
using BotSample.DialogManager;

namespace BotSample.Dialogs
{
    [Serializable]
    public class RootDialog : IDialog<object>
    {
        public async Task StartAsync(IDialogContext context)
        {
            context.Wait(OnMessageReceivedAsync);
        }

        private async Task OnMessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            IMessageActivity messageActivity = await result;
            var dialogManager = DialogManager.Dialogs.GetInstance();

            if (messageActivity != null)
            {
                //start
                if (messageActivity.Text.ToLower() == @"\start")
                {                                     
                    if (!dialogManager.IsActiveDialog(messageActivity.Conversation.Id))
                    {
                        var party = new Party(messageActivity.ServiceUrl, messageActivity.ChannelId, messageActivity.From, messageActivity.Conversation);
                        var dialog = new Dialog(messageActivity.Conversation.Id, party);
                        dialogManager.Add(dialog);
                        await context.PostAsync($"A new dialog is started. You will not be interrupted during this dialog.");
                    }
                    else
                    {
                        await context.PostAsync($"There is already and ongoing dialog. Use '\\stop' to stop the current dialog.");
                    }
                }
                //reset
                else if(messageActivity.Text.ToLower() == @"\reset")
                {
                    dialogManager.Reset();
                    await context.PostAsync($"All dialogs resetted. List is empty.");
                }
                //stop
                else if(messageActivity.Text.ToLower() == @"\stop")
                {
                    if (!dialogManager.IsActiveDialog(messageActivity.Conversation.Id))
                    {
                        await context.PostAsync($"There is no active dialog.");
                    }
                    else
                    {
                        dialogManager.Remove(messageActivity.Conversation.Id);
                        await context.PostAsync($"The dialog is stopped. You can start a new one with '\\start'.");
                        var nextDialog = dialogManager.ReActivateNextDialog();
                        while (null != nextDialog)
                        {
                            await context.PostAsync($"The was an notification during last dialog that was paused. This is re-activated.");
                            await context.PostAsync($"The message was: {nextDialog.LastMessage}");
                            dialogManager.Remove(nextDialog);
                            nextDialog = dialogManager.ReActivateNextDialog();
                        }
                    }
                }
                //Other utterances
                else
                {
                    if (dialogManager.IsActiveDialog(messageActivity.Conversation.Id))
                    {
                        var dialog = dialogManager.UpdateDialog(messageActivity);
                        await context.PostAsync($"Message {dialog.MessageCount} in current dialog.");
                    }
                    else
                    {
                        await context.PostAsync($"No Active dialog exist. Use \\start to start a dialog.");
                    }
                }
            }
            else
            {
                await context.PostAsync("The received activity was null or not of type IMessageActivity!");
            }

            context.Done(new object());
        }
    }
}