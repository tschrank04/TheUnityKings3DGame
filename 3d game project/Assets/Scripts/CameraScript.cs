using UnityEngine;

public class CameraScript : MonoBehaviour
{
    [Header("Target")]
    public Transform target;

    [Header("Position")]
    public Vector3 offset = new Vector3(0f, 10f, -5f); // sits above and behind player
    [Range(0.01f, 1f)]
    public float followSmoothTime = 0.12f;
    private Vector3 velocity = Vector3.zero;

    void LateUpdate()
    {
        if (target == null) return;

        // Desired position: offset in world space (so camera doesn't rotate with player)
        Vector3 desiredPosition = target.position + offset;

        // Smoothly move the camera toward desired position
        transform.position = Vector3.SmoothDamp(transform.position, desiredPosition, ref velocity, followSmoothTime);

        // Keep the camera looking straight down at a fixed angle
        transform.rotation = Quaternion.Euler(60f, 0f, 0f); // tweak angle if you want more/less top-down
    }
}
