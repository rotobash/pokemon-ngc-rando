using XDCommon.Utility;

namespace XDCommon.PokemonDefinitions
{
    public class ColTrainerPool : TrainerPool
    {
        public ColTrainerPool(TrainerPoolType poolType, ISO iso, Pokemon[] pokemonList, Move[] moveList) : base(poolType, iso, pokemonList, moveList)
        {
            ExtractedFile = iso.CommonRel.ExtractedFile;
            var trainerCount = GetEntries(DTNRHeaderOffset);
            var trainers = new ITrainer[trainerCount];
            for (int i = 0; i < trainerCount; i++)
            {
                trainers[i] = new ColTrainer(i, this, iso);
            }
            AllTrainers = trainers;
        }
    }
}