using System;
using System.Collections.Generic;
using System.Text;

namespace XDCommon.PokemonDefinitions
{
    public static partial class Pokemon
    {
        public enum AITypes
        {
            None,
            Defensive,
            Simple,
            Cycle
        }
        
        public enum TrainerTeamTypes
        {
            Bingo,
            Colosseum,
            DarkPokemon,
            Hundred,
            Imasugu,
            Sample,
            Story,
            Virtual
        }

        public static TrainerTeamTypes[] MainTeams = new[] { TrainerTeamTypes.Story, TrainerTeamTypes.Colosseum, TrainerTeamTypes.Hundred, TrainerTeamTypes.Virtual };
        public static TrainerTeamTypes[] Trainers = new[] { TrainerTeamTypes.Story, TrainerTeamTypes.Colosseum, TrainerTeamTypes.Hundred, TrainerTeamTypes.Virtual, TrainerTeamTypes.Imasugu, TrainerTeamTypes.Bingo, TrainerTeamTypes.Sample };
    }
}
