﻿using System;
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
    public class XDExtractor: IGameExtractor
	{
        ISO iso;
        public XDExtractor(ISO iso)
        {
            this.iso = iso;
        }

        public TrainerPool[] ExtractPools()
        {
			var poolFsys = iso.GetFSysFile("deck_archive.fsys") ?? throw new KeyNotFoundException($"Could not extract deck_archive.fsys, it doesn't exist in the TOC.");
			var poolTypes = Enum.GetValues<TrainerTeamTypes>();
			var trainerPool = new TrainerPool[poolTypes.Length];

			for (int i = 0; i < poolTypes.Length; i++)
            {
				var pool = poolTypes[i];
				if (Configuration.Verbose) 
				{
					Console.WriteLine($"Extracting deck: {pool}");
				}
				var multiplier = iso.Region == Region.Japan ? 1 : 2;
				var offset = iso.Region == Region.Europe ? 1 : 0;
				var index = (i * multiplier) + offset;
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

				trainerPool[i] = new TrainerPool(pool, file, iso);
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
			var p = pokemon.Where(pk => pk.Name != string.Empty);
			return pokemon;
		}
    }
}