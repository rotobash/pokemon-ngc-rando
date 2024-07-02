using System;
using System.Linq;
using XDCommon.Utility;

namespace XDCommon.PokemonDefinitions
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
        public Pokemon[] NoLegendaryPokemon => PokemonList.Where(p => !ExtractorConstants.Legendaries.Contains(p.Index)).ToArray();

        public Items[] ValidItems { get; }
        public Items[] NonKeyItems { get; }
        public Items[] GoodItems {  get; }
        public Items[] ValidHeldItems {  get; } 
        public Items[] GoodHeldItems { get; }

        public TM[] TMs { get; }
        public TutorMove[] TutorMoves { get; }

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
            ValidPokemon = PokemonList.Where(p => !ExtractorConstants.SpecialPokemon.Contains(p.Index)).ToArray();
            ValidItems = ItemList.Where(i => i.BagSlot != BagSlots.None && i.NameId > 0).ToArray();

            NonKeyItems = ValidItems.Where(i => i.BagSlot != BagSlots.KeyItems && i.BagSlot != BagSlots.None && i.BagSlot != BagSlots.Colognes).ToArray();
            TMs = ItemList.Where(i => i is TM).Select(i => i as TM).ToArray();

            ValidHeldItems = NonKeyItems.Where(i => i.CanBeHeld == true).ToArray();
            GoodItems = NonKeyItems.Where(i => !ExtractorConstants.BadItemList.Contains(i.Index)).ToArray();
            GoodHeldItems = ValidHeldItems.Where(i => !ExtractorConstants.BadItemList.Contains(i.Index)).ToArray();

            if (extractor is XDExtractor xd)
            {
                TutorMoves = xd.ExtractTutorMoves();
            }
            else
            {
                TutorMoves = Array.Empty<TutorMove>();
            }
        }
    }
}
