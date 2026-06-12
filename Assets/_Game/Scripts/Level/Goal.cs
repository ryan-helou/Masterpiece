using UnityEngine;

namespace Velocity
{
    /// <summary>Finish trigger. Touching it completes the level.</summary>
    public class Goal : MonoBehaviour
    {
        private bool _used;

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (_used) return;
            if (other.GetComponentInParent<PlayerController>() == null) return;
            _used = true;
            if (GameManager.Instance != null) GameManager.Instance.OnReachGoal();
        }
    }
}
