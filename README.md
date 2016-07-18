# PokemonGoApi
An API developed in C# with the premise to interact with Pokémon GO server.
 * Use at your own risk!
 * Secondary accounts should be used.

## What's done so far
 * Server query backend
 * Pokémon Club login
 * Handshake
 * Retrieve profile status

## Example

    using System;
    
    namespace PokemonGoTestApp
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

## Credits
This project it's possible due the work from another people.
 * [/r/pokemongodev](https://www.reddit.com/r/pokemongodev)
 * [tejado](https://github.com/tejado/pokemongo-api-demo) (Pokémon Club login)
 * [Grover-c13](https://github.com/Grover-c13/PokeGOAPI-Java/) (Stolen .proto files)
