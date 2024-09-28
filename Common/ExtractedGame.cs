using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XDCommon.Contracts;
using XDCommon.PokemonDefinitions;
using XDCommon;
using XDCommon.Utility;

namespace XDCommon
{
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
        public Items[] GoodItems { get; }
        public Items[] ValidHeldItems { get; }
        public Items[] GoodHeldItems { get; }

        public TM[] TMs { get; }
        public Move[] HMs { get; }
        public TutorMove[] TutorMoves { get; }
        public PokeSpotPokemon[] PokeSpotPokemon { get; }

        public Game Game => TutorMoves.Length == 0 ? Game.Colosseum : Game.XD;

        public Dictionary<int, ushort[]> PokemonLegalMovePool = new Dictionary<int, ushort[]>();

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
            HMs = new[]
            {
                MoveList.First(m => m.Name.Equals(new UnicodeString(Encoding.UTF8.GetBytes("\0C\0U\0T")))),
                MoveList.First(m => m.Name.Equals(new UnicodeString(Encoding.UTF8.GetBytes("\0F\0L\0Y")))),
                MoveList.First(m => m.Name.Equals(new UnicodeString(Encoding.UTF8.GetBytes("\0S\0U\0R\0F")))),
                MoveList.First(m => m.Name.Equals(new UnicodeString(Encoding.UTF8.GetBytes("\0S\0T\0R\0E\0N\0G\0T\0H")))),
                MoveList.First(m => m.Name.Equals(new UnicodeString(Encoding.UTF8.GetBytes("\0F\0L\0A\0S\0H")))),
                MoveList.First(m => m.Name.Equals(new UnicodeString(Encoding.UTF8.GetBytes("\0R\0O\0C\0K\0 \0S\0M\0A\0S\0H")))),
                MoveList.First(m => m.Name.Equals(new UnicodeString(Encoding.UTF8.GetBytes("\0W\0A\0T\0E\0R\0F\0A\0L\0L")))),
                MoveList.First(m => m.Name.Equals(new UnicodeString(Encoding.UTF8.GetBytes("\0D\0I\0V\0E")))),
            };

            ValidHeldItems = NonKeyItems.Where(i => i.CanBeHeld == true).ToArray();
            GoodItems = NonKeyItems.Where(i => !ExtractorConstants.BadItemList.Contains(i.Index)).ToArray();
            GoodHeldItems = ValidHeldItems.Where(i => !ExtractorConstants.BadItemList.Contains(i.Index)).ToArray();

            if (extractor is XDExtractor xd)
            {
                TutorMoves = xd.ExtractTutorMoves();
                PokeSpotPokemon = xd.ExtractPokeSpotPokemon();
            }
            else
            {
                TutorMoves = Array.Empty<TutorMove>();
            }

            BuildLegalMoveList();
        }

        void BuildLegalMoveList()
        {
            foreach (var pokemon in ValidPokemon)
            {
                if (pokemon.Index == ExtractorConstants.BonslyIndex)
                {
                    Logger.Log("Fixing Bonsly\n\n");
                    XDPokemon.FixBonsly(pokemon);
                }

                var legalMoveList = pokemon.LevelUpMoves.Select(m => m.Move).ToList();

                var tmMoves = TMs.Where(t => pokemon.LearnableTMs[t.TMIndex - 1]).Select(t => t.Move);
                legalMoveList.AddRange(tmMoves);
                
                for (int i = 0; i < HMs.Length; i++)
                {
                    if (pokemon.LearnableTMs[Constants.NumberOfTMs + i])
                    {
                        legalMoveList.Add((ushort)HMs[i].MoveIndex);
                    }
                }

                if (Game == Game.XD)
                {
                    var tutorMoves = TutorMoves.Where(t => pokemon.TutorMoves[Constants.TutorMoveToPokemonOrderMapping[t.Index]]).Select(t => t.Move);
                    legalMoveList.AddRange(tutorMoves);
                }

                legalMoveList.AddRange(MoveList.Where(m => m.IsShadowMove).Select(m => (ushort)m.MoveIndex));

                PokemonLegalMovePool[pokemon.Index] = legalMoveList.Where(l => l > 0).ToArray();
            }
        }
    }
}
