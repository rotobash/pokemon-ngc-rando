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
			var trainerPool = new ITrainerPool[XDTrainerPool.MainTeams.Length + 1];
			trainerPool[0] = new XDShadowTrainerPool(iso, pokemon, moves);

			for (int i = 1; i <= XDTrainerPool.MainTeams.Length; i++)
            {
				var pool = XDTrainerPool.MainTeams[i - 1];
                trainerPool[i] = new XDTrainerPool(pool, iso, pokemon, moves);
				trainerPool[i].SetShadowPokemon(trainerPool[0] as XDShadowTrainerPool);
				trainerPool[i].LoadTrainers();
            }
			return trainerPool;
		}

		public Items[] ExtractItems()
		{
			var numItems = (int)iso.CommonRel.GetValueAtPointer(Constants.NumberOfItems);
			var items = new Items[numItems];
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

			return items;
		}

		public TutorMove[] ExtractTutorMoves()
		{
			var tutorMoves = new TutorMove[Constants.NumberOfTutorMoves];
			for (int i = 0; i < tutorMoves.Length; i++)
			{
				tutorMoves[i] = new TutorMove(i, iso);
			}
			return tutorMoves;
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

		public IGiftPokemon[] ExtractGiftPokemon()
        {
			var giftPokemon = new IGiftPokemon[8];
			for (int i = 0; i < giftPokemon.Length; i++)
            {
				if (i < 4)
                {
					giftPokemon[i] = new XDTradePokemon((byte)i, iso);
                }
				else if (i == 4)
                {
					giftPokemon[i] = new XDShadowGiftPokemon(iso);
                }
				else
                {
					// have to fiddle with the index
					var index = giftPokemon.Length - 1 - i;
					giftPokemon[i] = new XDMtBattlePokemon((byte)index, iso);
                }
            }
			return giftPokemon;
		}

		public XDStarterPokemon GetStarter()
        {
			return new XDStarterPokemon(iso);
		}

        public Ability[] ExtractAbilities()
        {
			var numAbilities = Constants.NumberOfAbilities(iso);
			var abilities = new Ability[numAbilities];
			for (int i = 0; i < abilities.Length; i++)
            {
				abilities[i] = new Ability(i, iso);
            }
			return abilities;
        }
    }
}
