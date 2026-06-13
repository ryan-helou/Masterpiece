using UnityEngine;

namespace Velocity
{
    /// <summary>
    /// Smooth follow camera with velocity-based look-ahead and clamping to the level bounds.
    /// </summary>
    [RequireComponent(typeof(Camera))]
    public class CameraController : MonoBehaviour
    {
        public float smoothTime = 0.11f;
        public float lookAhead = 1.8f;
        public float lookAheadSmooth = 6f;
        public Vector2 offset = new Vector2(0f, 1.0f);
        public float lookDown = 1.3f;        // bias down when falling fast (see the landing)
        public float lookUp = 0.45f;         // slight bias up when rising fast
        public float vLookSmooth = 2.8f;
        public float fallLookDelay = 0.3f;   // only real drops engage look-down, never jump arcs
        public float maxShake = 0.6f;
        public float shakeDecay = 1.6f;

        [Header("Fling ease-out")]
        public float flingSpeed = 15f;       // speed where the camera starts pulling back
        public float flingSpeedMax = 27f;    // speed at full pull-back (boost-pad fling ~25)
        public float flingZoom = 0.12f;      // extra orthographic size at full fling (+12%)
        public float flingLook = 2.4f;       // extra look-ahead units at full fling
        public float flingSmooth = 3.5f;     // how fast zoom/look ease in and out

        private Camera _cam;
        private Transform _target;
        private PlayerController _pc;
        private Vector3 _vel;
        private Vector3 _pos;          // smoothed base position (before shake)
        private float _curLookAhead;
        private float _curLookY;
        private float _fallTime;
        private float _baseOrtho;      // resting zoom; flings ease outward from this
        private float _fling;          // 0..1 eased fling amount
        private float _trauma;
        private float _shakeSeed;

        private Bounds _bounds;
        private bool _hasBounds;

        private void Awake()
        {
            _cam = GetComponent<Camera>();
            _pos = transform.position;
            _shakeSeed = 12.34f;
            _baseOrtho = _cam.orthographicSize;
        }

        /// <summary>Add screenshake. 0..1 trauma; the effect scales with trauma squared.</summary>
        public void AddShake(float amount)
        {
            if (!Settings.ScreenShake || Settings.ReduceMotion) return;
            _trauma = Mathf.Clamp01(_trauma + amount);
        }

        public void SetTarget(Transform t)
        {
            _target = t;
            _pc = t != null ? t.GetComponent<PlayerController>() : null;
        }

        public void SetBounds(Bounds b)
        {
            _bounds = b;
            _hasBounds = true;
        }

        public void ClearBounds() => _hasBounds = false;

        public void Snap()
        {
            if (_target == null) return;
            _curLookAhead = 0f;
            _vel = Vector3.zero;
            _pos = Clamp(DesiredCenter());
            transform.position = _pos;
        }

        private void LateUpdate()
        {
            if (_target == null) return;

            // fling ease-out: at high speed (boost pad, dash, glide, ring) the camera pulls back
            // and leads further so you can read where you're being thrown, then eases back in
            float speed = _pc != null ? _pc.Velocity.magnitude : 0f;
            float flingT = Mathf.Clamp01((speed - flingSpeed) / Mathf.Max(0.01f, flingSpeedMax - flingSpeed));
            _fling = Mathf.Lerp(_fling, flingT, flingSmooth * Time.deltaTime);
            if (_cam.orthographic)
                _cam.orthographicSize = _baseOrtho * (1f + flingZoom * _fling);

            float reach = lookAhead + flingLook * _fling;
            float targetLook = 0f;
            if (_pc != null && Mathf.Abs(_pc.Velocity.x) > 0.5f)
                targetLook = Mathf.Sign(_pc.Velocity.x) * reach *
                             Mathf.Clamp01(Mathf.Abs(_pc.Velocity.x) / 10f);
            _curLookAhead = Mathf.Lerp(_curLookAhead, targetLook, lookAheadSmooth * Time.deltaTime);

            // bias vertically toward where you're heading so you can read the landing.
            // Gated behind a sustained-fall timer: an ordinary jump's fall phase (~0.25s) never
            // engages it, so the camera doesn't bob on every hop — that read as "shaking" in
            // the elevated sections, where the bounds clamp no longer hides vertical motion.
            float targetLookY = 0f;
            if (_pc != null)
            {
                float vy = _pc.Velocity.y;
                _fallTime = vy < -6f ? _fallTime + Time.deltaTime : 0f;
                if (vy < -9f && _fallTime > fallLookDelay)
                    targetLookY = -lookDown * Mathf.Clamp01((-vy - 9f) / 16f);
                else if (vy > 9f)
                    targetLookY = lookUp * Mathf.Clamp01((vy - 9f) / 16f);
            }
            _curLookY = Mathf.Lerp(_curLookY, targetLookY, vLookSmooth * Time.deltaTime);

            Vector3 desired = Clamp(DesiredCenter());
            _pos = Vector3.SmoothDamp(_pos, desired, ref _vel, smoothTime);

            // shake applied on top of the base position (no feedback into the smoothing)
            Vector3 shake = Vector3.zero;
            if (_trauma > 0f)
            {
                float s = _trauma * _trauma;
                float time = Time.unscaledTime * 28f;
                shake.x = (Mathf.PerlinNoise(_shakeSeed, time) - 0.5f) * 2f * s * maxShake;
                shake.y = (Mathf.PerlinNoise(_shakeSeed + 5f, time) - 0.5f) * 2f * s * maxShake;
                _trauma = Mathf.Max(0f, _trauma - shakeDecay * Time.unscaledDeltaTime);
            }

            transform.position = _pos + shake;
        }

        private Vector3 DesiredCenter()
        {
            Vector3 p = _target.position + (Vector3)offset;
            p.x += _curLookAhead;
            p.y += _curLookY;
            p.z = -10f;
            return p;
        }

        private Vector3 Clamp(Vector3 p)
        {
            if (!_hasBounds) { p.z = -10f; return p; }

            float halfH = _cam.orthographicSize;
            float halfW = halfH * _cam.aspect;

            float minX = _bounds.min.x + halfW;
            float maxX = _bounds.max.x - halfW;
            float minY = _bounds.min.y + halfH;
            float maxY = _bounds.max.y - halfH;

            p.x = minX <= maxX ? Mathf.Clamp(p.x, minX, maxX) : _bounds.center.x;
            p.y = minY <= maxY ? Mathf.Clamp(p.y, minY, maxY) : _bounds.center.y;
            p.z = -10f;
            return p;
        }
    }
}
