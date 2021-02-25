using XDCommon.PokemonDefinitions;
using XDCommon.Utility;

namespace XDCommon.PokemonDefinitions
{
    public class ColMtBattlePokemon : IGiftPokemon
    {
        private byte index;
        private ISO iso;

        public ColMtBattlePokemon(byte index, ISO iso)
        {
            this.index = index;
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