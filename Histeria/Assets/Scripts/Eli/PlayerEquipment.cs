using UnityEngine;

public class PlayerEquipment : MonoBehaviour
{
    private GameObject currentEquip;

    public void Equip(GameObject equipPrefab)
    {
        if (currentEquip != null)
            Destroy(currentEquip);

        currentEquip = Instantiate(equipPrefab, transform);
        currentEquip.transform.localPosition = Vector3.zero; // ajusta según tu modelo
        Debug.Log("Equipado: " + equipPrefab.name);
    }
}
