using UnityEngine;

public class MobileUIManager : MonoBehaviour
{
    public GameObject mobileControls;

    void Start()
    {
#if UNITY_ANDROID || UNITY_IOS
        mobileControls.SetActive(true);
#else
        mobileControls.SetActive(false);
#endif
    }
}
