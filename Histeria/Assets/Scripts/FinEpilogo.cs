using UnityEngine;
using UnityEngine.SceneManagement;

public class FinEpilogo : MonoBehaviour
{
    public void CargarEscena(string nombreDeLaEscena)
    {
        SceneManager.LoadScene(nombreDeLaEscena);
    }
}