using UnityEngine;

namespace Velocity
{
    /// <summary>A solid bounce block (Mario note block): land on top and you rocket upward.</summary>
    public class NoteBlock : MonoBehaviour
    {
        public float bounce = 18f;

        private Collider2D _col;
        private SpriteRenderer _sr;
        private Color _c0;
        private float _cooldown;

        private void Awake()
        {
            _col = GetComponent<Collider2D>();
            _sr = GetComponent<SpriteRenderer>();
            if (_sr != null) _c0 = _sr.color;
        }

        private void Update()
        {
            if (_cooldown > 0f) { _cooldown -= Time.deltaTime; if (_sr != null) _sr.color = Color.Lerp(Palette.GoalGlow, _c0, 1f - _cooldown / 0.2f); return; }

            var b = _col.bounds;
            var hit = Physics2D.OverlapBox(new Vector2(b.center.x, b.max.y + 0.1f),
                new Vector2(b.size.x * 0.9f, 0.18f), 0f, 1 << GameLayers.Player);
            if (hit == null) return;
            var pc = hit.GetComponentInParent<PlayerController>();
            if (pc == null || pc.Velocity.y > 0.6f) return;     // only when landing/resting

            pc.LaunchUp(bounce);
            _cooldown = 0.2f;
            if (_sr != null) _sr.color = Palette.GoalGlow;
            if (Sfx.Instance != null) Sfx.Instance.PlayNote();
        }
    }
}
