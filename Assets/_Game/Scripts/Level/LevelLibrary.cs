using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Velocity
{
    /// <summary>
    /// Authoring helper: a char grid with a bottom-left origin (y grows up) and convenience
    /// drawing ops, so levels are built precisely in code rather than miscounted ASCII art.
    /// </summary>
    public class LevelDraw
    {
        private readonly int _w, _h;
        private readonly char[,] _g;   // [x, y] with y from bottom

        public LevelDraw(int w, int h)
        {
            _w = w; _h = h;
            _g = new char[w, h];
            for (int x = 0; x < w; x++)
                for (int y = 0; y < h; y++)
                    _g[x, y] = ' ';
        }

        public LevelDraw Set(int x, int y, char c)
        {
            if (x >= 0 && x < _w && y >= 0 && y < _h) _g[x, y] = c;
            return this;
        }

        public LevelDraw Fill(int x0, int y0, int w, int h, char c)
        {
            for (int x = x0; x < x0 + w; x++)
                for (int y = y0; y < y0 + h; y++)
                    Set(x, y, c);
            return this;
        }

        /// <summary>Solid floor band [x0..x1] inclusive, thickness rows from y=0.</summary>
        public LevelDraw Floor(int x0, int x1, int thickness = 2)
        {
            for (int x = x0; x <= x1; x++)
                for (int y = 0; y < thickness; y++)
                    Set(x, y, '#');
            return this;
        }

        /// <summary>A solid block at [x..x+w-1] x [y..y+h-1].</summary>
        public LevelDraw Block(int x, int y, int w, int h = 1) => Fill(x, y, w, h, '#');

        /// <summary>Carve a spike pit through a 2-thick floor over [x0..x1] inclusive.</summary>
        public LevelDraw Pit(int x0, int x1)
        {
            for (int x = x0; x <= x1; x++)
            {
                Set(x, 1, ' ');
                Set(x, 0, '^');
            }
            return this;
        }

        public LevelDraw Spikes(int x0, int x1, int y = 0)
        {
            for (int x = x0; x <= x1; x++) Set(x, y, '^');
            return this;
        }

        public LevelDraw OneWay(int x0, int x1, int y)
        {
            for (int x = x0; x <= x1; x++) Set(x, y, '-');
            return this;
        }

        public LevelDraw Wall(int x)
        {
            for (int y = 0; y < _h; y++) Set(x, y, '#');
            return this;
        }

        public LevelDraw Spawn(int x, int y) => Set(x, y, 'P');
        public LevelDraw Goal(int x, int y) => Set(x, y, 'G');

        // World-2 mechanics
        public LevelDraw Coin(int x, int y) => Set(x, y, 'o');
        public LevelDraw Spring(int x, int y) => Set(x, y, 'b');
        public LevelDraw Crumble(int x0, int x1, int y)
        {
            for (int x = x0; x <= x1; x++) Set(x, y, 'c');
            return this;
        }
        public LevelDraw ConveyorR(int x0, int x1, int y)
        {
            for (int x = x0; x <= x1; x++) Set(x, y, '>');
            return this;
        }
        public LevelDraw ConveyorL(int x0, int x1, int y)
        {
            for (int x = x0; x <= x1; x++) Set(x, y, '<');
            return this;
        }
        public LevelDraw SawH(int x, int y) => Set(x, y, 's');
        public LevelDraw SawV(int x, int y) => Set(x, y, 'S');

        public string[] Rows()
        {
            var rows = new string[_h];
            for (int ry = 0; ry < _h; ry++)
            {
                int y = _h - 1 - ry;       // top row first
                var sb = new StringBuilder(_w);
                for (int x = 0; x < _w; x++) sb.Append(_g[x, y]);
                rows[ry] = sb.ToString();
            }
            return rows;
        }
    }

    /// <summary>A named group of levels.</summary>
    public class World
    {
        public readonly string Id;
        public readonly string Name;
        public readonly List<LevelDefinition> Levels;
        public World(string id, string name, List<LevelDefinition> levels)
        {
            Id = id; Name = name; Levels = levels;
        }
    }

    /// <summary>All worlds and the flat level list. Edit / add levels freely.</summary>
    public static class LevelLibrary
    {
        private static List<World> _worlds;
        private static List<LevelDefinition> _flat;

        private static void Ensure()
        {
            if (_worlds != null) return;
            _worlds = Build();
            _flat = new List<LevelDefinition>();
            foreach (var w in _worlds) _flat.AddRange(w.Levels);
        }

        public static IReadOnlyList<World> Worlds { get { Ensure(); return _worlds; } }
        public static int WorldCount { get { Ensure(); return _worlds.Count; } }
        public static World WorldAt(int wi) { Ensure(); return _worlds[Mathf.Clamp(wi, 0, _worlds.Count - 1)]; }

        public static int Count { get { Ensure(); return _flat.Count; } }
        public static LevelDefinition Get(int i) { Ensure(); return _flat[Mathf.Clamp(i, 0, _flat.Count - 1)]; }

        public static List<string> AllIds()
        {
            Ensure();
            var ids = new List<string>();
            foreach (var l in _flat) ids.Add(l.Id);
            return ids;
        }

        /// <summary>(world, local) -> flat index.</summary>
        public static int GlobalIndex(int worldIdx, int localIdx)
        {
            Ensure();
            int g = 0;
            for (int w = 0; w < worldIdx && w < _worlds.Count; w++) g += _worlds[w].Levels.Count;
            return g + localIdx;
        }

        /// <summary>flat index -> (world, local).</summary>
        public static (int world, int local) Locate(int global)
        {
            Ensure();
            int g = global;
            for (int w = 0; w < _worlds.Count; w++)
            {
                if (g < _worlds[w].Levels.Count) return (w, g);
                g -= _worlds[w].Levels.Count;
            }
            int last = _worlds.Count - 1;
            return (last, _worlds[last].Levels.Count - 1);
        }

        private static List<World> Build()
        {
            // ONE world — eight masterpieces. Each level is themed to a famous painting (art +
            // palette via PaintingThemes, indexed by level order) AND its mechanics. Order here MUST
            // match PaintingThemes.All. Exactly 3 coins per level, always in nasty spots.
            var worlds = new List<World>
            {
                new World("G", "The Gallery",
                    new List<LevelDefinition>
                    {
                        Level1(), Level2(), Level3(), Level4(),
                    }),
            };
            var custom = LevelAssetLoader.LoadCustomWorld();
            if (custom != null && custom.Levels.Count > 0) worlds.Add(custom);
            return worlds;
        }

        // Toy legend: warp portals (p/q/r), jump rings (j), boost pads (k/K), note/bounce blocks (n),
        // falling platforms (l), glide feathers (w), wall-climb chimneys, saws (S), mobs (g/f/y).

        // ---------------------------------------------- 1  STARRY NIGHT
        // dreamy low-gravity night: swirl-rings to double-jump, feathers to glide the long gaps.
        private static LevelDefinition Level1()
        {
            var c = new LevelComposer(16);
            c.Start(6).Power('u').Hops(new[] { 2, 3, 2 }).Gauntlet("g f").RingGap(9).Power('w')
             .Hops(new[] { 2, 3 }).Gauntlet("f g f").HopsCoin(new[] { 3, 2, 4 }).RingGap(10)
             .Gauntlet("g f").Bridge(3).PitCoin(6).Gauntlet("f g").RingGap(9).Hops(new[] { 2, 3, 2 })
             .Gauntlet("g f").SpringCoin().Gauntlet("f g").Rest(3).End();
            return new LevelDefinition("1", "Starry Night",
                "Drift through the night — jump into a swirl-ring and tap jump again for a second boost.",
                38.0, 58.0, c.Rows());
        }

        // ---------------------------------------------- 2  THE CREATION OF ADAM
        // "the reach": precise hops and spark-pad boosts to bridge the famous gap.
        private static LevelDefinition Level2()
        {
            var c = new LevelComposer(16);
            c.Start(6).Power('u').Hops(new[] { 2, 3, 2 }).Gauntlet("g f").BoostGap(9).Gauntlet("f g")
             .HopsCoin(new[] { 3, 2, 4, 2 }).Power('w').BoostGap(9).Gauntlet("g z g").PitCoin(6)
             .Rest(3).DashGap(7).Gauntlet("f g").BoostGap(9).Hops(new[] { 2, 3 }).Gauntlet("g f")
             .SpringCoin().Gauntlet("f g").Rest(3).End();
            return new LevelDefinition("2", "The Creation of Adam",
                "Bridge the gap. The spark-pads fling you across — almost touching is never enough.",
                42.0, 62.0, c.Rows());
        }

        // ---------------------------------------------- 3  THE SWING
        // momentum / pendulum: note-blocks, springs and swinging saws — keep the swing alive.
        private static LevelDefinition Level3()
        {
            var c = new LevelComposer(18);
            c.Start(6).Hops(new[] { 2, 3 }).Gauntlet("g f").NoteBounce(3).Power('u').Gauntlet("f g")
             .SpringGap(6).HopsCoin(new[] { 3, 2, 4, 2 }).Gauntlet("g z y").NoteBounce(4).Power('w')
             .SawHall(2).Gauntlet("f g").PitCoin(7).SpringGap(6).Gauntlet("g f").SpringCoin()
             .NoteBounce(3).Gauntlet("f g").Rest(3).End();
            return new LevelDefinition("3", "The Swing",
                "Keep the swing alive — bounce the note-blocks and springs, carry your momentum through the garden.",
                46.0, 66.0, c.Rows());
        }

        // ---------------------------------------------- 4  THE SCREAM
        // disorientation: crumbling and falling bridges over the fjord — never stop moving.
        private static LevelDefinition Level4()
        {
            var c = new LevelComposer(18);
            c.Start(6).Power('w').Bridge(3).Gauntlet("g f").FallBridge(2).HopsCoin(new[] { 2, 4, 2, 3 })
             .Power('u').Gauntlet("f g").FallBridge(3).SawHall(2).Gauntlet("g z y").PitCoin(7)
             .FallBridge(2).Gauntlet("g f").Bridge(4).Gauntlet("f g").SpringCoin().FallBridge(2)
             .Gauntlet("g f").Rest(3).End();
            return new LevelDefinition("4", "The Scream",
                "The bridge is falling. Don't stop — the boards drop the instant you set foot on them.",
                50.0, 70.0, c.Rows());
        }

    }
}
