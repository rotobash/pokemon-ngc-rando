using System;
using System.Collections.Generic;
using System.Text;
using XDCommon.Contracts;

namespace XDCommon.Patches
{
    public abstract class DolPatch : IDolPatchInfo
    {
        public string Name { get; set; }
        public string Description { get; set; }

        public DolPatch()
        {

        }
    }
}
