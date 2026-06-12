using UnityEngine;

namespace Velocity
{
    /// <summary>
    /// A floating orb that refills your air-dash on contact (so you can chain two dashes to cross a
    /// gap that's too wide for one). Only triggers if you've actually used your dash, then it
    /// fades out and respawns after a short delay.
    /// </summary>
    public class DashOrb : MonoBehaviour
    {
        public float respawnDelay = 1.4f;

        private Collider2D _col;
        private SpriteRenderer _sr;
        private Color _onColor;
        private float _cooldown;

        private void Awake()
        {
            _col = GetComponent<Collider2D>();
            _sr = GetComponentInChildren<SpriteRenderer>();
            if (_sr != null) _onColor = _sr.color;
        }

        private void Update()
        {
            if (_cooldown > 0f)
            {
                _cooldown -= Time.deltaTime;
                if (_cooldown <= 0f) Reactivate();
            }
        }

        private void OnTriggerEnter2D(Collider2D other) => TryUse(other);
        private void OnTriggerStay2D(Collider2D other) => TryUse(other);

        private void TryUse(Collider2D other)
        {
            if (_cooldown > 0f) return;
            var pc = other.GetComponentInParent<PlayerController>();
            if (pc == null || !pc.RefillDash()) return;     // only spend it if it gave a dash back

            _cooldown = respawnDelay;
            if (_sr != null) { var c = _onColor; c.a = 0.15f; _sr.color = c; }
            if (Sfx.Instance != null) Sfx.Instance.PlayOrb();
            if (FX.Instance != null) FX.Instance.Burst(transform.position, Palette.UiAccent, 10, 6f, 0.16f, 0.45f);
        }

        private void Reactivate()
        {
            if (_sr != null) _sr.color = _onColor;
        }
    }
}
