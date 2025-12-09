using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CogerObjetos : MonoBehaviour
{
    [Header("Objetos a recoger")]
    [Tooltip("Arrastra aquí los objetos que deben recogerse en este nivel")]
    public List<GameObject> objetos; // Lista de objetos que el jugador debe recoger

    [Header("UI")]
    public TextMeshProUGUI contadorObjetos; // Texto que muestra cuántos faltan
    public GameObject incompletoUI;         // Se muestra si faltan objetos
    public GameObject completoUI;           // Se muestra si ya recogió todos

    private int totalObjetos;

    void Start()
    {
        totalObjetos = objetos.Count;

        if (incompletoUI != null) 
            incompletoUI.SetActive(true);

        if (completoUI != null) 
            completoUI.SetActive(false);

        ActualizarUI();
    }

    void Update()
    {
        // Contar cuántos objetos de la lista siguen existiendo en la escena
        int restantes = 0;
        foreach (var obj in objetos)
        {
            if (obj != null) restantes++;
        }

        // Actualizar contador
        if (contadorObjetos != null)
            contadorObjetos.text = restantes.ToString();

        // Mostrar paneles según estado
        if (restantes == 0)
        {
            if (completoUI != null) completoUI.SetActive(true);
            if (incompletoUI != null) incompletoUI.SetActive(false);
        }
        else
        {
            if (incompletoUI != null) incompletoUI.SetActive(true);
            if (completoUI != null) completoUI.SetActive(false);
        }
    }

    private void ActualizarUI()
    {
        int restantes = objetos.Count;
        if (contadorObjetos != null)
            contadorObjetos.text = restantes.ToString();
    }
}
