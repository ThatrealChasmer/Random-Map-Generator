using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour {

    private List<Vector2> ph;
    private int mapSize;
    private List<GameObject> rooms;
    private GameObject map;
    
    // Use this for initialization
	void Start () {
        this.map = GetComponent<MapData>().GetMap();
        this.mapSize = GetComponent<MapData>().GetMapSize();
        this.rooms = GetComponent<MapData>().GetRooms();
        this.ph = GeneratePlaceholders(mapSize);

        InstantiateRooms(rooms, ph);

	}

    // We use this function to generate general layout of our map without thinking about doors
    private List<Vector2> GeneratePlaceholders(int mapSize)
    {
        List<Vector2> placeholders = new List<Vector2>(); // This is where we will store our map info. I chose List over Array because it's dynamic and we have our map generate random count of rooms
        placeholders.Add(new Vector2(0, 0)); // We add first room from which our map will start
        List<int> freeRooms = new List<int>(); // This is list of rooms that we can use as hooks (later).
        freeRooms.Add(0); // As earlier, we add first entry to start
        int hook; // hook is basically the room chosen randomly from existing placeholders to place new room somewhere around it
        Vector2 newSpot; // Variable to store new room's coords before it gets added to list

        // Simple loop to add new room to placeholders, nothing interesting there
        for(int i = 1; i < mapSize; i++)
        {
            hook = freeRooms[Random.Range(0, freeRooms.Count - 1)];
            newSpot = ChooseFree(placeholders, hook);
            placeholders.Add(newSpot);
            freeRooms.Add(placeholders.Count - 1);
            freeRooms = DeleteTakenRooms(freeRooms, placeholders);
        }
        return placeholders;
    }

    // This is where the fun begins... We use this function to randomly choose coords for new room around existing room
    private Vector2 ChooseFree(List<Vector2> list, int hook)
    {
        Vector2 freeSpot;
        List<Vector2> freeSpots = GetFreeSpots(list[hook], list);
        for(int h = 0; h < freeSpots.Count; h++)
        {
            Debug.Log(h + " = " + freeSpots[h]);
        }

        freeSpot = freeSpots[Random.Range(0, freeSpots.Count - 1)];
        
        return freeSpot;
    }

    // This function on the other hand is to delete rooms from freeRooms list so we cannot randomly choose a room that already has all four sides occupied
    private List<int> DeleteTakenRooms(List<int> freeList, List<Vector2> list)
    {
        Vector2 spot;
        List<Vector2> freeSpots;
        for(int k = 0; k < freeList.Count; k++)
        {
            spot = list[freeList[k]];
            freeSpots = GetFreeSpots(spot, list);
            if(freeSpots.Count == 0)
            {
                freeList.RemoveAt(k);
            }
        }
        return freeList;
    }

    // Notice that both previous functions use this one to look for free spots around room
    private List<Vector2> GetFreeSpots(Vector2 spot, List<Vector2> list)
    {
        List<Vector2> freeSpots = new List<Vector2>();
        bool free = false;
        Vector2 tmpSpot;
        for (int i = 0; i < 4; i++)
        {
            tmpSpot = spot;
            // There we have switch statement that changes coords so we find new spot on one side of room, 0 is top, 1 is right, 2 is bottom and 3 is left
            switch (i)
            {
                case 0:
                    tmpSpot.y++;
                    break;
                case 1:
                    tmpSpot.x++;
                    break;
                case 2:
                    tmpSpot.y--;
                    break;
                case 3:
                    tmpSpot.x--;
                    break;
            }
            
            for (int j = 0; j < list.Count; j++)
            {
                if (list[j] == tmpSpot)
                {
                    free = false;
                    break;
                }
                else
                {
                    free = true;
                }
            }
            if (free == true)
            {
                freeSpots.Add(tmpSpot);
            }
        }
        return freeSpots;
    }

    //And finally there we have instantiating our simple map with proper doors so we cannot go through a door and hit the wall or go ouside the map
    private void InstantiateRooms(List<GameObject> rooms, List<Vector2> placeholders)
    {
        bool top = false;
        bool bottom = false;
        bool left = false;
        bool right = false;
        for(int i = 0; i < placeholders.Count; i++)
        {
            for(int k = 0; k < placeholders.Count; k++)
            {
                if (placeholders[i].x == placeholders[k].x && placeholders[i].y + 1 == placeholders[k].y) top = true;
                if (placeholders[i].x + 1 == placeholders[k].x && placeholders[i].y == placeholders[k].y) right = true;
                if (placeholders[i].x == placeholders[k].x && placeholders[i].y - 1 == placeholders[k].y) bottom = true;
                if (placeholders[i].x - 1 == placeholders[k].x && placeholders[i].y == placeholders[k].y) left = true;
            }
            for (int j = 0; j < rooms.Count; j++)
            {
                    if(rooms[j].GetComponent<RoomData>().top == top && rooms[j].GetComponent<RoomData>().bottom == bottom && rooms[j].GetComponent<RoomData>().left == left && rooms[j].GetComponent<RoomData>().right == right)
                {
                    Instantiate(rooms[j], new Vector3(placeholders[i].x, placeholders[i].y), Quaternion.identity, map.transform);
                }
            }
        }
    }
}
