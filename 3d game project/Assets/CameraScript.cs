using UnityEngine;

public class CameraScript:MonoBehaviour
{
    //The player that the camera follows
    [Header("Target")] public Transform target;

    //Where camera looks relative to target
    public Vector3 lookAtOffset = Vector3.up * 1.5f;

    [Header("Position")]
    public bool useLocalOffset = true;       // If true uses target's local space for offset (common for third-person)
    public Vector3 offset = new Vector3(0f, 2f, -5f);
    [Tooltip("Time for smooth damp. Lower = snappier.")]
    [Range(0.01f, 1f)]
    public float followSmoothTime = 0.12f;
    private Vector3 velocity = Vector3.zero;

    [Header("Rotation")]
    public bool lookAtTarget = true;
    [Tooltip("How fast the camera rotates to look at the target.")]
    public float rotationSpeed = 10f;

    [Header("Collision / Clipping")]
    public bool enableCollisionAvoidance = true;
    [Tooltip("Radius for spherecast to avoid geometry clipping")]
    public float sphereRadius = 0.2f;
    [Tooltip("Minimum allowed distance from the target (prevents camera being pushed inside target)")]
    public float minDistance = 0.5f;
    [Tooltip("Padding from hit point to keep camera off geometry")]
    public float collisionPadding = 0.05f;
    public LayerMask collisionMask = ~0; // default: everything

    void LateUpdate()
    {
        if (target == null) return;

        // Desired world position for the camera
        Vector3 desiredPosition = useLocalOffset ? target.TransformPoint(offset) : target.position + offset;

        // Collision avoidance: keep a clear sight line from target's look point to the camera
        if (enableCollisionAvoidance)
        {
            Vector3 rayOrigin = target.position + lookAtOffset;
            Vector3 direction = (desiredPosition - rayOrigin);
            float fullDistance = direction.magnitude;

            if (fullDistance > 0.001f)
            {
                direction /= fullDistance; // normalize

                RaycastHit hit;
                // SphereCast to detect potential clipping geometry
                if (Physics.SphereCast(rayOrigin, sphereRadius, direction, out hit, fullDistance, collisionMask, QueryTriggerInteraction.Ignore))
                {
                    float clampedDistance = Mathf.Max(hit.distance - collisionPadding, minDistance);
                    desiredPosition = rayOrigin + direction * clampedDistance;
                }
            }
        }

        // Smoothly move camera to desired position
        transform.position = Vector3.SmoothDamp(transform.position, desiredPosition, ref velocity, followSmoothTime);

        // Smoothly rotate so camera looks at the target (if enabled)
        if (lookAtTarget)
        {
            Vector3 lookPoint = target.position + lookAtOffset;
            Vector3 toTarget = lookPoint - transform.position;
            if (toTarget.sqrMagnitude > 0.0001f)
            {
                Quaternion desiredRot = Quaternion.LookRotation(toTarget.normalized, Vector3.up);
                transform.rotation = Quaternion.Slerp(transform.rotation, desiredRot, rotationSpeed * Time.deltaTime);
            }
        }
    }

    // Keep inspector-friendly ranges
    void OnValidate()
    {
        followSmoothTime = Mathf.Max(0.0001f, followSmoothTime);
        sphereRadius = Mathf.Max(0f, sphereRadius);
        minDistance = Mathf.Max(0f, minDistance);
        collisionPadding = Mathf.Max(0f, collisionPadding);
        rotationSpeed = Mathf.Max(0f, rotationSpeed);
    }
}
