using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PokemonGoApi;
using PokemonGoApi.Proto;

namespace Pogo
{
    class Program
    {
        static void Main(string[] args)
        {
            var goApi = new GoApi(AuthenticationService.pokemonClubLogin("PTC_ID", "PTC_PW"), AuthenticationService.AuthenticationType.PTC);
            var clientPlayerDetails = goApi.getPlayerDetails();
            Console.WriteLine($"[+] Username : {clientPlayerDetails.Username}");
            Console.WriteLine($"[+] Poke Storage : {clientPlayerDetails.PokeStorage}");
            Console.WriteLine($"[+] Item Storage : {clientPlayerDetails.ItemStorage}");
            Console.WriteLine($"[+] Account creation time : {clientPlayerDetails.CreationTime}");
            Console.ReadKey();
        }
    }
}
