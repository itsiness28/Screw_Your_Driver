using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("Movimiento")]
    public float speed = 6f;
    public float sprintSpeed = 10f;
    public float gravity = -9.81f;
    

    [Header("Mirar alrededor")]
    public float mouseSensitivity = 120f;
    public Transform cameraTransform;

    private CharacterController controller;
    private Vector3 velocity;
    private float xRotation = 0f;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        Movimiento();
        RotacionCamara();
    }

    void Movimiento()
    {
        // Comprobar si está en el suelo
        if (controller.isGrounded && velocity.y < 0)
            velocity.y = -2f;

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        // Movimiento base
        Vector3 move = transform.right * x + transform.forward * z;

        // Sprint
        float currentSpeed = Input.GetKey(KeyCode.LeftShift) ? sprintSpeed : speed;

        controller.Move(move * currentSpeed * Time.deltaTime);

        // Aplicar gravedad
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    void RotacionCamara()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        // Rotación vertical (cámara)
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -80f, 80f);

        cameraTransform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        // Rotación horizontal (jugador)
        transform.Rotate(Vector3.up * mouseX);
    }
}

