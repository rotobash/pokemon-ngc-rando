using System;
using System.Collections.Generic;
using System.Text;
using XDCommon.Contracts;
using XDCommon.Utility;

namespace XDCommon.PokemonDefinitions
{
    public class TutorMove
    {
        const byte SizeOfTutorMoveEntry = 0x0C;
        const byte AvailabilityFlagOffset = 0x07;

        static readonly int[] DOLOffsets = new int[] {
		    0x1C2ECA, // 0
		    0x1C2EBE, // 1
		    0x1C2ED6, // 2
		    0x1C2EEE, // 3
            0x1C2EA6, // 4
		    0x1C2EB2, // 5
		    0x1C2F06, // 6
		    0x1C2F2A, // 7
		    0x1C2F1E, // 8
		    0x1C2F12, // 9
		    0x1C2EE2, // 10
		    0x1C2EFA, // 11
        };

        const int FirstUSDOLOffset = 0x1C2EA6;
        int FirstDOLOffset()
        {
            return iso.Region switch
            {
                Region.US => FirstUSDOLOffset,
                Region.Japan => 0x1BE3B6,
                Region.Europe => 0x1C47A2,
                _ => 0,
            };
        }

        public int Index { get; }
        private ISO iso;

        public TutorMove(int index, ISO iso)
        {
            Index = index;
            this.iso = iso;
        }

        public uint TutorStartOffset => (uint)(iso.CommonRel.GetPointer(Constants.TutorMoves) + (Index * SizeOfTutorMoveEntry));

        public ushort Move
        {
            get => iso.CommonRel.ExtractedFile.GetUShortAtOffset(TutorStartOffset);
            set
            { 
                iso.CommonRel.ExtractedFile.WriteBytesAtOffset(TutorStartOffset, value.GetBytes());

                var offsetDifference = FirstDOLOffset() - FirstUSDOLOffset;
                var offset = DOLOffsets[Index] + offsetDifference;
                iso.DOL.ExtractedFile.WriteBytesAtOffset(offset, Move.GetBytes());
            }
        }
        public ushort Availability
        {
            get => iso.CommonRel.ExtractedFile.GetUShortAtOffset(TutorStartOffset+ AvailabilityFlagOffset);
            set => iso.CommonRel.ExtractedFile.WriteBytesAtOffset(TutorStartOffset + AvailabilityFlagOffset, (value / 10).GetBytes());
        }
    }
}
