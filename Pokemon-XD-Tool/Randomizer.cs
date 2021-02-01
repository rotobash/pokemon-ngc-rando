using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XDCommon.Utility;

namespace Randomizer
{
    public class Randomizer
    {
        Random random;
        public Randomizer(ISO iso, IGameExtractor extractor, int seed = -1)
        {
            if (seed < 0)
            {
                random = new Random();
            }
            else
            {
                random = new Random(seed);
            }
        }

        public void RandomizeMoves()
        {

        }

        public void RandomizePokemon()
        {

        }

        public void RandomizeTrainers()
        {

        }

        public void RandomizeTMs()
        {

        }

        public void RandomizeTraits()
        {

        }

        public void RandomizeBattleBingo()
        {

        }


    }
}
