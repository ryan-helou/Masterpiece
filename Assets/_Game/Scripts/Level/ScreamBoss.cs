using UnityEngine;

namespace Velocity
{
    /// <summary>
    /// THE SCREAM, made flesh — Level 4's final boss. A giant anguish-knot that hovers high over
    /// the arena, locks onto the player, telegraphs with a burning floor-marker, then PLUNGES.
    /// Dodge the slam, then stomp its crown while it's low; three hits and the scream dies and the
    /// sealed exit opens (see <see cref="Goal"/>, which checks <see cref="Alive"/>). Touching its
    /// body any other way kills you. It grows more frantic with every hit.
    ///
    /// Self-contained: own trigger resolution (full control of i-frames), own state machine. The
    /// arena floor is found by a downward raycast so it dives to the real ground, not a guess.
    /// </summary>
    public class ScreamBoss : MonoBehaviour
    {
        /// <summary>How many Screams are still alive in the level — the Goal stays sealed while > 0.
        /// Reset to 0 by LevelBuilder at the start of every build so it can never leak across loads.</summary>
        public static int Alive;

        public int maxHits = 3;
        public float hoverHeight = 7f;       // world units above the arena floor it returns to
        public float bobAmp = 0.4f, bobFreq = 2.2f;
        public float trackSpeed = 3.2f;      // horizontal chase while hovering (slower than the player)
        public float diveSpeed = 17f;
        public float riseSpeed = 7f;
        public float telegraphTime = 0.55f;  // the tell: lock the target, flash, then drop
        public float hoverTime = 1.35f;      // gap between dives (shrinks as HP falls)
        public float invulnTime = 0.85f;     // i-frames after a hit (boss passes harmlessly through you)
        public float arenaHalf = 11f;        // it never drifts more than this from its spawn column
        public float engageDist = 13f;       // ...and won't start a dive until the player is this close

        private enum S { Intro, Hover, Telegraph, Dive, Stun, Rise, Hurt, Dead }
        private S _state = S.Intro;
        private int _hits;
        private float _timer, _t, _floorY, _lockX, _invuln, _homeX;

        private Collider2D _col;
        private SpriteRenderer _sr;
        private Transform _visual;
        private Color _baseColor;
        private Vector3 _baseScale;
        private SpriteRenderer _marker;      // floor target shown during the telegraph

        private void Awake()
        {
            Alive++;
            _col = GetComponent<Collider2D>();
            _sr = GetComponentInChildren<SpriteRenderer>();
            _visual = _sr != null ? _sr.transform : transform;
            if (_sr != null) _baseColor = _sr.color;
            _baseScale = _visual.localScale;

            _homeX = transform.position.x;
            var hit = Physics2D.Raycast(transform.position, Vector2.down, 40f, GameLayers.GroundMask);
            _floorY = hit.collider != null ? hit.point.y : transform.position.y - hoverHeight;

            // burning floor-marker (telegraph) — its own object so it stays on the ground
            var mk = new GameObject("ScreamTarget");
            mk.transform.SetParent(transform, false);
            _marker = mk.AddComponent<SpriteRenderer>();
            _marker.sprite = TextureFactory.RadialGlow(48);
            _marker.color = new Color(1f, 0.18f, 0.18f, 0f);
            _marker.sortingOrder = 4;
            _marker.transform.localScale = Vector3.one * 2.2f;
            _marker.enabled = false;

            _timer = 1.0f;     // intro grace before the first dive
        }

        private void OnDestroy()
        {
            Alive = Mathf.Max(0, Alive - 1);
        }

        private float HomeY => _floorY + hoverHeight;
        private float HoverGap => Mathf.Max(0.45f, hoverTime * (1f - 0.28f * _hits));   // frantic as HP drops

        private void Update()
        {
            if (_state == S.Dead) return;
            float dt = Time.deltaTime;
            _t += dt;
            _invuln -= dt;
            var pos = transform.position;
            var p = PlayerController.Active;

            switch (_state)
            {
                case S.Intro:
                    pos.y = Mathf.MoveTowards(pos.y, HomeY, riseSpeed * dt);
                    _timer -= dt;
                    if (_timer <= 0f && Mathf.Abs(pos.y - HomeY) < 0.15f) { _state = S.Hover; _timer = HoverGap; }
                    break;

                case S.Hover:
                    pos.y = HomeY + Mathf.Sin(_t * bobFreq) * bobAmp;
                    if (p != null) pos.x = Mathf.MoveTowards(pos.x, p.transform.position.x, trackSpeed * dt);
                    _timer -= dt;
                    bool inRange = p != null && Mathf.Abs(p.transform.position.x - _homeX) < engageDist;
                    if (_timer <= 0f && inRange)
                    {
                        _state = S.Telegraph; _timer = telegraphTime;
                        _lockX = p.transform.position.x;
                    }
                    else if (_timer <= 0f) _timer = 0.2f;     // player not here yet — keep idling
                    break;

                case S.Telegraph:
                    // ease toward the locked column (still beatable: slower than a sprint), flash + swell
                    pos.x = Mathf.MoveTowards(pos.x, _lockX, trackSpeed * 1.4f * dt);
                    pos.y = HomeY + Mathf.Sin(_t * bobFreq) * bobAmp;
                    Swell(1f + 0.12f * Mathf.Sin(_t * 42f));
                    ShowMarker(pos.x);
                    _timer -= dt;
                    if (_timer <= 0f) { _state = S.Dive; HideMarker(); }
                    break;

                case S.Dive:
                    pos.y -= diveSpeed * dt;
                    float bottom = _floorY + _col.bounds.extents.y;     // its underside meets the floor
                    if (pos.y <= bottom)
                    {
                        pos.y = bottom;
                        _state = S.Stun; _timer = 0.5f;     // beached low for a beat — the stomp window
                        if (GameManager.Instance != null) { GameManager.Instance.Shake(0.35f); GameManager.Instance.Kick(Vector2.down, 0.5f); }
                        if (Sfx.Instance != null) Sfx.Instance.PlaySlam();
                        if (FX.Instance != null) FX.Instance.Burst(new Vector3(pos.x, _floorY, 0f), Palette.Hazard, 16, 7f, 0.16f, 0.5f, -9f, 1.2f);
                    }
                    break;

                case S.Stun:
                    pos.y = _floorY + _col.bounds.extents.y + Mathf.Sin(_t * 30f) * 0.05f;   // shudder, low + stompable
                    _timer -= dt;
                    if (_timer <= 0f) _state = S.Rise;
                    break;

                case S.Rise:
                    pos.y = Mathf.MoveTowards(pos.y, HomeY, riseSpeed * dt);
                    if (pos.y >= HomeY - 0.05f) { _state = S.Hover; _timer = HoverGap; }
                    break;

                case S.Hurt:
                    pos.y = Mathf.MoveTowards(pos.y, HomeY, riseSpeed * 1.7f * dt);   // recoil upward
                    _timer -= dt;
                    if (_timer <= 0f) { _state = S.Hover; _timer = HoverGap; }
                    break;
            }

            pos.x = Mathf.Clamp(pos.x, _homeX - arenaHalf, _homeX + arenaHalf);   // never leave the arena
            transform.position = pos;
            if (_state != S.Telegraph) { Swell(1f); HideMarker(); }
            // flicker the body while invulnerable so the i-frames read
            if (_sr != null)
            {
                var c = _baseColor;
                if (_invuln > 0f) c.a = 0.45f + 0.4f * Mathf.PingPong(_t * 12f, 1f);
                _sr.color = c;
            }
        }

        private void Swell(float s)
        {
            if (_visual != null) _visual.localScale = Vector3.Lerp(_visual.localScale, _baseScale * s, 12f * Time.deltaTime);
        }

        private void ShowMarker(float x)
        {
            if (_marker == null) return;
            _marker.enabled = true;
            _marker.transform.position = new Vector3(x, _floorY + 0.12f, 0f);
            float a = Mathf.PingPong(_t * 6f, 1f) * 0.5f + 0.3f;
            _marker.color = new Color(1f, 0.18f, 0.18f, a);
            _marker.transform.localScale = Vector3.one * (2.2f + 0.5f * Mathf.Sin(_t * 10f));
        }

        private void HideMarker() { if (_marker != null) _marker.enabled = false; }

        private void OnTriggerEnter2D(Collider2D other) => Resolve(other);
        private void OnTriggerStay2D(Collider2D other) => Resolve(other);

        private void Resolve(Collider2D other)
        {
            if (_state == S.Dead || _state == S.Intro || _invuln > 0f) return;
            var pc = other.GetComponentInParent<PlayerController>();
            if (pc == null) return;

            bool stomp = pc.Velocity.y < -0.5f && other.bounds.min.y > _col.bounds.center.y - 0.1f;
            if (stomp) Hit(pc);
            else if (GameManager.Instance != null) GameManager.Instance.OnPlayerDied();
        }

        private void Hit(PlayerController pc)
        {
            pc.StompBounce();
            _hits++;
            _invuln = invulnTime;
            if (Sfx.Instance != null) Sfx.Instance.PlayStomp();
            if (GameManager.Instance != null) { GameManager.Instance.Hitstop(0.08f); GameManager.Instance.Shake(0.25f); }
            if (FX.Instance != null) FX.Instance.Burst(transform.position, _sr != null ? _sr.color : Palette.Hazard, 18, 7f, 0.18f, 0.55f);

            if (_hits >= maxHits) Die();
            else { _state = S.Hurt; _timer = 0.45f; }
        }

        private void Die()
        {
            _state = S.Dead;
            HideMarker();
            if (_col != null) _col.enabled = false;
            if (Sfx.Instance != null) Sfx.Instance.PlayStomp();
            if (GameManager.Instance != null) { GameManager.Instance.Shake(0.7f); GameManager.Instance.Hitstop(0.12f); }
            if (FX.Instance != null)
                FX.Instance.Burst(transform.position, Palette.Hazard, 42, 10f, 0.3f, 0.9f, -5f, 2.5f);
            // a beat of spin-and-shrink, then gone — OnDestroy drops Alive and unseals the Goal
            var spin = _visual != null ? _visual.gameObject.AddComponent<Spinner>() : null;
            if (spin != null) spin.degreesPerSecond = 520f;
            Destroy(gameObject, 0.45f);
        }
    }
}
