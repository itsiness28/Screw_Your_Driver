using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    public Inventory inventory;

    public Text slot1Text;
    public Text slot2Text;

    void Start()
    {
        inventory.inventoryChanged += UpdateUI;
        UpdateUI();
    }

    void UpdateUI()
    {
        slot1Text.text = FormatSlot(inventory.slot1Type, inventory.slot1Amount);
        slot2Text.text = FormatSlot(inventory.slot2Type, inventory.slot2Amount);
    }

    string FormatSlot(ItemType type, int amount)
    {
        if (type == ItemType.None)
            return "[Vacío]";

        if (type == ItemType.Nails)
            return "Clavos: " + amount;

        return type.ToString();
    }
}