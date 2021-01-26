using System;
using System.Collections.Generic;
using System.Text;

namespace XDCommon.Pokemon
{
    public static partial class Pokemon
    {
        public enum EvolutionMethods
        {
            None,
            MaxHappiness,
            HappinessDay,
            HappinessNight,
            LevelUp,
            Trade,
            TradeWithItem,
            EvolutionStone,
            MoreAttack,
            EqualAttack,
            MoreDefense,
            Silcoon,
            Cascoon,
            Ninjask,
            Shedinja,
            MaxBeauty,
            LevelUpWithKeyItem,
            Gen4
        }

        public static EvolutionConditionType ConditionType(this EvolutionMethods evolutionMethod)
        {
            switch (evolutionMethod)
            {
                case EvolutionMethods.LevelUp:
                case EvolutionMethods.MoreAttack:
                case EvolutionMethods.MoreDefense:
                case EvolutionMethods.EqualAttack:
                case EvolutionMethods.Silcoon:
                case EvolutionMethods.Cascoon:
                case EvolutionMethods.Ninjask:
                case EvolutionMethods.Shedinja:
                    return EvolutionConditionType.Level;
                case EvolutionMethods.TradeWithItem:
                case EvolutionMethods.EvolutionStone:
                case EvolutionMethods.LevelUpWithKeyItem:
                    return EvolutionConditionType.Item;
                default:
                    return EvolutionConditionType.None;
            };
        }
    }
}
