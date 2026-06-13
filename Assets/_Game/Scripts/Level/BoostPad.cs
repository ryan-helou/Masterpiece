using UnityEngine;

namespace Velocity
{
    /// <summary>Geometry-Dash dash panel: flings the player fast in a set direction.</summary>
    public class BoostPad : MonoBehaviour
    {
        public Vector2 velocity = new Vector2(22f, 4f);
        private float _cooldown;

        private void Update()
        {
            if (_cooldown > 0f) _cooldown -= Time.deltaTime;
        }

        private void OnTriggerEnter2D(Collider2D other) => TryBoost(other);
        private void OnTriggerStay2D(Collider2D other) => TryBoost(other);

        private void TryBoost(Collider2D other)
        {
            if (_cooldown > 0f) return;
            var pc = other.GetComponentInParent<PlayerController>();
            if (pc == null) return;

            _cooldown = 0.25f;
            pc.Boost(velocity.x, velocity.y);
            if (Sfx.Instance != null) Sfx.Instance.PlayBoost();
            if (FX.Instance != null) FX.Instance.DashPuff(transform.position, velocity.x >= 0 ? 1 : -1);
            // catapult kick: the fling launches you 25-31 u/s, so give it a punch of shake to
            // match (scaled by launch strength; the camera honours the reduce-shake setting)
            if (GameManager.Instance != null) GameManager.Instance.Shake(0.10f + velocity.magnitude * 0.005f);
        }
    }
}
