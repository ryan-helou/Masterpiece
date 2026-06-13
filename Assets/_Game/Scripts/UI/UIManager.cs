using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Velocity
{
    /// <summary>
    /// Builds and drives every screen in the game from code: main menu, level select, in-game
    /// HUD (the speedrun timer), pause, results, settings (with live control rebinding) and the
    /// leaderboard. GameManager owns navigation; this class only shows/refreshes panels.
    /// </summary>
    public class UIManager : MonoBehaviour
    {
        private GameManager _gm;
        private GameInput _input;
        private Canvas _canvas;

        // panels
        private GameObject _mainMenu, _levelSelect, _settings, _leaderboard, _pause, _results, _hud;

        // hud refs
        private MonoText _timerMono;
        private Text _hudLevel, _hudBest, _fpsText, _splitText, _hudCoins, _countdownText, _comboText, _deathsText, _livesText;
        private float _fpsSmooth = 60f;
        private int _hudCoinPrev;
        private float _coinPulse;
        private Image[] _coinStarFills, _coinStarHalos;
        private Transform _coinPulseTarget;
        private RectTransform _splitPill;
        private CanvasGroup _splitGroup;
        private float _splitTarget, _splitPulse;
        private int _splitSign;   // -1 ahead, +1 behind, 0 unknown (for the cross-over pulse)
        private double _paceGold, _paceSilver;   // live medal targets; 0 = no pace coloring (full-run)
        private GameObject _powerPanel;
        private Image _powerIcon, _powerBar;
        private Text _powerLabel;
        private Image _dashBar;

        // game over
        private GameObject _gameOver;
        private Text _goSub;
        private GameObject _selGameOver;

        // results refs
        private Text _resTitle, _resTime, _resDelta, _resMedal, _resRank, _resCoins, _resExtra;
        private Button _resNext, _resRetry;

        // full-run results
        private GameObject _fullResults;
        private Text _frTitle, _frTotal, _frDelta;
        private GameObject _frSplitsHolder, _selFull;

        // world tab selection (level select + leaderboard)
        private int _selectedWorld;
        private Text _lsTitle, _lbTitle;

        // settings rebind refs
        private readonly List<(GameInput.RebindEntry entry, Text value)> _rebindRows = new();
        private Text _rebindingHint;

        // overlays
        private Image _flash;
        private float _flashFade = 4f;
        private GameObject _intro;
        private CanvasGroup _introGroup;
        private Text _introTitle, _introHint, _introMedals;
        private float _introHold;
        private bool _introFading;

        // default-selected buttons (for gamepad/keyboard navigation)
        private GameObject _selMain, _selPause, _selSettings, _selLeaderboard, _selLevels;

        // overworld (museum gallery) plaque overlay
        private GameObject _owPanel;
        private Text _owNum, _owName, _owArtist, _owMeta, _owPrompt;

        public void Init(GameManager gm, GameInput input)
        {
            _gm = gm;
            _input = input;
            BuildAll();
        }

        /// <summary>Tear down and rebuild the whole canvas (used when the color theme changes).</summary>
        public void RebuildAll()
        {
            if (_canvas != null) Destroy(_canvas.gameObject);
            _cardHolder = null;
            _lbHolder = null;
            _frSplitsHolder = null;
            BuildAll();
        }

        private void BuildAll()
        {
            _canvas = UIFactory.Canvas("UICanvas", 100);
            _canvas.transform.SetParent(transform, false);

            BuildHud();
            BuildIntro();
            BuildMainMenu();
            BuildLevelSelect();
            BuildSettings();
            BuildLeaderboard();
            BuildPause();
            BuildResults();
            BuildFullResults();
            BuildGameOver();
            BuildOverworldUI();
            BuildFlash();   // built last => draws on top of everything

            HideAll();
        }

        private void Update()
        {
            // star pop on pickup
            if (_coinPulse > 0f && _coinPulseTarget != null)
            {
                _coinPulse = Mathf.MoveTowards(_coinPulse, 0f, Time.unscaledDeltaTime * 4f);
                _coinPulseTarget.localScale = Vector3.one * (1f + 0.5f * _coinPulse);
            }

            // delta pill: smooth fade in/out + a small pulse when crossing ahead/behind the ghost
            if (_splitGroup != null)
            {
                _splitGroup.alpha = Mathf.MoveTowards(_splitGroup.alpha, _splitTarget, Time.unscaledDeltaTime * 5f);
                if (_splitPulse > 0f)
                {
                    _splitPulse = Mathf.MoveTowards(_splitPulse, 0f, Time.unscaledDeltaTime * 5f);
                    _splitPill.localScale = Vector3.one * (1f + 0.12f * _splitPulse);
                }
            }

            // screen flash fade-out
            if (_flash != null && _flash.color.a > 0f)
            {
                var c = _flash.color;
                c.a = Mathf.MoveTowards(c.a, 0f, _flashFade * Time.unscaledDeltaTime);
                _flash.color = c;
            }

            // level-intro hold then fade
            if (_intro != null && _intro.activeSelf)
            {
                if (!_introFading)
                {
                    _introHold -= Time.unscaledDeltaTime;
                    if (_introHold <= 0f) _introFading = true;
                }
                else
                {
                    _introGroup.alpha = Mathf.MoveTowards(_introGroup.alpha, 0f, 2.5f * Time.unscaledDeltaTime);
                    if (_introGroup.alpha <= 0.001f) _intro.SetActive(false);
                }
            }
        }

        // ================================================================ HUD
        private void BuildHud()
        {
            _hud = UIFactory.Panel(_canvas.transform, new Color(0, 0, 0, 0), "HUD").gameObject;
            _hud.GetComponent<Image>().raycastTarget = false;
            var t = _hud.transform;

            // soft top scrim (dark fading to clear) so the HUD reads over any painting behind it
            var scrim = UIFactory.Rect(t, "TopScrim");
            scrim.anchorMin = new Vector2(0f, 1f);
            scrim.anchorMax = new Vector2(1f, 1f);
            scrim.pivot = new Vector2(0.5f, 1f);
            scrim.anchoredPosition = Vector2.zero;
            scrim.sizeDelta = new Vector2(0, 215);
            var scrimImg = scrim.gameObject.AddComponent<Image>();
            scrimImg.sprite = TextureFactory.VerticalFade(2, 64,
                new Color(0.02f, 0.03f, 0.06f, 0f), new Color(0.02f, 0.03f, 0.06f, 0.55f));
            scrimImg.raycastTarget = false;

            // fixed-width timer so the digits never jiggle; milliseconds smaller + dimmer
            var timerRt = UIFactory.Rect(t, "Timer");
            UIFactory.Place(timerRt, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0, -22), new Vector2(450, 92));
            _timerMono = timerRt.gameObject.AddComponent<MonoText>();
            _timerMono.Build(9, 50f, 92f, 84, Palette.UiText, tailCells: 4, tailScale: 0.62f, tailAlpha: 0.8f);
            _timerMono.SetText("0:00.000");

            // live +/- vs PB ghost: a translucent pill under the timer that fades in/out
            _splitPill = UIFactory.Rect(t, "SplitPill");
            UIFactory.Place(_splitPill, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0, -124), new Vector2(180, 46));
            _splitGroup = _splitPill.gameObject.AddComponent<CanvasGroup>();
            _splitGroup.alpha = 0f;
            _splitGroup.interactable = false;
            _splitGroup.blocksRaycasts = false;
            var pillImg = _splitPill.gameObject.AddComponent<Image>();
            pillImg.sprite = TextureFactory.RoundedBoxSliced(22);
            pillImg.type = Image.Type.Sliced;
            pillImg.color = new Color(0.03f, 0.04f, 0.08f, 0.62f);
            pillImg.raycastTarget = false;
            _splitText = UIFactory.Label(_splitPill, "", 30, Palette.UiAccent, TextAnchor.MiddleCenter, FontStyle.Bold);
            UIFactory.Stretch((RectTransform)_splitText.transform);

            // top-left "museum plaque": gold number + title over a thin gold rule, then PB
            _hudLevel = UIFactory.LabelAt(t, "", 30, Palette.UiText,
                new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(28, -24), new Vector2(560, 40),
                TextAnchor.UpperLeft, FontStyle.Bold);

            var rule = UIFactory.Box(t, new Vector2(30, -64), new Vector2(64, 3),
                new Color(Palette.UiGold.r, Palette.UiGold.g, Palette.UiGold.b, 0.85f),
                new Vector2(0f, 1f), new Vector2(0f, 1f), "TitleRule");
            rule.raycastTarget = false;

            _hudBest = UIFactory.LabelAt(t, "", 24, Palette.UiTextDim,
                new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(28, -76), new Vector2(560, 34),
                TextAnchor.UpperLeft);

            _fpsText = UIFactory.LabelAt(t, "", 22, Palette.UiTextDim,
                new Vector2(1f, 1f), new Vector2(1f, 1f), new Vector2(-28, -96), new Vector2(220, 34),
                TextAnchor.UpperRight);

            _livesText = UIFactory.LabelAt(t, "", 30, Palette.UiDanger,
                new Vector2(1f, 1f), new Vector2(1f, 1f), new Vector2(-28, -22), new Vector2(260, 38),
                TextAnchor.UpperRight, FontStyle.Bold);

            _hudCoins = UIFactory.LabelAt(t, "", 26, Palette.UiGold,
                new Vector2(1f, 1f), new Vector2(1f, 1f), new Vector2(-28, -60), new Vector2(220, 36),
                TextAnchor.UpperRight, FontStyle.Bold);

            // 3 collectible stars (top-left): hollow outlines that fill gold (with a soft halo) on pickup
            _coinStarFills = new Image[3];
            _coinStarHalos = new Image[3];
            var starSprite = TextureFactory.Star(40, 5);
            var starOutline = TextureFactory.StarOutline(40, 5);
            var haloSprite = TextureFactory.RadialGlow(64);
            for (int i = 0; i < 3; i++)
            {
                var slot = UIFactory.Rect(t, "CoinStar" + i);
                UIFactory.Place(slot, new Vector2(0f, 1f), new Vector2(0f, 1f),
                    new Vector2(30 + i * 48, -118), new Vector2(38, 38));
                var bg = slot.gameObject.AddComponent<Image>();
                bg.sprite = starOutline; bg.color = new Color(0.85f, 0.88f, 0.95f, 0.42f); bg.raycastTarget = false;

                var haloRt = UIFactory.Rect(slot, "Halo");
                UIFactory.Place(haloRt, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(76, 76));
                var halo = haloRt.gameObject.AddComponent<Image>();
                halo.sprite = haloSprite;
                halo.color = new Color(Palette.UiGold.r, Palette.UiGold.g, Palette.UiGold.b, 0.4f);
                halo.raycastTarget = false;
                halo.enabled = false;
                _coinStarHalos[i] = halo;

                var fillRt = UIFactory.Rect(slot, "Fill");
                UIFactory.Stretch(fillRt);
                var fill = fillRt.gameObject.AddComponent<Image>();
                fill.sprite = starSprite; fill.color = Palette.UiGold; fill.raycastTarget = false;
                fill.enabled = false;
                _coinStarFills[i] = fill;
            }

            // power-up indicator (top-left, below the stars): icon + label + draining timer bar
            _powerPanel = UIFactory.Rect(t, "PowerPanel").gameObject;
            UIFactory.Place((RectTransform)_powerPanel.transform, new Vector2(0f, 1f), new Vector2(0f, 1f),
                new Vector2(34, -210), new Vector2(240, 44));
            var pIconRt = UIFactory.Rect(_powerPanel.transform, "PIcon");
            UIFactory.Place(pIconRt, new Vector2(0f, 0.5f), new Vector2(0f, 0.5f), new Vector2(14, 0), new Vector2(26, 26));
            _powerIcon = pIconRt.gameObject.AddComponent<Image>();
            _powerIcon.sprite = TextureFactory.Star(28, 5); _powerIcon.raycastTarget = false;
            _powerLabel = UIFactory.LabelAt(_powerPanel.transform, "", 19, Palette.UiText,
                new Vector2(0f, 0.5f), new Vector2(0f, 0.5f), new Vector2(38, 8), new Vector2(190, 26),
                TextAnchor.MiddleLeft, FontStyle.Bold);
            var pBarBg = UIFactory.Box(_powerPanel.transform, new Vector2(38, -13), new Vector2(150, 6),
                Palette.UiPanel, new Vector2(0f, 0.5f), new Vector2(0f, 0.5f), "PBarBg");
            var pBarRt = UIFactory.Rect(pBarBg.transform, "PBarFill");
            pBarRt.anchorMin = new Vector2(0f, 0f); pBarRt.anchorMax = new Vector2(0f, 1f);
            pBarRt.pivot = new Vector2(0f, 0.5f); pBarRt.anchoredPosition = Vector2.zero; pBarRt.sizeDelta = new Vector2(150, 0);
            _powerBar = pBarRt.gameObject.AddComponent<Image>();
            _powerBar.raycastTarget = false;
            _powerPanel.SetActive(false);

            // dash cooldown bar (bottom-center) — fills up as the dash recharges, bright when ready
            var dashPanel = UIFactory.Rect(t, "DashPanel");
            UIFactory.Place(dashPanel, new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(0, 62), new Vector2(260, 26));
            UIFactory.LabelAt(dashPanel, "DASH", 18, Palette.UiTextDim,
                new Vector2(0f, 0.5f), new Vector2(0f, 0.5f), new Vector2(6, 0), new Vector2(74, 24),
                TextAnchor.MiddleLeft, FontStyle.Bold);
            var dBg = UIFactory.Box(dashPanel, new Vector2(80, 0), new Vector2(170, 12), Palette.UiPanel,
                new Vector2(0f, 0.5f), new Vector2(0f, 0.5f), "DBarBg");
            var dFillRt = UIFactory.Rect(dBg.transform, "DBarFill");
            dFillRt.anchorMin = new Vector2(0f, 0f); dFillRt.anchorMax = new Vector2(0f, 1f);
            dFillRt.pivot = new Vector2(0f, 0.5f); dFillRt.anchoredPosition = Vector2.zero; dFillRt.sizeDelta = new Vector2(170, 0);
            _dashBar = dFillRt.gameObject.AddComponent<Image>();
            _dashBar.raycastTarget = false;

            _deathsText = UIFactory.LabelAt(t, "", 22, Palette.UiTextDim,
                new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(28, -166), new Vector2(360, 32),
                TextAnchor.UpperLeft);

            _comboText = UIFactory.LabelAt(t, "", 56, Palette.UiGold,
                new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0, 150), new Vector2(500, 70),
                TextAnchor.MiddleCenter, FontStyle.Bold);
            _comboText.enabled = false;

            _countdownText = UIFactory.LabelAt(t, "", 160, Palette.UiText,
                new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0, 40), new Vector2(600, 200),
                TextAnchor.MiddleCenter, FontStyle.Bold);
            _countdownText.enabled = false;

            UIFactory.LabelAt(t,
                "Move • Jump • Dash • Wall-jump (jump while sliding a wall) • R Restart • Esc Pause", 22, Palette.UiTextFaint,
                new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(0, 20), new Vector2(900, 30),
                TextAnchor.LowerCenter);
        }

        public void ShowHud(bool show) => _hud.SetActive(show);

        public void SetHudCoins(int banked, int pending, int total)
        {
            if (_hudCoins != null) _hudCoins.enabled = false;   // replaced by the star indicator

            if (_coinStarFills != null)
            {
                for (int i = 0; i < _coinStarFills.Length; i++)
                {
                    var slot = _coinStarFills[i].transform.parent.gameObject;
                    bool used = i < total;
                    if (slot.activeSelf != used) slot.SetActive(used);
                    bool isBanked = i < banked;
                    bool isPending = i >= banked && i < banked + pending;   // grabbed, not yet safe
                    _coinStarFills[i].enabled = isBanked || isPending;
                    var col = Palette.UiGold; col.a = isPending ? 0.4f : 1f; // pending reads dim
                    _coinStarFills[i].color = col;
                    _coinStarHalos[i].enabled = isBanked;                    // halo only once banked
                }
            }

            int shown = banked + pending;
            if (shown > _hudCoinPrev && _coinStarFills != null && shown - 1 < _coinStarFills.Length)
            {
                _coinPulse = 1f;
                _coinPulseTarget = _coinStarFills[shown - 1].transform.parent;
            }
            _hudCoinPrev = shown;
        }

        /// <summary>Show/refresh the active power-up + its remaining-time bar (0..1).</summary>
        public void SetHudPower(PowerUp p, float fraction)
        {
            if (_powerPanel == null) return;
            bool on = p != PowerUp.None;
            if (_powerPanel.activeSelf != on) _powerPanel.SetActive(on);
            if (!on) return;

            Color col = p == PowerUp.Glide ? new Color(0.45f, 0.85f, 1f) : new Color(0.45f, 1f, 0.5f);
            _powerIcon.color = col;
            _powerBar.color = col;
            _powerLabel.text = p == PowerUp.Glide ? "GLIDE" : "HIGH JUMP";
            var rt = (RectTransform)_powerBar.transform;
            rt.sizeDelta = new Vector2(150f * Mathf.Clamp01(fraction), rt.sizeDelta.y);
        }

        /// <summary>Dash cooldown bar: charge 0..1 (1 = recharged), bright when ready to fire.</summary>
        public void SetHudDash(float charge, bool ready)
        {
            if (_dashBar == null) return;
            var rt = (RectTransform)_dashBar.transform;
            rt.sizeDelta = new Vector2(170f * Mathf.Clamp01(charge), rt.sizeDelta.y);
            _dashBar.color = ready
                ? Palette.Player
                : new Color(Palette.UiAccent.r, Palette.UiAccent.g, Palette.UiAccent.b, 0.65f);
        }

        public void SetCountdown(float seconds)
        {
            if (_countdownText == null) return;
            if (seconds < 0f) { _countdownText.enabled = false; return; }
            _countdownText.enabled = true;
            _countdownText.text = seconds > 0.25f ? Mathf.CeilToInt(seconds).ToString() : "GO!";
            _countdownText.color = seconds > 0.25f ? Palette.UiText : Palette.UiAccent;
        }

        /// <summary>Live medal targets for the running timer's pace tint. Pass 0,0 to disable
        /// (e.g. a full-gallery run, where the on-screen clock is cumulative, not per-level).</summary>
        public void SetPace(double gold, double silver)
        {
            _paceGold = gold;
            _paceSilver = silver;
        }

        public void SetHudLevel(string name, double? best)
        {
            _hudLevel.text = name;
            string faint = ColorUtility.ToHtmlStringRGB(Palette.UiTextFaint);
            _hudBest.text = $"<color=#{faint}>PB</color>  " + (best.HasValue ? SpeedrunTimer.Format(best.Value) : "—");
            _splitText.text = "";
            _splitTarget = 0f;
            _splitSign = 0;
            if (_splitGroup != null) _splitGroup.alpha = 0f;
        }

        public void TickHud(double elapsed, bool running, int combo = 0, float delta = 0f, bool hasDelta = false)
        {
            if (_timerMono == null) return;
            _timerMono.SetVisible(Settings.ShowTimer);
            _timerMono.SetText(SpeedrunTimer.Format(elapsed));
            // medal pace: the running clock glows GOLD while you're still under the gold target,
            // sits WHITE while only silver is in reach, and fades GREY once both have slipped —
            // so a racer reads their medal standing without doing arithmetic. (Targets are 0 in a
            // full-gallery run, where the displayed clock is cumulative and per-level pace is moot.)
            Color tc = running ? Palette.UiText : Palette.UiTextDim;
            if (running && _paceGold > 0.0)
            {
                if (elapsed <= _paceGold) tc = Palette.UiGold;
                else if (elapsed <= _paceSilver) tc = Palette.UiText;
                else tc = Palette.UiTextDim;
            }
            _timerMono.SetColor(tc);

            // combo meter
            if (_comboText != null)
            {
                _comboText.enabled = combo >= 2;
                if (combo >= 2)
                {
                    _comboText.text = $"x{combo}  COMBO";
                    _comboText.color = combo >= 5 ? Palette.UiDanger : Palette.UiGold;
                    _comboText.fontSize = Mathf.Min(56 + combo * 4, 96);
                }
            }

            // live +/- vs your PB ghost (pill under the timer; fades in Update)
            if (_splitText != null)
            {
                bool show = hasDelta && running && Settings.ShowTimer;
                _splitTarget = show ? 1f : 0f;
                if (show)
                {
                    bool ahead = delta < 0f;
                    float mag = Mathf.Abs(delta);
                    // compact: "+8.796" under a minute, full "1:08.796" beyond
                    _splitText.text = (ahead ? "-" : "+") + (mag < 60f ? mag.ToString("0.000") : SpeedrunTimer.Format(mag));
                    _splitText.color = ahead ? Palette.UiAccent : Palette.UiDanger;
                    int sign = ahead ? -1 : 1;
                    if (_splitSign != 0 && _splitSign != sign) _splitPulse = 1f;   // crossed the ghost
                    _splitSign = sign;
                    _splitPill.sizeDelta = new Vector2(Mathf.Max(150f, _splitText.preferredWidth + 52f), 46f);
                }
                else _splitSign = 0;
            }

            _fpsSmooth = Mathf.Lerp(_fpsSmooth, 1f / Mathf.Max(0.0001f, Time.unscaledDeltaTime), 0.1f);
            _fpsText.enabled = Settings.ShowFps;
            _fpsText.text = Mathf.RoundToInt(_fpsSmooth) + " fps";
        }

        public void SetHudDeaths(int n)
        {
            if (_deathsText == null) return;
            _deathsText.enabled = n > 0;
            _deathsText.text = n > 0 ? "Deaths  " + n : "";
        }

        public void SetHudLives(int lives, bool limited)
        {
            if (_livesText == null) return;
            _livesText.enabled = limited;
            if (limited)
            {
                lives = Mathf.Clamp(lives, 0, 9);
                _livesText.text = new string('♥', lives) + (lives == 0 ? "—" : "");
            }
        }

        // ============================================================== overlays
        private void BuildFlash()
        {
            _flash = UIFactory.Panel(_canvas.transform, new Color(1f, 1f, 1f, 0f), "Flash");
            _flash.raycastTarget = false;
        }

        public void Flash(Color color, float intensity)
        {
            if (_flash == null) return;
            if (Settings.ReduceMotion) intensity *= 0.35f;
            color.a = intensity;
            _flash.color = color;
        }

        private void BuildIntro()
        {
            _intro = UIFactory.Panel(_canvas.transform, new Color(0, 0, 0, 0f), "Intro").gameObject;
            _intro.GetComponent<Image>().raycastTarget = false;
            _introGroup = _intro.AddComponent<CanvasGroup>();
            _introGroup.interactable = false;
            _introGroup.blocksRaycasts = false;
            var t = _intro.transform;

            _introTitle = UIFactory.LabelAt(t, "", 90, Palette.UiText,
                new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0, 90), new Vector2(1400, 110),
                TextAnchor.MiddleCenter, FontStyle.Bold);
            _introHint = UIFactory.LabelAt(t, "", 30, Palette.UiAccent,
                new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0, 10), new Vector2(1200, 60),
                TextAnchor.MiddleCenter, FontStyle.Italic);
            _introMedals = UIFactory.LabelAt(t, "", 26, Palette.UiTextDim,
                new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0, -56), new Vector2(1200, 40),
                TextAnchor.MiddleCenter, FontStyle.Bold);
            UIFactory.LabelAt(t, "move to start the clock", 22, Palette.UiTextFaint,
                new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0, -110), new Vector2(1000, 34),
                TextAnchor.MiddleCenter);
        }

        public void ShowLevelIntro(LevelDefinition lvl)
        {
            _introTitle.text = $"{lvl.Id}   {lvl.DisplayName}";
            _introHint.text = lvl.Hint;
            _introMedals.text = $"GOLD {SpeedrunTimer.Format(lvl.GoldTime)}    SILVER {SpeedrunTimer.Format(lvl.SilverTime)}";
            _introGroup.alpha = 1f;
            _introHold = 2.4f;
            _introFading = false;
            _intro.SetActive(true);
        }

        public void DismissIntro()
        {
            if (_intro != null && _intro.activeSelf) _introFading = true;
        }

        public void FlashSplit(string text, Color color)
        {
            _splitText.text = text;
            _splitText.color = color;
        }

        // ========================================================== MAIN MENU
        private void BuildMainMenu()
        {
            _mainMenu = MenuRoot("MainMenu");
            var t = _mainMenu.transform;

            // painted wordmark if one exists (Resources/UI/logo, magenta-keyed like level art),
            // otherwise the plain text title
            var logo = ArtKey.Load("UI/logo", ArtKey.Mode.Auto, new Vector2(0.5f, 0.5f));
            if (logo != null)
            {
                var lrt = UIFactory.Rect(t, "Logo");
                UIFactory.Place(lrt, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f),
                    new Vector2(0, -40), new Vector2(1100, 250));
                var img = lrt.gameObject.AddComponent<Image>();
                img.sprite = logo;
                img.preserveAspect = true;
            }
            else
                UIFactory.LabelAt(t, "MASTERPIECE", 130, Palette.UiText,
                    new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0, -150), new Vector2(1200, 150),
                    TextAnchor.UpperCenter, FontStyle.Bold);
            UIFactory.LabelAt(t, "a speedrun platformer", 34, Palette.UiAccent,
                new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0, -290), new Vector2(1200, 50),
                TextAnchor.UpperCenter, FontStyle.Italic);

            float y = -40;
            _selMain = UIFactory.Button(t, "PLAY", new Vector2(0, y), new Vector2(420, 78),
                () => _gm.RequestPlay(), Palette.UiAccentDim, Palette.UiText, 38).gameObject;
            UIFactory.Button(t, "LEADERBOARDS", new Vector2(0, y - 98), new Vector2(420, 70),
                () => _gm.RequestLeaderboard());
            UIFactory.Button(t, "SETTINGS", new Vector2(0, y - 184), new Vector2(420, 70),
                () => _gm.RequestSettings());
            UIFactory.Button(t, "QUIT", new Vector2(0, y - 270), new Vector2(420, 70),
                () => _gm.RequestQuit());

            UIFactory.LabelAt(t, "v0.1 • built with Claude Code", 20, Palette.UiTextFaint,
                new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(0, 24), new Vector2(600, 30),
                TextAnchor.LowerCenter);
        }

        public void ShowMainMenu() { HideAll(); _mainMenu.SetActive(true); SetSelected(_selMain); }

        private void SetSelected(GameObject go)
        {
            if (EventSystem.current == null || go == null) return;
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(go);
        }

        // ======================================================== LEVEL SELECT
        private void BuildLevelSelect()
        {
            _levelSelect = MenuRoot("LevelSelect");
            var t = _levelSelect.transform;

            _lsTitle = UIFactory.LabelAt(t, "", 50, Palette.UiText,
                new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0, -44), new Vector2(1100, 62),
                TextAnchor.UpperCenter, FontStyle.Bold);

            // world prev / next tabs (top-anchored, flanking the card list)
            UIFactory.Button(t, "◀", new Vector2(-640, -330), new Vector2(66, 66),
                () => ChangeWorld(-1), Palette.UiPanelSoft, Palette.UiText, 32, new Vector2(0.5f, 1f));
            UIFactory.Button(t, "▶", new Vector2(640, -330), new Vector2(66, 66),
                () => ChangeWorld(+1), Palette.UiPanelSoft, Palette.UiText, 32, new Vector2(0.5f, 1f));

            // FULL RUN button is created in RefreshLevelCards (below the dynamic card list)
            BuildBackButton(t, () => _gm.RequestMainMenu());
        }

        private void ChangeWorld(int dir)
        {
            int n = LevelLibrary.WorldCount;
            _selectedWorld = ((_selectedWorld + dir) % n + n) % n;
            if (Sfx.Instance != null) Sfx.Instance.PlayClick();
            RefreshLevelCards();
            SetSelected(_selLevels);
        }

        public void ShowLevelSelect()
        {
            HideAll();
            _selectedWorld = Mathf.Clamp(_selectedWorld, 0, LevelLibrary.WorldCount - 1);
            _levelSelect.SetActive(true);
            RefreshLevelCards();
            SetSelected(_selLevels);
        }

        private GameObject _cardHolder;
        private void RefreshLevelCards()
        {
            var t = _levelSelect.transform;
            if (_cardHolder != null) Destroy(_cardHolder);
            _cardHolder = UIFactory.Rect(t, "Cards").gameObject;
            var holder = (RectTransform)_cardHolder.transform;
            UIFactory.Stretch(holder);

            var world = LevelLibrary.WorldAt(_selectedWorld);
            _lsTitle.text = world.Name;

            int n = world.Levels.Count;
            float cardW = 760, cardH = 82, gap = 12;
            float startY = -130;

            for (int i = 0; i < n; i++)
            {
                var lvl = world.Levels[i];
                bool hasBest = Leaderboard.TryGetBest(lvl.Id, out double best);
                int globalIdx = LevelLibrary.GlobalIndex(_selectedWorld, i);

                var card = UIFactory.Box(holder, new Vector2(0, startY - i * (cardH + gap)),
                    new Vector2(cardW, cardH), Palette.UiPanelSoft,
                    new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), "Card");
                var btn = card.gameObject.AddComponent<Button>();
                btn.targetGraphic = card;
                var cb = btn.colors;
                cb.highlightedColor = UIFactory.Lighten(Palette.UiPanelSoft, 0.1f);
                cb.pressedColor = UIFactory.Darken(Palette.UiPanelSoft, 0.1f);
                btn.colors = cb;
                btn.onClick.AddListener(() => { if (Sfx.Instance != null) Sfx.Instance.PlayClick(); _gm.RequestStartLevel(globalIdx); });
                if (i == 0) _selLevels = card.gameObject;

                UIFactory.LabelAt(card.transform, lvl.Id, 30, Palette.UiAccent,
                    new Vector2(0, 0.5f), new Vector2(0, 0.5f), new Vector2(26, 0), new Vector2(110, cardH),
                    TextAnchor.MiddleLeft, FontStyle.Bold);
                UIFactory.LabelAt(card.transform, lvl.DisplayName, 32, Palette.UiText,
                    new Vector2(0, 0.5f), new Vector2(0, 0.5f), new Vector2(140, 8), new Vector2(420, 44),
                    TextAnchor.MiddleLeft, FontStyle.Bold);
                UIFactory.LabelAt(card.transform, lvl.Hint, 18, Palette.UiTextDim,
                    new Vector2(0, 0.5f), new Vector2(0, 0.5f), new Vector2(140, -26), new Vector2(440, 30),
                    TextAnchor.MiddleLeft);

                string medal = hasBest ? MedalName(lvl, best) : "";
                Color mcol = MedalColor(lvl, hasBest, best);
                int coinBest = Coins.Best(lvl.Id);
                UIFactory.LabelAt(card.transform, hasBest ? SpeedrunTimer.Format(best) : "— : —",
                    34, mcol,
                    new Vector2(1, 0.5f), new Vector2(1, 0.5f), new Vector2(-26, 8), new Vector2(220, 44),
                    TextAnchor.MiddleRight, FontStyle.Bold);
                UIFactory.LabelAt(card.transform, hasBest ? medal + (coinBest > 0 ? "  ◎" + coinBest : "") : "not cleared",
                    18, Palette.UiTextDim,
                    new Vector2(1, 0.5f), new Vector2(1, 0.5f), new Vector2(-26, -26), new Vector2(240, 28),
                    TextAnchor.MiddleRight);
            }

            // world total + FULL RUN button, stacked below the card list
            float belowY = startY - n * (cardH + gap) - 8;
            var ids = new List<string>();
            foreach (var l in world.Levels) ids.Add(l.Id);
            var (total, completed, count) = Leaderboard.WorldSummary(ids);
            string totalStr = completed == count
                ? "World total  " + SpeedrunTimer.Format(total)
                : $"Cleared {completed}/{count}";
            UIFactory.LabelAt(holder, totalStr, 24, Palette.UiGold,
                new Vector2(0.5f, 1f), new Vector2(0.5f, 1f),
                new Vector2(0, belowY), new Vector2(800, 34),
                TextAnchor.UpperCenter, FontStyle.Bold);

            UIFactory.Button(holder, "FULL RUN  ▶▶", new Vector2(0, belowY - 68), new Vector2(340, 56),
                () => _gm.RequestFullRun(_selectedWorld), Palette.UiAccentDim, Palette.UiText, 28,
                new Vector2(0.5f, 1f));
        }

        // ====================================================== OVERWORLD (museum)
        private void BuildOverworldUI()
        {
            _owPanel = UIFactory.Panel(_canvas.transform, new Color(0, 0, 0, 0f), "Overworld").gameObject;
            _owPanel.GetComponent<Image>().raycastTarget = false;
            var t = _owPanel.transform;

            UIFactory.LabelAt(t, "THE GALLERY", 48, Palette.UiText,
                new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0, -42), new Vector2(1200, 60),
                TextAnchor.UpperCenter, FontStyle.Bold);
            UIFactory.LabelAt(t, "◀  ▶   walk the hall        ▲ JUMP   step into the painting        Esc   menu",
                22, Palette.UiTextDim,
                new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0, -100), new Vector2(1400, 34),
                TextAnchor.UpperCenter);

            // the plaque that updates as you approach each piece
            var plaque = UIFactory.Box(t, new Vector2(0, 70), new Vector2(980, 196), Palette.UiPanelSoft,
                new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), "Plaque");
            // thin gold rule across the top of the plaque
            UIFactory.Box(plaque.transform, new Vector2(0, 0), new Vector2(980, 4), Palette.UiGold,
                new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), "Rule");
            var p = plaque.transform;

            _owNum = UIFactory.LabelAt(p, "", 20, Palette.UiAccent,
                new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0, -16), new Vector2(920, 28),
                TextAnchor.UpperCenter, FontStyle.Bold);
            _owName = UIFactory.LabelAt(p, "", 38, Palette.UiText,
                new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0, -46), new Vector2(920, 52),
                TextAnchor.UpperCenter, FontStyle.Bold);
            _owArtist = UIFactory.LabelAt(p, "", 22, Palette.UiTextDim,
                new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0, -94), new Vector2(920, 30),
                TextAnchor.UpperCenter, FontStyle.Italic);
            _owMeta = UIFactory.LabelAt(p, "", 24, Palette.UiGold,
                new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0, -128), new Vector2(920, 34),
                TextAnchor.UpperCenter, FontStyle.Bold);
            _owPrompt = UIFactory.LabelAt(p, "", 23, Palette.UiAccent,
                new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0, -164), new Vector2(920, 30),
                TextAnchor.UpperCenter, FontStyle.Bold);
        }

        public void ShowOverworld(bool show)
        {
            if (_owPanel == null) return;
            _owPanel.SetActive(show);
            if (show && EventSystem.current != null) EventSystem.current.SetSelectedGameObject(null);
        }

        /// <summary>Refresh the plaque for the frame the avatar is standing in front of (or clear it).</summary>
        public void SetOverworldInfo(int num, string name, string artist,
            string meta, string coins, bool canEnter, bool isFullRun)
        {
            if (_owName == null) return;

            if (string.IsNullOrEmpty(name))
            {
                _owNum.text = "";
                _owName.text = "Walk the gallery";
                _owArtist.text = "";
                _owMeta.text = "";
                _owPrompt.text = "◀  ▶   stroll to a masterpiece";
                _owPrompt.color = Palette.UiTextDim;
                return;
            }

            if (isFullRun)
            {
                _owNum.text = "★ FINALE CHALLENGE";
                _owName.text = name;
                _owArtist.text = artist;
                _owMeta.text = meta;
                _owPrompt.text = $"▲ JUMP   race all {LevelLibrary.WorldAt(0).Levels.Count}, one timer";
            }
            else
            {
                _owNum.text = $"MASTERPIECE  {num} / {PaintingThemes.Count}";
                _owName.text = name;
                _owArtist.text = artist;
                _owMeta.text = string.IsNullOrEmpty(coins) ? meta : meta + "        " + coins;
                _owPrompt.text = "▲ JUMP   step into the painting";
            }
            _owPrompt.color = Palette.UiAccent;
        }

        // ============================================================ SETTINGS
        private const float C1 = -560f, C2 = 40f, C3 = 620f;

        private void BuildSettings()
        {
            _settings = MenuRoot("Settings");
            var t = _settings.transform;

            UIFactory.LabelAt(t, "SETTINGS", 54, Palette.UiText,
                new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0, -30), new Vector2(800, 66),
                TextAnchor.UpperCenter, FontStyle.Bold);

            const float rowH = 54;

            // ---------- Column 1: CONTROLS ----------
            float y1 = -130;
            SectionHeaderAt(t, "CONTROLS", C1 - 150, y1); y1 -= 44;
            _rebindRows.Clear();
            foreach (var entry in _input.Rebindable) { RebindRow(t, entry, C1, y1); y1 -= rowH; }
            UIFactory.Button(t, "RESET CONTROLS", new Vector2(C1, y1 - 6), new Vector2(300, 50),
                () => { _input.ResetToDefaults(); RefreshBindings(); },
                Palette.UiPanelSoft, Palette.UiText, 22, new Vector2(0.5f, 1f));

            // ---------- Column 2: AUDIO + GAME ----------
            float y2 = -130;
            SectionHeaderAt(t, "AUDIO", C2 - 180, y2); y2 -= 44;
            SliderRow(t, "Master", Settings.MasterVolume, C2, y2, v => { Settings.MasterVolume = v; Settings.Save(); }); y2 -= rowH;
            SliderRow(t, "SFX", Settings.SfxVolume, C2, y2, v =>
            {
                Settings.SfxVolume = v; Settings.Save();
                if (Sfx.Instance != null) Sfx.Instance.PlayClick();
            }); y2 -= rowH;
            SliderRow(t, "Music", Settings.MusicVolume, C2, y2, v => { Settings.MusicVolume = v; Settings.Save(); }); y2 -= rowH + 8;

            SectionHeaderAt(t, "GAME", C2 - 180, y2); y2 -= 44;
            ToggleRow(t, "Show Timer", Settings.ShowTimer, C2, y2, v => { Settings.ShowTimer = v; Settings.Save(); }); y2 -= 46;
            ToggleRow(t, "Show FPS", Settings.ShowFps, C2, y2, v => { Settings.ShowFps = v; Settings.Save(); }); y2 -= 46;
            ToggleRow(t, "Ghost Racer", Settings.ShowGhost, C2, y2, v => { Settings.ShowGhost = v; Settings.Save(); }); y2 -= 46;
            ToggleRow(t, "Reset Timer on Death", Settings.ResetTimerOnDeath, C2, y2, v => { Settings.ResetTimerOnDeath = v; Settings.Save(); }); y2 -= 46;
            ToggleRow(t, "Countdown (3-2-1)", Settings.CountdownStart, C2, y2, v => { Settings.CountdownStart = v; Settings.Save(); }); y2 -= 46;

            // ---------- Column 3: DISPLAY + ACCESSIBILITY ----------
            float y3 = -130;
            SectionHeaderAt(t, "DISPLAY", C3 - 180, y3); y3 -= 44;
            ToggleRow(t, "Fullscreen", Settings.Fullscreen, C3, y3, v => { Settings.Fullscreen = v; Settings.Save(); DisplaySettings.Apply(); }); y3 -= 46;
            ToggleRow(t, "VSync", Settings.VSync, C3, y3, v => { Settings.VSync = v; Settings.Save(); DisplaySettings.Apply(); }); y3 -= 46;
            CycleRow(t, "Frame Cap", Settings.FpsCapLabel, C3, y3, () =>
            {
                Settings.CycleFpsCap(); Settings.Save(); DisplaySettings.Apply();
                return Settings.FpsCapLabel;
            }); y3 -= 54;

            SectionHeaderAt(t, "ACCESSIBILITY", C3 - 180, y3); y3 -= 44;
            ToggleRow(t, "Screen Shake", Settings.ScreenShake, C3, y3, v => { Settings.ScreenShake = v; Settings.Save(); }); y3 -= 46;
            ToggleRow(t, "Reduce Motion", Settings.ReduceMotion, C3, y3, v => { Settings.ReduceMotion = v; Settings.Save(); }); y3 -= 46;
            // theme rebuilds the whole UI, so it's a plain button (nothing touches it afterward)
            UIFactory.LabelAt(t, "Color Theme", 24, Palette.UiText,
                new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(C3 - 150, y3), new Vector2(170, 44),
                TextAnchor.MiddleLeft);
            UIFactory.Button(t, Palette.ThemeNames[Mathf.Clamp(Settings.ColorTheme, 0, Palette.ThemeNames.Length - 1)],
                new Vector2(C3 + 100, y3), new Vector2(230, 42),
                () =>
                {
                    Settings.ColorTheme = (Settings.ColorTheme + 1) % Palette.ThemeNames.Length;
                    Settings.Save();
                    _gm.RequestApplyTheme();
                },
                Palette.UiPanel, Palette.UiAccent, 22, new Vector2(0.5f, 1f));
            y3 -= 56;

            UIFactory.Button(t, "RESET RECORDS", new Vector2(C3, y3 - 6), new Vector2(320, 50),
                () =>
                {
                    foreach (var id in LevelLibrary.AllIds()) { Leaderboard.Clear(id); GhostStore.Clear(id); Coins.Clear(id); }
                    for (int w = 0; w < LevelLibrary.WorldCount; w++)
                        PlayerPrefs.DeleteKey("vel.fullrun." + LevelLibrary.WorldAt(w).Id);
                    PlayerPrefs.Save();
                },
                Palette.UiPanelSoft, Palette.UiDanger, 22, new Vector2(0.5f, 1f));

            _rebindingHint = UIFactory.LabelAt(t, "", 26, Palette.UiGold,
                new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(0, 86), new Vector2(1400, 36),
                TextAnchor.LowerCenter, FontStyle.Bold);

            _selSettings = BuildBackButton(t, () => _gm.RequestCloseSettings());
        }

        public void ShowSettings()
        {
            HideAll();
            _settings.SetActive(true);
            RefreshBindings();
            _rebindingHint.text = "";
            SetSelected(_selSettings);
        }

        private void RebindRow(Transform parent, GameInput.RebindEntry entry, float x, float y)
        {
            UIFactory.LabelAt(parent, entry.Label, 24, Palette.UiText,
                new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(x - 115, y), new Vector2(210, 46),
                TextAnchor.MiddleLeft);

            var btn = UIFactory.Button(parent, entry.DisplayString(),
                new Vector2(x + 120, y), new Vector2(240, 44),
                null, Palette.UiPanel, Palette.UiAccent, 22, new Vector2(0.5f, 1f));
            var valueText = btn.GetComponentInChildren<Text>();
            _rebindRows.Add((entry, valueText));

            btn.onClick.AddListener(() =>
            {
                if (Sfx.Instance != null) Sfx.Instance.PlayClick();
                valueText.text = "press…";
                _rebindingHint.text = $"Press a key for \"{entry.Label}\"  (Esc cancels)";
                _input.StartRebind(entry, () =>
                {
                    RefreshBindings();
                    _rebindingHint.text = "";
                });
            });
        }

        private void RefreshBindings()
        {
            foreach (var (entry, value) in _rebindRows)
                if (value != null) value.text = entry.DisplayString();
        }

        private void SliderRow(Transform parent, string label, float value, float x, float y, System.Action<float> onChange)
        {
            UIFactory.LabelAt(parent, label, 24, Palette.UiText,
                new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(x - 150, y), new Vector2(150, 46),
                TextAnchor.MiddleLeft);
            UIFactory.Slider(parent, value, new Vector2(x + 95, y), new Vector2(320, 34), onChange,
                new Vector2(0.5f, 1f));
        }

        private void ToggleRow(Transform parent, string label, bool value, float x, float y, System.Action<bool> onChange)
        {
            UIFactory.Toggle(parent, label, value, new Vector2(x, y), new Vector2(380, 40), onChange,
                new Vector2(0.5f, 1f));
        }

        /// <summary>A "Label [value ▸]" row whose button cycles a value; the action returns the new label.</summary>
        private void CycleRow(Transform parent, string label, string value, float x, float y, System.Func<string> onCycle)
        {
            UIFactory.LabelAt(parent, label, 24, Palette.UiText,
                new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(x - 150, y), new Vector2(170, 44),
                TextAnchor.MiddleLeft);
            var btn = UIFactory.Button(parent, value, new Vector2(x + 100, y), new Vector2(230, 42),
                null, Palette.UiPanel, Palette.UiAccent, 22, new Vector2(0.5f, 1f));
            var txt = btn.GetComponentInChildren<Text>();
            btn.onClick.AddListener(() =>
            {
                if (Sfx.Instance != null) Sfx.Instance.PlayClick();
                txt.text = onCycle();
            });
        }

        private void SectionHeaderAt(Transform parent, string label, float x, float y)
        {
            UIFactory.LabelAt(parent, label, 24, Palette.UiAccent,
                new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(x, y), new Vector2(420, 36),
                TextAnchor.MiddleLeft, FontStyle.Bold);
        }

        // ========================================================= LEADERBOARD
        private GameObject _lbHolder;
        private void BuildLeaderboard()
        {
            _leaderboard = MenuRoot("Leaderboard");
            var t = _leaderboard.transform;
            UIFactory.LabelAt(t, "LEADERBOARDS", 52, Palette.UiText,
                new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0, -36), new Vector2(1000, 64),
                TextAnchor.UpperCenter, FontStyle.Bold);
            _lbTitle = UIFactory.LabelAt(t, "", 28, Palette.UiAccent,
                new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0, -100), new Vector2(1000, 40),
                TextAnchor.UpperCenter, FontStyle.Bold);

            UIFactory.Button(t, "◀", new Vector2(-620, -108), new Vector2(64, 56),
                () => ChangeLbWorld(-1), Palette.UiPanelSoft, Palette.UiText, 30);
            UIFactory.Button(t, "▶", new Vector2(620, -108), new Vector2(64, 56),
                () => ChangeLbWorld(+1), Palette.UiPanelSoft, Palette.UiText, 30);

            _selLeaderboard = BuildBackButton(t, () => _gm.RequestCloseLeaderboard());
        }

        private void ChangeLbWorld(int dir)
        {
            int n = LevelLibrary.WorldCount;
            _selectedWorld = ((_selectedWorld + dir) % n + n) % n;
            if (Sfx.Instance != null) Sfx.Instance.PlayClick();
            RefreshLeaderboard();
        }

        public void ShowLeaderboard()
        {
            HideAll();
            _selectedWorld = Mathf.Clamp(_selectedWorld, 0, LevelLibrary.WorldCount - 1);
            _leaderboard.SetActive(true);
            RefreshLeaderboard();
            SetSelected(_selLeaderboard);
        }

        private void RefreshLeaderboard()
        {
            var t = _leaderboard.transform;
            if (_lbHolder != null) Destroy(_lbHolder);
            _lbHolder = UIFactory.Rect(t, "LBHolder").gameObject;
            var holder = (RectTransform)_lbHolder.transform;
            UIFactory.Stretch(holder);

            var world = LevelLibrary.WorldAt(_selectedWorld);
            _lbTitle.text = world.Name;

            // full-run best for this world
            string frKey = "vel.fullrun." + world.Id;
            string frStr = PlayerPrefs.HasKey(frKey)
                ? "Best Full Run  " + SpeedrunTimer.Format(PlayerPrefs.GetFloat(frKey))
                : "Best Full Run  —";
            UIFactory.LabelAt(holder, frStr, 24, Palette.UiGold,
                new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0, -150), new Vector2(900, 36),
                TextAnchor.UpperCenter, FontStyle.Bold);

            // grid: up to 4 columns across, wrapping to a second row (handles the 8-painting world)
            int n = world.Levels.Count;
            int perRow = n <= 4 ? n : Mathf.CeilToInt(n / 2f);
            int rowCount = Mathf.CeilToInt((float)n / perRow);
            float colW = 420, gapX = 28, gapY = 24;
            float colH = rowCount > 1 ? 338 : 540;
            int showRows = rowCount > 1 ? 6 : 10;
            float totalW = perRow * colW + (perRow - 1) * gapX;
            float startX = -totalW / 2f + colW / 2f;
            float topY = -196;

            for (int i = 0; i < n; i++)
            {
                var lvl = world.Levels[i];
                int rr = i / perRow, cc = i % perRow;
                float cx = startX + cc * (colW + gapX);
                float cy = topY - rr * (colH + gapY);
                var col = UIFactory.Box(holder, new Vector2(cx, cy), new Vector2(colW, colH),
                    Palette.UiPanelSoft, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), "Col");

                UIFactory.LabelAt(col.transform, lvl.Id + " · " + lvl.DisplayName, 22, Palette.UiAccent,
                    new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0, -16), new Vector2(colW - 24, 36),
                    TextAnchor.UpperCenter, FontStyle.Bold);

                var times = Leaderboard.GetTimes(lvl.Id);
                if (times.Count == 0)
                {
                    UIFactory.LabelAt(col.transform, "no times yet", 22, Palette.UiTextDim,
                        new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0, -84), new Vector2(colW - 40, 36),
                        TextAnchor.UpperCenter, FontStyle.Italic);
                }
                int rowsToShow = Mathf.Min(times.Count, showRows);
                for (int r = 0; r < rowsToShow; r++)
                {
                    Color c = r == 0 ? Palette.UiGold : Palette.UiText;
                    UIFactory.LabelAt(col.transform, $"{r + 1,2}.", 24, Palette.UiTextDim,
                        new Vector2(0, 1f), new Vector2(0, 1f), new Vector2(28, -62 - r * 40), new Vector2(60, 36),
                        TextAnchor.UpperLeft);
                    UIFactory.LabelAt(col.transform, SpeedrunTimer.Format(times[r]), 26, c,
                        new Vector2(1, 1f), new Vector2(1, 1f), new Vector2(-30, -62 - r * 40), new Vector2(220, 36),
                        TextAnchor.UpperRight, r == 0 ? FontStyle.Bold : FontStyle.Normal);
                }
            }
        }

        // =============================================================== PAUSE
        private void BuildPause()
        {
            _pause = UIFactory.Panel(_canvas.transform, new Color(0.04f, 0.05f, 0.08f, 0.85f), "Pause").gameObject;
            var t = _pause.transform;

            UIFactory.LabelAt(t, "PAUSED", 80, Palette.UiText,
                new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0, -200), new Vector2(800, 100),
                TextAnchor.UpperCenter, FontStyle.Bold);

            float y = 40;
            _selPause = UIFactory.Button(t, "RESUME", new Vector2(0, y), new Vector2(380, 72),
                () => _gm.RequestResume(), Palette.UiAccentDim, Palette.UiText, 34).gameObject;
            UIFactory.Button(t, "RESTART", new Vector2(0, y - 92), new Vector2(380, 64),
                () => _gm.RequestRetry());
            UIFactory.Button(t, "SETTINGS", new Vector2(0, y - 168), new Vector2(380, 64),
                () => _gm.RequestSettings());
            UIFactory.Button(t, "LEVEL SELECT", new Vector2(0, y - 244), new Vector2(380, 64),
                () => _gm.RequestLevelSelect());
            UIFactory.Button(t, "MAIN MENU", new Vector2(0, y - 320), new Vector2(380, 64),
                () => _gm.RequestMainMenu());
        }

        public void ShowPause(bool show)
        {
            _pause.SetActive(show);
            if (show) SetSelected(_selPause);
        }

        // ============================================================= RESULTS
        private void BuildResults()
        {
            _results = UIFactory.Panel(_canvas.transform, new Color(0.04f, 0.05f, 0.08f, 0.88f), "Results").gameObject;
            var t = _results.transform;

            _resTitle = UIFactory.LabelAt(t, "LEVEL COMPLETE", 72, Palette.UiAccent,
                new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0, -120), new Vector2(1200, 90),
                TextAnchor.UpperCenter, FontStyle.Bold);

            _resTime = UIFactory.LabelAt(t, "0:00.000", 110, Palette.UiText,
                new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0, -230), new Vector2(1000, 130),
                TextAnchor.UpperCenter, FontStyle.Bold);

            _resDelta = UIFactory.LabelAt(t, "", 34, Palette.UiTextDim,
                new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0, -360), new Vector2(900, 44),
                TextAnchor.UpperCenter, FontStyle.Bold);

            _resMedal = UIFactory.LabelAt(t, "", 30, Palette.UiGold,
                new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0, -410), new Vector2(900, 40),
                TextAnchor.UpperCenter, FontStyle.Bold);

            _resRank = UIFactory.LabelAt(t, "", 26, Palette.UiTextDim,
                new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0, -454), new Vector2(900, 36),
                TextAnchor.UpperCenter);

            _resCoins = UIFactory.LabelAt(t, "", 28, Palette.UiGold,
                new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0, -496), new Vector2(900, 38),
                TextAnchor.UpperCenter, FontStyle.Bold);

            _resExtra = UIFactory.LabelAt(t, "", 24, Palette.UiTextDim,
                new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0, -536), new Vector2(1000, 34),
                TextAnchor.UpperCenter, FontStyle.Bold);

            float y = -600;
            _resNext = UIFactory.Button(t, "NEXT", new Vector2(-300, y), new Vector2(260, 70),
                () => _gm.RequestNext(), Palette.UiAccentDim, Palette.UiText, 32, new Vector2(0.5f, 1f));
            _resRetry = UIFactory.Button(t, "RETRY", new Vector2(0, y), new Vector2(260, 70),
                () => _gm.RequestRetry(), Palette.UiPanelSoft, Palette.UiText, 32, new Vector2(0.5f, 1f));
            UIFactory.Button(t, "LEVELS", new Vector2(300, y), new Vector2(260, 70),
                () => _gm.RequestLevelSelect(), Palette.UiPanelSoft, Palette.UiText, 32, new Vector2(0.5f, 1f));
        }

        public void ShowResults(LevelDefinition lvl, double time, double? prevBest,
            bool isRecord, int rank, bool hasNext, int coins, int coinsTotal, int deaths, int bestCombo)
        {
            HideAll();
            _results.SetActive(true);

            _resCoins.enabled = coinsTotal > 0;
            if (coinsTotal > 0)
            {
                _resCoins.text = $"◎ Coins  {coins}/{coinsTotal}" + (coins == coinsTotal ? "   ALL!" : "");
                _resCoins.color = coins == coinsTotal ? Palette.UiGold : Palette.UiTextDim;
            }

            string grade = LevelRank(lvl, time, coins, coinsTotal, deaths);
            _resExtra.text = $"RANK {grade}     Deaths {deaths}" + (bestCombo >= 2 ? $"     Best combo x{bestCombo}" : "");
            _resExtra.color = grade == "S" ? Palette.UiGold : (grade == "A" ? Palette.UiAccent : Palette.UiTextDim);

            _resTitle.text = isRecord ? "NEW RECORD!" : "LEVEL COMPLETE";
            _resTitle.color = isRecord ? Palette.UiGold : Palette.UiAccent;
            _resTime.text = SpeedrunTimer.Format(time);

            if (isRecord && prevBest.HasValue)
            {
                _resDelta.text = "-" + SpeedrunTimer.Format(prevBest.Value - time) + " vs old PB";
                _resDelta.color = Palette.UiAccent;
            }
            else if (prevBest.HasValue)
            {
                double diff = time - prevBest.Value;
                _resDelta.text = "+" + SpeedrunTimer.Format(diff) + " off your PB";
                _resDelta.color = Palette.UiDanger;
            }
            else
            {
                _resDelta.text = "first clear!";
                _resDelta.color = Palette.UiAccent;
            }

            _resMedal.text = MedalLine(lvl, time);
            _resMedal.color = MedalColor(lvl, true, time);
            _resRank.text = rank > 0 ? $"#{rank} on your local board" : "outside your top 10";

            _resNext.gameObject.SetActive(hasNext);
            SetSelected(hasNext ? _resNext.gameObject : _resRetry.gameObject);
        }

        // ======================================================= FULL-RUN RESULTS
        private void BuildFullResults()
        {
            _fullResults = UIFactory.Panel(_canvas.transform, new Color(0.04f, 0.05f, 0.08f, 0.92f), "FullResults").gameObject;
            var t = _fullResults.transform;

            _frTitle = UIFactory.LabelAt(t, "WORLD COMPLETE", 64, Palette.UiAccent,
                new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0, -80), new Vector2(1200, 80),
                TextAnchor.UpperCenter, FontStyle.Bold);
            _frTotal = UIFactory.LabelAt(t, "0:00.000", 100, Palette.UiText,
                new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0, -180), new Vector2(1000, 120),
                TextAnchor.UpperCenter, FontStyle.Bold);
            _frDelta = UIFactory.LabelAt(t, "", 30, Palette.UiTextDim,
                new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0, -300), new Vector2(900, 40),
                TextAnchor.UpperCenter, FontStyle.Bold);

            float y = -560;
            _selFull = UIFactory.Button(t, "RUN AGAIN", new Vector2(-200, y), new Vector2(320, 70),
                () => _gm.RequestRetry(), Palette.UiAccentDim, Palette.UiText, 30, new Vector2(0.5f, 1f)).gameObject;
            UIFactory.Button(t, "LEVELS", new Vector2(200, y), new Vector2(320, 70),
                () => _gm.RequestLevelSelect(), Palette.UiPanelSoft, Palette.UiText, 30, new Vector2(0.5f, 1f));
        }

        public void ShowFullRunResults(World world, double total, List<double> splits,
            double? prevBest, bool isRecord)
        {
            HideAll();
            _fullResults.SetActive(true);

            _frTitle.text = isRecord ? "NEW BEST RUN!" : "WORLD COMPLETE";
            _frTitle.color = isRecord ? Palette.UiGold : Palette.UiAccent;
            _frTotal.text = SpeedrunTimer.Format(total);

            if (isRecord && prevBest.HasValue)
            {
                _frDelta.text = "-" + SpeedrunTimer.Format(prevBest.Value - total) + " vs old best run";
                _frDelta.color = Palette.UiAccent;
            }
            else if (prevBest.HasValue)
            {
                _frDelta.text = "+" + SpeedrunTimer.Format(total - prevBest.Value) + " off your best run";
                _frDelta.color = Palette.UiDanger;
            }
            else
            {
                _frDelta.text = "first full run!";
                _frDelta.color = Palette.UiAccent;
            }

            if (_frSplitsHolder != null) Destroy(_frSplitsHolder);
            _frSplitsHolder = UIFactory.Rect(_fullResults.transform, "Splits").gameObject;
            var holder = (RectTransform)_frSplitsHolder.transform;
            UIFactory.Stretch(holder);

            for (int i = 0; i < splits.Count && i < world.Levels.Count; i++)
            {
                float ry = -360 - i * 46;
                var lvl = world.Levels[i];
                UIFactory.LabelAt(holder, lvl.Id + "  " + lvl.DisplayName, 26, Palette.UiText,
                    new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(-180, ry), new Vector2(420, 40),
                    TextAnchor.MiddleLeft);
                UIFactory.LabelAt(holder, SpeedrunTimer.Format(splits[i]), 26, Palette.UiTextDim,
                    new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(220, ry), new Vector2(220, 40),
                    TextAnchor.MiddleRight, FontStyle.Bold);
            }

            SetSelected(_selFull);
        }

        // ============================================================== GAME OVER
        private void BuildGameOver()
        {
            _gameOver = UIFactory.Panel(_canvas.transform, new Color(0.07f, 0.02f, 0.04f, 0.93f), "GameOver").gameObject;
            var t = _gameOver.transform;

            UIFactory.LabelAt(t, "GAME OVER", 92, Palette.UiDanger,
                new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0, -170), new Vector2(1200, 110),
                TextAnchor.UpperCenter, FontStyle.Bold);
            _goSub = UIFactory.LabelAt(t, "", 30, Palette.UiTextDim,
                new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0, -296), new Vector2(1000, 44),
                TextAnchor.UpperCenter, FontStyle.Bold);

            float y = 40;
            _selGameOver = UIFactory.Button(t, "RETRY", new Vector2(0, y), new Vector2(380, 72),
                () => _gm.RequestRetry(), Palette.UiAccentDim, Palette.UiText, 34).gameObject;
            UIFactory.Button(t, "LEVEL SELECT", new Vector2(0, y - 92), new Vector2(380, 64),
                () => _gm.RequestLevelSelect());
            UIFactory.Button(t, "MAIN MENU", new Vector2(0, y - 168), new Vector2(380, 64),
                () => _gm.RequestMainMenu());
        }

        public void ShowGameOver(int deaths)
        {
            HideAll();
            _gameOver.SetActive(true);
            _goSub.text = "out of lives   •   " + deaths + (deaths == 1 ? " death" : " deaths");
            SetSelected(_selGameOver);
        }

        // ============================================================ helpers
        private GameObject MenuRoot(string name)
        {
            var panel = UIFactory.Panel(_canvas.transform, Palette.Background, name);
            // thin accent bar pinned across the very top
            var bar = UIFactory.Rect(panel.transform, "TopAccent");
            bar.anchorMin = new Vector2(0f, 1f);
            bar.anchorMax = new Vector2(1f, 1f);
            bar.pivot = new Vector2(0.5f, 1f);
            bar.sizeDelta = new Vector2(0f, 5f);
            bar.anchoredPosition = Vector2.zero;
            bar.gameObject.AddComponent<Image>().color = Palette.UiAccent;
            return panel.gameObject;
        }

        private GameObject BuildBackButton(Transform parent, UnityEngine.Events.UnityAction onBack)
        {
            return UIFactory.Button(parent, "BACK", new Vector2(40, 40), new Vector2(200, 60),
                () => onBack(), Palette.UiPanelSoft, Palette.UiText, 28, new Vector2(0f, 0f)).gameObject;
        }

        public void HideAll()
        {
            _mainMenu.SetActive(false);
            _levelSelect.SetActive(false);
            _settings.SetActive(false);
            _leaderboard.SetActive(false);
            _pause.SetActive(false);
            _results.SetActive(false);
            if (_fullResults != null) _fullResults.SetActive(false);
            if (_gameOver != null) _gameOver.SetActive(false);
            if (_owPanel != null) _owPanel.SetActive(false);
            if (_intro != null) _intro.SetActive(false);
        }

        // ---- rank / medal helpers ------------------------------------------
        private static string LevelRank(LevelDefinition lvl, double time, int coins, int coinsTotal, int deaths)
        {
            bool allCoins = coinsTotal == 0 || coins >= coinsTotal;
            if (time <= lvl.GoldTime && allCoins && deaths == 0) return "S";
            if (time <= lvl.GoldTime) return "A";
            if (time <= lvl.SilverTime) return "B";
            return "C";
        }

        private static string MedalName(LevelDefinition lvl, double time)
        {
            if (time <= lvl.GoldTime) return "GOLD";
            if (time <= lvl.SilverTime) return "SILVER";
            return "CLEAR";
        }

        private static string MedalLine(LevelDefinition lvl, double time)
        {
            if (time <= lvl.GoldTime) return "★ GOLD  (target " + SpeedrunTimer.Format(lvl.GoldTime) + ")";
            if (time <= lvl.SilverTime) return "● SILVER  (gold " + SpeedrunTimer.Format(lvl.GoldTime) + ")";
            return "○ CLEAR  (silver " + SpeedrunTimer.Format(lvl.SilverTime) + ")";
        }

        private static Color MedalColor(LevelDefinition lvl, bool has, double time)
        {
            if (!has) return Palette.UiTextDim;
            if (time <= lvl.GoldTime) return Palette.UiGold;
            if (time <= lvl.SilverTime) return new Color(0.78f, 0.82f, 0.9f);
            return Palette.UiText;
        }
    }
}
