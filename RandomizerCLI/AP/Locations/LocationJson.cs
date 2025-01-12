namespace RandomizerCLI.AP.Locations
{
    public abstract class LocationJson
    {
        public required virtual int Index { get; set; }
        public required virtual string AreaName { get; set; }
        public required virtual int RoomId { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}