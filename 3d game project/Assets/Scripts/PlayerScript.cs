using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(AudioSource))] // Ensure AudioSource is attached
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

    [Header("Jump and Roll Settings")]
    [SerializeField] private float jumpHeight = 2f;
    [SerializeField] private float jumpDuration = 0.5f;
    [SerializeField] private float rollSpeed = 360f; // degrees per second
    private bool isJumping = false;
    private bool isRolling = false;

    private InputAction jumpAction;
    private InputAction rollAction;

    [Header("Audio Settings")]
    [SerializeField] private AudioClip jumpSFX;
    [SerializeField] private AudioClip spinSFX;
    [SerializeField] private AudioClip absorptionSFX;
    private AudioSource audioSource;

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

        // Input setup for jump and roll
        jumpAction = new InputAction("Jump", InputActionType.Button, "<Keyboard>/space");
        rollAction = new InputAction("Roll", InputActionType.Button, "<Keyboard>/leftCtrl");

        audioSource = GetComponent<AudioSource>();
    }

    void OnEnable()
    {
        moveAction.Enable();
        jumpAction.Enable();
        rollAction.Enable();
    }

    void OnDisable()
    {
        moveAction.Disable();
        jumpAction.Disable();
        rollAction.Disable();

        moveAction.Dispose();
        jumpAction.Dispose();
        rollAction.Dispose();
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

        // Handle jump input
        if (jumpAction.triggered && !isJumping)
        {
            if (jumpSFX != null)
            {
                audioSource.PlayOneShot(jumpSFX);
            }
            StartCoroutine(PerformJump());
        }

        // Handle roll input
        if (rollAction.triggered && !isRolling)
        {
            if (spinSFX != null)
            {
                audioSource.PlayOneShot(spinSFX);
            }
            StartCoroutine(PerformRoll());
        }
    }

    private System.Collections.IEnumerator PerformJump()
    {
        isJumping = true;
        float elapsedTime = 0f;
        Vector3 startPosition = transform.position;
        Vector3 peakPosition = startPosition + Vector3.up * jumpHeight;

        // Ascend
        while (elapsedTime < jumpDuration / 2f)
        {
            elapsedTime += Time.deltaTime;
            transform.position = Vector3.Lerp(startPosition, peakPosition, elapsedTime / (jumpDuration / 2f));
            yield return null;
        }

        elapsedTime = 0f;

        // Descend
        while (elapsedTime < jumpDuration / 2f)
        {
            elapsedTime += Time.deltaTime;
            transform.position = Vector3.Lerp(peakPosition, startPosition, elapsedTime / (jumpDuration / 2f));
            yield return null;
        }

        isJumping = false;
    }

    private System.Collections.IEnumerator PerformRoll()
    {
        isRolling = true;
        float elapsedTime = 0f;
        float rollDuration = 1f; // Roll lasts for 1 second

        while (elapsedTime < rollDuration)
        {
            elapsedTime += Time.deltaTime;
            transform.Rotate(Vector3.up, rollSpeed * Time.deltaTime, Space.World);
            yield return null;
        }

        isRolling = false;
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
                float massGain = obj.massValue * 0.40f; // smaller incremental gain
                obj.OnConsumed();

                // Play absorption sound effect
                if (absorptionSFX != null)
                {
                    audioSource.PlayOneShot(absorptionSFX);
                }

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
