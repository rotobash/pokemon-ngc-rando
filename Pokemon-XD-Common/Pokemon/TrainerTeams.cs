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
            DarkPokemon,
            Story,
            Bingo,
            Colosseum,
            Hundred,
            Imasugu,
            Sample,
            Virtual
        }

        public static TrainerTeamTypes[] MainTeams = new[] { TrainerTeamTypes.Story, TrainerTeamTypes.Colosseum, TrainerTeamTypes.Hundred, TrainerTeamTypes.Virtual };
        public static TrainerTeamTypes[] Trainers = new[] { TrainerTeamTypes.Story, TrainerTeamTypes.Colosseum, TrainerTeamTypes.Hundred, TrainerTeamTypes.Virtual, TrainerTeamTypes.Imasugu, TrainerTeamTypes.Bingo, TrainerTeamTypes.Sample };
    }
}
