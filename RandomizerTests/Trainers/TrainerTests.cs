using Newtonsoft.Json;
using System.IO.Compression;
using XDCommon;
using XDCommon.PokemonDefinitions;
using XDCommon.Utility;
using XDCommon.Shufflers;
using Randomizer;
using RandomizerTests.ReferenceData;

namespace RandomizerTests.Trainers
{
    public class TrainerTests : BaseTestSetup
    {
        [Test]
        public void TestNoShadowDuplicates()
        {
            shuffleSettings.RandomizerSettings.TeamShufflerSettings = new TeamShufflerSettings
            {
                RandomizePokemon = true,
                NoDuplicateShadows = true
            };

            for (int i = 0; i < Helpers.RerunTimes; i++)
            {
                TeamShuffler.ShuffleTeams(shuffleSettings);

                var trainerPool = gameExtractor.ExtractPools(pokemon, moves);
                Assert.That(TrainerAssertions.AssertNoDuplicates(trainerPool), Is.True);
            }
        }

        [Test]
        public void TestLevelBoost()
        {
            shuffleSettings.RandomizerSettings.TeamShufflerSettings = new TeamShufflerSettings
            {
                BoostTrainerLevel = false,
                BoostTrainerLevelPercent = 0.1f
            };
            TeamShuffler.ShuffleTeams(shuffleSettings);

            var trainerPool = gameExtractor.ExtractPools(pokemon, moves);
            // assert no boost if flag is false but percent is defined
            Assert.That(TrainerAssertions.AssertLevelBoost(trainerPool, shuffleSettings.RandomizerSettings), Is.True);

            shuffleSettings.RandomizerSettings.TeamShufflerSettings = new TeamShufflerSettings
            {
                BoostTrainerLevel = true,
                BoostTrainerLevelPercent = 0.1f
            };
            TeamShuffler.ShuffleTeams(shuffleSettings);

            trainerPool = gameExtractor.ExtractPools(pokemon, moves);
            Assert.That(TrainerAssertions.AssertLevelBoost(trainerPool, shuffleSettings.RandomizerSettings), Is.True);
        }

        [Test]
        public void TestLegalMovesOnly()
        {
            shuffleSettings.RandomizerSettings.TeamShufflerSettings = new TeamShufflerSettings
            {
                RandomizePokemon = true,
                MoveSetOptions = new RandomMoveSetOptions
                {
                    RandomizeMovesets = true,
                    LegalMovesOnly = true,
                }
            };

            for (int i = 0; i < Helpers.RerunTimes; i++)
            {
                TeamShuffler.ShuffleTeams(shuffleSettings);

                var trainerPool = gameExtractor.ExtractPools(pokemon, moves);
                Assert.That(TrainerAssertions.AssertLegalMoveset(trainerPool, shuffleSettings.ExtractedGame), Is.True);
            }

            shuffleSettings.RandomizerSettings.ItemShufflerSettings = new ItemShufflerSettings
            {
                RandomizeTMs = true,
                RandomizeTutorMoves = true
            };

            for (int i = 0; i < Helpers.RerunTimes; i++)
            {
                ItemShuffler.ShuffleTMs(shuffleSettings);
                TeamShuffler.ShuffleTeams(shuffleSettings);

                var trainerPool = gameExtractor.ExtractPools(pokemon, moves);
                Assert.That(TrainerAssertions.AssertLegalMoveset(trainerPool, shuffleSettings.ExtractedGame), Is.True);
            }
        }
    }
}