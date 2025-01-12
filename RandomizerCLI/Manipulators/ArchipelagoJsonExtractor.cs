using RandomizerCLI.Options;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using XDCommon.PokemonDefinitions;
using XDCommon.Utility;
using RandomizerCLI.Options;
using System.Runtime.InteropServices;
using XDCommon.Shufflers;
using RandomizerCLI.AP.Locations;
using RandomizerCLI.AP.Items;
using RandomizerCLI.AP;
using XDCommon;
using XDCommon.PokemonDefinitions.XD;
using System.Text.RegularExpressions;

namespace RandomizerCLI.Manipulators
{

    enum APLocationType { GetPokemon, TreasureSparkling, TreasureChest, Purify }

    public class UnicodeStringJsonConverter : JsonConverter<UnicodeString>
    {
        public override UnicodeString Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var bytes = Encoding.UTF8.GetBytes(reader.GetString() ?? "\0").SelectMany(b => new byte[] { 0, b });
            return new UnicodeString(bytes);
        }

        public override void Write(Utf8JsonWriter writer, UnicodeString unicodeString, JsonSerializerOptions options)
        {
            writer.WriteStringValue(unicodeString.ToString());
        }
    }

    public class ArchipelagoJsonExtractor : GameManipulator
    {
        string OutputDirectory { get; }
        string LocationDirectory { get; }
        string ItemsDirectory { get; }
        string RegionsDirectory { get; }
        IEnumerable<SlotTypes> Slots { get; }
        JsonSerializerOptions Options { get; }

        Dictionary<int, bool> IsRegionUsed = new Dictionary<int, bool>();

        public ArchipelagoJsonExtractor(ExtractOptions options): base(options)
        {
            OutputDirectory = options.OutputPath;
            LocationDirectory = $"{OutputDirectory}{Path.DirectorySeparatorChar}locations";
            ItemsDirectory = $"{OutputDirectory}{Path.DirectorySeparatorChar}items";
            RegionsDirectory = $"{OutputDirectory}{Path.DirectorySeparatorChar}regions";

            if (!Directory.Exists(LocationDirectory))
            {
                Directory.CreateDirectory(LocationDirectory);
            }

            if (!Directory.Exists(ItemsDirectory))
            {
                Directory.CreateDirectory(ItemsDirectory);
            }

            if (!Directory.Exists(RegionsDirectory))
            {
                Directory.CreateDirectory(RegionsDirectory);
            }

            Slots = Enum.GetValues<SlotTypes>();
            Options = new JsonSerializerOptions
            {
                WriteIndented = true,
                Converters =
                {
                    new UnicodeStringJsonConverter()
                }
            };
        }

        public void Extract()
        {
            var pokemon = GameExtractor.ExtractPokemon();
            var moves = GameExtractor.ExtractMoves();
            var items = GameExtractor.ExtractItems();

            XDExtractor xd = GameExtractor as XDExtractor;

            var storyFlags = ExtractStoryFlags();
            var regions = ExtractRegions(storyFlags);

            IsRegionUsed.Clear();
            foreach (var region in regions)
            {
                IsRegionUsed.Add(region.RoomIndex, false);
            }

            var offset = 0;
            foreach (var slot in Slots)
            {
                switch (slot)
                {
                    case SlotTypes.Overworld:
                        offset += ExtractTreasureLocations(items, offset);
                        break;
                    case SlotTypes.Trainers:
                        offset += ExtractTrainerLocations(pokemon, moves, items, regions, offset);
                        break;
                    case SlotTypes.Pokespots:
                        offset += ExtractPokeSpotLocations(pokemon, offset);
                        break;
                    case SlotTypes.TutorMoves:
                        offset += ExtractTutorMoveLocations(moves, offset);
                        break;
                    case SlotTypes.Bingo:
                        //ExtractBingo();
                        break;
                }
            }

            ExtractReferences(pokemon, items);

            var stringBuilder = new StringBuilder();
            foreach (var kvp in IsRegionUsed)
            {
                if (!kvp.Value)
                {
                    var room = regions.Find(i => i.RoomIndex == kvp.Key);
                    stringBuilder.AppendLine($"{room.AreaName},{room.RoomIndex}");
                }
            }
            File.WriteAllText($"{OutputDirectory}/regions.csv", stringBuilder.ToString());

            //File.WriteAllText($"{OutputDirectory}/regions.json", JsonSerializer.Serialize(regions, Options));
        }

        private void ExtractBingo()
        {
            if (GameExtractor is XDExtractor xd) 
            {
                File.WriteAllText($"{LocationDirectory}/bingo_cards.json", JsonSerializer.Serialize(xd.ExtractBattleBingoCards(), Options));
            }
        }


        private int ExtractTutorMoveLocations(Move[] moves, int offset)
        {
            if (GameExtractor is XDExtractor xd)
            {
                var tutorMoves = xd.ExtractTutorMoves();
                List<TutorMoveLocationJson> tutorMoveLocations = new List<TutorMoveLocationJson>();
                List<TutorMoveItemJson> tutorMoveItems = new List<TutorMoveItemJson>();
                var itemClass = new[] { ItemClassification.Useful.ToString() };
                var tutorRoom = Room.FromId(135, ISO); 
                var tutorRoomName = ReplaceInvalidCharacters(tutorRoom.Name);
                foreach (var tutorMove in tutorMoves)
                {
                    tutorMoveLocations.Add(new TutorMoveLocationJson 
                    {
                        Index = offset,
                        AreaName = tutorRoomName,
                        RoomId = tutorRoom.RoomId, 
                        TutorMoveIndex = tutorMove.Index, 
                        MoveId = tutorMove.Move, 
                        Availability = tutorMove.Availability, 
                        Name = $"Tutor Move {tutorMove.Index} ({moves[tutorMove.Move].Name})" 
                    });

                    tutorMoveItems.Add(new TutorMoveItemJson 
                    { 
                        Index = offset, 
                        ItemClassification = itemClass, 
                        Quantity = 1, 
                        TutorMoveIndex = tutorMove.Index, 
                        Name = $"Tutor Move {tutorMove.Index}" 
                    });

                    offset += 1;
                }

                if (!IsRegionUsed[tutorRoom.RoomId])
                {
                    IsRegionUsed[tutorRoom.RoomId] = true;
                }

                File.WriteAllText($"{LocationDirectory}/tutor_moves.json", JsonSerializer.Serialize(tutorMoveLocations, Options));
                File.WriteAllText($"{ItemsDirectory}/tutor_moves.json", JsonSerializer.Serialize(tutorMoveItems, Options));
            }
            return offset;
        }

        private int ExtractPokeSpotLocations(Pokemon[] pokemon, int offset)
        {
            if (GameExtractor is XDExtractor xd)
            {
                var pokeSpots = xd.ExtractPokeSpotPokemon().SkipLast(2);
                var pokeSpotLocations = new List<PokeSpotLocationJson>();
                var pokeSpotItems = new List<PokeSpotItemJson>();
                var itemClass = new[] { ItemClassification.Useful.ToString() };
                var pokeSpotRooms = Enum.GetValues<PokeSpotType>().SkipLast(1).Select(p => Room.FromId(90 + (int)p, ISO)).ToList();

                foreach (var pokeSpot in pokeSpots)
                {
                    var room = pokeSpotRooms[(int)pokeSpot.PokeSpot.PokeSpotType];
                    var roomName = ReplaceInvalidCharacters(room.Name);
                    pokeSpotLocations.Add(new PokeSpotLocationJson 
                    { 
                        Index = offset,
                        AreaName = roomName, 
                        RoomId = room.RoomId, 
                        PokemonIndex = pokeSpot.Pokemon, 
                        PokeSpot = pokeSpot.PokeSpot.PokeSpotType.ToString(), 
                        PokeSpotIndex = pokeSpot.PokeSpot.Index, 
                        Name = $"{pokeSpot.PokeSpot.PokeSpotType.ToString()} PokeSpot - Capture {pokemon[pokeSpot.Pokemon].Name}" 
                    });
                    pokeSpotItems.Add(new PokeSpotItemJson 
                    { 
                        Index = offset,
                        ItemClassification = itemClass, 
                        PokemonIndex = pokeSpot.Pokemon, 
                        Name = $"{pokeSpot.PokeSpot.PokeSpotType.ToString()} PokeSpot - {pokemon[pokeSpot.Pokemon].Name}" 
                    });
                    offset++;

                    if (!IsRegionUsed[room.RoomId])
                    {
                        IsRegionUsed[room.RoomId] = true;
                    }
                }

                File.WriteAllText($"{LocationDirectory}/pokespot_pokemon.json", JsonSerializer.Serialize(pokeSpotLocations, Options));
                File.WriteAllText($"{ItemsDirectory}/pokespot_pokemon.json", JsonSerializer.Serialize(pokeSpotItems, Options));
            }
            return offset;
        }

        private int ExtractTrainerLocations(Pokemon[] pokemonReference, Move[] moveReference, Items[] items, List<RegionsJson> areas, int offset)
        {
            List<TrainerBattleLocationJson> battleLocationJsons = new List<TrainerBattleLocationJson>();
            List<ShadowPokemonLocationJson> shadowPokemonLocationJsons = new List<ShadowPokemonLocationJson>();
            List<PurifyShadowPokemonLocationJson> purifyPokemonLocationJsons = new List<PurifyShadowPokemonLocationJson>();

            List<TrainerBattleItemJson> battleItemJsons = new List<TrainerBattleItemJson>();
            List<ShadowPokemonItemJson> shadowPokemonItemJsons = new List<ShadowPokemonItemJson>();
            List<PurifyShadowPokemonItemJson> purifyPokemonItemJsons = new List<PurifyShadowPokemonItemJson>();

            var trainerPools = GameExtractor.ExtractPools(pokemonReference, moveReference);
            var battlesRandomized = new List<int>();
            var itemClass = new[] { ItemClassification.Useful.ToString() };
            foreach (var trainerPool in trainerPools)
            {
                BattleTypes[] battleTypes = GetBattleTypesForTrainerPool(trainerPool);
                if (battleTypes.Length == 0)
                {
                    continue;
                }

                foreach (var trainer in trainerPool.AllTrainers)
                {
                    if (!trainer.IsSet)
                    {
                        continue;
                    }

                    var battle = XDBattle.FindPlayerBattleFromTrainer(trainer.Index, battleTypes, ISO);
                    if (battle == null || battlesRandomized.Contains(battle.Index))
                    {
                        continue;
                    }

                    Room room = Room.FromId(new BattleField(battle.BattleFieldId, ISO).RoomId, ISO);
                    if (room == null)
                    {
                        continue;
                    }

                    if (!IsRegionUsed[room.RoomId])
                    {
                        IsRegionUsed[room.RoomId] = true;
                    }

                    var battleTypeName = trainerPool.TeamType switch
                    {
                        TrainerPoolType.Colosseum => "THE COLOSSEUM",
                        TrainerPoolType.Hundred => $"MT. BATTLE (Round {trainer.Index})",
                        TrainerPoolType.Virtual => battle?.BattleType == BattleTypes.BattleCd ? $"BATTLE CD" : $"{battle?.BattleType}",
                        _ => ReplaceInvalidCharacters(room.Name.ToString())
                    };

                    battleLocationJsons.Add(new TrainerBattleLocationJson
                    {
                        Index = offset,
                        AreaName = battleTypeName,
                        RoomId = room.RoomId,
                        TrainerIndex = trainer.Index,
                        Name = $"Defeat {trainer.Name} at {battleTypeName}", //{.Name}"
                        BattleIndex = battle?.Index ?? 0,
                        TrainerBattleType = trainerPool.TeamType switch
                        {
                            TrainerPoolType.Colosseum => "Colosseum",
                            TrainerPoolType.Hundred => "Mt. Battle",
                            TrainerPoolType.Virtual => "Battle CD",
                            _ => trainerPool.TeamType.ToString()
                        }
                    });

                    var trainerClass = XDTrainerClass.FromTrainer(trainer as XDTrainer, ISO);
                    var payout = (trainerClass.Payout * trainer.Pokemon.Max(p => p.Level) * 2);
                    battleItemJsons.Add(new TrainerBattleItemJson
                    {
                        Index = offset,
                        Quantity = payout,
                        Name = $"{payout} PokeDollars",
                        ItemClassification = payout > 1000 ? itemClass : new[] { ItemClassification.Filler.ToString() }
                    });

                    offset++;

                    if (trainerPool.TeamType == TrainerPoolType.Story)
                    {
                        foreach (var pokemon in trainer.Pokemon)
                        {
                            if (pokemon.IsSet && pokemon.IsShadow)
                            {
                                var shadowPokemonLocation = new ShadowPokemonLocationJson
                                {
                                    Index = offset,
                                    AreaName = battleTypeName,
                                    RoomId = room.RoomId,
                                    TrainerIndex = trainer.Index,
                                    Name = $"Capture {trainer.Name}s Shadow {pokemonReference[pokemon.Pokemon].Name}",
                                    ShadowIndex = ((XDTrainerPokemon)pokemon).DPKMIndex
                                };

                                var shadowPokemonItem = new ShadowPokemonItemJson
                                {
                                    Index = offset,
                                    Quantity = 1,
                                    PokemonIndex = pokemon.Pokemon,
                                    Name = $"{pokemonReference[pokemon.Pokemon].Name}",
                                    ItemClassification = GetHighestBST(pokemonReference, pokemon.Pokemon) > 450 ? itemClass : new[] { ItemClassification.Filler.ToString() }
                                };

                                offset++;

                                var purifyShadowPokemonLocation = new PurifyShadowPokemonLocationJson
                                {
                                    Index = offset,
                                    AreaName = battleTypeName,
                                    RoomId = room.RoomId,
                                    Name = $"Purify {trainer.Name}s Shadow {pokemonReference[pokemon.Pokemon].Name}",
                                    ShadowIndex = ((XDTrainerPokemon)pokemon).DPKMIndex
                                };

                                // let ap server decide
                                var purifyShadowPokemonItem = new PurifyShadowPokemonItemJson
                                {
                                    Index = offset,
                                    Quantity = 0,
                                    ItemIndex = 0,
                                    Name = $"{pokemonReference[pokemon.Pokemon].Name}",
                                    ItemClassification = itemClass
                                };

                                // zook vs ardos gets picked up as a story battle with shadow pokemon
                                // chec
                                var duplicateShadowIndex = shadowPokemonLocationJsons.FindIndex(p => p.ShadowIndex == shadowPokemonLocation.ShadowIndex);
                                if (duplicateShadowIndex >= 0)
                                {
                                    shadowPokemonLocationJsons[duplicateShadowIndex] = shadowPokemonLocation;
                                    shadowPokemonItemJsons[duplicateShadowIndex] = shadowPokemonItem;
                                    purifyPokemonLocationJsons[duplicateShadowIndex] = purifyShadowPokemonLocation;
                                    purifyPokemonItemJsons[duplicateShadowIndex] = purifyShadowPokemonItem;
                                }
                                else
                                {
                                    shadowPokemonLocationJsons.Add(shadowPokemonLocation);
                                    shadowPokemonItemJsons.Add(shadowPokemonItem);
                                    purifyPokemonLocationJsons.Add(purifyShadowPokemonLocation);
                                    purifyPokemonItemJsons.Add(purifyShadowPokemonItem);
                                }
                            }

                        }
                    }

                    battlesRandomized.Add(battle.Index);
                }
            }

            File.WriteAllText($"{LocationDirectory}/trainers.json", JsonSerializer.Serialize(battleLocationJsons, Options));
            File.WriteAllText($"{LocationDirectory}/shadow_pokemon.json", JsonSerializer.Serialize(shadowPokemonLocationJsons, Options));
            File.WriteAllText($"{LocationDirectory}/purify_pokemon.json", JsonSerializer.Serialize(purifyPokemonLocationJsons, Options));

            File.WriteAllText($"{ItemsDirectory}/trainers.json", JsonSerializer.Serialize(battleItemJsons, Options));
            File.WriteAllText($"{ItemsDirectory}/shadow_pokemon.json", JsonSerializer.Serialize(shadowPokemonItemJsons, Options));
            File.WriteAllText($"{ItemsDirectory}/purify_pokemon.json", JsonSerializer.Serialize(purifyPokemonItemJsons, Options));

            return offset;
        }

        private int GetHighestBST(Pokemon[] pokemonList, ushort pokemonIndex)
        {
            var pokemon = pokemonList[pokemonIndex];
            var highestBst = pokemon.BST;
            while (true)
            {
                var isSplitOrEnd = Helpers.CheckForSplitOrEndEvolution(pokemon, out int count);
                if (isSplitOrEnd)
                {
                    if (count > 0)
                    {
                        for (int i = 0; i < count; i++)
                        {
                            var evolvedPokemon = pokemonList[pokemon.Evolutions[i].EvolvesInto];
                            if (evolvedPokemon.BST > highestBst)
                            {
                                highestBst = evolvedPokemon.BST;
                            }
                        }
                    }
                    return highestBst;
                }
                else
                {
                    var evolvedPokemon = pokemonList[pokemon.Evolutions[0].EvolvesInto];
                    if (evolvedPokemon.BST > highestBst)
                    {
                        highestBst = evolvedPokemon.BST;
                    }
                    pokemon = evolvedPokemon;
                }
            }
        }

        private static BattleTypes[] GetBattleTypesForTrainerPool(ITrainerPool? trainerPool)
        {
            BattleTypes[] battleTypes = trainerPool.TeamType switch
            {
                TrainerPoolType.Story => new[] { BattleTypes.Story, BattleTypes.MirorBPokespot, BattleTypes.StoryAdminColo },
                TrainerPoolType.Hundred => new[] { BattleTypes.MtBattle, BattleTypes.MtBattleFinal },
                TrainerPoolType.Colosseum => new[] { BattleTypes.ColosseumFinal, BattleTypes.ColosseumOrreFinal, BattleTypes.ColosseumOrrePrelim, BattleTypes.ColosseumPrelim },
                //TrainerPoolType.Bingo => new[] { BattleTypes.BattleBingo },
                //TrainerPoolType.Virtual => new[] { BattleTypes.BattleCd, BattleTypes.BattleTraining, BattleTypes.BattleMode, BattleTypes.LinkBattle },
                _ => Array.Empty<BattleTypes>()
            };
            return battleTypes;
        }

        public int ExtractTreasureLocations(Items[] items, int offset)
        {
            var treasureLocations = new List<TreasureLocationJson>();
            var treasureItems = new List<TreasureItemJson>();

            foreach (var treasure in GameExtractor.ExtractOverworldItems().SkipLast(2))
            {
                if (treasure.Model == TreasureTypes.NotSet)
                {
                    continue;
                }

                var room = Room.FromId(treasure.TreasureRoom, ISO);
                var item = items.FirstOrDefault(it => it.OriginalIndex == treasure.Item);

                if (item == null) { continue; }

                var roomName = ReplaceInvalidCharacters(room.Name);
                var itemName = ReplaceInvalidCharacters(item.Name.ToString());

                var itemClass = new List<string>();
                if (item.BagSlot == BagSlots.KeyItems)
                {
                    itemClass.Add(ItemClassification.Progression.ToString());
                }
                else if (item.BagSlot == BagSlots.Tms || item.Price > 1000 || item.BagSlot == BagSlots.Pokeballs)
                {
                    itemClass.Add(ItemClassification.Useful.ToString());
                }

                if (treasure.Model == TreasureTypes.Sparkle)
                {
                    treasureLocations.Add(new TreasureLocationJson
                    {
                        Index = offset,
                        AreaName = roomName,
                        RoomId = room.RoomId,
                        TreasureIndex = treasure.index,
                        Name = $"{roomName} - Sparkling Item ({itemName} x {treasure.Quantity})",
                        IsSparkling = true
                    });

                    treasureItems.Add(new TreasureItemJson
                    {
                        Index = offset,
                        ItemIndex = treasure.Item,
                        Quantity = treasure.Quantity,
                        Name = $"{item?.Name} x {treasure.Quantity}",
                        ItemClassification = itemClass.ToArray()
                    });
                }
                else
                {
                    treasureLocations.Add(new TreasureLocationJson
                    {
                        Index = offset,
                        AreaName = roomName,
                        RoomId = room.RoomId,
                        TreasureIndex = treasure.index,
                        Name = $"{roomName} - Treasure Chest Item ({itemName} x {treasure.Quantity})",
                        IsSparkling = false
                    });

                    treasureItems.Add(new TreasureItemJson
                    {
                        Index = offset,
                        ItemIndex = treasure.Item,
                        Quantity = treasure.Quantity,
                        Name = $"{item?.Name} x {treasure.Quantity}",
                        ItemClassification = itemClass.ToArray()
                    });
                }
                offset++;

                if (!IsRegionUsed[room.RoomId])
                {
                    IsRegionUsed[room.RoomId] = true;
                }
            }
            File.WriteAllText($"{LocationDirectory}/treasure_items.json", JsonSerializer.Serialize(treasureLocations, Options));
            File.WriteAllText($"{ItemsDirectory}/treasure_items.json", JsonSerializer.Serialize(treasureItems, Options));

            return offset;
        }

        private void ExtractReferences(Pokemon[] pokemon, Items[] items)
        {
            var itemsJson = items.Where(i => i.BagSlot != BagSlots.None && i.NameId != 0).Select(i => new { Index = i.OriginalIndex, Name = ReplaceInvalidCharacters(i.Name.ToString()), BagSlot = i.BagSlot.ToString() }).SkipLast(1);
            var pokemonJson = pokemon.Where(p => p.NameID != 0).Select(p => new { p.Index, p.Name });

            File.WriteAllText($"{OutputDirectory}/all_items.json", JsonSerializer.Serialize(itemsJson, Options));
            File.WriteAllText($"{OutputDirectory}/all_pokemon.json", JsonSerializer.Serialize(itemsJson, Options));
        }

        string AddSpacesToSentence(string text, bool preserveAcronyms)
        {
            if (string.IsNullOrWhiteSpace(text))
                return string.Empty;
            StringBuilder newText = new StringBuilder(text.Length * 2);
            newText.Append(text[0]);
            for (int i = 1; i < text.Length; i++)
            {
                if (char.IsUpper(text[i]))
                    if ((text[i - 1] != ' ' && !char.IsUpper(text[i - 1])) ||
                        (preserveAcronyms && char.IsUpper(text[i - 1]) &&
                         i < text.Length - 1 && !char.IsUpper(text[i + 1])))
                        newText.Append(' ');
                newText.Append(text[i]);
            }
            return newText.ToString();
        }

        private List<StoryFlagEventJson> ExtractStoryFlags()
        {
            var storyFlagEventList = new List<StoryFlagEventJson>();
            foreach (var storyFlag in Enum.GetValues<XDStoryFlags>().Skip(1))
            {
                storyFlagEventList.Add(new StoryFlagEventJson
                {
                    StoryFlag = (int)storyFlag,
                    Name = AddSpacesToSentence(storyFlag.ToString(), true)
                });
            }
            File.WriteAllText($"{OutputDirectory}/story_flags.json", JsonSerializer.Serialize(storyFlagEventList, Options));
            return storyFlagEventList;
        }

        private List<RegionsJson> ExtractRegions(List<StoryFlagEventJson> storyFlagEvents)
        {
            var regions = new List<RegionsJson>();
            var rooms = GameExtractor.ExtractRooms().Where(r => r.RoomId > 0);
            var areas = GameExtractor.ExtractAreas().Skip(1);

            foreach (var room in rooms)
            {
                var regionJson = new RegionsJson
                {
                    AreaName = ReplaceInvalidCharacters(room.Name),
                    RoomIndex = room.RoomId,
                };

                switch (room.AreaID)
                {
                    case 14:
                        regionJson.AreaName = "COLOSSEUM";
                        break;
                    case 16:
                    case 5:
                    case 6:
                        regionJson.Starting = true;
                        break;
                }

                if (string.IsNullOrEmpty(regionJson.AreaName))
                {
                    var otherRoom = rooms.First(r => r.AreaID == room.AreaID && r.NameID > 0);
                    regionJson.AreaName = ReplaceInvalidCharacters(otherRoom.Name);
                }

                var containsRooms = new List<int>();
                if (string.IsNullOrEmpty(regionJson.Name))
                {
                    regionJson.Name = $"{regionJson.AreaName} - Room {room.RoomId}";
                }

                regions.Add(regionJson);
            }
            return regions;
        }

        private string ReplaceInvalidCharacters(string input)
        {
            return input.Replace("\u0027", string.Empty).Replace("\uFFFD", "E");
        }
    }
}