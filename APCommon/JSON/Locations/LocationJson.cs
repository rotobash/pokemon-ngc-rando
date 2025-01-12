namespace APCommon.JSON
{
    public abstract class LocationJson
    {
        public virtual int Index { get; set; }
        public virtual string AreaName { get; set; }
        public virtual int RoomId { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}