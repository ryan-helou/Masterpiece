using System.Collections.Generic;
using UnityEngine;

namespace Velocity
{
    /// <summary>
    /// The GLOBAL player character — loaded once from Resources/Player and used on every level so the
    /// hero is identical across the whole game. Background-keyed like the rest (magenta/white/checker
    /// auto-detected). idle/jump/fall are single frames; the run is individual frames run1.png, run2.png …
    /// (auto-counts however many you make — 8 is smooth, add more for smoother), or a 4×2 movement.png
    /// sheet as a fallback. Missing files just fall back (null).
    /// </summary>
    public static class PlayerArt
    {
        private static bool _loaded;
        public static Sprite Idle, Jump, Fall;
        public static Sprite[] Run;

        public static bool Active => Idle != null;

        public static void EnsureLoaded()
        {
            if (_loaded) return;
            _loaded = true;
            var ctr = new Vector2(0.5f, 0.5f);
            var feet = new Vector2(0.5f, 0f);    // trim + plant by the feet so frames don't jitter

            Idle = ArtKey.LoadTrimmed("Player/idle", ArtKey.Mode.Auto, feet);
            Jump = ArtKey.LoadTrimmed("Player/jump", ArtKey.Mode.Auto, feet);
            Fall = ArtKey.LoadTrimmed("Player/fall", ArtKey.Mode.Auto, feet);

            var frames = new List<Sprite>();
            for (int i = 1; i <= 16; i++)
            {
                var s = ArtKey.LoadTrimmed("Player/run" + i, ArtKey.Mode.Auto, feet);
                if (s == null) break;
                frames.Add(s);
            }
            Run = frames.Count > 0
                ? frames.ToArray()
                : ArtKey.LoadSheet("Player/movement", ArtKey.Mode.Auto, 4, 2, ctr);
        }
    }
}
