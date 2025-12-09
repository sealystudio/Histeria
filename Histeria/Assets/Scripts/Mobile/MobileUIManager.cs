using UnityEngine;

public class MobileUIManager : MonoBehaviour
{
    public GameObject canvasMobile;

    void Start()
    {
#if UNITY_ANDROID || UNITY_IOS
        canvasMobile.SetActive(true);
#else
        canvasMobile.SetActive(false);
#endif
    }
}
