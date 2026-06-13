namespace Velocity
{
    /// <summary>
    /// Builds dense, hard levels left-to-right from challenge chunks. The default surface under a
    /// challenge is SPIKES (a miss = death). Supports elevation (the floor can rise/fall), enabling
    /// REQUIRED wall-jump chimneys you must climb to progress. Exactly 3 coins per level, in nasty
    /// spots. Floor surface starts at world y=2.
    /// </summary>
    public class LevelComposer
    {
        public readonly LevelDraw D;
        public readonly int H;
        public int X;
        public int Coins;

        private int _floor = 2;        // world Y of the current floor surface
        private int _warpCount;        // cycles portal chars p/q/r so pairs link cleanly
        private const int Pad = 800;

        public LevelComposer(int height)
        {
            H = height;
            D = new LevelDraw(Pad, height);
            X = 0;
            D.Bound(0);          // invisible left edge (no dark soil pillar)
            X = 1;
        }

        // ---- primitives (relative to the current floor) ----
        private void Ground(int x0, int n) { for (int i = 0; i < n; i++) for (int y = 0; y < _floor; y++) D.Set(x0 + i, y, '#'); }
        private void Spikes(int x0, int n) { for (int i = 0; i < n; i++) D.Set(x0 + i, 0, '^'); }
        private void Plat(int x0, int w, int topY) { int yb = topY - 1; for (int i = 0; i < w; i++) D.Set(x0 + i, yb, '#'); }

        public LevelComposer Mob(int ahead, int cellRow, char e) { D.Set(X + ahead, cellRow, e); return this; }
        public LevelComposer Coin(int ahead, int cellRow) { D.Set(X + ahead, cellRow, 'o'); Coins++; return this; }

        // ============================ chunks ============================

        public LevelComposer Start(int n = 6) { Ground(X, n); D.Spawn(X + 1, _floor); X += n; return this; }
        public LevelComposer Rest(int n) { Ground(X, n); X += n; return this; }
        public LevelComposer Pit(int n) { Spikes(X, n); X += n; return this; }

        public LevelComposer Hops(int[] heights, int pw = 2, int g = 2)
        {
            int region = heights.Length * pw + (heights.Length - 1) * g;
            Spikes(X, region);
            int x = X;
            foreach (var h in heights) { Plat(x, pw, _floor + h); x += pw + g; }
            X += region;
            return this;
        }

        public LevelComposer HopsCoin(int[] heights, int pw = 2, int g = 2)
        {
            int mid = heights.Length / 2;
            int region = heights.Length * pw + (heights.Length - 1) * g;
            Spikes(X, region);
            int x = X;
            for (int k = 0; k < heights.Length; k++)
            {
                Plat(x, pw, _floor + heights[k]);
                if (k == mid) { D.Set(x, _floor + heights[mid] + 3, 'o'); Coins++; }
                x += pw + g;
            }
            X += region;
            return this;
        }

        public LevelComposer PitCoin(int n)
        {
            Spikes(X, n);
            D.Set(X + n / 2, _floor, 'o'); Coins++;       // dangling low over the spikes
            X += n;
            return this;
        }

        public LevelComposer SpringCoin()
        {
            Ground(X, 6);
            D.Set(X + 3, _floor, 'b');
            D.Set(X + 3, _floor + 5, 'o'); Coins++;        // near the apex of the bounce
            X += 6;
            return this;
        }

        public LevelComposer Bridge(int count, int pw = 2, int g = 2)
        {
            int region = count * pw + (count - 1) * g;
            Spikes(X, region);
            int x = X;
            for (int k = 0; k < count; k++) { for (int i = 0; i < pw; i++) D.Set(x + i, _floor - 1, 'c'); x += pw + g; }
            X += region;
            return this;
        }

        public LevelComposer SawHall(int count, int spacing = 6)
        {
            int region = count * spacing + spacing;
            Ground(X, region);
            for (int k = 0; k < count; k++) D.Set(X + spacing + k * spacing, _floor + 3, 'S');
            X += region;
            return this;
        }

        public LevelComposer Gauntlet(string pattern, int spacing = 5)
        {
            int region = pattern.Length * spacing + spacing;
            Ground(X, region);
            for (int k = 0; k < pattern.Length; k++)
            {
                char e = pattern[k];
                int cx = X + spacing + k * spacing;
                if (e == 'g' || e == 'y') D.Set(cx, _floor, e);          // ground patrollers
                else if (e == 'f') D.Set(cx, _floor + 3, e);             // flyer
                else if (e == 'z') D.Set(cx, _floor + 4, e);             // diver (swoops down)
            }
            X += region;
            return this;
        }

        public LevelComposer Pillars(int count, int gap = 3)
        {
            int region = count * 3 + (count - 1) * gap;
            Spikes(X, region);
            int x = X;
            for (int k = 0; k < count; k++)
            {
                for (int y = 0; y < _floor; y++) for (int i = 0; i < 3; i++) D.Set(x + i, y, '#');
                D.Set(x + 1, _floor, 'g');
                x += 3 + gap;
            }
            X += region;
            return this;
        }

        public LevelComposer Conveyor(int n, int dir)
        {
            Ground(X, n);
            char belt = dir > 0 ? '>' : '<';
            for (int i = 0; i < n; i++) D.Set(X + i, _floor - 1, belt);
            D.Set(X + n / 2, _floor + 3, 'S');
            X += n;
            return this;
        }

        public LevelComposer SpringGap(int n)
        {
            Ground(X, 2); D.Set(X + 1, _floor, 'b'); X += 2;
            Spikes(X, n); X += n;
            return this;
        }

        /// <summary>A pit too wide for one dash — a mid-air dash orb refills you to dash again.</summary>
        public LevelComposer DashGap(int n)
        {
            Spikes(X, n);
            D.Set(X + n / 2, _floor + 2, 'd');     // grab mid-flight to refill the dash
            X += n;
            return this;
        }

        // ============================ new toys ============================

        /// <summary>Warp portal: a wall blocks the path, walk into the portal to teleport past it.</summary>
        public LevelComposer Warp(int barrierW = 6)
        {
            char ch = (char)('p' + (_warpCount++ % 3));
            Ground(X, 3);
            D.Set(X + 2, _floor + 1, ch);                       // entry portal you walk into
            X += 3;
            for (int y = 0; y < H; y++) D.Set(X, y, '#');       // an impassable wall
            Spikes(X, barrierW); X += barrierW;
            Ground(X, 3);
            D.Set(X, _floor + 1, ch);                           // exit portal beyond the wall
            X += 3;
            return this;
        }

        /// <summary>Wide pit with a jump ring near the start — jump, hit the ring, jump again for a boost.</summary>
        public LevelComposer RingGap(int n)
        {
            Spikes(X, n);
            D.Set(X + 2, _floor + 3, 'j');
            X += n;
            return this;
        }

        /// <summary>A dash panel that flings you across a very wide spike pit.</summary>
        public LevelComposer BoostGap(int n)
        {
            Ground(X, 2); D.Set(X + 1, _floor, 'k'); X += 2;
            Spikes(X, n); X += n;
            return this;
        }

        /// <summary>Bounce-block ladder over spikes — land and rocket to the next.</summary>
        public LevelComposer NoteBounce(int count)
        {
            int region = count * 4;
            Spikes(X, region);
            for (int k = 0; k < count; k++) { D.Set(X + 1 + k * 4, _floor - 1, 'n'); D.Set(X + 2 + k * 4, _floor - 1, 'n'); }
            X += region;
            return this;
        }

        /// <summary>Platforms that drop the instant you land — cross before they fall.</summary>
        public LevelComposer FallBridge(int count)
        {
            int region = count * 4;
            Spikes(X, region);
            for (int k = 0; k < count; k++) { D.Set(X + 1 + k * 4, _floor - 1, 'l'); D.Set(X + 2 + k * 4, _floor - 1, 'l'); }
            X += region;
            return this;
        }

        /// <summary>Grab a feather, then glide across a huge floaty gap on low gravity.</summary>
        public LevelComposer FeatherFloat(int n)
        {
            Ground(X, 2); D.Set(X + 1, _floor + 1, 'w'); X += 2;
            Spikes(X, n); X += n;
            return this;
        }

        /// <summary>A power-up pickup floating over safe ground: 'w' = glide, 'u' = high-jump.</summary>
        public LevelComposer Power(char kind, int n = 4)
        {
            Ground(X, n);
            D.Set(X + n / 2, _floor + 2, kind);
            X += n;
            return this;
        }

        // ===================== challenge chunks (Level-1 redesign) =====================
        // Built from classic-platformer patterns: enemies own a zone the player must read
        // (never loose mobs on open ground), every gap pairs a safe slow answer with a fast
        // risky one, and timed hazards have fixed phases so a no-stop line always exists.

        /// <summary>Spike gap onto a small patrolled island — read the patrol, then leap into
        /// its window or stomp it on arrival. Chain a few for a rhythm section.</summary>
        public LevelComposer MobIsle(int gap, int isle = 6, char mob = 'g')
        {
            Spikes(X, gap); X += gap;
            Ground(X, isle);
            D.Set(X + isle / 2, _floor, mob);
            X += isle;
            return this;
        }

        /// <summary>A low stone roof over stepping platforms — a full jump bonks the ceiling,
        /// so you must feather short hops across the pit (variable jump height under pressure).</summary>
        public LevelComposer TunnelHops(int count = 3, int pw = 2, int g = 2)
        {
            int region = count * pw + (count - 1) * g;
            Spikes(X, region);
            int x = X;
            for (int k = 0; k < count; k++) { Plat(x, pw, _floor + 1); x += pw + g; }
            for (int i = 1; i < region - 1; i++) D.Set(X + i, _floor + 4, '#');   // cave roof
            X += region;
            return this;
        }

        /// <summary>A spike pit with a saw sweeping vertically through the jump arc — leap when
        /// the blade dips. The phase is fixed per column, so a no-stop line threads it every run.</summary>
        public LevelComposer SawGap(int n = 6)
        {
            Spikes(X, n);
            D.Set(X + n / 2, _floor + 2, 'S');
            X += n;
            return this;
        }

        /// <summary>A flyer bobs across the pit's jump line — slip past in a gap of its drift,
        /// or stomp it mid-air and bounce the rest of the way (the fast line).</summary>
        public LevelComposer FlyerGap(int n = 7)
        {
            Spikes(X, n);
            D.Set(X + n / 2, _floor + 2, 'f');
            X += n;
            return this;
        }

        /// <summary>Two lanes: the low road is safe but slow (two spike hops under open sky);
        /// the high road is a fast platform line — but a patroller owns the middle platform,
        /// so the quick route costs a mid-air read. Falling off only costs time, not life.
        /// The coin floats on the high road's jump arc, right above the patroller.</summary>
        public LevelComposer HighRoad(int len = 18)
        {
            Ground(X, len);
            D.Set(X + 5, _floor, '^');                  // low road: two clean hops,
            D.Set(X + 11, _floor, '^');                 // nothing overhead to bonk
            Plat(X + 1, 3, _floor + 3);                 // high road: mount...
            Plat(X + 7, 3, _floor + 3);                 // ...cross the patrolled middle...
            Plat(X + 13, 3, _floor + 3);                // ...and ride off the far end
            D.Set(X + 8, _floor + 3, 'g');              // it owns the middle platform
            D.Set(X + 8, _floor + 5, 'o'); Coins++;     // on the arc between platforms
            X += len;
            return this;
        }

        /// <summary>Descending steps with a patroller holding a landing — commitment hops where
        /// each drop is a pre-read decision, then the descent feeds you speed.</summary>
        public LevelComposer StairDown(int steps = 3, int run = 4, char mob = 'g')
        {
            for (int k = 0; k < steps; k++)
            {
                Ground(X, run);
                if (k == steps / 2) D.Set(X + run / 2, _floor, mob);
                X += run;
                _floor = System.Math.Max(2, _floor - 2);
            }
            return this;
        }

        /// <summary>The climax — a gap no jump can clear alone: hit the swirl-ring, double-jump,
        /// and thread past a sweeping saw on the way down. The coin hangs above the blade's arc
        /// for the brave; the fast line and the coin line are the same line.</summary>
        public LevelComposer RingSawGap(int n = 9, bool coin = true)
        {
            Spikes(X, n);
            D.Set(X + 2, _floor + 3, 'j');
            // the saw guards the DESCENT, not the apex — the boosted arc clears the sweep at
            // its peak, so the timing read happens on the way down toward the landing
            D.Set(X + n - 2, _floor + 2, 'S');
            if (coin) { D.Set(X + n - 4, _floor + 6, 'o'); Coins++; }
            X += n;
            return this;
        }

        /// <summary>Victory lap: open ground taken at full tilt with single-tile spike hops —
        /// finish the level fast and high instead of trudging to the flag.</summary>
        public LevelComposer Runout(int len = 14)
        {
            Ground(X, len);
            D.Set(X + len / 3, _floor, '^');
            D.Set(X + 2 * len / 3, _floor, '^');
            X += len;
            return this;
        }

        // ================== signature chunks (Levels 2-4 redesign) ==================

        /// <summary>A spark-pad over safe ground — the fling is pure joy here, so its speed is
        /// learned before it's ever asked to carry you over death. The fling carries ~12 cells,
        /// so the chunk must be long enough that the whole flight lands on THIS ground.</summary>
        public LevelComposer BoostRun(int n = 16)
        {
            Ground(X, n);
            D.Set(X + 1, _floor, 'k');
            X += n;
            return this;
        }

        /// <summary>Level 2's climax — THE CLAW: the great wave itself. Two thin crest-lips
        /// reach toward each other across a sea no single fling can cross — the painting's
        /// frozen moment, made playable. Sprint out the near lip, boost off its curling tip,
        /// catch the first spray pearl on the fling line, dash, catch the second hanging
        /// lower on the descent, and dash again to the far crest. The coin glints between
        /// the pearls on the dash line — the brave thread all three in one breath.</summary>
        public LevelComposer Claw(int n = 15, int tip = 3, bool coin = true)
        {
            Ground(X, 3); X += 3;                                  // the last solid deck
            Spikes(X, tip);                                        // the near crest: a thin foam lip
            for (int i = 0; i < tip; i++) D.Set(X + i, _floor - 1, '#');
            D.Set(X + tip - 1, _floor, 'k');                       // wave-pad on the very tip
            X += tip;
            Spikes(X, n);                                          // the open sea under the claw
            D.Set(X + (2 * n) / 5, _floor + 2, 'd');               // first pearl, on the fling line
            if (coin) { D.Set(X + n / 2 + 1, _floor + 2, 'o'); Coins++; }
            D.Set(X + (3 * n) / 4, _floor + 1, 'd');               // second pearl, lower, on the descent
            X += n;
            Spikes(X, tip);                                        // the far crest reaches back
            for (int i = 0; i < tip; i++) D.Set(X + i, _floor - 1, '#');
            X += tip;
            return this;
        }

        /// <summary>Note-blocks on safe ground — bounce for free before bouncing for your life.</summary>
        public LevelComposer NoteRun(int n = 8)
        {
            Ground(X, n);
            D.Set(X + 2, _floor, 'n'); D.Set(X + 3, _floor, 'n');
            D.Set(X + 5, _floor, 'n'); D.Set(X + 6, _floor, 'n');
            X += n;
            return this;
        }

        /// <summary>Pendulum blades skimming the floor — each swings its own fixed arc, so the
        /// gaps between their sweeps are safe pockets and the no-stop line threads the rhythm.</summary>
        public LevelComposer SwingAlley(int count = 2, int spacing = 7)
        {
            int region = count * spacing + spacing;
            Ground(X, region);
            for (int k = 0; k < count; k++) D.Set(X + spacing + k * spacing, _floor, 's');
            X += region;
            return this;
        }

        /// <summary>A spring at the foot of a sheer column — ride the full bounce and drift onto
        /// its top. The Swing's answer to the wall-jump chimney.</summary>
        public LevelComposer SpringTower(int rise = 5)
        {
            Ground(X, 3); D.Set(X + 1, _floor, 'b'); X += 3;
            int newF = _floor + rise;
            for (int y = 0; y < newF; y++) D.Set(X, y, '#');   // the column you bounce up beside
            _floor = newF;
            X += 1;
            Ground(X, 5); X += 5;
            return this;
        }

        /// <summary>A geyser spout — a VERTICAL wave-pad at the foot of a sheer hull — launches
        /// you straight up the cliff onto the upper deck, drifting onto the terrace as you peak.
        /// The Great Wave's answer to the wall-jump: pure water, no climbing. The launch (~8u)
        /// clears the hull face; a fumble just drops you back onto the safe deck below.</summary>
        public LevelComposer Geyser(int rise = 5)
        {
            Ground(X, 3); D.Set(X + 1, _floor, 'K'); X += 3;   // the spout pad on a 3-wide deck
            int newF = _floor + rise;
            for (int y = 0; y < newF; y++) D.Set(X, y, '#');   // the hull face you launch up beside
            _floor = newF;
            X += 1;
            Ground(X, 5); X += 5;
            return this;
        }

        /// <summary>The Swing's signature toy: an empty swing-seat pendulums over a briar gap.
        /// Board it near its close apex, ride the arc to the far apex, and leap to the far ledge.
        /// Boarding is the read — the seat only comes close at one end of its rhythm, the briar
        /// forgives nothing, and you must build run speed on the seat to make the exit leap.</summary>
        public LevelComposer SwingRide(int gap = 9)
        {
            Ground(X, 2); X += 2;                          // the takeoff ledge
            Spikes(X, gap);
            D.Set(X + gap / 2, _floor + 1, 'M');           // the seat, swinging ±2.5 around mid-gap
            X += gap;
            return this;
        }

        /// <summary>A fountain jet — a vertical pendulum platform — beside a sheer marble rise.
        /// Board the crown low over the basin, ride the surge, and step off at the crest onto
        /// the upper terrace. The Swing's elevator: pure rhythm, no ladder.</summary>
        public LevelComposer FountainLift(int rise = 5)
        {
            Ground(X, 2); X += 2;                          // the basin lip
            Spikes(X, 4);                                  // the basin: thorned, not wet
            // the jet crown sweeps floor+0.7 .. floor+5.7: its low point sits just above the lip
            // (a gentle step-on) and its crest rises just past the terrace (walk off as it tops out)
            D.Set(X + 2, _floor + 3, 'V');
            X += 4;
            int newF = _floor + rise;
            for (int y = 0; y < newF; y++) D.Set(X, y, '#');   // the terrace face you crest onto
            _floor = newF;
            X += 1;
            Ground(X, 4); X += 4;
            return this;
        }

        /// <summary>Route choice, speedrunner style: a floor-blade lane with the coin dangling in
        /// the pocket between sweeps — OR a warp arbor floating over the lane's mouth. Jump up
        /// into the arch and you skip the whole lane (the exit arch hangs too high to re-enter
        /// from the ground). The fast line and the 100% line are different lines; choose.</summary>
        public LevelComposer PortalSkip(int len = 16)
        {
            char ch = (char)('p' + (_warpCount++ % 3));
            Ground(X, len + 4);
            D.Set(X + 1, _floor + 3, ch);                  // entry arbor: one clean jump up
            D.Set(X + 5, _floor, 's');                     // the lane: blades skim the floor...
            D.Set(X + 13, _floor, 's');
            D.Set(X + 9, _floor + 1, 'o'); Coins++;        // ...and the coin lives between them
            D.Set(X + len + 2, _floor + 5, ch);            // exit arch: above plain-jump reach
            X += len + 4;
            return this;
        }

        /// <summary>Level 3's climax — THE SWING itself. A grand seat pendulums over a briar
        /// chasm; its close apex swings right over the bower deck, so you board from home ground
        /// and commit at the far apex. The leap threads a pendulum blade, grazes the coin on the
        /// descent, and lands on a note-block relay that bounces you home. The relay is the
        /// reliable line; a direct leap to the far deck is a frame-perfect gamble at the very edge
        /// of the jump (a skip for the bold, never the expected route).</summary>
        public LevelComposer GrandSwing(int n = 16, bool coin = true)
        {
            Ground(X, 3); X += 3;                          // the bower deck
            Spikes(X, n);
            D.Set(X + 5, _floor + 2, 'm');                 // the grand swing: sweeps X+1 .. X+9
            // the blade guards the LEAP ARC, three cells clear of the relay so its low sweep can
            // never clip a player standing on the notes — it's a timed read in open air, not a trap
            D.Set(X + 10, _floor + 5, 'S');                // vertical blade over the open chasm
            if (coin) { D.Set(X + 11, _floor + 3, 'o'); Coins++; }   // just past the blade, on the descent
            D.Set(X + 13, _floor + 1, 'n');                // the note-block relay home
            D.Set(X + 14, _floor + 1, 'n');
            X += n;
            return this;
        }

        /// <summary>A note-block chain over spikes with saws sweeping the junction arcs. The
        /// bounce rhythm is fixed; read the blades and keep the swing alive. The blade rides at
        /// floor+5 so its LOW sweep stays above the note-contact height — it guards the bounce
        /// ARC (a timed read), never clipping a player at the instant they touch a block.</summary>
        public LevelComposer NoteSawChain(int count = 4, bool coin = true)
        {
            int region = count * 4 + 2;
            Spikes(X, region);
            for (int k = 0; k < count; k++) { D.Set(X + 1 + k * 4, _floor - 1, 'n'); D.Set(X + 2 + k * 4, _floor - 1, 'n'); }
            for (int k = 1; k < count; k += 2) D.Set(X + k * 4, _floor + 5, 'S');
            if (coin) { D.Set(X + 2 + (count / 2) * 4, _floor + 2, 'o'); Coins++; }
            X += region;
            return this;
        }

        /// <summary>The screamer taught the Mario way: it dives over SAFE ground first. The perch
        /// hangs above open floor — walk under it, it plunges, and you learn its three truths
        /// (it triggers on approach, it falls slower than you run, its head is a trampoline)
        /// where the only thing a fumble costs is pride.</summary>
        public LevelComposer ScreamerPerch(int n = 9)
        {
            Ground(X, n);
            D.Set(X + n / 2, _floor + 5, 'z');
            X += n;
            return this;
        }

        /// <summary>The world drags you backward: clear the pit onto a reversed belt that hauls
        /// you toward the spikes you just escaped, while a saw sweeps the middle of the lane.
        /// Airborne frames ignore the drag, so the answer is the level's law — never stop,
        /// and cross the belt in chained hops timed to the blade.</summary>
        public LevelComposer DragBelt(int n = 8)
        {
            Spikes(X, 3); X += 3;                              // the pit the belt feeds back into
            Ground(X, n);
            for (int i = 0; i < n; i++) D.Set(X + i, _floor - 1, '<');
            D.Set(X + n / 2, _floor + 2, 'S');                 // blade owns the middle of the lane
            X += n;
            return this;
        }

        /// <summary>Level 4's signature — THE BRIDGE: the painting's walkway as one long dying
        /// sprint. Board-pairs in a plank–plank–gap rhythm over the fjord, each pair dead 0.55s
        /// after your first step, while two screamers plunge through the line in sequence. It
        /// all falls slower than you run; the only safe speed is full speed. The first screamer
        /// can be stomped mid-dive for the fast line — the scream becomes the springboard.</summary>
        public LevelComposer TheBridge(int spans = 5)
        {
            Ground(X, 2); X += 2;                              // the last firm planks
            int region = spans * 3 + 1;                        // board-board-gap rhythm
            Spikes(X, region);
            for (int k = 0; k < spans; k++)
            {
                D.Set(X + k * 3,     _floor - 1, 'c');
                D.Set(X + k * 3 + 1, _floor - 1, 'c');
            }
            D.Set(X + 4,          _floor + 5, 'z');            // first screamer, high over span two
            D.Set(X + region - 5, _floor + 4, 'z');            // second, lower and later — no rhythm reuse
            X += region;
            return this;
        }

        /// <summary>Crumbling boards under a diving screamer — it plunges the moment you pass
        /// beneath its perch, but it falls slower than you run. Stop and you die twice.</summary>
        public LevelComposer DiveBridge(int count = 3)
        {
            int region = count * 4 - 2;
            Spikes(X, region);
            int x = X;
            for (int k = 0; k < count; k++) { D.Set(x, _floor - 1, 'c'); D.Set(x + 1, _floor - 1, 'c'); x += 4; }
            D.Set(X + region / 2, _floor + 5, 'z');
            X += region;
            return this;
        }

        /// <summary>Level 4's climax — grab the feather and drift a gap no jump can clear while
        /// a screamer swoops the glide line. The coin floats late in the drift: hold the nerve.</summary>
        public LevelComposer GlideGap(int n = 11, bool coin = true)
        {
            Ground(X, 2); D.Set(X + 1, _floor + 1, 'w'); X += 2;
            Spikes(X, n);
            D.Set(X + n / 2, _floor + 4, 'z');
            if (coin) { D.Set(X + n - 3, _floor + 1, 'o'); Coins++; }
            X += n;
            return this;
        }

        // ============================ wall jump ============================

        /// <summary>
        /// REQUIRED wall-jump: a chimney you must scale to continue. A right wall blocks forward
        /// progress; you wall-jump between the two walls to climb out the top onto a higher floor.
        /// </summary>
        public LevelComposer WallClimb(int rise = 4, int gap = 3)
        {
            int baseF = _floor;
            int chimTop = baseF + rise + 3;
            int leftX = X;
            int rightX = X + gap + 1;

            Ground(X, gap + 2);                                          // base you walk into
            for (int y = baseF; y < chimTop; y++) D.Set(rightX, y, '#'); // right wall blocks the path
            for (int y = baseF + 3; y < chimTop; y++) D.Set(leftX, y, '#'); // left wall, doorway below

            _floor = baseF + rise;
            X = rightX + 1;
            Ground(X, 6);                                               // landing at the new height
            X += 6;
            return this;
        }

        /// <summary>A low overhang you must DUCK (hold Down) to slide under — safe ground below.
        /// The ceiling 'T' tiles get a collider dropped just low enough that you can't pass standing.</summary>
        public LevelComposer DuckTunnel(int len = 5)
        {
            Ground(X, len);
            for (int i = 0; i < len; i++) D.Set(X + i, _floor + 1, 'T');   // low ceiling
            X += len;
            return this;
        }

        /// <summary>Step the floor back down (you drop onto a lower ledge).</summary>
        public LevelComposer Drop(int amount)
        {
            Ground(X, 2); X += 2;
            _floor = System.Math.Max(2, _floor - amount);
            Ground(X, 5); X += 5;
            return this;
        }

        public LevelComposer End()
        {
            Ground(X, 7);
            D.Goal(X + 3, _floor);
            X += 7;
            D.Bound(X);          // invisible right edge
            X += 1;
            return this;
        }

        public string[] Rows()
        {
            var full = D.Rows();
            var rows = new string[full.Length];
            int width = System.Math.Min(X + 2, full.Length > 0 ? full[0].Length : X + 2);
            for (int i = 0; i < full.Length; i++)
                rows[i] = full[i].Substring(0, System.Math.Min(width, full[i].Length));
            return rows;
        }
    }
}
