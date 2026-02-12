using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    public Inventory inventory;

    [Header("Slot de herramienta")]
    public RawImage toolIcon;
    public Texture toolAIcon;
    public Texture toolBIcon;

    [Header("Slot de clavos")]
    public RawImage nailsIcon;
    public Text nailsCountText;
    public Texture nailsTexture;

    void Start()
    {
        inventory.inventoryChanged += UpdateUI;
        UpdateUI();
    }

    void UpdateUI()
    {
        // SLOT 1 → HERRAMIENTA (solo icono)
        switch (inventory.toolSlot)
        {
            case ItemType.ToolA:
                toolIcon.texture = toolAIcon;
                toolIcon.enabled = true;
                break;

            case ItemType.ToolB:
                toolIcon.texture = toolBIcon;
                toolIcon.enabled = true;
                break;

            default:
                toolIcon.enabled = false; // Ocultar si no hay herramienta
                break;
        }

        // SLOT 2 → CLAVOS (icono + número)
        nailsIcon.texture = nailsTexture;
        nailsCountText.text = inventory.nailsAmount.ToString();
    }
}
