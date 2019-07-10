using System.Data.Common;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SocketIO;

public class Encounter : MonoBehaviour {
    [SerializeField] GameObject encounterDisplay;
    [SerializeField] Transform enemyHp;
    [SerializeField] Transform selector;
    [SerializeField] AudioClip victory;
    [SerializeField] AudioClip hit;
    [SerializeField] AudioClip miss;
    [SerializeField] AudioClip flee;
    [SerializeField] [Range(0, 1)] float volume = .3f;

    Player player;
    Communicator communicator;
    MusicManager music;
    bool active;
    bool waitingForServer;
    int chosenAction;
    bool inputBlocked = false;

    string[] actions = new string[] { "fight", "flee" };
    float[] selectorY = new float[] { 0.8f, 0.36f };

    // Start is called before the first frame update
    void Start() {
        player = FindObjectOfType<Player>();
        communicator = FindObjectOfType<Communicator>();
        encounterDisplay.SetActive(false);
        music = FindObjectOfType<MusicManager>();
    }

    // Update is called once per frame
    void Update() {
        if (active && !inputBlocked) {
            bool swipeUp = false;
            bool swipeDown = false;
            bool longPress = false;
            if (Input.touchCount == 1) {
                Touch touch = Input.GetTouch(0);
                if (touch.phase == TouchPhase.Ended) {
                    //after a swipe is done
                    if (touch.deltaPosition.y > 2.5) swipeUp = true;
                    if (touch.deltaPosition.y < -2.5) swipeDown = true;
                    if (Mathf.Abs(touch.deltaPosition.y) < .5 && Mathf.Abs(touch.deltaPosition.x) < .5 && touch.deltaTime > .5) longPress = true;
                }
            }
            if ((Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.Keypad2) || swipeDown) && chosenAction < actions.Length - 1) {
                chosenAction++;
                MoveSelector();
            }
            if ((Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.Keypad8) || swipeUp) && chosenAction > 0) {
                chosenAction--;
                MoveSelector();
            }
            if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter) || longPress) {
                SubmitAction(actions[chosenAction]);
            }
        }
    }

    void MoveSelector() {
        var pos = selector.localPosition;
        pos.y = selectorY[chosenAction];
        selector.localPosition = pos;
    }

    void SubmitAction(string fightOrFlee) {
        waitingForServer = true;
        communicator.SendMessage("encounteraction", new EncounterActionData(fightOrFlee).ToJSONObject());
    }

    int _startingHp = 0;
    int _currentHp = 0;
    public void StartEncounter(EncounterData data) {
        chosenAction = 0;
        MoveSelector();
        player.encounter = true;
        active = true;
        waitingForServer = false;
        _startingHp = data.hp;
        SetCurrentHp(data.hp);
        StartCoroutine(blockInput());
        music.PlayBattle();
        encounterDisplay.SetActive(true);
    }

    IEnumerator blockInput(int seconds = 2) {
        inputBlocked = true;
        yield return new WaitForSeconds(seconds);
        inputBlocked = false;
    }

    public void SetCurrentHp(int currentHp) {
        _currentHp = currentHp;
        Debug.Log("start:" + _startingHp + ",current:" + _currentHp);
        var scale = enemyHp.localScale;
        float percent = (float)_currentHp / (float)_startingHp;
        scale.x = percent;
        Debug.Log("setting enemy scale:" + scale.x);
        enemyHp.localScale = scale;
    }

    public void EndEncounter(bool playerDead) {
        if (!playerDead) {
            music.PlayField();
        }
        player.encounter = false;
        active = false;
        waitingForServer = false;
        encounterDisplay.SetActive(false);
        communicator.DropHandler("fightresult", HandleFightResult);
        communicator.DropHandler("fleeresult", HandleFleeResult);
    }

    public void HandleFightResult(SocketIOEvent ev) {
        //if death of player-- what do we do?
        //if enemy death, player.encounter = false;
        //else, update hpbars for both player and enemy
        //if the player has died, we won't get this message.  The player must explicitly call EndEncounter(true)
        // to clean up the encounter.
        FightResultData data = FightResultData.FromJSONObject(ev.data);
        bool enemyDamaged = data.enemyHp < _currentHp;
        bool playerDamaged = data.playerDamaged;
        bool enemyDead = data.enemyHp <= 0;
        bool anyDamage = enemyDamaged || playerDamaged;
        if (!enemyDead) {
            if (anyDamage) {
                AudioSource.PlayClipAtPoint(hit, Camera.main.transform.position, volume);
            } else {
                AudioSource.PlayClipAtPoint(miss, Camera.main.transform.position, volume);
            }
            SetCurrentHp(data.enemyHp);
        } else {
            EndEncounter(false);
            if (data.enemyHp <= 0) {
                AudioSource.PlayClipAtPoint(victory, Camera.main.transform.position, volume);
            }
        }
        waitingForServer = false;
    }

    public void HandleFleeResult(SocketIOEvent ev) {
        FleeResultData data = FleeResultData.FromJSONObject(ev.data);
        if (data.success) {
            EndEncounter(false);
            AudioSource.PlayClipAtPoint(flee, Camera.main.transform.position, volume);
        } else {
            AudioSource.PlayClipAtPoint(miss, Camera.main.transform.position, volume);
        }
        waitingForServer = false;
    }
}
