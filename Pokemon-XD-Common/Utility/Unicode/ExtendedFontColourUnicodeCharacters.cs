using System;
using System.Collections.Generic;
using System.Text;

namespace XDCommon.Utility
{
    public class ExtendedFontColourUnicodeCharacters : BaseUnicodeCharacters
    {
        static readonly byte[] RedBytes = new byte[] { 0xFF,0x00,0x00,0xFF };
        static readonly byte[] GreenBytes = new byte[] { 0x00,0xFF,0x00,0xFF };
        static readonly byte[] BlueBytes = new byte[] { 0x00,0x00,0xFF,0xFF};
        static readonly byte[] YellowBytes = new byte[] {0xFF,0xFF,0x00,0xFF };
        static readonly byte[] CyanBytes = new byte[] {0x00,0xFF,0xFF,0xFF };
        static readonly byte[] MagentaBytes = new byte[] {0xFF,0x00,0xFF,0xFF };
        static readonly byte[] LightGreenBytes = new byte[] {0xC8,0xFF,0x00,0xFF };
        static readonly byte[] OrangeBytes = new byte[] {0xC8,0x00,0xC8,0xFF };
        static readonly byte[] PurpleBytes = new byte[] {0xFF,0xB0,0x00,0xFF };
        static readonly byte[] GreyBytes = new byte[] { 0xC8,0xC8,0xC8,0xFF };
        static readonly byte[] WhiteBytes = new byte[] {0xFF,0xFF,0xFF,0xFF };
        static readonly byte[] BlackBytes = new byte[] { 0x00,0x00,0x00,0xFF };

        public override byte[] ByteStream { get; }

        public override byte Unicode { get; }

        public override int ExtraBytes { get; }

        public override bool IsFormattingChar { get; }
        public override string Name => ToString();

        private readonly SpecialUnicodeCharacters specialUnicode;
        private readonly ExtendedFontColours colour;
        public ExtendedFontColourUnicodeCharacters(ExtendedFontColours colour)
        {
            specialUnicode = new SpecialUnicodeCharacters(SpecialCharacters.ChangeColourExtended, GetBytes());
            ByteStream = specialUnicode.ByteStream;
            Unicode = 0xFF;
            ExtraBytes = 4;
            IsFormattingChar = true;
            this.colour = colour;
        }
        
        public ExtendedFontColourUnicodeCharacters(byte[] colour)
        {
            specialUnicode = new SpecialUnicodeCharacters(SpecialCharacters.ChangeColourExtended, colour);
            ByteStream = specialUnicode.ByteStream;
            Unicode = 0xFF;
            ExtraBytes = 4;
            IsFormattingChar = true;
            this.colour = ExtendedFontColours.Custom;
        }

        public override string ToString()
        {
            return $"Specified {colour}";
        }

        private byte[] GetBytes()
        {
            return colour switch
            {
                ExtendedFontColours.Red => RedBytes,
                ExtendedFontColours.Green => GreenBytes,
                ExtendedFontColours.Blue => BlueBytes,
                ExtendedFontColours.Yellow => YellowBytes,
                ExtendedFontColours.Cyan => CyanBytes,
                ExtendedFontColours.Magenta => MagentaBytes,
                ExtendedFontColours.LightGreen => LightGreenBytes,
                ExtendedFontColours.Orange => OrangeBytes,
                ExtendedFontColours.Purple => PurpleBytes,
                ExtendedFontColours.Grey => GreyBytes,
                ExtendedFontColours.White => WhiteBytes,
                ExtendedFontColours.Black => BlackBytes,
                _ => Array.Empty<byte>()
            };
        }
    }
}
