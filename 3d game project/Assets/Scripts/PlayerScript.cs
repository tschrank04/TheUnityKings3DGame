using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerScript : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float gravity = 0f;

    private CharacterController controller;
    private InputAction moveAction;
    private Vector3 velocity;

    void Awake()
    {
        controller = GetComponent<CharacterController>();

        // Create a 2D composite for WASD and arrow keys
        moveAction = new InputAction("Move", InputActionType.Value);
        moveAction.AddCompositeBinding("2DVector")
            .With("Up", "<Keyboard>/w")
            .With("Down", "<Keyboard>/s")
            .With("Left", "<Keyboard>/a")
            .With("Right", "<Keyboard>/d");
        moveAction.AddCompositeBinding("2DVector")
            .With("Up", "<Keyboard>/upArrow")
            .With("Down", "<Keyboard>/downArrow")
            .With("Left", "<Keyboard>/leftArrow")
            .With("Right", "<Keyboard>/rightArrow");
    }

    void OnEnable()
    {
        moveAction.Enable();
    }

    void OnDisable()
    {
        moveAction.Disable();

        // Dispose created actions to avoid leaks
        moveAction.Dispose();
    }

    void Update()
    {
        Vector2 input = moveAction.ReadValue<Vector2>(); // x = left/right, y = forward/back
        Vector3 inputDir = new Vector3(input.x, 0f, input.y);

        // Normalize input so diagonal movement isn't faster
        if (inputDir.sqrMagnitude > 1f)
            inputDir.Normalize();

        // Move relative to world (no rotation influence)
        Vector3 move = inputDir * moveSpeed * Time.deltaTime;
        controller.Move(move);

        // Only rotate if actually moving
        Vector3 horizontalVelocity = new Vector3(controller.velocity.x, 0f, controller.velocity.z);
        if (horizontalVelocity.sqrMagnitude > 0.001f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(horizontalVelocity, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 10f * Time.deltaTime);
        }
    }
}