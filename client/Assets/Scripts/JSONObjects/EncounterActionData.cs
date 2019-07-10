public class EncounterActionData : JSONSerializable<EncounterActionData> {
    public string action { get; set; }

    public EncounterActionData(string action) {
        this.action = action;
    }
}