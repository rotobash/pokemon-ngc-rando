using System;
using System.Collections.Generic;
using System.Text;
using XDCommon.Contracts;
using XDCommon.Utility;

namespace XDCommon.PokemonDefinitions
{
    public class TM : Items
    {
        public int TMIndex => (Index + 1) - Constants.FirstTMItemIndex;
        public uint TMStartOffset => (uint)(FirstTMListOffset + 6 + ((TMIndex - 1) * Constants.SizeOfTMEntry));
        public TM(int index, ISO iso) : base(index, iso)
        {

        }

        int FirstTMListOffset => iso.Region switch
        {
            Region.US => 0x4023A0,
            Region.Europe => 0x43CC80,
            _ => 0x3DFA60
        };

        public ushort Move
        {
            get => iso.DOL.ExtractedFile.GetUShortAtOffset(TMStartOffset);
            set => iso.DOL.ExtractedFile.WriteBytesAtOffset(TMStartOffset, value.GetBytes());
        }
    }

    public class TutorMove : Items
    {
        public TutorMove(int index, ISO iso) : base(index, iso)
        {

        }

        public uint TutorStartOffset => (uint)(iso.CommonRel.GetPointer(Constants.TutorMoves) + ((Index - 1) * Constants.SizeOfTutorMoveEntry));

        public int Move
        {
            get => iso.CommonRel.ExtractedFile.GetUShortAtOffset(TutorStartOffset + Constants.TutorMoveMoveIndexOffset);
            set => iso.CommonRel.ExtractedFile.WriteBytesAtOffset(TutorStartOffset + Constants.TutorMoveMoveIndexOffset, value.GetBytes());
        }
    }
}
