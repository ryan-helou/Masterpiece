using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Velocity
{
    /// <summary>
    /// Eases a set of menu background layers on a slow autonomous sine so the gallery has depth and
    /// never sits dead-still. Each layer drifts by the shared wobble times its own parallax factor
    /// (far layers small, near layers large). No input dependency (works under either Input backend);
    /// frozen flat when <see cref="Settings.ReduceMotion"/> is on. Layers are registered after they're
    /// positioned, so each captures its own resting position as the base to drift around.
    /// </summary>
    public class MenuParallax : MonoBehaviour
    {
        private readonly List<RectTransform> _rt = new();
        private readonly List<Vector2> _base = new();
        private readonly List<float> _factor = new();

        public float ampX = 15f, ampY = 8f, speedX = 0.27f, speedY = 0.21f;

        public void Add(RectTransform rt, float factor)
        {
            if (rt == null) return;
            _rt.Add(rt);
            _base.Add(rt.anchoredPosition);
            _factor.Add(factor);
        }

        private void Update()
        {
            bool reduce = Settings.ReduceMotion;
            float t = Time.unscaledTime;
            float dx = reduce ? 0f : Mathf.Sin(t * speedX) * ampX;
            float dy = reduce ? 0f : Mathf.Sin(t * speedY + 1.3f) * ampY;
            var d = new Vector2(dx, dy);
            for (int i = 0; i < _rt.Count; i++)
                if (_rt[i] != null)
                    _rt[i].anchoredPosition = _base[i] + d * _factor[i];
        }
    }

    /// <summary>
    /// Fades a CanvasGroup in from transparent every time its object is shown — the menu's soft
    /// entrance. Snaps straight to visible under <see cref="Settings.ReduceMotion"/>. Uses unscaled
    /// time so it still plays while the game is paused (timeScale 0).
    /// </summary>
    [RequireComponent(typeof(CanvasGroup))]
    public class MenuFadeIn : MonoBehaviour
    {
        public float duration = 0.45f;
        private CanvasGroup _cg;

        private void Awake() { _cg = GetComponent<CanvasGroup>(); }

        private void OnEnable()
        {
            if (_cg == null) _cg = GetComponent<CanvasGroup>();
            if (_cg == null) return;
            if (Settings.ReduceMotion) { _cg.alpha = 1f; return; }
            StopAllCoroutines();
            StartCoroutine(Fade());
        }

        private IEnumerator Fade()
        {
            float t = 0f;
            _cg.alpha = 0f;
            while (t < duration)
            {
                t += Time.unscaledDeltaTime;
                _cg.alpha = Mathf.Clamp01(t / duration);
                yield return null;
            }
            _cg.alpha = 1f;
        }
    }
}
