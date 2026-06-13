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
        // The ring is the star: introduce it safely (~25%), DEVELOP it wider mid-level (~45%)
        // so its rhythm never goes quiet, then twist it through a sweeping saw at the climax
        // (~85%), then a fast victory lap. No flat dead stretches — every chunk asks for a
        // read, a jump, or a timing call.
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
             .RingGap(8).Rest(3)                    // the ring returns, wider now — develop the double-jump
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

        // ---------------------------------------------- 2  THE GREAT WAVE OFF KANAGAWA
        // The surge is the star. Classic-platformer arc — teach, develop, twist, climax:
        // the wave-pad fling is pure joy over safe planking first (Mario's "play with the
        // toy before it can kill you"), then carries you over open sea; the spray pearl
        // (mid-air dash refill) enters in act 3 and is rehearsed wider; the climax is
        // THE CLAW — the painting's frozen moment made playable: a void no single fling
        // can cross, boost off the curling lip, chain BOTH pearls, dash out from under
        // the claw. Every solid surface is shipwreck timber; the blue sea is death.
        private static LevelDefinition Level2()
        {
            var c = new LevelComposer(16);
            c.Start(6)
             // --- act 1: teach the surge (speed feels great before it's dangerous) ---
             .Pit(3).Rest(2)                        // first chop — the friendliest gap
             .Hops(new[] { 2, 3, 2 }).Rest(2)       // bobbing plank-rafts over the foam
             .Gauntlet("g", 5)                      // a beach crab patrols the deck — stomp lesson
             .BoostRun(16)                          // the wave-pad over safe planking: full flight, zero stakes
             .BoostGap(9).Rest(4)                   // now the surge carries you over open sea
             // --- act 2: develop on the rising swell ---
             .MobIsle(4, 6, 'g').MobIsle(5, 5, 'g') // island rhythm: leap into the patrol's window
             .SawGap(6)                             // a whirlpool sweeps the jump arc
             .Geyser(5)                             // a geyser spout launches you up the hull face
             .HopsCoin(new[] { 2, 3, 2 }, 2, 3)     // spray-top hops, coin on the high middle
             .FlyerGap(6)                           // a gull cuts across the arc — slip by or stomp it
             .BoostGap(9).Rest(3)                   // same fling, but now at height — a sea-fall on a miss
             .Drop(5)                               // commit to the trough
             // --- act 3: the dash twist (the pearl enters) ---
             .DashGap(7).Rest(2)                    // meet the spray pearl: dash, refill, dash again
             .MobIsle(5, 5, 'y')                    // a sea urchin holds the landing — timing only
             .TunnelHops(3)                         // under a capsized hull: feathered hops or bonk
             .PitCoin(6)                            // coin dangling low over the foam claws
             .DashGap(8).Rest(3)                    // a wider pearl gap — the climax skill, rehearsed
             // --- climax: THE CLAW ---
             .Claw(15)                              // fling off the lip, chain both pearls, escape
             // --- victory lap ---
             .Runout(12)
             .End();
            return new LevelDefinition("2", "The Great Wave off Kanagawa",
                "Ride the surge — wave-pads fling you, spray pearls refill the dash. The sea is not a place to stand.",
                38.0, 58.0, c.Rows());
        }

        // ---------------------------------------------- 3  THE SWING
        // Momentum is the star, and the pendulum is the verb. The signature toy is the
        // swing-seat (a ridden pendulum platform), and it gets a full teach -> develop ->
        // showcase -> climax arc the way every mechanic in this game earns one: a gentle
        // near-flush swing FIRST (the toy is joy before it can kill you), then over real
        // briar, then the long high chasm, then the painting's own swing at the climax.
        // Around it: note-block bounce chains, a fountain-jet lift up the garden terrace,
        // floor blades whose pockets are the no-stop line, and a warp-arbor route choice
        // where the fast line skips a blade lane that hides a coin — speed or 100%, pick one.
        private static LevelDefinition Level3()
        {
            var c = new LevelComposer(20);
            c.Start(5)
             // --- act 1: teach (bounce, then the swing — both eased in safely) ---
             .Hops(new[] { 2, 3 }).Rest(2)          // warmup precision over the briar
             .NoteRun(8)                            // meet the note-block, zero stakes
             .NoteBounce(3).Rest(2)                 // the blocks bridge a thorn pit
             .SwingRide(5).Rest(3)                  // MEET the swing: it bridges this short gap
                                                    // nearly flush — board off the ledge, ride,
                                                    // step off, no leap-of-faith over death yet
             // --- act 2: the pendulum garden climbs ---
             .SwingAlley(2)                         // floor blades skim fixed arcs
             .MobIsle(4, 6, 'g')                    // patrolled landing in the hedge
             .SwingRide(9).Rest(2)                  // DEVELOP: now the arc spans real briar —
                                                    // hop on at the near apex, leap from the far
             .SpringGap(6).Rest(2)                  // spring fling over thorns
             .FountainLift(5)                       // the fountain jet carries you up the terrace
             .HopsCoin(new[] { 2, 3, 2 }, 2, 3)     // sky hops, coin on the middle
             .FlyerGap(6)                           // a dove crosses the arc
             .SwingRide(12).Rest(2)                 // SHOWCASE: the long pendulum over the chasm
             .StairDown(2, 4, 'y')                  // hedgehog holds the descent
             // --- act 3: route choice, then the rhythm tightens ---
             .PortalSkip(16)                        // warp arbor or blade lane: speed vs coin
             .NoteSawChain(3, false)                // bounce the chain through the blades
             .SwingAlley(2, 6).Rest(3)              // closer pendulums, then a breath
             // --- climax: THE SWING ---
             .GrandSwing(16)                        // board at the bower, leap at full sweep
             // --- victory lap ---
             .Runout(12)
             .End();
            return new LevelDefinition("3", "The Swing",
                "Ride the pendulum — board the swing at one apex, leap from the other, and the garden's rhythm is yours.",
                42.0, 64.0, c.Rows());
        }

        // ---------------------------------------------- 4  THE SCREAM
        // Urgency is the star: the ground itself dies under you, and the world will not wait.
        // Classic teach → develop → twist → climax, all in The Scream's exclusive vocabulary:
        // crumble boards and donut boards (ground that lies), the diving SCREAMER (taught over
        // safe floor, then weaponized), the DRAG BELT (the world hauling you back into the pit
        // — only this level reverses a conveyor), and THE TEAR (a warp gate where reality
        // rips). The signature is THE BRIDGE — the painting's walkway as one long dying sprint
        // under two plunging screamers — and the climax trades ground for sky entirely: a
        // feather glide where the screamer mid-gap is both the threat AND the fast line (stomp
        // its head for the bounce). The hardest level: stopping is the only real mistake.
        private static LevelDefinition Level4()
        {
            var c = new LevelComposer(18);
            c.Start(6)
             // --- act 1: the ground lies (every lesson over survivable stakes) ---
             .Hops(new[] { 2, 3, 2 }).Rest(2)       // warmup precision over the first spikes
             .Bridge(2).Rest(2)                     // crumble boards, short and survivable
             .FallBridge(2).Rest(2)                 // donut boards shake, then drop away
             .DuckTunnel(4).Rest(2)                 // slide under a fallen beam — taught over safe ground
             .ScreamerPerch(9).Rest(2)              // meet the screamer over SAFE ground
             // --- act 2: the bridge dies over the fjord ---
             .MobIsle(4, 6, 'g')                    // patrolled island: leap into the window
             .DiveBridge(3)                         // crumble + a plunging screamer, combined
             .WallClimb(4, 2)                       // climb the fjord cliff
             .DuckTunnel(5).Rest(2)                 // duck the cliff overhang at height (developed)
             .HopsCoin(new[] { 2, 4, 2 }, 2, 3)     // sky hops, coin 1 on the tall middle
             .FlyerGap(6).Rest(3)                   // a flyer crosses the arc at height, then a ledge
             .DragBelt(8)                           // the world drags you back toward the pit
             .StairDown(2, 4, 'y')                  // spiked patroller holds the descent
             // --- act 3: reality tears, no safe footing left ---
             .Warp(6)                               // THE TEAR: the only way on is through
             .TunnelHops(3)                         // roof-capped feathered hops
             .PitCoin(6)                            // coin 2 dangling low over the spikes
             .TheBridge(5).Rest(2)                  // THE BRIDGE: the signature dying sprint
             // --- climax: the leap into the sky ---
             .GlideGap(10)                          // feather-drift clears it clean; stomp the diving screamer for the fast line
             // --- FINAL BOSS: the Scream itself ---
             .ScreamBossArena(24)                   // dodge its plunges, stomp its crown 3× — the sealed exit opens on its death
             .End();
            return new LevelDefinition("4", "The Scream",
                "The bridge is falling. Don't stop — then face the Scream itself: dodge its plunges and stomp it three times.",
                58.0, 82.0, c.Rows());
        }

    }
}
