using APCommon.JSON;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace APCommon.Memory
{
    public enum APLocationType { Item, Pokemon, Purify, Battle }
    public class APLocationReferences
    {
        public List<LocationJson> NPCItemLocations = new List<LocationJson>();
        public List<TreasureItemJson> NPCItems = new List<TreasureItemJson>();
        public List<TreasureLocationJson> ItemLocations = new List<TreasureLocationJson>();
        public List<TreasureItemJson> Items = new List<TreasureItemJson>();

        public List<PokemonLocationJson> PokemonLocations = new List<PokemonLocationJson>();
        public List<TrainerBattleLocationJson> TrainerBattleLocations = new List<TrainerBattleLocationJson>();
        public List<PurifyShadowPokemonLocationJson> PurifyShadowPokemonLocations = new List<PurifyShadowPokemonLocationJson>();

        public Dictionary<int, string> nameToId = new Dictionary<int, string>();

        public APLocationReferences()
        {
            Items = JsonSerializer.Deserialize<List<TreasureItemJson>>(File.ReadAllText("Data/items/treasure_items.json"));
            ItemLocations = JsonSerializer.Deserialize<List<TreasureLocationJson>>(File.ReadAllText("Data/locations/treasure_items.json"));

            foreach (var location in ItemLocations)
            {
                nameToId.Add(location.Index, location.Name);
            }

            PokemonLocations = JsonSerializer.Deserialize<List<PokemonLocationJson>>(File.ReadAllText("Data/locations/shadow_pokemon.json"));
            PokemonLocations.AddRange(JsonSerializer.Deserialize<List<PokemonLocationJson>>(File.ReadAllText("Data/locations/pokespot_pokemon.json")));
            foreach (var location in PokemonLocations)
            {
                nameToId.Add(location.Index, location.Name);
            }

            TrainerBattleLocations = JsonSerializer.Deserialize<List<TrainerBattleLocationJson>>(File.ReadAllText("Data/locations/trainers.json"));
            foreach (var location in TrainerBattleLocations)
            {
                nameToId.Add(location.Index, location.Name);
            }

            PurifyShadowPokemonLocations = JsonSerializer.Deserialize<List<PurifyShadowPokemonLocationJson>>(File.ReadAllText("Data/locations/purify_pokemon.json"));
            foreach (var location in PurifyShadowPokemonLocations)
            {
                nameToId.Add(location.Index, location.Name);
            }
        }

        public int LookupItem(APItemCheck apItem)
        {
            switch (apItem.Type)
            {
                case 0:
                case 1:
                    var loc = Items.FirstOrDefault(i => i.ItemIndex == apItem.ItemIndex && ItemLocations.Any(l => l.Index == i.Index && apItem.RoomId == l.RoomId));
                    if (loc != null)
                    {
                        return loc.Index;
                    }
                    break;

                case 2:
                    var npcLoc = NPCItems.FirstOrDefault(i => i.ItemIndex == apItem.ItemIndex && NPCItemLocations.Any(l => l.Index == i.Index && apItem.RoomId == l.RoomId));
                    if (npcLoc != null)
                    {
                        return npcLoc.Index;
                    }
                    break;
            }
            return 0;
        }

        public int LookupPokemon(APPokemonItem apItem)
        {
            var locationRef = PokemonLocations.Find(i => i.RoomId == apItem.BattleRoom && i.ShadowIndex == apItem.ShadowIndex && apItem.Species == i.PokemonIndex);
            if (locationRef != null)
            {
                return locationRef.Index;
            }
            return 0;
        }

        public int LookupTrainer(ushort battleIndex)
        {
            var locationRef = TrainerBattleLocations.Find(i => i.BattleIndex == battleIndex);
            if (locationRef != null)
            {
                return locationRef.Index;
            }
            return 0;
        }

        public int LookupPurify(int shadowId)
        {
            var locationRef = PurifyShadowPokemonLocations.Find(i => i.ShadowIndex == shadowId);
            if (locationRef != null)
            {
                return locationRef.Index;
            }
            return 0;
        }
    }
}
