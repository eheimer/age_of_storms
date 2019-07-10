public class PlayerData : JSONSerializable<PlayerData> {
    public int playerId { get; set; }
    public int x { get; set; }
    public int y { get; set; }
}