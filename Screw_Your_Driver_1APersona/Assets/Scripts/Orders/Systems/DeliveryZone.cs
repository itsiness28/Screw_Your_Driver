using UnityEngine;

public class DeliveryZone : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private OrderManager orderManager;
    private InteractSystem interactSystem;
    private bool playerInside;

    public void Start()
    {
        playerInside = false;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<InteractSystem>() != null)
        {
            playerInside = true;
            if(interactSystem == null)
                interactSystem = other.GetComponent<InteractSystem>();
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
    
    public void TryDeliver(InteractSystem player)
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

