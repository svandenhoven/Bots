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

namespace MyPersonalBot
{
    [LuisModel("<appid>", "<appkey>")]
    public class CustomLuisDialog : LuisDialog<object>
    {
        [LuisIntent("Greeting")]
        public async Task Greeting(IDialogContext context, LuisResult result)
        {

            string msg = "Hi, nice that you contacted me. Hope you're fine.";
            await context.PostAsync(msg);

            context.Wait(MessageReceived);
        }

        [LuisIntent("")]
        public async Task DidNotUnderstand(IDialogContext context, LuisResult result)
        {
            string msg = "Sorry I did not understand you. Please refrase.";
            await context.PostAsync(msg);
            context.Wait(MessageReceived);
        }

        [LuisIntent("MyPlace")]
        public async Task MyLocation(IDialogContext context, LuisResult result)
        {
            string city = GetMyLocation();
            string msg = $"Hi, I am in {city}.";
            await context.PostAsync(msg);
            context.Wait(MessageReceived);
        }

        [LuisIntent("LocalWeather")]
        public async Task LocalWeather(IDialogContext context, LuisResult result)
        {
            string city = GetMyLocation();
            var locInfo = await LocationInfo.GetLocationInfo(city);
            string msg = $"Current weather in {city} is {locInfo.weather[0].description}. It is now {locInfo.main.temp} C degrees.";

            await context.PostAsync(msg);
            context.Wait(MessageReceived);
        }

        private static string GetMyLocation()
        {
            return "Amsterdam";
        }
    }
}
