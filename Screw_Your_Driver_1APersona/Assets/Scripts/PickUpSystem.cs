using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpSystem : MonoBehaviour
{
    [Header("Ajustes")]
    public float pickUpRange = 3f;
    public float moveSpeed = 20f;
    public float holdDistance = 1.5f;
    public KeyCode pickUpKey = KeyCode.E;

    private Rigidbody heldObject;
    private Collider heldCollider;
    private Collider playerCollider;

    private Vector3 targetHoldPosition; // Punto virtual calculado

    void Start()
    {
        playerCollider = GetComponent<Collider>();
    }

    void Update()
    {
        if (Input.GetKeyDown(pickUpKey))
        {
            if (heldObject == null)
                TryPickUp();
            else
                DropObject();
        }
    }

    void FixedUpdate()
    {
        if (heldObject != null)
        {
            // Calculamos el punto delante de la c�mara EN FIXEDUPDATE
            targetHoldPosition = Camera.main.transform.position + Camera.main.transform.forward * holdDistance;

            MoveObjectSmooth();
        }
    }

    void TryPickUp()
    {
        Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, pickUpRange))
        {
            if (hit.collider.CompareTag("PickUp"))
            {
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
