using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using SocketIO;

public abstract class FormManager : MonoBehaviour {
    [SerializeField] protected Text formError;
    [SerializeField] protected Selectable[] tabs;

    Communicator communicator;
    protected MusicManager music;

    // Start is called before the first frame update
    protected virtual void Start() {
        formError.enabled = false;
        FocusGameObject(false);
        communicator = FindObjectOfType<Communicator>();
        music = FindObjectOfType<MusicManager>();
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.Tab)) {
            FocusGameObject(Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift));
        }
    }

    void FocusGameObject(bool isBackward) {
        GameObject selectedObject = EventSystem.current.currentSelectedGameObject;
        bool none = !selectedObject;
        int currentIndex = 0;
        int nextIndex;
        if (!none) {
            //find the current tab index
            for (var i = 0; i < tabs.Length; i++) {
                if (tabs[i].gameObject == selectedObject.gameObject) {
                    currentIndex = i;
                    break;
                }
                // none = true;
            }
        }
        if (none) {
            nextIndex = isBackward ? tabs.Length - 1 : 0;
        } else {
            nextIndex = (isBackward ? currentIndex - 1 : currentIndex + 1);
            if (nextIndex < 0) nextIndex = tabs.Length - 1;
            if (nextIndex > tabs.Length - 1) nextIndex = 0;
        }
        Selectable next = tabs[nextIndex];
        InputField inputField = next.GetComponent<InputField>();
        if (inputField != null) {
            inputField.OnPointerClick(new PointerEventData(EventSystem.current));
        }
        next.Select();
    }

    public void Submit() {
        formError.enabled = false;
        AddHandlers();
        communicator.SendMessage(GetFormEvent(), GetFormData());
    }

    protected void AddHandlers() {
        communicator.AddHandler(GetFormEvent() + "error", Error);
        communicator.AddHandler(GetFormEvent() + "success", Success);
    }

    protected void DropHandlers() {
        // communicator.DropHandler(GetFormEvent() + "error", Error);
        // communicator.DropHandler(GetFormEvent() + "success", Success);
    }

    protected abstract string GetFormEvent();
    protected abstract JSONObject GetFormData();
    protected abstract void OnError(SocketIOEvent ev);
    protected abstract void OnSuccess(SocketIOEvent ev);
    protected void Error(SocketIOEvent ev) {
        DropHandlers();
        OnError(ev);
        formError.enabled = true;
    }
    protected void Success(SocketIOEvent ev) {
        DropHandlers();
        OnSuccess(ev);
    }
}