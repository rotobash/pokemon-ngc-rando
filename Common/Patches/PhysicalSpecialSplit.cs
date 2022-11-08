using System;
using System.Collections.Generic;
using System.Text;
using XDCommon.Contracts;

namespace XDCommon.Patches
{
    class PhysicalSpecialSplit : DolPatch, IColoDolPatch
    {
        const uint MoveDataByte = 0x8863001F;

        public bool CheckColosseum(Region region)
        {
            if (region == Region.US)
            {

            }
            return false;
        }

        public void PatchColosseum(Region region)
        {
            throw new NotImplementedException();
        }
    }
}
