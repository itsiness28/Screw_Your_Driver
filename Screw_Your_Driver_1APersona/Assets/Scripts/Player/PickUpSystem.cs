using UnityEngine;

public class PickUpSystem : MonoBehaviour
{
    [Header("Ajustes")]
    public float pickUpRange = 3f;
    public float moveSpeed = 20f;
    public float holdDistance = 1.5f;
    public KeyCode pickUpKey = KeyCode.E;

    [Header("Referencias")]
    public Inventory inventory;

    private Rigidbody heldObject;
    private Collider heldCollider;
    private Collider playerCollider;

    private Vector3 targetHoldPosition;

    void Start()
    {
        playerCollider = GetComponent<Collider>();
    }

    void Update()
    {
        if (Input.GetKeyDown(pickUpKey))
        {
            if (heldObject != null)
            {
                DropObject();
                return;
            }

            TryPickUp();
        }
    }

    void FixedUpdate()
    {
        if (heldObject != null)
        {
            targetHoldPosition = Camera.main.transform.position +
                                 Camera.main.transform.forward * holdDistance;

            MoveObjectSmooth();
        }
    }

    void TryPickUp()
    {
        Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
        RaycastHit hit;

        if (!Physics.Raycast(ray, out hit, pickUpRange))
            return;

        // 1) CUBO RECARGABLE DE CLAVOS
        NailsRespawnPickup respawnPickup = hit.collider.GetComponent<NailsRespawnPickup>();
        if (respawnPickup != null)
        {
            // Solo hace algo si está disponible; si no, seguimos con el resto
            bool picked = respawnPickup.TryPickup(inventory);
            if (picked)
                return;
        }

        // 2) OBJETOS DE INVENTARIO (ToolA, ToolB, Nails)
        Item item = hit.collider.GetComponent<Item>();
        if (item != null && hit.collider.CompareTag("PickUp"))
        {
            bool added = inventory.AddItem(item);

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

        // 3) OBJETOS SOSTENIBLES (Rigidbody)
        Rigidbody rb = hit.rigidbody; // más fiable que GetComponent en el collider

        if (rb != null)
        {
            heldObject = rb;
            heldCollider = rb.GetComponent<Collider>();

            if (playerCollider != null && heldCollider != null)
                Physics.IgnoreCollision(playerCollider, heldCollider, true);

            heldObject.useGravity = false;
            heldObject.linearDamping = 10f;
            heldObject.angularDamping = 10f;
            heldObject.constraints = RigidbodyConstraints.FreezeRotation;
        }
    }



    void MoveObjectSmooth()
    {
        Vector3 newPos = Vector3.Lerp(
            heldObject.position,
            targetHoldPosition,
            moveSpeed * Time.fixedDeltaTime
        );

        heldObject.MovePosition(newPos);
    }

    void DropObject()
    {
        if (playerCollider != null && heldCollider != null)
            Physics.IgnoreCollision(playerCollider, heldCollider, false);

        heldObject.useGravity = true;
        heldObject.linearDamping = 1f;
        heldObject.angularDamping = 0.05f;
        heldObject.constraints = RigidbodyConstraints.None;

        heldObject = null;
        heldCollider = null;
    }
}
