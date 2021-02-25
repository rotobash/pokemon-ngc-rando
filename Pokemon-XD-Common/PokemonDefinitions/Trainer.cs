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
        protected Stream ExtractedFile;
        protected int index;

        protected byte StringOffset => ConstStringOffset;
        protected byte ShadowMaskOffset => ConstShadowMaskOffset;

        public Trainer(int index, TrainerPool trainers, ISO iso)
        {
            this.index = index;
            pool = trainers;
            this.iso = iso;
            var name = "";
            var currentChar = 0x1;

            var offset = trainers.DTNRDataOffset + TrainerStringID;
            while (currentChar != 0)
            {
                currentChar = trainers.ExtractedFile.GetByteAtOffset(offset);
                if (currentChar != 0)
                {
                    name += new UnicodeCharacters(currentChar);
                }
                offset++;
            }
            TrainerString = name;
            Pokemon = new ITrainerPokemon[6];
        }

        public string Name => iso.CommonRelStringTable.GetStringWithId(NameID).ToString();

        public ushort TrainerStringID
        {
            get => pool.ExtractedFile.GetUShortAtOffset(StartOffset + StringOffset);
            set => pool.ExtractedFile.WriteBytesAtOffset(StartOffset + StringOffset, value.GetBytes());
        }
        public string TrainerString { get; }
        public byte ShadowMask
        {
            get => pool.ExtractedFile.GetByteAtOffset(StartOffset + ShadowMaskOffset);
            set => pool.ExtractedFile.WriteByteAtOffset(StartOffset + ShadowMaskOffset, value);
        }

        // some models have unique animations at the start of battle which require special camera movements
        public ushort CameraEffects
        {
            get => pool.ExtractedFile.GetUShortAtOffset(StartOffset + TrainerCameraEffectOffset);
            set => pool.ExtractedFile.WriteBytesAtOffset(StartOffset + TrainerCameraEffectOffset, value.GetBytes());
        }
    }
}
