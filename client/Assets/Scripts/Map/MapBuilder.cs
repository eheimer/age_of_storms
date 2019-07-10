using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

/*
** TODO: This class started out as throwing a lot of shit at the wall trying to
**  figure out all the vector path and positioning.
**  there are probably a few variables and calculations in here that we don't
**  need and should be cleaned up to clarify the code
*/

public class MapBuilder : MonoBehaviour {
    string[] tiles = { "plains", "mountain", "forest", "water" };
    [SerializeField] Tile emptyTile;

    //windowOffset is the distance from the player (center of the camera)
    //to the lower-left visible tile
    Vector2Int windowOffset = new Vector2Int(-9, -5);
    Vector2Int pad = new Vector2Int(1, 1);
    Vector2Int lastUpdatePosition;  //the player world position
    Vector2Int initialPlayerPosition; //the player world position at initialization
    int drawWidth;
    int drawHeight;
    Vector2Int initialTransformPosition;
    //worldOffset is the difference vector between the initialTransformPosition
    //and the initial player world position
    Vector2Int worldOffset;
    bool initialized = false;
    //virtualTilemapOffset is a "virtual" vector that maps the unity
    // transform.position of the tileMap to the players "world" position
    // this value must be used to position any icons on the map by
    // world position
    Vector2Int virtualTilemapOffset = new Vector2Int(9, 5);

    //movement
    bool moving = false;
    Vector3 dest;
    float moveSpeed;

    int?[,] mapData;

    static Tilemap tileMap;
    static Player player;
    static Communicator communicator;
    static MapIcons mapIcons;

    void Start() {
        tileMap = FindObjectOfType<Tilemap>();
        player = FindObjectOfType<Player>();
        communicator = FindObjectOfType<Communicator>();
        mapIcons = FindObjectOfType<MapIcons>();
    }

    // Update is called once per frame
    void Update() {
        if (moving) {
            tileMap.transform.position = Vector3.MoveTowards(tileMap.transform.position, dest, moveSpeed * Time.deltaTime);
            if (tileMap.transform.position == dest) {
                moving = false;
                moveSpeed = 0;
            }
        }
        UpdatePosition(player.GetWorldPosition());
    }

    public void Initialize(Vector2Int playerWorldPosition) {
        initialized = false;
        drawWidth = Mathf.Abs(windowOffset.x - pad.x) * 2;
        drawHeight = Mathf.Abs(windowOffset.y - pad.y) * 2;
        lastUpdatePosition = playerWorldPosition;
        initialTransformPosition = new Vector2Int();
        worldOffset = initialTransformPosition - playerWorldPosition;
        tileMap.ClearAllTiles();
        tileMap.transform.position = new Vector3(initialTransformPosition.x, initialTransformPosition.y, tileMap.transform.position.z);
        initialPlayerPosition = playerWorldPosition;
        communicator.SendMapRequest(initialPlayerPosition);
        initialized = true;
    }

    public Vector2Int GetTilePositionRelativeToWorldPosition(Vector2Int worldPosition) {
        Vector2Int relativeToPlayer = worldPosition - lastUpdatePosition;
        Vector2Int playerTile = lastUpdatePosition - initialPlayerPosition;
        return playerTile + relativeToPlayer - windowOffset;
    }

    public bool IsMoving() {
        return moving;
    }

    public void MoveToNewPosition(Vector2Int newPosition, float speed) {
        if (!moving) {
            moving = true;
            // this is the vector that the grid position needs to change by
            Vector2Int changeVector = lastUpdatePosition - newPosition;
            Vector3 change3 = new Vector3(changeVector.x, changeVector.y, 0);
            //destination is the current position + change3
            dest = tileMap.transform.position + change3;
            moveSpeed = speed;
        }
    }

    public void SetMapData(int?[,] mapData) {
        this.mapData = mapData;
        DebugMapToString();
        DrawMap();
    }

    void UpdatePosition(Vector2Int newPosition, bool force = false) {
        if (lastUpdatePosition != newPosition || force) {
            lastUpdatePosition = newPosition;
            communicator.SendMapRequest(newPosition);
        }
    }

    void DrawMap() {
        try {
            Vector2Int startingAt = lastUpdatePosition + worldOffset - pad;
            for (var row = 0; row < mapData.GetLength(1); row++) {
                for (var col = 0; col < mapData.GetLength(0); col++) {
                    var tilePosition = new Vector3Int(startingAt.x + col, startingAt.y + row, 0);
                    tileMap.SetTile(tilePosition, ChooseTile(col, row));
                }
            }
        } catch (Exception ex) {
            Debug.Log("DrawMap exception: " + ex.Message);
        }
        tileMap.RefreshAllTiles();
    }


    Tile ChooseTile(int x, int y) {
        try {
            int?[,] grid = TileGrid(x, y);
            int? tileType = mapData[x, y];
            if (tileType == null) return emptyTile;
            // //redraw plains based on surrounding tiles
            if (tileType == 0) {
                //mountains
                if (grid[1, 2] == 1) {
                    return GetTile("mountain_s");
                } else if (grid[1, 0] == 1) {
                    return GetTile("mountain_n");
                } else if (grid[0, 1] == 1) {
                    return GetTile("mountain_e");
                } else if (grid[2, 1] == 1) {
                    return GetTile("mountain_w");
                }
                //forest
                if (grid[1, 2] == 2) {
                    return GetTile("forest_s");
                } else if (grid[1, 0] == 2) {
                    return GetTile("forest_n");
                } else if (grid[0, 1] == 2) {
                    return GetTile("forest_e");
                } else if (grid[2, 1] == 2) {
                    return GetTile("forest_w");
                }
            }
            return GetTile(tiles[(int)mapData[x, y]]);
        } catch (Exception ex) {
            Debug.Log("ChooseTile exception: " + ex.Message);
            return null;
        }
    }

    Dictionary<string, Tile> _mapTiles = new Dictionary<string, Tile>();

    Tile GetTile(string tilename) {
        try {
            if (!_mapTiles.ContainsKey(tilename)) {
                Debug.Log("loading " + tilename);
                _mapTiles.Add(tilename, Resources.Load<Tile>(tilename));
            }
            return _mapTiles[tilename];
        } catch (Exception ex) {
            Debug.Log("GetTile exception: " + ex.Message);
            return null;
        }
    }

    int?[,] TileGrid(int x, int y) {
        try {
            int?[,] grid = new int?[3, 3];
            bool maxX = x == mapData.GetLength(0) - 1;
            bool minX = x == 0;
            bool maxY = y == mapData.GetLength(1) - 1;
            bool minY = y == 0;
            grid[0, 0] = (minX || minY) ? null : mapData[x - 1, y - 1];
            grid[0, 1] = minX ? null : mapData[x - 1, y];
            grid[0, 2] = (minX || maxY) ? null : mapData[x - 1, y + 1];
            grid[1, 0] = minY ? null : mapData[x, y - 1];
            grid[1, 1] = mapData[x, y];
            grid[1, 2] = maxY ? null : mapData[x, y + 1];
            grid[2, 0] = (minY || maxX) ? null : mapData[x + 1, y - 1];
            grid[2, 1] = (maxX) ? null : mapData[x + 1, y];
            grid[2, 2] = (maxX || maxY) ? null : mapData[x + 1, y + 1];
            return grid;
        } catch (Exception ex) {
            Debug.Log("TileGrid exception (" + x + "," + y + ") : " + ex.Message);
            return null;
        }
    }

    public int? GetTileAt(Vector2Int location) {
        Vector2Int playerInArray = new Vector2Int(10, 6);
        Vector2Int valueToCheck = playerInArray + location - lastUpdatePosition;
        int? value = mapData[valueToCheck.x, valueToCheck.y];
        return mapData[valueToCheck.x, valueToCheck.y];
    }

    public void AddPlayerIcon(int id, Vector2Int position) {
        mapIcons.AddPlayer(id, position, GetTilePositionRelativeToWorldPosition(position));
    }

    public void DebugMapToString() {
        bool debug = false;
        if (debug) {
            string output = "";
            for (var row = mapData.GetLength(1) - 1; row >= 0; row--) {
                for (var col = 0; col < mapData.GetLength(0); col++) {
                    output += "[" + TypeToString(mapData[col, row]) + "] ";
                }
                output += "\n";
            }
            Debug.Log(output);
        }
    }

    public string TypeToString(int? type) {
        if (type == null) return " ";
        if (type == 0) return "P";
        if (type == 1) return "M";
        if (type == 2) return "F";
        if (type == 3) return "W";
        return "WHAT!?";
    }
}
