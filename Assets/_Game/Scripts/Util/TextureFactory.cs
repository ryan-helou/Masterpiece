using UnityEngine;

namespace Velocity
{
    /// <summary>
    /// Generates simple sprites at runtime (solid quads, rounded capsules, circles, triangles,
    /// rings) so the project needs no imported art. Everything is white and tinted via
    /// SpriteRenderer.color, except shapes that need baked alpha (rounded corners etc.).
    /// </summary>
    public static class TextureFactory
    {
        private static Sprite _whitePixel;

        /// <summary>A 1x1 white sprite at 1 pixel-per-unit => one world unit when scaled by 1.</summary>
        public static Sprite WhitePixel()
        {
            if (_whitePixel != null) return _whitePixel;
            var tex = new Texture2D(1, 1, TextureFormat.RGBA32, false)
            {
                filterMode = FilterMode.Point,
                wrapMode = TextureWrapMode.Clamp,
                name = "WhitePixel"
            };
            tex.SetPixel(0, 0, Color.white);
            tex.Apply();
            _whitePixel = Sprite.Create(tex, new Rect(0, 0, 1, 1), new Vector2(0.5f, 0.5f), 1f);
            _whitePixel.name = "WhitePixel";
            return _whitePixel;
        }

        /// <summary>Solid rounded-rectangle / capsule with baked anti-aliased alpha.</summary>
        public static Sprite RoundedBox(int w, int h, int radius, int ppu = 16)
        {
            var tex = NewTex(w, h);
            radius = Mathf.Clamp(radius, 0, Mathf.Min(w, h) / 2);
            for (int y = 0; y < h; y++)
            for (int x = 0; x < w; x++)
                tex.SetPixel(x, y, new Color(1, 1, 1, RoundedAlpha(x, y, w, h, radius)));
            tex.Apply();
            return ToSprite(tex, ppu);
        }

        public static Sprite Circle(int diameter, int ppu = 16)
        {
            return RoundedBox(diameter, diameter, diameter / 2, ppu);
        }

        /// <summary>Up-pointing triangle (used for spikes). Pivot at bottom-center.</summary>
        public static Sprite Triangle(int size, int ppu = 16)
        {
            var tex = NewTex(size, size);
            for (int y = 0; y < size; y++)
            for (int x = 0; x < size; x++)
            {
                float t = (float)y / (size - 1);              // 0 at base, 1 at tip
                float halfWidth = (1f - t) * (size / 2f);
                float dx = Mathf.Abs(x - (size - 1) / 2f);
                float a = Mathf.Clamp01(halfWidth - dx + 0.5f);
                tex.SetPixel(x, y, new Color(1, 1, 1, a));
            }
            tex.Apply();
            var s = Sprite.Create(tex, new Rect(0, 0, size, size), new Vector2(0.5f, 0f), ppu);
            s.name = "Triangle";
            return s;
        }

        /// <summary>Hollow ring (used for goal / flag glow).</summary>
        public static Sprite Ring(int diameter, int thickness, int ppu = 16)
        {
            var tex = NewTex(diameter, diameter);
            float c = (diameter - 1) / 2f;
            float outer = diameter / 2f;
            float inner = outer - thickness;
            for (int y = 0; y < diameter; y++)
            for (int x = 0; x < diameter; x++)
            {
                float d = Vector2.Distance(new Vector2(x, y), new Vector2(c, c));
                float a = Mathf.Clamp01(outer - d + 0.5f) * Mathf.Clamp01(d - inner + 0.5f);
                tex.SetPixel(x, y, new Color(1, 1, 1, a));
            }
            tex.Apply();
            return ToSprite(tex, ppu);
        }

        /// <summary>Vertical gradient that KEEPS each color's own alpha — for translucent
        /// shading overlays (e.g. darkening the soil band of a repeated terrain tile).</summary>
        public static Sprite VerticalFade(int w, int h, Color bottom, Color top)
        {
            var tex = NewTex(w, h);
            for (int y = 0; y < h; y++)
            {
                float f = h > 1 ? (float)y / (h - 1) : 1f;
                var col = Color.Lerp(bottom, top, f);
                for (int x = 0; x < w; x++) tex.SetPixel(x, y, col);
            }
            tex.Apply();
            return ToSprite(tex, 16);
        }

        /// <summary>Opaque vertical gradient (bottom→top). Used for skies and lit ground slabs.</summary>
        public static Sprite VerticalGradient(int w, int h, Color bottom, Color top)
        {
            var tex = NewTex(w, h);
            for (int y = 0; y < h; y++)
            {
                float f = h > 1 ? (float)y / (h - 1) : 1f;
                var col = Color.Lerp(bottom, top, f); col.a = 1f;
                for (int x = 0; x < w; x++) tex.SetPixel(x, y, col);
            }
            tex.Apply();
            return ToSprite(tex, 16);
        }

        /// <summary>Inverse radial mask: clear centre, alpha rising toward the corners — a
        /// screen vignette. Tint with the scene's darkest color at low alpha.</summary>
        public static Sprite Vignette(int size)
        {
            var tex = NewTex(size, size);
            float c = (size - 1) / 2f;
            for (int y = 0; y < size; y++)
                for (int x = 0; x < size; x++)
                {
                    float dx = (x - c) / c, dy = (y - c) / c;
                    float d = Mathf.Sqrt(dx * dx + dy * dy);
                    float a = Mathf.SmoothStep(0f, 1f, Mathf.InverseLerp(0.62f, 1.28f, d));
                    tex.SetPixel(x, y, new Color(1f, 1f, 1f, a));
                }
            tex.Apply();
            return ToSprite(tex, 16);
        }

        /// <summary>Soft radial glow (alpha 1 at the centre falling to 0 at the edge). Tint to taste —
        /// stars, moons, sun-pools, fog, foam, light shafts.</summary>
        public static Sprite RadialGlow(int size, float power = 2.2f)
        {
            var tex = NewTex(size, size);
            float c = (size - 1) / 2f, rad = size / 2f;
            for (int y = 0; y < size; y++)
            for (int x = 0; x < size; x++)
            {
                float d = Vector2.Distance(new Vector2(x, y), new Vector2(c, c));
                float a = Mathf.Pow(Mathf.Clamp01(1f - d / rad), power);
                tex.SetPixel(x, y, new Color(1, 1, 1, a));
            }
            tex.Apply();
            return ToSprite(tex, 16);
        }

        /// <summary>A sharp, shaded spike: bright near the tip, dark at the base, with a dark rim.
        /// White-baked value so a single SpriteRenderer tint (the hazard colour) reads as 3-D.
        /// Pivot at bottom-centre so it sits on a surface.</summary>
        public static Sprite Spike(int size = 24)
        {
            var tex = NewTex(size, size);
            float cx = (size - 1) / 2f;
            for (int y = 0; y < size; y++)
            for (int x = 0; x < size; x++)
            {
                float t = (float)y / (size - 1);              // 0 base → 1 tip
                float halfWidth = (1f - t) * (size / 2f) * 0.80f;   // narrow = sharp
                float dx = Mathf.Abs(x - cx);
                float cover = Mathf.Clamp01(halfWidth - dx + 0.5f);
                if (cover <= 0f) { tex.SetPixel(x, y, new Color(0, 0, 0, 0)); continue; }
                float val = Mathf.Lerp(0.45f, 1.15f, t);      // shade up toward the tip
                float edge = halfWidth - dx;                  // distance inside the slanted edge
                if (edge < 1.3f) val *= 0.55f;                // dark outline rim
                else if (x < cx) val = Mathf.Min(1.4f, val * 1.18f); // left-face highlight
                tex.SetPixel(x, y, new Color(val, val, val, cover));
            }
            tex.Apply();
            var s = Sprite.Create(tex, new Rect(0, 0, size, size), new Vector2(0.5f, 0f), 16);
            s.name = "Spike";
            return s;
        }

        private static Sprite _groundSlab;
        /// <summary>A 1×1 lit slab (brighter top, darker base) for ground. Tint by Palette.Ground.</summary>
        public static Sprite GroundSlab()
        {
            if (_groundSlab != null) return _groundSlab;
            _groundSlab = VerticalGradient(6, 24, new Color(0.6f, 0.6f, 0.6f), new Color(1.12f, 1.12f, 1.12f));
            _groundSlab.name = "GroundSlab";
            return _groundSlab;
        }

        /// <summary>A multi-arm logarithmic swirl (Van Gogh sky vortex). White-baked; tint + spin it.</summary>
        public static Sprite Swirl(int size, int arms = 2, float turns = 2.3f, float sharp = 2.4f)
        {
            var tex = NewTex(size, size);
            float c = (size - 1) / 2f;
            for (int y = 0; y < size; y++)
            for (int x = 0; x < size; x++)
            {
                float dx = (x - c) / c, dy = (y - c) / c;
                float r = Mathf.Sqrt(dx * dx + dy * dy);
                if (r > 1f) { tex.SetPixel(x, y, new Color(0, 0, 0, 0)); continue; }
                float theta = Mathf.Atan2(dy, dx);
                float arm = Mathf.Cos(arms * theta + turns * r * 6.2831853f);
                float band = Mathf.Pow(Mathf.Clamp01(arm), sharp);
                float fade = Mathf.Sin(Mathf.Clamp01(r) * Mathf.PI);  // 0 at centre & rim, 1 mid
                tex.SetPixel(x, y, new Color(1, 1, 1, band * fade));
            }
            tex.Apply();
            return ToSprite(tex, 16);
        }

        /// <summary>A 4-point sparkle with a bright core — star centres / glints.</summary>
        public static Sprite Sparkle(int size)
        {
            var tex = NewTex(size, size);
            float c = (size - 1) / 2f;
            for (int y = 0; y < size; y++)
            for (int x = 0; x < size; x++)
            {
                float dx = (x - c) / c, dy = (y - c) / c;
                float r = Mathf.Sqrt(dx * dx + dy * dy);
                float ray1 = Mathf.Clamp01(1f - Mathf.Abs(dy) / 0.10f) * Mathf.Clamp01(1f - Mathf.Abs(dx));
                float ray2 = Mathf.Clamp01(1f - Mathf.Abs(dx) / 0.10f) * Mathf.Clamp01(1f - Mathf.Abs(dy));
                float core = Mathf.Clamp01(1f - r / 0.3f);
                float a = Mathf.Clamp01(Mathf.Max(Mathf.Max(ray1, ray2), core));
                tex.SetPixel(x, y, new Color(1, 1, 1, a));
            }
            tex.Apply();
            return ToSprite(tex, 16);
        }

        /// <summary>A faceted crystal shard (lit left face, dark right face, dark rim, bright ridge).
        /// White-baked value so a single tint reads as a 3-D shard. Pivot bottom-centre.</summary>
        public static Sprite Crystal(int size = 28)
        {
            var tex = NewTex(size, size);
            float c = (size - 1) / 2f;
            for (int y = 0; y < size; y++)
            for (int x = 0; x < size; x++)
            {
                float t = (float)y / (size - 1);
                float halfW = (1f - t) * (size / 2f) * 0.66f;
                float dx = x - c;
                float cover = Mathf.Clamp01(halfW - Mathf.Abs(dx) + 0.5f);
                if (cover <= 0f) { tex.SetPixel(x, y, new Color(0, 0, 0, 0)); continue; }
                float val = Mathf.Lerp(0.5f, 1.12f, t);
                val *= dx < 0f ? 1f : 0.6f;                              // lit / shaded face
                if (Mathf.Abs(dx) > halfW - 1.3f) val *= 0.5f;          // dark rim
                else if (Mathf.Abs(dx) < 1.0f) val = Mathf.Min(1.35f, val * 1.25f); // centre ridge
                tex.SetPixel(x, y, new Color(val, val, val, cover));
            }
            tex.Apply();
            var s = Sprite.Create(tex, new Rect(0, 0, size, size), new Vector2(0.5f, 0f), 16);
            s.name = "Crystal";
            return s;
        }

        private static Sprite _brushBand;
        /// <summary>A 1×1 impasto slab: vertical light-to-dark gradient + wavy brush-stroke bands.
        /// Tint by Palette.Ground. Cached.</summary>
        public static Sprite BrushBand()
        {
            if (_brushBand != null) return _brushBand;
            int w = 24, h = 28;
            var tex = NewTex(w, h);
            for (int y = 0; y < h; y++)
            for (int x = 0; x < w; x++)
            {
                float g = Mathf.Lerp(0.58f, 1.1f, (float)y / (h - 1));
                float band = Mathf.Sin(y * 1.5f + Mathf.Sin(x * 0.22f) * 1.2f);
                float v = g * (0.92f + 0.08f * band);
                tex.SetPixel(x, y, new Color(v, v, v, 1f));
            }
            tex.Apply();
            _brushBand = ToSprite(tex, 16);
            _brushBand.name = "BrushBand";
            return _brushBand;
        }

        /// <summary>A filled 5-point star (anti-aliased), pointing up. Tint to taste.</summary>
        public static Sprite Star(int size, int points = 5)
        {
            var tex = NewTex(size, size);
            float c = (size - 1) / 2f;
            float outer = size * 0.47f, inner = outer * 0.42f;
            int n = points * 2;
            var vx = new float[n]; var vy = new float[n];
            for (int i = 0; i < n; i++)
            {
                float ang = -Mathf.PI / 2f + i * Mathf.PI / points;     // first point straight up
                float rad = (i % 2 == 0) ? outer : inner;
                vx[i] = c + Mathf.Cos(ang) * rad;
                vy[i] = c + Mathf.Sin(ang) * rad;
            }
            for (int y = 0; y < size; y++)
            for (int x = 0; x < size; x++)
            {
                float a = 0f;                                           // 2×2 supersample for smooth edges
                for (int sy = 0; sy < 2; sy++)
                    for (int sx = 0; sx < 2; sx++)
                        if (PointInPoly(x + 0.25f + sx * 0.5f, y + 0.25f + sy * 0.5f, vx, vy, n)) a += 0.25f;
                tex.SetPixel(x, y, new Color(1, 1, 1, a));
            }
            tex.Apply();
            return ToSprite(tex, 16);
        }

        /// <summary>A hollow star outline (anti-aliased) — the "empty slot" state for collectibles.
        /// Thickness is a fraction of the star's radius.</summary>
        public static Sprite StarOutline(int size, int points = 5, float thickness = 0.26f)
        {
            var tex = NewTex(size, size);
            float c = (size - 1) / 2f;
            float outer = size * 0.47f, inner = outer * 0.42f;
            float shrink = 1f - thickness;
            int n = points * 2;
            var ox = new float[n]; var oy = new float[n];
            var ix = new float[n]; var iy = new float[n];
            for (int i = 0; i < n; i++)
            {
                float ang = -Mathf.PI / 2f + i * Mathf.PI / points;
                float rad = (i % 2 == 0) ? outer : inner;
                ox[i] = c + Mathf.Cos(ang) * rad;
                oy[i] = c + Mathf.Sin(ang) * rad;
                ix[i] = c + Mathf.Cos(ang) * rad * shrink;
                iy[i] = c + Mathf.Sin(ang) * rad * shrink;
            }
            for (int y = 0; y < size; y++)
            for (int x = 0; x < size; x++)
            {
                float a = 0f;                                           // 2×2 supersample for smooth edges
                for (int sy = 0; sy < 2; sy++)
                    for (int sx = 0; sx < 2; sx++)
                    {
                        float px = x + 0.25f + sx * 0.5f, py = y + 0.25f + sy * 0.5f;
                        if (PointInPoly(px, py, ox, oy, n) && !PointInPoly(px, py, ix, iy, n)) a += 0.25f;
                    }
                tex.SetPixel(x, y, new Color(1, 1, 1, a));
            }
            tex.Apply();
            return ToSprite(tex, 16);
        }

        /// <summary>A 9-sliceable rounded box (set Image.type = Sliced) so one small texture
        /// stretches into a pill/panel of any size without distorting the corners.</summary>
        public static Sprite RoundedBoxSliced(int radius, int ppu = 16)
        {
            int s = radius * 2 + 6;
            var tex = NewTex(s, s);
            for (int y = 0; y < s; y++)
            for (int x = 0; x < s; x++)
                tex.SetPixel(x, y, new Color(1, 1, 1, RoundedAlpha(x, y, s, s, radius)));
            tex.Apply();
            var sp = Sprite.Create(tex, new Rect(0, 0, s, s), new Vector2(0.5f, 0.5f), ppu,
                0, SpriteMeshType.FullRect, new Vector4(radius + 1, radius + 1, radius + 1, radius + 1));
            sp.name = "RoundedBoxSliced";
            return sp;
        }

        private static bool PointInPoly(float px, float py, float[] vx, float[] vy, int n)
        {
            bool inside = false;
            for (int i = 0, j = n - 1; i < n; j = i++)
                if (((vy[i] > py) != (vy[j] > py)) &&
                    (px < (vx[j] - vx[i]) * (py - vy[i]) / (vy[j] - vy[i]) + vx[i]))
                    inside = !inside;
            return inside;
        }

        // ---- helpers -------------------------------------------------------

        private static float RoundedAlpha(int x, int y, int w, int h, int r)
        {
            if (r <= 0) return 1f;
            float cx = Mathf.Clamp(x, r, w - 1 - r);
            float cy = Mathf.Clamp(y, r, h - 1 - r);
            float d = Vector2.Distance(new Vector2(x, y), new Vector2(cx, cy));
            return Mathf.Clamp01(r - d + 0.5f);
        }

        private static Texture2D NewTex(int w, int h)
        {
            return new Texture2D(w, h, TextureFormat.RGBA32, false)
            {
                filterMode = FilterMode.Bilinear,
                wrapMode = TextureWrapMode.Clamp
            };
        }

        private static Sprite ToSprite(Texture2D tex, int ppu)
        {
            var s = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height),
                new Vector2(0.5f, 0.5f), ppu);
            return s;
        }
    }
}
