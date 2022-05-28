﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XDCommon;
using XDCommon.PokemonDefinitions;

namespace Randomizer.Shufflers
{
    public struct PokeSpotShufflerSettings
    {
        public bool RandomizePokeSpotPokemon { get; set; }
        public bool UseSimilarBSTs { get; set; }
        public bool RandomizeHeldItems { get; set; }
        public bool BanBadHeldItems { get; set; }
        public bool EasyBonsly { get; set; }
        public bool SetMinimumCatchRate { get; set; }
        public int MinimumCatchRate { get; set; }
        public bool BoostPokeSpotLevel { get; set; }
        public float BoostPokeSpotLevelPercent { get; set; }
    }

    public static class PokeSpotShuffler
    {
        public static void ShufflePokeSpots(ShuffleSettings shuffleSettings, PokeSpotPokemon[] pokeSpotPokemon)
        {
            var settings = shuffleSettings.RandomizerSettings.PokeSpotShufflerSettings;
            var extractedGame = shuffleSettings.ExtractedGame;
            var random = shuffleSettings.RNG;

            var validPokemon = extractedGame.ValidPokemon;
            var validItems = settings.BanBadHeldItems ? extractedGame.GoodItems : extractedGame.NonKeyItems;

            Logger.Log("=============================== Poke Spots ===============================\n\n");
            foreach (var pokeSpotPoke in pokeSpotPokemon)
            {
                Logger.Log($"PokeSpot Type: {pokeSpotPoke.PokeSpot.PokeSpotType}\n\n");
                var pokemon = extractedGame.PokemonList[pokeSpotPoke.Pokemon];
                if (pokeSpotPoke.Pokemon == RandomizerConstants.BonslyIndex && settings.EasyBonsly)
                {
                    // I don't know if this'll work or not...
                    pokeSpotPoke.EncounterPercentage = 100;
                    Logger.Log("Bonsly damnit stop running away!\n");
                    continue;
                }

                if (settings.RandomizePokeSpotPokemon)
                {
                    Pokemon[] pokeFilter = validPokemon;
                    if (settings.UseSimilarBSTs)
                    {
                        var similarStrengthPoke = pokeSpotPoke.Pokemon;
                        pokeFilter = Helpers.GetSimilarBsts(similarStrengthPoke, validPokemon.AsEnumerable(), extractedGame.PokemonList).ToArray();
                    }

                    var newPokemon = pokeFilter[random.Next(pokeFilter.Length)];
                    Logger.Log($"Pokemon {pokemon.Name} -> {newPokemon.Name}\n");
                    pokeSpotPoke.Pokemon = (ushort)newPokemon.Index;
                    pokemon = newPokemon;
                }

                if (settings.RandomizeHeldItems)
                {
                    var newItem = validItems[random.Next(0, validItems.Length)];
                    Logger.Log($"Pokemon {pokemon.HeldItem} -> {newItem.Name}\n");
                    pokemon.HeldItem = (ushort)newItem.Index;
                }

                if (settings.BoostPokeSpotLevel)
                {
                    var maxLevel = pokeSpotPoke.MaxLevel;
                    var minLevel = pokeSpotPoke.MinLevel;
                    var maxLevelIncrease = (byte)Math.Clamp(maxLevel + maxLevel * settings.BoostPokeSpotLevelPercent, 1, 100);
                    var minLevelIncrease = (byte)Math.Clamp(minLevel + minLevel * settings.BoostPokeSpotLevelPercent, 1, 100);
                    Logger.Log($"Max Level {maxLevel} -> {maxLevelIncrease}\n");
                    Logger.Log($"Min Level {minLevel} -> {minLevelIncrease}\n");
                    pokeSpotPoke.MaxLevel = maxLevelIncrease;
                    pokeSpotPoke.MinLevel = minLevelIncrease;
                }

                if (settings.SetMinimumCatchRate)
                {
                    var catchRate = Math.Max(pokemon.CatchRate, settings.MinimumCatchRate);
                    var catchRateIncrease = (byte)Math.Clamp(catchRate, 0, byte.MaxValue);
                    Logger.Log($"Catch Rate {pokemon.CatchRate} -> {catchRateIncrease}\n");
                    pokemon.CatchRate = catchRateIncrease;
                }
            }
        }
    }
}
