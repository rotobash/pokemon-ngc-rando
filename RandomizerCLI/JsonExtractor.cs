using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Common.PokemonDefinitions;
using Common.Utility;

namespace RandomizerCLI
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

    public class JsonExtractor
    {
        public void Extract(IGameExtractor gameExtractor, string outputDirectory)
        {
            var serializeOptions = new JsonSerializerOptions
            {
                WriteIndented = true,
                Converters =
                {
                    new UnicodeStringJsonConverter()
                }
            };

            var rooms = gameExtractor.ExtractRooms();
            var pokemon = gameExtractor.ExtractPokemon();
            var moves = gameExtractor.ExtractMoves();

            if (gameExtractor is XDExtractor xd)
            {
                var tutorMoves = xd.ExtractTutorMoves().Select(t => new { t.Index, Move = moves[t.Move], t.Availability, t.TutorStartOffset });
                File.WriteAllBytes($"{outputDirectory}/tutor_moves.json", Encoding.UTF8.GetBytes(JsonSerializer.Serialize(tutorMoves, serializeOptions)));
                File.WriteAllBytes($"{outputDirectory}/pokespot_pokemon.json", Encoding.UTF8.GetBytes(JsonSerializer.Serialize(xd.ExtractPokeSpotPokemon(), serializeOptions)));
                File.WriteAllBytes($"{outputDirectory}/bingo_cards.json", Encoding.UTF8.GetBytes(JsonSerializer.Serialize(xd.ExtractBattleBingoCards(), serializeOptions)));
            }

            var items = gameExtractor.ExtractItems().Where(i => i.BagSlot != BagSlots.None).Select(i =>
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

            var trainers = gameExtractor.ExtractPools(pokemon, moves)
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

            var areas = gameExtractor.ExtractAreas()
                .Select(a => new 
                { 
                    a.AreaID, 
                    rooms.FirstOrDefault(r => r.AreaID == a.AreaID)?.Name, 
                    EnterRoom = a.RoomId 
                });


            var overworldItems = new List<object>();
            var bingocardindex = 0;
            foreach (var i in gameExtractor.ExtractOverworldItems())
            {
                var room = rooms.FirstOrDefault(r => r.RoomId == i.TreasureRoom);
                var new_item = new { Index = i.index, Item = items.FirstOrDefault(it => it.Index == i.Item), i.Quantity, TreasureType = ((TreasureTypes)i.Model).ToString(), TreasureRoom = new { room?.Name, room?.AreaID, room?.RoomId } };
                overworldItems.Add(new_item);
            }

            //File.WriteAllBytes($"{outputDirectory}/overworld_items.json", Encoding.UTF8.GetBytes(JsonSerializer.Serialize(overworldItems, serializeOptions)));
            //File.WriteAllBytes($"{outputDirectory}/pokemon.json", Encoding.UTF8.GetBytes(JsonSerializer.Serialize(pokemon, serializeOptions)));
            //File.WriteAllBytes($"{outputDirectory}/trainers.json", Encoding.UTF8.GetBytes(JsonSerializer.Serialize(trainers, serializeOptions)));
            //File.WriteAllBytes($"{outputDirectory}/items.json", Encoding.UTF8.GetBytes(JsonSerializer.Serialize(items, serializeOptions)));
            //File.WriteAllBytes($"{outputDirectory}/areas.json", Encoding.UTF8.GetBytes(JsonSerializer.Serialize(areas, serializeOptions)));
            File.WriteAllBytes($"{outputDirectory}/moves.json", Encoding.UTF8.GetBytes(JsonSerializer.Serialize(moves, serializeOptions)));
        }
    }
}
