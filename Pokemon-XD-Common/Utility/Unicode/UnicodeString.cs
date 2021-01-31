using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace XDCommon.Utility
{
    public class UnicodeString : List<IUnicodeCharacters>
    {
        public override string ToString()
        {
            return string.Join("", this);
        }
    }
}
