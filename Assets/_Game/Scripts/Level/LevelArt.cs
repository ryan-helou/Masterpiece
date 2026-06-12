using System.Collections.Generic;
using UnityEngine;

namespace Velocity
{
    /// <summary>
    /// The set of imported, background-keyed sprites for the level currently being built. Loaded
    /// per art folder and CACHED (keying is heavy — we never want to redo it on a death-reload).
    /// Builders call <see cref="Get"/>; a null result means "no art, use the procedural fallback".
    /// </summary>
    public static class LevelArt
    {
        private class Set
        {
            public readonly Dictionary<string, Sprite> Sprites = new Dictionary<string, Sprite>();
            public Sprite[] Walk;
        }

        private static readonly Dictionary<string, Set> _cache = new Dictionary<string, Set>();
        private static Set _active;

        public static bool Active => _active != null;

        public static void Use(string folder)
        {
            if (string.IsNullOrEmpty(folder)) { _active = null; return; }
            if (_cache.TryGetValue(folder, out var set)) { _active = set; return; }
            set = Build(folder);
            _cache[folder] = set;
            _active = set;
        }

        public static Sprite Get(string name) =>
            (_active != null && _active.Sprites.TryGetValue(name, out var s)) ? s : null;

        public static Sprite[] Walk => _active?.Walk;

        private static Set Build(string folder)
        {
            var set = new Set();
            var A = ArtKey.Mode.Auto;    // detects magenta / white / checker per file
            var ctr = new Vector2(0.5f, 0.5f); var bot = new Vector2(0.5f, 0f);

            void Add(string name, Vector2 pivot)
            {
                var s = ArtKey.Load(folder + "/" + name, A, pivot);
                if (s != null) set.Sprites[name] = s;
            }

            Add("player",     ctr);
            Add("ground",     ctr);
            Add("platform",   ctr);
            Add("spike",      bot);
            Add("pool",       ctr);      // optional continuous death-liquid (replaces spikes if present)
            Add("coin",       ctr);
            Add("spring",     bot);
            Add("ring",       ctr);
            Add("feather",    ctr);
            Add("goal",       bot);
            Add("enemy",      ctr);
            Add("bg-moon",    ctr);
            Add("bg-star",    ctr);
            Add("bg-swirl",   ctr);
            Add("bg-cypress", bot);
            Add("bg-village", bot);
            Add("bg-hills",   bot);

            set.Walk = ArtKey.LoadSheet(folder + "/movement", A, 2, 3, ctr);
            return set;
        }
    }
}
