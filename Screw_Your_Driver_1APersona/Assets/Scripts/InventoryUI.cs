using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    public Inventory inventory;

    public Text toolSlotText;   // Slot 1 → Herramientas
    public Text nailsSlotText;  // Slot 2 → Clavos

    void Start()
    {
        inventory.inventoryChanged += UpdateUI;
        UpdateUI();
    }

    void UpdateUI()
    {
        // SLOT 1 → HERRAMIENTAS
        if (inventory.toolSlot == ItemType.None)
            toolSlotText.text = "[Herramienta: Vacío]";
        else
            toolSlotText.text = "Herramienta: " + inventory.toolSlot.ToString();

        // SLOT 2 → CLAVOS
        nailsSlotText.text = "Clavos: " + inventory.nailsAmount + " / " + inventory.maxNails;
    }
}