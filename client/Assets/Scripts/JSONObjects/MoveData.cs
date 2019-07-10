public class MoveData : JSONSerializable<MoveData> {
    public int playerId { get; set; }
    public int x { get; set; }
    public int y { get; set; }
    public int width { get; set; }
    public int height { get; set; }

    public MoveData(int playerId, int x, int y, int width, int height) {
        this.playerId = playerId;
        this.x = x;
        this.y = y;
        this.width = width;
        this.height = height;
    }
}