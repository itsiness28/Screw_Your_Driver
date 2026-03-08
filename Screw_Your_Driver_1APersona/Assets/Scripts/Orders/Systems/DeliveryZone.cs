using UnityEngine;

public class DeliveryZone : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private OrderManager orderManager;
    private bool playerInside;
    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<InteractSystem>() != null)
        {
            playerInside = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<InteractSystem>() != null)
            playerInside = false;
    }

    public bool IsPlayerInside()
    {
        return playerInside;
    }
    
    public bool TryDeliver(PickUpItems currentHeldItem)
    {
        if (orderManager.activeOrders.Count == 0 )
            return false;
        Debug.Log("No da null TryDeliver");
        OrderInstance currentOrder = orderManager.activeOrders[0];

        CraftedItem furniture = currentHeldItem.GetComponent<CraftedItem>();

        if (furniture == null)
        {
            Debug.Log("Lo que llevas no es un mueble válido.");
            return false;
        }

        if (furniture.recipe == currentOrder.data.recipe)
        {
            Debug.Log("Orden completada correctamente.");

            currentOrder.CompleteOrder();
            Destroy(currentHeldItem.gameObject);
            return true;
        }
        else
        {
            Debug.Log("Este mueble no corresponde a la orden activa.");
            return false;
        }
    }
}

