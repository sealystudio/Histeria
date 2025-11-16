using UnityEngine;

public class ShowLevelStartCanvas : MonoBehaviour
{
    public GameObject levelStartCanvas;
    public GameObject hudCanvas;
    public GameObject inventoryCanvas;
    public GameObject textCanvas;

    private void Start()
    {
        if (levelStartCanvas != null)
        {
            levelStartCanvas.SetActive(true);

            Cursor.lockState = CursorLockMode.None; 
            Cursor.visible = true;                 

            hudCanvas.SetActive(false);
            inventoryCanvas.SetActive(false);
            textCanvas.SetActive(false);
            Time.timeScale = 0f; 
        }
    }

    public void QuitGame()
    {
        Time.timeScale = 1f;
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
