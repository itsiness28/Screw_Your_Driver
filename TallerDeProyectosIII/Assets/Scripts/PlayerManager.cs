using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    InputManager inputManager;
    Player_Locomotion playerLocomotion; 
    
    private void Awake()
    {
        inputManager = GetComponent<InputManager>();
        playerLocomotion = GetComponent<Player_Locomotion>();
    }


    private void Update()
    {
        inputManager.HandleAllInputP1();
        inputManager.HandleAllInputP2();

    }
    private void FixedUpdate()
    {
        playerLocomotion.HandleAllMovementP1();
        playerLocomotion.HandleAllMovementP2();
    }

    
}
