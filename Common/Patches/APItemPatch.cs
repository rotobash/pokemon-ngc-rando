using System;
using System.Collections.Generic;
using System.Text;
using XDCommon.Contracts;
using XDCommon.Utility;

namespace XDCommon.Patches
{
    public class APItemPatch : DolPatch
    {
        public override byte[] PatchInstructions { get; }

        EntryPoint[] ItemGiveRoutines = new EntryPoint[]
        {
            // Function for "Found item" text display
            new EntryPoint
            {
                Pointer = 0,
            },
            // Function for "Obtained item" text display
            new EntryPoint
            {
                Pointer = 0x8014CAE8,
                Source = 29,
            },
            // Function for item give
            new EntryPoint
            {
                Pointer = 0,
            },
        };

        public APItemPatch()
        {
            Name = "AP Item Patch";
            Description = "This patch changes item give routines in order to be detected by the AP client. Will set quantity to ushort.MaxValue as a signal";
            GameEntryPoints.Add(Game.XD, new RegionEntryPoints
            {
                { Region.US, ItemGiveRoutines },
            });

            var newInstructions = new List<byte>();
            newInstructions.AddRange(PowerPCInstruction.LoadImmediate(30, ushort.MaxValue));
        }

    }
}
