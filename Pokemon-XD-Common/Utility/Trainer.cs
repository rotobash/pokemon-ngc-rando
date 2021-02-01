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
        const byte kTrainerAIOffset = 0x28;

        int index = 0x0;
        TrainerPool pool;

        ushort nameID = 0x0;
        ushort trainerStringID = 0x0;
        string trainerString = "";
        ushort preBattleTextID = 0x0;
        ushort victoryTextID = 0x0;
        ushort defeatTextID = 0x0;
        byte shadowMask = 0x0;

        TrainerPokemonPool[] pokemon = new TrainerPokemonPool[6];
        Pokemon.TrainerClasses trainerClass = Pokemon.TrainerClasses.Michael1;
        Pokemon.TrainerModels trainerModel = Pokemon.TrainerModels.Michael1WithoutSnagMachine;
        
        ushort AI = 0;
        ushort cameraEffects = 0; // some models have unique animations at the start of battle which require special camera movements

        int StartOffset
        {
            get
            {
                return TrainerPool.DTNRDataOffset + (index * kSizeOfTrainerData);
            }
        }


        public Trainer(int index, TrainerPool trainers)
        {
            this.index = index;
            pool = trainers;

            var start = StartOffset;
            trainerStringID = trainers.ExtractedFile.GetUShortAtOffset(start + kStringOffset);
            var name = "";
            var currentChar = 0x1;

            var offset = TrainerPool.DSTRDataOffset + trainerStringID;
            while (currentChar != 0)
            {
                currentChar = trainers.ExtractedFile.GetByteAtOffset(offset);
                if (currentChar != 0)
                {
                    name += new UnicodeCharacters(currentChar);
                }
                offset++;
            }

            trainerString = name;
            nameID = pool.ExtractedFile.GetUShortAtOffset(start + kTrainerNameIDOffset);
            preBattleTextID = pool.ExtractedFile.GetUShortAtOffset(start + kTrainerNameIDOffset);
            victoryTextID = pool.ExtractedFile.GetUShortAtOffset(start + kTrainerVictoryTextIDOffset);
            defeatTextID = pool.ExtractedFile.GetUShortAtOffset(start + kTrainerDefeatTextIDOffset);
            shadowMask = pool.ExtractedFile.GetByteAtOffset(start + kShadowMaskOffset);

            var mask = shadowMask;
            for (int i = 0; i < 6; i++)
            {
                var id = pool.ExtractedFile.GetByteAtOffset(start + kFirstTrainerPokemonOffset + (i * 2));
                var m = mask % 2;
                if (m == 1)
                {
                    pokemon[i] = new TrainerPokemonPool(id);
                } 
                else
                {
                    pokemon[i] = new TrainerPokemonPool(id, pool);
                }
                mask >>= 1;
            }

            var tClass = pool.ExtractedFile.GetByteAtOffset(start + kTrainerClassNameOffset);
            var tModel = pool.ExtractedFile.GetByteAtOffset(start + kTrainerClassModelOffset);

            trainerClass = (Pokemon.TrainerClasses)tClass;
            trainerModel = (Pokemon.TrainerModels)tModel;
            AI = pool.ExtractedFile.GetUShortAtOffset(start + kTrainerAIOffset);
            cameraEffects = pool.ExtractedFile.GetUShortAtOffset(start + kTrainerCameraEffectOffset);
        }
    }
}
