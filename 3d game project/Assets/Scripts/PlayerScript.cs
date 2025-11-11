using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerScript : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float rotationSpeed = 10f;
    private CharacterController controller;
    private InputAction moveAction;
    private Vector3 velocity;

    [Header("Mass & Growth Settings")]
    [SerializeField] private float playerMass = 1f; // starting mass, same as die
    [SerializeField] private float growthRate = 0.25f; // how fast player scales after eating
    [SerializeField] private float scaleMultiplier = 1f; // for fine-tuning visual scale

    void Awake()
    {
        controller = GetComponent<CharacterController>();

        // Input setup
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

    void OnEnable() => moveAction.Enable();
    void OnDisable()
    {
        moveAction.Disable();
        moveAction.Dispose();
    }

    void Update()
    {
        // --- MOVEMENT ---
        Vector2 input = moveAction.ReadValue<Vector2>();
        Vector3 inputDir = new Vector3(input.x, 0f, input.y);
        if (inputDir.sqrMagnitude > 1f)
            inputDir.Normalize();

        controller.Move(inputDir * moveSpeed * Time.deltaTime);

        // --- ROTATION (face direction of actual movement) ---
        Vector3 horizontalVelocity = new Vector3(controller.velocity.x, 0f, controller.velocity.z);
        if (horizontalVelocity.sqrMagnitude > 0.001f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(horizontalVelocity, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

        // --- GRAVITY PLACEHOLDER ---
        controller.Move(velocity * Time.deltaTime);
    }

    private System.Collections.IEnumerator GrowToNewSize()
    {
        // Calculate new target scale based on mass
        Vector3 startScale = transform.localScale;
        Vector3 targetScale = Vector3.one * Mathf.Pow(playerMass, 1f / 3f) * scaleMultiplier;
        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime * growthRate;
            transform.localScale = Vector3.Lerp(startScale, targetScale, t);
            yield return null;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        MassObject obj = other.GetComponent<MassObject>();
        if (obj != null)
        {
            // Only consume if object is small enough
            if (obj.massValue <= playerMass)
            {
                float massGain = obj.massValue * 0.25f; // smaller incremental gain
                obj.OnConsumed();

                // Stop previous animations to prevent stacking
                StopAllCoroutines();

                // Run the quick pulse and apply permanent growth right after
                StartCoroutine(PulseAndGrow(massGain));
            }
        }
    }

    private System.Collections.IEnumerator PulseAndGrow(float massGain)
    {
        Vector3 originalScale = transform.localScale;
        Vector3 pulseScale = originalScale * 1.1f; // smaller, faster pulse
        float pulseUpTime = 0.07f;   // quick expansion
        float pulseDownTime = 0.07f; // quick return

        // --- Pulse up ---
        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / pulseUpTime;
            transform.localScale = Vector3.Lerp(originalScale, pulseScale, t);
            yield return null;
        }

        // --- Pulse down ---
        t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / pulseDownTime;
            transform.localScale = Vector3.Lerp(pulseScale, originalScale, t);
            yield return null;
        }

        // --- Apply permanent mass gain ---
        playerMass += massGain;

        // Smoothly lerp to the new correct scale
        Vector3 startScale = transform.localScale;
        Vector3 targetScale = Vector3.one * Mathf.Pow(playerMass, 1f / 3f) * scaleMultiplier;

        float growthSpeed = 5f; // faster transition to avoid stacking visually
        float lerpT = 0f;

        while (lerpT < 1f)
        {
            lerpT += Time.deltaTime * growthSpeed;
            transform.localScale = Vector3.Lerp(startScale, targetScale, lerpT);
            yield return null;
        }
    }
}
