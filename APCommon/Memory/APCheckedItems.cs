using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XDCommon.PokemonDefinitions;

namespace APCommon.Memory
{
    public class APCheckedItems : APMemoryObject
    {
        public const int SizeOfAPCheckedItems = 8 + (APItemCheck.SizeOfAPItemCheck * NUMBER_OF_ITEMS) + (APPokemonItem.SizeOfGetPokemon * NUMBER_OF_POKEMON) + (4 * NUMBER_OF_POKEMON) + (2 * NUMBER_OF_TRAINERS);

        const int NUMBER_OF_ITEMS = 300;
        const int NUMBER_OF_POKEMON = 100;
        const int NUMBER_OF_TRAINERS = 450;

        ushort oldCheckedItemCounter = 0;
        ushort oldPokemonCounter = 0;
        ushort oldPurifyCounter = 0;
        ushort oldBeatTrainerCounter = 0;

        ushort checkedItemCounter;
        public APItemCheck[] CheckedItems { get; } = new APItemCheck[NUMBER_OF_ITEMS];

        ushort pokemonCounter;
        public APPokemonItem[] PokemonChecks { get; } = new APPokemonItem[NUMBER_OF_POKEMON];

        ushort purifyCounter;
        public int[] PurifiedShadowChecks { get; } = new int[NUMBER_OF_POKEMON];

        ushort beatTrainerCounter;
        public ushort[] BeatTrainerBattleChecks { get; } = new ushort[NUMBER_OF_TRAINERS];

        public bool CheckedNewItems => oldCheckedItemCounter != checkedItemCounter;
        public bool CheckedNewPokemon => oldPokemonCounter != pokemonCounter;
        public bool CheckedNewPurifications => oldPurifyCounter != purifyCounter;
        public bool CheckedNewBattles => oldBeatTrainerCounter != beatTrainerCounter;

        public APCheckedItems()
        {
            for (int i = 0; i < NUMBER_OF_ITEMS; i++)
            {
                CheckedItems[i] = new APItemCheck();
            }
            
            for (int i = 0; i < NUMBER_OF_POKEMON; i++)
            {
                PokemonChecks[i] = new APPokemonItem();
            }
        }

        public override byte[] GetBytes()
        {
            var intBuffer = new byte[4];
            var ushortBuffer = new byte[2];
            var bytes = new List<byte>(SizeOfAPCheckedItems);

            BinaryPrimitives.WriteUInt16BigEndian(ushortBuffer, checkedItemCounter);
            bytes.AddRange(ushortBuffer);

            for (int i = 0; i < NUMBER_OF_ITEMS; i++)
            {
                bytes.AddRange(CheckedItems[i].GetBytes());
            }

            BinaryPrimitives.WriteUInt16BigEndian(ushortBuffer, pokemonCounter);
            bytes.AddRange(ushortBuffer);

            for (int i = 0; i < NUMBER_OF_POKEMON; i++)
            {
                bytes.AddRange(PokemonChecks[i].GetBytes());
            }

            BinaryPrimitives.WriteUInt16BigEndian(ushortBuffer, purifyCounter);
            bytes.AddRange(ushortBuffer);

            for (int i = 0; i < NUMBER_OF_POKEMON; i++)
            {
                BinaryPrimitives.WriteInt32BigEndian(intBuffer, PurifiedShadowChecks[i]);
                bytes.AddRange(intBuffer);
            }

            BinaryPrimitives.WriteUInt16BigEndian(ushortBuffer, beatTrainerCounter);
            bytes.AddRange(ushortBuffer);

            for (int i = 0; i < NUMBER_OF_TRAINERS; i++)
            {
                BinaryPrimitives.WriteUInt16BigEndian(intBuffer, BeatTrainerBattleChecks[i]);
                bytes.AddRange(intBuffer);
            }

            return bytes.ToArray();
        }

        public override void ReadFromBytes(Span<byte> bytes)
        {
            var offset = 0;
            checkedItemCounter = BinaryPrimitives.ReadUInt16BigEndian(bytes.Slice(offset, 2));
            offset += 2;

            for (int i = 0; i < NUMBER_OF_ITEMS; i++)
            {
                var itemBytes = bytes.Slice(offset, APItemCheck.SizeOfAPItemCheck);
                CheckedItems[i].ReadFromBytes(itemBytes);
                offset += APItemCheck.SizeOfAPItemCheck;
            }

            beatTrainerCounter = BinaryPrimitives.ReadUInt16BigEndian(bytes.Slice(offset, 2));
            offset += 2;

            for (int i = 0; i < NUMBER_OF_TRAINERS; i++)
            {
                var beatTrainerBytes = bytes.Slice(offset, 2);
                BeatTrainerBattleChecks[i] = BinaryPrimitives.ReadUInt16BigEndian(beatTrainerBytes);
                offset += 2;
            }

            purifyCounter = BinaryPrimitives.ReadUInt16BigEndian(bytes.Slice(offset, 2));
            offset += 2;

            for (int i = 0; i < NUMBER_OF_POKEMON; i++)
            {
                var purifyBytes = bytes.Slice(offset, 4);
                PurifiedShadowChecks[i] = BinaryPrimitives.ReadInt32BigEndian(purifyBytes);
                offset += 4;
            }

            pokemonCounter = BinaryPrimitives.ReadUInt16BigEndian(bytes.Slice(offset, 2));
            offset += 2;

            for (int i = 0; i < NUMBER_OF_POKEMON; i++)
            {
                var pokemonBytes = bytes.Slice(offset, APPokemonItem.SizeOfGetPokemon);
                PokemonChecks[i].ReadFromBytes(pokemonBytes);
                offset += APPokemonItem.SizeOfGetPokemon;
            }
        }

        public APItemCheck[] GetNewItemChecks()
        {
            if (CheckedNewItems)
            {
                var items = CheckedItems.AsSpan(oldCheckedItemCounter, checkedItemCounter);
                oldCheckedItemCounter = checkedItemCounter;
                return items.ToArray();
            }
            else
            {
                return Array.Empty<APItemCheck>();
            }
        }

        public APPokemonItem[] GetNewPokemonChecks()
        {
            if (CheckedNewPokemon)
            {
                var pokemonChecks = PokemonChecks.AsSpan(oldPokemonCounter, pokemonCounter);
                oldPokemonCounter = pokemonCounter;
                return pokemonChecks.ToArray();
            }
            else
            {
                return Array.Empty<APPokemonItem>();
            }
        }

        public int[] GetNewShadowPurifyChecks()
        {
            if (CheckedNewPurifications)
            {
                var purifyChecks = PurifiedShadowChecks.AsSpan(oldPurifyCounter, purifyCounter);
                oldPurifyCounter = purifyCounter;
                return purifyChecks.ToArray();
            }
            else
            {
                return Array.Empty<int>();
            }
        }

        public ushort[] GetNewBattleChecks()
        {
            if (CheckedNewBattles)
            {
                var trainerChecks = BeatTrainerBattleChecks.AsSpan(oldBeatTrainerCounter, beatTrainerCounter);
                oldBeatTrainerCounter = beatTrainerCounter;
                return trainerChecks.ToArray();
            }
            else
            {
                return Array.Empty<ushort>();
            }
        }
    }
}
