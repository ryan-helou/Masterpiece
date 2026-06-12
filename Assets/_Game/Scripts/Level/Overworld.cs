using UnityEngine;

namespace Velocity
{
    /// <summary>
    /// The level-select, reimagined as an art museum (New-Super-Mario-Bros style world map). You
    /// walk a little avatar left/right down a gallery hall; each masterpiece hangs in a gilded frame
    /// above a plinth. Stand in front of one and a plaque shows its name, your PB and coins — press
    /// JUMP to step into the painting. A final pedestal at the end starts a full gallery run.
    ///
    /// Built and torn down by GameManager; driven by GameManager.Update via <see cref="Tick"/>.
    /// </summary>
    public class Overworld : MonoBehaviour
    {
        // museum palette (explicit — independent of the per-painting Palette)
        public static readonly Color WallColor  = Hex("#2a1d22");
        public static readonly Color WallTrim   = Hex("#3c2a31");
        public static readonly Color FloorColor = Hex("#4a3528");
        public static readonly Color FloorTrim  = Hex("#5e4636");
        public static readonly Color FrameGold  = Hex("#c9a24a");
        public static readonly Color FrameDark  = Hex("#17110b");
        public static readonly Color Plinth     = Hex("#6b6258");
        public static readonly Color PlinthCap  = Hex("#928979");
        public static readonly Color GlowWarm   = Hex("#ffe6b0");

        private const float Spacing = 13f;
        private const float FirstX  = 9f;
        private const float FloorY  = 2f;
        private const float FrameY  = 8.4f;
        private const float FrameH  = 4.6f;
        private const float Reach   = 3.2f;   // how close you must stand to enter

        private GameInput _input;
        private GameManager _gm;
        private UIManager _ui;
        private CameraController _cam;
        private OverworldWalker _walker;

        private float[] _nodeX;
        private int _levelCount, _nodeCount;
        private int _near = -2;

        private static Color Hex(string h) { ColorUtility.TryParseHtmlString(h, out var c); return c; }

        public void Setup(GameInput input, GameManager gm, UIManager ui, CameraController cam, int startNode)
        {
            _input = input; _gm = gm; _ui = ui; _cam = cam;

            _levelCount = LevelLibrary.WorldAt(0).Levels.Count;
            _nodeCount = _levelCount + 1;                 // + the full-run pedestal
            _nodeX = new float[_nodeCount];
            for (int i = 0; i < _nodeCount; i++) _nodeX[i] = FirstX + i * Spacing;
            float endX = _nodeX[_nodeCount - 1] + FirstX;

            BuildHall(endX);
            for (int i = 0; i < _levelCount; i++) BuildFrame(i, _nodeX[i]);
            BuildFullRunPedestal(_nodeX[_nodeCount - 1]);

            var wgo = new GameObject("Walker");
            wgo.transform.SetParent(transform, false);
            _walker = wgo.AddComponent<OverworldWalker>();
            int startIdx = Mathf.Clamp(startNode, 0, _levelCount - 1);
            _walker.Init(input, new Vector2(_nodeX[startIdx], FloorY), 2f, endX - 2f);

            var bounds = new Bounds(new Vector3(endX / 2f, 8f, 0f), new Vector3(endX + 12f, 12f, 1f));
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

            float px = _walker.transform.position.x;
            int nearest = 0; float bestD = float.MaxValue;
            for (int i = 0; i < _nodeCount; i++)
            {
                float d = Mathf.Abs(px - _nodeX[i]);
                if (d < bestD) { bestD = d; nearest = i; }
            }
            int near = bestD <= Reach ? nearest : -1;
            if (near != _near) UpdatePlaque(near);

            if (near >= 0 && _input.JumpPressed)
            {
                if (Sfx.Instance != null) Sfx.Instance.PlayClick();
                if (near < _levelCount) _gm.RequestStartLevel(LevelLibrary.GlobalIndex(0, near));
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
                    "all eight masterpieces, one timer", best, "", true, true);
            }
        }

        // ================================================================ build
        private void BuildHall(float endX)
        {
            // back wall
            Box("Wall", new Vector3(endX / 2f, 8f, 0f), new Vector2(endX + 24f, 26f), WallColor, -30);
            // top + bottom mouldings
            Box("Cornice", new Vector3(endX / 2f, 13.2f, 0f), new Vector2(endX + 24f, 0.5f), WallTrim, -28);
            Box("Skirting", new Vector3(endX / 2f, 2.3f, 0f), new Vector2(endX + 24f, 0.45f), FloorTrim, -22);
            // parquet floor
            Box("Floor", new Vector3(endX / 2f, 1f, 0f), new Vector2(endX + 24f, 2f), FloorColor, -24);
            // floorboard seams for a parquet feel
            for (float x = 0; x < endX + 4; x += 2.2f)
                Box("seam", new Vector3(x, 1f, 0f), new Vector2(0.06f, 2f), FloorTrim, -23);
        }

        private void BuildFrame(int i, float x)
        {
            var theme = PaintingThemes.For(i);
            var tex = theme != null ? Resources.Load<Texture2D>(theme.ResourceKey) : null;

            float w = FrameH;   // default square-ish if no image
            if (tex != null && tex.height > 0) w = FrameH * tex.width / tex.height;
            w = Mathf.Clamp(w, 2.6f, 7.4f);

            // gilded frame + dark mat behind the canvas
            Box("FrameGold", new Vector3(x, FrameY, 0f), new Vector2(w + 0.7f, FrameH + 0.7f), FrameGold, -12);
            Box("FrameMat",  new Vector3(x, FrameY, 0f), new Vector2(w + 0.28f, FrameH + 0.28f), FrameDark, -11);

            // the painting itself
            var go = new GameObject("Canvas");
            go.transform.SetParent(transform, false);
            var sr = go.AddComponent<SpriteRenderer>();
            if (tex != null)
            {
                tex.wrapMode = TextureWrapMode.Clamp; tex.filterMode = FilterMode.Bilinear;
                var spr = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f), 100f);
                sr.sprite = spr;
                go.transform.localScale = Vector3.one * (FrameH / spr.bounds.size.y);
            }
            else { sr.sprite = TextureFactory.WhitePixel(); sr.color = theme != null ? theme.SkyNear : Palette.BackdropNear; go.transform.localScale = new Vector3(w, FrameH, 1f); }
            go.transform.position = new Vector3(x, FrameY, 0f);
            sr.sortingOrder = -10;

            // soft floor glow beneath the piece
            Box("Glow", new Vector3(x, 2.2f, 0f), new Vector2(w + 1.2f, 0.5f),
                new Color(GlowWarm.r, GlowWarm.g, GlowWarm.b, 0.16f), -21);

            // plinth + a "cleared" gem on top
            Box("Plinth", new Vector3(x, 3.0f, 0f), new Vector2(1.7f, 2.0f), Plinth, -8);
            Box("PlinthCap", new Vector3(x, 4.05f, 0f), new Vector2(2.0f, 0.3f), PlinthCap, -7);

            var lvl = LevelLibrary.WorldAt(0).Levels[i];
            bool cleared = Leaderboard.HasCompleted(lvl.Id);
            var gem = new GameObject("Gem");
            gem.transform.SetParent(transform, false);
            var gsr = gem.AddComponent<SpriteRenderer>();
            gsr.sprite = TextureFactory.Circle(20, 24);
            gsr.color = cleared ? (theme != null ? theme.Coin : Palette.Coin) : new Color(1, 1, 1, 0.12f);
            gem.transform.position = new Vector3(x, 4.5f, 0f);
            gem.transform.localScale = Vector3.one * 0.5f;
            gsr.sortingOrder = -6;
        }

        private void BuildFullRunPedestal(float x)
        {
            // a tall banner instead of a painting
            Box("RunBannerGold", new Vector3(x, FrameY, 0f), new Vector2(FrameH * 0.8f + 0.6f, FrameH + 0.7f), FrameGold, -12);
            Box("RunBannerDark", new Vector3(x, FrameY, 0f), new Vector2(FrameH * 0.8f + 0.2f, FrameH + 0.25f), Hex("#241616"), -11);
            // three stacked "wave" bars to read as a marathon icon
            for (int k = 0; k < 3; k++)
                Box("bar", new Vector3(x, FrameY + 1.1f - k * 1.1f, 0f), new Vector2(FrameH * 0.55f, 0.55f),
                    Hex("#c9a24a"), -10);
            Box("Glow", new Vector3(x, 2.2f, 0f), new Vector2(4.4f, 0.5f),
                new Color(GlowWarm.r, GlowWarm.g, GlowWarm.b, 0.16f), -21);
            Box("Plinth", new Vector3(x, 3.0f, 0f), new Vector2(1.7f, 2.0f), Plinth, -8);
            Box("PlinthCap", new Vector3(x, 4.05f, 0f), new Vector2(2.0f, 0.3f), PlinthCap, -7);
        }

        private void Box(string name, Vector3 pos, Vector2 size, Color color, int order)
        {
            var go = new GameObject(name);
            go.transform.SetParent(transform, false);
            go.transform.position = new Vector3(pos.x, pos.y, 0f);
            go.transform.localScale = new Vector3(size.x, size.y, 1f);
            var sr = go.AddComponent<SpriteRenderer>();
            sr.sprite = TextureFactory.WhitePixel();
            sr.color = color;
            sr.sortingOrder = order;
        }
    }

    /// <summary>A lightweight, deathless avatar that strolls the gallery floor. No real physics —
    /// just clamped horizontal walking with a tiny step-bob and facing flip.</summary>
    public class OverworldWalker : MonoBehaviour
    {
        private GameInput _input;
        private float _minX, _maxX, _x, _baseY, _bob;
        private int _face = 1;
        private Transform _visual;

        public void Init(GameInput input, Vector2 feet, float minX, float maxX)
        {
            _input = input; _minX = minX; _maxX = maxX; _x = feet.x; _baseY = feet.y;
            transform.position = new Vector3(_x, _baseY, 0f);

            _visual = new GameObject("Visual").transform;
            _visual.SetParent(transform, false);

            // body
            Part(_visual, "Body", TextureFactory.RoundedBox(28, 40, 12), Hex("#ff5c7a"), new Vector3(0, 0.9f, 0), new Vector3(0.9f, 1.3f, 1), 2);
            // head
            Part(_visual, "Head", TextureFactory.Circle(28, 24), Hex("#ffe1c8"), new Vector3(0, 1.85f, 0), Vector3.one * 0.82f, 3);
            // a jaunty scarf accent
            Part(_visual, "Scarf", TextureFactory.RoundedBox(28, 12, 6), Hex("#ffd166"), new Vector3(0, 1.32f, 0), new Vector3(0.95f, 0.34f, 1), 3);
            // eyes (offset toward facing direction)
            Part(_visual, "EyeL", TextureFactory.Circle(8, 24), Hex("#241018"), new Vector3(0.12f, 1.9f, 0), Vector3.one * 0.16f, 4);
            Part(_visual, "EyeR", TextureFactory.Circle(8, 24), Hex("#241018"), new Vector3(0.3f, 1.9f, 0), Vector3.one * 0.16f, 4);
        }

        public void TickMove()
        {
            float mx = _input.MoveX;
            _x = Mathf.Clamp(_x + mx * 7.2f * Time.deltaTime, _minX, _maxX);
            if (mx > 0.15f) _face = 1; else if (mx < -0.15f) _face = -1;

            _bob += Mathf.Abs(mx) * Time.deltaTime * 11f;
            float y = _baseY + Mathf.Abs(Mathf.Sin(_bob)) * 0.13f;
            transform.position = new Vector3(_x, y, 0f);
            if (_visual != null) _visual.localScale = new Vector3(_face, 1f, 1f);
        }

        private static void Part(Transform parent, string name, Sprite sprite, Color color, Vector3 lp, Vector3 ls, int order)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent, false);
            go.transform.localPosition = lp;
            go.transform.localScale = ls;
            var sr = go.AddComponent<SpriteRenderer>();
            sr.sprite = sprite; sr.color = color; sr.sortingOrder = order;
        }

        private static Color Hex(string h) { ColorUtility.TryParseHtmlString(h, out var c); return c; }
    }
}
