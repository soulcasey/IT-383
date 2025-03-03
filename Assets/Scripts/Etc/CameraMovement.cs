using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [Header("Mouse Settings")]
    public float mouseSensitivity = 300f;
    private float xRotation = 0f;

    [Header("Movement Settings")]
    public float moveSpeed = 5f;

    private void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        HandleMouseLook();
        HandleMovement();
    }

    private void HandleMouseLook()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);         // Vertical rotation
        transform.parent.Rotate(Vector3.up * mouseX);                         // Horizontal rotation (assumes camera is child of player)
    }

    private void HandleMovement()
    {
        float x = Input.GetAxis("Horizontal"); // A/D
        float z = Input.GetAxis("Vertical");   // W/S

        Vector3 move = transform.parent.right * x + transform.parent.forward * z;
        transform.parent.position += move * moveSpeed * Time.deltaTime;
    }
}
