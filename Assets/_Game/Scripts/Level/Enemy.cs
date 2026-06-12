using UnityEngine;

namespace Velocity
{
    /// <summary>
    /// Base mob. Resolves player contact: a stomp (player falling onto the top) kills the enemy and
    /// bounces the player; any other contact kills the player. Enemies are script-moved kinematic
    /// triggers (no gravity), so subclasses just implement their patrol in Update.
    /// </summary>
    public abstract class Enemy : MonoBehaviour
    {
        protected Collider2D _col;
        protected SpriteRenderer _sr;
        protected bool _dead;

        protected virtual bool Stompable => true;

        protected virtual void Awake()
        {
            _col = GetComponent<Collider2D>();
            _sr = GetComponentInChildren<SpriteRenderer>();
        }

        protected void OnTriggerEnter2D(Collider2D other) => Resolve(other);
        protected void OnTriggerStay2D(Collider2D other) => Resolve(other);

        private void Resolve(Collider2D other)
        {
            if (_dead) return;
            var pc = other.GetComponentInParent<PlayerController>();
            if (pc == null) return;

            bool stomp = Stompable && pc.Velocity.y < -0.5f &&
                         other.bounds.min.y > _col.bounds.center.y - 0.05f;

            if (stomp) Stomped(pc);
            else if (GameManager.Instance != null) GameManager.Instance.OnPlayerDied();
        }

        protected virtual void Stomped(PlayerController pc)
        {
            _dead = true;
            pc.StompBounce();
            if (Sfx.Instance != null) Sfx.Instance.PlayStomp();
            if (GameManager.Instance != null) GameManager.Instance.Hitstop(0.05f);
            if (FX.Instance != null)
                FX.Instance.Burst(transform.position, _sr != null ? _sr.color : Palette.Hazard, 12, 5f, 0.15f, 0.45f);
            Destroy(gameObject);
        }

        // ---- patrol helpers (raycasts vs ground/walls) ----
        protected bool GroundAhead(int dir)
        {
            Vector2 o = new Vector2(transform.position.x + dir * (_col.bounds.extents.x + 0.05f),
                                    _col.bounds.min.y + 0.05f);
            return Physics2D.Raycast(o, Vector2.down, 0.45f, GameLayers.GroundMask).collider != null;
        }

        protected bool WallAhead(int dir)
        {
            Vector2 o = new Vector2(transform.position.x, _col.bounds.center.y);
            return Physics2D.Raycast(o, new Vector2(dir, 0f), _col.bounds.extents.x + 0.15f,
                                     GameLayers.GroundMask).collider != null;
        }

        protected void Face(int dir)
        {
            if (_sr != null) _sr.flipX = dir < 0;
        }
    }

    /// <summary>Goomba-style ground patroller. Turns at walls, ledges, or a patrol range.</summary>
    public class EnemyWalker : Enemy
    {
        public float speed = 2.2f;
        public int dir = -1;
        public float range = 0f;        // 0 = roam until wall/ledge

        protected Vector3 _origin;

        protected override void Awake()
        {
            base.Awake();
            _origin = transform.position;
        }

        protected virtual void Update()
        {
            if (_dead) return;

            if (WallAhead(dir) || !GroundAhead(dir) ||
                (range > 0f && Mathf.Abs(transform.position.x - _origin.x) > range && Mathf.Sign(transform.position.x - _origin.x) == dir))
                dir = -dir;

            transform.position += new Vector3(dir * speed * Time.deltaTime, 0f, 0f);
            Face(dir);
        }
    }

    /// <summary>Hovering enemy: drifts horizontally within a range and bobs up and down.</summary>
    public class EnemyFlyer : Enemy
    {
        public float speed = 2.6f;
        public int dir = -1;
        public float range = 4f;
        public float bobAmp = 0.6f, bobFreq = 2.2f;

        private Vector3 _origin;
        private float _t;

        protected override void Awake()
        {
            base.Awake();
            _origin = transform.position;
        }

        private void Update()
        {
            if (_dead) return;
            _t += Time.deltaTime;

            float dx = transform.position.x - _origin.x;
            if (WallAhead(dir) || (dx > range && dir > 0) || (dx < -range && dir < 0)) dir = -dir;

            var p = transform.position;
            p.x += dir * speed * Time.deltaTime;
            p.y = _origin.y + Mathf.Sin(_t * bobFreq) * bobAmp;
            transform.position = p;
            Face(dir);
        }
    }

    /// <summary>Spiked patroller — cannot be stomped (touching it anywhere kills you).</summary>
    public class EnemySpiky : EnemyWalker
    {
        protected override bool Stompable => false;
    }

    /// <summary>Hovering hunter: bobs in place, then SWOOPS straight down when the player passes
    /// underneath, and climbs back to its perch. Stompable from above on the way down.</summary>
    public class EnemyDiver : Enemy
    {
        public float diveSpeed = 9f, riseSpeed = 3.5f, triggerDist = 3.5f, diveDepth = 4.5f;
        public float bobAmp = 0.3f, bobFreq = 3f;

        private Vector3 _home;
        private float _t, _bottom;
        private int _state;   // 0 hover, 1 dive, 2 rise

        protected override void Awake()
        {
            base.Awake();
            _home = transform.position;
        }

        private void Update()
        {
            if (_dead) return;
            _t += Time.deltaTime;
            var pos = transform.position;

            switch (_state)
            {
                case 0:
                    pos.y = _home.y + Mathf.Sin(_t * bobFreq) * bobAmp;
                    var p = PlayerController.Active;
                    if (p != null && Mathf.Abs(p.transform.position.x - _home.x) < triggerDist &&
                        p.transform.position.y < _home.y - 0.5f)
                    { _state = 1; _bottom = _home.y - diveDepth; }
                    break;
                case 1:
                    pos.y -= diveSpeed * Time.deltaTime;
                    if (pos.y <= _bottom) _state = 2;
                    break;
                case 2:
                    pos.y += riseSpeed * Time.deltaTime;
                    if (pos.y >= _home.y) { pos.y = _home.y; _state = 0; }
                    break;
            }
            transform.position = pos;
        }
    }
}
