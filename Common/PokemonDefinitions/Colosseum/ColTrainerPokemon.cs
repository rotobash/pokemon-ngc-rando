using XDCommon.Utility;

namespace XDCommon.PokemonDefinitions
{
    internal class ColTrainerPokemon : TrainerPokemon
    {
        const byte SizeOfPokemonData = 0x50;
        const byte SizeOfMoveData = 0x8;

        const byte ConstPokemonAbilityOffset = 0x00;
        const byte ConstPokemonGenderOffset = 0x01;
        const byte ConstPokemonNatureOffset = 0x02;
        const byte ConstPokemonShadowIDOffset = 0x03;
        const byte ConstPokemonLevelOffset = 0x04;
        const byte ConstPokemonPriorityOffset = 0x05;

        const byte ConstPokemonHappinessOffset = 0x08;
        const byte ConstPokemonIndexOffset = 0x0A;
        const byte ConstPokemonPokeballOffset = 0x0D;
        const byte ConstPokemonItemOffset = 0x12;
        const byte ConstPokemonNameIDOffset = 0x14;
        const byte ConstFirstPokemonIVOffset = 0x1C;
        const byte ConstFirstPokemonEVOffset = 0x23;
        const byte ConstFirstPokemonMoveOffset = 0x36;

        static readonly byte[] ConstFFOffsets = new byte[] { 0, 1, 2, 8, 9, 0x10, 0x11, 0x12, 0x13, 0x1C, 0x1D, 0x1E, 0x1F, 0x20, 0x21, 0x22, 0x23, 0x24, 0x25, 0x26, 0x27, 0x28, 0x29, 0x2A, 0x2B, 0x2C, 0x2D };

        const byte ConstSizeOfShadowData = 0x38;

        const byte ConstShadowCatchRateOffset = 0x00; // overrides species' catch rate
        const byte ConstShadowSpeciesOffset = 0x02;
        const byte ConstShadowTIDOffset = 0x04; // trainer index of the first time the shadow pokemon is encountered
        const byte ConstShadowTID2Offset = 0x06; // trainer index of an alternate possibility for first encounter
        const byte ConstShadowCounterOffset = 0x09;
        const byte ConstShadowIDOffset = 0x0B;
        const byte ConstShadowJapaneseMSGOffset = 0x22;

        private byte ShadowId => pool.ExtractedFile.GetByteAtOffset(StartOffset + ConstPokemonShadowIDOffset);

        protected override uint StartOffset => (uint)(iso.CommonRel.GetPointer(Constants.TrainerPokemonData) + (Index * SizeOfPokemonData));
        protected override uint ShadowStartOffset => (uint)(iso.CommonRel.GetPointer(Constants.ShadowData) + (ShadowId * ConstSizeOfShadowData));

        protected override byte PokemonIndexOffset => ConstPokemonIndexOffset;
        protected override byte PokemonItemOffset => ConstPokemonItemOffset;
        protected override byte PokemonHappinessOffset => ConstPokemonHappinessOffset;
        protected override byte FirstPokemonEVOffset => ConstFirstPokemonEVOffset;
        protected override byte FirstPokemonMoveOffset => ConstFirstPokemonMoveOffset;
        protected override byte PokemonMoveDataSize => SizeOfMoveData;
        protected override byte ShadowCatchRateOffset => ConstShadowCatchRateOffset;
        protected override byte ShadowCounterOffset => ConstShadowCounterOffset;

        public override byte Level
        {
            get => pool.ExtractedFile.GetByteAtOffset(StartOffset + ConstPokemonLevelOffset);
            set => pool.ExtractedFile.WriteByteAtOffset(StartOffset + ConstPokemonLevelOffset, value);
        }

        public override byte ShadowLevel { get => Level; set => Level = value; }

        public override ushort Pokemon 
        { 
            get => base.Pokemon;
            set
            { 
                var poke = pokemonList[value];
                pool.ExtractedFile.WriteBytesAtOffset(StartOffset + ConstPokemonNameIDOffset, poke.NameID.GetBytes());
                base.Pokemon = value; 
            }
        }


        public byte Ability
        {
            get => pool.ExtractedFile.GetByteAtOffset(StartOffset + ConstPokemonAbilityOffset);
            set => pool.ExtractedFile.WriteByteAtOffset(StartOffset + ConstPokemonAbilityOffset, value);
        }
        public Genders Gender
        {
            get => (Genders)pool.ExtractedFile.GetByteAtOffset(StartOffset + ConstPokemonGenderOffset);
            set => pool.ExtractedFile.WriteByteAtOffset(StartOffset + ConstPokemonGenderOffset, (byte)value);
        }
        public Natures Nature
        {
            get => (Natures)pool.ExtractedFile.GetByteAtOffset(StartOffset + ConstPokemonNatureOffset);
            set => pool.ExtractedFile.WriteByteAtOffset(StartOffset + ConstPokemonNatureOffset, (byte)value);
        }

        public override bool IsShadow => ShadowId > 0;

        public override bool IsSet => Pokemon > 0;

        Pokemon[] pokemonList;

        public ColTrainerPokemon(ushort id, ISO iso, ColTrainerPool pool) : base(id, pool)
        {
            this.iso = iso;
            pokemonList = pool.PokemonList;

            for (int i = 0; i < Moves.Length; i++)
            {
                Moves[i] = pool.ExtractedFile.GetUShortAtOffset(StartOffset + FirstPokemonMoveOffset + i * PokemonMoveDataSize);
            }
        }

        public ushort ShadowFirstTID
        {
            get => pool.ExtractedFile.GetByteAtOffset(ShadowStartOffset + ConstShadowTIDOffset);
            set => pool.ExtractedFile.WriteBytesAtOffset(ShadowStartOffset + ConstShadowTIDOffset, value.GetBytes());
        }

        // do nothing, there's only one shadow move in colo
        public override void SetShadowMove(int index, ushort moveNum)
        {
        }
    }
}