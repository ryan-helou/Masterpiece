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

            void Add(string name, Vector2 pivot, ArtKey.Mode mode = ArtKey.Mode.Auto,
                float featherX = 0f, bool featherBottom = false)
            {
                var s = ArtKey.Load(folder + "/" + name, mode == ArtKey.Mode.Auto ? A : mode, pivot,
                    featherX, featherBottom);
                if (s != null) set.Sprites[name] = s;
            }

            Add("player",     ctr);
            Add("ground",     ctr, ArtKey.Mode.Opaque);
            Add("platform",   ctr);
            Add("spike",      bot);
            Add("pool",       ctr);      // optional continuous death-liquid (replaces spikes if present)
            Add("coin",       ctr);
            Add("spring",     bot);
            Add("ring",       ctr);
            Add("feather",    ctr);
            Add("goal",       bot);
            Add("enemy",      ctr);
            Add("enemy-flyer", ctr);
            Add("enemy-spiky", ctr);
            Add("saw",        ctr);
            Add("overhang",   ctr);
            Add("wall",       ctr, ArtKey.Mode.Opaque);
            Add("ground-edge", ctr);
            Add("fg-mist",    ctr, ArtKey.Mode.Auto, 0.08f);
            Add("boost-pad",  ctr);
            Add("dash-orb",   ctr);
            Add("bg-moon",    ctr);
            Add("bg-wind",    ctr);
            Add("bg-mountain", bot);
            Add("bg-rays",    ctr);
            Add("bg-clouds",  ctr, ArtKey.Mode.Auto, 0.08f);
            Add("bg-drape",   ctr);
            Add("bg-arch",    bot);
            Add("bg-star",    ctr);
            Add("bg-swirl",   ctr);
            // wave backdrop bands: art runs hard off their edges, so feather the borders
            // (fuji also fades its bottom — its haze block would otherwise float as a card)
            Add("bg-fuji",    bot, ArtKey.Mode.Auto, 0.12f, true);
            Add("bg-swell",   ctr, ArtKey.Mode.Auto, 0.10f);
            Add("bg-claw",    bot, ArtKey.Mode.Auto, 0.08f);
            Add("bg-boat",    ctr);
            Add("bg-cypress", bot);
            Add("bg-village", bot);
            Add("bg-hills",   bot);

            set.Walk = ArtKey.LoadSheet(folder + "/movement", A, 2, 3, ctr);
            return set;
        }
    }
}
