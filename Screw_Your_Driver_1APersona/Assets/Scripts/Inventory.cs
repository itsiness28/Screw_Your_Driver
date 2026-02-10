using UnityEngine;

public class Inventory : MonoBehaviour
{
    [Header("Slot 1 (Herramientas)")]
    public ItemType toolSlot = ItemType.None;   // ToolA o ToolB
    public int toolAmount = 0;                  // Siempre 1

    [Header("Slot 2 (Clavos)")]
    public int nailsAmount = 0;                 // 0–8
    public int maxNails = 8;

    public delegate void OnInventoryChanged();
    public event OnInventoryChanged inventoryChanged;

    // Añadir objeto al inventario
    public bool AddItem(Item item)
    {
        // -------------------------------
        // 1) CLAVOS → Slot 2
        // -------------------------------
        if (item.type == ItemType.Nails)
        {
            int space = maxNails - nailsAmount;

            if (space <= 0)
                return false; // Slot lleno

            int toAdd = Mathf.Min(space, item.amount);
            nailsAmount += toAdd;
            item.amount -= toAdd;

            if (item.amount <= 0)
            {
                Destroy(item.gameObject);
            }

            inventoryChanged?.Invoke();
            return true;
        }

        // -------------------------------
        // 2) HERRAMIENTAS → Slot 1
        // -------------------------------
        if (item.type == ItemType.ToolA || item.type == ItemType.ToolB)
        {
            // Si ya hay una herramienta → soltarla
            if (toolSlot != ItemType.None)
            {
                DropTool();
            }

            // Guardar la nueva herramienta
            toolSlot = item.type;
            toolAmount = 1;

            Destroy(item.gameObject);
            inventoryChanged?.Invoke();
            return true;
        }

        // Si no es ni clavos ni herramienta → no se puede guardar
        return false;
    }

    // Soltar herramienta del slot 1
    void DropTool()
    {
        // Aquí debes instanciar el prefab de la herramienta en el suelo
        // Ejemplo:
        // Instantiate(prefabToolA, transform.position + transform.forward, Quaternion.identity);

        toolSlot = ItemType.None;
        toolAmount = 0;
    }
}