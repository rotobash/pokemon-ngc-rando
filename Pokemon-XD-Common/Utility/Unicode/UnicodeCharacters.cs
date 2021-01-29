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
                (byte)(unicodeByte % 0x100)
            };

            Unicode = (byte)(unicodeByte % 0x100);
            ExtraBytes = 0;
            IsFormattingChar = false;
            encoding = new UnicodeEncoding();
        }

        public override string ToString()
        {
            return Encoding.UTF8.GetString(ByteStream) ?? string.Empty;
        }
    }
}
