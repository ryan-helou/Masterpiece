using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Velocity
{
    public enum GameState { Boot, MainMenu, Overworld, LevelSelect, Settings, Leaderboard, Playing, Paused, Results }

    /// <summary>
    /// Central state machine: owns the run timer, loads/unloads levels, spawns the player, drives
    /// the camera and routes every UI navigation request. There is exactly one in the scene.
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        private enum RunMode { Single, FullRun }
        private enum StartState { WaitInput, Countdown, Running }

        private GameInput _input;
        private UIManager _ui;
        private CameraController _cam;
        private PlayerController _player;
        private Transform _levelHolder;

        private readonly SpeedrunTimer _timer = new SpeedrunTimer();

        private GameState _state = GameState.Boot;
        private GameState _overlayReturn = GameState.MainMenu;

        private int _levelIndex;
        private BuiltLevel _built;
        private bool _levelLoaded;
        private float _killY;
        private float _lastDeathTime;
        private bool _dying;

        private readonly GhostRecorder _recorder = new GhostRecorder();
        private GhostRunner _ghost;

        private Overworld _overworld;
        private int _lastNode;            // gallery frame to return to

        private int _coinsCollected, _coinsTotal, _coinsPending;
        private int _deaths;
        private int _lives;
        private bool _hitstopActive;

        // run / start state
        private RunMode _runMode = RunMode.Single;
        private int _runWorld, _runLocalIndex;
        private double _runTotal;
        private readonly List<double> _runSplits = new List<double>();
        private StartState _startState;
        private float _countdown;
        private bool _suppressIntroOnce;   // skip the intro card on a death-reload

        public void Setup(GameInput input, UIManager ui, CameraController cam, Transform levelHolder)
        {
            Instance = this;
            _input = input;
            _ui = ui;
            _cam = cam;
            _levelHolder = levelHolder;
        }

        public void Boot() => RequestMainMenu();

        private double DisplayTime => _runMode == RunMode.FullRun ? _runTotal + _timer.Elapsed : _timer.Elapsed;

        // ============================================================== update
        private void Update()
        {
            if (_input == null) return;

            switch (_state)
            {
                case GameState.Playing:   UpdatePlaying(); break;
                case GameState.Paused:    if (_input.PausePressed) RequestResume(); break;
                case GameState.Overworld: if (_overworld != null) _overworld.Tick(); break;
            }
        }

        private void UpdatePlaying()
        {
            if (_dying)
            {
                _timer.Tick(Time.unscaledDeltaTime);
                _ui.TickHud(DisplayTime, _timer.Running);
                return;
            }

            // countdown to GO
            if (_startState == StartState.Countdown)
            {
                if (_input.PausePressed) { Pause(); return; }
                _countdown -= Time.unscaledDeltaTime;
                _ui.SetCountdown(_countdown);
                if (_countdown <= 0f)
                {
                    _ui.SetCountdown(-1f);
                    _startState = StartState.Running;
                    if (_player != null) _player.SetControl(true);
                    _timer.Begin();
                }
                _ui.TickHud(DisplayTime, _timer.Running);
                return;
            }

            if (_input.PausePressed) { Pause(); return; }
            if (_input.RestartPressed) { RequestRetry(); return; }

            if (_startState == StartState.WaitInput &&
                _player != null && _player.MovementInputThisFrame)
            {
                _startState = StartState.Running;
                _timer.Begin();
                _ui.DismissIntro();
            }

            _timer.Tick(Time.unscaledDeltaTime);

            if (_timer.Running && _player != null)
                _recorder.Record(_timer.Elapsed, _player.transform.position);
            if (_ghost != null)
            {
                if (_timer.Running) _ghost.SetTime(_timer.Elapsed);
                else _ghost.ResetToStart();
            }

            if (_levelLoaded && _player != null && _player.transform.position.y < _killY)
                OnPlayerDied();

            // live +/- vs your PB ghost (compares time at the same horizontal progress)
            float delta = 0f; bool hasDelta = false;
            if (_ghost != null && _timer.Running && _player != null)
            {
                float gt = _ghost.TimeAtX(_player.transform.position.x);
                if (gt >= 0f) { delta = (float)_timer.Elapsed - gt; hasDelta = true; }
            }
            if (Music.Instance != null && _player != null && _built.Bounds.size.x > 1f)
                Music.Instance.SetIntensity(
                    (_player.transform.position.x - _built.Bounds.min.x) / _built.Bounds.size.x,
                    Mathf.Abs(_player.Velocity.x) / 11f);

            int combo = _player != null ? _player.Combo : 0;
            _ui.TickHud(DisplayTime, _timer.Running, combo, delta, hasDelta);
            if (_player != null)
            {
                _ui.SetHudPower(_player.ActivePower, _player.PowerFraction);
                _ui.SetHudDash(_player.DashCharge, _player.DashReady);
            }
        }

        // ============================================================ level I/O
        private void LoadLevel(int index)
        {
            index = Mathf.Clamp(index, 0, LevelLibrary.Count - 1);
            _levelIndex = index;
            DestroyOverworld();
            UnloadLevel();

            var def = LevelLibrary.Get(index);
            var theme = PaintingThemes.For(index);
            Palette.ApplyPainting(theme);                 // retint the whole world to this painting
            LevelArt.Use(theme.ArtFolder);                // load imported art for this level (cached)
            if (Music.Instance != null) Music.Instance.SetTheme(theme.Style);   // the painting's score
            if (Camera.main != null) Camera.main.backgroundColor = Palette.Background;

            _built = LevelBuilder.Build(def, _levelHolder);
            _killY = _built.Bounds.min.y - 4f;
            _coinsTotal = _built.CoinCount;
            _coinsCollected = 0;
            _coinsPending = 0;

            var bg = new GameObject("Backdrop").AddComponent<PaintingBackdrop>();
            bg.transform.SetParent(_built.Root, false);
            bg.Setup(_cam.transform, theme, _built.Bounds);

            var playerGo = new GameObject("Player");
            playerGo.transform.SetParent(_levelHolder, false);
            _player = playerGo.AddComponent<PlayerController>();
            _player.Init(_input);
            _player.Respawn(_built.Spawn);

            _recorder.Reset();
            _ghost = null;
            if (Settings.ShowGhost)
            {
                var data = GhostStore.Load(def.Id);
                if (data != null && data.Count > 1)
                {
                    _ghost = new GameObject("Ghost").AddComponent<GhostRunner>();
                    _ghost.transform.SetParent(_levelHolder, false);
                    _ghost.Setup(data);
                }
            }

            _cam.SetTarget(_player.transform);
            _cam.SetBounds(_built.Bounds);
            _cam.Snap();

            _timer.Reset();
            _levelLoaded = true;
            _dying = false;
            _countdown = -1f;
            _ui.SetCountdown(-1f);

            Time.timeScale = 1f;
            _state = GameState.Playing;

            _ui.HideAll();
            _ui.ShowHud(true);

            Leaderboard.TryGetBest(def.Id, out double best);
            string gold = ColorUtility.ToHtmlStringRGB(Palette.UiGold);
            string label = $"<color=#{gold}>{def.Id}</color>  {def.DisplayName}";
            if (_runMode == RunMode.FullRun)
            {
                int n = LevelLibrary.WorldAt(_runWorld).Levels.Count;
                label = $"<color=#{gold}>RUN {_runLocalIndex + 1}/{n}</color>  •  {def.DisplayName}";
            }
            _ui.SetHudLevel(label, Leaderboard.HasCompleted(def.Id) ? best : (double?)null);
            // live medal-pace tint on the clock — single runs only (a full run's clock is cumulative)
            if (_runMode == RunMode.Single) _ui.SetPace(def.GoldTime, def.SilverTime);
            else _ui.SetPace(0, 0);
            _ui.SetHudCoins(_coinsCollected, _coinsPending, _coinsTotal);
            _ui.SetHudDeaths(_deaths);
            _ui.SetHudLives(0, false);
            _ui.SetHudPower(PowerUp.None, 0f);

            // decide how the level starts
            bool continuation = _runMode == RunMode.FullRun && _runLocalIndex > 0;
            if (continuation)
            {
                _startState = StartState.Running;
                _player.SetControl(true);
                _timer.Begin();
            }
            else if (Settings.CountdownStart && !_suppressIntroOnce)
            {
                _startState = StartState.Countdown;
                _countdown = 3.2f;
                _player.SetControl(false);
                _ui.SetCountdown(_countdown);
            }
            else
            {
                _startState = StartState.WaitInput;
                _player.SetControl(true);
                if (!_suppressIntroOnce) _ui.ShowLevelIntro(def);
            }
            _suppressIntroOnce = false;
        }

        private void UnloadLevel()
        {
            if (_ghost != null) Destroy(_ghost.gameObject);
            _ghost = null;
            if (_built.Root != null) Destroy(_built.Root.gameObject);
            if (_player != null) Destroy(_player.gameObject);
            _player = null;
            _levelLoaded = false;
            _dying = false;
            _ui.SetCountdown(-1f);
            _cam.SetTarget(null);
            _cam.ClearBounds();
        }

        // ============================================================ gameplay
        private void Pause()
        {
            _state = GameState.Paused;
            Time.timeScale = 0f;
            if (_player != null) _player.SetControl(false);
            _ui.ShowPause(true);
        }

        // Celeste-style banking: a grabbed coin is PENDING until you next touch safe ground —
        // die first (a full-run respawn) and it's forfeit. Grabbing while already grounded banks
        // at once. (In single-level mode a death reloads the level, so this is the full-run hook.)
        public void OnCoinGrabbed()
        {
            _coinsPending++;
            _ui.SetHudCoins(_coinsCollected, _coinsPending, _coinsTotal);
            if (_player != null && _player.Grounded) BankCoins();
        }

        public void OnPlayerLandedSafe() => BankCoins();

        private void BankCoins()
        {
            if (_coinsPending <= 0) return;
            _coinsCollected += _coinsPending;
            _coinsPending = 0;
            _ui.SetHudCoins(_coinsCollected, _coinsPending, _coinsTotal);
            if (Sfx.Instance != null) Sfx.Instance.PlayBank();
        }

        /// <summary>A quick screen pulse used for portal/pipe warps.</summary>
        public void FlashTransition() => _ui.Flash(Palette.UiAccent, 0.3f);

        /// <summary>Camera shake hook (respects accessibility settings inside the camera).</summary>
        public void Shake(float amount) { if (_cam != null) _cam.AddShake(amount); }

        /// <summary>Brief freeze-frame for impact (stomps). Safe around pause/death.</summary>
        public void Hitstop(float seconds)
        {
            if (_hitstopActive || _dying || _state != GameState.Playing) return;
            StartCoroutine(HitstopCo(seconds));
        }

        private IEnumerator HitstopCo(float seconds)
        {
            _hitstopActive = true;
            if (_state == GameState.Playing && !_dying) Time.timeScale = 0f;
            yield return new WaitForSecondsRealtime(seconds);
            if (_state == GameState.Playing && !_dying) Time.timeScale = 1f;
            _hitstopActive = false;
        }

        public void OnReachGoal()
        {
            if (_state != GameState.Playing) return;

            _timer.Stop();
            if (_player != null) _player.Freeze();

            var def = LevelLibrary.Get(_levelIndex);
            double time = _timer.Elapsed;

            _coinsCollected += _coinsPending; _coinsPending = 0;   // banked at the goal
            Coins.Submit(def.Id, _coinsCollected);
            bool hadBest = Leaderboard.TryGetBest(def.Id, out double prevBest);
            int rank = Leaderboard.Submit(def.Id, time, out bool isRecord);
            if (isRecord) GhostStore.Save(def.Id, _recorder.Samples);

            if (Music.Instance != null) Music.Instance.Duck(0.35f, 1.2f);
            if (Sfx.Instance != null)
            {
                if (isRecord) Sfx.Instance.PlayRecord();
                else Sfx.Instance.PlayFinish();
            }
            Vector3 goalPos = _player != null ? _player.transform.position : Vector3.zero;
            if (FX.Instance != null) FX.Instance.Finish(goalPos);
            _cam.AddShake(0.35f);
            _ui.Flash(Palette.Goal, 0.22f);

            if (_runMode == RunMode.FullRun)
            {
                _runSplits.Add(time);
                _runTotal += time;
                int worldLevels = LevelLibrary.WorldAt(_runWorld).Levels.Count;
                if (_runLocalIndex < worldLevels - 1)
                {
                    _runLocalIndex++;
                    LoadLevel(LevelLibrary.GlobalIndex(_runWorld, _runLocalIndex));
                    return;
                }
                FinishFullRun();
                return;
            }

            Time.timeScale = 1f;
            _state = GameState.Results;
            _ui.ShowHud(false);
            var (world, local) = LevelLibrary.Locate(_levelIndex);
            bool hasNext = local < LevelLibrary.WorldAt(world).Levels.Count - 1;
            int bestCombo = _player != null ? _player.BestCombo : 0;
            _ui.ShowResults(def, time, hadBest ? prevBest : (double?)null,
                isRecord, rank, hasNext, _coinsCollected, _coinsTotal, _deaths, bestCombo);
        }

        private void FinishFullRun()
        {
            var world = LevelLibrary.WorldAt(_runWorld);
            double total = _runTotal;
            string key = "vel.fullrun." + world.Id;
            bool hadBest = PlayerPrefs.HasKey(key);
            double prevBest = hadBest ? PlayerPrefs.GetFloat(key) : 0;
            bool isRecord = !hadBest || total < prevBest;
            if (isRecord) { PlayerPrefs.SetFloat(key, (float)total); PlayerPrefs.Save(); }

            if (Sfx.Instance != null) Sfx.Instance.PlayRecord();

            Time.timeScale = 1f;
            _state = GameState.Results;
            _ui.ShowHud(false);
            _ui.ShowFullRunResults(world, total, new List<double>(_runSplits),
                hadBest ? prevBest : (double?)null, isRecord);
        }

        private void GameOver()
        {
            Time.timeScale = 1f;
            _state = GameState.Results;          // gates gameplay; uses the results-style flow
            _ui.ShowHud(false);
            if (Sfx.Instance != null) Sfx.Instance.PlayDeath();
            _ui.ShowGameOver(_deaths);
        }

        public void OnPlayerDied()
        {
            if (_state != GameState.Playing || _dying) return;
            if (Time.unscaledTime - _lastDeathTime < 0.1f) return;
            _lastDeathTime = Time.unscaledTime;
            _dying = true;
            _deaths++;
            _ui.SetHudDeaths(_deaths);
            _coinsPending = 0;                                  // forfeit un-banked coins
            _ui.SetHudCoins(_coinsCollected, _coinsPending, _coinsTotal);
            // cancel any in-progress hit-stop so timeScale can't get stuck frozen
            _hitstopActive = false;
            Time.timeScale = 1f;

            Vector3 deathPos = _player != null ? _player.transform.position : Vector3.zero;
            if (Music.Instance != null) Music.Instance.Duck(0.45f, 0.7f);
            if (Sfx.Instance != null) Sfx.Instance.PlayDeath();
            if (FX.Instance != null) FX.Instance.Death(deathPos);
            _cam.AddShake(0.7f);
            _ui.Flash(Palette.UiDanger, 0.45f);
            if (_player != null) _player.Freeze();

            StartCoroutine(DeathSequence());
        }

        private IEnumerator DeathSequence()
        {
            yield return new WaitForSecondsRealtime(0.09f);
            if (_state != GameState.Playing) { _dying = false; yield break; }

            // One life — death just restarts the attempt. A single level reloads clean (enemies,
            // crumbles and moving pieces reset for a fair fresh try); a full-gallery run keeps its
            // cumulative clock and respawns in place.
            if (_runMode == RunMode.Single)
            {
                _dying = false;
                _suppressIntroOnce = true;
                LoadLevel(_levelIndex);
                yield break;
            }

            if (_player != null)
            {
                _player.Respawn(_built.Spawn);
                _player.SetControl(true);
            }
            _cam.Snap();
            if (_ghost != null) _ghost.ResetToStart();
            _dying = false;
        }

        // ====================================================== UI navigation
        public void RequestPlay() => OpenOverworld();

        public void RequestStartLevel(int globalIndex)
        {
            _runMode = RunMode.Single;
            _lastNode = globalIndex;          // remember which gallery frame we entered from
            _deaths = 0;
            _lives = Settings.StartingLives;
            LoadLevel(globalIndex);
        }

        public void RequestFullRun(int worldIndex)
        {
            _runMode = RunMode.FullRun;
            _runWorld = Mathf.Clamp(worldIndex, 0, LevelLibrary.WorldCount - 1);
            _runLocalIndex = 0;
            _runTotal = 0;
            _deaths = 0;
            _lives = Settings.StartingLives;
            _runSplits.Clear();
            LoadLevel(LevelLibrary.GlobalIndex(_runWorld, 0));
        }

        public void RequestRetry()
        {
            if (_runMode == RunMode.FullRun) RequestFullRun(_runWorld);
            else { _runMode = RunMode.Single; _deaths = 0; _lives = Settings.StartingLives; LoadLevel(_levelIndex); }
        }

        public void RequestNext()
        {
            _deaths = 0;
            _lives = Settings.StartingLives;
            var (world, local) = LevelLibrary.Locate(_levelIndex);
            if (local < LevelLibrary.WorldAt(world).Levels.Count - 1)
                LoadLevel(LevelLibrary.GlobalIndex(world, local + 1));
            else RequestLevelSelect();
        }

        public void RequestResume()
        {
            _state = GameState.Playing;
            Time.timeScale = 1f;
            if (_player != null && _startState != StartState.Countdown) _player.SetControl(true);
            _ui.ShowPause(false);
        }

        public void RequestLevelSelect() => OpenOverworld();

        /// <summary>The museum gallery walk that replaces the old card-based level select.</summary>
        private void OpenOverworld()
        {
            _runMode = RunMode.Single;
            DestroyOverworld();
            UnloadLevel();
            Time.timeScale = 1f;
            _state = GameState.Overworld;
            if (Music.Instance != null) Music.Instance.SetGallery();

            if (Camera.main != null) Camera.main.backgroundColor = Overworld.WallColor;

            var go = new GameObject("Overworld");
            go.transform.SetParent(_levelHolder, false);
            _overworld = go.AddComponent<Overworld>();

            _ui.HideAll();
            _ui.ShowHud(false);
            _ui.ShowOverworld(true);
            _overworld.Setup(_input, this, _ui, _cam, _lastNode);
        }

        private void DestroyOverworld()
        {
            if (_overworld != null)
            {
                Destroy(_overworld.gameObject);
                _overworld = null;
                _cam.SetTarget(null);
                _cam.ClearBounds();
            }
            if (_ui != null) _ui.ShowOverworld(false);
        }

        public void RequestMainMenu()
        {
            _runMode = RunMode.Single;
            DestroyOverworld();
            UnloadLevel();
            Time.timeScale = 1f;
            _state = GameState.MainMenu;
            if (Music.Instance != null) Music.Instance.SetGallery();
            _ui.ShowHud(false);
            _ui.ShowMainMenu();
        }

        public void RequestSettings()
        {
            _overlayReturn = _state;
            _state = GameState.Settings;
            _ui.ShowSettings();
        }

        public void RequestCloseSettings()
        {
            _input.CancelRebind();
            if (_overlayReturn == GameState.Paused)
            {
                _state = GameState.Paused;
                _ui.HideAll();
                _ui.ShowHud(true);
                _ui.ShowPause(true);
            }
            else RequestMainMenu();
        }

        public void RequestApplyTheme()
        {
            Palette.ApplyTheme(Settings.ColorTheme);
            // defer one frame so we don't destroy the clicked button mid-event
            StartCoroutine(DeferredThemeRebuild());
        }

        private IEnumerator DeferredThemeRebuild()
        {
            yield return null;
            _ui.RebuildAll();
            _ui.ShowSettings();
        }

        public void RequestLeaderboard()
        {
            _overlayReturn = _state;
            _state = GameState.Leaderboard;
            _ui.ShowLeaderboard();
        }

        public void RequestCloseLeaderboard()
        {
            if (_overlayReturn == GameState.Paused)
            {
                _state = GameState.Paused;
                _ui.HideAll();
                _ui.ShowHud(true);
                _ui.ShowPause(true);
            }
            else RequestMainMenu();
        }

        public void RequestQuit()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}
