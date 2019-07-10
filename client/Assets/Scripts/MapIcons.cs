using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
** TODO: This class started out as throwing a lot of shit at the wall trying to
**  figure out all the vector path and positioning.
**  there are probably a few variables and calculations in here that we don't
**  need and should be cleaned up to clarify the code
*/

public class MapIcons : MonoBehaviour {
    [SerializeField] GameObject otherPlayerPrefab;

    List<MapIcon> players = new List<MapIcon>();
    Vector2Int worldOffset;

    // Start is called before the first frame update
    void Start() {

    }

    // Update is called once per frame
    void Update() {
    }

    public void AddPlayer(int id, Vector2Int worldPosition, Vector2Int tilePosition) {
        //check if we already have the id
        MapIcon existing = null;
        foreach (MapIcon m in players) {
            if (m.id == id) {
                Debug.Log("Already have the player: " + m.ToString());
                existing = m;
                break;
            }
        }
        //if we do not, add it
        if (existing == null) {
            Debug.Log("Creating a new player: " + worldPosition.ToString());
            GameObject icon = Instantiate(otherPlayerPrefab, new Vector3(tilePosition.x, tilePosition.y, transform.position.z), Quaternion.identity, transform);
            players.Add(new MapIcon(id, worldPosition, icon));
        } else {
            //if we do and the position has changed, move it
            if (existing.position != worldPosition) {
                Debug.Log("Moving the player");
                existing.gameObject.transform.position = new Vector3(tilePosition.x, tilePosition.y, existing.gameObject.transform.position.z) + existing.gameObject.transform.parent.position;
                existing.position = worldPosition;
            }
        }
    }

    protected class MovingIcon {
        public MapIcon icon;
        public Vector2Int destination;

        public MovingIcon(MapIcon icon, Vector2Int dest) {
            this.icon = icon;
            this.destination = dest;
        }
    }

    protected class MapIcon {
        public int id;
        public Vector2Int position;
        public GameObject gameObject;

        public MapIcon(int id, Vector2Int position, GameObject gameObject) {
            this.id = id;
            this.position = position;
            this.gameObject = gameObject;
        }

        public override string ToString() {
            return this.id + ":" + this.position.ToString() + ":" + this.gameObject.transform.position.ToString();
        }
    }
}
