public class PlayerStatusData : JSONSerializable<PlayerStatusData> {
    public int? hp { get; set; }
    public int? currentHp { get; set; }
    public int? gold { get; set; }
    public int? x { get; set; }
    public int? y { get; set; }
}