using System.Collections.Generic;
using UnityEngine;

namespace Velocity
{
    /// <summary>
    /// The level select, reimagined as an explorable WORLD MAP (Super Mario World / Cuphead
    /// overworld). Each masterpiece is an island landmark themed to its painting, strung along a
    /// winding dotted trail across a painted land; a grand pavilion at the end is the full-gallery
    /// finale. You walk a little avatar along the trail (left/right travels it, climbing the hills
    /// and dipping through the valleys), stop on a node to read its plaque, and press JUMP to dive
    /// into the painting. The finale unlocks once every masterpiece is cleared.
    ///
    /// Built and torn down by GameManager; driven by GameManager.Update via <see cref="Tick"/>.
    /// </summary>
    public class Overworld : MonoBehaviour
    {
        // base sky tone — GameManager paints the camera background with this on entry
        public static readonly Color WallColor = Hex("#1a1430");

        // map palette (a twilight "world of paintings", independent of any one canvas)
        private static readonly Color SkyTop    = Hex("#0e0b22");
        private static readonly Color SkyBottom = Hex("#3a2a52");
        private static readonly Color Horizon   = Hex("#c98a6a");
        private static readonly Color TrailWarm = Hex("#ffe6b0");
        private static readonly Color FrameGold = Hex("#c9a24a");
        private static readonly Color FrameDark = Hex("#17110b");
        private static readonly Color GemDim    = new Color(1f, 1f, 1f, 0.13f);

        private const float Reach = 2.6f;     // how close (2-D) to a node counts as "standing on it"
        private const float FrameH = 4.2f;    // painting landmark height

        private GameInput _input;
        private GameManager _gm;
        private UIManager _ui;
        private CameraController _cam;
        private MapWalker _walker;

        private Vector2[] _nodePos;            // world position of each node (levels + finale)
        private int _levelCount, _nodeCount;
        private bool _finaleUnlocked;
        private int _near = -2;
        private Sprite _box, _disc, _glow, _ring, _spark, _tri;

        private static Color Hex(string h) { ColorUtility.TryParseHtmlString(h, out var c); return c; }

        public void Setup(GameInput input, GameManager gm, UIManager ui, CameraController cam, int startNode)
        {
            _input = input; _gm = gm; _ui = ui; _cam = cam;

            _box   = TextureFactory.WhitePixel();
            _disc  = TextureFactory.Circle(48, 24);
            _glow  = TextureFactory.RadialGlow(64);
            _ring  = TextureFactory.Ring(28, 3, 16);
            _spark = TextureFactory.Sparkle(40);
            _tri   = TextureFactory.Triangle(48, 16);

            _levelCount = LevelLibrary.WorldAt(0).Levels.Count;
            _nodeCount  = _levelCount + 1;                 // + the finale pavilion

            // -------- node layout: a winding journey, each node at its own height --------
            // hand-placed so the trail climbs and dips like a real overworld trek
            var layout = new Vector2[]
            {
                new Vector2(13f, 11f),
                new Vector2(35f, 17f),
                new Vector2(58f, 9f),
                new Vector2(82f, 18f),
                new Vector2(106f, 25f),                    // finale, highest + last
            };
            _nodePos = new Vector2[_nodeCount];
            for (int i = 0; i < _nodeCount; i++)
                _nodePos[i] = layout[Mathf.Min(i, layout.Length - 1)];

            // finale unlocks only when every masterpiece is cleared
            _finaleUnlocked = true;
            for (int i = 0; i < _levelCount; i++)
                if (!Leaderboard.HasCompleted(LevelLibrary.WorldAt(0).Levels[i].Id)) _finaleUnlocked = false;

            // -------- the trail: a polyline through the nodes, sagging between islands --------
            var pts = new List<Vector2>();
            var nodePtIdx = new List<int>();
            for (int i = 0; i < _nodeCount; i++)
            {
                nodePtIdx.Add(pts.Count);
                pts.Add(_nodePos[i]);
                if (i < _nodeCount - 1)
                {
                    Vector2 mid = (_nodePos[i] + _nodePos[i + 1]) * 0.5f + new Vector2(0f, -2.4f);
                    pts.Add(mid);                          // a gentle valley dip between islands
                }
            }
            var route = new MapRoute(pts, nodePtIdx);

            // map world extents for the camera + sky sizing
            float minX = 0f, maxX = _nodePos[_nodeCount - 1].x + 14f;
            var bounds = new Bounds(new Vector3((minX + maxX) / 2f, 14f, 0f),
                                    new Vector3(maxX - minX + 8f, 42f, 1f));

            // -------- build the world --------
            BuildSky(bounds);
            BuildTrail(route);
            for (int i = 0; i < _levelCount; i++) BuildNode(i, _nodePos[i]);
            BuildFinale(_nodePos[_nodeCount - 1]);

            // -------- the avatar walks the trail --------
            var wgo = new GameObject("MapWalker");
            wgo.transform.SetParent(transform, false);
            _walker = wgo.AddComponent<MapWalker>();
            int startIdx = Mathf.Clamp(startNode, 0, _levelCount - 1);
            _walker.Init(input, route, route.NodeDistance(startIdx));

            _cam.SetTarget(_walker.transform);
            _cam.SetBounds(bounds);
            _cam.Snap();

            _near = -2;
            UpdatePlaque(startIdx);
        }

        // ---- per-frame tick (called from GameManager while in Overworld) ----
        public void Tick()
        {
            if (_walker == null) return;
            _walker.TickMove();

            Vector2 p = _walker.transform.position;
            int nearest = 0; float bestD = float.MaxValue;
            for (int i = 0; i < _nodeCount; i++)
            {
                float d = Vector2.Distance(p, _nodePos[i]);
                if (d < bestD) { bestD = d; nearest = i; }
            }
            int near = bestD <= Reach ? nearest : -1;
            if (near != _near) UpdatePlaque(near);

            if (near >= 0 && _input.JumpPressed)
            {
                bool isFinale = near >= _levelCount;
                if (isFinale && !_finaleUnlocked)
                {
                    if (Sfx.Instance != null) Sfx.Instance.PlayClick();   // a soft "locked" tap
                    return;
                }
                if (Sfx.Instance != null) Sfx.Instance.PlayClick();
                _walker.PlayEnterHop();
                if (!isFinale) _gm.RequestStartLevel(LevelLibrary.GlobalIndex(0, near));
                else _gm.RequestFullRun(0);
                return;
            }
            if (_input.PausePressed) _gm.RequestMainMenu();
        }

        private void UpdatePlaque(int node)
        {
            _near = node;
            if (node < 0) { _ui.SetOverworldInfo(0, null, null, null, null, false, false); return; }

            if (node < _levelCount)
            {
                var lvl = LevelLibrary.WorldAt(0).Levels[node];
                bool cleared = Leaderboard.TryGetBest(lvl.Id, out double best);
                string pb = cleared ? "PB  " + SpeedrunTimer.Format(best) : "PB  —";
                int coins = Coins.Best(lvl.Id);
                string coinStr = "◎ " + coins + "/3";
                var theme = PaintingThemes.For(node);
                _ui.SetOverworldInfo(node + 1, theme != null ? theme.Name : lvl.DisplayName,
                    theme != null ? theme.Painter : "", pb, coinStr, true, false);
            }
            else
            {
                string frKey = "vel.fullrun.G";
                string best = PlayerPrefs.HasKey(frKey)
                    ? "Best run  " + SpeedrunTimer.Format(PlayerPrefs.GetFloat(frKey)) : "Best run  —";
                _ui.SetOverworldInfo(0, "The Full Gallery",
                    "all four masterpieces, one timer", best, "", _finaleUnlocked, true);
            }
        }

        // ================================================================ environment
        private void BuildSky(Bounds b)
        {
            float cx = b.center.x, cy = b.center.y, w = b.size.x, h = b.size.y;

            // gradient sky (locked to the camera-ish: huge, behind everything)
            var sky = Spr("Sky", TextureFactory.VerticalGradient(8, 64, SkyBottom, SkyTop), Color.white,
                new Vector3(cx, cy, 0f), new Vector2(w * 1.4f, h * 1.4f), -60);

            // warm horizon glow low across the land
            Spr("Horizon", _glow, new Color(Horizon.r, Horizon.g, Horizon.b, 0.4f),
                new Vector3(cx, b.min.y + 6f, 0f), new Vector2(w * 1.2f, 16f), -58);

            // scattered stars (this is the night-side of a dream-gallery sky)
            for (int i = 0; i < 40; i++)
            {
                float x = Random.Range(b.min.x, b.max.x);
                float y = Random.Range(cy - 2f, b.max.y);
                var s = Spr("star", _disc, new Color(1f, 1f, 1f, Random.Range(0.3f, 0.8f)),
                    new Vector3(x, y, 0f), Vector2.one * Random.Range(0.1f, 0.26f), -55);
                Twinkleify(s, Random.Range(1.2f, 2.6f), 0.3f, 0.5f, x + y);
            }

            // soft parallax clouds drifting mid-sky
            for (int i = 0; i < 7; i++)
            {
                float x = Random.Range(b.min.x, b.max.x);
                float y = Random.Range(cy + 2f, b.max.y - 3f);
                Color c = new Color(0.55f, 0.45f, 0.65f, 0.32f);
                for (int k = 0; k < 4; k++)
                    Spr("cloud", _disc, c, new Vector3(x + k * 1.5f - 2f, y + Mathf.Sin(k) * 0.4f, 0f),
                        new Vector2(Random.Range(4f, 6f), Random.Range(2f, 2.8f)), -52);
            }

            // a few distant gilded frames floating in the haze — this is a world MADE of paintings
            for (int i = 0; i < 5; i++)
            {
                float x = Random.Range(b.min.x + 6f, b.max.x - 6f);
                float y = Random.Range(cy + 4f, b.max.y - 2f);
                float fw = Random.Range(1.6f, 2.6f), fh = fw * 1.2f;
                Spr("farframe", _box, new Color(FrameGold.r, FrameGold.g, FrameGold.b, 0.18f),
                    new Vector3(x, y, 0f), new Vector2(fw + 0.3f, fh + 0.3f), -51);
                Spr("farcanvas", _box, new Color(0.2f, 0.16f, 0.3f, 0.18f),
                    new Vector3(x, y, 0f), new Vector2(fw, fh), -50);
            }
        }

        // ---- the dotted cobble trail between islands (Mario-World path) ----
        private void BuildTrail(MapRoute route)
        {
            // a dot every ~0.85 units along the whole route; dim the final segment until unlocked
            float lockDist = route.NodeDistance(_levelCount - 1);   // start of the finale leg
            float step = 0.85f;
            int i = 0;
            for (float d = 0f; d <= route.Total; d += step, i++)
            {
                Vector2 p = route.Sample(d, out _);
                bool locked = !_finaleUnlocked && d > lockDist + 0.5f;
                float a = locked ? 0.16f : 0.9f;
                Color c = locked ? new Color(0.7f, 0.7f, 0.8f, a)
                                 : new Color(TrailWarm.r, TrailWarm.g, TrailWarm.b, a);
                // alternate big cobble / small stud for a hand-laid feel
                float sz = (i % 2 == 0) ? 0.42f : 0.26f;
                Spr("trail", _disc, c, new Vector3(p.x, p.y - 0.6f, 0f), Vector2.one * sz, -14);
            }
        }

        // ================================================================ nodes
        private void BuildNode(int i, Vector2 pos)
        {
            var theme = PaintingThemes.For(i);
            Color region = theme != null ? theme.SkyNear : new Color(0.3f, 0.3f, 0.4f);
            Color accent = theme != null ? theme.Coin : Palette.Coin;

            Island(pos, region);
            NodeDecor(i, pos, theme);

            // gilded frame + dark mat + the painting itself, standing on the island
            var tex = theme != null ? Resources.Load<Texture2D>(theme.ResourceKey) : null;
            float w = FrameH;
            if (tex != null && tex.height > 0) w = FrameH * tex.width / tex.height;
            w = Mathf.Clamp(w, 3f, 6.4f);
            float fy = pos.y + 3.4f;                       // hover the canvas above the island

            Spr("FrameGold", _box, FrameGold, new Vector3(pos.x, fy, 0f), new Vector2(w + 0.6f, FrameH + 0.6f), -8);
            Spr("FrameMat",  _box, FrameDark, new Vector3(pos.x, fy, 0f), new Vector2(w + 0.24f, FrameH + 0.24f), -7);
            var canvas = new GameObject("Canvas"); canvas.transform.SetParent(transform, false);
            var csr = canvas.AddComponent<SpriteRenderer>();
            if (tex != null)
            {
                tex.wrapMode = TextureWrapMode.Clamp; tex.filterMode = FilterMode.Bilinear;
                var spr = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f), 100f);
                csr.sprite = spr;
                canvas.transform.localScale = Vector3.one * (FrameH / spr.bounds.size.y);
            }
            else { csr.sprite = _box; csr.color = region; canvas.transform.localScale = new Vector3(w, FrameH, 1f); }
            canvas.transform.position = new Vector3(pos.x, fy, 0f);
            csr.sortingOrder = -6;

            // a warm light pool washing up the canvas
            Spr("NodeGlow", _glow, new Color(accent.r, accent.g, accent.b, 0.22f),
                new Vector3(pos.x, fy - 0.4f, 0f), new Vector2(w + 3f, FrameH + 3f), -9);

            // number signpost on the island
            Spr("SignPost", _box, Hex("#5e4636"), new Vector3(pos.x - w * 0.5f - 0.9f, pos.y + 0.5f, 0f),
                new Vector2(0.16f, 2.2f), -5);
            Spr("Sign", _box, Hex("#3c2a31"), new Vector3(pos.x - w * 0.5f - 0.9f, pos.y + 1.4f, 0f),
                new Vector2(0.95f, 0.7f), -4);

            // cleared gem floating over the frame
            var lvl = LevelLibrary.WorldAt(0).Levels[i];
            bool cleared = Leaderboard.HasCompleted(lvl.Id);
            var gem = Spr("Gem", _disc, cleared ? accent : GemDim,
                new Vector3(pos.x, fy + FrameH * 0.5f + 0.9f, 0f), Vector2.one * 0.55f, -3);
            if (cleared)
            {
                Spr("GemHalo", _glow, new Color(accent.r, accent.g, accent.b, 0.5f),
                    new Vector3(pos.x, fy + FrameH * 0.5f + 0.9f, 0f), Vector2.one * 1.8f, -4);
                Twinkleify(gem, 1.6f, 0.16f, 0.3f, i);
            }
        }

        /// <summary>A rounded island the node stands on, tinted to the painting's world.</summary>
        private void Island(Vector2 pos, Color region)
        {
            Color land = Color.Lerp(region, Hex("#2a2030"), 0.45f);
            Color lip  = Color.Lerp(region, Color.white, 0.25f);
            // body: a wide disc + a deeper disc below for thickness
            Spr("IslandBody", _disc, Color.Lerp(land, Color.black, 0.35f),
                new Vector3(pos.x, pos.y - 2.6f, 0f), new Vector2(9.5f, 5.5f), -18);
            Spr("IslandTop", _disc, land, new Vector3(pos.x, pos.y - 0.7f, 0f), new Vector2(9f, 3.2f), -17);
            // grassy/lit top lip
            Spr("IslandLip", _disc, lip, new Vector3(pos.x, pos.y - 0.2f, 0f), new Vector2(8.4f, 1.4f), -16);
        }

        /// <summary>A couple of floating motifs per region so each island reads as its painting.</summary>
        private void NodeDecor(int i, Vector2 pos, PaintingTheme theme)
        {
            if (theme == null) return;
            Color glow = theme.Glow, motif = theme.Motif;
            switch (theme.Style)
            {
                case BackdropStyle.StarryNight:
                    // a crescent + swirl sparks
                    Spr("moon", _disc, glow, new Vector3(pos.x + 3.6f, pos.y + 5.4f, 0f), Vector2.one * 1.1f, -6);
                    Spr("moonCut", _box, SkyTop, new Vector3(pos.x + 4.0f, pos.y + 5.6f, 0f), Vector2.one * 1.0f, -5);
                    for (int k = 0; k < 4; k++)
                        Twinkleify(Spr("sparkN", _spark, glow,
                            new Vector3(pos.x - 4f + k * 2.6f, pos.y + 5f + Mathf.Sin(k) * 0.8f, 0f),
                            Vector2.one * 0.6f, -6), 1.8f, 0.3f, 0.5f, k);
                    break;
                case BackdropStyle.Wave:
                    // a curling foam wave to the side
                    for (int k = 0; k < 5; k++)
                        Spr("waveArc", _disc, k == 4 ? Color.white : motif,
                            new Vector3(pos.x - 4.4f + k * 0.7f, pos.y + 1.2f + k * 0.5f, 0f),
                            Vector2.one * (1.6f - k * 0.18f), -6);
                    break;
                case BackdropStyle.Garden:
                    // rose dots + a swaying empty swing silhouette
                    for (int k = 0; k < 5; k++)
                        Spr("rose", _disc, new Color(0.95f, 0.65f, 0.78f, 1f),
                            new Vector3(pos.x - 4f + k * 2f, pos.y + 4.8f + Mathf.Sin(k * 1.6f) * 0.7f, 0f),
                            Vector2.one * 0.4f, -6);
                    var swing = new GameObject("swingDecor"); swing.transform.SetParent(transform, false);
                    swing.transform.position = new Vector3(pos.x + 4.6f, pos.y + 5.6f, 0f);
                    var sw = swing.AddComponent<Sway>(); sw.speed = 0.8f; sw.degrees = 9f;
                    Spr2(swing.transform, _box, Hex("#cbb089"), new Vector3(-0.5f, -1.6f, 0f), new Vector2(0.08f, 1.6f), -6);
                    Spr2(swing.transform, _box, Hex("#cbb089"), new Vector3(0.5f, -1.6f, 0f), new Vector2(0.08f, 1.6f), -6);
                    Spr2(swing.transform, _box, Hex("#8a6a44"), new Vector3(0f, -2.4f, 0f), new Vector2(1.3f, 0.22f), -5);
                    break;
                case BackdropStyle.Scream:
                    // burning sky bands behind the frame
                    for (int k = 0; k < 3; k++)
                        Spr("fireBand", _disc, Color.Lerp(glow, theme.Sky, k / 3f),
                            new Vector3(pos.x, pos.y + 4.5f + k * 1.1f, 0f), new Vector2(8f - k, 1.0f), -7);
                    break;
                default: break;
            }
        }

        private void BuildFinale(Vector2 pos)
        {
            Color stone = Hex("#6b6258"), gild = FrameGold;
            Island(pos, new Color(0.45f, 0.4f, 0.5f));

            // a grand gilded pavilion: two columns + pediment + a star
            Spr("colL", _box, stone, new Vector3(pos.x - 2.4f, pos.y + 2.6f, 0f), new Vector2(0.7f, 5.2f), -6);
            Spr("colR", _box, stone, new Vector3(pos.x + 2.4f, pos.y + 2.6f, 0f), new Vector2(0.7f, 5.2f), -6);
            Spr("lintel", _box, gild, new Vector3(pos.x, pos.y + 5.4f, 0f), new Vector2(6.4f, 0.7f), -6);
            Spr("pediment", _tri, gild, new Vector3(pos.x, pos.y + 6.3f, 0f), new Vector2(6.4f, 1.8f), -6);
            Spr("portal", _box, _finaleUnlocked ? new Color(1f, 0.92f, 0.7f, 0.85f) : new Color(0.3f, 0.3f, 0.4f, 0.7f),
                new Vector3(pos.x, pos.y + 2.6f, 0f), new Vector2(3.6f, 5.0f), -7);
            var star = Spr("finaleStar", _spark, _finaleUnlocked ? Hex("#ffe08a") : GemDim,
                new Vector3(pos.x, pos.y + 7.4f, 0f), Vector2.one * 1.3f, -5);
            if (_finaleUnlocked)
            {
                Spr("finaleGlow", _glow, new Color(1f, 0.9f, 0.6f, 0.5f),
                    new Vector3(pos.x, pos.y + 3f, 0f), Vector2.one * 9f, -8);
                Twinkleify(star, 1.4f, 0.2f, 0.4f, 0f);
            }
        }

        // ================================================================ helpers
        private SpriteRenderer Spr(string name, Sprite s, Color c, Vector3 pos, Vector2 size, int order)
        {
            var go = new GameObject(name); go.transform.SetParent(transform, false);
            go.transform.position = pos;
            go.transform.localScale = new Vector3(size.x / Mathf.Max(1e-4f, s.bounds.size.x),
                                                  size.y / Mathf.Max(1e-4f, s.bounds.size.y), 1f);
            var sr = go.AddComponent<SpriteRenderer>(); sr.sprite = s; sr.color = c; sr.sortingOrder = order;
            return sr;
        }
        private void Spr2(Transform parent, Sprite s, Color c, Vector3 lp, Vector2 size, int order)
        {
            var go = new GameObject("p"); go.transform.SetParent(parent, false);
            go.transform.localPosition = lp;
            go.transform.localScale = new Vector3(size.x / Mathf.Max(1e-4f, s.bounds.size.x),
                                                  size.y / Mathf.Max(1e-4f, s.bounds.size.y), 1f);
            var sr = go.AddComponent<SpriteRenderer>(); sr.sprite = s; sr.color = c; sr.sortingOrder = order;
        }
        private void Twinkleify(SpriteRenderer sr, float speed, float scaleAmp, float alphaAmp, float phase)
        {
            var tw = sr.gameObject.AddComponent<Twinkle>();
            tw.speed = speed; tw.scaleAmp = scaleAmp; tw.alphaAmp = alphaAmp; tw.phase = phase;
        }
    }

    /// <summary>A polyline route through the map's nodes, with arc-length sampling so the avatar
    /// walks it smoothly and so we can find where each node sits along the trail.</summary>
    public class MapRoute
    {
        private readonly List<Vector2> _pts;
        private readonly List<int> _nodePt;
        private readonly float[] _cum;        // cumulative length at each point
        public float Total { get; private set; }

        public MapRoute(List<Vector2> pts, List<int> nodePtIndex)
        {
            _pts = pts; _nodePt = nodePtIndex;
            _cum = new float[pts.Count];
            float acc = 0f;
            for (int i = 1; i < pts.Count; i++) { acc += Vector2.Distance(pts[i - 1], pts[i]); _cum[i] = acc; }
            Total = acc;
        }

        public float NodeDistance(int nodeIdx)
        {
            nodeIdx = Mathf.Clamp(nodeIdx, 0, _nodePt.Count - 1);
            return _cum[_nodePt[nodeIdx]];
        }

        /// <summary>Position at distance d along the trail; out tangentX = travel facing (+/-1).</summary>
        public Vector2 Sample(float d, out float tangentX)
        {
            d = Mathf.Clamp(d, 0f, Total);
            for (int i = 1; i < _pts.Count; i++)
            {
                if (d <= _cum[i] || i == _pts.Count - 1)
                {
                    float segLen = Mathf.Max(1e-4f, _cum[i] - _cum[i - 1]);
                    float t = Mathf.Clamp01((d - _cum[i - 1]) / segLen);
                    Vector2 a = _pts[i - 1], b = _pts[i];
                    tangentX = Mathf.Sign((b - a).x == 0 ? 1f : (b - a).x);
                    return Vector2.Lerp(a, b, t);
                }
            }
            tangentX = 1f;
            return _pts[_pts.Count - 1];
        }
    }

    /// <summary>A deathless avatar that strolls the map trail. Left/right drives distance ALONG the
    /// route (climbing hills, dipping through valleys), with a step-bob, facing flip, and a little
    /// hop when it dives into a painting.</summary>
    public class MapWalker : MonoBehaviour
    {
        private GameInput _input;
        private MapRoute _route;
        private float _dist, _bob, _hop, _hopT;
        private int _face = 1;
        private Transform _visual;
        private const float Speed = 9f;

        public void Init(GameInput input, MapRoute route, float startDist)
        {
            _input = input; _route = route; _dist = startDist;
            _visual = new GameObject("Visual").transform;
            _visual.SetParent(transform, false);

            Part("Body", TextureFactory.RoundedBox(28, 40, 12), Hex("#ff5c7a"), new Vector3(0, 0.9f, 0), new Vector3(0.9f, 1.3f, 1), 2);
            Part("Head", TextureFactory.Circle(28, 24), Hex("#ffe1c8"), new Vector3(0, 1.85f, 0), Vector3.one * 0.82f, 3);
            Part("Scarf", TextureFactory.RoundedBox(28, 12, 6), Hex("#ffd166"), new Vector3(0, 1.32f, 0), new Vector3(0.95f, 0.34f, 1), 3);
            Part("EyeL", TextureFactory.Circle(8, 24), Hex("#241018"), new Vector3(0.12f, 1.9f, 0), Vector3.one * 0.16f, 4);
            Part("EyeR", TextureFactory.Circle(8, 24), Hex("#241018"), new Vector3(0.3f, 1.9f, 0), Vector3.one * 0.16f, 4);

            Place();
        }

        public void TickMove()
        {
            float mx = _input != null ? _input.MoveX : 0f;
            _dist = Mathf.Clamp(_dist + mx * Speed * Time.deltaTime, 0f, _route.Total);
            if (mx > 0.15f) _face = 1; else if (mx < -0.15f) _face = -1;
            _bob += Mathf.Abs(mx) * Time.deltaTime * 11f;
            if (_hopT > 0f) _hopT -= Time.deltaTime;
            Place();
        }

        public void PlayEnterHop() { _hopT = 0.25f; }

        private void Place()
        {
            Vector2 p = _route.Sample(_dist, out _);
            float bob = Mathf.Abs(Mathf.Sin(_bob)) * 0.13f;
            float hop = _hopT > 0f ? Mathf.Sin((1f - _hopT / 0.25f) * Mathf.PI) * 0.9f : 0f;
            transform.position = new Vector3(p.x, p.y + bob + hop, 0f);
            if (_visual != null) _visual.localScale = new Vector3(_face, 1f, 1f);
        }

        private void Part(string name, Sprite sprite, Color color, Vector3 lp, Vector3 ls, int order)
        {
            var go = new GameObject(name); go.transform.SetParent(_visual, false);
            go.transform.localPosition = lp; go.transform.localScale = ls;
            var sr = go.AddComponent<SpriteRenderer>(); sr.sprite = sprite; sr.color = color; sr.sortingOrder = order;
        }

        private static Color Hex(string h) { ColorUtility.TryParseHtmlString(h, out var c); return c; }
    }
}
