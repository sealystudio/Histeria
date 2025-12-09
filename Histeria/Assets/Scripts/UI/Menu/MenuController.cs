using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    public GameObject ajustesPanel; // Panel de ajustes

    private void Start()
    {
        if (ajustesPanel == null)
            return;
    }
    public void StartGame()
    {
        SceneManager.LoadScene("LevelsScene"); 
    }

    public void ShowCredits()
    {
        SceneManager.LoadScene("CreditosScene");

    }

    public void ExitGame()
    {
        Application.Quit(); //salir del juego
    }

    public void BackToMenu()
    {
        SceneManager.LoadScene("MenuScene");
        Inventory.canOpenInventory = true;

    }

    //Ajustes
    public void AbrirAjustes()
    {
        Inventory.canOpenInventory = false;
        ajustesPanel.SetActive(true);
    }

    public void CerrarAjustes()
    {
        Inventory.canOpenInventory = true;
        ajustesPanel.SetActive(false);
    }



}
