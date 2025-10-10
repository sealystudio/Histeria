using UnityEngine;

public class CrosshairController : MonoBehaviour
{
    [Header("Ajustes")]
    public float distanceLimit = 5f; 
    public Transform player;

    void Start()
    {
        Cursor.visible = false;
    }


    void Update()
    {
        if (player == null) return;

        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0f;

        Vector3 dir = mousePos - player.position;
        dir.z = 0f;

        if (dir.magnitude > distanceLimit) 
        {
            dir = dir.normalized * distanceLimit;
        }
            

        transform.position = player.position + dir;
    }
}
