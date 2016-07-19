using System.Collections.Generic;
using System.Diagnostics;

namespace PokemonGoApi
{
    public static class AuthenticationService
    {
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
            SLTResponse LTResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<SLTResponse>(
                HttpHelper.PostString(szLogin, null, new List<KeyValuePair<string, string>>()
                {
                    new KeyValuePair<string, string>("User-Agent", "niantic")
                }));
            Debug.WriteLine("[!] LT Key : {0} Execution : {1}", LTResponse.lt, LTResponse.execution);

            string loginToken = HttpHelper.PostString(szLogin,
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
            Debug.WriteLine("[!] loginToken : {0}", loginToken, null);

            string accessToken = HttpHelper.PostString(szLoginOAuth,
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
    }
}
