using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XDCommon.Utility
{
    public class SpecialUnicodeCharacters : BaseUnicodeCharacters
    {
        public override byte[] ByteStream { get; }

        public override byte Unicode { get; }

        public override int ExtraBytes { get; }

        public override bool IsFormattingChar => !IsNewLine;
        public override string Name => ToString();
        public bool IsNewLine => chars == SpecialCharacters.NewLine;

        private readonly SpecialCharacters chars;
        public SpecialUnicodeCharacters(SpecialCharacters specialChars, params byte[] unicodeBytes)
        {
            var bs = new List<byte> 
			{
				0xFF,
				0xFF,
				(byte)specialChars
			};

            ExtraBytes = unicodeBytes.Length;

            foreach (var uByte in unicodeBytes)
            {
                bs.Add(uByte);
            }

            ByteStream = bs.ToArray();
            Unicode = 0xFF;
            chars = specialChars;
        }

		private string CharToString()
        {
			if (IsNewLine)
            {
				return "\n";
            }

            return $"[{chars.AsString()}]";
        }

        public override string ToString()
        {
            var str = CharToString();
            if (ByteStream.Length > 0)
            {
                str += "{";
                foreach (var hex in ByteStream)
                {
                    str += string.Format("{0:X}", hex);
                }
                str += "}";
            }
            return str;
        }
    }
}
