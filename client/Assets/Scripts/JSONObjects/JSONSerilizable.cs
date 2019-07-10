using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

public class JSONSerializable<T> {
    public static T FromJSONObject(JSONObject json) {
        return JsonConvert.DeserializeObject<T>(json.ToString());
    }

    public string Serialize() {
        return JObject.FromObject(this).ToString();
    }

    public JSONObject ToJSONObject() {
        return new JSONObject(Serialize());
    }
}