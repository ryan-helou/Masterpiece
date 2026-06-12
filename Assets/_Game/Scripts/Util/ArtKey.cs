using UnityEngine;

namespace Velocity
{
    /// <summary>
    /// Loads an imported JPEG from Resources and keys out its background at runtime (the files have
    /// no real alpha), returning a transparent Sprite. Three modes cover what the image generator
    /// produced: a flat MAGENTA fill, a WHITE fill, or a gray CHECKERBOARD "transparency" pattern.
    ///
    /// Works without "Read/Write Enabled" on the import by blitting the texture to a temporary
    /// RenderTexture and reading that back. Results should be cached (see <see cref="LevelArt"/>).
    /// </summary>
    public static class ArtKey
    {
        public enum Mode { Auto, Magenta, White, Checker }

        /// <summary>Guess the background type from a corner pixel (top-right).</summary>
        private static Mode Detect(Color32 c)
        {
            float r = c.r / 255f, g = c.g / 255f, b = c.b / 255f;
            if (r > 0.55f && b > 0.45f && g < 0.5f) return Mode.Magenta;   // pink/magenta fill
            float mx = Mathf.Max(r, Mathf.Max(g, b)), mn = Mathf.Min(r, Mathf.Min(g, b));
            if (mx - mn < 0.12f) return mx > 0.82f ? Mode.White : Mode.Checker; // neutral: white vs grey
            return Mode.Magenta;
        }

        public static Sprite Load(string path, Mode mode, Vector2 pivot)
        {
            var src = Resources.Load<Texture2D>(path);
            if (src == null) return null;
            var tex = ToReadable(src);
            KeyOut(tex, mode);
            tex.Apply(false, false);
            var s = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), pivot, 100f);
            s.name = path;
            return s;
        }

        /// <summary>Slice a keyed sheet into cols×rows frames, left-to-right then top-to-bottom.</summary>
        public static Sprite[] LoadSheet(string path, Mode mode, int cols, int rows, Vector2 pivot)
        {
            var src = Resources.Load<Texture2D>(path);
            if (src == null) return null;
            var tex = ToReadable(src);
            KeyOut(tex, mode);
            tex.Apply(false, false);

            int fw = tex.width / cols, fh = tex.height / rows;
            var frames = new Sprite[cols * rows];
            int i = 0;
            for (int ry = 0; ry < rows; ry++)            // visual top row first
                for (int cx = 0; cx < cols; cx++)
                {
                    int y = tex.height - (ry + 1) * fh;  // texture origin is bottom-left
                    frames[i++] = Sprite.Create(tex, new Rect(cx * fw, y, fw, fh), pivot, 100f);
                }
            return frames;
        }

        /// <summary>Like Load, but crops tightly to the character (ignoring sparse stray pixels like a
        /// corner watermark) and pivots at <paramref name="pivot"/> of that crop — pass (0.5,0) for the
        /// feet so animation frames of different in-canvas sizes all plant on the same line.</summary>
        public static Sprite LoadTrimmed(string path, Mode mode, Vector2 pivot)
        {
            var src = Resources.Load<Texture2D>(path);
            if (src == null) return null;
            var tex = ToReadable(src);
            KeyOut(tex, mode);
            tex.Apply(false, false);

            var px = tex.GetPixels32();
            int w = tex.width, h = tex.height;
            var col = new int[w]; var row = new int[h];
            for (int y = 0; y < h; y++)
            {
                int yo = y * w;
                for (int x = 0; x < w; x++)
                    if (px[yo + x].a > 40) { col[x]++; row[y]++; }
            }
            // a column/row only counts as "the character" if it has real density (drops the watermark)
            int colT = Mathf.Max(3, Mathf.RoundToInt(h * 0.03f));
            int rowT = Mathf.Max(3, Mathf.RoundToInt(w * 0.03f));
            int minX = 0, maxX = w - 1, minY = 0, maxY = h - 1;
            while (minX < w && col[minX] < colT) minX++;
            while (maxX > minX && col[maxX] < colT) maxX--;
            while (minY < h && row[minY] < rowT) minY++;
            while (maxY > minY && row[maxY] < rowT) maxY--;

            Rect rect = (maxX <= minX || maxY <= minY)
                ? new Rect(0, 0, w, h)
                : new Rect(minX, minY, maxX - minX + 1, maxY - minY + 1);
            // anchor X at the IMAGE centre (stable across frames → the character doesn't slide
            // left/right when it flips or changes pose); Y from the caller (0 = feet).
            float pivX = Mathf.Clamp01((w * 0.5f - rect.x) / Mathf.Max(1f, rect.width));
            var s = Sprite.Create(tex, rect, new Vector2(pivX, pivot.y), 100f);
            s.name = path;
            return s;
        }

        private static Texture2D ToReadable(Texture2D src)
        {
            var rt = RenderTexture.GetTemporary(src.width, src.height, 0,
                RenderTextureFormat.ARGB32, RenderTextureReadWrite.sRGB);
            Graphics.Blit(src, rt);
            var prev = RenderTexture.active;
            RenderTexture.active = rt;
            var tex = new Texture2D(src.width, src.height, TextureFormat.RGBA32, false)
            {
                wrapMode = TextureWrapMode.Clamp,
                filterMode = FilterMode.Bilinear
            };
            tex.ReadPixels(new Rect(0, 0, src.width, src.height), 0, 0);
            tex.Apply();
            RenderTexture.active = prev;
            RenderTexture.ReleaseTemporary(rt);
            return tex;
        }

        private static void KeyOut(Texture2D tex, Mode mode)
        {
            var px = tex.GetPixels32();
            Color32 c0 = px[px.Length - 1];   // top-right corner = background sample
            if (mode == Mode.Auto) mode = Detect(c0);
            float kr = c0.r / 255f, kg = c0.g / 255f, kb = c0.b / 255f;

            for (int i = 0; i < px.Length; i++)
            {
                var p = px[i];
                float r = p.r / 255f, g = p.g / 255f, b = p.b / 255f;
                float mx = Mathf.Max(r, Mathf.Max(g, b));
                float mn = Mathf.Min(r, Mathf.Min(g, b));
                float sat = mx <= 0f ? 0f : (mx - mn) / mx;
                float a = 1f;

                switch (mode)
                {
                    case Mode.Magenta:
                    {
                        float d = Mathf.Sqrt((r - kr) * (r - kr) + (g - kg) * (g - kg) + (b - kb) * (b - kb));
                        a = Mathf.SmoothStep(0f, 1f, Mathf.InverseLerp(0.16f, 0.34f, d));
                        break;
                    }
                    case Mode.White:
                        // remove bright near-white background, keep saturated art
                        if (sat < 0.16f && mx > 0.7f) a = 0f;
                        else if (sat < 0.24f && mx > 0.7f) a = Mathf.InverseLerp(0.16f, 0.24f, sat);
                        break;

                    case Mode.Checker:
                    {
                        // the "transparency" checkerboard is NEUTRAL grey (R≈G≈B) at every brightness
                        // (light squares AND dark squares). Key by how grey a pixel is so we strip the
                        // whole grid while keeping coloured art and its warm/blue highlights.
                        // includes faint glow PAINTED over the checker (only slightly tinted grey),
                        // which is where the grid was bleeding through. Strongly-coloured cores survive.
                        float chroma = mx - mn;                 // 0 = perfectly grey
                        if (mx > 0.13f && chroma < 0.12f) a = 0f;
                        else if (mx > 0.13f && chroma < 0.20f) a = Mathf.InverseLerp(0.12f, 0.20f, chroma);
                        break;
                    }
                }

                // decontaminate magenta spill so keyed objects don't keep a pink edge fringe
                // (only pixels where BOTH red and blue exceed green — i.e. pink/magenta — are touched)
                float m = Mathf.Min(r, b) - g;
                if (m > 0f) { r = Mathf.Clamp01(r - m); b = Mathf.Clamp01(b - m); }

                p.r = (byte)(r * 255f);
                p.g = (byte)(g * 255f);
                p.b = (byte)(b * 255f);
                p.a = (byte)(Mathf.Clamp01(a) * 255f);
                px[i] = p;
            }
            tex.SetPixels32(px);
        }
    }
}
