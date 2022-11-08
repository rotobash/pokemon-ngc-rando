using System;
using System.Collections.Generic;
using System.Text;
using XDCommon.Contracts;
using XDCommon.Utility;

namespace XDCommon.PokemonDefinitions
{
    public class Ability
    {
        readonly ISO iso;
        readonly int startOffset;
        readonly int sizeOfAbilityEntry;
        readonly int abilityNameIDOffset;
        readonly int abilityDescriptionIDOffset;

        public int Index { get; }
        public int NameIdOffset => startOffset + (Index * sizeOfAbilityEntry) + abilityNameIDOffset;
        public int NameId => iso.DOL.ExtractedFile.GetIntAtOffset(NameIdOffset);
        public string Name => iso.CommonRelStringTable.GetStringWithId(NameId).ToString();

        public int DescriptionIdOffset => startOffset + (Index * sizeOfAbilityEntry) + abilityDescriptionIDOffset;
        public int DescriptionId => iso.DOL.ExtractedFile.GetIntAtOffset(DescriptionIdOffset);
        public string Description => iso.CommonRelStringTable.GetStringWithId(DescriptionId).ToString();

        public Ability(int index, ISO iso)
        {
            Index = index;
            this.iso = iso;
            startOffset = Constants.AbilityStartOffset(iso);
            sizeOfAbilityEntry = Constants.SizeOfAbilityEntry(iso);
            abilityNameIDOffset = Constants.AbilityNameIDOffset(iso);
            abilityDescriptionIDOffset = Constants.AbilityDescriptionIDOffset(iso);
        }
    }
}
