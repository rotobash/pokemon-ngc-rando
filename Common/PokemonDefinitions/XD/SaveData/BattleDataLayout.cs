using DolphinMemoryAccess;
using System;
using System.Collections.Generic;
using System.Text;

namespace XDCommon.PokemonDefinitions.XD.SaveData
{
    public class BattleDataLayout : SaveDataLayout
    {
        public BattlePokemon[] BattleData
        {
            get;
        } = new BattlePokemon[6];


        public override void LoadFromMemory(Dolphin dolphin)
        {
            base.LoadFromMemory(dolphin);
            if (NextSectionOffset == 0)
            {
                return;
            }

            for (int i = 0; i < BattleData.Length; i++)
            {
                var pokemonData = dolphin.ReadData(NextSectionOffset + (PokemonInstance.SizeOfData * i), PokemonInstance.SizeOfData);
                BattleData[i] = new BattlePokemon(pokemonData);
            }
        }
    }
}
