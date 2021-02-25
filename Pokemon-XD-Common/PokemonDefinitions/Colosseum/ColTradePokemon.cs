using XDCommon.PokemonDefinitions;
using XDCommon.Utility;

namespace XDCommon.PokemonDefinitions
{
    public class ColTradePokemon : IGiftPokemon
    {
        private byte i;
        private ISO iso;

        public ColTradePokemon(byte i, ISO iso)
        {
            this.i = i;
            this.iso = iso;
        }

        public byte Index => throw new System.NotImplementedException();

        public ushort Exp => throw new System.NotImplementedException();

        public ushort[] Moves => throw new System.NotImplementedException();

        public string GiftType => throw new System.NotImplementedException();

        public byte Level { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
        public ushort Pokemon { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }

        public void SetMove(int i, ushort move)
        {
            throw new System.NotImplementedException();
        }
    }
}