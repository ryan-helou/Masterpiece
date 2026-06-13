using UnityEngine;

namespace Velocity
{
    /// <summary>Bounce pad: launches the player straight up on contact.</summary>
    public class Spring : MonoBehaviour
    {
        public float launchVelocity = 26f;   // keeps the bounce-coin reachable under stronger gravity
        private float _cooldown;

        private void Update()
        {
            if (_cooldown > 0f) _cooldown -= Time.deltaTime;
        }

        private void OnTriggerEnter2D(Collider2D other) => TryLaunch(other);
        private void OnTriggerStay2D(Collider2D other) => TryLaunch(other);

        private void TryLaunch(Collider2D other)
        {
            if (_cooldown > 0f) return;
            var p = other.GetComponentInParent<PlayerController>();
            if (p == null) return;

            _cooldown = 0.25f;
            p.LaunchUp(launchVelocity);
            if (Sfx.Instance != null) Sfx.Instance.PlaySpring();
            if (FX.Instance != null) FX.Instance.JumpPuff(transform.position);
            // a light kick so the bounce reads as a launch, not a free float (gentler than a boost pad)
            if (GameManager.Instance != null) GameManager.Instance.Shake(0.12f);
        }
    }
}
