using UnityEngine;

namespace Velocity
{
    /// <summary>
    /// Lightweight runtime particle bursts (no art, no prefabs). One singleton spawns short-lived
    /// sprite particles that fly out, fall, fade and self-destroy. Used for death/finish/jump/land/dash juice.
    /// </summary>
    public class FX : MonoBehaviour
    {
        public static FX Instance { get; private set; }

        private static Sprite _dot;

        public static void Bootstrap()
        {
            if (Instance != null) return;
            var go = new GameObject("FX");
            DontDestroyOnLoad(go);
            Instance = go.AddComponent<FX>();
            _dot = TextureFactory.Circle(8, 24);
        }

        public void Burst(Vector3 pos, Color color, int count, float speed,
            float size = 0.16f, float life = 0.55f, float gravity = -16f, float upBias = 2f)
        {
            for (int i = 0; i < count; i++)
            {
                var go = new GameObject("fx");
                go.transform.position = pos;
                go.transform.localScale = Vector3.one * (size * Random.Range(0.7f, 1.3f));
                var sr = go.AddComponent<SpriteRenderer>();
                sr.sprite = _dot;
                sr.color = color;
                sr.sortingOrder = 30;

                var p = go.AddComponent<FxParticle>();
                Vector2 v = Random.insideUnitCircle * speed;
                v.y = Mathf.Abs(v.y) * 0.5f + upBias + Random.Range(0f, speed * 0.5f);
                p.Init(v, life * Random.Range(0.7f, 1.2f), gravity);
            }
        }

        public void Death(Vector3 p)
        {
            Burst(p, Palette.Hazard, 20, 9f, 0.18f, 0.6f, -20f, 3f);
            Burst(p, Palette.PlayerHi, 10, 6f, 0.14f, 0.5f, -14f, 4f);
        }

        public void Finish(Vector3 p)
        {
            Burst(p, Palette.Goal, 16, 8f, 0.18f, 0.9f, -10f, 6f);
            Burst(p, Palette.GoalGlow, 12, 6f, 0.14f, 1.0f, -8f, 7f);
            Burst(p, Palette.UiGold, 12, 7f, 0.16f, 0.9f, -10f, 6f);
        }

        public void LandPuff(Vector3 p)
        {
            if (Settings.ReduceMotion) return;
            Burst(p, Palette.GroundCap, 6, 3.5f, 0.12f, 0.35f, -8f, 0.5f);
        }

        public void JumpPuff(Vector3 p)
        {
            if (Settings.ReduceMotion) return;
            Burst(p, Palette.GroundCap, 4, 2.5f, 0.1f, 0.3f, -6f, 0.2f);
        }

        /// <summary>A fading ghost copy of a sprite, used for dash trails.</summary>
        public void Afterimage(Vector3 pos, Sprite sprite, Color color, bool flipX, float scale = 1f, float life = 0.22f)
        {
            if (Settings.ReduceMotion || sprite == null) return;
            var go = new GameObject("after");
            go.transform.position = pos;
            go.transform.localScale = Vector3.one * scale;
            var sr = go.AddComponent<SpriteRenderer>();
            sr.sprite = sprite;
            sr.color = color;
            sr.flipX = flipX;
            sr.sortingOrder = 9;
            go.AddComponent<FxParticle>().Init(Vector2.zero, life, 0f);
        }

        public void DashPuff(Vector3 p, int dir)
        {
            if (Settings.ReduceMotion) return;
            for (int i = 0; i < 6; i++)
            {
                var go = new GameObject("fx");
                go.transform.position = p;
                go.transform.localScale = Vector3.one * 0.14f;
                var sr = go.AddComponent<SpriteRenderer>();
                sr.sprite = _dot;
                sr.color = Palette.PlayerHi;
                sr.sortingOrder = 30;
                var part = go.AddComponent<FxParticle>();
                Vector2 v = new Vector2(-dir * Random.Range(2f, 6f), Random.Range(-1.5f, 1.5f));
                part.Init(v, 0.35f, 0f);
            }
        }
    }

    /// <summary>One particle: integrate velocity, apply gravity, fade + shrink, then destroy.</summary>
    public class FxParticle : MonoBehaviour
    {
        private Vector2 _vel;
        private float _life, _age, _gravity;
        private SpriteRenderer _sr;
        private Color _c0;
        private Vector3 _scale0;

        public void Init(Vector2 vel, float life, float gravity)
        {
            _vel = vel;
            _life = Mathf.Max(0.05f, life);
            _gravity = gravity;
            _sr = GetComponent<SpriteRenderer>();
            _c0 = _sr.color;
            _scale0 = transform.localScale;
        }

        private void Update()
        {
            float dt = Time.deltaTime;
            _age += dt;
            if (_age >= _life) { Destroy(gameObject); return; }

            _vel.y += _gravity * dt;
            transform.position += (Vector3)(_vel * dt);

            float t = _age / _life;
            var c = _c0;
            c.a = _c0.a * (1f - t);
            _sr.color = c;
            transform.localScale = _scale0 * (1f - 0.6f * t);
        }
    }
}
