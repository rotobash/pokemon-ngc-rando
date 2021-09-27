using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Randomizer.Shufflers
{
    public class NetRandom : AbstractRNG
    {
        Random random;

        public NetRandom(int seed = -1)
        {
            if (seed > 0)
                random = new Random(seed);
        }

        public override ulong Next()
        {
            var bytes = new byte[sizeof(ulong)];
            random.NextBytes(bytes);
            return BitConverter.ToUInt64(bytes);
        }

        public override int Next(int minValue, int maxValue)
        {
            return random.Next(minValue, maxValue);
        }

        public override int Next(int maxValue)
        {
            return random.Next(maxValue);
        }

        public override void NextBytes(byte[] buffer)
        {
            random.NextBytes(buffer);
        }

        public override double NextDouble()
        {
            return random.NextDouble();
        }
    }
}
