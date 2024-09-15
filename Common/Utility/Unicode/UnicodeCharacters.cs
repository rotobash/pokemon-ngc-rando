using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XDCommon.Utility
{
    public interface IUnicodeCharacters
    {
        byte[] ByteStream { get; }
        bool Unicode { get; }
        int ExtraBytes { get; }
        bool IsFormattingChar { get; }
        string Name { get; }
    }

    public abstract class BaseUnicodeCharacters : IUnicodeCharacters
    {
        public abstract byte[] ByteStream { get; }
        public abstract bool Unicode { get; }
        public abstract int ExtraBytes { get; }
        public abstract bool IsFormattingChar { get; }
        public abstract string Name { get; }

        public override string ToString()
        {
            return base.ToString();
        }

        public override bool Equals(object obj)
        {
            if (obj is BaseUnicodeCharacters other)
            {
                var matches = ByteStream.Zip(other.ByteStream, (f, s) => f == s);
                return !matches.Any(m => m == false);
            }

            return false;
        }
    }

    public class UnicodeCharacters : BaseUnicodeCharacters
    {
        public override byte[] ByteStream { get; }

        public override bool Unicode { get; }

        public override int ExtraBytes { get; }

        public override bool IsFormattingChar { get; }
        public override string Name => ToString();

        private readonly UnicodeEncoding encoding;
        public UnicodeCharacters(int unicodeByte)
        {
            ByteStream = new byte[]
            {
                0,
                (byte)(unicodeByte % 0x100)
            };

            Unicode = false;
            ExtraBytes = 0;
            IsFormattingChar = false;
            encoding = new UnicodeEncoding();
        }

        public override string ToString()
        {
            return Encoding.UTF8.GetString(ByteStream.Where(b => b != 0).ToArray()) ?? string.Empty;
        }
    }
}
