public class RegisterData : JSONSerializable<RegisterData> {
    public string username { get; set; }
    public string password { get; set; }
    public string playerName { get; set; }

    public RegisterData(string username, string password, string playerName) {
        this.username = username;
        this.password = password;
        this.playerName = playerName;
    }
}