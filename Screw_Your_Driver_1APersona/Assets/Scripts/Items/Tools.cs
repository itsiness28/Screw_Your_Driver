using UnityEngine;


public class Tools : MonoBehaviour, IInteractable
{
    [SerializeField] private Item item;
    public void Start()
    {
        item = GetComponent<Item>();
    }
    public void Interact(GameObject interactor)
    {
        Debug.Log("Entro Interact tool");
        Inventory inv = interactor.GetComponent<Inventory>();
        bool added = inv.AddItem(item);

        if (added)
        {
            Destroy(item.gameObject);
        }
        else
        {
            Debug.Log("Inventario lleno o no se puede añadir este objeto.");
        }

        return;
    }
}
