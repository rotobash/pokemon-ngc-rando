using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XDCommon.Shufflers
{
    public abstract class AbstractRNG
    {
        public abstract ulong Next();

        public virtual int Next(int minValue, int maxValue)
        {
            if (minValue > maxValue)
                throw new ArgumentOutOfRangeException(nameof(minValue));

            if (minValue == maxValue) return minValue;

            long diff = maxValue - minValue;
            while (true)
            {
                var rand = (uint)Next();

                long max = 1 + (long)uint.MaxValue;
                long remainder = max % diff;
                if (rand < max - remainder)
                {
                    return (int)(minValue + (rand % diff));
                }
            }
        }

        public virtual int Next(int maxValue)
        {
            if (maxValue < 0)
                throw new ArgumentOutOfRangeException(nameof(maxValue));

            return Next(0, maxValue);
        }

        public virtual T NextElement<T>(IEnumerable<T> list)
        {
            var count = list.Count();
            if (count == 0)
                throw new ArgumentOutOfRangeException(nameof(list));

            var i = Next(0, count);
            return list.ElementAt(i);
        }

        public virtual double NextDouble()
        {
            ulong rand = Next();
            return rand / (1.0 + ulong.MaxValue);
        }

        public virtual void NextBytes(byte[] buffer)
        {
            if (buffer == null) throw new ArgumentNullException(nameof(buffer));

            var index = 0;
            while (index < buffer.Length)
            {
                var randBytes = BitConverter.GetBytes(Next());
                for (int b = 0; b < randBytes.Length; b++)
                {
                    if (index + b >= buffer.Length)
                        break;
                    buffer[index + b] = randBytes[b];
                }

                index += randBytes.Length;
            }
        }
    }
}
