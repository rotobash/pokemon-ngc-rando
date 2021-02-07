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

			var sStream = File.Open($"{Configuration.ExtractDirectory}/DeckData_DarkPokemon.bin", FileMode.Open, FileAccess.ReadWrite);
			var shadowPokemonPool = FSysFileEntry.CreateExtractedFile(Configuration.ExtractDirectory, "DeckData_DarkPokemon.bin", FileTypes.BIN, sStream);

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
				var fStream = File.Open($"{Configuration.ExtractDirectory}/{fileName}", FileMode.Open, FileAccess.ReadWrite);

				IExtractedFile file = FSysFileEntry.CreateExtractedFile(Configuration.ExtractDirectory, fileName, FileTypes.BIN, fStream);
				//if (!poolFsys.ExtractedEntries.ContainsKey(fileName))
				//{
				//	file = FSysFileEntry.ExtractFromFSys(poolFsys, index);
				//}
				//else
				//{
				//	file = poolFsys.ExtractedEntries[fileName];
				//}

				trainerPool[i + 1] = new XDTrainerPool(pool, file, pokemon, moves);
				trainerPool[i + 1].SetShadowPokemon(trainerPool[0] as XDShadowTrainerPool);
				trainerPool[i + 1].LoadTrainers(iso);
            }
			return trainerPool;
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
    }
}
