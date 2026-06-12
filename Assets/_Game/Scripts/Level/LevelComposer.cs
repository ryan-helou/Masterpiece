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
            D.Wall(0);
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
            D.Wall(X);
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
