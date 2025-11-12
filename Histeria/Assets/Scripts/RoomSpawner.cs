using UnityEngine;

public class RoomSpawner : MonoBehaviour
{
    public int openSide;

    private RoomTemplates templates;
    private bool spawned = false;


    void Start()
    {
        templates = RoomTemplates.instance;

        Invoke("SpawnRoom", Random.Range(0.1f, 0.3f));
    }

    void SpawnRoom()
    {
        if (spawned)
        {
            return;
        }

        foreach (Vector3 pos in templates.roomPositions)
        {
            if (Vector3.Distance(transform.position, pos) < 0.5f)
            {
                spawned = true;
                return;
            }
        }

        spawned = true;
        templates.roomPositions.Add(transform.position);

        GameObject roomPrefab = null;

        switch (openSide)
        {
            case 1:
                int randB = Random.Range(0, templates.bottomRooms.Length);
                roomPrefab = templates.bottomRooms[randB];
                break;
            case 2:
                int randT = Random.Range(0, templates.topRooms.Length);
                roomPrefab = templates.topRooms[randT];
                break;
            case 3:
                int randL = Random.Range(0, templates.leftRooms.Length);
                roomPrefab = templates.leftRooms[randL];
                break;
            case 4:
                int randR = Random.Range(0, templates.rightRooms.Length);
                roomPrefab = templates.rightRooms[randR];
                break;
        }

        if (roomPrefab != null)
        {
            Instantiate(roomPrefab, transform.position, Quaternion.identity);
        }
    }
}