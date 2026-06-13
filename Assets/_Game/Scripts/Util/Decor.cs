using UnityEngine;

namespace Velocity
{
    /// <summary>Breathing pulse for stars / glows — gently scales and brightens a SpriteRenderer.</summary>
    public class Twinkle : MonoBehaviour
    {
        public float speed = 1.6f;
        public float scaleAmp = 0.15f;
        public float alphaAmp = 0.3f;
        public float phase;

        private Vector3 _baseScale;
        private SpriteRenderer _sr;
        private float _baseA;

        private void Start()
        {
            _baseScale = transform.localScale;
            _sr = GetComponent<SpriteRenderer>();
            if (_sr != null) _baseA = _sr.color.a;
        }

        private void Update()
        {
            float t = Mathf.Sin((Time.time + phase) * speed);
            transform.localScale = _baseScale * (1f + t * scaleAmp);
            if (_sr != null)
            {
                var c = _sr.color;
                c.a = Mathf.Clamp01(_baseA * (1f + t * alphaAmp));
                _sr.color = c;
            }
        }
    }

    /// <summary>Gentle rotational sway (cypress / foliage). Pivot the visual at its base.</summary>
    public class Sway : MonoBehaviour
    {
        public float speed = 1f;
        public float degrees = 4f;
        public float phase;

        private void Update()
        {
            transform.localRotation = Quaternion.Euler(0f, 0f, Mathf.Sin((Time.time + phase) * speed) * degrees);
        }
    }

    /// <summary>Slow positional drift around the spawn point — wind-blown spray, foam, motes.
    /// Horizontal sweep with a smaller, out-of-phase vertical bob so paths trace lazy ovals.</summary>
    public class Drift : MonoBehaviour
    {
        public float speed = 0.2f;
        public float amount = 0.4f;
        public float phase;

        private Vector3 _base;
        private bool _has;

        private void Start() { _base = transform.localPosition; _has = true; }

        private void Update()
        {
            if (!_has) return;
            float t = (Time.time + phase) * speed;
            transform.localPosition = _base + new Vector3(Mathf.Sin(t) * amount, Mathf.Cos(t * 0.7f) * amount * 0.35f, 0f);
        }
    }
}
