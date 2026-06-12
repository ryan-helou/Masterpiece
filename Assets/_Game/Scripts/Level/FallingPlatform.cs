using UnityEngine;

namespace Velocity
{
    /// <summary>Donut-block style platform: stand on it, it shakes, then drops away. Respawns.</summary>
    [RequireComponent(typeof(Rigidbody2D))]
    public class FallingPlatform : MonoBehaviour
    {
        public float shakeTime = 0.45f;
        public float fallSpeed = 16f;
        public float respawnDelay = 2.6f;

        private enum State { Idle, Shaking, Falling, Gone }
        private State _state = State.Idle;
        private float _timer;

        private Rigidbody2D _rb;
        private Collider2D _col;
        private SpriteRenderer _sr;
        private Vector3 _home;
        private Color _c0;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
            _rb.bodyType = RigidbodyType2D.Kinematic;
            _col = GetComponent<Collider2D>();
            _sr = GetComponent<SpriteRenderer>();
            _home = transform.position;
            if (_sr != null) _c0 = _sr.color;
        }

        private void Update()
        {
            float dt = Time.deltaTime;
            switch (_state)
            {
                case State.Idle:
                    if (PlayerOnTop())
                    {
                        _state = State.Shaking; _timer = shakeTime;
                        if (Sfx.Instance != null) Sfx.Instance.PlayCreak();
                    }
                    break;

                case State.Shaking:
                    _timer -= dt;
                    transform.position = _home + (Vector3)(Random.insideUnitCircle * 0.05f);
                    if (_sr != null) _sr.color = Color.Lerp(_c0, Palette.Hazard, 0.4f);
                    if (_timer <= 0f) { _state = State.Falling; _timer = 1.2f; }
                    break;

                case State.Falling:
                    transform.position += Vector3.down * fallSpeed * dt;
                    _timer -= dt;
                    if (_timer <= 0f)
                    {
                        _col.enabled = false;
                        if (_sr != null) _sr.enabled = false;
                        _state = State.Gone;
                        _timer = respawnDelay;
                    }
                    break;

                case State.Gone:
                    _timer -= dt;
                    if (_timer <= 0f) Restore();
                    break;
            }
        }

        private bool PlayerOnTop()
        {
            var b = _col.bounds;
            var hit = Physics2D.OverlapBox(new Vector2(b.center.x, b.max.y + 0.08f),
                new Vector2(b.size.x * 0.9f, 0.14f), 0f, 1 << GameLayers.Player);
            return hit != null && hit.GetComponentInParent<PlayerController>() != null;
        }

        private void Restore()
        {
            transform.position = _home;
            _col.enabled = true;
            if (_sr != null) { _sr.enabled = true; _sr.color = _c0; }
            _state = State.Idle;
        }
    }
}
