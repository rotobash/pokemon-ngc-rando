using System;
using System.Collections.Generic;
using System.Text;
using XDCommon.Contracts;
using XDCommon.Utility;

namespace XDCommon.Patches
{
    public abstract class DolPatch : IDolPatchInfo
    {
        public abstract byte[] PatchInstructions { get; }
        public Dictionary<Game, RegionEntryPoints> GameEntryPoints { get; } = new Dictionary<Game, RegionEntryPoints>();
        public string Name { get; set; }
        public string Description { get; set; }

        public DolPatch()
        {
        }

        public void InsertPatch(DOL dol, Region region, Game game)
        {
            var entryPoints = GameEntryPoints[game][region];
            foreach (EntryPoint entry in entryPoints)
            {
                var freeSpacePointer = dol.FindFreeSpace(game, region);

                dol.ExtractedFile.WriteBytesAtOffset(entry.Pointer, PowerPCInstruction.Branch(freeSpacePointer));

                dol.ExtractedFile.WriteBytesAtOffset(freeSpacePointer, PatchInstructions);
                freeSpacePointer += (uint)PatchInstructions.Length;

                if (entry.Next is uint returnAdr)
                {
                    dol.ExtractedFile.WriteBytesAtOffset(freeSpacePointer, PowerPCInstruction.Return(returnAdr));
                }
            }
        }
    }
}
