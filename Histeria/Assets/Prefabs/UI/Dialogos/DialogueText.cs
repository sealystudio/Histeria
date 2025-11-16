using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.Rendering.Universal;

[System.Serializable]
public class DialogueLine
{
    public string speaker;
    public string text;
    public bool showSpeaker = true;
    public string portrait;
}

[System.Serializable]
public class DialogueData
{
    public string scene;
    public List<DialogueLine> lines;
}

public class DialogueText : MonoBehaviour
{
    public TextMeshProUGUI dialogueText;
    public float textSpeed = 0.1f;
    private int index;
    private DialogueData dialogueData;
    public UnityEngine.UI.Image CharacterPortrait;
    
    [Header("Opciones")]
    public bool autoStart = false;
    public string nombreJSON = "";

    [Header("HUD")]
    public Canvas hudCanvas;

    private bool abrirDialogo;
    private PlayerMovement playerMovement;
    public Light2D luzAmbiente;



    public void InitDialogue(string jsonName)
    {
        nombreJSON = jsonName;

        if (!gameObject.activeSelf)
            gameObject.SetActive(true); 

        if (dialogueText == null)
        {
            Debug.LogError("TMP text no asignado en DialogueText!");
            return;
        }

        playerMovement = GameObject.FindGameObjectWithTag("Player")?.GetComponent<PlayerMovement>();
        if (playerMovement != null)
        {
            playerMovement.canMove = false;
            playerMovement.puedeDisparar = false;
        }

        abrirDialogo = true;
        gameObject.SetActive(true);

        LoadDialogue(nombreJSON);
        StartDialogue();
    }




    void Start()
    {
        if (!autoStart) return;

        if (string.IsNullOrEmpty(nombreJSON))
            return;

        luzAmbiente.intensity = 0;
        abrirDialogo = true;

        playerMovement = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>();
        if (playerMovement != null)
        {
            playerMovement.canMove = false;
            playerMovement.puedeDisparar = false;
        }

        LoadDialogue(nombreJSON);
        StartDialogue();
    }



    void Update()
    {
        Cursor.visible = abrirDialogo;

        if (!abrirDialogo || dialogueData == null) return;

        if (Input.GetMouseButtonDown(0))
        {
            string fullLine = dialogueData.lines[index].showSpeaker
                ? $"{dialogueData.lines[index].speaker.ToUpper()}: {dialogueData.lines[index].text}"
                : dialogueData.lines[index].text;

            if (dialogueText.text == fullLine)
                NextLine();
            else
            {
                StopAllCoroutines();
                dialogueText.text = fullLine;
            }
        }
    }



    List<string> SplitLongLine(string text, int maxLength = 200)
    {
        List<string> parts = new List<string>();

        while (text.Length > maxLength)
        {
            int splitIndex = text.LastIndexOf(' ', maxLength);
            if (splitIndex == -1) splitIndex = maxLength;

            parts.Add(text.Substring(0, splitIndex).Trim());
            text = text.Substring(splitIndex).Trim();
        }

        if (!string.IsNullOrEmpty(text))
            parts.Add(text);

        return parts;
    }



    void LoadDialogue(string fileName)
    {
        TextAsset jsonFile = Resources.Load<TextAsset>(fileName);

        if (jsonFile == null)
        {
            Debug.LogError($"? JSON NO ENCONTRADO: Resources/{fileName}.json");
            return;
        }

        dialogueData = JsonUtility.FromJson<DialogueData>(jsonFile.text);

        List<DialogueLine> processedLines = new List<DialogueLine>();

        foreach (var line in dialogueData.lines)
        {
            List<string> splitParts = SplitLongLine(line.text);

            for (int i = 0; i < splitParts.Count; i++)
            {
                processedLines.Add(new DialogueLine
                {
                    speaker = line.speaker,
                    text = splitParts[i],
                    showSpeaker = (i == 0),
                    portrait = line.portrait
                });
            }
        }

        dialogueData.lines = processedLines;
    }



    public void StartDialogue()
    {
        index = 0;
        dialogueText.text = string.Empty;
        StartCoroutine(WriteLine());
    }



    IEnumerator WriteLine()
    {
        dialogueText.text = "";

        string fullLine = dialogueData.lines[index].showSpeaker
            ? $"{dialogueData.lines[index].speaker}: {dialogueData.lines[index].text}"
            : dialogueData.lines[index].text;

        // Retrato
        string portraitName = string.IsNullOrEmpty(dialogueData.lines[index].portrait)
            ? dialogueData.lines[index].speaker.ToLower()
            : dialogueData.lines[index].portrait.ToLower();

        Sprite portrait = Resources.Load<Sprite>($"Characters/{portraitName}");
        if (portrait != null)
        {
            CharacterPortrait.sprite = portrait;
            CharacterPortrait.enabled = true;
        }
        else
        {
            CharacterPortrait.enabled = false;
        }

        foreach (char letter in fullLine.ToCharArray())
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(textSpeed);
        }
    }



    public void NextLine()
    {
        if (index < dialogueData.lines.Count - 1)
        {
            index++;
            dialogueText.text = "";
            StartCoroutine(WriteLine());
        }
        else
        {
            abrirDialogo = false;
            luzAmbiente.intensity = 0.35f;

            gameObject.SetActive(false);


            if (playerMovement != null)
            {
                playerMovement.canMove = true;
                Time.timeScale = 1f;

            }


            if (CharacterPortrait != null)
                CharacterPortrait.enabled = false;
        }
    }
}
