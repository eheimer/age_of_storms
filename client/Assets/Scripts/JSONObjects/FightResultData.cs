public class FightResultData : JSONSerializable<FightResultData> {
    public int enemyHp { get; set; }
    public bool playerDamaged { get; set; }
}