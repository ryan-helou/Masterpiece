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

        /// <summary>An INVISIBLE full-height world edge: solid collision, no art (so the level
        /// doesn't end in an ugly dark soil pillar). Used for the left/right map bounds.</summary>
        public LevelDraw Bound(int x)
        {
            for (int y = 0; y < _h; y++) Set(x, y, 'B');
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
        // The ring is the star: introduce it safely, develop it among enemy-timed islands,
        // twist it through a sweeping saw at the climax (~85%), then a fast victory lap.
        // No flat dead stretches — every chunk asks for a read, a jump, or a timing call.
        private static LevelDefinition Level1()
        {
            var c = new LevelComposer(16);
            c.Start(6)
             // --- act 1: teach (pure movement, nothing cycles yet) ---
             .Pit(3).Rest(3)                        // the first, friendliest gap
             .Gauntlet("g", 4)                      // one slow walker — the stomp lesson
             .DuckTunnel(4).Rest(2)                 // slide the overhang, stand back up
             .Hops(new[] { 2, 3, 2 }).Rest(2)       // first platforms over spikes
             .RingGap(7).Rest(4)                    // meet the swirl-ring (room to overshoot)
             // --- act 2: develop (enemies own the landings) ---
             .MobIsle(4, 6, 'g').MobIsle(5, 5, 'g') // island rhythm: leap into the patrol's window
             .SawHall(1)                            // the blade shows itself over safe ground...
             .SawGap(6).Rest(3)                     // ...then guards a pit
             .WallClimb(4, 2)                       // climb into the sky
             .HighRoad(18)                          // fast clean line above, slow spiky lane below
             .FlyerGap(6)                           // a flyer crosses your arc — slip by or stomp it
             .StairDown(2, 4, 'g')                  // committed hops back down, speed for free
             // --- act 3: twist and climax ---
             .TunnelHops(3)                         // short hops only — the roof punishes greed
             .MobIsle(4, 6, 'y')                    // spiked patroller: no stomp, timing only
             .SpringCoin().Rest(3)                  // breather — the calm before the leap
             .RingSawGap(9)                         // THE jump: ring double-jump past the blade
             // --- act 4: victory lap ---
             .Runout(14)                            // full-speed spike hops to the flag
             .End();
            return new LevelDefinition("1", "Starry Night",
                "Drift through the night — jump into a swirl-ring and tap jump again for a second boost.",
                32.0, 52.0, c.Rows());
        }

        // ---------------------------------------------- 2  THE CREATION OF ADAM
        // "The reach" is the star: the spark-pad fling is learned over safe ground, developed
        // over pits, then twisted at the climax — a gap where the fling alone falls short and
        // only a mid-air dash closes the last almost-touching inch.
        private static LevelDefinition Level2()
        {
            var c = new LevelComposer(16);
            c.Start(6)
             // --- act 1: teach the fling ---
             .Hops(new[] { 2, 3, 2 }).Rest(2)       // warmup platforms
             .MobIsle(4, 6, 'g')                    // a patrolled landing to read
             .BoostRun(16)                          // the spark-pad, over safe ground (full flight)
             .BoostGap(9).Rest(5)                   // now it carries you over spikes
             // --- act 2: develop at height ---
             .SawGap(6)                             // blade-timed jump
             .WallClimb(5, 2)                       // a taller chimney than Level 1's
             .HopsCoin(new[] { 2, 3, 2 }, 2, 3)     // sky hops, coin on the high middle
             .FlyerGap(6)                           // a flyer crosses the arc
             .Drop(5)                               // commit to the fall
             // --- act 3: the dash twist ---
             .DashGap(7).Rest(2)                    // meet the mid-air refill orb
             .MobIsle(5, 5, 'y')                    // spiked patroller, timing only
             .PitCoin(6)                            // coin dangling low over the spikes
             .BoostGap(10).Rest(3)                  // a longer fling, then a breath
             // --- climax: the reach ---
             .Reach(12)                             // fingertip to fingertip, spark between
             // --- victory lap ---
             .Runout(12)
             .End();
            return new LevelDefinition("2", "The Creation of Adam",
                "Bridge the gap. The spark-pads fling you across — almost touching is never enough.",
                36.0, 56.0, c.Rows());
        }

        // ---------------------------------------------- 3  THE SWING
        // Momentum is the star: bounce is taught for free, developed over spikes, lifted
        // skyward by a spring tower, then twisted at the climax — a note-block chain whose
        // junction arcs are swept by pendulum saws. Keep the swing alive or fall out of it.
        private static LevelDefinition Level3()
        {
            var c = new LevelComposer(18);
            c.Start(6)
             // --- act 1: teach the bounce ---
             .Hops(new[] { 2, 3 }).Rest(2)          // warmup
             .NoteRun(8)                            // note-blocks over safe ground
             .NoteBounce(3).Rest(3)                 // now the blocks bridge a pit
             // --- act 2: the pendulum garden ---
             .SwingAlley(2)                         // blades skim the floor in fixed arcs
             .MobIsle(4, 6, 'g')                    // patrolled landing
             .SpringGap(6).Rest(2)                  // spring fling over spikes
             .SpringTower(5)                        // ride the full bounce up the column
             .HopsCoin(new[] { 2, 3, 2 }, 2, 3)     // sky hops, coin on the middle
             .FlyerGap(6).Rest(3)                   // flyer in the arc, then a landing
             .SawGap(7)                             // wide blade-timed jump at height
             .StairDown(2, 4, 'y')                  // spiked patroller holds the descent
             // --- act 3: tighten the rhythm ---
             .SwingAlley(2, 6)                      // closer pendulums, tighter pockets
             .SpringCoin().Rest(3)                  // coin at the apex, then a breath
             // --- climax: the swing itself ---
             .NoteSawChain(4)                       // bounce the chain through the blades
             // --- victory lap ---
             .Runout(12)
             .End();
            return new LevelDefinition("3", "The Swing",
                "Keep the swing alive — bounce the note-blocks and springs, carry your momentum through the garden.",
                40.0, 60.0, c.Rows());
        }

        // ---------------------------------------------- 4  THE SCREAM
        // Urgency is the star: ground that dies under you. Crumble boards are re-taught fast,
        // developed with donut drops at height, twisted by a diving screamer over the bridge,
        // and the climax trades ground for sky — a feather glide through the swoop. The
        // hardest level: stopping is the only real mistake.
        private static LevelDefinition Level4()
        {
            var c = new LevelComposer(18);
            c.Start(6)
             // --- act 1: the ground gives way ---
             .Hops(new[] { 2, 3, 2 }).Rest(2)       // warmup
             .Bridge(2).Rest(3)                     // crumble boards, short and survivable
             .MobIsle(4, 6, 'g')                    // patrolled landing
             .FallBridge(2).Rest(2)                 // donut boards drop away
             // --- act 2: pressure at height ---
             .SawGap(7)                             // wide blade-timed jump
             .WallClimb(4, 2)                       // climb the fjord wall
             .HopsCoin(new[] { 2, 4, 2 }, 2, 3)     // sky hops, coin on the tall middle
             .FlyerGap(6)                           // flyer in the arc
             .DiveBridge(3)                         // the screamer plunges as you cross
             .StairDown(2, 4, 'y')                  // spiked patroller on the way down
             // --- act 3: no safe footing left ---
             .TunnelHops(3)                         // roof-capped short hops
             .PitCoin(7)                            // coin dangling low over the spikes
             .Bridge(3).Rest(3)                     // crumble sprint, then the cliff edge
             // --- climax: leap into the sky ---
             .GlideGap(11)                          // feather-drift past the diving screamer
             // --- victory lap ---
             .Runout(12)
             .End();
            return new LevelDefinition("4", "The Scream",
                "The bridge is falling. Don't stop — the boards drop the instant you set foot on them.",
                42.0, 62.0, c.Rows());
        }

    }
}
