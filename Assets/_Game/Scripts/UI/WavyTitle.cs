using UnityEngine;
using UnityEngine.UI;

namespace Velocity
{
    /// <summary>
    /// A playful animated banner. Each character becomes its own <see cref="Text"/>, laid out by the
    /// font's own glyph-advance widths (so the proportional serif still spaces correctly), then
    /// continuously color-cycled through a vivid shimmer, bobbed on a travelling sine wave, and given
    /// a gentle scale pulse. Built code-only like the rest of the UI — no prefabs, no TextMeshPro.
    /// Honors <see cref="Settings.ReduceMotion"/> by freezing to a static gilded gradient instead.
    /// </summary>
    public class WavyTitle : MonoBehaviour
    {
        private Text[] _cells;
        private float[] _baseX;
        private float _amp, _waveLen, _speed, _hueSpeed, _pulse, _t;

        /// <param name="amplitude">vertical bob in px</param>
        /// <param name="waveLength">phase offset per character (radians) — the travelling-wave look</param>
        /// <param name="speed">how fast the wave travels</param>
        /// <param name="hueSpeed">color-cycle rate</param>
        /// <param name="pulse">scale-pulse depth (0 = none)</param>
        public void Build(string text, int fontSize, float amplitude = 11f,
            float waveLength = 0.55f, float speed = 3.4f, float hueSpeed = 0.28f, float pulse = 0.10f)
        {
            _amp = amplitude; _waveLen = waveLength; _speed = speed;
            _hueSpeed = hueSpeed; _pulse = pulse;

            var font = UIFactory.Font;
            font.RequestCharactersInTexture(text, fontSize, FontStyle.Bold);

            // measure each glyph's advance so we can centre the whole line ourselves
            float total = 0f;
            var adv = new float[text.Length];
            for (int i = 0; i < text.Length; i++)
            {
                font.GetCharacterInfo(text[i], out CharacterInfo ci, fontSize, FontStyle.Bold);
                adv[i] = ci.advance > 0 ? ci.advance : fontSize * 0.4f; // fallback for spaces
                total += adv[i];
            }

            _cells = new Text[text.Length];
            _baseX = new float[text.Length];
            float x = -total / 2f;
            for (int i = 0; i < text.Length; i++)
            {
                var rt = UIFactory.Rect(transform, "Ch" + i);
                rt.anchorMin = rt.anchorMax = new Vector2(0.5f, 0.5f);
                rt.pivot = new Vector2(0.5f, 0.5f);
                rt.sizeDelta = new Vector2(Mathf.Max(adv[i], fontSize), fontSize * 1.7f);
                float cx = x + adv[i] * 0.5f;
                _baseX[i] = cx;
                rt.anchoredPosition = new Vector2(cx, 0f);
                x += adv[i];

                var t = rt.gameObject.AddComponent<Text>();
                t.font = font;
                t.fontSize = fontSize;
                t.fontStyle = FontStyle.Bold;
                t.alignment = TextAnchor.MiddleCenter;
                t.horizontalOverflow = HorizontalWrapMode.Overflow;
                t.verticalOverflow = VerticalWrapMode.Overflow;
                t.raycastTarget = false;
                t.text = text[i].ToString();

                // a dark drop-shadow so the bright colors read against the museum-navy wall
                var sh = rt.gameObject.AddComponent<Shadow>();
                sh.effectColor = new Color(0f, 0f, 0f, 0.55f);
                sh.effectDistance = new Vector2(2f, -3f);

                _cells[i] = t;
            }

            ApplyStatic();
        }

        private void Update()
        {
            if (_cells == null || Settings.ReduceMotion) return;
            _t += Time.unscaledDeltaTime;
            for (int i = 0; i < _cells.Length; i++)
            {
                float phase = _t * _speed - i * _waveLen;
                float wave = Mathf.Sin(phase);
                var rt = (RectTransform)_cells[i].transform;
                rt.anchoredPosition = new Vector2(_baseX[i], wave * _amp);
                float s = 1f + wave * _pulse;
                rt.localScale = new Vector3(s, s, 1f);
                _cells[i].color = Shimmer(Mathf.Repeat(_t * _hueSpeed + i * 0.045f, 1f));
            }
        }

        // bright, celebratory sweep — vivid but kept light enough to read on dark navy
        private static Color Shimmer(float hue) => Color.HSVToRGB(hue, 0.62f, 1f);

        // ReduceMotion fallback: a static left→right gilded gradient, no bob/pulse/cycle
        private void ApplyStatic()
        {
            var gilt = new Color(0.83f, 0.66f, 0.18f); // antique brass
            for (int i = 0; i < _cells.Length; i++)
            {
                float f = _cells.Length <= 1 ? 0f : (float)i / (_cells.Length - 1);
                _cells[i].color = Color.Lerp(gilt, Palette.UiGold, f);
                ((RectTransform)_cells[i].transform).anchoredPosition = new Vector2(_baseX[i], 0f);
            }
        }
    }
}
