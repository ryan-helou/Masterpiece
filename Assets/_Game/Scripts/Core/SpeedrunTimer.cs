using System;

namespace Velocity
{
    /// <summary>
    /// High-precision run timer. Armed at level load, begins on first input, accumulates while
    /// running, freezes on finish. Plain C# object ticked by the GameManager.
    /// </summary>
    public class SpeedrunTimer
    {
        public double Elapsed { get; private set; }
        public bool Running { get; private set; }
        public bool Finished { get; private set; }

        public void Reset()
        {
            Elapsed = 0;
            Running = false;
            Finished = false;
        }

        public void Begin()
        {
            if (Finished) return;
            Running = true;
        }

        public void Tick(float dt)
        {
            if (Running && !Finished) Elapsed += dt;
        }

        public void Stop()
        {
            Running = false;
            Finished = true;
        }

        /// <summary>Formats seconds as M:SS.mmm (e.g. 12.345s -> "0:12.345").</summary>
        public static string Format(double seconds)
        {
            if (seconds < 0) seconds = 0;
            int totalMs = (int)Math.Round(seconds * 1000.0);
            int ms = totalMs % 1000;
            int totalSec = totalMs / 1000;
            int s = totalSec % 60;
            int m = totalSec / 60;
            return $"{m}:{s:00}.{ms:000}";
        }
    }
}
