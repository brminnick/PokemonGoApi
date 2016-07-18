using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pogo
{
    class Program
    {
        static void Main(string[] args)
        {
            PokemonGoApi.GoApi goApi = new PokemonGoApi.GoApi(PokemonGoApi.AuthenticationService.pokemonClubLogin("PTC_ID", "PTC_PW"));
            PokemonGoApi.Proto.ClientPlayerDetails clientPlayerDetails = goApi.getPlayerDetails();
            Console.WriteLine("[+] Username : {0}", clientPlayerDetails.Username);
            Console.WriteLine("[+] Poke Storage : {0}", clientPlayerDetails.PokeStorage);
            Console.WriteLine("[+] Item Storage : {0}", clientPlayerDetails.ItemStorage);
            Console.WriteLine("[+] Account creation time : {0}", clientPlayerDetails.CreationTime);
            Console.ReadKey();
        }
    }
}
