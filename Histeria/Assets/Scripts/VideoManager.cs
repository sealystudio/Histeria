using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using UnityEngine.SceneManagement;
using TMPro;

public class VideoManager : MonoBehaviour
{
    public string nombreDelArchivo;

    [Header("Video Settings")]
    public VideoPlayer videoPlayer;

    [Header("UI Button")]
    public Button continueButton;

    [Header("Blink speed (seconds)")]
    public float blinkSpeed = 0.5f;

    private bool buttonActive = false;
    private TMP_Text buttonText;

    void Start()
    {
        continueButton.gameObject.SetActive(false);
        buttonText = continueButton.GetComponentInChildren<TMP_Text>();

        videoPlayer.playOnAwake = false;
        videoPlayer.source = VideoSource.Url;
        videoPlayer.skipOnDrop = false;

        string rutaCompleta = System.IO.Path.Combine(Application.streamingAssetsPath, nombreDelArchivo);
        videoPlayer.url = rutaCompleta;

        videoPlayer.prepareCompleted += OnVideoPrepared;
        videoPlayer.errorReceived += OnVideoError;
        videoPlayer.loopPointReached += OnVideoEnd;

        videoPlayer.Prepare();
    }

    void Update()
    {
        if (buttonActive && buttonText != null)
        {
            float alpha = Mathf.PingPong(Time.time / blinkSpeed, 1f);
            Color c = buttonText.color;
            c.a = alpha;
            buttonText.color = c;
        }
    }

    void OnVideoPrepared(VideoPlayer vp)
    {
        vp.Play();
    }

    void OnVideoError(VideoPlayer vp, string message)
    {
        Debug.Log("Error cargando video: " + message);
        OnVideoEnd(vp);
    }

    void OnVideoEnd(VideoPlayer vp)
    {
        continueButton.gameObject.SetActive(true);
        buttonActive = true;

        continueButton.onClick.RemoveAllListeners();
        continueButton.onClick.AddListener(OnButtonClicked);
    }

    void OnButtonClicked()
    {
        int currentIndex = SceneManager.GetActiveScene().buildIndex;
        if (currentIndex + 1 < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(currentIndex + 1);
        }
    }
}