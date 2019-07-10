using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapTile : MonoBehaviour
{
  [SerializeField] Color color;
  [SerializeField] bool passable;

  Vector3Int _position;
  public void setPosition(Vector3Int position) { this._position = position; }
  public Vector3Int getPosition() { return this._position; }

  public Color GetColor() { return this.color; }

  // Start is called before the first frame update
  void Start()
  {

  }

  // Update is called once per frame
  void Update()
  {

  }
}
