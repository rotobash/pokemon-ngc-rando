using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APCommon.Memory
{
    public abstract class APMemoryObject
    {
        public abstract byte[] GetBytes();
        public abstract void ReadFromBytes(Span<byte> bytes);
    }
}
