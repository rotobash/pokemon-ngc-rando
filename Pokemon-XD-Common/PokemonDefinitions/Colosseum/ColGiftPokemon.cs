using XDCommon.Contracts;
using XDCommon.PokemonDefinitions;
using XDCommon.Utility;

namespace XDCommon.PokemonDefinitions
{
    public class ColGiftPokemon : IGiftPokemon
	{
		const byte PokemonSpeciesOffset = 0x02;
		const byte PokemonLevelOffset = 0x07;

		private ISO iso;
        uint PlusleOffset => iso.Region switch 
		{
			Region.US => 0x12D9C8,
			 Region.Japan => 0x12B098,
			 _ => 0x131BF4
		};
		
		uint HoohOffsetswitch => iso.Region switch 
		{
			Region.US => 0x12D8E4,
			Region.Japan => 0x12AFB8,
			_ => 0x131B10
		};
	
		uint CelebiOffset => iso.Region switch 
		{
			Region.US => 0x12D6B4,
			Region.Japan => 0x12ADD0,
			_ => 0x1318E0
		};

		uint PikachuOffset => iso.Region switch 
		{
			Region.US => 0x12D7C4,
			Region.Japan => 0x12AEBC,
			_ => 0x1319F0
		};

        public ColGiftPokemon(int index, ISO iso)
        {
            this.iso = iso;
			Index = (byte)index;
        }

		uint StartOffset => Index switch
		{
			0 => PlusleOffset,
			1 => HoohOffsetswitch,
			2 => CelebiOffset,
			_ => PikachuOffset
		};

        public byte Index { get; }
        public ushort Exp => 0;

        public ushort[] Moves => new ushort[Constants.NumberOfPokemonMoves];
        public void SetMove(int i, ushort move) { }

        public string GiftType => Index switch
		{
			0 => "Duking",
			1 => "Mt Battle",
			2 => "Agate Celebi",
			_ => "Agate Pikachu"
		};

		public byte Level 
		{ 
			get => iso.DOL.ExtractedFile.GetByteAtOffset(StartOffset + PokemonLevelOffset); 
			set => iso.DOL.ExtractedFile.WriteByteAtOffset(StartOffset + PokemonLevelOffset, value); 
		}

        public ushort Pokemon
		{
			get => iso.DOL.ExtractedFile.GetByteAtOffset(StartOffset + PokemonSpeciesOffset);
			set => iso.DOL.ExtractedFile.WriteBytesAtOffset(StartOffset + PokemonSpeciesOffset, value.GetBytes());
		}

	}
}