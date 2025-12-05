using UnityEngine;

public class DialogueCaller : MonoBehaviour
{
    public GameObject dialoguePrefab; // Prefab del Canvas completo
    public bool soloUnaVez = true;

    private bool usado = false;

    public bool soloPrimerEnemigo = false;
    public static bool dialogoSombraActivado = false;


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;
        if (usado && soloUnaVez) return;
        if (soloPrimerEnemigo && dialogoSombraActivado) return;

        usado = true;
        if (soloPrimerEnemigo) dialogoSombraActivado = true;
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
