using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Randomizer.Shufflers
{
    public class Xoroshiro128StarStar : AbstractRNG
    {
        public static ulong s0;
        public static ulong s1;
        static ulong[] JUMP = new ulong[] { 0xdf900294d8f554a5, 0x170865df4b3201fc };
        static ulong[] LONGJUMP = new ulong[] { 0xd2a98b26625eee7b, 0xdddf9b1090aa7ac1 };

        public Xoroshiro128StarStar(ulong seed = 0)
        {
            if (seed == 0)
                seed = (ulong)DateTime.UtcNow.Ticks;

            s0 = seed;
            Jump();
        }

        static ulong rotl(ulong x, int k)
        {
            return (x << k) | (x >> (64 - k));
        }

        public override ulong Next()
        {
            var result = rotl(s0 * 5, 7) * 9;
            var temp_s1 = s1;

            temp_s1 ^= s0;
            s0 = rotl(s0, 24) ^ temp_s1 ^ (temp_s1 << 16);
            s1 = rotl(temp_s1, 37);

            return result;
        }

        public void SetSeed(params ulong[] seed)
        {
            if (seed.Length != 2)
                return;

            s0 = seed[0];
            s1 = seed[1];
        }

        public void Jump(bool longJump = false)
        {
            ulong temp_s0 = s0;
            ulong temp_s1 = s1;
            var jumpConst = longJump ? LONGJUMP : JUMP;

            for (int i = 0; i < jumpConst.Length; i++)
            {
                for (int b = 0; b < 64; b++)
                {
                    if ((jumpConst[i] & 1u << b) != 0)
                    {
                        temp_s0 ^= s0;
                        temp_s1 ^= s1;
                    }
                    Next();
                }
            }
            s0 = temp_s0;
            s1 = temp_s1;
        }
    }
}
