using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    public void StartGame()
    {
        SceneManager.LoadScene("DialogoIntro"); 
    }

    public void ShowCredits()
    {
        SceneManager.LoadScene("CreditosScene");

    }

    public void ExitGame()
    {
        Application.Quit(); //salir del juego
    }

//    public void BackToMenu()
//    {
//        SceneManager.LoadScene("MenuScene");
//    }
}
