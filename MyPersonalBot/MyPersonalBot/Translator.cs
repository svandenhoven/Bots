using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace MyPersonalBot
{
    internal class Translator
    {
        internal AzureDataMarket.Token bearer;
        internal Translator()
        {
            bearer = Task.Run(GetBearerTokenForTranslator).Result;
        }

        internal string Detect(string txtToTranslate)
        {
            string uri = "http://api.microsofttranslator.com/v2/Http.svc/Detect?text=" + HttpUtility.UrlEncode(txtToTranslate);
            System.Net.WebRequest translationWebRequest = System.Net.WebRequest.Create(uri);
            translationWebRequest.Headers.Add("Authorization", bearer.Header);

            System.Net.WebResponse response = null;
            response = translationWebRequest.GetResponse();
            System.IO.Stream stream = response.GetResponseStream();
            System.Text.Encoding encode = System.Text.Encoding.GetEncoding("utf-8");

            System.IO.StreamReader translatedStream = new System.IO.StreamReader(stream, encode);
            System.Xml.XmlDocument xTranslation = new System.Xml.XmlDocument();
            xTranslation.LoadXml(translatedStream.ReadToEnd());

            return xTranslation.InnerText;
        }

        internal string GetLanguageName(string language)
        {
            return new CultureInfo(language).DisplayName;
        }

        internal string Translate(string txtToTranslate, string from, string to)
        {

            string uri = "http://api.microsofttranslator.com/v2/Http.svc/Translate?text=" +
                 System.Web.HttpUtility.UrlEncode(txtToTranslate) + $"&to={to}";
            System.Net.WebRequest translationWebRequest = System.Net.WebRequest.Create(uri);
            translationWebRequest.Headers.Add("Authorization", bearer.Header);

            System.Net.WebResponse response = null;
            response = translationWebRequest.GetResponse();
            System.IO.Stream stream = response.GetResponseStream();
            System.Text.Encoding encode = System.Text.Encoding.GetEncoding("utf-8");

            System.IO.StreamReader translatedStream = new System.IO.StreamReader(stream, encode);
            System.Xml.XmlDocument xTranslation = new System.Xml.XmlDocument();
            xTranslation.LoadXml(translatedStream.ReadToEnd());

            return xTranslation.InnerText;
        }

        internal async Task<AzureDataMarket.Token> GetBearerTokenForTranslator()
        {
            string clientID = "replace wiht own";
            string clientSecret = "replace with own";
            var _Authentication = new AzureDataMarket(clientID, clientSecret);
            var m_Token = await _Authentication.GetTokenAsync();
            bearer = m_Token;
            return m_Token;
        }

        internal static string GetTransMsg(string msg, string to)
        {
            var langName = new CultureInfo(to).DisplayName;

            var translator = new Translator();
            var translatedMsg = $"{langName}: {translator.Translate(msg, "en", to)}";
            return translatedMsg;
        }

    }
}