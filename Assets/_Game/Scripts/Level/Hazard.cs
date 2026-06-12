using UnityEngine;

namespace Velocity
{
    /// <summary>Anything that kills the player on contact (spikes, hazard blocks).</summary>
    public class Hazard : MonoBehaviour
    {
        private void OnTriggerEnter2D(Collider2D other) => TryKill(other);
        private void OnTriggerStay2D(Collider2D other) => TryKill(other);

        private void TryKill(Collider2D other)
        {
            if (other.GetComponentInParent<PlayerController>() == null) return;
            if (GameManager.Instance != null) GameManager.Instance.OnPlayerDied();
        }
    }
}
