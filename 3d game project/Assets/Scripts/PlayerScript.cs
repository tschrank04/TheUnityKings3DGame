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
        Vector2 input = moveAction.ReadValue<Vector2>(); // x = left/right, y = up/down (W/S)
        Vector3 localMove = new Vector3(input.x, 0f, input.y);

        // Convert local movement to world space (so movement respects player rotation)
        Vector3 worldMove = transform.TransformDirection(localMove);

        // Move character
        controller.Move(worldMove * (moveSpeed * Time.deltaTime));

        controller.Move(velocity * Time.deltaTime);
    }
}