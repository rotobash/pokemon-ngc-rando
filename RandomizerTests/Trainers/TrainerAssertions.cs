using Newtonsoft.Json;
using XDCommon;
using XDCommon.Shufflers;
using RandomizerTests.ReferenceData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XDCommon.PokemonDefinitions;

namespace RandomizerTests.Trainers
{
    public static class TrainerAssertions
    {
        public static List<TrainerReference> TrainerReference = JsonConvert.DeserializeObject<List<TrainerReference>>(File.ReadAllText("ReferenceData\\JSON\\trainers.json")) ?? new List<TrainerReference>();

        public static bool AssertNoDuplicates(ITrainerPool[] trainerPools)
        {
            var shadowHashSet = new HashSet<int>();
            var randomizedPokemon = new HashSet<int>();

            foreach (var pool in trainerPools)
            {
                foreach (var trainer in pool.AllTrainers)
                {
                    foreach (var pokemon in trainer.Pokemon)
                    {
                        if (randomizedPokemon.Contains(pokemon.Index)) continue;

                        if (pokemon.IsSet && pokemon.IsShadow)
                        {
                            Assert.That(shadowHashSet, Does.Not.Contain(pokemon.Pokemon));
                            shadowHashSet.Add(pokemon.Pokemon);
                        }
                        randomizedPokemon.Add(pokemon.Index);
                    }
                }
            }

            return true;
        }

        public static bool AssertLevelBoost(ITrainerPool[] trainerPools, Settings settings)
        {
            var boostAmount = settings.TeamShufflerSettings.BoostTrainerLevel ? settings.TeamShufflerSettings.BoostTrainerLevelPercent : 0;
            var randomizedPokemon = new HashSet<int>();
            foreach (var pool in trainerPools)
            {
                foreach (var trainer in pool.AllTrainers)
                {
                    if (!trainer.IsSet) continue;

                    var trainerReference = TrainerReference.Find(t => t.TeamType == pool.TeamType && t.Index == trainer.Index);
                    if (trainerReference == null)
                        return false;

                    for (int i = 0; i < trainer.Pokemon.Length; i++)
                    {
                        var pokemon = trainer.Pokemon[i];
                        if (randomizedPokemon.Contains(pokemon.Index)) continue;

                        var pokeReference = trainerReference.Pokemon[i];
                        if (pokemon.IsSet)
                        {
                            var expectedLevel = (byte)Math.Round(Math.Clamp(pokeReference.Level + pokeReference.Level * boostAmount, 1, 100), MidpointRounding.AwayFromZero);
                            Assert.That(pokemon.Level, Is.EqualTo(expectedLevel));

                            if (pokemon.IsShadow && pokemon is IShadowPokemon shadowPokemon)
                            {
                                var expectedShadowLevel = (byte)Math.Round(Math.Clamp(pokeReference.ShadowLevel + pokeReference.ShadowLevel * boostAmount, 1, 100), MidpointRounding.AwayFromZero);
                                Assert.That(shadowPokemon.ShadowLevel, Is.EqualTo(expectedShadowLevel));
                            }

                            randomizedPokemon.Add(pokemon.Index);
                        }
                    }
                }
            }

            return true;
        }

        public static bool AssertLegalMoveset(ITrainerPool[] trainerPools, ExtractedGame extractedGame)
        {
            var randomizedPokemon = new HashSet<int>();
            foreach (var pool in trainerPools)
            {
                foreach (var trainer in pool.AllTrainers)
                {
                    if (!trainer.IsSet) continue;

                    for (int i = 0; i < trainer.Pokemon.Length; i++)
                    {
                        var pokemon = trainer.Pokemon[i];

                        if (pokemon.IsSet)
                        {
                            foreach (var move in pokemon.Moves)
                            {
                                if (move != 0)
                                {
                                    Assert.That(extractedGame.PokemonLegalMovePool[pokemon.Pokemon], Does.Contain(move));
                                }
                            }

                            randomizedPokemon.Add(pokemon.Index);
                        }
                    }
                }
            }
            return true;
        }
    }
}
