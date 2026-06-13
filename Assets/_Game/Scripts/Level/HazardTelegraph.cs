using UnityEngine;

namespace Velocity
{
    /// <summary>
    /// Makes a sweeping saw READABLE: a glow behind the blade brightens as it swings toward the
    /// dangerous extreme of its arc (the part that crosses the player's path), and a soft tick
    /// fires once per cycle at that extreme — so the rhythm is learnable by ear and the hit never
    /// feels like a surprise. Reads <see cref="MovingPlatform.Extent"/>, so the cue is perfectly
    /// in step with the blade and deterministic from level start (consistent every run).
    /// </summary>
    public class HazardTelegraph : MonoBehaviour
    {
        public SpriteRenderer glow;
        public float peakAlpha = 0.55f;
        public bool verticalDanger = true;   // saws in a pit: the LOW extreme is the threat
        public float tickAt = 0.9f;           // fire the tick once the blade is this deep into the extreme

        private MovingPlatform _mp;
        private float _baseScale;
        private bool _armed = true;

        private void Start()
        {
            _mp = GetComponent<MovingPlatform>();
            if (glow != null) _baseScale = glow.transform.localScale.x;
        }

        private void Update()
        {
            if (_mp == null) return;

            // danger ramp: 0 (safe) .. 1 (at the dangerous extreme). For a pit saw the threat is
            // the low end (Extent < 0); a horizontal sweep is dangerous at either extreme.
            float e = _mp.Extent;
            float danger = verticalDanger ? Mathf.Clamp01(-e) : Mathf.Abs(e);

            if (glow != null)
            {
                var c = glow.color;
                c.a = danger * danger * peakAlpha;          // ease-in so it blooms late, near the strike
                glow.color = c;
                float s = _baseScale * (1f + 0.35f * danger);
                glow.transform.localScale = new Vector3(s, s, 1f);
            }

            // one tick per cycle, at the extreme
            if (_armed && danger >= tickAt)
            {
                _armed = false;
                if (Sfx.Instance != null) Sfx.Instance.PlayTick();
            }
            else if (danger < tickAt * 0.6f)
            {
                _armed = true;
            }
        }
    }
}
