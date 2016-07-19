using System.Collections.Generic;
using System.Diagnostics;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text.RegularExpressions;

namespace PokemonGoApi
{
    public static class AuthenticationService
    {
        public enum AuthenticationType
        {
            PTC = 0,
            Google = 1
        }
        struct SLTResponse
        {
            public string lt;
            public string execution;
        }

        static string szLogin = "https://sso.pokemon.com/sso/login?service=https%3A%2F%2Fsso.pokemon.com%2Fsso%2Foauth2.0%2FcallbackAuthorize";
        static string szLoginOAuth = "https://sso.pokemon.com/sso/oauth2.0/accessToken";

        //Heavily based on https://github.com/tejado/pokemongo-api-demo/blob/master/main.py#L129
        public static string pokemonClubLogin(string user, string password)
        {
            Debug.WriteLine("[!] Started login procces");
            var LTResponse = JsonConvert.DeserializeObject<SLTResponse>(
                HttpHelper.PostString(szLogin, null, new List<KeyValuePair<string, string>>()
                {
                    new KeyValuePair<string, string>("User-Agent", "niantic")
                }));
            Debug.WriteLine($"[!] LT Key : {LTResponse.lt} Execution : {LTResponse.execution}");

            var loginToken = HttpHelper.PostString(szLogin,
                HttpHelper.parseQuery(new List<KeyValuePair<string, string>>()
                {
                    new KeyValuePair<string, string>("lt", LTResponse.lt),
                    new KeyValuePair<string, string>("execution", LTResponse.execution),
                    new KeyValuePair<string, string>("_eventId", "submit"),
                    new KeyValuePair<string, string>("username", user),
                    new KeyValuePair<string, string>("password", password),

                }), new List<KeyValuePair<string, string>>()
                {
                   new KeyValuePair<string, string>("User-Agent", "niantic"),
                   new KeyValuePair<string, string>("Content-Type", "application/x-www-form-urlencoded")
                },
                "Location");

            if (loginToken == null) return "error";

            loginToken = loginToken.Substring(loginToken.LastIndexOf("=") + 1);
            Debug.WriteLine($"[!] loginToken : {loginToken}");

            var accessToken = HttpHelper.PostString(szLoginOAuth,
                HttpHelper.parseQuery(new List<KeyValuePair<string, string>>()
                {
                    new KeyValuePair<string, string>("client_id", "mobile-app_pokemon-go"),
                    new KeyValuePair<string, string>("redirect_uri", "https://www.nianticlabs.com/pokemongo/error"),
                    new KeyValuePair<string, string>("client_secret", "w8ScCUXJQc6kXKw8FiOhd8Fixzht18Dq3PEVkUCP5ZPxtgyWsbTvWHFLm2wNY0JR"),
                    new KeyValuePair<string, string>("grant_type", "refresh_token"),
                    new KeyValuePair<string, string>("code", loginToken)
                }), new List<KeyValuePair<string, string>>()
                {
                   new KeyValuePair<string, string>("User-Agent", "Niantic App"),
                   new KeyValuePair<string, string>("Content-Type", "application/x-www-form-urlencoded")
                });
            return accessToken.Substring(accessToken.IndexOf("=") + 1, accessToken.LastIndexOf("&") - accessToken.IndexOf("=") - 1);
        }
        public static string googleLogin(string user, string password)
        {
            Debug.WriteLine($"[!] Google login for: {user}");
            var first = "https://accounts.google.com/o/oauth2/auth?client_id=848232511240-73ri3t7plvk96pj4f85uj8otdat2alem.apps.googleusercontent.com&redirect_uri=urn%3Aietf%3Awg%3Aoauth%3A2.0%3Aoob&response_type=code&scope=openid%20email%20https%3A%2F%2Fwww.googleapis.com%2Fauth%2Fuserinfo.email";
            var second = "https://accounts.google.com/AccountLoginInfo";
            var third = "https://accounts.google.com/signin/challenge/sl/password";
            var last = "https://accounts.google.com/o/oauth2/token";
            using (var clientHandler = new HttpClientHandler())
            {
                clientHandler.AllowAutoRedirect = true;
                using (var client = new HttpClient(clientHandler))
                {
                    client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (iPad; CPU OS 8_4 like Mac OS X) AppleWebKit/600.1.4 (KHTML, like Gecko) Mobile/12H143");
                    var response = client.GetAsync(first).Result;
                    var r = response.Content.ReadAsStringAsync().Result;

                    var galx_regex = "name=\"GALX\" value=\"(.*?)\"";
                    var matches = Regex.Matches(r, galx_regex);
                    var galx = matches[0].Groups[1].Value;
                    var gxf_regex = "name=\"gxf\" value=\"(.*?)\"";
                    var gxf = Regex.Matches(r, gxf_regex)[0].Groups[1].Value;
                    var cont_regex = "name=\"continue\" value=\"(.*?)\"";
                    matches = Regex.Matches(r, cont_regex);
                    var cont = matches[0].Groups[1].Value.Replace("&amp;", "&");
                    var data1 = new[]
                    {
                        new KeyValuePair<string, string>("Page", "PasswordSeparationSignIn"),
                        new KeyValuePair<string, string>("GALX", galx),
                        new KeyValuePair<string, string>("gxf", gxf),
                        new KeyValuePair<string, string>("continue", cont),
                        new KeyValuePair<string, string>("ltmpl", "embedded"),
                        new KeyValuePair<string, string>("scc", "1"),
                        new KeyValuePair<string, string>("sarp", "1"),
                        new KeyValuePair<string, string>("oauth", "1"),
                        new KeyValuePair<string, string>("ProfileInformation", ""),
                        new KeyValuePair<string, string>("_utf8", "?"),
                        new KeyValuePair<string, string>("bgresponse", "js_disabled"),
                        new KeyValuePair<string, string>("Email", user),
                        new KeyValuePair<string, string>("signIn", "Next"),
                    };
                    response = client.PostAsync(second, new FormUrlEncodedContent(data1)).Result;
                    r = response.Content.ReadAsStringAsync().Result;
                    gxf = Regex.Matches(r, gxf_regex)[0].Groups[1].Value;
                    var profileinformation_regex = "name=\"ProfileInformation\" type=\"hidden\" value=\"(.*?)\"";
                    var profileinformation = Regex.Matches(r, profileinformation_regex)[0].Groups[1].Value;
                    var data2 = new[]
                    {
                        new KeyValuePair<string, string>("Page", "PasswordSeparationSignIn"),
                        new KeyValuePair<string, string>("GALX", galx),
                        new KeyValuePair<string, string>("gxf", gxf),
                        new KeyValuePair<string, string>("continue", cont),
                        new KeyValuePair<string, string>("ltmpl", "embedded"),
                        new KeyValuePair<string, string>("scc", "1"),
                        new KeyValuePair<string, string>("sarp", "1"),
                        new KeyValuePair<string, string>("oauth", "1"),
                        new KeyValuePair<string, string>("ProfileInformation", profileinformation),
                        new KeyValuePair<string, string>("_utf8", "?"),
                        new KeyValuePair<string, string>("bgresponse", "js_disabled"),
                        new KeyValuePair<string, string>("Email", user),
                        new KeyValuePair<string, string>("Passwd", password),
                        new KeyValuePair<string, string>("signIn", "Sign in"),
                        new KeyValuePair<string, string>("PersistentCookie", "yes"),
                    };
                    response = client.PostAsync(third, new FormUrlEncodedContent(data2)).Result;
                    r = response.Content.ReadAsStringAsync().Result;
                    var clientid = "848232511240-73ri3t7plvk96pj4f85uj8otdat2alem.apps.googleusercontent.com";
                    var statewrapper_regex = "name=\"state_wrapper\" value=\"(.*?)\"";
                    var statewrapper = Regex.Matches(r, statewrapper_regex)[0].Groups[1].Value;
                    var connect_approve_regex = "id=\"connect-approve\" action=\"(.*?)\"";
                    var connect_approve = Regex.Matches(r, connect_approve_regex)[0].Groups[1].Value.Replace("&amp;", "&");
                    var data3 = new[]
                    {
                        new KeyValuePair<string, string>("submit_access", "true"),
                        new KeyValuePair<string, string>("state_wrapper", statewrapper),
                        new KeyValuePair<string, string>("_utf8", "?"),
                        new KeyValuePair<string, string>("bgresponse", "js_disabled"),
                    };
                    response = client.PostAsync(connect_approve, new FormUrlEncodedContent(data3)).Result;
                    r = response.Content.ReadAsStringAsync().Result;
                    var code_regex = "id=\"code\" type=\"text\" readonly=\"readonly\" value=\"(.*?)\"";
                    var code = Regex.Matches(r, code_regex)[0].Groups[1].Value.Replace("&amp;", "&");
                    var data4 = new[]
                    {
                        new KeyValuePair<string, string>("client_id", clientid),
                        new KeyValuePair<string, string>("client_secret", "NCjF1TLi2CcY6t5mt0ZveuL7"),
                        new KeyValuePair<string, string>("code", code),
                        new KeyValuePair<string, string>("grant_type", "authorization_code"),
                        new KeyValuePair<string, string>("redirect_uri", "urn:ietf:wg:oauth:2.0:oob"),
                        new KeyValuePair<string, string>("scope", "openid email https://www.googleapis.com/auth/userinfo.email"),
                    };
                    response = client.PostAsync(last, new FormUrlEncodedContent(data4)).Result;
                    r = response.Content.ReadAsStringAsync().Result;
                    var jdata = Newtonsoft.Json.Linq.JObject.Parse(r);
                    return jdata["id_token"].ToString();
                }
            }
        }
    }
}
