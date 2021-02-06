using System;
using System.Collections.Generic;
using System.Text;
using XDCommon.Contracts;
using XDCommon.Utility;

namespace XDCommon.PokemonDefinitions
{
    public class Ability
    {
        ISO iso;

        int StartOffset
        {
            get
            {
                if (iso.Game == Game.XD)
                {
                    switch (iso.Region)
                    {
                        case Region.US:
                            return 0x3FCC50;
                        case Region.Europe:
                            return 0x437530;
                        case Region.Japan:
                            return 0x3DA310;
                    }
                }
                else
                {
                    switch (iso.Region)
                    {
                        case Region.US:
                            return 0x35C5E0;
                        case Region.Europe:
                            return 0x3A9688;
                        case Region.Japan:
                            return 0x348D20;
                    }
                }
                return 0;
            }
        }

        bool AbilityListUpdated => iso.Game == Game.Colosseum ? false : iso.DOL.ExtractedFile.GetIntAtOffset(StartOffset + 8) != 0;
        public int NumberOfAbilities => AbilityListUpdated ? 0x75 : 0x4E;
        int AbilityNameIDOffset => AbilityListUpdated ? 0 : 4;
        int AbilityDescriptionIDOffset => AbilityListUpdated ? 4 : 8;
        int SizeOfAbilityEntry => AbilityListUpdated ? 8 : 12;

        public int Index { get; }
        public int NameIdOffset => StartOffset + (Index * SizeOfAbilityEntry) + AbilityNameIDOffset;
        public int NameId => iso.DOL.ExtractedFile.GetIntAtOffset(NameIdOffset);
        public string Name => iso.CommonRelStringTable.GetStringWithId(NameId).ToString();

        public int DescriptionIdOffset => StartOffset + (Index * SizeOfAbilityEntry) + AbilityDescriptionIDOffset;
        public int DescriptionId => iso.DOL.ExtractedFile.GetIntAtOffset(DescriptionIdOffset);
        public string Description => iso.CommonRelStringTable.GetStringWithId(DescriptionId).ToString();

        public Ability(int index, ISO iso)
        {
            Index = index;
            this.iso = iso;
        }
    }
}
