namespace APCommon.JSON
{
    public class TrainerBattleLocationJson : LocationJson
    {
        public int TrainerIndex { get; set; }
        public int BattleIndex { get; set; }
        public string TrainerBattleType { get; set; }
    }
}