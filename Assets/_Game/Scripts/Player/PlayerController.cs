using UnityEngine;

namespace Velocity
{
    /// <summary>A temporary, Mario-style pickup ability. Changes the character's look while held.</summary>
    public enum PowerUp { None, Glide, HighJump }

    /// <summary>
    /// Precise 2D platformer controller tuned for speedrunning. Custom gravity (gravityScale = 0)
    /// so we get full control of jump arcs: variable jump height, coyote time, jump buffering,
    /// apex hang, fast-fall, snappy turn-around, and a horizontal dash. Self-builds its
    /// Rigidbody2D / collider / sprite so it can be spawned with a single AddComponent.
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D))]
    public class PlayerController : MonoBehaviour
    {
        [Header("Run")]
        public float maxRunSpeed = 11f;
        public float groundAccel = 130f;
        public float groundDecel = 150f;
        public float airAccel = 95f;
        public float airDecel = 70f;
        public float turnMultiplier = 2.0f;     // extra accel when reversing direction (snappy)

        [Header("Jump")]
        public float jumpHeight = 3.3f;
        public float timeToApex = 0.33f;
        public float fallGravityMult = 1.95f;
        public float lowJumpGravityMult = 2.5f; // when jump released early -> short hop
        public float apexThreshold = 1.8f;
        public float apexGravityMult = 0.82f;   // brief, light hang at the top (snappy, not floaty)
        public float maxFallSpeed = 30f;
        public float coyoteTime = 0.09f;
        public float jumpBufferTime = 0.10f;
        public float jumpCutMultiplier = 0.4f;

        [Header("Dash")]
        public bool enableDash = true;
        public float dashSpeed = 17f;
        public float dashDuration = 0.12f;
        public float dashCooldown = 0.42f;      // quick, but stops dash-spam to fly across
        public int maxAirDashes = 1;
        public float dashBufferTime = 0.1f;     // press dash slightly early and it still fires

        [Header("Combat")]
        public float stompBounceVelocity = 15f;

        [Header("Wall jump")]
        public bool enableWallJump = true;
        public float wallSlideSpeed = 4f;       // gentle slide so you have time to think
        public float wallJumpX = 11.5f;         // push away from the wall
        public float wallJumpY = 18f;           // and up (bumped for the stronger gravity)
        public float wallJumpLockTime = 0.09f;  // briefly ignore input so the push-off sticks
        public float wallCoyoteTime = 0.12f;    // grace just after leaving a wall
        public float wallCatchDamp = 90f;       // how fast a rising grab bleeds upward speed

        [Header("Feel")]
        public bool enableCornerCorrect = true; // slip past clipped ledge corners instead of bonking
        public float cornerCorrectDist = 0.26f;

        [Header("Duck / Slide / Ground-pound")]
        public float duckSpeedMult = 0.42f;      // crawl this slow while crouched (from a standstill)
        public float slideFriction = 18f;        // a running crouch SLIDES: bleed momentum off this gently
        public float slideMinSpeed = 5f;         // below this a slide settles into the normal crouch-crawl
        public float slamSpeed = 36f;            // air + Down: rocket straight down (Mario ground-pound)
        public float slamArmThreshold = 1.5f;    // only start a slam once you're no longer rising hard

        [Header("Mechanics")]
        public float ringJumpBoost = 21f;       // Geometry-Dash ring: boosted air-jump
        public float floatGravityMult = 0.18f;  // GLIDE power-up: very floaty low gravity on the way down
        public float glideMaxFallMult = 0.32f;  // GLIDE: clamp the fall slow so you drift far
        public float boostLockTime = 0.3f;      // input lock so a boost-pad fling carries you across
        public float highJumpMult = 1.4f;       // HIGH-JUMP power-up: jump this much higher
        public const float PowerDuration = 15f; // power-ups last this long

        // ---- runtime state -------------------------------------------------
        private Rigidbody2D _rb;
        private CapsuleCollider2D _col;
        private SpriteRenderer _sr;
        private Transform _visual;
        private Sprite[] _walkFrames;
        private Sprite _idleArt, _jumpArt, _fallArt;
        private Transform _artBody;
        private bool _usingArt, _feetMode;
        private float _animTime;
        private Transform _face;
        private GameInput _input;

        private float _baseGravity;
        private float _jumpVelocity;

        private bool _controlEnabled;
        private bool _frozen;

        private bool _grounded;
        private bool _wasGrounded;
        private float _coyoteTimer;
        private float _jumpBufferTimer;
        private bool _jumpCutQueued;

        private bool _dashing;
        private bool _dashWasOnCd;
        private float _dashTimer;
        private float _dashCdTimer;
        private float _dashDir;
        private int _airDashesLeft;
        private float _dashBufferTimer;

        private int _facing = 1;
        private MovingPlatform _platform;
        private Vector2 _carry;
        private Vector2 _beltVel;       // conveyor velocity while grounded on one
        private float _pendingLaunch;   // spring launch queued for the next physics step

        private bool _hasGroundSurface; // a surface top was found under the feet this step
        private float _groundSurfaceY;  // its world Y — SeatOnGround plants the collider bottom here

        private int _wallDir;           // +1 wall on right, -1 wall on left, 0 none
        private int _lastWallDir;
        private float _wallCoyote;
        private float _wallJumpLock;
        private bool _wallSliding;

        private bool _ducking;          // crouched on the ground (shrunk collider, slow crawl)
        private bool _sliding;          // a ducked, momentum-preserving slide (entered the crouch at speed)
        private bool _slamming;         // air + Down ground-pound in progress
        private Vector2 _colSizeStand, _colSizeDuck, _colOffDuck;   // capsule presets

        private float _afterTimer;      // dash afterimage cadence
        private float _lastAirVelY;     // fall speed captured for landing impact
        private float _dustTimer;       // run-dust cadence
        private float _stepTimer;       // footstep cadence (scales with run speed)
        private float _scrapeTimer;     // wall-grind scuff cadence
        public int Combo { get; private set; }
        public int BestCombo { get; private set; }

        private float _ringTimer, _ringBoost;   // jump-ring window
        private Vector2 _pendingBoost; private bool _hasBoost;   // boost-pad fling
        private PowerUp _power;                   // active power-up (None / Glide / HighJump)
        private float _powerTimer;                // counts down to 0, then it's lost
        private SpriteRenderer _aura;             // glowing aura shown while powered

        public bool IsDashing => _dashing;
        public bool IsDucking => _ducking;
        public bool IsSliding => _sliding;
        public bool IsSlamming => _slamming;
        public int Facing => _facing;
        public float DashCharge => Mathf.Clamp01(1f - Mathf.Max(0f, _dashCdTimer) / Mathf.Max(0.01f, dashCooldown)); // 0 just dashed → 1 ready
        public bool Grounded => _grounded;
        public bool DashReady => _dashCdTimer <= 0f && (_grounded || _airDashesLeft > 0);
        public Vector2 Velocity => _rb != null ? _rb.linearVelocity : Vector2.zero;
        public bool MovementInputThisFrame { get; private set; }

        private void Awake()
        {
            BuildSelf();
            RecomputeGravity();
        }

        public static PlayerController Active { get; private set; }

        public void Init(GameInput input)
        {
            _input = input;
            Active = this;
        }

        private void OnDestroy()
        {
            if (Active == this) Active = null;
        }

        public void SetControl(bool enabled)
        {
            _controlEnabled = enabled;
        }

        public void Respawn(Vector3 position)
        {
            _rb.linearVelocity = Vector2.zero;
            _rb.position = position;
            transform.position = position;
            _dashing = false;
            _frozen = false;
            _coyoteTimer = 0;
            _jumpBufferTimer = 0;
            _dashCdTimer = 0;
            _airDashesLeft = maxAirDashes;
            _platform = null;
            _carry = Vector2.zero;
            _beltVel = Vector2.zero;
            _pendingLaunch = 0f;
            _wallDir = 0;
            _lastWallDir = 0;
            _wallCoyote = 0f;
            _wallJumpLock = 0f;
            _wallSliding = false;
            _ducking = false;
            _sliding = false;
            _slamming = false;
            ApplyDuckCollider(false);
            Combo = 0;
            _ringTimer = 0f;
            _hasBoost = false;
            _power = PowerUp.None; _powerTimer = 0f; ApplyPowerVisual();
            SetVisualScale(Vector3.one);
        }

        /// <summary>Stop the player dead (used on level complete).</summary>
        public void Freeze()
        {
            _frozen = true;
            _controlEnabled = false;
            if (_rb != null) _rb.linearVelocity = Vector2.zero;
        }

        /// <summary>Launch the player straight up next physics step (bounce pads).</summary>
        public void LaunchUp(float velocity)
        {
            _pendingLaunch = velocity;
        }

        /// <summary>Dash-orb refill. Returns false (so the orb isn't consumed) if dash is already full.</summary>
        public bool RefillDash()
        {
            if (_airDashesLeft >= maxAirDashes) return false;
            _airDashesLeft = maxAirDashes;
            _dashCdTimer = 0f;           // let you dash again immediately
            return true;
        }

        /// <summary>Jump ring (Geometry Dash): tap jump inside it for a boosted air-jump.</summary>
        public void OfferRing(float boost) { _ringTimer = 0.06f; _ringBoost = boost; }

        /// <summary>Boost pad / dash panel: fling the player next physics step.</summary>
        public void Boost(float vx, float vy) { _pendingBoost = new Vector2(vx, vy); _hasBoost = true; }

        /// <summary>Glide feather: floaty low-gravity for a few seconds.</summary>
        public PowerUp ActivePower => _power;
        public float PowerFraction => Mathf.Clamp01(_powerTimer / PowerDuration);

        public void GrantPower(PowerUp p)
        {
            _power = p;
            _powerTimer = PowerDuration;
            ApplyPowerVisual();
            if (Sfx.Instance != null) Sfx.Instance.PlayPowerUp();
            if (FX.Instance != null) FX.Instance.Burst(transform.position, PowerColor(p), 14, 6f, 0.16f, 0.5f, -8f, 3f);
        }

        private static Color PowerColor(PowerUp p)
        {
            switch (p)
            {
                case PowerUp.Glide:    return new Color(0.45f, 0.85f, 1f);   // cyan
                case PowerUp.HighJump: return new Color(0.45f, 1f, 0.5f);    // green
                default:               return Color.white;
            }
        }

        private void ApplyPowerVisual()
        {
            if (_aura != null)
            {
                _aura.enabled = _power != PowerUp.None;
                var c = PowerColor(_power); c.a = 0.5f;
                _aura.color = c;
            }
            if (_sr != null)
            {
                if (_usingArt)
                    _sr.color = _power == PowerUp.None ? Color.white : Color.Lerp(Color.white, PowerColor(_power), 0.55f);
                else
                    _sr.color = _power == PowerUp.None ? Palette.Player : Color.Lerp(Palette.Player, PowerColor(_power), 0.6f);
            }
        }

        /// <summary>Warp the player to a position, preserving velocity (portals/pipes).</summary>
        public void Teleport(Vector3 pos) { _rb.position = pos; transform.position = pos; }

        /// <summary>Small hop after stomping an enemy; refreshes the air dash and builds combo.</summary>
        public void StompBounce()
        {
            _pendingLaunch = stompBounceVelocity;
            _airDashesLeft = maxAirDashes;
            AddCombo();
        }

        private void AddCombo()
        {
            Combo++;
            if (Combo > BestCombo) BestCombo = Combo;
            if (Combo >= 2 && Sfx.Instance != null) Sfx.Instance.PlayCombo(Combo - 2);
        }

        // ---- input sampling (per render frame) -----------------------------
        private void Update()
        {
            MovementInputThisFrame = false;
            if (!_controlEnabled || _input == null) return;

            float mx = _input.MoveX;
            if (Mathf.Abs(mx) > 0.01f)
            {
                _facing = mx > 0 ? 1 : -1;
                MovementInputThisFrame = true;
            }

            if (_input.JumpPressed)
            {
                _jumpBufferTimer = jumpBufferTime;
                MovementInputThisFrame = true;
            }
            if (_input.JumpReleased && _rb.linearVelocity.y > 0f)
                _jumpCutQueued = true;

            if (enableDash && _input.DashPressed)
            {
                _dashBufferTimer = dashBufferTime;
                MovementInputThisFrame = true;
            }
        }

        // ---- physics step --------------------------------------------------
        private void FixedUpdate()
        {
            float dt = Time.fixedDeltaTime;

            if (_frozen)
            {
                _rb.linearVelocity = Vector2.zero;
                _carry = Vector2.zero;
                return;
            }

            // Work in the player's OWN velocity: strip the platform velocity we added last step
            // so riding a moving platform doesn't feed back into runaway acceleration.
            Vector2 vel = _rb.linearVelocity - _carry;
            if (!_grounded) _lastAirVelY = vel.y;   // remember fall speed for the landing impact

            _coyoteTimer -= dt;
            _jumpBufferTimer -= dt;
            _dashCdTimer -= dt;
            if (_dashWasOnCd && _dashCdTimer <= 0f)
            {
                _dashWasOnCd = false;
                if (Sfx.Instance != null) Sfx.Instance.PlayDashReady();
            }
            _dashBufferTimer -= dt;
            _ringTimer -= dt;
            if (_powerTimer > 0f)
            {
                _powerTimer -= dt;
                if (_powerTimer <= 0f) { _power = PowerUp.None; ApplyPowerVisual(); }
            }

            ProbeGround();
            if (_grounded)
            {
                _coyoteTimer = coyoteTime;
                _airDashesLeft = maxAirDashes;
            }

            ProbeWalls();
            if (!_grounded && _wallDir != 0) { _wallCoyote = wallCoyoteTime; _lastWallDir = _wallDir; }
            else _wallCoyote -= dt;
            _wallJumpLock -= dt;

            float mx = _controlEnabled && _input != null ? _input.MoveX : 0f;

            if (_dashing)
            {
                _dashTimer -= dt;
                vel = new Vector2(_dashDir * dashSpeed, 0f);

                // (speed trail emitted centrally below — covers dash, boost, ring, glide)

                if (_jumpBufferTimer > 0f && _coyoteTimer > 0f)
                {
                    EndDash(ref vel);
                    DoJump(ref vel);
                }
                else if (_dashTimer <= 0f)
                {
                    EndDash(ref vel);
                }
            }
            else if (_dashBufferTimer > 0f && _dashCdTimer <= 0f && (_grounded || _airDashesLeft > 0))
            {
                _dashing = true;
                _ducking = false; _sliding = false; _slamming = false; ApplyDuckCollider(false);
                _dashTimer = dashDuration;
                _dashDir = Mathf.Abs(mx) > 0.01f ? Mathf.Sign(mx) : _facing;
                if (!_grounded) { _airDashesLeft--; AddCombo(); }
                _dashBufferTimer = 0f;
                if (Sfx.Instance != null) Sfx.Instance.PlayDash();
                if (FX.Instance != null) FX.Instance.DashPuff(transform.position, (int)_dashDir);
                vel = new Vector2(_dashDir * dashSpeed, 0f);
            }
            else
            {
                bool wantDown = _controlEnabled && _input != null && _input.MoveY < -0.5f;
                bool buffered = _jumpBufferTimer > 0f;

                // ----- GROUND-POUND / SLAM: in the air + Down, rocket straight down (Mario-style) -----
                // A buffered jump cancels the intent (so you can still duck-jump off the ground).
                if (!_slamming && !_grounded && wantDown && !buffered &&
                    _power != PowerUp.Glide && vel.y < slamArmThreshold)
                    BeginSlam();

                if (_slamming)
                {
                    if (_grounded) { EndSlam(ref vel); }
                    else
                    {
                        vel.x = Mathf.MoveTowards(vel.x, 0f, groundDecel * 1.4f * dt);  // lock horizontal
                        vel.y = -slamSpeed;
                    }
                    _wallSliding = false;
                }
                else
                {
                    // ----- DUCK: crouch on the ground; stay crouched while a low ceiling blocks standing -----
                    bool canStand = HasHeadroom();
                    _ducking = _grounded && (wantDown || !canStand);

                    // ----- SLIDE: press Down while running and you keep your momentum in a low slide
                    // instead of snapping to the crouch-crawl speed — blast under low ceilings at speed
                    // and slide-jump out the far side. Speed bleeds off via gentle friction, then it
                    // settles into the normal crawl; the slide shares the crouch's shrunk hitbox.
                    float duckCap = maxRunSpeed * duckSpeedMult;
                    if (_ducking && _grounded && !_sliding && wantDown && Mathf.Abs(vel.x) > duckCap + 1f)
                    {
                        _sliding = true;
                        if (FX.Instance != null) FX.Instance.LandPuff(FeetPos());   // a puff as you drop into it
                    }
                    if (_sliding && (!_ducking || !_grounded || Mathf.Abs(vel.x) <= slideMinSpeed))
                        _sliding = false;
                    ApplyDuckCollider(_ducking);

                    // gripping a wall: airborne and touching it — a neutral stick still grips
                    // (only pressing AWAY refuses the wall), and a slow rise can soft-catch, so
                    // chimney rhythm doesn't demand frame-perfect stick flips
                    bool wallSlide = enableWallJump && !_grounded && _wallDir != 0 &&
                                     (Mathf.Abs(mx) <= 0.1f || Mathf.Sign(mx) == _wallDir) &&
                                     vel.y < 3f;

                    // horizontal (briefly suppressed after a wall jump so the push-off sticks)
                    if (_wallJumpLock <= 0f)
                    {
                        if (_sliding)
                        {
                            // pure momentum: steering is locked, speed bleeds off gently
                            vel.x = Mathf.MoveTowards(vel.x, 0f, slideFriction * dt);
                        }
                        else
                        {
                            float cap = _ducking ? duckCap : maxRunSpeed;
                            float target = mx * cap;
                            float accel;
                            if (Mathf.Abs(mx) > 0.01f)
                            {
                                accel = _grounded ? groundAccel : airAccel;
                                if (Mathf.Abs(vel.x) > 0.1f && Mathf.Sign(mx) != Mathf.Sign(vel.x))
                                    accel *= turnMultiplier;
                            }
                            else
                            {
                                accel = _grounded ? groundDecel : airDecel;
                            }
                            vel.x = Mathf.MoveTowards(vel.x, target, accel * dt);
                        }
                    }

                    // jump: ground/coyote (a duck-jump un-ducks first), then jump-ring, then wall jump
                    if (buffered && _coyoteTimer > 0f && canStand)
                    {
                        if (_ducking) { _ducking = false; ApplyDuckCollider(false); }
                        DoJump(ref vel);
                    }
                    else if (buffered && _ringTimer > 0f)
                        DoRingJump(ref vel);
                    else if (buffered && enableWallJump && !_grounded &&
                             (_wallDir != 0 || _wallCoyote > 0f))
                        DoWallJump(ref vel);

                    // vertical
                    if (_grounded && vel.y <= 0.01f)
                    {
                        vel.y = 0f;
                        SeatOnGround();    // sit the feet exactly on the surface (kill the probe-height hover)
                    }
                    else
                    {
                        float g = _baseGravity;
                        if (vel.y > 0f && !(_input != null && _input.JumpHeld))
                            g *= lowJumpGravityMult;
                        else if (Mathf.Abs(vel.y) < apexThreshold)
                            g *= apexGravityMult;
                        else if (vel.y < 0f)
                            g *= fallGravityMult;
                        if (_power == PowerUp.Glide && vel.y < 0f) g *= floatGravityMult;   // GLIDE: ONLY softens the fall, never boosts the jump

                        vel.y -= g * dt;
                        float maxFall = _power == PowerUp.Glide ? maxFallSpeed * glideMaxFallMult : maxFallSpeed;
                        if (vel.y < -maxFall) vel.y = -maxFall;

                        // wall slide: a rising grab bleeds its lift into the wall (soft catch,
                        // no bonk-and-wait), and the fall clamps so you have time to read it
                        if (wallSlide && vel.y > 0f) vel.y = Mathf.MoveTowards(vel.y, 0f, wallCatchDamp * dt);
                        if (wallSlide && vel.y < -wallSlideSpeed) vel.y = -wallSlideSpeed;
                    }

                    // jump cut for variable height
                    if (_jumpCutQueued)
                    {
                        if (vel.y > 0f) vel.y *= jumpCutMultiplier;
                        _jumpCutQueued = false;
                    }

                    _wallSliding = wallSlide;
                }
            }

            // boost pad / dash panel fling
            if (_hasBoost)
            {
                vel = _pendingBoost;
                _hasBoost = false;
                _wallJumpLock = boostLockTime;
                _grounded = false;
                _airDashesLeft = maxAirDashes;
                if (GameManager.Instance != null) GameManager.Instance.Kick(_pendingBoost, 0.7f);
            }

            // spring / bounce-pad launch
            if (_pendingLaunch > 0f)
            {
                vel.y = _pendingLaunch;
                _pendingLaunch = 0f;
                _grounded = false;
                _coyoteTimer = 0f;
            }

            // re-apply platform + conveyor carry exactly once, remember it for next step's strip
            Vector2 carry = _beltVel;
            if (_grounded && _platform != null) carry += _platform.Velocity;
            _rb.linearVelocity = vel + carry;
            _carry = carry;

            EmitSpeedTrail((vel + carry).magnitude, dt);

            if (vel.y > 0.1f) CornerCorrect();

            PostStep(vel);
        }

        private void PostStep(Vector2 vel)
        {
            // landing feedback (and the combo ends) — squash + dust scaled by how hard you hit
            if (_grounded && !_wasGrounded && vel.y <= 0f)
            {
                float impact = Mathf.Clamp01(-_lastAirVelY / 24f);
                if (Sfx.Instance != null) Sfx.Instance.PlayLand();
                if (FX.Instance != null)
                {
                    FX.Instance.LandPuff(FeetPos());
                    if (impact > 0.55f)
                        FX.Instance.Burst(FeetPos(), Palette.GroundCap, 6, 4.5f, 0.14f, 0.4f, -10f, 0.6f);
                }
                SetVisualScale(new Vector3(1.18f + 0.2f * impact, 0.82f - 0.14f * impact, 1f));
                if (impact > 0.8f && GameManager.Instance != null) GameManager.Instance.Shake(0.12f);
                if (GameManager.Instance != null) GameManager.Instance.OnPlayerLandedSafe();   // bank pending coins
                Combo = 0;
            }
            // ease the squash/stretch back toward neutral, plus a little air-stretch
            Vector3 targetScale = Vector3.one;
            if (_slamming)
                targetScale = new Vector3(0.7f, 1.3f, 1f);          // tucked + plummeting
            else if (_sliding)
                targetScale = new Vector3(1.46f, 0.5f, 1f);         // long and flat — a streaking slide
            else if (_ducking)
                targetScale = new Vector3(1.28f, 0.62f, 1f);        // wide crouch
            else if (!_grounded && !_dashing)
            {
                float s = Mathf.Clamp(Mathf.Abs(vel.y) * 0.012f, 0f, 0.18f);
                targetScale = new Vector3(1f - s, 1f + s, 1f);
            }
            float ease = (_ducking || _slamming) ? 26f : 18f;
            _visual.localScale = Vector3.Lerp(_visual.localScale, targetScale, ease * Time.fixedDeltaTime);

            // facing flip (independent of squash/stretch)
            if (_sr != null) _sr.flipX = _facing < 0;
            if (_face != null)
                _face.localPosition = new Vector3(0.12f * _facing, 0.18f, 0f);
            if (_usingArt) AnimateArt(vel);

            // footfall + wall-grind feedback. Movement is the thing you do every frame, so it
            // earns its own audio: footsteps while sprinting, a scuff while grinding a wall.
            bool running = _grounded && !_dashing && Mathf.Abs(vel.x) > 6f;
            bool sliding = _wallSliding;

            // AUDIO — independent of ReduceMotion (that only gates particles)
            if (running)
            {
                _stepTimer -= Time.fixedDeltaTime;
                if (_stepTimer <= 0f)
                {
                    // stride quickens as you speed up (slow lope → quick patter)
                    _stepTimer = Mathf.Lerp(0.33f, 0.19f, Mathf.InverseLerp(6f, maxRunSpeed, Mathf.Abs(vel.x)));
                    if (Sfx.Instance != null) Sfx.Instance.PlayStep();
                }
            }
            else _stepTimer = 0f;
            if (sliding)
            {
                _scrapeTimer -= Time.fixedDeltaTime;
                if (_scrapeTimer <= 0f)
                {
                    _scrapeTimer = 0.13f;
                    if (Sfx.Instance != null) Sfx.Instance.PlayScrape();
                }
            }
            else _scrapeTimer = 0f;

            // DUST — kicked up while sprinting or grinding (a wall grind sheds a touch more debris)
            if (!Settings.ReduceMotion && FX.Instance != null && (running || sliding))
            {
                _dustTimer -= Time.fixedDeltaTime;
                if (_dustTimer <= 0f)
                {
                    _dustTimer = running ? 0.1f : 0.08f;
                    int wd = _wallDir != 0 ? _wallDir : _lastWallDir;
                    Vector3 at = running
                        ? FeetPos()
                        : new Vector3(_col.bounds.center.x + wd * _col.bounds.extents.x, _col.bounds.center.y, 0f);
                    FX.Instance.Burst(at, Palette.GroundCap, sliding ? 2 : 1,
                        sliding ? 2.0f : 1.6f, 0.08f, 0.22f, -5f, running ? 0.2f : 0.5f);
                }
            }

            // powered-up look: pulsing aura + body tint, blinking faster as the timer runs out
            if (_power != PowerUp.None)
            {
                float blink = 1f;
                if (_powerTimer < 3f)
                {
                    float freq = Mathf.Lerp(11f, 4f, Mathf.Clamp01(_powerTimer / 3f));
                    blink = Mathf.Sin(Time.time * freq) > -0.25f ? 1f : 0.35f;
                }
                var pcCol = PowerColor(_power);
                if (_aura != null)
                {
                    _aura.transform.localScale = Vector3.one * (0.62f + 0.08f * Mathf.Sin(Time.time * 6f));
                    var ac = pcCol; ac.a = 0.5f * blink; _aura.color = ac;
                }
                if (_sr != null)
                    _sr.color = _usingArt
                        ? Color.Lerp(Color.white, pcCol, 0.55f * blink)
                        : Color.Lerp(Palette.Player, pcCol, 0.6f * blink);
            }

            _wasGrounded = _grounded;
        }

        // ---- duck / ground-pound -------------------------------------------
        private void ApplyDuckCollider(bool duck)
        {
            if (_col == null) return;
            Vector2 wantSize = duck ? _colSizeDuck : _colSizeStand;
            Vector2 wantOff = duck ? _colOffDuck : Vector2.zero;
            if (_col.size != wantSize) _col.size = wantSize;
            if (_col.offset != wantOff) _col.offset = wantOff;
        }

        /// <summary>True if the FULL standing capsule would fit here (no low ceiling overhead).</summary>
        private bool HasHeadroom()
        {
            var b = _col.bounds;
            float standTop = b.min.y + _colSizeStand.y;     // where the head would be if standing
            float extra = standTop - b.max.y;               // headroom we'd need above the current collider
            if (extra <= 0.04f) return true;
            Vector2 center = new Vector2(b.center.x, b.max.y + extra * 0.5f);
            Vector2 size = new Vector2(_colSizeStand.x * 0.85f, extra - 0.04f);
            return Physics2D.OverlapBox(center, size, 0f, GameLayers.SolidMask) == null;
        }

        private void BeginSlam()
        {
            _slamming = true;
            _ducking = false;
            _sliding = false;
            ApplyDuckCollider(false);
            _airDashesLeft = 0;
            if (Sfx.Instance != null) Sfx.Instance.PlaySlam();      // downward whoosh + rumble
            if (FX.Instance != null) FX.Instance.DashPuff(transform.position, 0);
            SetVisualScale(new Vector3(0.66f, 1.34f, 1f));          // stretch tall as you commit
        }

        private void EndSlam(ref Vector2 vel)
        {
            _slamming = false;
            vel.y = 0f;
            if (Sfx.Instance != null) Sfx.Instance.PlayLand();
            if (GameManager.Instance != null) { GameManager.Instance.Shake(0.22f); GameManager.Instance.Hitstop(0.05f); GameManager.Instance.Kick(Vector2.down, 0.6f); }
            if (FX.Instance != null)
                FX.Instance.Burst(FeetPos(), Palette.GroundCap, 18, 8f, 0.18f, 0.5f, -10f, 1.3f);
            SetVisualScale(new Vector3(1.45f, 0.58f, 1f));          // hard pancake squash on impact
        }

        private void DoJump(ref Vector2 vel)
        {
            vel.y = _jumpVelocity * (_power == PowerUp.HighJump ? highJumpMult : 1f);
            _jumpBufferTimer = 0f;
            _coyoteTimer = 0f;
            _grounded = false;
            _sliding = false;     // a slide-jump keeps your horizontal momentum (vel.x is untouched)
            if (Sfx.Instance != null) Sfx.Instance.PlayJump();
            if (FX.Instance != null) FX.Instance.JumpPuff(FeetPos());
            SetVisualScale(new Vector3(0.8f, 1.2f, 1f));
        }

        private void DoWallJump(ref Vector2 vel)
        {
            int wall = _wallDir != 0 ? _wallDir : _lastWallDir;   // +1 = wall on the right
            int away = -wall;                                     // launch away from it
            vel.x = away * wallJumpX;
            vel.y = wallJumpY;
            _jumpBufferTimer = 0f;
            _coyoteTimer = 0f;
            _wallCoyote = 0f;
            _wallJumpLock = wallJumpLockTime;
            _airDashesLeft = maxAirDashes;     // refresh dash so you can extend the leap
            _facing = away;
            AddCombo();
            if (Sfx.Instance != null) Sfx.Instance.PlayWallJump();
            if (FX.Instance != null) FX.Instance.JumpPuff(FeetPos());
            SetVisualScale(new Vector3(0.82f, 1.18f, 1f));
        }

        private void DoRingJump(ref Vector2 vel)
        {
            vel.y = ringJumpBoost;
            _jumpBufferTimer = 0f;
            _ringTimer = 0f;
            _grounded = false;
            _airDashesLeft = maxAirDashes;
            AddCombo();
            if (Sfx.Instance != null) Sfx.Instance.PlayRing();
            if (FX.Instance != null) FX.Instance.JumpPuff(FeetPos());
            SetVisualScale(new Vector3(0.8f, 1.2f, 1f));
        }

        private void EndDash(ref Vector2 vel)
        {
            _dashing = false;
            _dashCdTimer = dashCooldown;
            _dashWasOnCd = true;
            // keep the dash momentum: it bleeds back to run speed via normal decel, and a dash-jump
            // launches fast across a gap instead of dropping to run speed (speedrun feel).
        }

        private void ProbeWalls()
        {
            var b = _col.bounds;
            // generous probes (Celeste grabs from ~2px away): forgiving wall detection beats
            // demanding flush contact, and it stops seam flicker while sliding
            Vector2 size = new Vector2(0.18f, b.size.y * 0.7f);
            bool left = Physics2D.OverlapBox(new Vector2(b.min.x - 0.09f, b.center.y), size, 0f, GameLayers.WallMask) != null;
            bool right = Physics2D.OverlapBox(new Vector2(b.max.x + 0.09f, b.center.y), size, 0f, GameLayers.WallMask) != null;
            _wallDir = right ? 1 : (left ? -1 : 0);
        }

        /// <summary>If only a top corner clips a ledge while rising, nudge sideways to slip past it.</summary>
        private void CornerCorrect()
        {
            if (!enableCornerCorrect) return;
            var b = _col.bounds;
            float probe = 0.16f;
            float cw = b.size.x * 0.4f;
            Vector2 vs = new Vector2(cw, probe);
            float hy = b.max.y + probe * 0.5f + 0.02f;
            bool left = Physics2D.OverlapBox(new Vector2(b.min.x + cw * 0.5f, hy), vs, 0f, GameLayers.SolidMask) != null;
            bool right = Physics2D.OverlapBox(new Vector2(b.max.x - cw * 0.5f, hy), vs, 0f, GameLayers.SolidMask) != null;
            if (left == right) return;                 // both blocked (real ceiling) or both clear

            int dir = left ? 1 : -1;                   // nudge toward the clear side
            Vector2 dest = new Vector2(b.center.x + dir * cornerCorrectDist, hy);
            if (Physics2D.OverlapBox(dest, new Vector2(b.size.x * 0.85f, probe), 0f, GameLayers.SolidMask) == null)
                _rb.position += new Vector2(dir * cornerCorrectDist, 0f);
        }

        private void ProbeGround()
        {
            Vector2 feet = new Vector2(_col.bounds.center.x, _col.bounds.min.y + 0.02f);
            Vector2 size = new Vector2(_col.bounds.size.x * 0.92f, 0.12f);
            var hit = Physics2D.OverlapBox(feet, size, 0f, GameLayers.GroundMask);

            // use our OWN vertical velocity (strip platform carry) so riding an ascending
            // lift doesn't read as "moving up" and drop the grounded state.
            bool descending = (_rb.linearVelocity.y - _carry.y) <= 0.05f;
            _grounded = hit != null && descending;
            _platform = _grounded && hit != null ? hit.GetComponent<MovingPlatform>() : null;

            if (_grounded && hit != null)
            {
                var belt = hit.GetComponent<Conveyor>();
                _beltVel = belt != null ? belt.Velocity : Vector2.zero;

                // find the precise top of whatever is under the feet so SeatOnGround can plant the
                // collider ON it. The overlap probe above reaches ~0.08 past the feet, which would
                // otherwise let the player come to rest hovering that far above the surface.
                var down = Physics2D.BoxCast(_col.bounds.center,
                    new Vector2(_col.bounds.size.x * 0.9f, 0.04f), 0f, Vector2.down,
                    _col.bounds.extents.y + 0.2f, GameLayers.GroundMask);
                _hasGroundSurface = down.collider != null;
                if (_hasGroundSurface) _groundSurfaceY = down.point.y;
            }
            else { _beltVel = Vector2.zero; _hasGroundSurface = false; }
        }

        /// <summary>Snap the collider's bottom onto the detected surface, so the player rests
        /// touching the ground instead of hovering up to the ground-probe's height above it.
        /// Only nudges within a small window (the hover gap) — never teleports across a ledge.</summary>
        private void SeatOnGround()
        {
            if (!_hasGroundSurface) return;
            float dy = _groundSurfaceY - _col.bounds.min.y;     // <0 drop to the surface, >0 lift onto it
            if (dy >= -0.12f && dy <= 0.06f && Mathf.Abs(dy) > 0.0005f)
                _rb.position += new Vector2(0f, dy);
        }

        // a glowing motion-smear behind the ball whenever it's moving FAST — boost, dash, ring,
        // spring, a downhill carry. Cadence + alpha scale with speed, so the trail intensifies the
        // faster you go and vanishes at walking pace. This is the momentum made visible.
        private void EmitSpeedTrail(float speed, float dt)
        {
            if (FX.Instance == null || _sr == null || _sr.sprite == null) return;
            float over = Mathf.Clamp01((speed - maxRunSpeed * 1.15f) / Mathf.Max(1f, maxRunSpeed));
            if (over <= 0f) { _afterTimer = 0f; return; }
            _afterTimer -= dt;
            if (_afterTimer > 0f) return;
            _afterTimer = Mathf.Lerp(0.04f, 0.016f, over);          // denser the faster you move
            var trail = Palette.Player; trail.a = 0.18f + 0.32f * over;
            FX.Instance.Afterimage(_sr.transform.position, _sr.sprite, trail, _sr.flipX,
                                   _sr.transform.lossyScale.y, 0.22f);
        }

        private void SetVisualScale(Vector3 s)
        {
            if (_visual != null) _visual.localScale = s;
        }

        private Vector3 FeetPos()
        {
            return new Vector3(_col.bounds.center.x, _col.bounds.min.y, 0f);
        }

        private void RecomputeGravity()
        {
            _baseGravity = (2f * jumpHeight) / (timeToApex * timeToApex);
            _jumpVelocity = _baseGravity * timeToApex;
        }

        // ---- self-construction --------------------------------------------
        private void BuildSelf()
        {
            gameObject.layer = GameLayers.Player;

            _rb = GetComponent<Rigidbody2D>();
            _rb.gravityScale = 0f;
            _rb.freezeRotation = true;
            _rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
            _rb.interpolation = RigidbodyInterpolation2D.Interpolate;
            _rb.sleepMode = RigidbodySleepMode2D.NeverSleep;

            _col = gameObject.GetComponent<CapsuleCollider2D>();
            if (_col == null) _col = gameObject.AddComponent<CapsuleCollider2D>();
            _col.size = new Vector2(0.78f, 1.08f);
            _col.direction = CapsuleDirection2D.Vertical;
            // duck preset: keep the feet planted (offset down by half the height lost) so a crouch
            // slips under low overhangs without the player popping up or sinking through the floor.
            // CRITICAL: a vertical CapsuleCollider2D needs height >= width, else Unity clamps it
            // up to a circle of the WIDTH — so a (0.78, 0.6) duck silently becomes a 0.78-tall
            // circle (the hitbox barely shrinks AND the offset drives it into the floor, popping
            // the player up). Keep width <= height so the 0.6 height is real.
            _colSizeStand = _col.size;
            _colSizeDuck = new Vector2(0.6f, 0.6f);
            _colOffDuck = new Vector2(0f, -(_colSizeStand.y - _colSizeDuck.y) * 0.5f);
            _col.sharedMaterial = new PhysicsMaterial2D("PlayerFrictionless")
            {
                friction = 0f,
                bounciness = 0f
            };

            // Visual child so we can squash/stretch without scaling the collider.
            var v = new GameObject("Visual");
            _visual = v.transform;
            _visual.SetParent(transform, false);

            PlayerArt.EnsureLoaded();
            if (PlayerArt.Active)
            {
                _idleArt = PlayerArt.Idle; _walkFrames = PlayerArt.Run;
                _jumpArt = PlayerArt.Jump; _fallArt = PlayerArt.Fall;
                _feetMode = true;   // frames are trimmed + planted by the feet
            }
            else { _idleArt = LevelArt.Get("player"); _walkFrames = LevelArt.Walk; }

            if (_idleArt != null)
            {
                // imported hand-drawn character (idle / run / jump / fall)
                _usingArt = true;
                var artGo = new GameObject("ArtBody");
                artGo.transform.SetParent(_visual, false);
                _artBody = artGo.transform;
                _sr = artGo.AddComponent<SpriteRenderer>();
                _sr.sortingOrder = 10;
                if (_feetMode)
                {
                    _artBody.localPosition = new Vector3(0f, -0.55f, 0f);       // feet at the collider bottom
                    SetArtFrame(_idleArt);                                       // normalised to a fixed height
                }
                else
                {
                    float fit = 2.0f / Mathf.Max(0.01f, _idleArt.bounds.size.y);
                    _artBody.localScale = new Vector3(fit, fit, 1f);
                    _artBody.localPosition = new Vector3(0f, 0.42f, 0f);
                    _sr.sprite = _idleArt;
                }
            }
            else
            {
                _sr = v.AddComponent<SpriteRenderer>();
                _sr.sprite = TextureFactory.RoundedBox(16, 22, 7, 20);
                _sr.color = Palette.Player;
                _sr.sortingOrder = 10;

                // dark outline so the player always reads against busy scenery
                var outline = new GameObject("Outline");
                outline.transform.SetParent(_visual, false);
                outline.transform.localScale = Vector3.one * 1.18f;
                var osr = outline.AddComponent<SpriteRenderer>();
                osr.sprite = TextureFactory.RoundedBox(16, 22, 7, 20);
                osr.color = Palette.Hex("#141019");
                osr.sortingOrder = 9;

                // a little face highlight
                var face = new GameObject("Face");
                _face = face.transform;
                _face.SetParent(_visual, false);
                _face.localPosition = new Vector3(0.12f, 0.18f, 0f);
                var fsr = face.AddComponent<SpriteRenderer>();
                fsr.sprite = TextureFactory.Circle(6, 22);
                fsr.color = Palette.PlayerHi;
                fsr.sortingOrder = 11;
            }

            // power-up aura (behind the body), shown only while a power-up is active
            var auraGo = new GameObject("Aura");
            auraGo.transform.SetParent(_visual, false);
            auraGo.transform.localPosition = new Vector3(0f, 0.3f, 0f);
            auraGo.transform.localScale = Vector3.one * 0.62f;
            _aura = auraGo.AddComponent<SpriteRenderer>();
            _aura.sprite = TextureFactory.RadialGlow(64);
            _aura.color = Color.clear;
            _aura.sortingOrder = 8;
            _aura.enabled = false;
        }

        private void SetArtFrame(Sprite s)
        {
            if (s == null || _sr == null) return;
            _sr.sprite = s;
            if (_feetMode && _artBody != null)
            {
                float sc = 1.75f / Mathf.Max(0.01f, s.bounds.size.y);   // every frame the same height
                _artBody.localScale = new Vector3(sc, sc, 1f);
            }
        }

        private void AnimateArt(Vector2 vel)
        {
            if (_sr == null) return;
            float speed = Mathf.Abs(vel.x);

            if (!_grounded)
            {
                if (vel.y > 0.5f && _jumpArt != null) SetArtFrame(_jumpArt);
                else if (_fallArt != null) SetArtFrame(_fallArt);
                else SetArtFrame(_idleArt);
                return;
            }

            if (speed > 1.2f && _walkFrames != null && _walkFrames.Length > 0)
            {
                _animTime += Time.fixedDeltaTime * Mathf.Clamp(speed * 0.62f, 3.5f, 8.5f);
                SetArtFrame(_walkFrames[Mathf.FloorToInt(_animTime) % _walkFrames.Length]);
            }
            else
            {
                SetArtFrame(_idleArt);
                if (speed < 0.2f) _animTime = 0f;
            }
        }
    }
}
