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
            if (Sfx.Instance != null) Sfx.Instance.PlayDash();
            if (FX.Instance != null) FX.Instance.DashPuff(transform.position, velocity.x >= 0 ? 1 : -1);
        }
    }
}
