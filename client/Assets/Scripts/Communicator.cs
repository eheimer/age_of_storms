using System.Text;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using SocketIO;
using UnityEngine.SceneManagement;

public class Communicator : MonoBehaviour {
    [SerializeField] int mapWidth;
    [SerializeField] int mapHeight;

    public static MapBuilder map;
    public static SocketIOComponent socket;
    public static Player player;
    public static Encounter encounter;
    public static PlayerInfoDisplay info = null;

    void Start() {
        socket = GetComponent<SocketIOComponent>();
        map = FindObjectOfType<MapBuilder>();
        player = FindObjectOfType<Player>();
        encounter = FindObjectOfType<Encounter>();
        info = FindObjectOfType<PlayerInfoDisplay>();
        AddHandler("map", OnSocketMap);
        AddHandler("player", OnSocketPlayer);
        AddHandler("playerstatus", OnSocketPlayerStatus);
        AddHandler("encounter", OnSocketEncounter);
        AddHandler("leaderboard", OnSocketLeaderboard);
        if (player.playerId == 0) {
            SceneManager.LoadScene("Login");
        }
    }

    public void SetInfo(PlayerInfoDisplay infoDisplay) {
        info = infoDisplay;
    }

    public void AddHandler(string ev, Action<SocketIOEvent> action) {
        socket.On(ev, action);
    }

    public void DropHandler(string ev, Action<SocketIOEvent> action) {
        socket.Off(ev, action);
    }

    public void SendMessage(string ev, JSONObject data) {
        socket.Emit(ev, data);
    }

    public void SendMapRequest(Vector2Int position) {
        // data needs to look like this:
        //{ playerId: 1, x: 0, y: 0, width: 20, height: 12 }
        MoveData data = new MoveData(player.playerId, position.x, position.y, mapWidth, mapHeight);
        SendMessage("move", data.ToJSONObject());
    }

    public void OnSocketMap(SocketIOEvent ev) {
        //send the new mapdata to the MapBuilder
        MapData data = MapData.FromJSONObject(ev.data);
        map.SetMapData(data.ToArray());
    }

    public void OnSocketPlayer(SocketIOEvent ev) {
        //this will get other player locations
        //we need to add these to our map
        PlayerData data = PlayerData.FromJSONObject(ev.data);
        Vector2Int position = new Vector2Int(data.x, data.y);
        map.AddPlayerIcon(data.playerId, position);
    }

    public void OnSocketPlayerStatus(SocketIOEvent ev) {
        PlayerStatusData data = PlayerStatusData.FromJSONObject(ev.data);
        if (data.hp != null) { player.hp = (int)data.hp; }
        if (data.currentHp != null) {
            player.currentHp = (int)data.currentHp;
            if (data.currentHp <= 0) {
                encounter.EndEncounter(true);
            }
        }
        if (data.gold != null) { player.gold = (int)data.gold; }
        //set player location;
        if (data.x != 0 && data.y != 0) {
            Vector2Int playerWorldPosition = new Vector2Int((int)data.x, (int)data.y);
            player.SetWorldPosition(playerWorldPosition);
            map.Initialize(playerWorldPosition);
            SendMapRequest(playerWorldPosition);
        }
    }

    public void OnSocketEncounter(SocketIOEvent ev) {
        AddHandler("fightresult", encounter.HandleFightResult);
        AddHandler("fleeresult", encounter.HandleFleeResult);
        encounter.StartEncounter(EncounterData.FromJSONObject(ev.data));
    }

    public void OnSocketLeaderboard(SocketIOEvent ev) {
        if (info != null) {
            LeaderboardData data = LeaderboardData.FromJSONObject(ev.data);
            info.UpdateLeaderboard(data);
        }
    }
}
