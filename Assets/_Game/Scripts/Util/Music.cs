using UnityEngine;

namespace Velocity
{
    /// <summary>
    /// Generative, per-painting music engine — no audio assets, no loops. Instead of a baked
    /// clip, the score is SYNTHESIZED live in OnAudioFilterRead: a tiny sequencer walks a
    /// per-theme scale over a slow chord progression, so phrases vary forever and nothing
    /// repeats verbatim (fixed short loops are the #1 cause of music fatigue in die-and-retry
    /// games). Each painting gets its own mode, tempo, meter and timbre; the arrangement is
    /// ADAPTIVE — a drone bed is always present, the melody swells with the player's speed
    /// (flow is rewarded with music), and a pulse layer + high shimmer fade in as the player
    /// pushes deeper into the level, mirroring the levels' four-act difficulty curves. Music
    /// never restarts on death (Meat Boy rule: the track keeps rolling so retries stay calm);
    /// instead it briefly ducks and the intensity layers naturally fall away with progress.
    /// All sequencing happens on the audio thread: no Unity API calls inside the callback,
    /// main-thread state crosses over via volatile floats and atomic reference swaps.
    /// </summary>
    public class Music : MonoBehaviour
    {
        public static Music Instance { get; private set; }

        // ---- a painting's musical identity --------------------------------
        private class Mood
        {
            public string Name;
            public float Root;            // melody-octave tonic (Hz)
            public int[] Scale;           // semitone offsets within the octave
            public int[][] Chords;        // progression: semitone sets, one per 4 bars
            public float Bpm;
            public int BeatsPerBar = 4;   // 3 = waltz
            public int StepsPerBeat = 2;
            public float RestProb;        // sparseness — silence is what keeps it un-annoying
            public int MelodyWave;        // 0 sine, 1 triangle, 2 saw-soft
            public float MelodyDecay;     // amplitude decay per second
            public float MelodyGain;
            public float[] Drone;         // additive bed partials (Hz)
            public float DroneGain;
            public float PulseGain;       // adaptive bass layer (act 3 of the level)
            public int PulseEvery = 2;    // pulse hits every N steps
            public float ShimmerGain;     // adaptive high sparkle (the climax)
            public float DelaySec, DelayFb, DelayMix;
        }

        private static Mood Night() => new Mood
        {   // STARRY NIGHT — drifting, nocturnal: A-minor pentatonic music box over a deep bed
            Name = "night", Root = 440f,
            Scale = new[] { 0, 3, 5, 7, 10 },
            Chords = new[] { new[] { 0, 3, 7 }, new[] { -4, 0, 5 }, new[] { -2, 3, 7 }, new[] { 0, 3, 7 } },
            Bpm = 66, RestProb = 0.38f,
            MelodyWave = 1, MelodyDecay = 3.2f, MelodyGain = 0.075f,
            Drone = new[] { 55f, 110f, 164.81f }, DroneGain = 0.10f,
            PulseGain = 0.055f, PulseEvery = 4, ShimmerGain = 0.035f,
            DelaySec = 0.42f, DelayFb = 0.38f, DelayMix = 0.30f,
        };

        private static Mood Tide() => new Mood
        {   // THE GREAT WAVE — heaving and vast: hirajoshi pentatonic plucks (koto-like)
            // over a deep surging swell; the pulse rolls in twos like breakers
            Name = "tide", Root = 293.66f,
            Scale = new[] { 0, 2, 3, 7, 8 },
            Chords = new[] { new[] { 0, 7, 12 }, new[] { -4, 3, 8 }, new[] { 0, 7, 12 }, new[] { -2, 5, 10 } },
            Bpm = 72, RestProb = 0.42f,
            MelodyWave = 2, MelodyDecay = 5f, MelodyGain = 0.06f,
            Drone = new[] { 36.71f, 73.42f, 110f }, DroneGain = 0.12f,
            PulseGain = 0.075f, PulseEvery = 2, ShimmerGain = 0.04f,
            DelaySec = 0.46f, DelayFb = 0.4f, DelayMix = 0.3f,
        };

        private static Mood Sacred() => new Mood
        {   // SACRED (fresco / chiaroscuro) — vast and reverent: slow Mixolydian organum, open fifths
            Name = "sacred", Root = 293.66f,
            Scale = new[] { 0, 2, 4, 5, 7, 9, 10 },
            Chords = new[] { new[] { 0, 7, 12 }, new[] { -2, 5, 10 }, new[] { 0, 7, 12 }, new[] { 5, 12, 17 } },
            Bpm = 52, RestProb = 0.55f, StepsPerBeat = 1,
            MelodyWave = 0, MelodyDecay = 1.2f, MelodyGain = 0.06f,
            Drone = new[] { 73.42f, 110f, 146.83f, 220f }, DroneGain = 0.115f,
            PulseGain = 0.04f, PulseEvery = 4, ShimmerGain = 0.045f,
            DelaySec = 0.55f, DelayFb = 0.42f, DelayMix = 0.34f,
        };

        private static Mood Waltz() => new Mood
        {   // THE SWING — a rococo garden in 3/4: bright harpsichord plucks, oom-pah-pah
            Name = "waltz", Root = 349.23f,
            Scale = new[] { 0, 2, 4, 5, 7, 9, 11 },
            Chords = new[] { new[] { 0, 4, 7 }, new[] { 5, 9, 12 }, new[] { 7, 11, 14 }, new[] { 0, 4, 7 } },
            Bpm = 104, BeatsPerBar = 3, RestProb = 0.22f,
            MelodyWave = 2, MelodyDecay = 6f, MelodyGain = 0.065f,
            Drone = new[] { 87.31f, 174.61f }, DroneGain = 0.06f,
            PulseGain = 0.07f, PulseEvery = 3, ShimmerGain = 0.03f,
            DelaySec = 0.19f, DelayFb = 0.25f, DelayMix = 0.16f,
        };

        private static Mood Anxious() => new Mood
        {   // THE SCREAM — urgency: Phrygian ostinato, driving low pulse, uneasy high wisps
            Name = "anxious", Root = 329.63f,
            Scale = new[] { 0, 1, 3, 5, 7, 8, 10 },
            Chords = new[] { new[] { 0, 3, 7 }, new[] { 1, 5, 8 }, new[] { 0, 3, 7 }, new[] { -2, 1, 5 } },
            Bpm = 140, RestProb = 0.5f,
            MelodyWave = 1, MelodyDecay = 4.5f, MelodyGain = 0.055f,
            // the second partial sits ~25 cents sharp — the slow audible beating IS the dread
            Drone = new[] { 41.2f, 83.6f, 123.47f }, DroneGain = 0.10f,
            PulseGain = 0.10f, PulseEvery = 2, ShimmerGain = 0.04f,
            DelaySec = 0.23f, DelayFb = 0.3f, DelayMix = 0.2f,
        };

        private static Mood Gallery() => new Mood
        {   // MENUS / OVERWORLD — the quiet museum: sparse piano-like notes, lots of air
            Name = "gallery", Root = 523.25f,
            Scale = new[] { 0, 2, 4, 7, 9 },
            Chords = new[] { new[] { 0, 4, 7 }, new[] { -3, 0, 4 }, new[] { 2, 5, 9 }, new[] { 0, 4, 7 } },
            Bpm = 56, RestProb = 0.62f,
            MelodyWave = 1, MelodyDecay = 2.2f, MelodyGain = 0.05f,
            Drone = new[] { 65.41f, 130.81f }, DroneGain = 0.05f,
            PulseGain = 0f, PulseEvery = 4, ShimmerGain = 0.02f,
            DelaySec = 0.5f, DelayFb = 0.35f, DelayMix = 0.25f,
        };

        // ---- main-thread state ---------------------------------------------
        private AudioSource _src;
        private int _sampleRate;
        private volatile float _settingsVol;     // cached for the audio thread
        private volatile float _progress;        // 0..1 across the level → pulse/shimmer layers
        private volatile float _speed;           // 0..1 player speed → melody swell
        private volatile float _duckTarget = 1f, _duckRecover = 1f;
        private volatile Mood _pending;          // swapped in atomically by the audio thread

        public static void Bootstrap()
        {
            if (Instance != null) return;
            var go = new GameObject("Music");
            DontDestroyOnLoad(go);
            Instance = go.AddComponent<Music>();
        }

        /// <summary>Pick the score for a painting. Same mood → no interruption.</summary>
        public void SetTheme(BackdropStyle style)
        {
            switch (style)
            {
                case BackdropStyle.StarryNight: Queue(Night()); break;
                case BackdropStyle.Fresco:      Queue(Sacred()); break;
                case BackdropStyle.Garden:      Queue(Waltz()); break;
                case BackdropStyle.Scream:      Queue(Anxious()); break;
                case BackdropStyle.Chiaroscuro: Queue(Sacred()); break;
                case BackdropStyle.Wave:        Queue(Tide()); break;
                default:                        Queue(Night()); break;
            }
        }

        public void SetGallery() => Queue(Gallery());

        /// <summary>Feed the adaptive layers: progress through the level and current speed.</summary>
        public void SetIntensity(float progress01, float speed01)
        {
            _progress = Mathf.Clamp01(progress01);
            _speed = Mathf.Clamp01(speed01);
        }

        /// <summary>Momentary sidechain dip (death sting, finish fanfare) — depth 0..1.</summary>
        public void Duck(float depth, float seconds)
        {
            _duckTarget = Mathf.Clamp01(1f - depth);
            _duckRecover = 1f / Mathf.Max(0.1f, seconds);
        }

        private void Queue(Mood m)
        {
            if (_mood != null && _pending == null && _mood.Name == m.Name) return;
            if (_pending != null && _pending.Name == m.Name) return;
            _pending = m;          // audio thread crossfades to it
            if (Sfx.Instance != null) Sfx.Instance.Tune(m.Root, m.Scale);   // pickups join the key
        }

        private void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
            _sampleRate = AudioSettings.outputSampleRate;
            _delay = new float[_sampleRate];                 // 1s max delay line
            _mood = Gallery();
            ApplyMood(_mood);

            // a silent looping clip keeps the source running; OnAudioFilterRead writes the score
            var silent = AudioClip.Create("score", _sampleRate, 1, _sampleRate, false);
            silent.SetData(new float[_sampleRate], 0);
            _src = gameObject.AddComponent<AudioSource>();
            _src.clip = silent;
            _src.loop = true;
            _src.playOnAwake = false;
            _src.spatialBlend = 0f;
            _src.Play();
        }

        private void Update() => _settingsVol = Settings.MusicVolume * Settings.MasterVolume;

        // ================= audio thread from here down =======================
        private volatile Mood _mood;
        private System.Random _rng = new System.Random(12345);

        private double _stepClock;           // samples until the next sequencer step
        private double _samplesPerStep;
        private int _step, _bar, _chordIdx;
        private int _degree;                 // melody position in the scale (random walk)
        private float _fade = 1f, _fadeVel;  // crossfade envelope for theme switches
        private float _duck = 1f;
        private float _melodySmooth, _pulseSmooth, _shimmerSmooth;

        private struct Voice { public double Phase, Hz; public float Amp, Decay; public int Wave; }
        private readonly Voice[] _voices = new Voice[24];
        private int _nextVoice;

        private float[] _delay;
        private int _delayWrite, _delaySamples;
        private double _dronePhase1, _dronePhase2, _dronePhase3, _dronePhase4;
        private double _shimmerPhase1, _shimmerPhase2;

        private void ApplyMood(Mood m)
        {
            _samplesPerStep = _sampleRate * 60.0 / (m.Bpm * m.StepsPerBeat);
            _delaySamples = (int)System.Math.Min(_delay.Length - 1, m.DelaySec * _sampleRate);
            _step = 0; _bar = 0; _chordIdx = 0;
            _degree = m.Scale.Length;        // start mid-register (octave above root center)
        }

        private void Trigger(double hz, float amp, float decay, int wave)
        {
            _voices[_nextVoice] = new Voice { Phase = 0, Hz = hz, Amp = amp, Decay = decay, Wave = wave };
            _nextVoice = (_nextVoice + 1) % _voices.Length;
        }

        private static float Note(int wave, double phase)
        {
            double p = phase % 1.0;
            switch (wave)
            {
                case 1:  return (float)(p < 0.5 ? p * 4.0 - 1.0 : 3.0 - p * 4.0);            // triangle
                case 2:  double s = p * 2.0 - 1.0; return (float)(s * 0.6 + System.Math.Sin(phase * 2.0 * System.Math.PI) * 0.4); // soft saw
                default: return (float)System.Math.Sin(phase * 2.0 * System.Math.PI);
            }
        }

        private void Step(Mood m)
        {
            int stepsPerBar = m.BeatsPerBar * m.StepsPerBeat;
            bool barStart = _step == 0;
            bool beatStart = _step % m.StepsPerBeat == 0;

            if (barStart)
            {
                _bar++;
                if (_bar % 4 == 0) _chordIdx = (_chordIdx + 1) % m.Chords.Length;
            }

            // every 8th bar the melody takes a breath — phrases need air to stay welcome
            bool breath = (_bar % 8) == 7;
            var chord = m.Chords[_chordIdx];

            // pulse layer: low chord-root heartbeat, adaptive gain set in the mixer
            if (_pulseSmooth > 0.003f && _step % m.PulseEvery == 0)
            {
                double bass = m.Root * 0.25 * System.Math.Pow(2.0, chord[0] / 12.0);
                Trigger(bass, _pulseSmooth, 7f, 0);
            }

            if (breath || _melodySmooth < 0.004f) { _step = (_step + 1) % stepsPerBar; return; }

            // melody: a lazy random walk over the scale, snapped toward chord tones on beats
            if (_rng.NextDouble() > m.RestProb)
            {
                int walk = _rng.Next(0, 6);
                _degree += walk == 0 ? -2 : walk <= 2 ? -1 : walk <= 4 ? 1 : 2;
                if (_rng.NextDouble() < 0.07) _degree += _rng.NextDouble() < 0.5 ? -m.Scale.Length : m.Scale.Length;
                _degree = System.Math.Max(0, System.Math.Min(m.Scale.Length * 3 - 1, _degree));

                int oct = _degree / m.Scale.Length, idx = _degree % m.Scale.Length;
                int semis = m.Scale[idx] + 12 * (oct - 1);
                if (beatStart)
                {   // pull strong beats onto the nearest chord tone so the walk stays harmonic
                    int best = semis, dist = 99;
                    foreach (var c in chord)
                        for (int o = -12; o <= 12; o += 12)
                        {
                            int cand = c + o, d = System.Math.Abs(cand - semis);
                            if (d < dist) { dist = d; best = cand; }
                        }
                    semis = best;
                }
                double hz = m.Root * System.Math.Pow(2.0, semis / 12.0);
                Trigger(hz, _melodySmooth, m.MelodyDecay, m.MelodyWave);
                // the sacred mood sings in parallel fifths — instant organum
                if (m.Name == "sacred") Trigger(hz * 1.4983, _melodySmooth * 0.6f, m.MelodyDecay, 0);
            }

            _step = (_step + 1) % stepsPerBar;
        }

        private void OnAudioFilterRead(float[] data, int channels)
        {
            var m = _mood;
            if (m == null) return;

            // theme crossfade: fade out, swap, fade back in (audio thread owns the swap)
            if (_pending != null && _fadeVel >= 0f) _fadeVel = -1.2f;

            float vol = _settingsVol;
            float dt = 1f / _sampleRate;

            // adaptive layer targets (smoothed per buffer; inputs are volatile floats)
            float melodyT = m.MelodyGain * (0.45f + 0.55f * _speed);
            float pulseT = m.PulseGain * Mathf.Clamp01((_progress - 0.45f) / 0.25f);
            float shimmerT = m.ShimmerGain * Mathf.Clamp01((_progress - 0.7f) / 0.2f);

            for (int i = 0; i < data.Length; i += channels)
            {
                // envelopes
                _fade += _fadeVel * dt;
                if (_fade <= 0f && _pending != null)
                {
                    _mood = m = _pending; _pending = null;
                    ApplyMood(m);
                    _fade = 0f; _fadeVel = 0.8f;
                }
                if (_fade >= 1f) { _fade = 1f; if (_fadeVel > 0f) _fadeVel = 0f; }
                _duck = _duck < _duckTarget ? System.Math.Min(_duckTarget, _duck + _duckRecover * dt)
                                            : System.Math.Max(_duckTarget, _duck - 4f * dt);
                if (_duck >= _duckTarget) _duckTarget = 1f;     // bounce back up after the dip

                // slow Celeste-style layer glides (~2-4s) — intensity creeps, never jumps
                _melodySmooth += (melodyT - _melodySmooth) * dt * 0.6f;
                _pulseSmooth += (pulseT - _pulseSmooth) * dt * 0.5f;
                _shimmerSmooth += (shimmerT - _shimmerSmooth) * dt * 0.5f;

                // sequencer
                _stepClock -= 1.0;
                if (_stepClock <= 0.0) { _stepClock += _samplesPerStep; Step(m); }

                // drone bed (always on — the painting's tonal floor)
                float s = 0f;
                if (m.Drone.Length > 0) { _dronePhase1 += m.Drone[0] * dt; s += Note(0, _dronePhase1) * m.DroneGain; }
                if (m.Drone.Length > 1) { _dronePhase2 += m.Drone[1] * dt; s += Note(0, _dronePhase2) * m.DroneGain * 0.5f; }
                if (m.Drone.Length > 2) { _dronePhase3 += m.Drone[2] * dt; s += Note(0, _dronePhase3) * m.DroneGain * 0.3f; }
                if (m.Drone.Length > 3) { _dronePhase4 += m.Drone[3] * dt; s += Note(0, _dronePhase4) * m.DroneGain * 0.22f; }

                // shimmer: two detuned high sines riding the current chord root (climax air)
                if (_shimmerSmooth > 0.002f)
                {
                    double sh = m.Root * 4.0 * System.Math.Pow(2.0, m.Chords[_chordIdx][0] / 12.0);
                    _shimmerPhase1 += sh * dt; _shimmerPhase2 += (sh + 3.0) * dt;
                    s += (Note(0, _shimmerPhase1) + Note(0, _shimmerPhase2)) * 0.5f * _shimmerSmooth;
                }

                // voices
                for (int v = 0; v < _voices.Length; v++)
                {
                    if (_voices[v].Amp <= 0.0005f) continue;
                    _voices[v].Phase += _voices[v].Hz * dt;
                    s += Note(_voices[v].Wave, _voices[v].Phase) * _voices[v].Amp;
                    _voices[v].Amp *= 1f - _voices[v].Decay * dt;
                }

                // feedback delay — cheap space, scaled per mood
                float echo = _delay[(_delayWrite - _delaySamples + _delay.Length) % _delay.Length];
                _delay[_delayWrite] = s + echo * m.DelayFb;
                _delayWrite = (_delayWrite + 1) % _delay.Length;
                s += echo * m.DelayMix;

                float outS = Mathf.Clamp(s, -0.95f, 0.95f) * vol * _fade * _duck;
                for (int c = 0; c < channels; c++) data[i + c] = outS;
            }
        }
    }
}
