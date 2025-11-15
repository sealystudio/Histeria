using UnityEngine;

public class ShowLevelStartCanvas : MonoBehaviour
{
    public GameObject levelStartCanvas;

    private void Start()
    {
        if (levelStartCanvas != null)
        {
            levelStartCanvas.SetActive(true);
            Time.timeScale = 0f; // Pausa el juego
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
