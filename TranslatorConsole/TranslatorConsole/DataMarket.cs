using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TranslatorConsole
{
    class AzureDataMarket
    {
        readonly string m_ClientId;
        readonly string m_ClientSecret;
        readonly string m_Request;
        readonly string DATAMARKET_ACCESS_URI = "https://datamarket.accesscontrol.windows.net/v2/OAuth2-13";

        public AzureDataMarket(string clientId, string clientSecret)
        {
            this.m_ClientId = Uri.EscapeDataString(clientId);
            this.m_ClientSecret = Uri.EscapeDataString(clientSecret);
            this.m_Request = string.Format("grant_type=client_credentials&client_id={0}&client_secret={1}&scope=http://api.microsofttranslator.com", m_ClientId, m_ClientSecret);
        }

        public async Task<Token> GetTokenAsync()
        {
            // build request
            var _Request = System.Net.WebRequest.Create(DATAMARKET_ACCESS_URI);
            _Request.ContentType = "application/x-www-form-urlencoded";
            _Request.Method = "POST";

            // make request
            var _Bytes = Encoding.UTF8.GetBytes(this.m_Request);
            using (var _Stream = await _Request.GetRequestStreamAsync())
                _Stream.Write(_Bytes, 0, _Bytes.Length);

            // deserialize response
            try
            {
                using (var _Response = await _Request.GetResponseAsync())
                {
                    var _Serializer = new System.Runtime.Serialization.Json.DataContractJsonSerializer(typeof(Token));
                    var _Token = await Task.Run(() => (Token)_Serializer.ReadObject(_Response.GetResponseStream()));
                    return _Token;
                }
            }
            catch (Exception)
            {
                System.Diagnostics.Debugger.Break();
                throw;
            }
        }

        [System.Runtime.Serialization.DataContract]
        public class Token
        {
            [System.Runtime.Serialization.DataMember]
            public string access_token { get; set; }

            [System.Runtime.Serialization.DataMember]
            public string token_type { get; set; }

            private int m_expires_in;
            [System.Runtime.Serialization.DataMember]
            public int expires_in
            {
                get { return m_expires_in; }
                set
                {
                    m_expires_in = value;
                    ExpirationDate = DateTime.Now.AddSeconds(value);
                }
            }

            [System.Runtime.Serialization.DataMember]
            public string scope { get; set; }

            private DateTime ExpirationDate = DateTime.MinValue;
            public bool IsExpired { get { return ExpirationDate < DateTime.Now; } }

            public string Header
            {
                get { return string.Format("Bearer {0}", access_token); }
            }
        }
    }
}