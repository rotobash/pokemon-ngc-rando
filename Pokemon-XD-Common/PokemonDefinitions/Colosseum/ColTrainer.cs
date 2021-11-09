using System;
using System.Collections.Generic;
using System.Text;
using XDCommon.Utility;

namespace XDCommon.PokemonDefinitions
{
    public class ColTrainer : Trainer
    {
        const byte ConstSizeOfTrainerData = 0x34;

        const byte TrainerClassNameOffset = 0x03;
        const byte FirstTrainerPokemonOffset = 0x04;
        const byte TrainerAIOffset = 0x06;
        const byte TrainerNameIDOffset = 0x0A;
        const byte TrainerBattleTransitionOffset = 0x0C;
        const byte TrainerClassModelOffset = 0x13;
        const byte TrainerPreBattleTextIDOffset = 0x24;
        const byte TrainerVictoryTextIDOffset = 0x28;
        const byte TrainerDefeatTextIDOffset = 0x2C;

        protected override uint StartOffset => (uint)(iso.CommonRel.GetPointer(Constants.Trainers) + (index * ConstSizeOfTrainerData));

        public override ushort NameID
        {
            get => pool.ExtractedFile.GetUShortAtOffset(StartOffset + TrainerNameIDOffset);
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
        public ColTrainerClasses TrainerClass
        {
            get => (ColTrainerClasses)pool.ExtractedFile.GetByteAtOffset(StartOffset + TrainerClassNameOffset);
            set => pool.ExtractedFile.WriteByteAtOffset(StartOffset + TrainerClassNameOffset, (byte)value);
        }
        public ColTrainerModels TrainerModel
        {
            get => (ColTrainerModels)pool.ExtractedFile.GetByteAtOffset(StartOffset + TrainerClassModelOffset);
            set => pool.ExtractedFile.WriteByteAtOffset(StartOffset + TrainerClassModelOffset, (byte)value);
        }

        public ushort AI
        {
            get => pool.ExtractedFile.GetUShortAtOffset(StartOffset + TrainerAIOffset);
            set => pool.ExtractedFile.WriteBytesAtOffset(StartOffset + TrainerAIOffset, value.GetBytes());
        }

        public bool IsPlayer => index == 1;
        public override bool IsSet => TrainerClass != ColTrainerClasses.None;

        public override int SizeOfTrainerData => ConstSizeOfTrainerData;

        

        public ColTrainer(int index, ColTrainerPool trainers, ISO iso) : base(index, trainers, iso)
        {
            var sizeOfPokemonData = iso.CommonRel.GetValueAtPointer(Constants.NumberOfTrainerPokemonData);
            var first = pool.ExtractedFile.GetUShortAtOffset(StartOffset + FirstTrainerPokemonOffset);

            if (first < sizeOfPokemonData)
            {
                for (int i = 0; i < Pokemon.Length; i++)
                {
                    Pokemon[i] = new ColTrainerPokemon((ushort)(first + i), iso, trainers);
                }
            }
        }
    }
}
