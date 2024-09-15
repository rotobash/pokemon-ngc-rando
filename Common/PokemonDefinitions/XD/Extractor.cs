using System;
using System.Collections.Generic;
using XDCommon.Utility;

namespace XDCommon.PokemonDefinitions
{
    public class XDExtractor : IGameExtractor
	{
        public ISO ISO { get; }
        public XDExtractor(ISO iso)
        {
            ISO = iso;
        }

        public ITrainerPool[] ExtractPools(Pokemon[] pokemon, Move[] moves)
        {
			var trainerPool = new ITrainerPool[XDTrainerPool.Trainers.Length + 1];
			trainerPool[0] = new ShadowTrainerPool(ISO, pokemon, moves);

			for (int i = 1; i <= XDTrainerPool.Trainers.Length; i++)
            {
				var poolType = XDTrainerPool.Trainers[i - 1];
				var pool = new XDTrainerPool(poolType, ISO, pokemon, moves)
				{
					DarkPokemon = trainerPool[0] as ShadowTrainerPool
				};
				pool.LoadTrainers();
				trainerPool[i] = pool;
            }
			return trainerPool;
		}

		public Items[] ExtractItems()
		{
			var numItems = (int)ISO.CommonRel.GetValueAtPointer(Constants.XDNumberOfItems);
			var items = new Items[numItems];

			for (int i = 0; i < numItems; i++)
			{
				if (i <= Constants.LastPokeballIndex)
				{
					items[i] = new Pokeballs(i, ISO);
				}
				else if (i >= Constants.FirstTMItemIndex && i < Constants.FirstTMItemIndex + Constants.NumberOfTMs)
				{
					items[i] = new TM(i, ISO);
				}
				else if (i >= 350)
                {
                    items[i] = new Items(i + 150, ISO);
                }
				else
				{
					items[i] = new Items(i, ISO);
				}
			}
			return items;
		}

		public TutorMove[] ExtractTutorMoves()
		{
			var tutorMoves = new TutorMove[Constants.NumberOfTutorMoves];
			for (int i = 0; i < tutorMoves.Length; i++)
			{
				tutorMoves[i] = new TutorMove(i, ISO);
			}
			return tutorMoves;
		}

		public OverworldItem[] ExtractOverworldItems()
		{
			var numItems = ISO.CommonRel.GetValueAtPointer(Constants.XDNumberTreasureBoxes);
			var items = new OverworldItem[numItems - 1];
			for (int i = 1;  i < numItems; i++)
            {
				items[i - 1] = new OverworldItem(i, ISO);
            }
			return items;
		}

		public Pokemarts[] ExtractPokemarts()
		{
			var marts = new List<Pokemarts>();

			foreach (var pocketfile in Constants.XDItemTables[ISO.Region])
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
			var moveNum = ISO.CommonRel.GetValueAtPointer(Constants.XDNumberOfMoves) - 1;
			var moves = new Move[moveNum];
			for (int i = 0; i < moveNum; i++) {
				moves[i] = new Move(i, ISO);
			}
			return moves;
		}

		public Pokemon[] ExtractPokemon()
		{
			var pokemonNum = ISO.CommonRel.GetValueAtPointer(Constants.XDNumberOfPokemon);
			var pokemon = new Pokemon[pokemonNum];
			for (int i = 0; i < pokemonNum; i++)
			{
				pokemon[i] = new XDPokemon(i, ISO);
			}

			return pokemon;
		}

		public BattleBingoCard[] ExtractBattleBingoCards()
		{
			var numCards = Constants.NumberOfBingoCards;
			var cards = new BattleBingoCard[numCards];
			for (int i = 0; i < numCards; i++)
			{
				cards[i] = new BattleBingoCard(i, ISO);
			}
			return cards;
		}

		public PokeSpotPokemon[] ExtractPokeSpotPokemon()
		{
			var pokeSpots = Enum.GetValues(typeof(PokeSpotType));
			var pokemon = new List<PokeSpotPokemon>();
			
			foreach (PokeSpotType pokeSpotType in pokeSpots)
            {
				var pokeSpot = new PokeSpot(pokeSpotType, ISO);
				for (int i = 0; i < pokeSpot.NumberOfEntries; i++)
                {
					pokemon.Add(new PokeSpotPokemon(i, pokeSpot, ISO));
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
					giftPokemon[i] = new XDTradePokemon((byte)i, ISO);
                }
				else if (i == 4)
                {
					giftPokemon[i] = new XDShadowGiftPokemon(ISO);
                }
				else
                {
					// have to fiddle with the index
					var index = giftPokemon.Length - 1 - i;
					giftPokemon[i] = new XDMtBattlePokemon((byte)index, ISO);
                }
            }
			return giftPokemon;
		}

		public XDStarterPokemon GetStarter()
        {
			return new XDStarterPokemon(ISO);
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
        public Room[] ExtractRooms()
        {
            var numRooms = ISO.CommonRel.GetValueAtPointer(Constants.XDNumberOfRooms);
            var rooms = new Room[numRooms];
            for (int i = 0; i < rooms.Length; i++)
            {
                rooms[i] = new Room(i, ISO);
            }
            return rooms;
        }

        public Area[] ExtractAreas()
        {
            var areaNum = ISO.CommonRel.GetValueAtPointer(Constants.XDNumberOfWorldMapLocations);
			var areas = new Area[areaNum];
			for (int i = 0; i < areas.Length; i++)
            {
				areas[i] = new Area(i, ISO);
            }
			return areas;
        }
    }
}
