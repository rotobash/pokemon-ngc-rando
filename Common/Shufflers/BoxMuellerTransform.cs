using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XDCommon.Shufflers
{
    public static class BoxMuellerTransform
    {
        public static double Sample(this AbstractRNG random, double mean = 0, double sigma2 = 1)
        {
            var u1 = random.NextDouble();
            var u2 = random.NextDouble();

            // this equation is approximately normally distributed
            var nSample = Math.Sqrt(-2 * Math.Log(u1)) * Math.Cos(2 * Math.PI * u2);
            return (Math.Sqrt(sigma2) * nSample) + mean;
        }
    }
}
