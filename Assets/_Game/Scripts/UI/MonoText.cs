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

        public void Build(int count, float cellWidth, float height, int fontSize, Color color)
        {
            _cells = new Text[count];
            float totalW = count * cellWidth;
            var rt = (RectTransform)transform;
            rt.sizeDelta = new Vector2(totalW, height);

            for (int i = 0; i < count; i++)
            {
                var cellRt = UIFactory.Rect(transform, "Cell" + i);
                cellRt.anchorMin = new Vector2(0.5f, 0.5f);
                cellRt.anchorMax = new Vector2(0.5f, 0.5f);
                cellRt.pivot = new Vector2(0.5f, 0.5f);
                cellRt.sizeDelta = new Vector2(cellWidth, height);
                cellRt.anchoredPosition = new Vector2(-totalW / 2f + cellWidth * (i + 0.5f), 0f);

                var t = cellRt.gameObject.AddComponent<Text>();
                t.font = UIFactory.Font;
                t.fontSize = fontSize;
                t.color = color;
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
            foreach (var t in _cells) t.color = c;
        }

        public void SetVisible(bool v)
        {
            if (_cells == null) return;
            foreach (var t in _cells) t.enabled = v;
        }
    }
}
