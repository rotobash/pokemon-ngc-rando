namespace APCommon.JSON
{
    public enum ItemClassification {  Filler, Useful, Progression, SkipBalancing }
    public abstract class ItemJson
    {
        public int Index { get; set; }
        public int Quantity { get; set; } = 1;
        public string Name { get; set; } = string.Empty;
        public bool Progression { get; set; }
        public string[] ItemClassification { get; set; }
    }
}