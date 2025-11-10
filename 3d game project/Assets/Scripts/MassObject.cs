using UnityEngine;

[RequireComponent(typeof(Collider))]
public class MassObject : MonoBehaviour
{
    [Tooltip("Mass value determining how large the player must be to consume this.")]
    public float massValue = 1f;

    [Tooltip("Should this object destroy itself upon being consumed?")]
    public bool destroyOnConsume = true;

    public void OnConsumed()
    {
        // Optional: Add sound, particle effects, etc.
        if (destroyOnConsume)
        {
            Destroy(gameObject);
        }
    }
}
