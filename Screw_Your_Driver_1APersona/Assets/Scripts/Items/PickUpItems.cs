using UnityEngine;

public class PickUpItems : MonoBehaviour, IInteractable
{
    [SerializeField] private Rigidbody rb;
    public Rigidbody heldObject;
    private Collider heldCollider;
    private Collider playerCollider;
    private Vector3 targetHoldPosition;
    public float holdDistance = 1.5f;
    public float moveSpeed = 20f;

    public void Start()
    {
        rb = GetComponent<Rigidbody>();
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
    public void Interact(GameObject interactor)
    {
        Debug.Log("Entro pickup");
        if (rb == null) return;

        playerCollider = interactor.GetComponent<Collider>();
        heldObject = rb;
        heldCollider = rb.GetComponent<Collider>();

        if (playerCollider != null && heldCollider != null)
            Physics.IgnoreCollision(playerCollider, heldCollider, true);

        heldObject.useGravity = false;
        heldObject.linearDamping = 10f;
        heldObject.angularDamping = 10f;
        heldObject.constraints = RigidbodyConstraints.FreezeRotation;
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

    public void DropObject()
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
