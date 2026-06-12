using UnityEngine;

namespace Velocity
{
    /// <summary>
    /// Kinematic platform that oscillates along an axis. Exposes its per-step Velocity so the
    /// PlayerController can inherit it while standing on top (order-independent carrying).
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D))]
    public class MovingPlatform : MonoBehaviour
    {
        public Vector2 axis = Vector2.right;
        public float distance = 2.5f;
        public float speed = 3f;
        public float phaseOffset = 0f;

        public Vector2 Velocity { get; private set; }

        private Rigidbody2D _rb;
        private Vector2 _origin;
        private float _t;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
            _rb.bodyType = RigidbodyType2D.Kinematic;
            _rb.interpolation = RigidbodyInterpolation2D.Interpolate;
            _origin = _rb.position;
            _t = phaseOffset;
        }

        private void FixedUpdate()
        {
            float w = speed / Mathf.Max(0.01f, distance);   // angular speed so peak ~= speed
            _t += w * Time.fixedDeltaTime;
            Vector2 newPos = _origin + axis.normalized * (Mathf.Sin(_t) * distance);
            Velocity = (newPos - _rb.position) / Time.fixedDeltaTime;
            _rb.MovePosition(newPos);
        }
    }
}
