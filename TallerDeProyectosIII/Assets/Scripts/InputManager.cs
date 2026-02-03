using UnityEngine;

public class InputManager : MonoBehaviour
{
    Player_Controls playerControls;
    AnimationManager animationManager;
    public Vector2 movementInputP1;
    public Vector2 movementInputP2;
    private float moveAmount; //esta línea no está para P2
    public float verticalInputP1;
    public float horizontalInputP1;
    public float verticalInputP2;
    public float horizontalInputP2;


    private void Awake()
    {
        animationManager = GetComponent<AnimationManager>(); //esta línea no está para P2
    }


    private void OnEnable()
    {
        if (playerControls == null)
        {
            playerControls = new Player_Controls();

            playerControls.PlayerMovement.Movement_Player1.performed += i => movementInputP1 = i.ReadValue<Vector2>();
            playerControls.PlayerMovement.Movement_Player2.performed += i => movementInputP2 = i.ReadValue<Vector2>();
        }

        playerControls.Enable();
    }

    private void OnDisable()
    {
        playerControls.Disable();
    }

    public void HandleAllInputP1()
    {
        HandleMovementInputP1();
    }

    public void HandleAllInputP2()
    {
        HandleMovementInputP2();
    }

    private void HandleMovementInputP1()
    {
        verticalInputP1 = movementInputP1.y;
        horizontalInputP1 = movementInputP1.x;
        moveAmount = Mathf.Clamp01(Mathf.Abs(horizontalInputP1) + Mathf.Abs(verticalInputP1));//esta línea no está para P2
        animationManager.UpdateAnimatorValues(0, moveAmount); //esta línea no está para P2
    }

    private void HandleMovementInputP2() 
    {
        verticalInputP2 = movementInputP2.y;
        horizontalInputP2 = movementInputP2.x;
    }
}
