using System;
using System.Collections.Generic;
using System.IO;
using XDCommon.Utility;

namespace XDCommon.PokemonDefinitions
{
    public interface ITrainer
    {
        string Name { get; }
        ITrainerPokemon[] Pokemon { get; }
        bool IsSet { get; }
    }

    public abstract class Trainer : ITrainer
    {
        const byte ConstStringOffset = 0x00;
        const byte ConstShadowMaskOffset = 0x04;
        const byte TrainerCameraEffectOffset = 0x12;
        public ITrainerPokemon[] Pokemon { get; }

        public abstract bool IsSet { get; }
        public abstract int SizeOfTrainerData { get; }
        public abstract int NameID { get; set; }
        protected abstract uint StartOffset { get; }

        protected TrainerPool pool;
        protected ISO iso;
        protected int index;

        protected byte StringOffset => ConstStringOffset;
        protected byte ShadowMaskOffset => ConstShadowMaskOffset;

        public Trainer(int index, TrainerPool trainers, ISO iso)
        {
            this.index = index;
            pool = trainers;
            this.iso = iso;
            Pokemon = new ITrainerPokemon[6];
        }

        public string Name => iso.CommonRelStringTable.GetStringWithId(NameID).ToString();

        // some models have unique animations at the start of battle which require special camera movements
        public ushort CameraEffects
        {
            get => pool.ExtractedFile.GetUShortAtOffset(StartOffset + TrainerCameraEffectOffset);
            set => pool.ExtractedFile.WriteBytesAtOffset(StartOffset + TrainerCameraEffectOffset, value.GetBytes());
        }
    }
}
