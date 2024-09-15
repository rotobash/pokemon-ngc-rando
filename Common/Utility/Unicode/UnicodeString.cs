using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using XDCommon.Contracts;

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

        public UnicodeString(IEnumerable<byte> fromBytes)
        {
            int currentOffset = 0;
            ushort nextChar = 1;
            MemoryStream memoryStream = new MemoryStream(fromBytes.ToArray());

            while (nextChar != 0) 
            {
                nextChar = memoryStream.GetUShortAtOffset(currentOffset);
                currentOffset += 2;

                if (nextChar == 0xFFFF)
                {
                    var specChar = (SpecialCharacters)memoryStream.GetByteAtOffset(currentOffset);
                    currentOffset += 1;

                    var extra = specChar.ExtraBytes();

                    var bytes = new byte[extra];
                    for (int i = 0; i < bytes.Length; i++)
                    {
                        bytes[i] = memoryStream.GetByteAtOffset(currentOffset + i);
                    }
                    currentOffset += extra;

                    if (specChar == SpecialCharacters.ChangeColour)
                    {
                        Add(new FontColourUnicodeCharacters((FontColours)bytes[0]));
                    }
                    else
                    {
                        Add(new SpecialUnicodeCharacters(specChar, bytes));
                    }
                }
                else if (nextChar != 0)
                {
                    Add(new UnicodeCharacters(nextChar));
                }
            }
        }

        public UnicodeString(IEnumerable<IUnicodeCharacters> fromChars)
        {
            AddRange(fromChars);
        }

        public UnicodeString Replace(UnicodeString replace, UnicodeString with)
        {
            var newStr = new List<IUnicodeCharacters>();
            var charCount = replace.Count;
            
            for (int i = 0; i < Count; i++)
            {
                if (i + charCount < Count)
                {
                    var chr = new UnicodeString(GetRange(i, charCount));

                    if (chr.Equals(replace))
                    {
                        newStr.AddRange(with);
                        i += charCount - 1;
                        continue;
                    }
                }

                newStr.Add(this[i]);
            }

            return new UnicodeString(newStr);
        }

        public bool Contains(UnicodeString str)
        {
            var charCount = str.Count();

            for (int i = 0; i < Count; i++)
            {
                if (i + charCount > Count)
                    break;

                var chr = new UnicodeString(GetRange(i, charCount));

                if (chr.Equals(str))
                {
                    return true;
                }
            }

            return false;
        }

        public override bool Equals(object obj)
        {
            if (obj is IEnumerable<IUnicodeCharacters> other && other.Count() == Count)
            {
                return this.CheckListEquality(other);
            }
            return false;
        }
    }
}
