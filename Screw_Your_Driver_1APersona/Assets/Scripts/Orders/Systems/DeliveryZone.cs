using UnityEngine;

public class DeliveryZone : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private OrderManager orderManager;

    private void OnTriggerStay(Collider other)
    {

        Debug.Log("Jugador dentro de la zona");
        // Solo nos interesa el Player
        InteractSystem player = other.GetComponent<InteractSystem>();

        if (player == null)
        {
            Debug.Log("player == null");
            return;
        }
            

        // Si no lleva nada en la mano, no hacemos nada
        if (player.currentHeldItem == null)
        {
            Debug.Log("player.currentHeldItem == null");
            return;
        }

        // Si pulsa la tecla de entregar (puedes cambiarla)
        if (Input.GetKeyDown(KeyCode.F))
        {
            Debug.Log("Pulso F en zona");
            TryDeliver(player);
        }
    }

    private void TryDeliver(InteractSystem player)
    {
        if (orderManager.activeOrders.Count == 0 )
            return;
        Debug.Log("No da null TryDeliver");
        OrderInstance currentOrder = orderManager.activeOrders[0];

        // Cogemos el objeto físico que está sosteniendo
        GameObject heldObject = player.currentHeldItem.gameObject;

        CraftedItem furniture = heldObject.GetComponent<CraftedItem>();

        if (furniture == null)
        {
            Debug.Log("Lo que llevas no es un mueble válido.");
            return;
        }

        // Comparación lógica por RecipeData
        if (furniture.recipe == currentOrder.data.recipe)
        {
            Debug.Log("Orden completada correctamente.");

            currentOrder.CompleteOrder();
            Destroy(heldObject);

            player.ClearHeldItem(); // limpiamos referencia en el player
        }
        else
        {
            Debug.Log("Este mueble no corresponde a la orden activa.");
        }
    }
}

