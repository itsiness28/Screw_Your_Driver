using UnityEngine;

public class NailsGiver : MonoBehaviour, IInteractable
{
    public int nailsToGive = 3; // cantidad para testear
    public float respawnTime = 5f;
    private bool available = true;

    public void Interact(GameObject interactor)
    {
        Debug.Log("Entro en interact");
        Inventory inv = interactor.GetComponent<Inventory>();
        if (inv != null && available)
        {
            StartCoroutine(RespawnRoutine());
            inv.AddNails(nailsToGive);
            inv.AddItem(new Item { type = ItemType.Nails, amount = nailsToGive });
            Debug.Log("Clavos añadidos: " + nailsToGive);
        }
    }
    private System.Collections.IEnumerator RespawnRoutine()
    {
        available = false;

        // Esperar
        yield return new WaitForSeconds(respawnTime);

        available = true;
    }
    //private void OnTriggerStay(Collider other)
    //{
    //    if (other.CompareTag("Player"))
    //    {
    //        if (Input.GetKeyDown(KeyCode.E))
    //        {
    //            Inventory inv = other.GetComponent<Inventory>();
    //            if (inv != null)
    //            {
    //                inv.AddItem(new Item { type = ItemType.Nails, amount = nailsToGive });
    //                Debug.Log("Clavos añadidos: " + nailsToGive);


    //            }
    //        }
    //    }
    //}

}
