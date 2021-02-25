using System;
using System.Collections.Generic;
using System.Text;
using XDCommon.Utility;

namespace XDCommon.PokemonDefinitions
{
    public class ColStarterPokemon : IGiftPokemon
    {
        private byte index;
        private ISO iso;

        public ColStarterPokemon(byte index, ISO iso)
        {
            this.index = index;
            this.iso = iso;
        }

        public byte Index => throw new NotImplementedException();

        public ushort Exp => throw new NotImplementedException();

        public ushort[] Moves => throw new NotImplementedException();

        public string GiftType => throw new NotImplementedException();

        public byte Level { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public ushort Pokemon { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public void SetMove(int i, ushort move)
        {
            throw new NotImplementedException();
        }
    }
}
