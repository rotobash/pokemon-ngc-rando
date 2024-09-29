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

namespace RandomizerCLI.Manipulators
{
    public class TrainerPokemonJson
    {
        public string Name { get; set; } = string.Empty;
        public int Level { get; set; }
        public IEnumerable<string> Moves { get; set; } = Array.Empty<string>();

        public int ShadowLevel { get; set; }
        public int ShadowCatchRate { get; set; }
    }

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

    public class JsonExtractor : GameManipulator
    {
        string OutputDirectory { get; }
        IEnumerable<SlotTypes> Slots { get; }

        public JsonExtractor(ExtractOptions options): base(options)
        {
            if (!Directory.Exists(options.OutputPath))
            {
                Directory.CreateDirectory(options.OutputPath);
            }

            OutputDirectory = options.OutputPath;
            Slots = options.Slots;
        }

        public void Extract()
        {
            var serializeOptions = new JsonSerializerOptions
            {
                WriteIndented = true,
                Converters =
                {
                    new UnicodeStringJsonConverter()
                }
            };

            var rooms = GameExtractor.ExtractRooms();
            var pokemon = GameExtractor.ExtractPokemon();
            var moves = GameExtractor.ExtractMoves();
            var items = GameExtractor.ExtractItems().Where(i => i.BagSlot != BagSlots.None).Select(i =>
                new
                {
                    Index = i.OriginalIndex,
                    i.Name,
                    i.Description,
                    BagSlot = i.BagSlot.ToString(),
                    i.InBattleUseID,
                    i.Price,
                    i.CouponPrice,
                });

            XDExtractor xd = GameExtractor as XDExtractor;

            foreach (var slot in Slots)
            {
                switch (slot)
                {
                    case SlotTypes.Overworld:
                        var overworldItems = new List<object>();
                        foreach (var i in GameExtractor.ExtractOverworldItems())
                        {
                            var room = rooms.FirstOrDefault(r => r.RoomId == i.TreasureRoom);
                            var new_item = new { Index = i.index, Item = items.FirstOrDefault(it => it.Index == i.Item), i.Quantity, TreasureType = ((TreasureTypes)i.Model).ToString(), TreasureRoom = new { room?.Name, room?.AreaID, room?.RoomId } };
                            overworldItems.Add(new_item);
                        }
                        File.WriteAllBytes($"{OutputDirectory}/overworld_items.json", Encoding.UTF8.GetBytes(JsonSerializer.Serialize(overworldItems, serializeOptions)));
                        break;
                    case SlotTypes.Areas:
                        var areas = GameExtractor.ExtractAreas()
                            .Select(a => new
                            {
                                a.AreaID,
                                rooms.FirstOrDefault(r => r.AreaID == a.AreaID)?.Name,
                                EnterRoom = a.RoomId
                            });
                        File.WriteAllBytes($"{OutputDirectory}/areas.json", Encoding.UTF8.GetBytes(JsonSerializer.Serialize(areas, serializeOptions)));
                        break;
                    case SlotTypes.Pokemon:
                        File.WriteAllBytes($"{OutputDirectory}/pokemon.json", Encoding.UTF8.GetBytes(JsonSerializer.Serialize(pokemon, serializeOptions)));
                        break;
                    case SlotTypes.Moves:
                        File.WriteAllBytes($"{OutputDirectory}/moves.json", Encoding.UTF8.GetBytes(JsonSerializer.Serialize(moves, serializeOptions)));
                        break;
                    case SlotTypes.Trainers:
                        var trainers = GameExtractor.ExtractPools(pokemon, moves)
                            .SelectMany(p =>
                                p.AllTrainers
                                    .Where(t => t.IsSet)
                                    .Select(t => new
                                    {
                                        TeamType = p.TeamType.ToString(),
                                        t.Index,
                                        t.Name,
                                        t.IsSet,
                                        Pokemon = t.Pokemon.Select(p =>
                                        {
                                            if (p.IsShadow && p is IShadowPokemon shadowPokemon)
                                            {
                                                return new TrainerPokemonJson
                                                {
                                                    Name = pokemon.First(pk => pk.Index == p.Pokemon).Name,
                                                    Level = p.Level,
                                                    ShadowLevel = shadowPokemon.ShadowLevel,
                                                    ShadowCatchRate = shadowPokemon.ShadowCatchRate,
                                                    Moves = p.Moves.Select(m => moves.FirstOrDefault(mv => mv.MoveIndex == m)?.Name?.ToString())
                                                };

                                            }
                                            else
                                            {
                                                return new TrainerPokemonJson
                                                {
                                                    Name = pokemon.First(pk => pk.Index == p.Pokemon).Name,
                                                    Level = p.Level,
                                                    Moves = p.Moves.Select(m => moves.FirstOrDefault(mv => mv.MoveIndex == m)?.Name?.ToString())
                                                };
                                            }
                                        })
                                    })
                            );
                        File.WriteAllBytes($"{OutputDirectory}/trainers.json", Encoding.UTF8.GetBytes(JsonSerializer.Serialize(trainers, serializeOptions)));
                        break;
                    case SlotTypes.Items:
                        File.WriteAllBytes($"{OutputDirectory}/items.json", Encoding.UTF8.GetBytes(JsonSerializer.Serialize(items, serializeOptions)));
                        break;

                    case SlotTypes.Pokespots:
                        if (xd != null)
                        {
                            File.WriteAllBytes($"{OutputDirectory}/pokespot_pokemon.json", Encoding.UTF8.GetBytes(JsonSerializer.Serialize(xd.ExtractPokeSpotPokemon(), serializeOptions)));
                        }
                        break;
                    case SlotTypes.TutorMoves:
                        if (xd != null)
                        {
                            var tutorMoves = xd.ExtractTutorMoves().Select(t => new { t.Index, Move = moves[t.Move], t.Availability, t.TutorStartOffset });
                            File.WriteAllBytes($"{OutputDirectory}/tutor_moves.json", Encoding.UTF8.GetBytes(JsonSerializer.Serialize(tutorMoves, serializeOptions)));
                        }
                        break;
                    case SlotTypes.Bingo:
                        if (xd != null)
                        {
                            File.WriteAllBytes($"{OutputDirectory}/bingo_cards.json", Encoding.UTF8.GetBytes(JsonSerializer.Serialize(xd.ExtractBattleBingoCards(), serializeOptions)));
                        }
                        break;
                }
            }
        }
    }
}
