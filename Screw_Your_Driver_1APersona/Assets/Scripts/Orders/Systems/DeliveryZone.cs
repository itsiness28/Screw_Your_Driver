using UnityEngine;

public class DeliveryZone : MonoBehaviour/*, IInteractable*/
{
    [Header("References")]
    [SerializeField] private OrderManager orderManager;

    private void OnTriggerStay(Collider other)
    {

        Debug.Log("Jugador dentro de la zona");
        InteractSystem player = other.GetComponent<InteractSystem>();

        if (player == null)
        {
            Debug.Log("player == null");
            return;
        }
            

        if (player.currentHeldItem == null)
        {
            Debug.Log("player.currentHeldItem == null");
            return;
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            Debug.Log("Pulso F en zona");
            TryDeliver(player);
        }
    }
    //public void Interact(GameObject interactor)
    //{
    //    InteractSystem interactor = interactor.GetComponent<InteractSystem>();
    //    if (interactor.currentHeldItem == null)
    //    {
    //        Debug.Log("player.currentHeldItem == null");
    //        return;
    //    }

    //    if (Input.GetKeyDown(KeyCode.F))
    //    {
    //        Debug.Log("Pulso F en zona");
    //        TryDeliver(interactor);
    //    }
    //}
    private void TryDeliver(InteractSystem player)
    {
        if (orderManager.activeOrders.Count == 0 )
            return;
        Debug.Log("No da null TryDeliver");
        OrderInstance currentOrder = orderManager.activeOrders[0];

        GameObject heldObject = player.currentHeldItem.gameObject;

        CraftedItem furniture = heldObject.GetComponent<CraftedItem>();

        if (furniture == null)
        {
            Debug.Log("Lo que llevas no es un mueble válido.");
            return;
        }

        if (furniture.recipe == currentOrder.data.recipe)
        {
            Debug.Log("Orden completada correctamente.");

            currentOrder.CompleteOrder();
            Destroy(heldObject);

            player.ClearHeldItem(); 
        }
        else
        {
            Debug.Log("Este mueble no corresponde a la orden activa.");
        }
    }
}

