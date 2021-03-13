using XDCommon.Utility;

namespace XDCommon.PokemonDefinitions
{
    public class ColTrainerPool : TrainerPool
    {
        public override TrainerPool DarkPokemon { get => this; set { } }

        public ColTrainerPool(ISO iso, Pokemon[] pokemonList, Move[] moveList) : base(TrainerPoolType.Colosseum, iso, pokemonList, moveList)
        {
            ExtractedFile = iso.CommonRel.ExtractedFile;
            var trainerCount = iso.CommonRel.GetValueAtPointer(Constants.NumberOfTrainers);
            var trainers = new ITrainer[trainerCount];
            for (int i = 0; i < trainerCount; i++)
            {
                trainers[i] = new ColTrainer(i, this, iso);
            }
            AllTrainers = trainers;
        }
    }
}