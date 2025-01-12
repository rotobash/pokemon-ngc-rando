namespace RandomizerCLI.AP
{
    public class RegionsJson
    {
        public required string AreaName { get; set; }
        public int RoomIndex { get; set; }
        public bool Starting { get; set; }
        public string Requires { get; set; }
        public string[] ConnectsTo { get; set; } = Array.Empty<string>();
        public string Name { get; set; } = string.Empty;
    }
}