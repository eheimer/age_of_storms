using System.Runtime.CompilerServices;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using SocketIO;

public class LoginForm : FormManager {
    [SerializeField] Text usernameComponent;
    [SerializeField] InputField passwordComponent;
    [SerializeField] AudioClip loginSuccess;
    [SerializeField] [Range(0, 1)] float volume = 0.3f;

    protected override void Start() {
        base.Start();
        music.PlayIntro();
    }

    protected override string GetFormEvent() {
        return "auth";
    }

    protected override JSONObject GetFormData() {
        AuthData data = new AuthData(usernameComponent.text, passwordComponent.text);
        return data.ToJSONObject();
    }

    protected override void OnSuccess(SocketIOEvent ev) {
        //handle authsuccess
        AuthSuccess data = AuthSuccess.FromJSONObject(ev.data);
        Player player = FindObjectOfType<Player>();
        player.playerId = data.playerId;
        player.playerName = data.name;
        StartCoroutine(VictoryAndWaitBeforeLoading());
    }

    IEnumerator VictoryAndWaitBeforeLoading() {
        music.StopMusic();
        AudioSource.PlayClipAtPoint(loginSuccess, Camera.main.transform.position, volume);
        yield return new WaitForSeconds(2);
        music.PlayField();
        SceneManager.LoadScene("Map");
    }

    protected override void OnError(SocketIOEvent ev) {
        //handle autherror
        FormErrorData data = FormErrorData.FromJSONObject(ev.data);
        formError.text = "[" + data.field + "] " + data.error;
    }

    public void Register() {
        SceneManager.LoadScene("Register");
    }
}
