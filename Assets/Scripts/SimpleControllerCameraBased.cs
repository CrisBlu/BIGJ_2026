using UnityEngine;
using UnityEngine.InputSystem;
public class SimpleControllerCameraBased : MonoBehaviour
{
    private float playerSpeed = 5.0f;
    private float gravityValue = -9.81f;

    public CharacterController controller;
    private Vector3 playerVelocity;
    public Camera playerCamera;

    [Header("Input Actions")]
    public InputActionReference moveAction;

    private void OnEnable()
    {
        moveAction.action.Enable();
    }

    private void OnDisable()
    {
        moveAction.action.Disable();
    }

    void Update()
    {
        if (controller.isGrounded && playerVelocity.y < -2f)
            playerVelocity.y = -2f;

        Vector2 input = moveAction.action.ReadValue<Vector2>();
        Vector3 move = new Vector3(input.x, 0, input.y);
        move = Vector3.ClampMagnitude(move, 1f);

        if (move != Vector3.zero)
        {
            float targetAngle = Mathf.Atan2(move.x, move.z) * Mathf.Rad2Deg + playerCamera.transform.eulerAngles.y;
            move = Quaternion.Euler(0, targetAngle, 0) * Vector3.forward;
            transform.forward = move;
        }

        playerVelocity.y += gravityValue * Time.deltaTime;

        controller.Move((move * playerSpeed + Vector3.up * playerVelocity.y) * Time.deltaTime);
    }
}
