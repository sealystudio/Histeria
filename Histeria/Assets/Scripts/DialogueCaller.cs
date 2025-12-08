using UnityEngine;

public class DialogueCaller : MonoBehaviour
{
    public GameObject dialoguePrefab; // Prefab del Canvas completo
    public bool soloUnaVez = true;

    private bool usado = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;
        if (usado && soloUnaVez) return;

        usado = true;
        if (dialoguePrefab == null)
        {
            Debug.LogError("No se asignó diálogo en " + gameObject.name);
            return;
        }

        // Instanciar prefab
        GameObject dialogueInstance = Instantiate(dialoguePrefab);
        dialogueInstance.SetActive(true); // asegurar que está activo

        // Inicializar diálogo
        DialogueText dt = dialogueInstance.GetComponentInChildren<DialogueText>();
        if (dt != null)
        {
            dt.InitDialogue(dt.nombreJSON);
        }
    }
}
