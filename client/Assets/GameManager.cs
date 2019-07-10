using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {
    public static GameManager instance;
    public static Communicator communicator;
    public static Player player;

    void Awake() {
        if (FindObjectsOfType<GameManager>().Length > 1) {
            DestroyAll();
        } else {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    // Start is called before the first frame update
    void Start() {
        //update references to all child objects
        communicator = FindObjectOfType<Communicator>();
    }

    public void Reset() {
        DestroyAll();
        SceneManager.LoadScene("Map");
    }

    void DestroyAll() {
        for (int i = 0; i < transform.childCount; i++) {
            var child = transform.GetChild(i);
            Destroy(child.gameObject);
        }
        Destroy(gameObject);
    }
}
