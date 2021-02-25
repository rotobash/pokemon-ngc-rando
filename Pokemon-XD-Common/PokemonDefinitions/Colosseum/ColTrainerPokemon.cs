namespace XDCommon.PokemonDefinitions
{
    internal class ColTrainerPokemon : ITrainerPokemon
    {
        private ushort id;
        private ColTrainerPool pool;
        private PokemonFileType pokemonType;

        public ColTrainerPokemon(ushort id, ColTrainerPool pool, PokemonFileType pokemonType)
        {
            this.id = id;
            this.pool = pool;
            this.pokemonType = pokemonType;
        }

        public ushort Pokemon { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }

        public bool IsShadow => throw new System.NotImplementedException();

        public byte ShadowCatchRate { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
        public byte Level { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
        public ushort Item { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }

        public ushort[] Moves => throw new System.NotImplementedException();

        public void SetMove(int index, ushort moveNum)
        {
            throw new System.NotImplementedException();
        }

        public void SetShadowMove(int index, ushort moveNum)
        {
            throw new System.NotImplementedException();
        }
    }
}