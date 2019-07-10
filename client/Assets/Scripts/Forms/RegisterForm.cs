using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using SocketIO;

public class RegisterForm : FormManager {
    [SerializeField] Text usernameComponent;
    [SerializeField] InputField passwordComponent;
    [SerializeField] Text characterNameComponent;

    protected override string GetFormEvent() {
        return "register";
    }

    protected override JSONObject GetFormData() {
        RegisterData data = new RegisterData(usernameComponent.text, passwordComponent.text, characterNameComponent.text);
        return data.ToJSONObject();
    }

    protected override void OnSuccess(SocketIOEvent ev) {
        //handle authsuccess
        SceneManager.LoadScene("Login");
    }

    protected override void OnError(SocketIOEvent ev) {
        //handle autherror
        FormErrorData data = FormErrorData.FromJSONObject(ev.data);
        formError.text = "[" + data.field + "] " + data.error;
    }
}
