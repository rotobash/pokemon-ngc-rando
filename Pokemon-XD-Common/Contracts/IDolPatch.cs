using System;
using System.Collections.Generic;
using System.Text;

namespace XDCommon.Contracts
{
    public interface IDolPatchInfo
    {
        string Name { get; set; }
        string Description { get; set; }
    }

    public interface IColoDolPatch
    {
        bool CheckColosseum(Region region);
        void PatchColosseum(Region region);
    }

    public interface IXDDolPatch
    {
        bool CheckXD(Region region);
        void PatchXD(Region region);
    }
}
