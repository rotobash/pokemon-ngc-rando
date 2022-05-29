using Randomizer.Shufflers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XDCommon.Contracts;
using XDCommon.PokemonDefinitions;
using XDCommon.Utility;

namespace Randomizer.Colosseum
{
    public class ColoExtractor : IGameExtractor
	{
		public ISO ISO { get; }
		public ColoExtractor(ISO iso)
		{
			this.ISO = iso;
		}

		public ITrainerPool[] ExtractPools(Pokemon[] pokemon, Move[] moves)
		{
			var trainerPool = new ITrainerPool[]
			{
				new ColTrainerPool(ISO, pokemon, moves)
			};

			return trainerPool;
		}

		public Items[] ExtractItems()
		{
			var numItems = Constants.ColNumberOfItems;
			var items = new Items[numItems];

			for (int i = 0; i < numItems; i++)
			{
				if (i <= Constants.LastPokeballIndex)
				{
					items[i] = new Pokeballs(i, ISO);
				}
				else if (i >= Constants.FirstTMItemIndex && i < Constants.FirstTMItemIndex + Constants.NumberOfTMsAndHMs)
				{
					items[i] = new TM(i, ISO);
				}
				else
				{
					items[i] = new Items(i, ISO);
				}
			}

			return items;
		}

		public OverworldItem[] ExtractOverworldItems()
		{
			var numItems = ISO.CommonRel.GetValueAtPointer(Constants.ColNumberTreasureBoxes);
			var items = new OverworldItem[numItems];
			for (int i = 0; i < numItems; i++)
			{
				items[i] = new OverworldItem(i, ISO);
			}
			return items;
		}


		public Pokemarts[] ExtractPokemarts()
		{
			var marts = new List<Pokemarts>();

			foreach (var pocketfile in Constants.ColItemTables[ISO.Region])
			{
				var pocket = ISO.GetFSysFile(pocketfile.Key).GetEntryByFileName("pocket_menu.rel") as REL;
				var numMarts = pocket.GetValueAtPointer(Constants.NumberOfMarts);
				for (int i = 0; i < numMarts; i++)
				{
					marts.Add(new Pokemarts(i, pocket));
				}
			}
			return marts.ToArray();
		}

		public Move[] ExtractMoves()
		{
			// last move is ???? 
			var moveNum = ISO.CommonRel.GetValueAtPointer(Constants.ColNumberOfMoves) - 1;
			var moves = new Move[moveNum];
			for (int i = 0; i < moveNum; i++)
			{
				moves[i] = new Move(i, ISO);
			}
			return moves;
		}

		public Pokemon[] ExtractPokemon()
		{
			var pokemonNum = ISO.CommonRel.GetValueAtPointer(Constants.ColNumberOfPokemon);
			var pokemon = new Pokemon[pokemonNum];
			for (int i = 0; i < pokemonNum; i++)
			{
				pokemon[i] = new ColPokemon(i, ISO);
			}

			return pokemon;
		}

		public IGiftPokemon[] ExtractGiftPokemon()
		{
			var giftPokemon = new IGiftPokemon[4];
			for (int i = 0; i < giftPokemon.Length; i++)
			{
				if (i < 4)
				{
					giftPokemon[i] = new ColGiftPokemon((byte)i, ISO);
				}
			}
			return giftPokemon;
		}

		public IGiftPokemon[] GetStarters()
		{
			return new[]
			{
				new ColStarterPokemon(ISO, true),
				new ColStarterPokemon(ISO, false),
			};
		}

		public Ability[] ExtractAbilities()
		{
			var numAbilities = Constants.NumberOfAbilities(ISO);
			var abilities = new Ability[numAbilities];
			for (int i = 0; i < abilities.Length; i++)
			{
				abilities[i] = new Ability(i, ISO);
			}
			return abilities;
		}
	}
}
