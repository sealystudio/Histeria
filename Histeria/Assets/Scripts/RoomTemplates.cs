using System.Collections.Generic;
using UnityEngine;

public class RoomTemplates : MonoBehaviour
{

    public static RoomTemplates instance;
    public GameObject[] topRooms;
    public GameObject[] bottomRooms;
    public GameObject[] leftRooms;
    public GameObject[] rightRooms;
    public List<Vector3> roomPositions;


    void Awake()
    {
        // Configura el Singleton
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        roomPositions = new List<Vector3>();

        roomPositions.Add(Vector3.zero);
    }
}