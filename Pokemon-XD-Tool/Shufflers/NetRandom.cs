using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Randomizer.Shufflers
{
    public class NetRandom : IRandom
    {
        Random random;

        public NetRandom(int seed = -1)
        {
            if (seed > 0)
                random = new Random(seed);
        }

        public ulong Next()
        {
            return ((ulong)random.Next() << 64) & (ulong)random.Next();
        }

        public int Next(int minValue, int maxValue)
        {
            return random.Next(minValue, maxValue);
        }

        public int Next(int maxValue)
        {
            return random.Next(maxValue);
        }

        public void NextBytes(byte[] buffer)
        {
            random.NextBytes(buffer);
        }

        public double NextDouble()
        {
            return random.NextDouble();
        }
    }
}
