using UnityEngine;

public class InteractSystem : MonoBehaviour
{
    [Header("Ajustes")]
    public float interactUpRange = 1f;
    public KeyCode pickUpKey = KeyCode.E;
    [SerializeField] private LayerMask interactableLayer;

    [Header("Referencias")]
    public PickUpItems currentHeldItem;
    public DeliveryZone DeliveryZone;

    private Vector3 targetHoldPosition;

    void Update()
    {
        if (Input.GetKeyDown(pickUpKey))
        {
            if (currentHeldItem != null)
            {
                if (DeliveryZone.IsPlayerInside())
                {
                    DeliveryZone.TryDeliver(this);
                    return;
                }
                currentHeldItem.DropObject();
                currentHeldItem = null;
                return;
            }
            TryInteract();
        }
    }

    void TryInteract()
    {
        if (currentHeldItem != null)
        {
            currentHeldItem.DropObject();
            currentHeldItem = null;
            return;
        }

        Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, interactUpRange, interactableLayer))
        {
            IInteractable interactable = hit.collider.GetComponent<IInteractable>();

            if (interactable != null)
            {
                PickUpItems pick = interactable as PickUpItems;

                if (pick != null)
                {
                    currentHeldItem = pick;
                }

                interactable.Interact(gameObject);
            }
        }
    }

    public void ClearHeldItem()
    {
        currentHeldItem = null;
    }
}
