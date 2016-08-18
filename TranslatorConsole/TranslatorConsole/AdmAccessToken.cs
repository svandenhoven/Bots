using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TranslatorConsole
{
    internal class AdmAccessToken
    {
        internal string access_token { get; set; }
        internal string token_type { get; set; }
        internal string expires_in { get; set; }
        internal string scope { get; set; }
    }
}
