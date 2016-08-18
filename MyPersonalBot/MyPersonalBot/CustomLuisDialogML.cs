using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Luis.Models;
using System.Globalization;
using Microsoft.Bot.Connector;
using System.Net.Http;
using Newtonsoft.Json;

namespace MyPersonalBot
{
    [LuisModel("a6395346-8677-46d0-a620-c35e2fd8acea", "94abce56901a476194c2f3d7c7c1f423")]
    public class CustomLuisDialogML : LuisDialog<object>
    {
        public string OriginalLanguage = "en";

        [LuisIntent("Greeting")]
        public async Task Greeting(IDialogContext context, LuisResult result)
        {
            
            string msg = "Hi, nice that you contacted the PA of Sander. What can I do for you?\n";
            msg += "You can ask me where Sander is and how the weather is at his location.";

            var translatedMsg = Translator.GetTransMsg(msg, OriginalLanguage);
            await context.PostAsync(translatedMsg);

            context.Wait(MessageReceived);
        }

        [LuisIntent("")]
        public async Task DidNotUnderstand(IDialogContext context, LuisResult result)
        {
            string msg = "Sorry I did not understand you. Please refrase.";
            var translatedMsg = Translator.GetTransMsg(msg, OriginalLanguage);
            await context.PostAsync(translatedMsg);
            context.Wait(MessageReceived);
        }

        [LuisIntent("MyPlace")]
        public async Task MyLocation(IDialogContext context, LuisResult result)
        {
            string location = await GetMyLocationAddress();
            string msg = $"The last known location of Sander is ";
            var translatedMsg = Translator.GetTransMsg(msg, OriginalLanguage) + $" '{location}'.";
            await context.PostAsync(translatedMsg);
            context.Wait(MessageReceived);
        }

        [LuisIntent("Make Appointment")]
        public async Task MakeAppointment(IDialogContext context, LuisResult result)
        {
            string msg = "Sorry, cannot respond yet on 'Make Appointment' intent.";
            var translatedMsg = Translator.GetTransMsg(msg, OriginalLanguage);
            await context.PostAsync(translatedMsg);
            context.Wait(MessageReceived);
        }

        [LuisIntent("GiveTask")]
        public async Task GiveTask(IDialogContext context, LuisResult result)
        {
            string msg = "Sorry, cannot respond yet on 'Make Appointment' intent.";
            var translatedMsg = Translator.GetTransMsg(msg, OriginalLanguage);
            await context.PostAsync(translatedMsg);
            context.Wait(MessageReceived);
        }

        [LuisIntent("LocalWeather")]
        public async Task LocalWeather(IDialogContext context, LuisResult result)
        {
            string city = await GetMyLocationRegion();
            var locInfo = await LocationInfo.GetLocationInfo(city);
            string msg = $"Current weather in {city} is {locInfo.weather[0].description}. It is now {locInfo.main.temp} C degrees.";

            var translatedMsg = Translator.GetTransMsg(msg, OriginalLanguage);
            await context.PostAsync(translatedMsg);
            context.Wait(MessageReceived);
        }

        private async static Task<string> GetMyLocationAddress()
        {
            var address = await LocationInfo.GetAddress();
            return $"{address.addressLine}, {address.adminDistrict2}";

        }

        private async static Task<string> GetMyLocationRegion()
        {
            var address = await LocationInfo.GetAddress();
            return address.adminDistrict2;

        }
    }
}