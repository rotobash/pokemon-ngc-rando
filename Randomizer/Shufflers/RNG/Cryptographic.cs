using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;

namespace Randomizer.Shufflers
{
    public class Cryptographic : AbstractRNG, IDisposable
    {
        private static RandomNumberGenerator rngCsp = RandomNumberGenerator.Create();
        private bool disposedValue;

        protected virtual void DisposeRNG()
        {
            if (!disposedValue)
            {
                rngCsp.Dispose();
                rngCsp = null;
                disposedValue = true;
            }
        }

        ~Cryptographic()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            DisposeRNG();
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            DisposeRNG();
            GC.SuppressFinalize(this);
        }

        public override ulong Next()
        {
            var buffer = new byte[sizeof(ulong)];
            NextBytes(buffer);
            return BitConverter.ToUInt64(buffer);
        }

        public override void NextBytes(byte[] buffer)
        {
            rngCsp.GetBytes(buffer);
        }
    }
}
