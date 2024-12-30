using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XDCommon.PokemonDefinitions;

namespace RandomizerTests.ReferenceData
{
    public class TrainerPokemonReference
    {
        public string Name { get; set; }
        public bool IsShadow { get; set; }
        public bool IsSet { get; set; }
        public int Level { get; set; }
        public int ShadowLevel { get; set; }
        public int ShadowCatchRate { get; set; }
        public string[] Moves { get; set; } = Array.Empty<string>();
    }
    public class TrainerReference
    {
        public int Index { get; set; }
        public TrainerPoolType TeamType { get; set; }
        public string Name { get; set; }
        public bool IsSet { get; set; }
        public TrainerPokemonReference[] Pokemon { get; set; } = Array.Empty<TrainerPokemonReference>();
    }
}
