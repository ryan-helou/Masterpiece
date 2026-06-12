using System.Collections.Generic;
using UnityEngine;

namespace Velocity
{
    public struct BuiltLevel
    {
        public Transform Root;
        public Vector3 Spawn;
        public Bounds Bounds;
        public int CoinCount;
    }

    /// <summary>
    /// Turns a <see cref="LevelDefinition"/> ASCII grid into live GameObjects: merged ground
    /// strips (clean collision), spikes, hazard blocks, one-way platforms, moving platforms and
    /// the goal. Cell size is 1 world unit; grid origin is the bottom-left at (0,0).
    /// </summary>
    public static class LevelBuilder
    {
        public static BuiltLevel Build(LevelDefinition def, Transform parent)
        {
            var rows = def.Rows;
            int h = rows.Length;
            int w = 0;
            foreach (var r in rows) w = Mathf.Max(w, r.Length);

            var root = new GameObject("Level_" + def.Id).transform;
            if (parent != null) root.SetParent(parent, false);

            Vector3 spawn = new Vector3(1.5f, 2f, 0f);

            char At(int c, int r) =>
                (r < 0 || r >= h || c < 0 || c >= rows[r].Length) ? ' ' : rows[r][c];

            float Wx(int c) => c + 0.5f;
            float Wy(int r) => (h - 1 - r) + 0.5f;     // cell-center world Y

            // --- solid ground: merge horizontal runs per row ---
            MergeRuns(w, h, At, '#', (start, r, len) =>
            {
                CreateSolid(root, start, r, len, Wx, Wy, At);
                if (LevelArt.Get("ground") == null)
                    CreateCaps(root, start, len, r, At, Wx, Wy);
            });
            // invisible world-edge bounds (collision, no art) and low duck-tunnel ceilings
            MergeRuns(w, h, At, 'B', (start, r, len) => CreateBoundary(root, start, r, len, Wy));
            MergeRuns(w, h, At, 'T', (start, r, len) => CreateDuckCeiling(root, start, r, len, Wx, Wy));
            // crumbling platforms and conveyors also merge into single strips
            MergeRuns(w, h, At, 'c', (start, r, len) => CreateCrumble(root, start, r, len, Wy));
            MergeRuns(w, h, At, '>', (start, r, len) => CreateConveyor(root, start, r, len, Wy, +1));
            MergeRuns(w, h, At, '<', (start, r, len) => CreateConveyor(root, start, r, len, Wy, -1));
            MergeRuns(w, h, At, 'l', (start, r, len) => CreateFallingPlatform(root, start, r, len, Wy));
            MergeRuns(w, h, At, 'n', (start, r, len) => CreateNoteBlock(root, start, r, len, Wy));
            // a continuous death-pool strip across each spike run (replaces the spikes visually)
            if (LevelArt.Get("pool") != null)
                MergeRuns(w, h, At, '^', (start, r, len) => CreatePoolStrip(root, start, r, len, Wy));

            // --- single-cell entities ---
            int coins = 0;
            for (int r = 0; r < h; r++)
            {
                for (int c = 0; c < w; c++)
                {
                    switch (At(c, r))
                    {
                        case 'P': spawn = new Vector3(Wx(c), Wy(r) + 0.05f, 0f); break;
                        case 'G': CreateGoal(root, c, r, Wx, Wy); break;
                        case '^': CreateSpike(root, c, r, Wx, Wy); break;
                        case 'x': CreateHazardBlock(root, c, r, Wx, Wy); break;
                        case '-': CreateOneWay(root, c, r, Wx, Wy); break;
                        case 'M': CreateMoving(root, c, r, Wx, Wy, Vector2.right); break;
                        case 'V': CreateMoving(root, c, r, Wx, Wy, Vector2.up); break;
                        case 'o': CreateCoin(root, c, r, Wx, Wy); coins++; break;
                        case 'b': CreateSpring(root, c, r, Wx, Wy); break;
                        case 's': CreateSaw(root, c, r, Wx, Wy, Vector2.right); break;
                        case 'S': CreateSaw(root, c, r, Wx, Wy, Vector2.up); break;
                        case 'g': CreateEnemy(root, c, r, Wx, Wy, EnemyKind.Walker); break;
                        case 'f': CreateEnemy(root, c, r, Wx, Wy, EnemyKind.Flyer); break;
                        case 'y': CreateEnemy(root, c, r, Wx, Wy, EnemyKind.Spiky); break;
                        case 'z': CreateEnemy(root, c, r, Wx, Wy, EnemyKind.Diver); break;
                        case 'd': CreateDashOrb(root, c, r, Wx, Wy); break;
                        case 'j': CreateJumpRing(root, c, r, Wx, Wy); break;
                        case 'k': CreateBoostPad(root, c, r, Wx, Wy, new Vector2(25f, 16f)); break;
                        case 'K': CreateBoostPad(root, c, r, Wx, Wy, new Vector2(0f, 31f)); break;
                        case 'w': CreateFeather(root, c, r, Wx, Wy); break;
                        case 'u': CreateJumpPower(root, c, r, Wx, Wy); break;
                    }
                }
            }

            // paired warp portals (p/q/r) linked after all positions are known
            LinkPortals(root, w, h, At, Wx, Wy);

            var bounds = new Bounds(new Vector3(w / 2f, h / 2f, 0f), new Vector3(w, h, 1f));
            return new BuiltLevel { Root = root, Spawn = spawn, Bounds = bounds, CoinCount = coins };
        }

        // ---- builders ------------------------------------------------------

        private static SpriteRenderer Quad(Transform parent, string name, Vector3 center,
            Vector2 size, Color color, int order)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent, false);
            go.transform.position = center;
            go.transform.localScale = new Vector3(size.x, size.y, 1f);
            var sr = go.AddComponent<SpriteRenderer>();
            sr.sprite = TextureFactory.WhitePixel();
            sr.color = color;
            sr.sortingOrder = order;
            return sr;
        }

        /// <summary>Like Quad but with an arbitrary sprite, scaled so it exactly fills <paramref name="size"/>.</summary>
        private static SpriteRenderer QuadSprite(Transform parent, string name, Vector3 center,
            Vector2 size, Sprite sprite, Color color, int order, float rot = 0f)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent, false);
            go.transform.position = center;
            go.transform.localRotation = Quaternion.Euler(0, 0, rot);
            var b = sprite.bounds.size;
            go.transform.localScale = new Vector3(size.x / Mathf.Max(1e-4f, b.x), size.y / Mathf.Max(1e-4f, b.y), 1f);
            var sr = go.AddComponent<SpriteRenderer>();
            sr.sprite = sprite;
            sr.color = color;
            sr.sortingOrder = order;
            return sr;
        }

        /// <summary>A small decorative sprite placed by LOCAL position/scale (for layered icons).</summary>
        private static void Decal(Transform parent, Sprite s, Color c, float scale, Vector3 localPos, int order)
        {
            var go = new GameObject("Decal");
            go.transform.SetParent(parent, false);
            go.transform.localPosition = localPos;
            go.transform.localScale = Vector3.one * scale;
            var sr = go.AddComponent<SpriteRenderer>();
            sr.sprite = s; sr.color = c; sr.sortingOrder = order;
        }

        /// <summary>Place an imported art sprite at a LOCAL position, scaled to a world HEIGHT
        /// (aspect preserved). The sprite's own pivot decides how it sits at that point.</summary>
        private static SpriteRenderer ArtPiece(Transform parent, string name, Sprite s,
            Vector3 localPos, float height, int order)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent, false);
            go.transform.localPosition = localPos;
            float scale = height / Mathf.Max(1e-4f, s.bounds.size.y);
            go.transform.localScale = new Vector3(scale, scale, 1f);
            var sr = go.AddComponent<SpriteRenderer>();
            sr.sprite = s; sr.color = Color.white; sr.sortingOrder = order;
            return sr;
        }

        private static void MergeRuns(int w, int h, System.Func<int, int, char> At, char ch,
            System.Action<int, int, int> make)
        {
            for (int r = 0; r < h; r++)
            {
                int c = 0;
                while (c < w)
                {
                    if (At(c, r) == ch)
                    {
                        int start = c;
                        while (c < w && At(c, r) == ch) c++;
                        make(start, r, c - start);
                    }
                    else c++;
                }
            }
        }

        private static void CreateCrumble(Transform root, int start, int r, int len,
            System.Func<int, float> Wy)
        {
            float cx = start + len / 2f;
            var sr = Quad(root, "Crumble", new Vector3(cx, Wy(r), 0f), new Vector2(len, 1f),
                Color.Lerp(Palette.Ground, Palette.Hazard, 0.35f), 0);
            var col = sr.gameObject.AddComponent<BoxCollider2D>();
            col.size = Vector2.one;
            sr.gameObject.layer = GameLayers.Ground;
            sr.gameObject.AddComponent<Crumble>();
        }

        private static void CreatePoolStrip(Transform root, int start, int r, int len,
            System.Func<int, float> Wy)
        {
            var pool = LevelArt.Get("pool");
            if (pool == null) return;
            float cx = start + len / 2f;
            // foam surface sits ~at the pit floor; the void body extends below, one strip per run
            QuadSprite(root, "PoolStrip", new Vector3(cx, Wy(r) - 0.3f, 0f),
                new Vector2(len + 0.04f, 2.2f), pool, Color.white, 5);
        }

        private static void CreateConveyor(Transform root, int start, int r, int len,
            System.Func<int, float> Wy, int dir)
        {
            float cx = start + len / 2f;
            var sr = Quad(root, "Conveyor", new Vector3(cx, Wy(r), 0f), new Vector2(len, 1f),
                Palette.Moving, 0);
            var col = sr.gameObject.AddComponent<BoxCollider2D>();
            col.size = Vector2.one;
            sr.gameObject.layer = GameLayers.Ground;
            var cv = sr.gameObject.AddComponent<Conveyor>();
            cv.Velocity = new Vector2(dir * 5f, 0f);
        }

        private static void CreateCoin(Transform root, int c, int r,
            System.Func<int, float> Wx, System.Func<int, float> Wy)
        {
            var go = new GameObject("Coin");
            go.transform.SetParent(root, false);
            go.transform.position = new Vector3(Wx(c), Wy(r), 0f);
            var col = go.AddComponent<BoxCollider2D>();
            col.isTrigger = true;
            col.size = new Vector2(0.7f, 0.7f);
            go.AddComponent<Collectible>();

            var artCoin = LevelArt.Get("coin");
            if (artCoin != null)
            {
                var sr = ArtPiece(go.transform, "CoinArt", artCoin, Vector3.zero, 0.95f, 6);
                var tw = sr.gameObject.AddComponent<Twinkle>();
                tw.speed = 2.2f; tw.scaleAmp = 0.08f; tw.alphaAmp = 0.12f;
            }
            else
            {
                var v = new GameObject("CoinV");
                v.transform.SetParent(go.transform, false);
                v.AddComponent<Spinner>().degreesPerSecond = 150f;
                var face = TextureFactory.Circle(28, 24);
                Decal(v.transform, face, Color.Lerp(Palette.Coin, Color.black, 0.42f), 0.62f, Vector3.zero, 5);
                Decal(v.transform, face, Palette.Coin, 0.5f, Vector3.zero, 6);
                Decal(v.transform, face, new Color(1f, 1f, 1f, 0.85f), 0.16f, new Vector3(-0.1f, 0.12f, 0f), 7);
            }
        }

        private static void CreateDashOrb(Transform root, int c, int r,
            System.Func<int, float> Wx, System.Func<int, float> Wy)
        {
            var go = new GameObject("DashOrb");
            go.transform.SetParent(root, false);
            go.transform.position = new Vector3(Wx(c), Wy(r), 0f);
            var col = go.AddComponent<BoxCollider2D>();
            col.isTrigger = true;
            col.size = new Vector2(0.85f, 0.85f);
            go.AddComponent<DashOrb>();

            var artOrb = LevelArt.Get("dash-orb");
            if (artOrb != null)
            {
                var os = ArtPiece(go.transform, "OrbArt", artOrb, Vector3.zero, 0.95f, 6);
                var tw = os.gameObject.AddComponent<Twinkle>();
                tw.speed = 2.2f; tw.scaleAmp = 0.12f; tw.alphaAmp = 0.25f;
                return;
            }
            var v = new GameObject("OrbV");
            v.transform.SetParent(go.transform, false);
            v.transform.localScale = Vector3.one * 0.62f;
            var sr = v.AddComponent<SpriteRenderer>();
            sr.sprite = TextureFactory.Ring(22, 6, 16);
            sr.color = Palette.UiAccent;
            sr.sortingOrder = 6;
            v.AddComponent<Spinner>().degreesPerSecond = 200f;
        }

        private static void CreateFallingPlatform(Transform root, int start, int r, int len,
            System.Func<int, float> Wy)
        {
            float cx = start + len / 2f;
            var sr = Quad(root, "FallingPlatform", new Vector3(cx, Wy(r), 0f), new Vector2(len, 1f),
                Color.Lerp(Palette.Ground, Palette.UiGold, 0.45f), 0);
            var col = sr.gameObject.AddComponent<BoxCollider2D>();
            col.size = Vector2.one;
            sr.gameObject.layer = GameLayers.Ground;
            sr.gameObject.AddComponent<Rigidbody2D>();        // FallingPlatform sets it kinematic
            sr.gameObject.AddComponent<FallingPlatform>();
        }

        private static void CreateNoteBlock(Transform root, int start, int r, int len,
            System.Func<int, float> Wy)
        {
            float cx = start + len / 2f;
            var sr = Quad(root, "NoteBlock", new Vector3(cx, Wy(r), 0f), new Vector2(len, 1f),
                Palette.UiAccent, 0);
            var col = sr.gameObject.AddComponent<BoxCollider2D>();
            col.size = Vector2.one;
            sr.gameObject.layer = GameLayers.Ground;
            sr.gameObject.AddComponent<NoteBlock>();
        }

        private static void CreateJumpRing(Transform root, int c, int r,
            System.Func<int, float> Wx, System.Func<int, float> Wy)
        {
            var go = new GameObject("JumpRing");
            go.transform.SetParent(root, false);
            go.transform.position = new Vector3(Wx(c), Wy(r), 0f);
            var col = go.AddComponent<BoxCollider2D>();
            col.isTrigger = true;
            col.size = new Vector2(1.2f, 1.2f);
            go.AddComponent<JumpRing>();

            var artRing = LevelArt.Get("ring");
            if (artRing != null)
            {
                var rs = ArtPiece(go.transform, "RingArt", artRing, Vector3.zero, 1.6f, 6);
                rs.gameObject.AddComponent<Spinner>().degreesPerSecond = 22f;
            }
            else
            {
                var v = new GameObject("RingV");
                v.transform.SetParent(go.transform, false);
                v.transform.localScale = Vector3.one * 0.85f;
                var sr = v.AddComponent<SpriteRenderer>();
                sr.sprite = TextureFactory.Ring(26, 4, 16);
                sr.color = Palette.UiGold;
                sr.sortingOrder = 6;
                v.AddComponent<Spinner>().degreesPerSecond = 150f;
            }
        }

        private static void CreateBoostPad(Transform root, int c, int r,
            System.Func<int, float> Wx, System.Func<int, float> Wy, Vector2 velocity)
        {
            var go = new GameObject("BoostPad");
            go.transform.SetParent(root, false);
            go.transform.position = new Vector3(Wx(c), Wy(r), 0f);
            var col = go.AddComponent<BoxCollider2D>();
            col.isTrigger = true;
            col.size = new Vector2(1.0f, 1.0f);
            go.AddComponent<BoostPad>().velocity = velocity;

            bool up = velocity.y > velocity.x;
            var artPad = LevelArt.Get("boost-pad");
            if (artPad != null)
            {
                // art faces RIGHT; the vertical variant is the same piece rotated to point up
                var ps = ArtPiece(go.transform, "PadArt", artPad, new Vector3(0f, -0.05f, 0f), 0.95f, 5);
                if (up) ps.transform.localRotation = Quaternion.Euler(0f, 0f, 90f);
                var tw = ps.gameObject.AddComponent<Twinkle>();
                tw.speed = 2.5f; tw.scaleAmp = 0.06f; tw.alphaAmp = 0.2f;
                return;
            }
            var v = new GameObject("PadV");
            v.transform.SetParent(go.transform, false);
            v.transform.localScale = up ? new Vector3(1.1f, 0.5f, 1f) : new Vector3(0.5f, 1.1f, 1f);
            var sr = v.AddComponent<SpriteRenderer>();
            sr.sprite = TextureFactory.RoundedBox(20, 8, 3, 16);
            sr.color = Palette.GoalGlow;
            sr.sortingOrder = 5;
        }

        private static void CreateJumpPower(Transform root, int c, int r,
            System.Func<int, float> Wx, System.Func<int, float> Wy)
        {
            var go = new GameObject("JumpPower");
            go.transform.SetParent(root, false);
            go.transform.position = new Vector3(Wx(c), Wy(r), 0f);
            var col = go.AddComponent<BoxCollider2D>();
            col.isTrigger = true;
            col.size = new Vector2(0.95f, 0.95f);
            go.AddComponent<PowerUpItem>().kind = PowerUp.HighJump;

            var glow = ArtPiece(go.transform, "PowerGlow", TextureFactory.RadialGlow(48), Vector3.zero, 1.5f, 5);
            glow.color = new Color(0.45f, 1f, 0.5f, 0.6f);
            glow.gameObject.AddComponent<Twinkle>();
            ArtPiece(go.transform, "PowerOrb", TextureFactory.Circle(24, 24), Vector3.zero, 0.78f, 6).color = Palette.Hex("#39d07a");
            ArtPiece(go.transform, "PowerArrow", TextureFactory.Triangle(24, 16), new Vector3(0f, -0.16f, 0f), 0.55f, 7).color = Color.white;
        }

        private static void CreateFeather(Transform root, int c, int r,
            System.Func<int, float> Wx, System.Func<int, float> Wy)
        {
            var go = new GameObject("Feather");
            go.transform.SetParent(root, false);
            go.transform.position = new Vector3(Wx(c), Wy(r), 0f);
            var col = go.AddComponent<BoxCollider2D>();
            col.isTrigger = true;
            col.size = new Vector2(0.9f, 0.9f);
            go.AddComponent<Feather>();

            var artFeather = LevelArt.Get("feather");
            if (artFeather != null)
            {
                var fs = ArtPiece(go.transform, "FeatherArt", artFeather, Vector3.zero, 1.2f, 6);
                var sw = fs.gameObject.AddComponent<Sway>(); sw.speed = 1.6f; sw.degrees = 8f;
            }
            else
            {
                var v = new GameObject("FeatherV");
                v.transform.SetParent(go.transform, false);
                v.transform.localScale = new Vector3(0.5f, 0.7f, 1f);
                var sr = v.AddComponent<SpriteRenderer>();
                sr.sprite = TextureFactory.Triangle(16, 16);
                sr.color = Palette.GoalGlow;
                sr.sortingOrder = 6;
                v.AddComponent<Spinner>().degreesPerSecond = 90f;
            }
        }

        private static void LinkPortals(Transform root, int w, int h,
            System.Func<int, int, char> At, System.Func<int, float> Wx, System.Func<int, float> Wy)
        {
            foreach (char ch in new[] { 'p', 'q', 'r' })
            {
                var portals = new List<Portal>();
                for (int r = 0; r < h; r++)
                    for (int c = 0; c < w; c++)
                        if (At(c, r) == ch) portals.Add(CreatePortal(root, Wx(c), Wy(r)));
                for (int i = 0; i + 1 < portals.Count; i += 2)
                {
                    portals[i].link = portals[i + 1];
                    portals[i + 1].link = portals[i];
                }
            }
        }

        private static Portal CreatePortal(Transform root, float x, float y)
        {
            var go = new GameObject("Portal");
            go.transform.SetParent(root, false);
            go.transform.position = new Vector3(x, y, 0f);
            var col = go.AddComponent<BoxCollider2D>();
            col.isTrigger = true;
            col.size = new Vector2(1.0f, 1.8f);
            var p = go.AddComponent<Portal>();

            var v = new GameObject("PortalV");
            v.transform.SetParent(go.transform, false);
            v.transform.localScale = new Vector3(0.9f, 1.7f, 1f);
            var sr = v.AddComponent<SpriteRenderer>();
            sr.sprite = TextureFactory.Ring(22, 5, 16);
            sr.color = Palette.UiAccent;
            sr.sortingOrder = 5;
            v.AddComponent<Spinner>().degreesPerSecond = 120f;
            return p;
        }

        private static void CreateSpring(Transform root, int c, int r,
            System.Func<int, float> Wx, System.Func<int, float> Wy)
        {
            var go = new GameObject("Spring");
            go.transform.SetParent(root, false);
            float baseY = Wy(r) - 0.5f;
            go.transform.position = new Vector3(Wx(c), baseY + 0.12f, 0f);

            var col = go.AddComponent<BoxCollider2D>();
            col.isTrigger = true;
            col.size = new Vector2(0.8f, 0.8f);
            col.offset = new Vector2(0f, 0.3f);
            go.AddComponent<Spring>();

            var artSpring = LevelArt.Get("spring");
            if (artSpring != null)
            {
                ArtPiece(go.transform, "SpringArt", artSpring, new Vector3(0f, -0.12f, 0f), 1.1f, 6);
            }
            else
            {
                Color steelDark = Palette.Hex("#2c313c");
                Color coil = Palette.Hex("#e8eef5");
                Color top = Palette.Hex("#ffb43a");
                float x = Wx(c);
                QuadSprite(go.transform, "SpringBase", new Vector3(x, baseY + 0.16f, 0f),
                    new Vector2(0.96f, 0.26f), TextureFactory.RoundedBox(24, 7, 3, 16), steelDark, 4);
                for (int i = 0; i < 3; i++)
                    QuadSprite(go.transform, "SpringCoil", new Vector3(x, baseY + 0.30f + i * 0.12f, 0f),
                        new Vector2(0.52f, 0.08f), TextureFactory.RoundedBox(24, 6, 3, 16), coil, 5);
                QuadSprite(go.transform, "SpringTop", new Vector3(x, baseY + 0.62f, 0f),
                    new Vector2(0.94f, 0.24f), TextureFactory.RoundedBox(24, 9, 4, 16), top, 6);
            }
        }

        private enum EnemyKind { Walker, Flyer, Spiky, Diver }

        private static void CreateEnemy(Transform root, int c, int r,
            System.Func<int, float> Wx, System.Func<int, float> Wy, EnemyKind kind)
        {
            var go = new GameObject("Enemy_" + kind);
            go.transform.SetParent(root, false);
            float baseY = Wy(r) - 0.5f;          // floor of the cell
            float cy = (kind == EnemyKind.Flyer || kind == EnemyKind.Diver) ? Wy(r) + 0.5f : baseY + 0.42f;
            go.transform.position = new Vector3(Wx(c), cy, 0f);

            var rb = go.AddComponent<Rigidbody2D>();
            rb.bodyType = RigidbodyType2D.Kinematic;
            rb.gravityScale = 0f;
            var col = go.AddComponent<BoxCollider2D>();
            col.isTrigger = true;
            col.size = new Vector2(0.82f, 0.82f);

            // body: imported art per kind (wings/spikes baked into the art), procedural otherwise
            var artEnemy = kind == EnemyKind.Walker ? LevelArt.Get("enemy")
                         : kind == EnemyKind.Flyer  ? LevelArt.Get("enemy-flyer")
                         : kind == EnemyKind.Spiky  ? LevelArt.Get("enemy-spiky")
                         : null;
            if (artEnemy != null)
            {
                ArtPiece(go.transform, "EnemyArt", artEnemy, Vector3.zero,
                    kind == EnemyKind.Flyer ? 1.15f : 1.0f, 9);
            }
            else
            {
                var body = new GameObject("Body");
                body.transform.SetParent(go.transform, false);
                var bsr = body.AddComponent<SpriteRenderer>();
                bsr.sprite = TextureFactory.RoundedBox(18, 16, 6, 22);
                bsr.color = kind == EnemyKind.Diver ? Palette.Hazard : Palette.Enemy;
                bsr.sortingOrder = 9;
                EnemyEye(body.transform, new Vector3(-0.16f, 0.06f, 0f));
                EnemyEye(body.transform, new Vector3(0.16f, 0.06f, 0f));
            }

            if (kind == EnemyKind.Flyer || kind == EnemyKind.Diver)
            {
                if (artEnemy == null)
                {
                    EnemyWing(go.transform, -0.5f, +1f);
                    EnemyWing(go.transform, 0.5f, -1f);
                }
                if (kind == EnemyKind.Flyer) { go.AddComponent<EnemyFlyer>().range = 3.5f; }
                else go.AddComponent<EnemyDiver>();
            }
            else if (kind == EnemyKind.Spiky)
            {
                if (artEnemy == null)
                {
                    for (int i = -1; i <= 1; i++)
                    {
                        var sp = new GameObject("Spike");
                        sp.transform.SetParent(go.transform, false);
                        sp.transform.localPosition = new Vector3(i * 0.26f, 0.42f, 0f);
                        sp.transform.localScale = new Vector3(0.32f, 0.32f, 1f);
                        var ssr = sp.AddComponent<SpriteRenderer>();
                        ssr.sprite = TextureFactory.Triangle(16, 16);
                        ssr.color = Palette.Hazard;
                        ssr.sortingOrder = 10;
                    }
                }
                var spk = go.AddComponent<EnemySpiky>();
                spk.range = 3f;
            }
            else
            {
                var wk = go.AddComponent<EnemyWalker>();
                wk.range = 3.5f;
            }
        }

        private static void EnemyEye(Transform parent, Vector3 pos)
        {
            var eye = new GameObject("Eye");
            eye.transform.SetParent(parent, false);
            eye.transform.localPosition = pos;
            eye.transform.localScale = Vector3.one * 0.2f;
            var sr = eye.AddComponent<SpriteRenderer>();
            sr.sprite = TextureFactory.Circle(6, 16);
            sr.color = Palette.EnemyHi;
            sr.sortingOrder = 11;
        }

        private static void EnemyWing(Transform parent, float x, float dir)
        {
            var w = new GameObject("Wing");
            w.transform.SetParent(parent, false);
            w.transform.localPosition = new Vector3(x, 0.1f, 0f);
            w.transform.localScale = new Vector3(0.4f * dir, 0.4f, 1f);
            var sr = w.AddComponent<SpriteRenderer>();
            sr.sprite = TextureFactory.Triangle(16, 16);
            sr.color = Palette.EnemyHi;
            sr.sortingOrder = 8;
        }

        private static void CreateSaw(Transform root, int c, int r,
            System.Func<int, float> Wx, System.Func<int, float> Wy, Vector2 axis)
        {
            var go = new GameObject("Saw");
            go.transform.SetParent(root, false);
            go.transform.position = new Vector3(Wx(c), Wy(r), 0f);

            var rb = go.AddComponent<Rigidbody2D>();
            rb.bodyType = RigidbodyType2D.Kinematic;
            var mp = go.AddComponent<MovingPlatform>();
            mp.axis = axis;
            mp.distance = 2.5f;
            mp.speed = 4f;
            mp.phaseOffset = c * 0.7f;

            var col = go.AddComponent<BoxCollider2D>();
            col.isTrigger = true;
            col.size = new Vector2(1.05f, 1.05f);
            go.layer = GameLayers.Hazard;
            go.tag = GameTags.Hazard;
            go.AddComponent<Hazard>();

            var artSaw = LevelArt.Get("saw");
            if (artSaw != null)
            {
                var bs = ArtPiece(go.transform, "SawArt", artSaw, Vector3.zero, 1.35f, 6);
                bs.gameObject.AddComponent<Spinner>().degreesPerSecond = 540f;
                return;
            }

            var blade = new GameObject("Blade");
            blade.transform.SetParent(go.transform, false);
            blade.transform.localScale = Vector3.one * 1.2f;
            var bsr = blade.AddComponent<SpriteRenderer>();
            bsr.sprite = TextureFactory.Ring(24, 7, 16);
            bsr.color = Palette.Hazard;
            bsr.sortingOrder = 6;
            blade.AddComponent<Spinner>().degreesPerSecond = 540f;

            var notch = new GameObject("Notch");
            notch.transform.SetParent(blade.transform, false);
            notch.transform.localPosition = new Vector3(0.45f, 0f, 0f);
            var nsr = notch.AddComponent<SpriteRenderer>();
            nsr.sprite = TextureFactory.Circle(6, 16);
            nsr.color = Palette.PlayerHi;
            nsr.sortingOrder = 7;
        }

        private static void CreateSolid(Transform root, int start, int r, int len,
            System.Func<int, float> Wx, System.Func<int, float> Wy, System.Func<int, int, char> At)
        {
            float cx = start + len / 2f;
            float cy = Wy(r);
            var go = new GameObject("Ground");
            go.transform.SetParent(root, false);
            go.transform.position = new Vector3(cx, cy, 0f);
            go.transform.localScale = new Vector3(len, 1f, 1f);
            var col = go.AddComponent<BoxCollider2D>();
            col.size = Vector2.one;                 // scaled by transform -> (len, 1)
            go.layer = GameLayers.Ground;

            var tile = LevelArt.Get("ground");
            if (tile != null)
            {
                // a short '#' run with nothing solid under it is a FLOATING ledge → use the island
                // art (frayed/dripping bottom) instead of hard-edged ground tiles
                var plat = LevelArt.Get("platform");
                int mid = start + len / 2;
                bool surface = At(mid, r - 1) != '#';
                bool hollow = At(mid, r + 1) != '#';
                if (plat != null && surface && hollow && len <= 5)
                {
                    QuadSprite(root, "PlatformArt", new Vector3(cx, Wy(r) - 0.2f, 0f),
                        new Vector2(len + 0.7f, 1.5f), plat, Color.white, 0);
                    return;
                }

                // grass tile on each surface cell; where the ground is genuinely solid below, extend
                // deep earth far down (off-screen) so the floor isn't a floating slab. Buried cells are
                // covered by that deep earth, so we skip them. (All parented to ROOT to dodge the
                // run's (len,1) collider scale.)
                Color soilDeep = Palette.GroundEdge;   // per-theme deep earth (set in PaintingThemes)
                var wallTile = LevelArt.Get("wall");
                for (int c = start; c < start + len; c++)
                {
                    if (At(c, r - 1) == '#')                           // buried — covered by deep earth,
                    {                                                  // but texture exposed vertical faces
                        if (wallTile != null && (At(c - 1, r) != '#' || At(c + 1, r) != '#'))
                            QuadSprite(root, "WallTile", new Vector3(Wx(c), Wy(r), 0f),
                                new Vector2(1.02f, 1.02f), wallTile, Color.white, -2);
                        continue;
                    }
                    QuadSprite(root, "GroundTile", new Vector3(Wx(c), Wy(r) - 0.05f, 0f),
                        new Vector2(1.01f, 1.4f), tile, Color.white, 0);
                    if (At(c, r + 1) == '#')                           // solid below → deep terrain
                    {
                        // flat, uniform soil that STARTS at the cell top (hidden behind the grass tile)
                        // and runs far down — no internal texture seam, no grass/soil border line.
                        float top = Wy(r) + 0.5f, bottom = -14f;
                        Quad(root, "DeepSoil", new Vector3(Wx(c), (top + bottom) * 0.5f, 0f),
                            new Vector2(1.04f, top - bottom), soilDeep, -3);
                    }
                }

                // painted lips where a surface run ends in open air (pit edges, ledge ends);
                // the art faces LEFT, mirrored for the right end
                var edge = LevelArt.Get("ground-edge");
                if (edge != null)
                {
                    if (At(start - 1, r) != '#' && At(start, r - 1) != '#')
                        QuadSprite(root, "GroundEdge", new Vector3(Wx(start) - 0.16f, Wy(r) - 0.05f, 0f),
                            new Vector2(0.9f, 1.45f), edge, Color.white, 1);
                    if (At(start + len, r) != '#' && At(start + len - 1, r - 1) != '#')
                    {
                        var esr = QuadSprite(root, "GroundEdge", new Vector3(Wx(start + len - 1) + 0.16f, Wy(r) - 0.05f, 0f),
                            new Vector2(0.9f, 1.45f), edge, Color.white, 1);
                        var sc = esr.transform.localScale; sc.x = -sc.x; esr.transform.localScale = sc;
                    }
                }
                return;
            }

            // procedural fallback: lit impasto slab
            var vis = new GameObject("GroundVis");
            vis.transform.SetParent(go.transform, false);
            var sr = vis.AddComponent<SpriteRenderer>();
            sr.sprite = TextureFactory.BrushBand();
            sr.color = Palette.Ground;
            sr.sortingOrder = 0;
            var b = sr.sprite.bounds.size;
            vis.transform.localScale = new Vector3(1f / b.x, 1f / b.y, 1f);  // fill 1×1 local cell
        }

        /// <summary>Invisible solid world-edge: collision only, no sprite (kills the dark soil pillar
        /// that a full-height '#' wall used to draw at the far left/right of every level).</summary>
        private static void CreateBoundary(Transform root, int start, int r, int len,
            System.Func<int, float> Wy)
        {
            float cx = start + len / 2f;
            var go = new GameObject("Bound");
            go.transform.SetParent(root, false);
            go.transform.position = new Vector3(cx, Wy(r), 0f);
            go.transform.localScale = new Vector3(len, 1f, 1f);
            var col = go.AddComponent<BoxCollider2D>();
            col.size = Vector2.one;
            go.layer = GameLayers.Ground;
        }

        /// <summary>A low duck-tunnel ceiling: the collider's bottom is dropped ~0.15 below the cell so
        /// the standing capsule (top ≈ +1.08) is firmly blocked while the crouch capsule (top ≈ +0.6)
        /// slips under. Renders the frayed island/overhang art so it reads as a rock shelf.</summary>
        private static void CreateDuckCeiling(Transform root, int start, int r, int len,
            System.Func<int, float> Wx, System.Func<int, float> Wy)
        {
            float cellTop = Wy(r) + 0.5f;
            float lowBottom = Wy(r) - 0.5f - 0.15f;       // drop the underside into the gap a touch
            float cx = start + len / 2f;
            var go = new GameObject("DuckCeiling");
            go.transform.SetParent(root, false);
            go.transform.position = new Vector3(cx, (cellTop + lowBottom) * 0.5f, 0f);
            go.transform.localScale = new Vector3(len, cellTop - lowBottom, 1f);
            var col = go.AddComponent<BoxCollider2D>();
            col.size = Vector2.one;
            go.layer = GameLayers.Ground;

            // overhang visual (purely cosmetic) — dedicated low-shelf art first, then the frayed
            // island art, then a grass strip
            var over = LevelArt.Get("overhang");
            var plat = LevelArt.Get("platform");
            var tile = LevelArt.Get("ground");
            if (over != null)
                QuadSprite(root, "DuckArt", new Vector3(cx, Wy(r) - 0.1f, 0f),
                    new Vector2(len + 0.7f, 1.6f), over, Color.white, 0);
            else if (plat != null)
                QuadSprite(root, "DuckArt", new Vector3(cx, Wy(r) - 0.1f, 0f),
                    new Vector2(len + 0.7f, 1.5f), plat, Color.white, 0);
            else if (tile != null)
                QuadSprite(root, "DuckArt", new Vector3(cx, Wy(r), 0f),
                    new Vector2(len + 0.04f, 1.3f), tile, Color.white, 0);
            else
                Quad(root, "DuckArt", new Vector3(cx, Wy(r), 0f), new Vector2(len, 1f), Palette.Ground, 0);
        }

        private static void CreateCaps(Transform root, int start, int len, int r,
            System.Func<int, int, char> At, System.Func<int, float> Wx, System.Func<int, float> Wy)
        {
            // bright top edge over any cells whose neighbour above is empty
            int c = start;
            int end = start + len;
            while (c < end)
            {
                if (At(c, r - 1) != '#')
                {
                    int s = c;
                    while (c < end && At(c, r - 1) != '#') c++;
                    int l = c - s;
                    float cx = s + l / 2f;
                    float topY = Wy(r) + 0.5f - 0.07f;
                    Quad(root, "GroundCap", new Vector3(cx, topY, 0f),
                        new Vector2(l, 0.14f), Palette.GroundCap, 1);
                    // soft starlight rim-glow catching the top edge
                    QuadSprite(root, "GroundRim", new Vector3(cx, topY + 0.22f, 0f),
                        new Vector2(l + 0.6f, 0.8f), TextureFactory.RadialGlow(48),
                        new Color(Palette.Coin.r, Palette.Coin.g, Palette.Coin.b, 0.2f), 1);
                }
                else c++;
            }
        }

        private static void CreateSpike(Transform root, int c, int r,
            System.Func<int, float> Wx, System.Func<int, float> Wy)
        {
            var go = new GameObject("Spike");
            go.transform.SetParent(root, false);
            float surf = Wy(r) - 0.5f;
            float x = Wx(c);
            go.transform.position = new Vector3(x, surf, 0f);              // sit on cell floor

            var artPool = LevelArt.Get("pool");
            var artSpike = LevelArt.Get("spike");
            if (artPool != null)
            {
                // the pool is drawn as one continuous strip per run in Build(); this cell only keeps
                // its hazard collider so the kill zones stay identical to the spikes.
            }
            else if (artSpike != null)
            {
                // vary height + mirror alternate shards so a pit doesn't look stamped
                float vh = 1.35f + 0.4f * Mathf.Abs(Mathf.Sin(x * 2.3f));
                var ssr = ArtPiece(go.transform, "SpikeArt", artSpike, Vector3.zero, vh, 6);
                if (Mathf.RoundToInt(x) % 2 == 0)
                {
                    var s = ssr.transform.localScale; s.x = -s.x; ssr.transform.localScale = s;
                }
            }
            else
            {
                Color tip = Color.Lerp(Palette.Hazard, Color.white, 0.6f);
                QuadSprite(go.transform, "SpikeBase", new Vector3(x, surf + 0.07f, 0f),
                    new Vector2(0.9f, 0.16f), TextureFactory.RoundedBox(20, 6, 2, 16),
                    Color.Lerp(Palette.Hazard, Color.black, 0.6f), 4);
                QuadSprite(go.transform, "SpikeCrystal", new Vector3(x, surf, 0f),
                    new Vector2(0.8f, 1.02f), TextureFactory.Crystal(28), Palette.Hazard, 6);
                var glow = QuadSprite(go.transform, "SpikeTipGlow", new Vector3(x, surf + 0.92f, 0f),
                    new Vector2(0.6f, 0.6f), TextureFactory.RadialGlow(48), new Color(tip.r, tip.g, tip.b, 0.5f), 5);
                var tw = glow.gameObject.AddComponent<Twinkle>();
                tw.speed = 2.6f; tw.scaleAmp = 0.2f; tw.alphaAmp = 0.45f; tw.phase = x;
                QuadSprite(go.transform, "SpikeTip", new Vector3(x, surf + 0.9f, 0f),
                    new Vector2(0.18f, 0.18f), TextureFactory.Circle(10, 24), Color.Lerp(tip, Color.white, 0.5f), 7);
            }

            var col = go.AddComponent<BoxCollider2D>();
            col.isTrigger = true;
            col.size = new Vector2(0.6f, 0.55f);
            col.offset = new Vector2(0f, 0.42f);
            go.layer = GameLayers.Hazard;
            go.tag = GameTags.Hazard;
            go.AddComponent<Hazard>();
        }

        private static void CreateHazardBlock(Transform root, int c, int r,
            System.Func<int, float> Wx, System.Func<int, float> Wy)
        {
            var sr = Quad(root, "HazardBlock", new Vector3(Wx(c), Wy(r), 0f),
                Vector2.one, Palette.Hazard, 5);
            var col = sr.gameObject.AddComponent<BoxCollider2D>();
            col.isTrigger = true;
            col.size = new Vector2(0.96f, 0.96f);
            sr.gameObject.layer = GameLayers.Hazard;
            sr.gameObject.tag = GameTags.Hazard;
            sr.gameObject.AddComponent<Hazard>();
        }

        private static void CreateOneWay(Transform root, int c, int r,
            System.Func<int, float> Wx, System.Func<int, float> Wy)
        {
            float topY = Wy(r) + 0.5f - 0.15f;
            var sr = Quad(root, "OneWay", new Vector3(Wx(c), topY, 0f),
                new Vector2(1f, 0.3f), Palette.Moving, 2);
            var col = sr.gameObject.AddComponent<BoxCollider2D>();
            col.size = Vector2.one;
            col.usedByEffector = true;
            var eff = sr.gameObject.AddComponent<PlatformEffector2D>();
            eff.useOneWay = true;
            eff.surfaceArc = 170f;
            sr.gameObject.layer = GameLayers.OneWay;
        }

        private static void CreateMoving(Transform root, int c, int r,
            System.Func<int, float> Wx, System.Func<int, float> Wy, Vector2 axis)
        {
            var sr = Quad(root, "MovingPlatform", new Vector3(Wx(c), Wy(r), 0f),
                new Vector2(1.6f, 0.4f), Palette.Moving, 3);
            var col = sr.gameObject.AddComponent<BoxCollider2D>();
            col.size = Vector2.one;
            sr.gameObject.layer = GameLayers.MovingPlatform;
            var rb = sr.gameObject.AddComponent<Rigidbody2D>();
            rb.bodyType = RigidbodyType2D.Kinematic;
            var mp = sr.gameObject.AddComponent<MovingPlatform>();
            mp.axis = axis;
            mp.distance = 2.5f;
            mp.speed = 3f;
        }

        private static void CreateGoal(Transform root, int c, int r,
            System.Func<int, float> Wx, System.Func<int, float> Wy)
        {
            var go = new GameObject("Goal");
            go.transform.SetParent(root, false);
            float baseY = Wy(r) - 0.5f;     // floor of the cell
            go.transform.position = new Vector3(Wx(c), baseY + 1.2f, 0f);

            var artGoal = LevelArt.Get("goal");
            if (artGoal != null)
            {
                ArtPiece(go.transform, "GoalArt", artGoal, new Vector3(0f, -1.2f, 0f), 3.2f, 6);
            }
            else
            {
                float gx = Wx(c);
                QuadSprite(go.transform, "Pole", new Vector3(gx, baseY + 1.3f, 0f),
                    new Vector2(0.16f, 2.6f), TextureFactory.RoundedBox(6, 40, 2, 16), Palette.Hex("#23262e"), 4);
                QuadSprite(go.transform, "Halo", new Vector3(gx, baseY + 2.5f, 0f),
                    new Vector2(1.5f, 1.5f), TextureFactory.RadialGlow(48), new Color(Palette.GoalGlow.r, Palette.GoalGlow.g, Palette.GoalGlow.b, 0.5f), 4);
                QuadSprite(go.transform, "Flag", new Vector3(gx + 0.52f, baseY + 2.28f, 0f),
                    new Vector2(0.9f, 0.6f), TextureFactory.RoundedBox(22, 16, 3, 16), Palette.Goal, 6);
                QuadSprite(go.transform, "Finial", new Vector3(gx, baseY + 2.62f, 0f),
                    new Vector2(0.34f, 0.34f), TextureFactory.Circle(16, 24), Palette.GoalGlow, 7);
            }

            var col = go.AddComponent<BoxCollider2D>();
            col.isTrigger = true;
            col.size = new Vector2(1.0f, 2.6f);
            col.offset = Vector2.zero;
            go.tag = GameTags.Finish;
            go.AddComponent<Goal>();
        }
    }
}
