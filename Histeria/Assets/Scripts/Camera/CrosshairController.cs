using UnityEngine;
using static UnityEngine.InputSystem.UI.VirtualMouseInput;

public class CrosshairController : MonoBehaviour
{
    [Header("Ajustes")]
    public float distanceLimit = 5f; 
    public Transform player;

    [Header("Vectores")]
    private Vector3 mousePos;
    public Vector3 dir;



    void Start()
    {
        Cursor.visible = false;

    }


    void Update()
    {
       
        if (player == null) return;

        mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0f;

        dir = mousePos - player.position;
        dir.z = 0f;

        if (dir.magnitude > distanceLimit) 
        {
            dir = dir.normalized * distanceLimit;
        }
            

        transform.position = player.position + dir;

       

    }
}
