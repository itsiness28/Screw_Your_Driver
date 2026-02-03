using UnityEngine;

public class Player_Locomotion : MonoBehaviour
{

    InputManager inputManager; 
    Vector3 moveDirectionP1;
    Vector3 moveDirectionP2;
    Transform cameraObject;
    Rigidbody player1Rigidbody;
    Rigidbody player2Rigidbody;

    public float movementSpeed = 7;

    public float rotationSpeed = 15;

    private void Awake()
    {
        inputManager = GetComponent<InputManager>();
        player1Rigidbody = GetComponent<Rigidbody>();
        player2Rigidbody = GetComponent<Rigidbody>();
        cameraObject = Camera.main.transform;
    }


    public void HandleAllMovementP1() //Estos métodos agrupan toda la info de movimiento (linear y rotación) de cada jugador, el de abajo también, para compartirlo 
    {                                  //al player manager. Es algo redundante pero así lo hace el tuto y luego hace que otros scripts queden más clean así que bueno.
        HandleMovementP1();
        HandleRotationP1();
    }
    public void HandleAllMovementP2()
    {
        HandleMovementP2();
        HandleRotationP2();
    }


    private void HandleMovementP1()
    {
        moveDirectionP1 = cameraObject.forward * inputManager.verticalInputP1;
        moveDirectionP1 = moveDirectionP1 + cameraObject.right * inputManager.horizontalInputP1;
        moveDirectionP1.Normalize();
        moveDirectionP1.y = 0;
        moveDirectionP1 = moveDirectionP1 * movementSpeed;

        Vector3 movementVelocityP1 = moveDirectionP1;
        player1Rigidbody.linearVelocity = movementVelocityP1;
    }

    private void HandleRotationP1()
    {
        Vector3 targetDirectionP1 = Vector3.zero;

        targetDirectionP1 = cameraObject.forward * inputManager.verticalInputP1;
        targetDirectionP1 = targetDirectionP1 + cameraObject.right * inputManager.horizontalInputP1;
        targetDirectionP1.Normalize();
        targetDirectionP1.y = 0;

        if (targetDirectionP1 == Vector3.zero)
            targetDirectionP1 = transform.forward;

        Quaternion targetRotationP1 = Quaternion.LookRotation(targetDirectionP1);
        Quaternion playerRotationP1 = Quaternion.Slerp(transform.rotation, targetRotationP1, rotationSpeed * Time.deltaTime);

        transform.rotation = playerRotationP1; 
    }


    private void HandleMovementP2()
    {
        moveDirectionP2 = cameraObject.forward * inputManager.verticalInputP1;
        moveDirectionP2 = moveDirectionP2 + cameraObject.right * inputManager.horizontalInputP1;
        moveDirectionP2.Normalize();
        moveDirectionP2.y = 0;
        moveDirectionP2 = moveDirectionP2 * movementSpeed;

        Vector3 movementVelocityP2 = moveDirectionP2;
        player2Rigidbody.linearVelocity = movementVelocityP2;
    }
    private void HandleRotationP2()
    {
        Vector3 targetDirectionP2 = Vector3.zero;

        targetDirectionP2 = cameraObject.forward * inputManager.verticalInputP1;
        targetDirectionP2 = targetDirectionP2 + cameraObject.right * inputManager.horizontalInputP1;
        targetDirectionP2.Normalize();
        targetDirectionP2.y = 0;

        if (targetDirectionP2 == Vector3.zero)
            targetDirectionP2 = transform.forward;


        Quaternion targetRotationP2 = Quaternion.LookRotation(targetDirectionP2);
        Quaternion playerRotationP2 = Quaternion.Slerp(transform.rotation, targetRotationP2, rotationSpeed * Time.deltaTime);

        transform.rotation = playerRotationP2;
    }

}
