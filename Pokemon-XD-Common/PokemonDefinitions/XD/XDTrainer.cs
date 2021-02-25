using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using XDCommon.Utility;

namespace XDCommon.PokemonDefinitions
{
    public class XDTrainer : Trainer
    {
        internal const byte ConstSizeOfTrainerData = 0x38;
        internal const byte SizeOfAIData = 0x20;

        const byte TrainerClassNameOffset = 0x05;
        const byte TrainerNameIDOffset = 0x06;
        const byte TrainerClassModelOffset = 0x11;
        const byte TrainerPreBattleTextIDOffset = 0x14;
        const byte TrainerVictoryTextIDOffset = 0x16;
        const byte TrainerDefeatTextIDOffset = 0x18;
        const byte FirstTrainerPokemonOffset = 0x1C;
        const ushort TrainerAIOffset = 0x28;

        public override int NameID
        {
            get => pool.ExtractedFile.GetIntAtOffset(StartOffset + TrainerNameIDOffset);
            set => pool.ExtractedFile.WriteBytesAtOffset(StartOffset + TrainerNameIDOffset, value.GetBytes());
        }

        public ushort PreBattleTextID
        {
            get => pool.ExtractedFile.GetUShortAtOffset(StartOffset + TrainerPreBattleTextIDOffset);
            set => pool.ExtractedFile.WriteBytesAtOffset(StartOffset + TrainerPreBattleTextIDOffset, value.GetBytes());
        }
        public ushort VictoryTextID
        {
            get => pool.ExtractedFile.GetUShortAtOffset(StartOffset + TrainerPreBattleTextIDOffset);
            set => pool.ExtractedFile.WriteBytesAtOffset(StartOffset + TrainerPreBattleTextIDOffset, value.GetBytes());
        }
        public ushort DefeatTextID
        {
            get => pool.ExtractedFile.GetUShortAtOffset(StartOffset + TrainerVictoryTextIDOffset);
            set => pool.ExtractedFile.WriteBytesAtOffset(StartOffset + TrainerVictoryTextIDOffset, value.GetBytes());
        }

        public XDTrainerClasses TrainerClass
        {
            get => (XDTrainerClasses)pool.ExtractedFile.GetByteAtOffset(StartOffset + TrainerClassNameOffset);
            set => pool.ExtractedFile.WriteByteAtOffset(StartOffset + TrainerClassNameOffset, (byte)value);
        }
        public XDTrainerModels TrainerModel
        {
            get => (XDTrainerModels)pool.ExtractedFile.GetByteAtOffset(StartOffset + TrainerClassModelOffset);
            set => pool.ExtractedFile.WriteByteAtOffset(StartOffset + TrainerClassModelOffset, (byte)value);
        }

        public ushort AI
        {
            get => pool.ExtractedFile.GetUShortAtOffset(StartOffset + TrainerAIOffset);
            set => pool.ExtractedFile.WriteBytesAtOffset(StartOffset + TrainerAIOffset, value.GetBytes());
        }

        public override bool IsSet => TrainerClass != XDTrainerClasses.None;
        public override int SizeOfTrainerData => ConstSizeOfTrainerData;

        protected override uint StartOffset
        {
            get
            {
                return (uint)(pool.DTNRDataOffset + index * SizeOfTrainerData);
            }
        }

        public XDTrainer(int index, XDTrainerPool trainers, ISO iso) : base(index, trainers, iso)
        {
            var mask = ShadowMask;
            for (int i = 0; i < Pokemon.Length; i++)
            {
                var id = pool.ExtractedFile.GetUShortAtOffset(StartOffset + FirstTrainerPokemonOffset + i * 2);
                var m = mask % 2;
                Pokemon[i] = new XDTrainerPokemon(
                    id, 
                    trainers, 
                    m == 1 ? PokemonFileType.DDPK : PokemonFileType.DPKM
                );
                mask >>= 1;
            }
        }
    }
}
