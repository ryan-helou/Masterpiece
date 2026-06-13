using UnityEngine;

namespace Velocity
{
    /// <summary>Which hand-built scenery a level paints behind itself — the *vibe* of the painting,
    /// not a photo of it.</summary>
    public enum BackdropStyle
    {
        StarryNight,   // swirling night sky, crescent moon, cypress hills
        Fresco,        // soft warm fresco glow + drifting clouds
        Garden,        // rococo green canopy + golden light shafts
        Scream,        // burning wavy sky over a dark fjord + bridge rails
        Alpine,        // stormy sky, snowy peaks, wind streaks
        Fog,           // layered mountains receding into mist
        Chiaroscuro,   // deep shadow with one warm spotlight pool
        Wave,          // stylised foam-claw waves + Mt Fuji
    }

    /// <summary>
    /// One level = one famous painting. Each theme carries a palette sampled from that painting and
    /// a <see cref="BackdropStyle"/> so the whole level — sky, ground, player, coins, UI accent and
    /// the hand-built scenery behind it — evokes the canvas. (The real image is only used small, as
    /// the framed artwork in the museum overworld.)
    ///
    /// The table order IS the level order. Swap a row to re-skin a level.
    /// </summary>
    public class PaintingTheme
    {
        public string Name;          // level / painting title
        public string Painter;       // "Artist · Year"
        public string Hint;          // intro-card flavour + mechanic tip
        public string ResourceKey;   // Resources.Load path, e.g. "Paintings/starry-night"

        // palette sampled from the painting
        public Color Sky, SkyFar, SkyNear;     // camera bg + two parallax layers
        public Color Ground, GroundCap, GroundEdge;
        public Color Player, Goal, Coin, Accent, Hazard;
        public float Scrim;          // how much to dim the framed artwork in the museum (0..1)

        // scenery
        public BackdropStyle Style;
        public Color Glow;           // stars / light / foam
        public Color Motif;          // silhouette / shape colour
        public string ArtFolder;     // Resources subfolder of imported art (null = procedural)

        private static Color H(string hex)
        {
            ColorUtility.TryParseHtmlString(hex, out var c);
            return c;
        }

        /// <summary>Assign the backdrop scenery style (chained after Make in the table).</summary>
        public PaintingTheme WithStyle(BackdropStyle style, string glow = null, string motif = null)
        {
            Style = style;
            Glow  = glow  != null ? H(glow)  : Coin;
            Motif = motif != null ? H(motif) : new Color(Ground.r * 0.5f, Ground.g * 0.5f, Ground.b * 0.5f, 1f);
            return this;
        }

        /// <summary>Point this level at a folder of imported art under Resources (chained in the table).</summary>
        public PaintingTheme WithArt(string folder) { ArtFolder = folder; return this; }

        // convenience builder so the table below stays readable
        public static PaintingTheme Make(
            string name, string painter, string key, string hint, float scrim,
            string sky, string skyFar, string skyNear,
            string ground, string groundCap, string groundEdge,
            string player, string goal, string coin, string accent, string hazard)
        {
            return new PaintingTheme
            {
                Name = name, Painter = painter, ResourceKey = key, Hint = hint, Scrim = scrim,
                Sky = H(sky), SkyFar = H(skyFar), SkyNear = H(skyNear),
                Ground = H(ground), GroundCap = H(groundCap), GroundEdge = H(groundEdge),
                Player = H(player), Goal = H(goal), Coin = H(coin), Accent = H(accent), Hazard = H(hazard),
            };
        }
    }

    public static class PaintingThemes
    {
        // index == level index (0-based). Exactly the order the player meets them.
        public static readonly PaintingTheme[] All =
        {
            // 1 — STARRY NIGHT : dreamy low-gravity night, swirl-rings + glide
            PaintingTheme.Make("Starry Night", "Vincent van Gogh · 1889",
                "Paintings/starry-night",
                "Drift through the night — tap jump inside a swirl-ring for a second boost, then glide the long gaps.",
                0.42f,
                "#0a1530", "#14264f", "#20386b",
                "#1d2b1a", "#3a5a2e", "#0a100d",
                "#ffe9b0", "#f4d35e", "#ffd23a", "#6fb7ff", "#ff4060")
                .WithStyle(BackdropStyle.StarryNight, "#ffe27a", "#0c1a10").WithArt("Level1"),

            // 2 — THE GREAT WAVE OFF KANAGAWA : "the surge" — wave-pad flings + spray-pearl dashes.
            // Warm timber = safe footing; Prussian blue + foam = the sea, which is death.
            // Color budget: mostly beige/cream/white world — blue stays a sea-and-danger accent.
            PaintingTheme.Make("The Great Wave off Kanagawa", "Katsushika Hokusai · 1831",
                "Paintings/great-wave",
                "Ride the surge — wave-pads fling you, spray pearls refill the dash. The sea is not a place to stand.",
                0.40f,
                "#cfc3a4", "#ddd2b4", "#e8dcc0",
                "#7a5a34", "#c89b62", "#4a3520",
                "#f4eedd", "#d24a37", "#e8c66a", "#7fa8c4", "#ff3b30")
                .WithStyle(BackdropStyle.Wave, "#f4eedd", "#5a7a96").WithArt("Level2"),

            // 3 — THE SWING : momentum / pendulum — ridden swing-seats, note-blocks, springs,
            // a fountain-jet lift and a warp arbor. Warm stone & wood = safe; rose briar = death.
            PaintingTheme.Make("The Swing", "Jean-Honoré Fragonard · 1767",
                "Paintings/the-swing",
                "Ride the pendulum — board the swing at one apex, leap from the other, and the garden's rhythm is yours.",
                0.40f,
                "#16240f", "#2a4019", "#426033",
                "#3a2b1c", "#5e4830", "#221a10",
                "#f4a6c0", "#ffd86a", "#ffe08a", "#f4a6c0", "#ff3b56")
                .WithStyle(BackdropStyle.Garden, "#ffe69a", "#102a12").WithArt("Level3"),

            // 4 — THE SCREAM : urgency — the ground dies under you; the sky burns over the fjord.
            // Readability law: warm bridge timber = safe footing; the deep blue-black fjord (and
            // anything vermilion-tipped) = death. The fire in the sky is atmosphere, never hazard.
            PaintingTheme.Make("The Scream", "Edvard Munch · 1893",
                "Paintings/the-scream",
                "The bridge is falling. Don't stop — the boards die under your feet, and the sky is the only way out.",
                0.34f,
                "#7a3b1a", "#a04a1c", "#c25a20",
                "#6e4527", "#b8804a", "#33200f",
                "#f2e23a", "#ff7a1a", "#ffd23a", "#ff7a33", "#ff2d2d")
                .WithStyle(BackdropStyle.Scream, "#ffd24a", "#0e1830").WithArt("Level4"),
        };

        public static int Count => All.Length;

        public static PaintingTheme For(int levelIndex)
        {
            if (All.Length == 0) return null;
            return All[Mathf.Clamp(levelIndex, 0, All.Length - 1)];
        }
    }
}
