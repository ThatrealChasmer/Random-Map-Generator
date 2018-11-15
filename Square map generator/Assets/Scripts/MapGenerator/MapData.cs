using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapData : MonoBehaviour {
    public int mapSize;
    public List<GameObject> rooms;
    public GameObject map;
	// Use this for initialization
	void Start () {
        mapSize = Random.Range(mapSize - mapSize / 5, mapSize + mapSize / 5);
	}
	
	public int GetMapSize()
    {
        return this.mapSize;
    }

    public List<GameObject> GetRooms()
    {
        return this.rooms;
    }

    public GameObject GetMap()
    {
        return this.map;
    }
}
