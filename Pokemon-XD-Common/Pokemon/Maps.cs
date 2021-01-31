using System;
using System.Collections.Generic;
using System.Text;

namespace XDCommon.PokemonDefinitions
{
    public static partial class Pokemon
    {
        public enum Maps
        {
            Demo,
            ShadowLab,
            MtBattle,
            SSLibra,
            RealgamTower,
            CipherKeyLair,
            CitadarkIsle,
            OrreColosseum,
            PhenacCity,
            PyriteTown,
            AgateVillage,
            TheUnder,
            PokemonHQ,
            GateonPort,
            OutskirtStand,
            SnagemHideout,
            KaminkosHouse,
            AncientColo,
            Pokespot,
            Unknown
        }

        public static string GetCode(this Maps map)
        {
            return map switch
            {
                Maps.Demo => "B1",
                Maps.ShadowLab => "D1",
                Maps.MtBattle => "D2",
                Maps.SSLibra => "D3",
                Maps.RealgamTower => "D4",
                Maps.CipherKeyLair => "D5",
                Maps.CitadarkIsle => "D6",
                Maps.OrreColosseum => "D7",
                Maps.PhenacCity => "M1",
                Maps.PyriteTown => "M2",
                Maps.AgateVillage => "M3",
                Maps.TheUnder => "M4",
                Maps.PokemonHQ => "M5",
                Maps.GateonPort => "M6",
                Maps.OutskirtStand => "S1",
                Maps.SnagemHideout => "S2",
                Maps.KaminkosHouse => "S3",
                Maps.AncientColo => "T1",
                Maps.Pokespot => "es",
                _ => "Unknown",
            };
        }
    }
}
