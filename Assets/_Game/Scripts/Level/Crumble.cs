using UnityEngine;

namespace Velocity
{
    /// <summary>
    /// A platform that breaks a moment after the player stands on it, then respawns. Keep moving.
    /// </summary>
    public class Crumble : MonoBehaviour
    {
        public float breakDelay = 0.55f;
        public float respawnDelay = 2.2f;

        private enum State { Solid, Breaking, Broken }
        private State _state = State.Solid;
        private float _timer;

        private Collider2D _col;
        private SpriteRenderer _sr;
        private Color _solidColor;
        private Vector3 _home;

        private void Awake()
        {
            _col = GetComponent<Collider2D>();
            _sr = GetComponent<SpriteRenderer>();
            if (_sr != null) _solidColor = _sr.color;
            _home = transform.position;
        }

        private void Update()
        {
            float dt = Time.deltaTime;
            switch (_state)
            {
                case State.Solid:
                    if (PlayerOnTop()) { _state = State.Breaking; _timer = breakDelay; }
                    break;

                case State.Breaking:
                    _timer -= dt;
                    // shudder + redden as it's about to go
                    float k = 1f - Mathf.Clamp01(_timer / Mathf.Max(0.01f, breakDelay));
                    if (_sr != null) _sr.color = Color.Lerp(_solidColor, Palette.Hazard, k * 0.6f);
                    transform.position = _home + (Vector3)(Random.insideUnitCircle * 0.04f * k);
                    if (_timer <= 0f) Break();
                    break;

                case State.Broken:
                    _timer -= dt;
                    if (_timer <= 0f) Restore();
                    break;
            }
        }

        private bool PlayerOnTop()
        {
            var b = _col.bounds;
            Vector2 top = new Vector2(b.center.x, b.max.y + 0.08f);
            Vector2 size = new Vector2(b.size.x * 0.9f, 0.14f);
            var hit = Physics2D.OverlapBox(top, size, 0f, 1 << GameLayers.Player);
            return hit != null && hit.GetComponentInParent<PlayerController>() != null;
        }

        private void Break()
        {
            _state = State.Broken;
            _timer = respawnDelay;
            _col.enabled = false;
            transform.position = _home;
            if (_sr != null) { var c = _solidColor; c.a = 0.12f; _sr.color = c; }
            if (Sfx.Instance != null) Sfx.Instance.PlayCrumble();
            if (FX.Instance != null) FX.Instance.Burst(transform.position, _solidColor, 8, 4f, 0.14f, 0.5f);
        }

        private void Restore()
        {
            _state = State.Solid;
            _col.enabled = true;
            if (_sr != null) _sr.color = _solidColor;
        }
    }
}
