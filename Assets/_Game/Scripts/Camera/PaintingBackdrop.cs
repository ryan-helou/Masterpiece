using System.Collections.Generic;
using UnityEngine;

namespace Velocity
{
    /// <summary>
    /// A clean, minimal backdrop per painting: a palette-matched gradient sky, one mood element
    /// (moon / sun / spotlight / Mt Fuji) and at most one tidy geometric silhouette layer. Kept
    /// deliberately sparse so it reads well once the whole frame is pixelated. Rebuilt per level.
    /// </summary>
    public class PaintingBackdrop : MonoBehaviour
    {
        private Transform _cam;
        private readonly List<Transform> _locked = new List<Transform>();
        private struct PLayer { public Transform t; public float f; }
        private readonly List<PLayer> _par = new List<PLayer>();

        private float _halfW, _halfH, _bMinX, _bMaxX, _bMinY;
        private Sprite _glow, _disc, _tri, _box;

        public void Setup(Transform cameraTransform, PaintingTheme theme, Bounds bounds)
        {
            _cam = cameraTransform;
            var camera = cameraTransform.GetComponent<Camera>();
            _halfH = camera != null ? camera.orthographicSize : 6.7f;
            _halfW = _halfH * (camera != null ? camera.aspect : 16f / 9f);
            _bMinX = bounds.min.x; _bMaxX = bounds.max.x; _bMinY = bounds.min.y;

            _glow = TextureFactory.RadialGlow(64);
            _disc = TextureFactory.Circle(48, 24);
            _tri  = TextureFactory.Triangle(48, 16);
            _box  = TextureFactory.WhitePixel();

            if (theme == null) { Sky(Palette.BackdropNear, Palette.Background); return; }

            switch (theme.Style)
            {
                case BackdropStyle.StarryNight: StarryNight(theme); break;
                case BackdropStyle.Fresco:      Fresco(theme); break;
                case BackdropStyle.Garden:      Garden(theme); break;
                case BackdropStyle.Scream:      Scream(theme); break;
                case BackdropStyle.Alpine:      Alpine(theme); break;
                case BackdropStyle.Fog:         Fog(theme); break;
                case BackdropStyle.Chiaroscuro: Chiaroscuro(theme); break;
                case BackdropStyle.Wave:        Wave(theme); break;
            }
        }

        // ---- scaffolding ----
        private Transform Locked(string n)
        {
            var go = new GameObject(n); go.transform.SetParent(transform, false);
            _locked.Add(go.transform); return go.transform;
        }
        private Transform Par(string n, float f)
        {
            var go = new GameObject(n); go.transform.SetParent(transform, false);
            _par.Add(new PLayer { t = go.transform, f = f }); return go.transform;
        }
        private SpriteRenderer Spr(Transform parent, Sprite s, Color c, Vector3 lp, Vector2 size, int order, float rot = 0f)
        {
            var go = new GameObject("p"); go.transform.SetParent(parent, false);
            go.transform.localPosition = lp;
            go.transform.localRotation = Quaternion.Euler(0, 0, rot);
            var b = s.bounds.size;
            go.transform.localScale = new Vector3(size.x / Mathf.Max(1e-4f, b.x), size.y / Mathf.Max(1e-4f, b.y), 1f);
            var sr = go.AddComponent<SpriteRenderer>();
            sr.sprite = s; sr.color = c; sr.sortingOrder = order;
            return sr;
        }

        private Transform Sky(Color bottom, Color top)
        {
            var root = Locked("Sky");
            Spr(root, TextureFactory.VerticalGradient(8, 64, bottom, top), Color.white,
                new Vector3(0, 0, 30f), new Vector2(_halfW * 2.6f, _halfH * 2.6f), -95);
            return root;
        }

        /// <summary>A soft wide glow hugging the horizon — cheap depth, no clutter.</summary>
        private void Horizon(Color c, float alpha)
        {
            var root = Locked("Horizon");
            Spr(root, _glow, new Color(c.r, c.g, c.b, alpha),
                new Vector3(0, -_halfH * 0.65f, 20f), new Vector2(_halfW * 3.2f, _halfH * 1.6f), -90);
        }

        private void Stars(Transform sky, Color c, int count)
        {
            for (int i = 0; i < count; i++)
            {
                float x = Random.Range(-_halfW * 0.95f, _halfW * 0.95f);
                float y = Random.Range(0f, _halfH * 0.9f);
                float s = Random.Range(0.18f, 0.4f);
                Spr(sky, _glow, new Color(c.r, c.g, c.b, 0.4f), new Vector3(x, y, 21f), Vector2.one * (s * 3f), -84);
                Spr(sky, _disc, Color.Lerp(c, Color.white, 0.5f), new Vector3(x, y, 21f), Vector2.one * s, -83);
            }
        }

        /// <summary>One clean silhouette row of triangles (mountains / trees) across the level.</summary>
        private void Ridge(string name, float factor, Color c, float baseY, float spacing, float wMin, float wMax, float hMin, float hMax, int order)
        {
            var layer = Par(name, factor);
            for (float x = _bMinX - spacing; x <= _bMaxX + spacing; x += spacing)
                Spr(layer, _tri, c, new Vector3(x, baseY, 8f), new Vector2(Random.Range(wMin, wMax), Random.Range(hMin, hMax)), order);
        }

        private void Motes(Color c, int count, float size, float factor)
        {
            var layer = Par("Motes", factor);
            for (int i = 0; i < count; i++)
                Spr(layer, _glow, new Color(c.r, c.g, c.b, Random.Range(0.3f, 0.7f)),
                    new Vector3(Random.Range(_bMinX - 4f, _bMaxX + 4f), Random.Range(_bMinY, _bMinY + _halfH * 2.2f), 5f),
                    Vector2.one * (size * Random.Range(0.7f, 1.4f)), -50);
        }

        // ---- styles (sparse & clean) ----
        private void StarryNight(PaintingTheme t)
        {
            if (LevelArt.Get("bg-moon") != null || LevelArt.Get("bg-hills") != null) { StarryNightArt(t); return; }

            Sky(t.SkyNear, t.Sky);
            var sky = Locked("Night");
            var swirl = TextureFactory.Swirl(96, 2, 2.3f);
            var ring  = TextureFactory.Ring(28, 3, 16);
            var spark = TextureFactory.Sparkle(40);

            // slow-rotating sky vortices (the Van Gogh swirl)
            AddSwirl(sky, swirl, new Color(0.50f, 0.78f, 1f, 0.22f), new Vector3(-_halfW * 0.28f, _halfH * 0.45f, 22f), 5.0f,  4.5f);
            AddSwirl(sky, swirl, new Color(1f, 1f, 1f, 0.15f),       new Vector3( _halfW * 0.18f, _halfH * 0.26f, 22f), 5.5f, -3.4f);
            AddSwirl(sky, swirl, new Color(t.Glow.r, t.Glow.g, t.Glow.b, 0.16f), new Vector3(_halfW * 0.52f, _halfH * 0.6f, 22f), 4.5f, 2.6f);

            // crescent moon + halo (upper-right), pulsing
            Vector3 m = new Vector3(_halfW * 0.62f, _halfH * 0.55f, 20f);
            Spr(sky, _glow, new Color(t.Glow.r, t.Glow.g, t.Glow.b, 0.4f), m, Vector2.one * 6f, -88);
            var moon = Spr(sky, _disc, t.Glow, m, Vector2.one * 2.0f, -86);
            Spr(sky, _disc, t.Sky, m + new Vector3(0.75f, 0.4f, 0f), Vector2.one * 1.9f, -85);
            Pulse(moon, 1.0f, 0.05f, 0.12f, 0f);

            // radiant haloed stars
            for (int i = 0; i < 9; i++)
            {
                float x = Random.Range(-_halfW * 0.95f, _halfW * 0.95f);
                float y = Random.Range(_halfH * 0.05f, _halfH * 0.92f);
                RadiantStar(sky, ring, spark, t.Glow, new Vector3(x, y, 21f), Random.Range(0.5f, 1.1f), i * 0.7f);
            }

            // rolling hills (two parallax bands)
            var hillsFar = Par("HillsFar", 0.72f);
            for (float x = _bMinX - 10f; x <= _bMaxX + 10f; x += 8f)
                Spr(hillsFar, _disc, Color.Lerp(t.Motif, t.SkyNear, 0.28f), new Vector3(x, _bMinY + 1.6f, 8f), new Vector2(Random.Range(10f, 14f), Random.Range(3.5f, 5f)), -64);
            var hills = Par("Hills", 0.55f);
            for (float x = _bMinX - 10f; x <= _bMaxX + 10f; x += 7f)
                Spr(hills, _disc, t.Motif, new Vector3(x, _bMinY + 0.4f, 8f), new Vector2(Random.Range(10f, 14f), Random.Range(4f, 6f)), -62);

            // sleepy village + swaying cypress along the level
            var town = Par("Town", 0.5f);
            for (float x = _bMinX + 12f; x <= _bMaxX; x += 34f) Village(town, t, x, _bMinY + 1.0f);
            var fg = Par("Cypress", 0.42f);
            for (float x = _bMinX + 6f; x <= _bMaxX + 6f; x += 30f) Cypress(fg, t, x, _bMinY, x);

            Motes(t.Glow, 5, 0.2f, 0.85f);
        }

        private void AddSwirl(Transform parent, Sprite s, Color c, Vector3 lp, float size, float spin)
        {
            var sr = Spr(parent, s, c, lp, Vector2.one * size, -84);
            sr.gameObject.AddComponent<Spinner>().degreesPerSecond = spin;
        }

        private void Pulse(SpriteRenderer sr, float speed, float scaleAmp, float alphaAmp, float phase)
        {
            var tw = sr.gameObject.AddComponent<Twinkle>();
            tw.speed = speed; tw.scaleAmp = scaleAmp; tw.alphaAmp = alphaAmp; tw.phase = phase;
        }

        private void RadiantStar(Transform parent, Sprite ring, Sprite spark, Color c, Vector3 lp, float size, float phase)
        {
            Spr(parent, _glow, new Color(c.r, c.g, c.b, 0.4f), lp, Vector2.one * size * 2.6f, -84);
            Spr(parent, ring,  new Color(c.r, c.g, c.b, 0.45f), lp, Vector2.one * size * 1.5f, -83);
            var core = Spr(parent, spark, Color.Lerp(c, Color.white, 0.5f), lp, Vector2.one * size, -82);
            Pulse(core, 1.6f, 0.2f, 0.4f, phase);
        }

        private void Village(Transform layer, PaintingTheme t, float x, float baseY)
        {
            Color wall = Color.Lerp(t.Motif, t.Sky, 0.3f);
            Color win = new Color(1f, 0.82f, 0.4f, 1f);
            for (int i = 0; i < 4; i++)
            {
                float hx = x + i * 1.6f;
                float hh = 1.0f + (i % 2) * 0.5f;
                Spr(layer, _box, wall, new Vector3(hx, baseY + hh / 2f, 7f), new Vector2(1.3f, hh), -58);
                var wsr = Spr(layer, _box, win, new Vector3(hx, baseY + hh * 0.5f, 6f), Vector2.one * 0.22f, -56);
                Pulse(wsr, Random.Range(2f, 4f), 0.1f, 0.55f, x + i);
            }
            // church: taller nave + spire
            float cx = x + 4 * 1.6f;
            Spr(layer, _box, wall, new Vector3(cx, baseY + 1.3f, 7f), new Vector2(1.4f, 2.6f), -58);
            Spr(layer, _tri, wall, new Vector3(cx, baseY + 2.6f, 7f), new Vector2(1.0f, 1.6f), -58);
        }

        private void Cypress(Transform layer, PaintingTheme t, float x, float baseY, float phase)
        {
            var holder = new GameObject("Cypress");
            holder.transform.SetParent(layer, false);
            holder.transform.localPosition = new Vector3(x, baseY, 0f);
            var sw = holder.AddComponent<Sway>(); sw.speed = 0.7f; sw.degrees = 2.5f; sw.phase = phase;
            Color dark = Color.Lerp(t.Motif, Color.black, 0.45f);
            Spr(holder.transform, _tri, dark, new Vector3(0f, 0f, 6f), new Vector2(2.2f, 5.5f), -57);
            Spr(holder.transform, _tri, dark, new Vector3(0f, 3.0f, 6f), new Vector2(1.5f, 4.0f), -57);
            Spr(holder.transform, _tri, dark, new Vector3(0f, 5.6f, 6f), new Vector2(0.9f, 2.6f), -57);
        }

        // ---- imported-art Starry Night (parallax scrolls with the player) ----
        private float AspectWidth(Sprite s, float height) =>
            height * (s.bounds.size.x / Mathf.Max(1e-4f, s.bounds.size.y));

        private SpriteRenderer ArtSpr(Transform parent, Sprite s, Color c, Vector3 lp, float height, int order)
            => Spr(parent, s, c, lp, new Vector2(AspectWidth(s, height), height), order);

        private void StarryNightArt(PaintingTheme t)
        {
            Sky(t.SkyNear, t.Sky);

            var swirl = LevelArt.Get("bg-swirl");
            var moon  = LevelArt.Get("bg-moon");
            var star  = LevelArt.Get("bg-star");
            var hills = LevelArt.Get("bg-hills");
            var villg = LevelArt.Get("bg-village");
            var cyp   = LevelArt.Get("bg-cypress");

            if (swirl != null)
            {
                var L = Par("Swirls", 0.9f);
                int i = 0;
                for (float x = _bMinX; x <= _bMaxX + 24f; x += 24f, i++)
                {
                    float y = _bMinY + _halfH * (1.5f + 0.45f * Mathf.Sin(i * 1.3f));
                    var sr = ArtSpr(L, swirl, new Color(1, 1, 1, 0.6f), new Vector3(x, y, 8f), 7f + (i % 2), -78);
                    sr.gameObject.AddComponent<Spinner>().degreesPerSecond = (i % 2 == 0) ? 4f : -3f;
                }
            }
            if (moon != null)
            {
                var L = Par("Moon", 0.95f);
                int i = 0;
                for (float x = _bMinX + 8f; x <= _bMaxX + 24f; x += 44f, i++)
                    Pulse(ArtSpr(L, moon, Color.white, new Vector3(x, _bMinY + _halfH * 2.0f, 7f), 5.5f, -76), 0.8f, 0.04f, 0.1f, i);
            }
            if (star != null)
            {
                var L = Par("Stars", 0.92f);
                for (int i = 0; i < 14; i++)
                {
                    float x = Random.Range(_bMinX - 4f, _bMaxX + 8f);
                    float y = Random.Range(_bMinY + _halfH * 1.3f, _bMinY + _halfH * 2.5f);
                    Pulse(ArtSpr(L, star, Color.white, new Vector3(x, y, 7f), Random.Range(0.8f, 1.6f), -74),
                        Random.Range(1.2f, 2.2f), 0.18f, 0.4f, i * 0.6f);
                }
            }
            if (hills != null)
            {
                var L = Par("Hills", 0.6f);
                float w = AspectWidth(hills, 4.5f);
                for (float x = _bMinX - w; x <= _bMaxX + w; x += w * 0.92f)
                    ArtSpr(L, hills, Color.white, new Vector3(x, _bMinY - 0.5f, 8f), 4.5f, -66);
            }
            if (villg != null)
            {
                var L = Par("Village", 0.5f);
                float w = AspectWidth(villg, 5f);
                for (float x = _bMinX; x <= _bMaxX + w; x += w * 0.95f)
                    ArtSpr(L, villg, Color.white, new Vector3(x, _bMinY + 0.2f, 7f), 5f, -64);
            }
            if (cyp != null)
            {
                var L = Par("Cypress", 0.42f);
                int i = 0;
                for (float x = _bMinX + 6f; x <= _bMaxX + 8f; x += 26f, i++)
                {
                    var holder = new GameObject("CypH");
                    holder.transform.SetParent(L, false);
                    holder.transform.localPosition = new Vector3(x, _bMinY, 0f);
                    var sw = holder.AddComponent<Sway>(); sw.speed = 0.6f; sw.degrees = 2.2f; sw.phase = i;
                    ArtSpr(holder.transform, cyp, Color.white, new Vector3(0f, 0f, 6f), 7f, -62);
                }
            }
        }

        private void Fresco(PaintingTheme t)
        {
            Sky(t.SkyNear, t.SkyFar);
            var sky = Locked("Glow");
            Spr(sky, _glow, new Color(t.Glow.r, t.Glow.g, t.Glow.b, 0.5f),
                new Vector3(_halfW * 0.05f, _halfH * 0.3f, 20f), Vector2.one * (_halfH * 2.6f), -88);
            Horizon(t.Glow, 0.18f);
            Motes(new Color(1f, 0.92f, 0.8f, 1f), 5, 0.2f, 0.85f);
        }

        private void Garden(PaintingTheme t)
        {
            Sky(t.SkyNear, t.Sky);
            var sky = Locked("Sun");
            Spr(sky, _glow, new Color(t.Glow.r, t.Glow.g, t.Glow.b, 0.4f),
                new Vector3(-_halfW * 0.65f, _halfH * 0.65f, 20f), Vector2.one * 7f, -88);
            // a clean dark tree-line (pines) along the bottom
            Ridge("Trees", 0.55f, t.Motif, _bMinY + 1.2f, 4.5f, 2.2f, 3.2f, 4f, 6f, -60);
            Motes(t.Player, 5, 0.2f, 0.82f);
        }

        private void Scream(PaintingTheme t)
        {
            Sky(t.SkyNear, t.Sky);
            var sky = Locked("Bands");
            // two smooth bands of fire — no clutter
            Spr(sky, _glow, new Color(t.Glow.r, t.Glow.g, t.Glow.b, 0.35f), new Vector3(0, _halfH * 0.45f, 20f), new Vector2(_halfW * 3.4f, 2.4f), -88);
            Spr(sky, _glow, new Color(t.Sky.r, t.Sky.g, t.Sky.b, 0.4f), new Vector3(0, _halfH * 0.0f, 20f), new Vector2(_halfW * 3.4f, 3f), -87);
            Horizon(t.Motif, 0.5f);
            Motes(t.Glow, 4, 0.2f, 0.85f);
        }

        private void Alpine(PaintingTheme t)
        {
            Sky(t.SkyNear, t.Sky);
            Ridge("PeaksFar",  0.78f, Color.Lerp(t.Motif, t.SkyNear, 0.45f), _bMinY + 1.2f, 9f,  9f, 13f, 6f, 9f, -62);
            Ridge("PeaksNear", 0.55f, t.Motif,                              _bMinY + 0.3f, 11f, 11f, 16f, 8f, 12f, -60);
            Motes(Color.white, 7, 0.2f, 0.82f);
        }

        private void Fog(PaintingTheme t)
        {
            Sky(t.SkyNear, t.SkyFar);
            float[] f = { 0.82f, 0.62f };
            for (int row = 0; row < 2; row++)
            {
                Color mc = Color.Lerp(t.Glow, t.Motif, 0.4f + row * 0.4f);
                float baseY = _bMinY + (2.2f - row * 1.3f);
                Ridge("Ridge" + row, f[row], mc, baseY, 10f - row * 2f, 11f, 15f, 6f + row * 2f, 9f + row * 2f, -62 + row);
                var layer = Par("Fog" + row, f[row]);
                for (float x = _bMinX - 10f; x <= _bMaxX + 10f; x += 10f)
                    Spr(layer, _glow, new Color(t.Glow.r, t.Glow.g, t.Glow.b, 0.2f), new Vector3(x, baseY + 0.6f, 7f), new Vector2(11f, 2.4f), -60 + row);
            }
        }

        private void Chiaroscuro(PaintingTheme t)
        {
            Sky(t.Sky, t.SkyFar);
            var sky = Locked("Spot");
            Spr(sky, _glow, new Color(t.Glow.r, t.Glow.g, t.Glow.b, 0.42f),
                new Vector3(_halfW * 0.25f, _halfH * 0.1f, 20f), Vector2.one * (_halfH * 3.2f), -88);
            Vector3[] corners = { new Vector3(-_halfW, _halfH, 21f), new Vector3(_halfW, _halfH, 21f), new Vector3(-_halfW, -_halfH, 21f), new Vector3(_halfW, -_halfH, 21f) };
            foreach (var cn in corners)
                Spr(sky, _glow, new Color(t.Motif.r, t.Motif.g, t.Motif.b, 0.85f), cn, Vector2.one * (_halfH * 2.4f), -84);
            Motes(t.Glow, 6, 0.18f, 0.85f);
        }

        private void Wave(PaintingTheme t)
        {
            Sky(t.SkyNear, new Color(0.86f, 0.82f, 0.72f));
            var sky = Locked("Fuji");
            Vector3 f = new Vector3(_halfW * 0.05f, -_halfH * 0.12f, 20f);
            Spr(sky, _tri, t.Motif, f, new Vector2(9f, 6.5f), -88);
            Spr(sky, _tri, new Color(0.95f, 0.96f, 1f), f + new Vector3(0, 4.1f, 0f), new Vector2(3.2f, 2.2f), -87);
            // one clean wave band with a few foam caps
            var layer = Par("Swell", 0.55f);
            for (float x = _bMinX - 8f; x <= _bMaxX + 8f; x += 6f)
            {
                Spr(layer, _disc, t.Motif, new Vector3(x, _bMinY + 0.6f, 8f), new Vector2(8f, 5f), -60);
                Spr(layer, _disc, new Color(1f, 1f, 1f, 0.9f), new Vector3(x + 1.5f, _bMinY + 2.6f, 7f), Vector2.one * 0.7f, -58);
            }
            Motes(Color.white, 5, 0.2f, 0.82f);
        }

        private void LateUpdate()
        {
            if (_cam == null) return;
            var p = _cam.position;
            foreach (var t in _locked) t.position = new Vector3(p.x, p.y, 0f);
            foreach (var l in _par) { var q = p * (1f - l.f); l.t.position = new Vector3(q.x, q.y, 0f); }
        }
    }
}
