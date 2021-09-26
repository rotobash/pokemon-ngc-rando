using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Randomizer.Shufflers
{
    public interface IRandom
    {
        ulong Next();
        int Next(int minValue, int maxValue);
        int Next(int maxValue);
        double NextDouble();
        void NextBytes(byte[] buffer);
    }
}
