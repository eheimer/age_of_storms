using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour {
    [SerializeField] float moveSpeed = 1f;
    [SerializeField] Transform hpbar;
    [SerializeField] GameObject deathDisplay;
    [SerializeField] AudioClip cantMove;
    [SerializeField] AudioClip playerDead;
    [SerializeField] [Range(0, 1)] float sfxVolume = .3f;
    [SerializeField] Text goldDisplay;

    MapBuilder map;
    MusicManager music;
    Vector2Int worldPosition;
    public string playerName { get; set; }
    public int playerId { get; set; }
    private int _gold;
    public int gold {
        get { return _gold; }
        set {
            //TODO: update the display
            _gold = value;
            goldDisplay.text = value.ToString();
        }
    }
    public int hp { get; set; }
    private int _currentHp;
    public int currentHp {
        get { return _currentHp; }
        set {
            if (value <= 0) {
                HasDied();
            } else {
                _currentHp = value;
                float percent = (float)_currentHp / (float)hp;
                var scale = hpbar.localScale;
                scale.x = percent;
                Debug.Log("setting player scale:" + scale.x);
                hpbar.localScale = scale;
            }
        }
    }
    public bool encounter { get; set; }
    public bool dead { get; set; }
    bool inputBlocked = false;

    //this starts at true so that we don't allow player movement
    //until we get the initial player position from the server,
    bool moving = true;
    Vector2Int moveTo;

    void Start() {
        map = FindObjectOfType<MapBuilder>();
        music = FindObjectOfType<MusicManager>();
        if (deathDisplay) deathDisplay.SetActive(false);
    }

    // Update is called once per frame
    void Update() {
        if (dead) {
            DeathCheck();
        } else {
            MoveCheck();
        }
    }

    public bool IsMoving() {
        return moving || map.IsMoving();
    }

    bool CanMove() {
        return !IsMoving() && !encounter && !dead;
    }

    /**
    ** Returns the speed adjustment, if the tile is passable
     */
    float CanMoveToLoc(Vector2Int dest) {
        float ret = 0f;
        if (CanMove()) {
            //get the tile at the dest (considering mapOffset)
            var intDest = new Vector2Int(Mathf.FloorToInt(dest.x), Mathf.FloorToInt(dest.y));
            int? tileType = map.GetTileAt(intDest);
            // return speed if the tile is passable (plains/forest)
            switch (tileType) {
                case 0:
                    ret = 1f; //plain
                    break;
                case 2:
                    ret = 0.8f; //forest
                    break;
                case 1: //mountain
                case 3: //water
                default:
                    break;
            }
            if (ret == 0f) {
                //movement is available, but we're not able to move into the location
                StartCoroutine(PlayMoveBlocked());
            }
        }
        return ret;
    }

    bool blockSoundPaused;
    IEnumerator PlayMoveBlocked() {
        if (!blockSoundPaused) {
            blockSoundPaused = true;
            AudioSource.PlayClipAtPoint(cantMove, Camera.main.transform.position, sfxVolume);
            yield return new WaitForSeconds(.5f);
            blockSoundPaused = false;
        }
        yield return null;
    }

    public void Move(Vector2Int move) {
        Vector2Int dest = worldPosition + move;
        float movement = moveSpeed * CanMoveToLoc(dest);
        if (movement > 0) {
            //tell the map to move in the opposite direction
            moving = true;
            map.MoveToNewPosition(dest, movement);
            worldPosition = dest;
        }
        moving = false;
    }

    void MoveCheck() {
        if (CanMove()) {
            int x = 0;
            int y = 0;
            bool swipeRight = false;
            bool swipeLeft = false;
            bool swipeUp = false;
            bool swipeDown = false;
            if (Input.touchCount == 1) {
                Touch touch = Input.GetTouch(0);
                if (touch.phase == TouchPhase.Ended) {
                    //after a swipe is done
                    if (touch.deltaPosition.x > 2.5) swipeRight = true;
                    if (touch.deltaPosition.x < -2.5) swipeLeft = true;
                    if (touch.deltaPosition.y > 2.5) swipeUp = true;
                    if (touch.deltaPosition.y < -2.5) swipeDown = true;
                }
            }
            if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.Keypad6) ||
              Input.GetKey(KeyCode.Keypad9) || Input.GetKey(KeyCode.PageUp) ||
              Input.GetKey(KeyCode.Keypad3) || Input.GetKey(KeyCode.PageDown) ||
              Input.GetKey(KeyCode.D) || swipeRight) {
                x = 1;
            }
            if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.Keypad4) ||
              Input.GetKey(KeyCode.Keypad1) || Input.GetKey(KeyCode.End) ||
              Input.GetKey(KeyCode.Keypad7) || Input.GetKey(KeyCode.Home) ||
              Input.GetKey(KeyCode.A) || swipeLeft) {
                x = -1;
            }
            if (Input.GetKey(KeyCode.Keypad9) || Input.GetKey(KeyCode.PageUp) ||
              Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.Keypad8) ||
              Input.GetKey(KeyCode.Keypad7) || Input.GetKey(KeyCode.Home) ||
              Input.GetKey(KeyCode.W) || swipeUp) {
                y = 1;
            }
            if (Input.GetKey(KeyCode.Keypad3) || Input.GetKey(KeyCode.PageDown) ||
              Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.Keypad2) ||
              Input.GetKey(KeyCode.Keypad1) || Input.GetKey(KeyCode.End) ||
              Input.GetKey(KeyCode.S) || swipeDown) {
                y = -1;
            }
            if (x != 0 || y != 0) {
                Move(new Vector2Int(x, y));
            }
        }
    }

    void DeathCheck() {
        bool longPress = false;
        if (Input.touchCount == 1) {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Ended) {
                if (Mathf.Abs(touch.deltaPosition.y) < .5 && Mathf.Abs(touch.deltaPosition.x) < .5 && touch.deltaTime > .5) longPress = true;
            }
        }
        if (!inputBlocked && (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter) || longPress)) {
            FindObjectOfType<GameManager>().Reset();
        }
    }

    IEnumerator blockInput(int seconds = 2) {
        inputBlocked = true;
        yield return new WaitForSeconds(seconds);
        inputBlocked = false;
    }

    public Vector2Int GetWorldPosition() {
        return worldPosition;
    }

    /**
       * This should be called once when the client makes the
       * initial call to the server and gets the player position.
       * Here we set moving=false to allow player movement
       */
    public void SetWorldPosition(Vector2Int position) {
        //TODO: should this initiate the request for the map?
        this.worldPosition = position;
        this.moving = false;
        map.Initialize(this.worldPosition);
    }

    //disable movement and display the death screen
    public void HasDied() {
        this.dead = true;
        StartCoroutine(blockInput(2));
        music.StopMusic();
        AudioSource.PlayClipAtPoint(playerDead, Camera.main.transform.position, sfxVolume);
        deathDisplay.SetActive(true);
    }
}
