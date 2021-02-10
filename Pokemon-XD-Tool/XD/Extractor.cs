using Randomizer.Shufflers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XDCommon;
using XDCommon.Contracts;
using XDCommon.PokemonDefinitions;
using XDCommon.Utility;

namespace Randomizer.XD
{
    public class XDExtractor : IGameExtractor
	{
        ISO iso;
        public XDExtractor(ISO iso)
        {
            this.iso = iso;
        }

		public ITrainerPool[] ExtractPools(Pokemon[] pokemon, Move[] moves)
		{
			var poolFsys = iso.GetFSysFile("deck_archive.fsys") ?? throw new KeyNotFoundException($"Could not extract deck_archive.fsys, it doesn't exist in the TOC.");
			var poolTypes = Enum.GetValues<TrainerPoolType>().ToList();
			var trainerPool = new ITrainerPool[XDTrainerPool.MainTeams.Length + 1];

			var shadowPokemonPool = poolFsys.ExtractEntryByFileName("DeckData_DarkPokemon.bin");

			trainerPool[0] = new XDShadowTrainerPool(shadowPokemonPool, iso, pokemon, moves);

			for (int i = 0; i < XDTrainerPool.MainTeams.Length; i++)
			{

				var pool = XDTrainerPool.MainTeams[i];
				if (Configuration.Verbose)
				{
					Console.WriteLine($"Extracting deck: {pool}");
				}
				var multiplier = iso.Region == Region.Japan ? 1 : 2;
				var offset = iso.Region == Region.Europe ? 1 : 0;
				var index = (poolTypes.IndexOf(pool) * multiplier) + offset;
				var fileName = poolFsys.GetFilenameForFile(index);

				// stub deck data for now
				//var fStream = File.Open($"{Configuration.ExtractDirectory}/{fileName}", FileMode.Open, FileAccess.ReadWrite);

				IExtractedFile file = poolFsys.ExtractEntryByFileName(fileName);

				trainerPool[i + 1] = new XDTrainerPool(pool, file, pokemon, moves);
				trainerPool[i + 1].SetShadowPokemon(trainerPool[0] as XDShadowTrainerPool);
				trainerPool[i + 1].LoadTrainers(iso);
			}
			return trainerPool;
		}

		public Items[] ExtractItems()
		{
			var numItems = (int)iso.CommonRel.GetValueAtPointer(Constants.NumberOfItems);
			var items = new Items[numItems + Constants.NumberOfTutorMoves];
			for (int i = 0; i < numItems; i++)
            {
				if (i <= 12)
                {
					items[i] = new Pokeballs(i, iso);
                }
				else if (i >= Constants.FirstTMItemIndex && i < Constants.FirstTMItemIndex + Constants.NumberOfTMsAndHMs)
                {
					items[i] = new TM(i, iso);
                }
				else
                {
					items[i] = new Items(i, iso);
                }
            }
			for (int i = numItems; i < items.Length; i++)
            {
				items[i] = new TutorMove(i - numItems, iso);
            }

			return items;
		}

		public OverworldItem[] ExtractOverworldItems()
		{
			var numItems = iso.CommonRel.GetValueAtPointer(Constants.XDNumberTreasureBoxes);
			var items = new OverworldItem[numItems];
			for (int i = 0;  i < numItems; i++)
            {
				items[i] = new OverworldItem(i, iso);
            }
			return items;
		}

		public Pokemarts[] ExtractPokemarts()
		{
			var pocket = iso.GetFSysFile("pocket_menu.fsys").ExtractEntryByFileName("pocket_menu.rel") as REL;
			var numMarts = pocket.GetValueAtPointer(Constants.NumberOfMarts);
			var marts = new Pokemarts[numMarts];
			for (int i = 0;  i < numMarts; i++)
            {
				marts[i] = new Pokemarts(i, iso);
            }
			return marts;
		}

		public Move[] ExtractMoves()
        {
			var moveNum = iso.CommonRel.GetValueAtPointer(Constants.XDNumberOfMoves);
			var moves = new Move[moveNum];
			for (int i = 0; i < moveNum; i++) {
				moves[i] = new Move(i, iso);
			}
			return moves;
		}

		public Pokemon[] ExtractPokemon()
		{
			var pokemonNum = iso.CommonRel.GetValueAtPointer(Constants.XDNumberOfPokemon);
			var pokemon = new Pokemon[pokemonNum];
			for (int i = 0; i < pokemonNum; i++)
			{
				pokemon[i] = new Pokemon(i, iso);
			}

			return pokemon;
		}

        public void RandomizeStatics(StaticPokemonShufflerSettings settings, Random random, Pokemon[] pokemon, Move[] moves)
        {
			StaticPokemonShuffler.RandomizeXDStatics(random, settings, new XDStarterPokemon(iso), Array.Empty<IGiftPokemon>(), pokemon, moves);
		}

		public BattleBingoCard[] ExtractBattleBingoCards()
		{
			var numCards = Constants.NumberOfBingoCards;
			var cards = new BattleBingoCard[numCards];
			for (int i = 0; i < numCards; i++)
			{
				cards[i] = new BattleBingoCard(i, iso);
			}
			return cards;
		}

		public PokeSpotPokemon[] ExtractPokeSpotPokemon()
		{
			var pokeSpots = Enum.GetValues<PokeSpotType>();
			var pokemon = new List<PokeSpotPokemon>();
			
			foreach (var pokeSpotType in pokeSpots)
            {
				var pokeSpot = new PokeSpot(pokeSpotType, iso);
				for (int i = 0; i < pokeSpot.NumberOfEntries; i++)
                {
					pokemon.Add(new PokeSpotPokemon(i, pokeSpot, iso));
                }
            }

			return pokemon.ToArray();
		}
	}
}
