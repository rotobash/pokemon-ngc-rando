using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using XDCommon.PokemonDefinitions;

namespace XDCommon.Utility
{
    public class Trainer
    {
        internal const byte kSizeOfTrainerData = 0x38;
        internal const byte kSizeOfAIData = 0x20;
        internal const byte kNumberOfTrainerPokemon = 0x06;

        const byte kStringOffset = 0x00;
        const byte kShadowMaskOffset = 0x04;
        const byte kTrainerClassNameOffset = 0x05;
        const byte kTrainerNameIDOffset = 0x06;
        const byte kTrainerClassModelOffset = 0x11;
        const byte kTrainerCameraEffectOffset = 0x12;
        const byte kTrainerPreBattleTextIDOffset = 0x14;
        const byte kTrainerVictoryTextIDOffset = 0x16;
        const byte kTrainerDefeatTextIDOffset = 0x18;
        const byte kFirstTrainerPokemonOffset = 0x1C;
        const ushort kTrainerAIOffset = 0x28;

        int index = 0x0;
        TrainerPool pool;
        ISO iso;

        public TrainerPokemon[] Pokemon { get; }
        public int NameID
        {
            get => pool.ExtractedFile.GetIntAtOffset(StartOffset + kTrainerNameIDOffset);
            set => pool.ExtractedFile.WriteBytesAtOffset(StartOffset + kTrainerNameIDOffset, value.GetBytes());
        }

        public ushort TrainerStringID
        {
            get => pool.ExtractedFile.GetUShortAtOffset(StartOffset + kStringOffset);
            set => pool.ExtractedFile.WriteBytesAtOffset(StartOffset + kStringOffset, value.GetBytes());
        }
        public string TrainerString { get; }
        public ushort PreBattleTextID
        {
            get => pool.ExtractedFile.GetUShortAtOffset(StartOffset + kTrainerPreBattleTextIDOffset);
            set => pool.ExtractedFile.WriteBytesAtOffset(StartOffset + kTrainerPreBattleTextIDOffset, value.GetBytes());
        }
        public ushort VictoryTextID
        {
            get => pool.ExtractedFile.GetUShortAtOffset(StartOffset + kTrainerPreBattleTextIDOffset);
            set => pool.ExtractedFile.WriteBytesAtOffset(StartOffset + kTrainerPreBattleTextIDOffset, value.GetBytes());
        }
        public ushort DefeatTextID
        {
            get => pool.ExtractedFile.GetUShortAtOffset(StartOffset + kTrainerVictoryTextIDOffset);
            set => pool.ExtractedFile.WriteBytesAtOffset(StartOffset + kTrainerVictoryTextIDOffset, value.GetBytes());
        }
        public byte ShadowMask
        {
            get => pool.ExtractedFile.GetByteAtOffset(StartOffset + kShadowMaskOffset);
            set => pool.ExtractedFile.WriteByteAtOffset(StartOffset + kShadowMaskOffset, value);
        }
        public XDTrainerClasses TrainerClass
        {
            get => (XDTrainerClasses)pool.ExtractedFile.GetByteAtOffset(StartOffset + kTrainerClassNameOffset);
            set => pool.ExtractedFile.WriteByteAtOffset(StartOffset + kTrainerClassNameOffset, (byte)value);
        }
        public XDTrainerModels TrainerModel
        {
            get => (XDTrainerModels)pool.ExtractedFile.GetByteAtOffset(StartOffset + kTrainerClassModelOffset);
            set => pool.ExtractedFile.WriteByteAtOffset(StartOffset + kTrainerClassModelOffset, (byte)value);
        }

        public ushort AI
        {
            get => pool.ExtractedFile.GetUShortAtOffset(StartOffset + kTrainerAIOffset);
            set => pool.ExtractedFile.WriteBytesAtOffset(StartOffset + kTrainerAIOffset, value.GetBytes());
        }

        // some models have unique animations at the start of battle which require special camera movements
        public ushort CameraEffects
        {
            get => pool.ExtractedFile.GetUShortAtOffset(StartOffset + kTrainerCameraEffectOffset);
            set => pool.ExtractedFile.WriteBytesAtOffset(StartOffset + kTrainerCameraEffectOffset, value.GetBytes());
        }

        int StartOffset
        {
            get
            {
                return pool.DTNRDataOffset + (index * kSizeOfTrainerData);
            }
        }

        public string Name => iso.CommonRelStringTable.GetStringWithId(NameID).ToString();


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

            Pokemon = new TrainerPokemon[6];
            var mask = ShadowMask;
            for (int i = 0; i < Pokemon.Length; i++)
            {
                var id = pool.ExtractedFile.GetUShortAtOffset(StartOffset + kFirstTrainerPokemonOffset + (i * 2));
                var m = mask % 2;
                if (m == 1)
                {
                    Pokemon[i] = new TrainerPokemon(id, pool, PokemonFileType.DDPK);
                } 
                else
                {
                    Pokemon[i] = new TrainerPokemon(id, pool, PokemonFileType.DPKM);
                }
                mask >>= 1;
            }
        }
    }
}
