using UnityEngine;

public class PickUpSystem : MonoBehaviour
{
    [Header("Ajustes")]
    public float pickUpRange = 3f;
    public float moveSpeed = 20f;
    public float holdDistance = 1.5f;
    public KeyCode pickUpKey = KeyCode.E;

    [Header("Referencias")]
    public Inventory inventory; // Inventario del jugador

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
            // Si ya estamos sosteniendo un objeto → soltarlo
            if (heldObject != null)
            {
                DropObject();
                return;
            }

            // Si no sostenemos nada → intentar recoger
            TryPickUp();
        }
    }

    void FixedUpdate()
    {
        if (heldObject != null)
        {
            // Punto delante de la cámara calculado en física
            targetHoldPosition = Camera.main.transform.position +
                                 Camera.main.transform.forward * holdDistance;

            MoveObjectSmooth();
        }
    }

    void TryPickUp()
    {
        Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, pickUpRange))
        {
            if (!hit.collider.CompareTag("PickUp"))
                return;

            Item item = hit.collider.GetComponent<Item>();

            if (item == null)
                return;

            // -------------------------------
            // 1) OBJETOS DE INVENTARIO
            // -------------------------------
            if (item.type == ItemType.ToolA ||
                item.type == ItemType.ToolB ||
                item.type == ItemType.Nails)
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

            // -------------------------------
            // 2) OBJETOS SOSTENIBLES EN EL AIRE
            // -------------------------------
            Rigidbody rb = hit.collider.GetComponent<Rigidbody>();

            if (rb != null)
            {
                heldObject = rb;
                heldCollider = rb.GetComponent<Collider>();

                // Ignorar colisiones con el jugador
                if (playerCollider != null && heldCollider != null)
                    Physics.IgnoreCollision(playerCollider, heldCollider, true);

                // Ajustes de estabilidad
                heldObject.useGravity = false;
                heldObject.linearDamping = 10f;
                heldObject.angularDamping = 10f;
                heldObject.constraints = RigidbodyConstraints.FreezeRotation;
            }
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