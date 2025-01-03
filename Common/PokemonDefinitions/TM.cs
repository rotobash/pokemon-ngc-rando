﻿using System;
using System.Collections.Generic;
using System.Text;
using XDCommon.Contracts;
using XDCommon.Utility;

namespace XDCommon.PokemonDefinitions
{
    public class TM : Items
    {
        const byte SizeOfTMEntry = 0x08;

        public int TMIndex => (Index + 1) - Constants.FirstTMItemIndex;
        public uint TMStartOffset => (uint)(FirstTMListOffset + 6 + ((TMIndex - 1) * SizeOfTMEntry));
        public TM(int index, ISO iso) : base(index, iso)
        {

        }

        int FirstTMListOffset 
        {
            get
            {
                if (iso.Game == Game.XD) return iso.Region switch
                {
                    Region.US => 0x4023A0,
                    Region.Europe => 0x43CC80,
                    _ => 0x3DFA60
                };
                else return iso.Region switch
                {
                    Region.US => 0x365018,
                    Region.Europe => 0x3B20D0,
                    _ => 0x351758
                };
            }
        }

        public ushort Move
        {
            get => iso.DOL.ExtractedFile.GetUShortAtOffset(TMStartOffset);
            set => iso.DOL.ExtractedFile.WriteBytesAtOffset(TMStartOffset, value.GetBytes());
        }
    }
}
