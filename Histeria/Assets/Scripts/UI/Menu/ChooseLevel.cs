using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ChooseLevel : MonoBehaviour
{
    [Header("Sprites")]
    public Sprite candadoSprite;
    public Sprite tutorialSprite;
    public Sprite nivel1Sprite;
    public Sprite nivel2Sprite;
    public Sprite nivel3Sprite;
    public Sprite finalSprite;

    [Header("Botones + Imágenes")]
    public Button tutorialButton;
    public Image tutorialImg;

    public Button level1Button;
    public Image level1Img;

    public Button level2Button;
    public Image level2Img;

    public Button level3Button;
    public Image level3Img;

    public Button finalButton;
    public Image finalImg;

    private void Start()
    {
        ActualizarBloqueos();
    }

    private void ActualizarBloqueos()
    {
        // TUTORIAL – SIEMPRE DISPONIBLE
        tutorialButton.interactable = true;
        tutorialImg.sprite = tutorialSprite;

        // LEVEL 1
        bool lvl1 = PlayerPrefs.GetInt("Nivel1_Desbloqueado", 0) == 1;
        level1Button.interactable = lvl1;
        level1Img.sprite = lvl1 ? nivel1Sprite : candadoSprite;

        // LEVEL 2
        bool lvl2 = PlayerPrefs.GetInt("Nivel2_Desbloqueado", 0) == 1;
        level2Button.interactable = lvl2;
        level2Img.sprite = lvl2 ? nivel2Sprite : candadoSprite;

        // LEVEL 3
        bool lvl3 = PlayerPrefs.GetInt("Nivel3_Desbloqueado", 0) == 1;
        level3Button.interactable = lvl3;
        level3Img.sprite = lvl3 ? nivel3Sprite : candadoSprite;

        // FINAL LEVEL
        bool final = PlayerPrefs.GetInt("NivelFinal_Desbloqueado", 0) == 1;
        finalButton.interactable = final;
        finalImg.sprite = final ? finalSprite : candadoSprite;
    }

    public void Tutorial() => SceneManager.LoadScene("TutorialScene");
    public void LevelOne() { if (level1Button.interactable) SceneManager.LoadScene("SampleScene"); }
    public void LevelTwo() { if (level2Button.interactable) SceneManager.LoadScene("Nivel2"); }
    public void LevelThree() { if (level3Button.interactable) SceneManager.LoadScene("Nivel3"); }
    public void FinalLevel() { if (finalButton.interactable) SceneManager.LoadScene("NivelFinal"); }
}
