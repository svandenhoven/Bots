using System;
using System.Configuration;
using System.Globalization;
using System.Threading.Tasks;
using System.Web;

namespace TranslatorConsole
{
    class Program
    {
        AzureDataMarket.Token bearer = new AzureDataMarket.Token();
        static void Main(string[] args)
        {
            var textIn = "stop";
            do
            {
                var bearer = Task.Run(GetBearerTokenForTranslator).Result;
                textIn = Console.ReadLine();
                if (!string.IsNullOrEmpty(textIn))
                {
                    var lang = Detect(textIn, bearer.Header);
                    var langName = new CultureInfo(lang).DisplayName;

                    var translatedText = Translate(textIn, lang, "en", bearer.Header);
                    Console.WriteLine($"See you speak in {langName}. Translated in English you said '{translatedText}'");

                    var responseMsg = "Sure thing! Your only limit is your imagination!";
                    var translatedResponse = Translate(responseMsg, "en", lang, bearer.Header);
                    Console.WriteLine($"Response text in English is '{responseMsg}'. In {langName} it is '{translatedResponse}'.\n");
                }
            } while(textIn!="stop");
        }

        private static string Detect(string txtToTranslate, string headerValue)
        {
            string uri = "http://api.microsofttranslator.com/v2/Http.svc/Detect?text=" + HttpUtility.UrlEncode(txtToTranslate);
            System.Net.WebRequest translationWebRequest = System.Net.WebRequest.Create(uri);
            translationWebRequest.Headers.Add("Authorization", headerValue);

            System.Net.WebResponse response = null;
            response = translationWebRequest.GetResponse();
            System.IO.Stream stream = response.GetResponseStream();
            System.Text.Encoding encode = System.Text.Encoding.GetEncoding("utf-8");

            System.IO.StreamReader translatedStream = new System.IO.StreamReader(stream, encode);
            System.Xml.XmlDocument xTranslation = new System.Xml.XmlDocument();
            xTranslation.LoadXml(translatedStream.ReadToEnd());

            return xTranslation.InnerText;
        }

        private static string Translate(string txtToTranslate, string from, string to, string headerValue)
        {

            string uri = "http://api.microsofttranslator.com/v2/Http.svc/Translate?text=" +
                 System.Web.HttpUtility.UrlEncode(txtToTranslate) + $"&to={to}";
            System.Net.WebRequest translationWebRequest = System.Net.WebRequest.Create(uri);
            translationWebRequest.Headers.Add("Authorization", headerValue);

            System.Net.WebResponse response = null;
            response = translationWebRequest.GetResponse();
            System.IO.Stream stream = response.GetResponseStream();
            System.Text.Encoding encode = System.Text.Encoding.GetEncoding("utf-8");

            System.IO.StreamReader translatedStream = new System.IO.StreamReader(stream, encode);
            System.Xml.XmlDocument xTranslation = new System.Xml.XmlDocument();
            xTranslation.LoadXml(translatedStream.ReadToEnd());

            return xTranslation.InnerText;
        }

        private async static Task<AzureDataMarket.Token> GetBearerTokenForTranslator()
        {
            string clientID = ConfigurationManager.AppSettings["TranslatorClientId"];
            string clientSecret = ConfigurationManager.AppSettings["TranslatorClientSecret"]; 
            var _Authentication = new AzureDataMarket(clientID, clientSecret);
            var m_Token = await _Authentication.GetTokenAsync();
            return m_Token;
        }
    }
}
