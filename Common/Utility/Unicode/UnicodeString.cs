using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XDCommon.Utility
{
    public class UnicodeString : List<IUnicodeCharacters>
    {
        public override string ToString()
        {
            return string.Join("", this);
        }
        
        public byte[] ToByteArray()
        {
            return this.SelectMany(u => u.ByteStream).ToArray();
        }

        public UnicodeString()
        {
        }

        public UnicodeString(char[] fromChars)
        {
            AddRange(fromChars.Select(c => new UnicodeCharacters((byte)c)));
        }

        public UnicodeString(IUnicodeCharacters[] fromChars)
        {
            AddRange(fromChars);
        }
    }
}
