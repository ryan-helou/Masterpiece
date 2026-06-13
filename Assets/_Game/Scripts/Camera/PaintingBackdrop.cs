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
        private float _camBaseY;                  // lowest camera Y (layers are authored for this view)
        private const float VerticalParallax = 0.25f;  // damp vertical drift so layers never leave the frame
        private Sprite _glow, _disc, _tri, _box;

        public void Setup(Transform cameraTransform, PaintingTheme theme, Bounds bounds)
        {
            _cam = cameraTransform;
            var camera = cameraTransform.GetComponent<Camera>();
            _halfH = camera != null ? camera.orthographicSize : 6.7f;
            _halfW = _halfH * (camera != null ? camera.aspect : 16f / 9f);
            _bMinX = bounds.min.x; _bMaxX = bounds.max.x; _bMinY = bounds.min.y;
            _camBaseY = _bMinY + _halfH;          // matches CameraController's bottom clamp

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

            // ---- atmosphere scrim: a sky-tinted veil OVER every backdrop layer, UNDER the
            // playfield. Without it the painted scenery competes with the platforms at full
            // contrast and the frame reads chaotic; one veil pushes the whole distance back
            // into the night and hands the contrast budget to the layer you actually play on.
            var scrimRoot = Locked("Scrim");
            Color veil = theme.Sky;
            veil.a = Mathf.Clamp01(theme.Scrim * 0.95f);
            Spr(scrimRoot, _box, veil, new Vector3(0f, 0f, 12f),
                new Vector2(_halfW * 2.8f, _halfH * 2.8f), -45);

            // ---- vignette: a breath of darkness at the frame's edges, over everything.
            // Center stays clean; the corners settle — the museum-frame focus that makes a
            // busy painted scene read as composed instead of wall-to-wall loud.
            var vinRoot = Locked("Vignette");
            Color edge = Color.Lerp(theme.Sky, Color.black, 0.7f);
            edge.a = 0.3f;
            Spr(vinRoot, TextureFactory.Vignette(96), edge, new Vector3(0f, 0f, -3f),
                new Vector2(_halfW * 2.15f, _halfH * 2.3f), 30);
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

        /// <summary>Deterministic pseudo-random in [0,1) from an index + salt — scatters decorations
        /// (height, scale, opacity, drift) without an RNG object, stable across rebuilds.</summary>
        private static float Hash01(int i, int salt)
        {
            float s = Mathf.Sin(i * 12.9898f + salt * 4.1414f) * 43758.5453f;
            return s - Mathf.Floor(s);
        }

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
                // sparser and quieter — these are huge, and at high alpha they shout over play
                var L = Par("Swirls", 0.9f);
                int i = 0;
                for (float x = _bMinX + 6f; x <= _bMaxX + 32f; x += 32f, i++)
                {
                    float y = _bMinY + _halfH * (1.5f + 0.45f * Mathf.Sin(i * 1.3f));
                    var sr = ArtSpr(L, swirl, new Color(1, 1, 1, 0.42f), new Vector3(x, y, 8f), 6.5f + (i % 2), -78);
                    sr.gameObject.AddComponent<Spinner>().degreesPerSecond = (i % 2 == 0) ? 4f : -3f;
                }
            }
            if (moon != null)
            {
                // ONE moon — the painting has one. Screen-locked (a real moon barely parallaxes)
                // but at deep SKY depth: it sits BEHIND the village, hills and cypress, not pasted
                // over them. Its own halo glows through the atmosphere scrim so it stays the
                // brightest thing in the sky — the single jewel, properly far away.
                var L = Locked("Moon");
                Vector3 mp = new Vector3(_halfW * 0.58f, _halfH * 0.52f, 9f);
                Spr(L, _glow, new Color(1f, 0.93f, 0.7f, 0.5f), mp, Vector2.one * 9f, -80);
                Pulse(ArtSpr(L, moon, Color.white, mp, 5f, -79), 0.8f, 0.04f, 0.1f, 0f);
            }
            if (star != null)
            {
                // a few HERO stars pierce the scrim at full glow; the rest twinkle behind it —
                // the "a lot and a little": a quiet field with a handful of bright accents
                var L = Par("Stars", 0.92f);
                for (int i = 0; i < 12; i++)
                {
                    float x = Random.Range(_bMinX - 4f, _bMaxX + 8f);
                    float y = Random.Range(_bMinY + _halfH * 1.3f, _bMinY + _halfH * 2.5f);
                    Pulse(ArtSpr(L, star, Color.white, new Vector3(x, y, 7f),
                            Random.Range(0.8f, 1.6f), i < 4 ? -44 : -74),
                        Random.Range(1.2f, 2.2f), 0.18f, 0.4f, i * 0.6f);
                }
            }
            var mist = LevelArt.Get("fg-mist");
            if (mist != null)
            {
                // a whisper-thin band drifting in FRONT of the action (factor > 1 moves against
                // the camera = foreground). Very low alpha so it never costs readability.
                var L = Par("Mist", 1.25f);
                float w = AspectWidth(mist, 2.4f);
                int i = 0;
                for (float x = _bMinX - w; x <= _bMaxX * 1.35f + w; x += w * 0.85f, i++)
                {
                    var sr = ArtSpr(L, mist, new Color(1f, 1f, 1f, 0.13f),
                        new Vector3(x, _bMinY + 2.8f, -2f), 2.4f, 14);
                    var sw = sr.gameObject.AddComponent<Sway>(); sw.speed = 0.25f; sw.degrees = 0.8f; sw.phase = i * 1.3f;
                }
            }
            var wind = LevelArt.Get("bg-wind");
            if (wind != null)
            {
                // the painting's rolling wind-river: a drifting band across the mid-sky, so the
                // view stays painterly when the level climbs high
                var L = Par("Wind", 0.84f);
                float w = AspectWidth(wind, 3.5f);
                int i = 0;
                for (float x = _bMinX; x <= _bMaxX + w; x += w * 0.9f, i++)
                {
                    float y = _bMinY + _halfH * (1.05f + 0.12f * ((i % 2) * 2 - 1));
                    var sr = ArtSpr(L, wind, new Color(1f, 1f, 1f, 0.38f), new Vector3(x, y, 8.5f), 3.5f, -72);
                    var sw = sr.gameObject.AddComponent<Sway>(); sw.speed = 0.35f; sw.degrees = 1.2f; sw.phase = i * 0.9f;
                }
            }
            var mtn = LevelArt.Get("bg-mountain");
            if (mtn != null)
            {
                var L = Par("Mountains", 0.7f);
                float w = AspectWidth(mtn, 5.5f);
                for (float x = _bMinX - w; x <= _bMaxX + w; x += w * 0.94f)
                    ArtSpr(L, mtn, Color.white, new Vector3(x, _bMinY - 0.4f, 9f), 5.5f, -70);
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
            if (LevelArt.Get("bg-rays") != null || LevelArt.Get("bg-clouds") != null) { FrescoArt(t); return; }

            Sky(t.SkyNear, t.SkyFar);
            var sky = Locked("Glow");
            Spr(sky, _glow, new Color(t.Glow.r, t.Glow.g, t.Glow.b, 0.5f),
                new Vector3(_halfW * 0.05f, _halfH * 0.3f, 20f), Vector2.one * (_halfH * 2.6f), -88);
            Horizon(t.Glow, 0.18f);
            Motes(new Color(1f, 0.92f, 0.8f, 1f), 5, 0.2f, 0.85f);
        }

        // ---- imported-art Fresco (Creation of Adam — divinity as light/cloth/architecture only) ----
        private void FrescoArt(PaintingTheme t)
        {
            Sky(t.SkyNear, t.SkyFar);

            var rays   = LevelArt.Get("bg-rays");
            var clouds = LevelArt.Get("bg-clouds");
            var drape  = LevelArt.Get("bg-drape");
            var arch   = LevelArt.Get("bg-arch");
            var mist   = LevelArt.Get("fg-mist");

            if (rays != null)
            {
                // the divine light pours from the upper-RIGHT — the direction of travel, so the
                // whole level is a run toward the light
                var L = Locked("Rays");
                var sr = ArtSpr(L, rays, new Color(1f, 1f, 1f, 0.5f),
                    new Vector3(_halfW * 0.45f, _halfH * 0.5f, 24f), _halfH * 1.7f, -86);
                Pulse(sr, 0.5f, 0.03f, 0.1f, 0f);
            }
            if (clouds != null)
            {
                var L = Par("Clouds", 0.8f);
                float w = AspectWidth(clouds, 4f);
                int i = 0;
                for (float x = _bMinX; x <= _bMaxX + w; x += w * 0.9f, i++)
                    ArtSpr(L, clouds, new Color(1f, 1f, 1f, 0.85f),
                        new Vector3(x, _bMinY + _halfH * (1.1f + 0.18f * Mathf.Sin(i * 1.7f)), 9f), 4f, -68);
            }
            if (drape != null)
            {
                // floating crimson silk — the cloak without its wearer
                var L = Par("Drapes", 0.55f);
                int i = 0;
                for (float x = _bMinX + 10f; x <= _bMaxX; x += 34f, i++)
                {
                    var holder = new GameObject("DrapeH");
                    holder.transform.SetParent(L, false);
                    holder.transform.localPosition = new Vector3(x, _bMinY + _halfH * 1.45f, 0f);
                    var sw = holder.AddComponent<Sway>(); sw.speed = 0.4f; sw.degrees = 3f; sw.phase = i * 1.1f;
                    ArtSpr(holder.transform, drape, new Color(1f, 1f, 1f, 0.9f), Vector3.zero, 6f, -60);
                }
            }
            if (arch != null)
            {
                var L = Par("Arches", 0.68f);
                float w = AspectWidth(arch, 5f);
                for (float x = _bMinX - w; x <= _bMaxX + w; x += w * 0.95f)
                    ArtSpr(L, arch, new Color(1f, 1f, 1f, 0.8f), new Vector3(x, _bMinY - 0.3f, 8f), 5f, -64);
            }
            if (mist != null)
            {
                // warm plaster-dust haze drifting in front of the action
                var L = Par("Mist", 1.25f);
                float w = AspectWidth(mist, 2.4f);
                int i = 0;
                for (float x = _bMinX - w; x <= _bMaxX * 1.35f + w; x += w * 0.85f, i++)
                {
                    var sr = ArtSpr(L, mist, new Color(1f, 0.96f, 0.88f, 0.12f),
                        new Vector3(x, _bMinY + 2.8f, -2f), 2.4f, 14);
                    var sw = sr.gameObject.AddComponent<Sway>(); sw.speed = 0.25f; sw.degrees = 0.8f; sw.phase = i * 1.3f;
                }
            }
            Motes(new Color(1f, 0.92f, 0.8f, 1f), 6, 0.2f, 0.85f);
        }

        private void Garden(PaintingTheme t)
        {
            if (LevelArt.Get("bg-canopy") != null || LevelArt.Get("bg-hedge") != null) { GardenArt(t); return; }

            Sky(t.SkyNear, t.Sky);
            var sky = Locked("Sun");
            Spr(sky, _glow, new Color(t.Glow.r, t.Glow.g, t.Glow.b, 0.4f),
                new Vector3(-_halfW * 0.65f, _halfH * 0.65f, 20f), Vector2.one * 7f, -88);
            // a clean dark tree-line (pines) along the bottom
            Ridge("Trees", 0.55f, t.Motif, _bMinY + 1.2f, 4.5f, 2.2f, 3.2f, 4f, 6f, -60);
            Motes(t.Player, 5, 0.2f, 0.82f);
        }

        // ---- imported-art Garden (Fragonard — the overgrown garden, the swing empty) ----
        private void GardenArt(PaintingTheme t)
        {
            Sky(t.SkyNear, t.Sky);

            var rays   = LevelArt.Get("bg-rays");
            var cloud  = LevelArt.Get("bg-clouds");
            var canopy = LevelArt.Get("bg-canopy");
            var swing  = LevelArt.Get("bg-swing");
            var hedge  = LevelArt.Get("bg-hedge");
            var fount  = LevelArt.Get("bg-fountain");
            var urn    = LevelArt.Get("bg-urn");
            var bloom  = LevelArt.Get("bg-bloom");
            var mist   = LevelArt.Get("fg-mist");

            if (rays != null)
            {
                // the painting's stolen sunlight: one warm shaft pouring from the upper-RIGHT —
                // the direction of travel, so the whole run is a sprint toward the light
                var L = Locked("Rays");
                var sr = ArtSpr(L, rays, new Color(1f, 1f, 1f, 0.45f),
                    new Vector3(_halfW * 0.4f, _halfH * 0.45f, 24f), _halfH * 1.8f, -86);
                Pulse(sr, 0.4f, 0.03f, 0.08f, 0f);
            }
            else
            {
                // no rays art — paint Fragonard's stolen sunlight procedurally: a warm bloom
                // from the upper-right so the garden still glows toward the goal
                var L = Locked("SunGlow");
                var sr = Spr(L, _glow, new Color(t.Glow.r, t.Glow.g, t.Glow.b, 0.5f),
                    new Vector3(_halfW * 0.5f, _halfH * 0.55f, 24f), Vector2.one * (_halfH * 2.8f), -86);
                Pulse(sr, 0.4f, 0.03f, 0.1f, 0f);
            }
            if (cloud != null)
            {
                // rococo cotton clouds in the high keyhole of sky the canopy leaves open
                var L = Par("Clouds", 0.88f);
                float w = AspectWidth(cloud, 3f);
                int i = 0;
                for (float x = _bMinX; x <= _bMaxX + w; x += w * 1.2f, i++)
                    ArtSpr(L, cloud, new Color(1f, 1f, 1f, 0.7f),
                        new Vector3(x, _bMinY + _halfH * (1.7f + 0.15f * Mathf.Sin(i * 1.9f)), 9f), 3f, -78);
            }
            if (canopy != null)
            {
                // the leaf ceiling — a continuous band hanging over everything (top pivot, so it
                // descends from above the frame even when the camera climbs the terraces)
                var L = Par("Canopy", 0.85f);
                float w = AspectWidth(canopy, 5f);
                int i = 0;
                for (float x = _bMinX - w; x <= _bMaxX + w; x += w * 0.9f, i++)
                {
                    var holder = new GameObject("CanopyH");
                    holder.transform.SetParent(L, false);
                    holder.transform.localPosition = new Vector3(x, _bMinY + _halfH * 2.55f, 0f);
                    var sw = holder.AddComponent<Sway>(); sw.speed = 0.3f; sw.degrees = 0.8f; sw.phase = i * 1.1f;
                    ArtSpr(holder.transform, canopy, Color.white, new Vector3(0f, 0f, 8.5f), 5f, -74);
                }
            }
            if (swing != null)
            {
                // THE swing — empty, still swaying, hung far back among the boughs. Sparse on
                // purpose: it should be found, not wallpapered. Top pivot = sways from the rope
                // anchor like a real pendulum, slow and a little mournful.
                var L = Par("Swing", 0.8f);
                int i = 0;
                for (float x = _bMinX + 20f; x <= _bMaxX; x += 64f, i++)
                {
                    var holder = new GameObject("SwingH");
                    holder.transform.SetParent(L, false);
                    holder.transform.localPosition = new Vector3(x, _bMinY + _halfH * 1.9f, 0f);
                    var sw = holder.AddComponent<Sway>(); sw.speed = 0.42f; sw.degrees = 6f; sw.phase = i * 2.3f;
                    ArtSpr(holder.transform, swing, new Color(1f, 1f, 1f, 0.85f), new Vector3(0f, 0f, 8f), 6.5f, -72);
                }
            }
            if (hedge != null)
            {
                // two hedge bands: a hazier far wall and a darker near one — the garden's maze
                // walls, rising from the floor line
                var LFar = Par("HedgeFar", 0.68f);
                float wF = AspectWidth(hedge, 4.5f);
                for (float x = _bMinX - wF; x <= _bMaxX + wF; x += wF * 0.92f)
                    ArtSpr(LFar, hedge, new Color(1f, 1f, 1f, 0.75f), new Vector3(x, _bMinY + 0.6f, 9f), 4.5f, -68);
                var LNear = Par("Hedge", 0.55f);
                float wN = AspectWidth(hedge, 3.2f);
                for (float x = _bMinX - wN + 4f; x <= _bMaxX + wN; x += wN * 0.95f)
                    ArtSpr(LNear, hedge, Color.white, new Vector3(x, _bMinY - 0.2f, 8f), 3.2f, -64);
            }
            if (fount != null)
            {
                // marble fountains punctuating the hedge line, far apart — landmarks, not noise
                var L = Par("Fountains", 0.6f);
                for (float x = _bMinX + 12f; x <= _bMaxX; x += 52f)
                    ArtSpr(L, fount, Color.white, new Vector3(x, _bMinY + 0.1f, 7.5f), 4.2f, -62);
            }
            if (urn != null)
            {
                // stone urns between the fountains, offset so the two never stack
                var L = Par("Urns", 0.5f);
                for (float x = _bMinX + 34f; x <= _bMaxX; x += 52f)
                    ArtSpr(L, urn, Color.white, new Vector3(x, _bMinY + 0.1f, 7f), 2.6f, -60);
            }
            if (bloom != null)
            {
                // rose clusters drifting at mid-height — the pink the painting is famous for
                var L = Par("Blooms", 0.45f);
                int i = 0;
                for (float x = _bMinX + 8f; x <= _bMaxX + 6f; x += 26f, i++)
                {
                    var holder = new GameObject("BloomH");
                    holder.transform.SetParent(L, false);
                    holder.transform.localPosition = new Vector3(x, _bMinY + 3.4f + 1.1f * Mathf.Sin(i * 2.4f), 0f);
                    var sw = holder.AddComponent<Sway>(); sw.speed = 0.5f; sw.degrees = 2.5f; sw.phase = i * 0.7f;
                    ArtSpr(holder.transform, bloom, Color.white, new Vector3(0f, 0f, 6f), 1.8f, -58);
                }
            }
            if (mist != null)
            {
                // drifting petals in FRONT of the action — barely-there, pure perfume
                var L = Par("Petals", 1.25f);
                float w = AspectWidth(mist, 2.2f);
                int i = 0;
                for (float x = _bMinX - w; x <= _bMaxX * 1.35f + w; x += w * 0.85f, i++)
                {
                    var sr = ArtSpr(L, mist, new Color(1f, 1f, 1f, 0.12f),
                        new Vector3(x, _bMinY + 2.6f, -2f), 2.2f, 14);
                    var sw = sr.gameObject.AddComponent<Sway>(); sw.speed = 0.28f; sw.degrees = 1.0f; sw.phase = i * 1.3f;
                }
            }
            Motes(t.Player, 6, 0.18f, 0.85f);
        }

        private void Scream(PaintingTheme t)
        {
            if (LevelArt.Get("bg-skyband") != null || LevelArt.Get("bg-fjord") != null) { ScreamArt(t); return; }

            Sky(t.SkyNear, t.Sky);
            var sky = Locked("Bands");
            // two smooth bands of fire — no clutter
            Spr(sky, _glow, new Color(t.Glow.r, t.Glow.g, t.Glow.b, 0.35f), new Vector3(0, _halfH * 0.45f, 20f), new Vector2(_halfW * 3.4f, 2.4f), -88);
            Spr(sky, _glow, new Color(t.Sky.r, t.Sky.g, t.Sky.b, 0.4f), new Vector3(0, _halfH * 0.0f, 20f), new Vector2(_halfW * 3.4f, 3f), -87);
            Horizon(t.Motif, 0.5f);
            Motes(t.Glow, 4, 0.2f, 0.85f);
        }

        // ---- imported-art Scream (Munch — the burning sky writhes, the fjord swallows) ----
        private void ScreamArt(PaintingTheme t)
        {
            Sky(t.SkyNear, t.Sky);

            var sky      = LevelArt.Get("bg-skyband");
            var fjord    = LevelArt.Get("bg-fjord");
            var headland = LevelArt.Get("bg-headland");
            var railing  = LevelArt.Get("bg-railing");
            var mist     = LevelArt.Get("fg-mist");

            if (sky != null)
            {
                // the famous bands of blood and fire writhing across the upper sky — two stacked
                // ribbons, the lower one slower, both undulating so the whole heaven seems to breathe
                var LHi = Par("SkyHi", 0.9f);
                float wH = AspectWidth(sky, 4.5f);
                int i = 0;
                for (float x = _bMinX - wH; x <= _bMaxX + wH; x += wH * 0.9f, i++)
                {
                    var sr = ArtSpr(LHi, sky, new Color(1f, 1f, 1f, 0.95f),
                        new Vector3(x, _bMinY + _halfH * 2.35f, 9f), 4.5f, -80);
                    var sw = sr.gameObject.AddComponent<Sway>(); sw.speed = 0.18f; sw.degrees = 1.1f; sw.phase = i * 1.2f;
                }
                var LLo = Par("SkyLo", 0.82f);
                float wL = AspectWidth(sky, 3.4f);
                i = 0;
                for (float x = _bMinX - wL + 5f; x <= _bMaxX + wL; x += wL * 0.92f, i++)
                {
                    var sr = ArtSpr(LLo, sky, new Color(1f, 1f, 1f, 0.8f),
                        new Vector3(x, _bMinY + _halfH * 1.78f, 9.5f), 3.4f, -78);
                    var sw = sr.gameObject.AddComponent<Sway>(); sw.speed = 0.13f; sw.degrees = 0.9f; sw.phase = i * 1.7f;
                }
            }
            if (headland != null)
            {
                // the dark promontory across the water — Munch's far shore, low and brooding;
                // bottom feathered so it dissolves into the fjord haze rather than floating
                var L = Par("Headland", 0.74f);
                float w = AspectWidth(headland, 3.6f);
                for (float x = _bMinX - w; x <= _bMaxX + w; x += w * 0.94f)
                    ArtSpr(L, headland, Color.white, new Vector3(x, _bMinY + 1.4f, 9f), 3.6f, -72);
            }
            if (fjord != null)
            {
                // the cold blue-black water heaving below the sky-fire — the death-colour of the
                // level, kept to the horizon band so it never creeps up into the playfield
                var L = Par("Fjord", 0.66f);
                float w = AspectWidth(fjord, 3f);
                int i = 0;
                for (float x = _bMinX - w; x <= _bMaxX + w; x += w * 0.92f, i++)
                {
                    var sr = ArtSpr(L, fjord, Color.white, new Vector3(x, _bMinY + 0.5f, 8.5f), 3f, -68);
                    var sw = sr.gameObject.AddComponent<Sway>(); sw.speed = 0.22f; sw.degrees = 0.7f; sw.phase = i * 1.5f;
                }
            }
            if (railing != null)
            {
                // the walkway railing knifing in from the left in steep perspective — the single
                // most iconic line in the painting. One long piece anchored to the lower-left,
                // riding close to the camera so its diagonal rakes across the frame.
                var L = Par("Railing", 0.5f);
                float w = AspectWidth(railing, 6f);
                for (float x = _bMinX - 2f; x <= _bMaxX + w; x += w * 0.98f)
                    ArtSpr(L, railing, new Color(1f, 1f, 1f, 0.9f), new Vector3(x, _bMinY + 1.0f, 7.5f), 6f, -60);
            }
            if (mist != null)
            {
                // a thin haze of the sky's colour drifting in FRONT — the anxiety made visible,
                // barely there so it never tints the gameplay read
                var L = Par("Haze", 1.22f);
                float w = AspectWidth(mist, 2.6f);
                int i = 0;
                for (float x = _bMinX - w; x <= _bMaxX * 1.35f + w; x += w * 0.85f, i++)
                {
                    var sr = ArtSpr(L, mist, new Color(1f, 0.86f, 0.6f, 0.11f),
                        new Vector3(x, _bMinY + 2.8f, -2f), 2.6f, 14);
                    var sw = sr.gameObject.AddComponent<Sway>(); sw.speed = 0.26f; sw.degrees = 1.1f; sw.phase = i * 1.3f;
                }
            }
            Motes(t.Glow, 5, 0.18f, 0.85f);
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
            if (LevelArt.Get("bg-fuji") != null || LevelArt.Get("bg-claw") != null) { WaveArt(t); return; }

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

        // ---- imported-art Great Wave (Hokusai — the sea looms, the boats are empty) ----
        private void WaveArt(PaintingTheme t)
        {
            Sky(t.SkyNear, t.Sky);

            var fuji  = LevelArt.Get("bg-fuji");
            var cloud = LevelArt.Get("bg-clouds");
            var swell = LevelArt.Get("bg-swell");
            var claw  = LevelArt.Get("bg-claw");
            var boat  = LevelArt.Get("bg-boat");
            var mist  = LevelArt.Get("fg-mist");

            if (fuji != null)
            {
                // the still center of the print — small, distant, serene while the sea rages;
                // sits low so its feathered haze base hides behind the swell band in front
                var L = Par("Fuji", 0.94f);
                int i = 0;
                for (float x = _bMinX + 14f; x <= _bMaxX + 28f; x += 56f, i++)
                    ArtSpr(L, fuji, new Color(1f, 1f, 1f, 0.9f), new Vector3(x, _bMinY + 2.2f, 9.5f), 4.5f, -80);
            }
            if (cloud != null)
            {
                var L = Par("Clouds", 0.86f);
                float w = AspectWidth(cloud, 2.8f);
                int i = 0;
                for (float x = _bMinX; x <= _bMaxX + w; x += w * 1.15f, i++)
                    ArtSpr(L, cloud, new Color(1f, 1f, 1f, 0.7f),
                        new Vector3(x, _bMinY + _halfH * (1.5f + 0.2f * Mathf.Sin(i * 2.1f)), 9f), 2.8f, -74);
            }
            if (swell != null)
            {
                // far rollers — a continuous heaving band along the horizon
                var L = Par("Swell", 0.7f);
                float w = AspectWidth(swell, 3f);
                int i = 0;
                for (float x = _bMinX - w; x <= _bMaxX + w; x += w * 0.92f, i++)
                {
                    var sr = ArtSpr(L, swell, Color.white, new Vector3(x, _bMinY + 1.6f, 8.5f), 3f, -68);
                    var sw = sr.gameObject.AddComponent<Sway>(); sw.speed = 0.3f; sw.degrees = 0.9f; sw.phase = i * 1.4f;
                }
            }
            if (boat != null)
            {
                // empty oshiokuri boats riding the far swell — pitched by the sea, nobody aboard.
                // Same parallax factor as the swell and claws: the sea moves as ONE body.
                var L = Par("Boats", 0.7f);
                int i = 0;
                for (float x = _bMinX + 18f; x <= _bMaxX; x += 42f, i++)
                {
                    var holder = new GameObject("BoatH");
                    holder.transform.SetParent(L, false);
                    holder.transform.localPosition = new Vector3(x, _bMinY + 2.4f, 0f);
                    var sw = holder.AddComponent<Sway>(); sw.speed = 0.45f; sw.degrees = 4f; sw.phase = i * 1.7f;
                    ArtSpr(holder.transform, boat, Color.white, Vector3.zero, 2.2f, -62);
                }
            }
            if (claw != null)
            {
                // the great wave cresting BEHIND the swell band (order -72 vs the swell's -68):
                // the rollers in front mask the claw image's rectangular bottom and flanks, so
                // only the iconic crest towers above the sea — the print's own composition.
                // Opaque (translucency read as a ghost poster), same parallax factor as the
                // swell so the sea moves as one body.
                var L = Par("Claws", 0.7f);
                int i = 0;
                for (float x = _bMinX + 10f; x <= _bMaxX + 12f; x += 44f, i++)
                {
                    var holder = new GameObject("ClawH");
                    holder.transform.SetParent(L, false);
                    holder.transform.localPosition = new Vector3(x, _bMinY - 0.6f, 0f);
                    var sw = holder.AddComponent<Sway>(); sw.speed = 0.25f; sw.degrees = 1.2f; sw.phase = i * 0.8f;
                    ArtSpr(holder.transform, claw, Color.white, new Vector3(0f, 0f, 8f), 6.0f + (i % 2) * 0.8f, -72);
                }
            }
            if (mist != null)
            {
                // the famous spray — flecks of foam drifting in FRONT of the action. Scattered, not
                // stamped: each clump gets its own height, scale, opacity, facing and drift so it reads
                // as wind-blown sea-spray instead of one repeating band.
                var L = Par("Spray", 1.25f);
                float w = AspectWidth(mist, 2.4f);
                float x = _bMinX - w;
                float xEnd = _bMaxX * 1.35f + w;
                int i = 0;
                while (x <= xEnd)
                {
                    float r1 = Hash01(i, 17), r2 = Hash01(i, 41), r3 = Hash01(i, 73), r4 = Hash01(i, 91);
                    float scale = Mathf.Lerp(1.5f, 3.1f, r1);            // big lazy clumps + small fast flecks
                    float y = _bMinY + Mathf.Lerp(1.7f, 4.7f, r2);       // drift across a vertical band, not one line
                    float alpha = Mathf.Lerp(0.07f, 0.17f, r3);          // some barely there, some catching light
                    float z = -2f - r4 * 1.5f;
                    var sr = ArtSpr(L, mist, new Color(1f, 1f, 1f, alpha), new Vector3(x, y, z), scale, 14);
                    if (r4 > 0.5f) { var s = sr.transform.localScale; s.x = -s.x; sr.transform.localScale = s; }  // mirror half
                    var sw = sr.gameObject.AddComponent<Sway>();
                    sw.speed = Mathf.Lerp(0.18f, 0.42f, r2); sw.degrees = Mathf.Lerp(0.7f, 1.8f, r1); sw.phase = r3 * 6.28f;
                    var dr = sr.gameObject.AddComponent<Drift>();
                    dr.speed = Mathf.Lerp(0.10f, 0.30f, r1); dr.amount = Mathf.Lerp(0.2f, 0.5f, r3); dr.phase = r4 * 6.28f;
                    x += w * Mathf.Lerp(0.45f, 0.95f, r1);               // uneven spacing: gusts and gaps
                    i++;
                }
            }
            Motes(new Color(0.96f, 0.97f, 0.95f, 1f), 6, 0.18f, 0.85f);
        }

        private void LateUpdate()
        {
            if (_cam == null) return;
            var p = _cam.position;
            foreach (var t in _locked) t.position = new Vector3(p.x, p.y, 0f);
            foreach (var l in _par)
            {
                // full parallax horizontally; vertically the layer rides almost 1:1 with the camera
                // (only a hint of drift), so climbing high never pushes the backdrop out of frame.
                float x = p.x * (1f - l.f);
                float y = _camBaseY * (1f - l.f) + (p.y - _camBaseY) * (1f - l.f * VerticalParallax);
                l.t.position = new Vector3(x, y, 0f);
            }
        }
    }
}
