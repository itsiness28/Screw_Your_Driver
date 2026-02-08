using UnityEngine;

public class Inventory : MonoBehaviour
{
    public ItemType slot1Type = ItemType.None;
    public int slot1Amount = 0;

    public ItemType slot2Type = ItemType.None;
    public int slot2Amount = 0;

    public int maxNails = 8;

    public delegate void OnInventoryChanged();
    public event OnInventoryChanged inventoryChanged;

    public bool AddItem(Item item)
    {
        // --- CLAVOS (acumulables) ---
        if (item.type == ItemType.Nails)
        {
            // Intentar meter en slot 1
            if (slot1Type == ItemType.Nails && slot1Amount < maxNails)
            {
                int space = maxNails - slot1Amount;
                int toAdd = Mathf.Min(space, item.amount);
                slot1Amount += toAdd;
                item.amount -= toAdd;

                if (item.amount <= 0)
                {
                    Destroy(item.gameObject);
                    inventoryChanged?.Invoke();
                    return true;
                }
            }

            // Intentar meter en slot 2
            if (slot2Type == ItemType.Nails && slot2Amount < maxNails)
            {
                int space = maxNails - slot2Amount;
                int toAdd = Mathf.Min(space, item.amount);
                slot2Amount += toAdd;
                item.amount -= toAdd;

                if (item.amount <= 0)
                {
                    Destroy(item.gameObject);
                    inventoryChanged?.Invoke();
                    return true;
                }
            }

            // Si no hay clavos en ningún slot, meter en slot vacío
            if (slot1Type == ItemType.None)
            {
                slot1Type = ItemType.Nails;
                slot1Amount = Mathf.Min(item.amount, maxNails);
                item.amount -= slot1Amount;

                if (item.amount <= 0)
                {
                    Destroy(item.gameObject);
                    inventoryChanged?.Invoke();
                    return true;
                }
            }

            if (slot2Type == ItemType.None)
            {
                slot2Type = ItemType.Nails;
                slot2Amount = Mathf.Min(item.amount, maxNails);
                item.amount -= slot2Amount;

                if (item.amount <= 0)
                {
                    Destroy(item.gameObject);
                    inventoryChanged?.Invoke();
                    return true;
                }
            }

            return false;
        }

        // --- HERRAMIENTAS (solo 1 por slot) ---
        if (item.type == ItemType.ToolA || item.type == ItemType.ToolB)
        {
            // Slot 1 vacío
            if (slot1Type == ItemType.None)
            {
                slot1Type = item.type;
                slot1Amount = 1;
                Destroy(item.gameObject);
                inventoryChanged?.Invoke();
                return true;
            }

            // Slot 2 vacío
            if (slot2Type == ItemType.None)
            {
                slot2Type = item.type;
                slot2Amount = 1;
                Destroy(item.gameObject);
                inventoryChanged?.Invoke();
                return true;
            }

            // Ambos slots ocupados → soltar herramienta del slot 1
            DropItem(slot1Type);
            slot1Type = item.type;
            slot1Amount = 1;
            Destroy(item.gameObject);
            inventoryChanged?.Invoke();
            return true;
        }

        return false;
    }

    public void DropItem(ItemType type)
    {
        // Aquí se instancia el prefab de la herramienta en el suelo
        // Instantiate(prefabToolA, transform.position + transform.forward, Quaternion.identity);

        if (slot1Type == type)
        {
            slot1Type = ItemType.None;
            slot1Amount = 0;
        }
        else if (slot2Type == type)
        {
            slot2Type = ItemType.None;
            slot2Amount = 0;
        }

        inventoryChanged?.Invoke();
    }
}