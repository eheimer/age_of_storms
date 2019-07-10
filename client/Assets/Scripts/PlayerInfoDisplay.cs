using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInfoDisplay : MonoBehaviour {
    [SerializeField] Text playername;
    [SerializeField] Text hitpoints;
    [SerializeField] Text location;
    [SerializeField] Text leaderboard;

    Player player;
    Canvas canvas;
    Communicator communicator;

    // Start is called before the first frame update
    void Start() {
        player = FindObjectOfType<Player>();
        canvas = GetComponent<Canvas>();
        communicator = FindObjectOfType<Communicator>();
        communicator.SetInfo(this);
        canvas.enabled = false;
    }

    // Update is called once per frame
    void Update() {
        //while canvas is enabled, update the display every frame
        if (canvas.enabled) {
            UpdateDisplay();
        }

        if (Input.GetKeyDown(KeyCode.I)) {
            if (!canvas.enabled) {
                UpdateDisplay();
            }
            //toggle canvas visibility
            canvas.enabled = !canvas.enabled;
        }
    }

    void UpdateDisplay() {
        //update all of the values
        playername.text = player.playerName;
        hitpoints.text = player.currentHp + "/" + player.hp;
        location.text = player.GetWorldPosition().ToString();
    }

    public void UpdateLeaderboard(LeaderboardData data) {
        StringBuilder sb = new StringBuilder("Leaders").AppendLine();
        foreach (LeaderboardData.Leader leader in data.leaders) {
            sb.Append(leader.name).Append(" - ").Append(leader.gold).AppendLine();
        }
        leaderboard.text = sb.ToString();
    }
}
