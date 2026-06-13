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
            // prefer the dedicated cliff art (wall_new) over the legacy generic tile, under "wall"
            var wallArt = ArtKey.Load(folder + "/wall_new", ArtKey.Mode.Opaque, ctr)
                        ?? ArtKey.Load(folder + "/wall", ArtKey.Mode.Opaque, ctr);
            if (wallArt != null) set.Sprites["wall"] = wallArt;
            Add("deep-earth",  ctr, ArtKey.Mode.Opaque);   // optional under-platform body (else tiles the cliff art)
            Add("ground-edge", ctr);
            Add("fg-mist",    ctr, ArtKey.Mode.Auto, 0.08f);
            Add("boost-pad",  ctr);
            Add("dash-orb",   ctr);
            Add("crumble",    ctr);      // dying boards ('c') — board-pair, drawn slightly tall
            Add("falling",    ctr);      // donut boards ('l') — drop after a shake
            Add("conveyor",   ctr);      // belt tile, art faces RIGHT (mirrored for '<')
            Add("portal",     ctr);      // warp gate — L3's arbor arch / the Scream's reality tear
            Add("enemy-diver", ctr);
            Add("boss",       ctr);      // Level 4 final boss — the giant Scream-knot (falls back to enemy-diver)
            // Level 3 (The Swing) gameplay pieces
            Add("note-block", ctr);
            Add("swing-seat", bot);                       // bench at the point, ropes rise above
            Add("jet",        new Vector2(0.5f, 1f));     // froth crown at the point, column hangs below
            Add("bg-moon",    ctr);
            Add("bg-wind",    ctr, ArtKey.Mode.Auto, 0.1f);   // feathered ends — no hard repeat seam
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
            Add("bg-claw",    bot, ArtKey.Mode.Auto, 0.16f);
            Add("bg-boat",    ctr);
            Add("bg-cypress", bot);
            Add("bg-village", bot);
            Add("bg-hills",   bot);
            // garden backdrop bands (The Swing): the canopy hangs from the frame top, hedges
            // rise from the floor line; both repeat horizontally, so feather their flanks
            Add("bg-canopy",  new Vector2(0.5f, 1f), ArtKey.Mode.Auto, 0.10f);
            Add("bg-swing",   new Vector2(0.5f, 1f));     // the empty swing — sways from its rope anchor
            Add("bg-hedge",   bot, ArtKey.Mode.Auto, 0.10f);
            Add("bg-fountain", bot);
            Add("bg-urn",     bot);
            Add("bg-bloom",   ctr);
            // scream backdrop (The Scream): sky/water bands run off their edges, the headland
            // fades its base into the fjord haze the way fuji does
            Add("bg-skyband",  ctr, ArtKey.Mode.Auto, 0.10f);
            Add("bg-fjord",    ctr, ArtKey.Mode.Auto, 0.10f);
            Add("bg-headland", bot, ArtKey.Mode.Auto, 0.12f, true);
            Add("bg-railing",  bot, ArtKey.Mode.Auto, 0.06f);

            set.Walk = ArtKey.LoadSheet(folder + "/movement", A, 2, 3, ctr);
            return set;
        }
    }
}
