using System;
using System.Collections.Generic;
using System.Text;

namespace XDCommon.Pokemon
{
    public static partial class Pokemon
    {
        public enum MoveEffectTypes
        {
            None,
            Attack,
            Healing,
            StatNerf,
            StatBuff,
            StatusEffect,
            GlobalEffect,
            OHKO,
            Multiturn,
            Misc,
            Misc2,
            Misc3,
            Misc4,
            Unknown
        }
    }
}
