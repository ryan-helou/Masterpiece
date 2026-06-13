using System.Collections.Generic;
using UnityEngine;

namespace Velocity
{
    /// <summary>A solid bounce block (Mario note block): land on top and you rocket upward.</summary>
    public class NoteBlock : MonoBehaviour
    {
        public float bounce = 18f;

        private Collider2D _col;
        private SpriteRenderer[] _flash;     // every visible renderer (own quad OR imported art tiles)
        private Color[] _base;
        private float _cooldown;

        private void Awake()
        {
            _col = GetComponent<Collider2D>();
        }

        private void Start()
        {
            // Collect the renderers to pulse on bounce. When the block carries imported art the
            // own quad is transparent (its art lives in child tiles), so flashing only the own
            // renderer would do nothing — gather every visible renderer and pulse them all.
            var all = GetComponentsInChildren<SpriteRenderer>();
            var list = new List<SpriteRenderer>();
            foreach (var s in all) if (s.color.a > 0.1f) list.Add(s);
            if (list.Count == 0 && all.Length > 0) list.Add(all[0]);
            _flash = list.ToArray();
            _base = new Color[_flash.Length];
            for (int i = 0; i < _flash.Length; i++) _base[i] = _flash[i].color;
        }

        private void Update()
        {
            if (_cooldown > 0f)
            {
                _cooldown -= Time.deltaTime;
                float t = 1f - _cooldown / 0.2f;
                for (int i = 0; i < _flash.Length; i++)
                    if (_flash[i] != null) _flash[i].color = Color.Lerp(Palette.GoalGlow, _base[i], t);
                return;
            }

            var b = _col.bounds;
            var hit = Physics2D.OverlapBox(new Vector2(b.center.x, b.max.y + 0.1f),
                new Vector2(b.size.x * 0.9f, 0.18f), 0f, 1 << GameLayers.Player);
            if (hit == null) return;
            var pc = hit.GetComponentInParent<PlayerController>();
            if (pc == null || pc.Velocity.y > 0.6f) return;     // only when landing/resting

            pc.LaunchUp(bounce);
            _cooldown = 0.2f;
            for (int i = 0; i < _flash.Length; i++)
                if (_flash[i] != null) _flash[i].color = Palette.GoalGlow;
            if (Sfx.Instance != null) Sfx.Instance.PlayNote();
        }
    }
}
