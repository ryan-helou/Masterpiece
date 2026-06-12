using UnityEngine;

namespace Velocity
{
    /// <summary>
    /// Persistent player preferences (audio, display, accessibility, gameplay). Backed by
    /// PlayerPrefs. Control rebindings live in <see cref="GameInput"/>; everything else is here.
    /// </summary>
    public static class Settings
    {
        private const string K_Master   = "vel.master";
        private const string K_Sfx      = "vel.sfx";
        private const string K_Music     = "vel.music";
        private const string K_Ghost    = "vel.ghost";
        private const string K_Timer    = "vel.timerVisible";
        private const string K_Fps      = "vel.showFps";
        private const string K_ResetTimer = "vel.resetTimerOnDeath";
        private const string K_Shake    = "vel.screenShake";
        private const string K_Fullscreen = "vel.fullscreen";
        private const string K_VSync    = "vel.vsync";
        private const string K_FpsCap   = "vel.fpsCap";
        private const string K_ReduceMotion = "vel.reduceMotion";
        private const string K_Theme    = "vel.theme";
        private const string K_Countdown = "vel.countdown";
        private const string K_Lives    = "vel.limitedLives";

        // audio
        public static float MasterVolume = 0.9f;
        public static float SfxVolume    = 0.8f;
        public static float MusicVolume  = 0.5f;

        // hud / game
        public static bool  ShowTimer    = true;
        public static bool  ShowFps      = false;
        public static bool  ShowGhost    = true;
        public static bool  ResetTimerOnDeath = true;
        public static bool  CountdownStart = false;
        public static bool  LimitedLives = false;      // speedrun default: one life, instant retry
        public const  int   StartingLives = 1;

        // display
        public static bool  Fullscreen   = true;
        public static bool  VSync        = true;
        public static int   FpsCap       = 0;       // 0 = uncapped
        public static readonly int[] FpsCapOptions = { 0, 60, 120, 144, 240 };

        // accessibility
        public static bool  ScreenShake  = true;
        public static bool  ReduceMotion = false;
        public static int   ColorTheme   = 0;       // index into Palette.ThemeNames

        public static void Load()
        {
            MasterVolume = PlayerPrefs.GetFloat(K_Master, 0.9f);
            SfxVolume    = PlayerPrefs.GetFloat(K_Sfx, 0.8f);
            MusicVolume  = PlayerPrefs.GetFloat(K_Music, 0.5f);
            ShowTimer    = PlayerPrefs.GetInt(K_Timer, 1) == 1;
            ShowFps      = PlayerPrefs.GetInt(K_Fps, 0) == 1;
            ShowGhost    = PlayerPrefs.GetInt(K_Ghost, 1) == 1;
            ResetTimerOnDeath = PlayerPrefs.GetInt(K_ResetTimer, 1) == 1;
            CountdownStart = PlayerPrefs.GetInt(K_Countdown, 0) == 1;
            LimitedLives = PlayerPrefs.GetInt(K_Lives, 0) == 1;
            Fullscreen   = PlayerPrefs.GetInt(K_Fullscreen, 1) == 1;
            VSync        = PlayerPrefs.GetInt(K_VSync, 1) == 1;
            FpsCap       = PlayerPrefs.GetInt(K_FpsCap, 0);
            ScreenShake  = PlayerPrefs.GetInt(K_Shake, 1) == 1;
            ReduceMotion = PlayerPrefs.GetInt(K_ReduceMotion, 0) == 1;
            ColorTheme   = PlayerPrefs.GetInt(K_Theme, 0);
        }

        public static void Save()
        {
            PlayerPrefs.SetFloat(K_Master, MasterVolume);
            PlayerPrefs.SetFloat(K_Sfx, SfxVolume);
            PlayerPrefs.SetFloat(K_Music, MusicVolume);
            PlayerPrefs.SetInt(K_Timer, ShowTimer ? 1 : 0);
            PlayerPrefs.SetInt(K_Fps, ShowFps ? 1 : 0);
            PlayerPrefs.SetInt(K_Ghost, ShowGhost ? 1 : 0);
            PlayerPrefs.SetInt(K_ResetTimer, ResetTimerOnDeath ? 1 : 0);
            PlayerPrefs.SetInt(K_Countdown, CountdownStart ? 1 : 0);
            PlayerPrefs.SetInt(K_Lives, LimitedLives ? 1 : 0);
            PlayerPrefs.SetInt(K_Fullscreen, Fullscreen ? 1 : 0);
            PlayerPrefs.SetInt(K_VSync, VSync ? 1 : 0);
            PlayerPrefs.SetInt(K_FpsCap, FpsCap);
            PlayerPrefs.SetInt(K_Shake, ScreenShake ? 1 : 0);
            PlayerPrefs.SetInt(K_ReduceMotion, ReduceMotion ? 1 : 0);
            PlayerPrefs.SetInt(K_Theme, ColorTheme);
            PlayerPrefs.Save();
        }

        public static void CycleFpsCap()
        {
            int i = System.Array.IndexOf(FpsCapOptions, FpsCap);
            i = (i + 1) % FpsCapOptions.Length;
            FpsCap = FpsCapOptions[i];
        }

        public static string FpsCapLabel => FpsCap == 0 ? "Uncapped" : FpsCap + " fps";
    }
}
