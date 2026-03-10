using System;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    [Header("Prefabs de herramientas en el mundo")]
    public GameObject toolAWorldPrefab;
    public GameObject toolBWorldPrefab;

    [Header("Referencia al jugador")]
    public Transform playerTransform;

    [Header("Slot 1 (Herramientas)")]
    public ItemType toolSlot = ItemType.None;   // ToolA o ToolB
    public int toolAmount = 0;                  // Siempre 1

    [Header("Slot 2 (Clavos)")]
    public int nailsAmount = 0;                 // 0–8
    public int maxNails = 8;

    [Header("Herramienta equipada")]
    public ItemType equippedTool = ItemType.None;

    public delegate void OnInventoryChanged();
    public event OnInventoryChanged inventoryChanged;

    // Añadir objeto al inventario
    public bool AddItem(Item item)
    {
        // -------------------------------
        // HERRAMIENTAS → Slot 1
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

            // Equipar automáticamente la herramienta recogida
            equippedTool = item.type;

            Destroy(item.gameObject);
            inventoryChanged?.Invoke();
            Debug.Log("Herramienta cogida");
            return true;
        }

        // Si no es ni clavos ni herramienta → no se puede guardar
        return false;
    }

    // Soltar herramienta del slot 1
    public void DropTool()
    {
        if (toolSlot == ItemType.None)
            return;

        GameObject prefabToDrop = null;

        // Elegir el prefab correcto
        if (toolSlot == ItemType.ToolA)
            prefabToDrop = toolAWorldPrefab;
        else if (toolSlot == ItemType.ToolB)
            prefabToDrop = toolBWorldPrefab;

        if (prefabToDrop != null)
        {
            // Instanciar delante del jugador
            Vector3 dropPos = playerTransform.position + playerTransform.forward * 1f;
            Quaternion dropRot = Quaternion.identity;

            GameObject dropped = GameObject.Instantiate(prefabToDrop, dropPos, dropRot);

            // Asegurar que tiene Item configurado
            Item item = dropped.GetComponent<Item>();
            if (item == null)
                item = dropped.AddComponent<Item>();

            item.type = toolSlot;
            item.amount = 1;
        }

        // Vaciar el slot
        toolSlot = ItemType.None;
        toolAmount = 0;

        // Desequipar
        equippedTool = ItemType.None;

        inventoryChanged?.Invoke();
    }

    public bool AddNails(int amount)
    {
        nailsAmount += amount;
        if (nailsAmount > maxNails)
            nailsAmount = maxNails;

        inventoryChanged?.Invoke();
        return true;
    }

    // ---------------------------------------------------------
    // MÉTODO NECESARIO PARA EL MINIJUEGO
    // ---------------------------------------------------------
    public bool HasTool(ItemType tool)
    {
        return equippedTool == tool;
    }
}