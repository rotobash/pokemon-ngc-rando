using Randomizer.Shufflers;
using Randomizer.XD;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XDCommon;
using XDCommon.PokemonDefinitions;
using XDCommon.Utility;

namespace Randomizer
{
    public interface IGameExtractor
    {
        ISO ISO { get; }
        ITrainerPool[] ExtractPools(Pokemon[] pokemon, Move[] moves);
        Ability[] ExtractAbilities();
        Items[] ExtractItems();
        OverworldItem[] ExtractOverworldItems();
        Pokemarts[] ExtractPokemarts();
        Move[] ExtractMoves();
        Pokemon[] ExtractPokemon();
        IGiftPokemon[] ExtractGiftPokemon();
    }

    public class ExtractedGame
    {
        public ITrainerPool[] TrainerPools { get; }
        public Move[] MoveList { get; }
        public Ability[] Abilities { get; }
        public Pokemon[] PokemonList { get; }
        public IGiftPokemon[] GiftPokemonList { get; }
        public Items[] ItemList { get; }
        public OverworldItem[] OverworldItemList { get; }
        public Pokemarts[] Pokemarts { get; }

        public Move[] ValidMoves { get; }
        public Move[] GoodDamagingMoves => MoveList.Where(m => m.BasePower >= Configuration.GoodDamagingMovePower).ToArray();

        public Pokemon[] ValidPokemon { get; }
        public Pokemon[] GoodPokemon => PokemonList.Where(p => p.BST >= Configuration.StrongPokemonBST).ToArray();

        public Items[] ValidItems { get; }
        public Items[] NonKeyItems { get; }
        public Items[] GoodItems => NonKeyItems.Where(i => !RandomizerConstants.BadItemList.Contains(i.Index)).ToArray();

        public TM[] TMs { get; }
        public TutorMove[] TutorMoves { get; }

        bool isXD;

        public ExtractedGame(IGameExtractor extractor)
        {
            MoveList = extractor.ExtractMoves();
            Abilities = extractor.ExtractAbilities();
            PokemonList = extractor.ExtractPokemon();
            GiftPokemonList = extractor.ExtractGiftPokemon();
            ItemList = extractor.ExtractItems();
            OverworldItemList = extractor.ExtractOverworldItems();
            Pokemarts = extractor.ExtractPokemarts().OrderBy(m => m.FirstItemIndex).ToArray();
            TrainerPools = extractor.ExtractPools(PokemonList, MoveList);

            ValidMoves = MoveList.Where(m => m.MoveIndex != 0 && m.MoveIndex != 355).ToArray();
            ValidPokemon = PokemonList.Where(p => !RandomizerConstants.SpecialPokemon.Contains(p.Index)).ToArray();
            ValidItems = ItemList.Where(i => !RandomizerConstants.InvalidItemList.Contains(i.Index)).ToArray();
            NonKeyItems = ValidItems.Where(i => i.BagSlot != BagSlots.KeyItems && i.BagSlot != BagSlots.None).ToArray();
            TMs = ItemList.Where(i => i is TM).Select(i => i as TM).ToArray();

            if (extractor is XDExtractor xd)
            {
                isXD = true;
                TutorMoves = xd.ExtractTutorMoves();
            }
            else
            {
                isXD = false;
                TutorMoves = Array.Empty<TutorMove>();
            }
        }
    }
}
