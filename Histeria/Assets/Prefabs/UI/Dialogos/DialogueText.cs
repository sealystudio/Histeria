using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;

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

    void Start()
    {
        LoadDialogue("dialogue_tutorial"); // nombre del archivo JSON sin extensión
        StartDialogue();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            string fullLine = dialogueData.lines[index].showSpeaker
                              ? $"{dialogueData.lines[index].speaker}: {dialogueData.lines[index].text}"
                              : dialogueData.lines[index].text;
            if (dialogueText.text == fullLine)
            {
                NextLine();
            }
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
        {
            parts.Add(text);
        }

        return parts;
    }

    void LoadDialogue(string fileName)
    {
        TextAsset jsonFile = Resources.Load<TextAsset>(fileName);
        dialogueData = JsonUtility.FromJson<DialogueData>(jsonFile.text);

        List<DialogueLine> processedLines = new List<DialogueLine>();

        foreach (var line in dialogueData.lines)
        {
            // Dividir la línea si es demasiado larga
            List<string> splitParts = SplitLongLine(line.text);

            for (int i = 0; i < splitParts.Count; i++)
            {
                processedLines.Add(new DialogueLine
                {
                    speaker = line.speaker,
                    text = splitParts[i],
                    showSpeaker = (i == 0) // solo la primera parte muestra el nombre
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
        // Limpiar texto anterior
        dialogueText.text = string.Empty;

        // Construir la línea completa con o sin speaker
        string fullLine = dialogueData.lines[index].showSpeaker
            ? $"{dialogueData.lines[index].speaker}: {dialogueData.lines[index].text}"
            : dialogueData.lines[index].text;

        // --- Mostrar retrato del personaje ---
        // Si el JSON tiene un campo portrait, úsalo. Si no, usa el speaker como nombre de archivo.
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
            CharacterPortrait.enabled = false; // ocultar si no hay imagen
        }

        // --- Efecto de escritura letra por letra ---
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
            dialogueText.text = string.Empty;
            StartCoroutine(WriteLine());
        }
        else
        {
            gameObject.SetActive(false);
            if (CharacterPortrait != null)
            {
                CharacterPortrait.enabled = false;
            }
        }
    }
}
