using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("Movimento")]
    public float walkSpeed = 5f;
    public float runSpeed = 9f;
    public float jumpHeight = 1.5f;
    public float gravity = -20f;

    [Header("Mouse Look")]
    public Transform cameraTransform;
    public float mouseSensitivity = 0.15f;
    public float maxLookAngle = 80f;

    private CharacterController controller;

    private Vector3 velocity;
    private float cameraRotationX = 0f;

    void Start()
    {
        controller = GetComponent<CharacterController>();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        Move();
        Look();
    }

    void Move()
    {
        Vector2 input = Vector2.zero;

        if (Keyboard.current != null)
        {
            input = new Vector2(
                Keyboard.current.aKey.isPressed ? -1 : Keyboard.current.dKey.isPressed ? 1 : 0,
                Keyboard.current.sKey.isPressed ? -1 : Keyboard.current.wKey.isPressed ? 1 : 0
            );
        }

        input = Vector2.ClampMagnitude(input, 1f);

        Vector3 move = transform.right * input.x +
                       transform.forward * input.y;

        float speed = walkSpeed;

        // Correr com Shift
        if (Keyboard.current != null &&
            (Keyboard.current.leftShiftKey.isPressed ||
             Keyboard.current.rightShiftKey.isPressed))
        {
            speed = runSpeed;
        }

        controller.Move(move * speed * Time.deltaTime);

        // Está no chão
        if (controller.isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        // Pular
        if (Keyboard.current != null &&
            Keyboard.current.spaceKey.wasPressedThisFrame &&
            controller.isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        // Gravidade
        velocity.y += gravity * Time.deltaTime;

        controller.Move(velocity * Time.deltaTime);
    }

    void Look()
    {
        if (Mouse.current == null)
            return;

        Vector2 mouseDelta = Mouse.current.delta.ReadValue();

        float mouseX = mouseDelta.x * mouseSensitivity;
        float mouseY = mouseDelta.y * mouseSensitivity;

        // Girar o Player para esquerda/direita
        transform.Rotate(Vector3.up * mouseX);

        // Girar a câmera para cima/baixo
        cameraRotationX -= mouseY;
        cameraRotationX = Mathf.Clamp(
            cameraRotationX,
            -maxLookAngle,
            maxLookAngle
        );

        cameraTransform.localRotation =
            Quaternion.Euler(cameraRotationX, 0f, 0f);
    }
}