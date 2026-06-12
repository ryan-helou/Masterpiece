using UnityEngine;

namespace Velocity
{
    /// <summary>A warp pipe / portal. Touching it teleports the player to its linked partner.</summary>
    public class Portal : MonoBehaviour
    {
        public Portal link;
        private float _cooldown;

        private void Update()
        {
            if (_cooldown > 0f) _cooldown -= Time.deltaTime;
        }

        public void SetCooldown(float t) => _cooldown = t;

        private void OnTriggerEnter2D(Collider2D other) => TryWarp(other);
        private void OnTriggerStay2D(Collider2D other) => TryWarp(other);

        private void TryWarp(Collider2D other)
        {
            if (_cooldown > 0f || link == null) return;
            var pc = other.GetComponentInParent<PlayerController>();
            if (pc == null) return;

            _cooldown = 0.6f;
            link.SetCooldown(0.6f);              // stop the exit from bouncing you back
            pc.Teleport(link.transform.position);

            if (Sfx.Instance != null) Sfx.Instance.PlayWarp();
            if (FX.Instance != null)
            {
                FX.Instance.Burst(transform.position, Palette.UiAccent, 12, 6f);
                FX.Instance.Burst(link.transform.position, Palette.GoalGlow, 12, 6f);
            }
            if (GameManager.Instance != null) GameManager.Instance.FlashTransition();
        }
    }
}
