using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XDCommon.Utility
{
    public interface IUnicodeCharacters
    {
        byte[] ByteStream { get; }
        byte Unicode { get; }
        int ExtraBytes { get; }
        bool IsFormattingChar { get; }
        string Name { get; }
    }

    public abstract class BaseUnicodeCharacters : IUnicodeCharacters
    {
        public abstract byte[] ByteStream { get; }
        public abstract byte Unicode { get; }
        public abstract int ExtraBytes { get; }
        public abstract bool IsFormattingChar { get; }
        public abstract string Name { get; }

        public static int HexStringToInt(string hex)
        {
            return Convert.ToInt32(hex);
        }
    }

    public class UnicodeCharacters : BaseUnicodeCharacters
    {
        public override byte[] ByteStream { get; }

        public override byte Unicode { get; }

        public override int ExtraBytes { get; }

        public override bool IsFormattingChar { get; }
        public override string Name => ToString();

        private readonly UnicodeEncoding encoding;
        public UnicodeCharacters(int unicodeByte)
        {
            ByteStream = new[]
            {
                (byte)(unicodeByte / 0x100),
                (byte)(unicodeByte % 0x100)
            };

            Unicode = (byte)(unicodeByte % 0x100);
            ExtraBytes = 0;
            IsFormattingChar = false;
            encoding = new UnicodeEncoding();
        }

        public override string ToString()
        {
            return encoding.GetString(ByteStream) ?? string.Empty;
        }
    }

    public class SpecialUnicodeCharacters : BaseUnicodeCharacters
    {
        public override byte[] ByteStream { get; }

        public override byte Unicode { get; }

        public override int ExtraBytes { get; }

        public override bool IsFormattingChar { get; }
        public override string Name => ToString();

        private readonly XGSpecialCharacters chars;
        public SpecialUnicodeCharacters(XGSpecialCharacters specialChars, IEnumerable<int> unicodeBytes)
        {
            var bs = new List<byte>();
            foreach(var b in specialChars.ByteStream)
            {
                bs.Add((byte)b);
            }

            ByteStream = bs.ToArray();

            Unicode = 0xFF;
            ExtraBytes = unicodeBytes.Count();
            IsFormattingChar = !chars.IsNewLine;

            chars = specialChars;
        }

        public override string ToString()
        {
            var str = chars.ToString();
            if (ByteStream.Length > 0)
            {
                str += "{";
                foreach (var hex in ByteStream)
                {
                    str += string.Format("%02x", hex);
                }
                str += "}";
            }
            return str;
        }
    }

    public class PredefinedUnicodeCharacters : BaseUnicodeCharacters
    {
        public override byte[] ByteStream { get; }

        public override byte Unicode { get; }

        public override int ExtraBytes { get; }

        public override bool IsFormattingChar { get; }
        public override string Name => colour.Name;

        private readonly XGFontColous colour;
        public PredefinedUnicodeCharacters(XGFontColours colour)
        {
            ByteStream = colour.SpecialUnicode.ByteStream;
            Unicode = 0xFF;
            ExtraBytes = 1;
            IsFormattingChar = true;
            this.colour = colour;
        }

        public override string ToString()
        {
            return colour.specialUnicode.ToString();
        }
    }
    
    public class RGBUnicodeCharacters : BaseUnicodeCharacters
    {
        public override byte[] ByteStream { get; }

        public override byte Unicode { get; }

        public override int ExtraBytes { get; }

        public override bool IsFormattingChar { get; }
        public override string Name => colour.Name;

        private readonly XGRGBAFontColours colour;
        public RGBUnicodeCharacters(XGRGBAFontColours colour)
        {
            ByteStream = colour.SpecialUnicode.ByteStream;
            Unicode = 0xFF;
            ExtraBytes = 4;
            IsFormattingChar = true;
            this.colour = colour;
        }

        public override string ToString()
        {
            return colour.specialUnicode.ToString();
        }
    }
}
