using UnityEngine;

public class NailsGiver : MonoBehaviour
{
    public int nailsToGive = 3; // cantidad para testear

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                Inventory inv = other.GetComponent<Inventory>();
                if (inv != null)
                {
                    inv.AddItem(new Item { type = ItemType.Nails, amount = nailsToGive });
                    Debug.Log("Clavos añadidos: " + nailsToGive);

                   
                }
            }
        }
    }
}
