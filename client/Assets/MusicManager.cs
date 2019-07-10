using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour {
    [SerializeField] [Range(0, 1)] float volume = 0.02f;
    [SerializeField] AudioClip intro;
    [SerializeField] AudioClip field;
    [SerializeField] AudioClip battle1;
    [SerializeField] AudioClip battle2;

    static MusicManager instance;

    AudioSource source;

    void Awake() {
        if (FindObjectsOfType<MusicManager>().Length > 1) {
            Destroy(gameObject);
        } else {
            instance = this;
        }
    }

    // Start is called before the first frame update
    void Start() {
        source = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update() {

    }

    public void PlayIntro() {
        SwitchMusic(intro, true, true);
    }

    public void PlayField() {
        SwitchMusic(field, true, true);
    }

    public void PlayBattle() {
        StartCoroutine(PlayBattleLoop());
    }

    public void StopMusic() {
        if (source.isPlaying) {
            source.Stop();
        }
    }

    bool _cancel;
    IEnumerator PlayBattleLoop() {
        SwitchMusic(battle1, false, false);
        yield return new WaitForSeconds(180);
        if (!_cancel) {
            SwitchMusic(battle2, true, true);
        }
        yield return null;
    }

    void SwitchMusic(AudioClip clip, bool loop, bool cancel) {
        if (cancel) {
            _cancel = true;
        }
        StopMusic();
        source.clip = clip;
        source.loop = loop;
        source.volume = volume;
        source.Play();
    }
}
