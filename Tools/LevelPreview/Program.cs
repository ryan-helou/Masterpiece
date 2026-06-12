using System;

// Minimal stand-ins for the Unity / asset APIs LevelLibrary touches, so the real level
// authoring code compiles in a plain console app.
namespace UnityEngine
{
    public static class Mathf
    {
        public static int Clamp(int v, int min, int max) => v < min ? min : (v > max ? max : v);
        public static float Clamp(float v, float min, float max) => v < min ? min : (v > max ? max : v);
    }
}

namespace Velocity
{
    public static class LevelAssetLoader
    {
        public static World LoadCustomWorld() => null;
    }
}

namespace LevelPreview
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            int index = args.Length > 0 ? int.Parse(args[0]) : 0;
            var def = Velocity.LevelLibrary.Get(index);
            var rows = def.Rows;
            int w = 0;
            foreach (var r in rows) w = Math.Max(w, r.Length);

            Console.WriteLine($"Level {def.Id} — {def.DisplayName}  ({w} x {rows.Length})");
            Console.WriteLine(def.Hint);
            Console.WriteLine();

            // print in screens of 120 columns with a column ruler
            const int screen = 120;
            for (int x0 = 0; x0 < w; x0 += screen)
            {
                int x1 = Math.Min(w, x0 + screen);
                Console.WriteLine($"--- columns {x0}..{x1 - 1} ---");
                var ruler = new char[x1 - x0];
                for (int x = x0; x < x1; x++) ruler[x - x0] = (x % 10 == 0) ? '|' : (x % 5 == 0 ? '+' : '.');
                foreach (var r in rows)
                {
                    string slice = x0 < r.Length ? r.Substring(x0, Math.Min(x1, r.Length) - x0) : "";
                    Console.WriteLine(slice.TrimEnd().Length == 0 ? "" : slice);
                }
                Console.WriteLine(new string(ruler));
                Console.WriteLine();
            }

            // basic sanity checks
            int coins = 0, spawns = 0, goals = 0;
            foreach (var r in rows)
                foreach (var ch in r)
                {
                    if (ch == 'o') coins++;
                    else if (ch == 'P') spawns++;
                    else if (ch == 'G') goals++;
                }
            Console.WriteLine($"spawn={spawns} goal={goals} coins={coins} width={w} height={rows.Length}");
            if (spawns != 1 || goals != 1) Console.WriteLine("!! BAD spawn/goal count");

            Qa(rows, w);
        }

        // ==================== automated beatability QA =====================
        // Static checks built from the ACTUAL movement numbers in PlayerController:
        //   plain jump: clears pits <= 7 cells, rises <= 3
        //   ring 'j':   boosted arc lands 6..10 cells past the ring
        //   pad 'k':    fling lands 11..15 cells past the pad (vy16/vx25, lock 0.3s)
        //   spring 'b': +5.6 height, ~9.4 forward at full hold
        //   dash 'd':   extends a crossing to ~15
        //   feather 'w': glide crossings to ~12
        private static void Qa(string[] rows, int w)
        {
            int h = rows.Length;
            char At(int x, int yUp)
            {
                int r = h - 1 - yUp;
                return (r < 0 || r >= h || x < 0 || x >= rows[r].Length) ? ' ' : rows[r][x];
            }
            bool Solid(char c) => c == '#' || c == 'c' || c == 'l' || c == 'n' || c == '>' || c == '<' || c == '-' || c == 'T';

            // top standable surface per column (y of the cell you stand ON, i.e. tile top)
            var top = new int[w];
            for (int x = 0; x < w; x++)
            {
                top[x] = -1;
                for (int y = h - 1; y >= 0; y--)
                    if (Solid(At(x, y))) { top[x] = y + 1; break; }
            }

            int issues = 0;
            void Bad(string msg) { Console.WriteLine("  !! " + msg); issues++; }
            void Info(string msg) => Console.WriteLine("  -- " + msg);

            bool AidNear(int x0, int x1, char aid)
            {
                for (int x = Math.Max(0, x0); x <= Math.Min(w - 1, x1); x++)
                    for (int y = 0; y < h; y++)
                        if (At(x, y) == aid) return true;
                return false;
            }

            Console.WriteLine("QA:");

            // ---- 1. void gaps vs. movement envelope ----
            for (int x = 1; x < w; x++)
            {
                if (top[x] >= 0 || top[x - 1] < 0) continue;     // start of a void run
                int start = x;
                while (x < w && top[x] < 0) x++;
                int width = x - start;
                if (x >= w) break;                                // runs into the right bound

                string aids = "";
                if (AidNear(start, x - 1, 'j')) aids += "ring ";
                if (AidNear(start - 3, start, 'k')) aids += "pad ";
                if (AidNear(start - 3, start, 'b')) aids += "spring ";
                if (AidNear(start, x - 1, 'd')) aids += "orb ";
                if (AidNear(start - 3, start, 'w')) aids += "feather ";

                int limit = aids.Contains("pad") || aids.Contains("orb") ? 15
                          : aids.Contains("feather") ? 12
                          : aids.Contains("ring") ? 10
                          : aids.Contains("spring") ? 10 : 7;
                if (width > limit)
                    Bad($"gap cols {start}-{x - 1} width {width} exceeds limit {limit} (aids: {(aids == "" ? "none" : aids.Trim())})");
            }

            // ---- 2. boost pads: the fling must land on something standable ----
            for (int px = 0; px < w; px++)
                for (int py = 0; py < h; py++)
                {
                    if (At(px, py) != 'k') continue;
                    bool landing = false;
                    for (int x = px + 10; x <= Math.Min(w - 1, px + 15); x++)
                        if (top[x] >= 0 && top[x] <= py + 1) { landing = true; break; }
                    // an orb mid-flight extends the reach (the Reach climax)
                    if (!landing && AidNear(px + 4, px + 10, 'd'))
                        for (int x = px + 10; x <= Math.Min(w - 1, px + 18); x++)
                            if (top[x] >= 0 && top[x] <= py + 1) { landing = true; break; }
                    if (!landing) Bad($"boost pad at col {px}: no standable landing 10-15 cells out at/below pad height");
                }

            // ---- 3. rings: the boosted arc must have a landing 6..10 out ----
            for (int rx = 0; rx < w; rx++)
                for (int ry = 0; ry < h; ry++)
                {
                    if (At(rx, ry) != 'j') continue;
                    bool landing = false;
                    for (int x = rx + 6; x <= Math.Min(w - 1, rx + 10); x++)
                        if (top[x] >= 0 && top[x] <= ry + 1) { landing = true; break; }
                    if (!landing) Bad($"jump ring at col {rx}: no standable landing 6-10 cells out");
                }

            // ---- 4. ground mobs stand on something; flyers/divers float free ----
            for (int x = 0; x < w; x++)
                for (int y = 0; y < h; y++)
                {
                    char c = At(x, y);
                    if ((c == 'g' || c == 'y') && !Solid(At(x, y - 1)))
                        Bad($"walker '{c}' at col {x} y {y} has no ground beneath it");
                    if ((c == 'f' || c == 'z') && Solid(At(x, y)))
                        Bad($"flyer '{c}' at col {x} y {y} embedded in solid");
                }

            // ---- 5. coins reachable ----
            for (int cx = 0; cx < w; cx++)
                for (int cy = 0; cy < h; cy++)
                {
                    if (At(cx, cy) != 'o') continue;
                    bool ok = false;
                    for (int x = Math.Max(0, cx - 2); x <= Math.Min(w - 1, cx + 2) && !ok; x++)
                        if (top[x] >= 0 && cy - top[x] <= 3) ok = true;                  // plain jump
                    for (int x = Math.Max(0, cx - 4); x <= Math.Min(w - 1, cx + 4) && !ok; x++)
                        if (top[x] >= 0 && Math.Abs(cy - top[x]) <= 1) ok = true;        // low coin on a pit's crossing line
                    if (!ok)
                        for (int x = Math.Max(0, cx - 2); x <= Math.Min(w - 1, cx + 2) && !ok; x++)
                            for (int y = 0; y < cy; y++)
                                if (At(x, y) == 'b' && cy - y <= 6) ok = true;           // spring
                    if (!ok && AidNear(cx - 6, cx + 2, 'j')) ok = true;                  // ring arc
                    if (!ok && AidNear(cx - 8, cx, 'k')) ok = true;                      // pad flight
                    if (!ok && AidNear(cx - 4, cx + 2, 'n')) ok = true;                  // note bounce
                    if (!ok && AidNear(cx - 4, cx + 2, 'd')) ok = true;                  // dash line
                    if (!ok) Bad($"coin at col {cx} y {cy} looks unreachable");
                }

            // ---- 6. duck tunnels leave a crawl space ----
            for (int x = 0; x < w; x++)
                for (int y = 0; y < h; y++)
                    if (At(x, y) == 'T' && (Solid(At(x, y - 1)) || !Solid(At(x, y - 2))))
                        Bad($"duck ceiling at col {x} y {y} has no 1-cell crawl space beneath");

            // ---- 7. saw sweep summary (manual review aid) ----
            for (int x = 0; x < w; x++)
                for (int y = 0; y < h; y++)
                    if (At(x, y) == 'S')
                        Info($"saw col {x}: sweeps y {y - 2}..{y + 3} (ground below at y {(top[x] >= 0 ? top[x].ToString() : "void")})");

            Console.WriteLine(issues == 0 ? "QA: PASS — no blocking issues found" : $"QA: {issues} issue(s) flagged");
        }
    }
}
