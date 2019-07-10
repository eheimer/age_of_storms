public class AuthData : JSONSerializable<AuthData> {
    public string username { get; set; }
    public string password { get; set; }

    public AuthData(string username, string password) {
        this.username = username;
        this.password = password;
    }
}