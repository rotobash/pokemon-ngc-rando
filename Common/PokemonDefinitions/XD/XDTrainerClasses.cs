using System;
using System.Collections.Generic;
using System.Text;
using XDCommon.Utility;

namespace XDCommon.PokemonDefinitions
{
	public class XDTrainerClass
	{
		const byte TrainerClassSize = 0xc;
		const byte TrainerNameIdOffset = 0x4;
		const byte TrainerPayoutOffset = 0x0;

		int index;
		ISO iso;

		int StartOffset => (int)(iso.CommonRel.GetPointer(Constants.XDTrainerClasses) + (index * TrainerClassSize));

		public UnicodeString Name
		{
			get => iso.CommonRelStringTable.GetStringWithId(NameId);
		}

		public int NameId 
		{
			get => iso.CommonRel.ExtractedFile.GetIntAtOffset(StartOffset + TrainerNameIdOffset);
			set => iso.CommonRel.ExtractedFile.WriteBytesAtOffset(StartOffset + TrainerNameIdOffset, value.GetBytes());
        }

		public ushort Payout
        {
            get => iso.CommonRel.ExtractedFile.GetUShortAtOffset(StartOffset + TrainerPayoutOffset);
            set => iso.CommonRel.ExtractedFile.WriteBytesAtOffset(StartOffset + TrainerPayoutOffset, value.GetBytes());
        }

		public XDTrainerClass(int index, ISO iso)
		{
			this.index = index;
			this.iso = iso;
		}

		public static XDTrainerClass FromTrainer(XDTrainer trainer, ISO iso)
		{
			return new XDTrainerClass(trainer.TrainerClass, iso);
		}
	}
}