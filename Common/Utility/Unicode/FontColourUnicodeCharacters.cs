using System;
using System.Collections.Generic;
using System.Text;

namespace XDCommon.Utility
{
    public class FontColourUnicodeCharacters : BaseUnicodeCharacters
    {
        public override byte[] ByteStream { get; }

        public override bool Unicode { get; }

        public override int ExtraBytes { get; }

        public override bool IsFormattingChar { get; }

        private readonly SpecialUnicodeCharacters specialUnicode;
        private readonly FontColours colour;
        public override string Name => ToString();
        public FontColourUnicodeCharacters(FontColours colour)
        {
            specialUnicode = new SpecialUnicodeCharacters(SpecialCharacters.ChangeColour, (byte)colour);
            ByteStream = specialUnicode.ByteStream;
            Unicode = true;
            ExtraBytes = 1; // might need to use specialUnicode.ExtraBytes unsure
            IsFormattingChar = true;
            this.colour = colour;
        }

        public override string ToString()
        {
            return $"Predefined {colour}";
        }
    }
}
