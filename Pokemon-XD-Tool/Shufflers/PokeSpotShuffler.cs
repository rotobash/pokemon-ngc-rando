using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XDCommon.PokemonDefinitions;

namespace Randomizer.Shufflers
{
    public struct PokeSpotShufflerSettings
    {
        public bool RandomizePokeSpotPokemon { get; set; }
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
        public static void ShufflePokeSpots(Random random, PokeSpotShufflerSettings settings, PokeSpotPokemon[] pokeSpotPokemon, ExtractedGame extractedGame)
        {
            var validPokemon = extractedGame.ValidPokemon;
            var validItems = settings.BanBadHeldItems ? extractedGame.GoodItems : extractedGame.NonKeyItems;

            foreach (var pokeSpotPoke in pokeSpotPokemon)
            {
                if (pokeSpotPoke.Pokemon == RandomizerConstants.BonslyIndex && settings.EasyBonsly)
                {
                    // I don't know if this'll work or not...
                    pokeSpotPoke.EncounterPercentage = 100;
                }

                if (settings.RandomizePokeSpotPokemon)
                {
                    pokeSpotPoke.Pokemon = (ushort)validPokemon[random.Next(0, validPokemon.Length)].Index;
                }

                if (settings.RandomizeHeldItems)
                {
                    extractedGame.PokemonList[pokeSpotPoke.Pokemon].HeldItem = (ushort)validItems[random.Next(0, validItems.Length)].Index;
                }

                if (settings.BoostPokeSpotLevel)
                {
                    var maxLevel = pokeSpotPoke.MaxLevel;
                    var minLevel = pokeSpotPoke.MinLevel;
                    var maxLevelIncrease = (byte)Math.Clamp(maxLevel + maxLevel * settings.BoostPokeSpotLevelPercent, 1, 100);
                    var minLevelIncrease = (byte)Math.Clamp(minLevel + minLevel * settings.BoostPokeSpotLevelPercent, 1, 100);
                    pokeSpotPoke.MaxLevel = maxLevelIncrease;
                    pokeSpotPoke.MinLevel = minLevelIncrease;
                }

                if (settings.SetMinimumCatchRate)
                {
                    var poke = extractedGame.PokemonList[pokeSpotPoke.Pokemon];
                    var catchRate = Math.Max(poke.CatchRate, settings.MinimumCatchRate);
                    var catchRateIncrease = (byte)Math.Clamp(catchRate, 0, byte.MaxValue);
                    poke.CatchRate = catchRateIncrease;
                }
            }
        }
    }
}
