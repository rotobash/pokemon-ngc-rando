using System;
using System.Linq;
using XDCommon.Contracts;
using XDCommon.Utility;

namespace XDCommon.PokemonDefinitions
{
    public interface IGameExtractor
    {
        ISO ISO { get; }
        ITrainerPool[] ExtractPools(Pokemon[] pokemon, Move[] moves);
        Ability[] ExtractAbilities();
        Items[] ExtractItems();
        OverworldItem[] ExtractOverworldItems();
        Pokemarts[] ExtractPokemarts();
        Move[] ExtractMoves();
        Pokemon[] ExtractPokemon();
        IGiftPokemon[] ExtractGiftPokemon();
        Area[] ExtractAreas();
        Room[] ExtractRooms();
        BattleField[] ExtractBattleFields();
        Battle[] ExtractBattles();
    }
}
