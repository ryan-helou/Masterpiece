using UnityEngine;

namespace Velocity
{
    /// <summary>Applies window / frame-pacing preferences. Call after Settings.Load and on change.</summary>
    public static class DisplaySettings
    {
        public static void Apply()
        {
            Screen.fullScreenMode = Settings.Fullscreen
                ? FullScreenMode.FullScreenWindow
                : FullScreenMode.Windowed;

            QualitySettings.vSyncCount = Settings.VSync ? 1 : 0;

            // when vsync is on, targetFrameRate is ignored; otherwise honor the cap (0 = uncapped)
            Application.targetFrameRate = Settings.VSync
                ? -1
                : (Settings.FpsCap == 0 ? -1 : Settings.FpsCap);
        }
    }
}
