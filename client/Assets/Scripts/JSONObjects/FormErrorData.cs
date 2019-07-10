public class FormErrorData : JSONSerializable<FormErrorData> {
    public string field { get; set; }
    public string error { get; set; }
}