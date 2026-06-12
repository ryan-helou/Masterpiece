using UnityEngine;

namespace Velocity
{
    /// <summary>
    /// The whole game's color scheme. Mutable so we can swap accessibility themes at runtime.
    /// Call <see cref="ApplyTheme"/> at boot (and on change) BEFORE things are built/retinted.
    /// Field names are stable so every reference keeps working across themes.
    /// </summary>
    public static class Palette
    {
        public static readonly string[] ThemeNames = { "Default", "Colorblind", "High Contrast" };

        public static Color Hex(string hex)
        {
            ColorUtility.TryParseHtmlString(hex, out var c);
            return c;
        }

        // Background / world
        public static Color Background, BackdropFar, BackdropNear;
        // Geometry
        public static Color Ground, GroundCap, GroundEdge;
        // Entities
        public static Color Player, PlayerHi, Hazard, Goal, GoalGlow, Moving, Coin, Enemy, EnemyHi;
        // UI
        public static Color UiPanel, UiPanelSoft, UiAccent, UiAccentDim,
                            UiText, UiTextDim, UiTextFaint, UiDanger, UiGold;

        static Palette()
        {
            ApplyTheme(0);
        }

        private static Color Lighten(Color c, float t) => Color.Lerp(c, Color.white, t);
        private static Color Darken(Color c, float t)  => Color.Lerp(c, Color.black, t);

        /// <summary>
        /// Retint the whole world to one painting. Called per-level (before the geometry is built)
        /// so the level's sky, ground, player, coins and accent all match the canvas behind them.
        /// Menus keep their neutral colours (they were built once at boot). Accessibility themes
        /// still apply on top: high-contrast brightens the entity triplet so it never washes out.
        /// </summary>
        public static void ApplyPainting(PaintingTheme t)
        {
            if (t == null) return;

            Background   = t.Sky;
            BackdropFar  = t.SkyFar;
            BackdropNear = t.SkyNear;
            Ground       = t.Ground;
            GroundCap    = t.GroundCap;
            GroundEdge   = t.GroundEdge;

            Player    = t.Player;
            PlayerHi  = Lighten(t.Player, 0.55f);
            Goal      = t.Goal;
            GoalGlow  = Lighten(t.Goal, 0.5f);
            Coin      = t.Coin;
            Moving    = t.Accent;
            Hazard    = t.Hazard;
            Enemy     = Darken(t.Hazard, 0.18f);
            EnemyHi   = Lighten(t.Hazard, 0.5f);

            UiAccent    = t.Accent;
            UiAccentDim = Darken(t.Accent, 0.45f);
            UiGold      = t.Coin;

            // accessibility on top of the painting
            if (Settings.ColorTheme == 2) // high contrast: punch up the things that can kill/clear you
            {
                Player = Lighten(Player, 0.25f);
                Hazard = Hex("#ff2d2d");
                Goal   = Lighten(Goal, 0.2f);
            }
            else if (Settings.ColorTheme == 1) // colorblind-friendly: hazard orange, goal cyan
            {
                Hazard = Hex("#ff9f1c");
                Goal   = Hex("#2ec4ff");
                GoalGlow = Hex("#bfeeff");
            }
        }

        public static void ApplyTheme(int theme)
        {
            // shared neutrals
            Background   = Hex("#11131c");
            BackdropFar  = Hex("#171a27");
            BackdropNear = Hex("#1d2233");
            Ground       = Hex("#3a4a63");
            GroundCap    = Hex("#5d7196");
            GroundEdge   = Hex("#2a3447");
            UiPanel      = Hex("#161a26");
            UiPanelSoft  = Hex("#1f2536");
            UiText       = Hex("#e7ecf5");
            UiTextDim    = Hex("#8893a8");
            UiTextFaint  = Hex("#5a647a");
            UiGold       = Hex("#ffd166");
            Coin         = Hex("#ffd166");
            Enemy        = Hex("#c75c3a");
            EnemyHi      = Hex("#ffd9c2");

            switch (theme)
            {
                case 1: // Colorblind-friendly: hazard=orange, goal=cyan (no red/green pairing)
                    Player    = Hex("#ff5c7a");
                    PlayerHi  = Hex("#ffd1da");
                    Hazard    = Hex("#ff9f1c");
                    Goal      = Hex("#2ec4ff");
                    GoalGlow  = Hex("#bfeeff");
                    Moving    = Hex("#ffd166");
                    UiAccent  = Hex("#2ec4ff");
                    UiAccentDim = Hex("#1d6f8f");
                    UiDanger  = Hex("#ff9f1c");
                    break;

                case 2: // High contrast
                    Background   = Hex("#000610");
                    BackdropFar  = Hex("#0a1424");
                    BackdropNear = Hex("#10203a");
                    Ground       = Hex("#3f5a82");
                    GroundCap    = Hex("#8fb3e6");
                    Player    = Hex("#ff4fa3");
                    PlayerHi  = Hex("#ffffff");
                    Hazard    = Hex("#ff2d2d");
                    Goal      = Hex("#39ff14");
                    GoalGlow  = Hex("#c8ffbf");
                    Moving    = Hex("#ffe000");
                    UiText    = Hex("#ffffff");
                    UiAccent  = Hex("#39ff14");
                    UiAccentDim = Hex("#1f8f10");
                    UiDanger  = Hex("#ff2d2d");
                    break;

                default: // Default
                    Player    = Hex("#ff5c7a");
                    PlayerHi  = Hex("#ffd1da");
                    Hazard    = Hex("#ff3b56");
                    Goal      = Hex("#48e0a0");
                    GoalGlow  = Hex("#9bffd9");
                    Moving    = Hex("#caa64b");
                    UiAccent  = Hex("#48e0a0");
                    UiAccentDim = Hex("#2f7f63");
                    UiDanger  = Hex("#ff5c7a");
                    break;
            }
        }
    }
}
