using Newtonsoft.Json;
using System.IO.Compression;
using XDCommon;
using XDCommon.PokemonDefinitions;
using XDCommon.Utility;
using XDCommon.Shufflers;
using Randomizer;
using RandomizerTests.ReferenceData;

namespace RandomizerTests.Items
{
    public class ItemTests : BaseTestSetup
    {
        #region OverworldItems
        [Test]
        public void TestOverworldItemRando()
        {
            shuffleSettings.RandomizerSettings.ItemShufflerSettings = new ItemShufflerSettings
            {
                RandomizeItems = true
            };

            for (int i = 0; i < Helpers.RerunTimes; i++)
            {
                ItemShuffler.ShuffleOverworldItems(shuffleSettings);

                var shuffledItems = gameExtractor.ExtractOverworldItems();
                var assertMessage = ItemAssertions.CheckOverworldItems(shuffledItems, shuffleSettings.RandomizerSettings);
                if (!string.IsNullOrEmpty(assertMessage))
                {
                    Assert.Fail(assertMessage);
                }
            }
            Assert.That(true, Is.True);
        }

        [Test]
        public void TestOverworldBanBadItems()
        {
            shuffleSettings.RandomizerSettings.ItemShufflerSettings = new ItemShufflerSettings
            {
                RandomizeItems = true,
                BanBadItems = true
            };

            for (int i = 0; i < Helpers.RerunTimes; i++)
            {
                ItemShuffler.ShuffleOverworldItems(shuffleSettings);

                var shuffledItems = gameExtractor.ExtractOverworldItems();
                var assertMessage = ItemAssertions.CheckOverworldItems(shuffledItems, shuffleSettings.RandomizerSettings);
                if (!string.IsNullOrEmpty(assertMessage))
                {
                    Assert.Fail(assertMessage);
                }
            }

            Assert.That(true, Is.True);
        }

        [Test]
        public void TestOverworldBanBattleCds()
        {
            if (shuffleSettings.ExtractedGame.Game == XDCommon.Contracts.Game.Colosseum)
            {
                Assert.Pass();
            }

            shuffleSettings.RandomizerSettings.ItemShufflerSettings = new ItemShufflerSettings
            {
                RandomizeItems = true,
                BanBattleCDs = true
            };

            for (int i = 0; i < Helpers.RerunTimes; i++)
            {
                ItemShuffler.ShuffleOverworldItems(shuffleSettings);

                var shuffledItems = gameExtractor.ExtractOverworldItems();
                var assertMessage = ItemAssertions.CheckOverworldItems(shuffledItems, shuffleSettings.RandomizerSettings);
                if (!string.IsNullOrEmpty(assertMessage))
                {
                    Assert.Fail(assertMessage);
                }
            }

            Assert.That(true, Is.True);
        }
        [Test]
        public void TestOverworldChangeTreasureModel()
        {
            Assert.Pass("Not implemented");

            shuffleSettings.RandomizerSettings.ItemShufflerSettings = new ItemShufflerSettings
            {
                RandomizeItems = true,
                MakeItemsBoxes = true
            };

            for (int i = 0; i < Helpers.RerunTimes; i++)
            {
                ItemShuffler.ShuffleOverworldItems(shuffleSettings);

                var shuffledItems = gameExtractor.ExtractOverworldItems();
                var assertMessage = ItemAssertions.CheckOverworldItems(shuffledItems, shuffleSettings.RandomizerSettings);
                if (!string.IsNullOrEmpty(assertMessage))
                {
                    Assert.Fail(assertMessage);
                }
            }

            shuffleSettings.RandomizerSettings.ItemShufflerSettings = new ItemShufflerSettings
            {
                RandomizeItems = true,
                MakeItemsSparkles = true
            };

            for (int i = 0; i < Helpers.RerunTimes; i++)
            {
                ItemShuffler.ShuffleOverworldItems(shuffleSettings);

                var shuffledItems = gameExtractor.ExtractOverworldItems();
                var assertMessage = ItemAssertions.CheckOverworldItems(shuffledItems, shuffleSettings.RandomizerSettings);
                if (!string.IsNullOrEmpty(assertMessage))
                {
                    Assert.Fail(assertMessage);
                }
            }

            Assert.That(true, Is.True);
        }
        #endregion

        #region PokemonItems

        [Test]
        public void TestPokemonItemRando()
        {
            shuffleSettings.RandomizerSettings.TeamShufflerSettings = new TeamShufflerSettings
            {
                RandomizeHeldItems = true,
            };

            for (int i = 0; i < Helpers.RerunTimes; i++)
            {
                TeamShuffler.ShuffleTeams(shuffleSettings);

                var shuffledTrainers = gameExtractor.ExtractPools(pokemon, moves);
                var assertMessage = ItemAssertions.CheckTrainerHeldItems(shuffledTrainers, shuffleSettings.RandomizerSettings);
                if (!string.IsNullOrEmpty(assertMessage))
                {
                    Assert.Fail(assertMessage);
                }
            }
            Assert.That(true, Is.True);
        }

        [Test]
        public void TestPokemonBanBadItems()
        {
            shuffleSettings.RandomizerSettings.TeamShufflerSettings = new TeamShufflerSettings
            {
                RandomizeHeldItems = true,
                BanBadItems = true
            };

            for (int i = 0; i < Helpers.RerunTimes; i++)
            {
                TeamShuffler.ShuffleTeams(shuffleSettings);

                var shuffledTrainers = gameExtractor.ExtractPools(pokemon, moves);
                var assertMessage = ItemAssertions.CheckTrainerHeldItems(shuffledTrainers, shuffleSettings.RandomizerSettings);
                if (!string.IsNullOrEmpty(assertMessage))
                {
                    Assert.Fail(assertMessage);
                }
            }

            Assert.That(true, Is.True);
        }

        [Test]
        public void TestPokemonBanBattleCds()
        {
            shuffleSettings.RandomizerSettings.TeamShufflerSettings = new TeamShufflerSettings
            {
                RandomizeHeldItems = true,
                BanBattleCDs = true
            };

            for (int i = 0; i < Helpers.RerunTimes; i++)
            {
                TeamShuffler.ShuffleTeams(shuffleSettings);

                var shuffledTrainers = gameExtractor.ExtractPools(pokemon, moves);
                var assertMessage = ItemAssertions.CheckTrainerHeldItems(shuffledTrainers, shuffleSettings.RandomizerSettings);
                if (!string.IsNullOrEmpty(assertMessage))
                {
                    Assert.Fail(assertMessage);
                }
            }

            Assert.That(true, Is.True);
        }
        #endregion

        #region Marts

        [Test]
        public void TestMartsItemRando()
        {
            shuffleSettings.RandomizerSettings.ItemShufflerSettings = new ItemShufflerSettings
            {
                RandomizeMarts = true
            };

            for (int i = 0; i < Helpers.RerunTimes; i++)
            {
                ItemShuffler.UpdatePokemarts(shuffleSettings);

                var shuffledMarts = gameExtractor.ExtractPokemarts();
                var assertMessage = ItemAssertions.CheckPokemarts(shuffledMarts, shuffleSettings.RandomizerSettings);
                if (!string.IsNullOrEmpty(assertMessage))
                {
                    Assert.Fail(assertMessage);
                }
            }
            Assert.That(true, Is.True);
        }

        [Test]
        public void TestMartsBanBadItems()
        {
            shuffleSettings.RandomizerSettings.ItemShufflerSettings = new ItemShufflerSettings
            {
                RandomizeMarts = true,
                BanBadItems = true
            };

            for (int i = 0; i < Helpers.RerunTimes; i++)
            {
                ItemShuffler.UpdatePokemarts(shuffleSettings);

                var shuffledMarts = gameExtractor.ExtractPokemarts();
                var assertMessage = ItemAssertions.CheckPokemarts(shuffledMarts, shuffleSettings.RandomizerSettings);
                if (!string.IsNullOrEmpty(assertMessage))
                {
                    Assert.Fail(assertMessage);
                }
            }

            Assert.That(true, Is.True);
        }

        [Test]
        public void TestMartsBanBattleCds()
        {
            if (shuffleSettings.ExtractedGame.Game == XDCommon.Contracts.Game.Colosseum)
            {
                Assert.Pass();
            }

            shuffleSettings.RandomizerSettings.ItemShufflerSettings = new ItemShufflerSettings
            {
                RandomizeMarts = true,
                BanBattleCDs = true
            };

            for (int i = 0; i < Helpers.RerunTimes; i++)
            {
                ItemShuffler.UpdatePokemarts(shuffleSettings);

                var shuffledMarts = gameExtractor.ExtractPokemarts();
                var assertMessage = ItemAssertions.CheckPokemarts(shuffledMarts, shuffleSettings.RandomizerSettings);
                if (!string.IsNullOrEmpty(assertMessage))
                {
                    Assert.Fail(assertMessage);
                }
            }

            Assert.That(true, Is.True);
        }
        [Test]
        public void TestMartsSellStones()
        {
            Assert.Pass("Not implemented");
            if (shuffleSettings.ExtractedGame.Game == XDCommon.Contracts.Game.Colosseum)
            {
                Assert.Pass("Not implemented");
            }

            shuffleSettings.RandomizerSettings.ItemShufflerSettings = new ItemShufflerSettings
            {
                MartsSellEvoStones = true,
            };

            for (int i = 0; i < Helpers.RerunTimes; i++)
            {
                ItemShuffler.UpdatePokemarts(shuffleSettings);

                var shuffledMarts = gameExtractor.ExtractPokemarts();
                var assertMessage = ItemAssertions.CheckPokemarts(shuffledMarts, shuffleSettings.RandomizerSettings);
                if (!string.IsNullOrEmpty(assertMessage))
                {
                    Assert.Fail(assertMessage);
                }
            }
            Assert.That(true, Is.True);
        }
        #endregion
    }
}