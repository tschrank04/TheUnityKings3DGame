using UnityEngine;

public class CameraScript : MonoBehaviour
{
    [Header("Target")]
    public Transform target;

    [Header("Position Settings")]
    [Tooltip("How high above the player the camera sits.")]
    public float cameraHeight = 10f;

    [Tooltip("How far behind the player the camera sits.")]
    public float cameraDistance = 5f;

    [Tooltip("How smoothly the camera follows the player. Lower = snappier.")]
    [Range(0.01f, 1f)]
    public float followSmoothTime = 0.12f;

    private Vector3 velocity = Vector3.zero;

    [Header("Angle Settings")]
    [Tooltip("The downward tilt angle of the camera.")]
    [Range(0f, 90f)]
    public float tiltAngle = 60f;

    void LateUpdate()
    {
        if (target == null)
            return;

        // Desired position: above and slightly behind player (world-relative, not rotating with them)
        Vector3 desiredPosition = target.position + new Vector3(0, cameraHeight, -cameraDistance);

        // Smoothly move toward the desired position
        transform.position = Vector3.SmoothDamp(transform.position, desiredPosition, ref velocity, followSmoothTime);

        // Keep a consistent downward tilt
        transform.rotation = Quaternion.Euler(tiltAngle, 0f, 0f);
    }
}
