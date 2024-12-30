using System;
using System.Collections.Generic;
using System.Text;

namespace XDCommon.Patches
{
    public static class PowerPCInstruction
    {
        public static byte[] NOP()
        {
            return new byte[]
            {
                0x60,
                0x00,
                0x00,
                0x00
            };
        }

        public static byte[] Branch(uint address)
        {
            return new byte[]
            {
                0,
            };
        }

        public static byte[] Return(uint address)
        {
            return new byte[]
            {
                0,
            };
        }

        public static byte[] LoadImmediate(uint register, uint value)
        {
            return new byte[]
            {
                0,
            };
        }
    }
}
