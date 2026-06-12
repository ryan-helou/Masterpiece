using UnityEngine;
using UnityEngine.UI;

namespace Velocity
{
    /// <summary>
    /// Fixed-width text built from per-character cells. The built-in UI font is proportional, so a
    /// changing number (like a running timer) would visually jiggle; here every glyph occupies an
    /// identical slot, so the block stays rock-steady. Right-aligned within the cells.
    /// </summary>
    public class MonoText : MonoBehaviour
    {
        private Text[] _cells;
        private float[] _alphaMul;

        /// <summary>
        /// The last <paramref name="tailCells"/> cells (e.g. the ".mmm" of a timer) can be rendered
        /// at <paramref name="tailScale"/> of the main size and <paramref name="tailAlpha"/> opacity,
        /// dropped slightly so the digits share a visual baseline — the seconds stay dominant while
        /// the fast-flickering milliseconds recede.
        /// </summary>
        public void Build(int count, float cellWidth, float height, int fontSize, Color color,
            int tailCells = 0, float tailScale = 1f, float tailAlpha = 1f)
        {
            _cells = new Text[count];
            _alphaMul = new float[count];
            int head = count - tailCells;
            float tailWidth = cellWidth * tailScale;
            int tailFont = Mathf.RoundToInt(fontSize * tailScale);
            // digit centre sits ~half the cap-height above the baseline, so smaller glyphs
            // drop by roughly half the cap-height difference to line up
            float tailDrop = (fontSize - tailFont) * 0.34f;
            float totalW = head * cellWidth + tailCells * tailWidth;
            var rt = (RectTransform)transform;
            rt.sizeDelta = new Vector2(totalW, height);

            float x = -totalW / 2f;
            for (int i = 0; i < count; i++)
            {
                bool tail = i >= head;
                float w = tail ? tailWidth : cellWidth;
                var cellRt = UIFactory.Rect(transform, "Cell" + i);
                cellRt.anchorMin = new Vector2(0.5f, 0.5f);
                cellRt.anchorMax = new Vector2(0.5f, 0.5f);
                cellRt.pivot = new Vector2(0.5f, 0.5f);
                cellRt.sizeDelta = new Vector2(w, height);
                cellRt.anchoredPosition = new Vector2(x + w * 0.5f, tail ? -tailDrop : 0f);
                x += w;

                var t = cellRt.gameObject.AddComponent<Text>();
                t.font = UIFactory.Font;
                t.fontSize = tail ? tailFont : fontSize;
                _alphaMul[i] = tail ? tailAlpha : 1f;
                t.color = new Color(color.r, color.g, color.b, color.a * _alphaMul[i]);
                t.alignment = TextAnchor.MiddleCenter;
                t.fontStyle = FontStyle.Bold;
                t.horizontalOverflow = HorizontalWrapMode.Overflow;
                t.verticalOverflow = VerticalWrapMode.Overflow;
                t.raycastTarget = false;
                _cells[i] = t;
            }
        }

        public void SetText(string s)
        {
            if (_cells == null) return;
            int n = _cells.Length;
            // right-align: pad with leading spaces, keep the rightmost n chars
            if (s.Length < n) s = s.PadLeft(n);
            else if (s.Length > n) s = s.Substring(s.Length - n);
            for (int i = 0; i < n; i++)
                _cells[i].text = s[i].ToString();
        }

        public void SetColor(Color c)
        {
            if (_cells == null) return;
            for (int i = 0; i < _cells.Length; i++)
                _cells[i].color = new Color(c.r, c.g, c.b, c.a * _alphaMul[i]);
        }

        public void SetVisible(bool v)
        {
            if (_cells == null) return;
            foreach (var t in _cells) t.enabled = v;
        }
    }
}
