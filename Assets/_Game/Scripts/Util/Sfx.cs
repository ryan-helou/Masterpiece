using System.Collections.Generic;
using UnityEngine;

namespace Velocity
{
    /// <summary>
    /// Tiny procedural sound engine. Generates all SFX at runtime (no audio assets) and plays
    /// them through a small pool of AudioSources so every shot gets a fresh random pitch —
    /// hearing the exact same samples hundreds of times per session is what makes game SFX
    /// grating, so nothing here ever plays back twice identically. Every mechanic gets its own
    /// distinct earcon (spring ≠ jump ≠ note-block), timbres lean on triangle/sine instead of
    /// raw squares so frequent sounds stay soft, and a per-sound cooldown stops doubled hits
    /// from stacking into a louder spike.
    /// </summary>
    public class Sfx : MonoBehaviour
    {
        public static Sfx Instance { get; private set; }

        private const int SampleRate = 44100;
        private const int Voices = 6;            // round-robin pool → per-shot pitch
        private const float PitchVar = 0.03f;    // ±3% (~±50 cents) — audible variety, same note
        private const float MinInterval = 0.03f; // anti-stack guard per sound

        private AudioSource[] _pool;
        private int _next;
        private readonly Dictionary<AudioClip, float> _lastPlay = new Dictionary<AudioClip, float>();

        private AudioClip _jump, _dash, _land, _death, _finish, _click, _record;
        private AudioClip _coin, _stomp, _walljump, _powerup, _dashReady;
        private AudioClip _spring, _note, _boost, _orb, _warp, _crumble, _creak, _slam, _ring;
        private AudioClip _tick, _bank, _step, _scrape;
        private AudioClip[] _combo;

        public static void Bootstrap()
        {
            if (Instance != null) return;
            var go = new GameObject("Sfx");
            DontDestroyOnLoad(go);
            Instance = go.AddComponent<Sfx>();
        }

        private void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;

            _pool = new AudioSource[Voices];
            for (int i = 0; i < Voices; i++)
            {
                _pool[i] = gameObject.AddComponent<AudioSource>();
                _pool[i].playOnAwake = false;
                _pool[i].spatialBlend = 0f;
            }

            // movement (heard constantly → soft triangle timbres, modest gains)
            _jump     = Sweep("jump", 500f, 840f, 0.10f, 0.30f, Wave.Triangle);
            _land     = Sweep("land", 180f, 110f, 0.07f, 0.28f, Wave.Triangle);
            _dash     = Sweep("dash", 240f, 70f, 0.16f, 0.32f, Wave.Saw);
            _walljump = Sweep("walljump", 320f, 660f, 0.10f, 0.28f, Wave.Triangle);
            // footfall: a soft low tap (heard most of all → very quiet, short, no sharp attack)
            _step     = Sweep("step", 200f, 132f, 0.05f, 0.16f, Wave.Triangle);
            // wall grind: a brief filtered-noise scuff, soft so a long slide never grates
            _scrape   = Noise("scrape", 0.09f, 0.16f);
            _slam     = Mix("slam",
                            Sweep01(420f, 80f, 0.20f, Wave.Saw),
                            Noise01(0.20f, 0.35f));

            // mechanics — every device sounds like itself
            _spring = Sweep("spring", 280f, 920f, 0.16f, 0.38f, Wave.Sine);
            _note   = Arp("note", new[] { 659.25f, 1318.5f }, 0.12f, 0.34f, Wave.Triangle);
            _boost  = Mix("boost",
                          Sweep01(160f, 540f, 0.22f, Wave.Saw),
                          Noise01(0.22f, 0.4f));
            _orb    = Detune("orb", 1318.5f, 6f, 0.26f, 0.28f);
            _warp   = Sweep("warp", 880f, 196f, 0.22f, 0.30f, Wave.Sine);
            _crumble = Noise("crumble", 0.12f, 0.35f);
            _creak  = Sweep("creak", 95f, 70f, 0.24f, 0.25f, Wave.Saw);
            _ring   = Detune("ring", 987.77f, 4f, 0.22f, 0.30f);
            _tick   = Sweep("tick", 1480f, 1480f, 0.035f, 0.12f, Wave.Sine);   // soft hazard-rhythm tick
            _bank   = Arp("bank", new[] { 1046.5f, 1568f }, 0.16f, 0.22f, Wave.Triangle); // coins banked

            // pickups / progress
            _coin      = Arp("coin", new[] { 987.77f, 1318.5f }, 0.14f, 0.36f, Wave.Triangle);
            _stomp     = Sweep("stomp", 200f, 80f, 0.10f, 0.45f, Wave.Square);
            _powerup   = Arp("powerup", new[] { 659.25f, 880f, 1174.66f, 1567.98f }, 0.32f, 0.38f, Wave.Triangle);
            _dashReady = Sweep("dashready", 1320f, 1760f, 0.05f, 0.16f, Wave.Sine);

            // events
            _death  = Mix("death",
                          Sweep01(320f, 60f, 0.42f, Wave.Saw),
                          Noise01(0.10f, 0.3f));
            _finish = Arp("finish", new[] { 523.25f, 659.25f, 783.99f, 1046.5f }, 0.40f, 0.40f, Wave.Triangle);
            _record = Arp("record", new[] { 783.99f, 1046.5f, 1318.5f }, 0.45f, 0.45f, Wave.Triangle);
            _click  = Sweep("click", 660f, 660f, 0.04f, 0.18f, Wave.Sine);

            _combo = new AudioClip[8];
            for (int i = 0; i < _combo.Length; i++)
            {
                float f = 523.25f * Mathf.Pow(2f, i / 12f);     // rises a semitone per combo
                _combo[i] = Sweep("combo" + i, f, f * 1.5f, 0.09f, 0.30f, Wave.Triangle);
            }
        }

        public void PlayJump()      => Play(_jump);
        public void PlayDash()      => Play(_dash);
        public void PlayLand()      => Play(_land);
        public void PlayDeath()     => Play(_death, 1f, 0.01f);  // the death sting stays recognizable
        public void PlayFinish()    => Play(_finish, 1f, 0f);
        public void PlayClick()     => Play(_click, 1f, 0.02f);
        public void PlayRecord()    => Play(_record, 1f, 0f);
        public void PlayCoin()      => Play(_coin);
        public void PlayStomp()     => Play(_stomp);
        public void PlayWallJump()  => Play(_walljump);
        public void PlayPowerUp()   => Play(_powerup);
        public void PlayDashReady() => Play(_dashReady);
        public void PlaySpring()    => Play(_spring);
        public void PlayNote()      => Play(_note, 1f, 0.06f);   // extra variance — bounced in runs
        public void PlayBoost()     => Play(_boost);
        public void PlayOrb()       => Play(_orb);
        public void PlayWarp()      => Play(_warp);
        public void PlayCrumble()   => Play(_crumble, 0.8f, 0.08f);
        public void PlayCreak()     => Play(_creak, 0.8f, 0.06f);
        public void PlaySlam()      => Play(_slam);
        public void PlayRing()      => Play(_ring);
        public void PlayTick()      => Play(_tick, 0.7f, 0.015f);
        public void PlayBank()      => Play(_bank, 1f, 0.02f);
        public void PlayStep()      => Play(_step, 0.34f, 0.11f);   // quiet, wide pitch jitter = organic gait
        public void PlayScrape()    => Play(_scrape, 0.45f, 0.06f);
        public void PlayCombo(int n) { if (_combo != null) Play(_combo[Mathf.Clamp(n, 0, _combo.Length - 1)]); }

        /// <summary>Retune the musical SFX (coin, note-block, ring, orb, fanfares) to the score
        /// currently playing, so every pickup lands INSIDE the level's music — reward sounds
        /// become part of the soundtrack instead of clashing with it. Called by Music on theme
        /// switch with the new root and scale.</summary>
        public void Tune(float root, int[] scale)
        {
            float N(int degree, int octave)
            {
                int idx = ((degree % scale.Length) + scale.Length) % scale.Length;
                int oct = octave + degree / scale.Length;
                return root * Mathf.Pow(2f, (scale[idx] + 12 * oct) / 12f);
            }
            _coin    = Arp("coin", new[] { N(4, 1), N(1, 2) }, 0.14f, 0.36f, Wave.Triangle);
            _note    = Arp("note", new[] { N(0, 1), N(0, 2) }, 0.12f, 0.34f, Wave.Triangle);
            _ring    = Detune("ring", N(4, 1), 4f, 0.22f, 0.30f);
            _orb     = Detune("orb", N(2, 2), 6f, 0.26f, 0.28f);
            _powerup = Arp("powerup", new[] { N(0, 1), N(2, 1), N(4, 1), N(0, 2) }, 0.32f, 0.38f, Wave.Triangle);
            _finish  = Arp("finish", new[] { N(0, 0), N(2, 0), N(4, 0), N(0, 1) }, 0.40f, 0.40f, Wave.Triangle);
            _record  = Arp("record", new[] { N(4, 0), N(0, 1), N(2, 1) }, 0.45f, 0.45f, Wave.Triangle);
        }

        private void Play(AudioClip clip, float gain = 1f, float pitchVar = PitchVar)
        {
            if (clip == null) return;
            if (_lastPlay.TryGetValue(clip, out float last) && Time.unscaledTime - last < MinInterval) return;
            _lastPlay[clip] = Time.unscaledTime;

            var src = _pool[_next];
            _next = (_next + 1) % Voices;
            src.pitch = 1f + Random.Range(-pitchVar, pitchVar);
            src.PlayOneShot(clip, gain * Settings.SfxVolume * Settings.MasterVolume);
        }

        // ---- synthesis -------------------------------------------------------

        private enum Wave { Sine, Square, Saw, Triangle }

        private static float Osc(Wave wave, float phase)
        {
            switch (wave)
            {
                case Wave.Square:   return Mathf.Sin(phase) >= 0 ? 1f : -1f;
                case Wave.Saw:      return Mathf.Repeat(phase / (2f * Mathf.PI), 1f) * 2f - 1f;
                case Wave.Triangle: return Mathf.PingPong(phase / Mathf.PI, 1f) * 2f - 1f;
                default:            return Mathf.Sin(phase);
            }
        }

        private static float[] Sweep01(float fromHz, float toHz, float dur, Wave wave)
        {
            int n = Mathf.CeilToInt(SampleRate * dur);
            var data = new float[n];
            float phase = 0f;
            for (int i = 0; i < n; i++)
            {
                float t = (float)i / n;
                float hz = Mathf.Lerp(fromHz, toHz, t);
                phase += 2f * Mathf.PI * hz / SampleRate;
                data[i] = Osc(wave, phase) * Mathf.Pow(1f - t, 1.4f);
            }
            return data;
        }

        private static float[] Noise01(float dur, float gain)
        {
            int n = Mathf.CeilToInt(SampleRate * dur);
            var data = new float[n];
            var rng = new System.Random(1234);
            float prev = 0f;
            for (int i = 0; i < n; i++)
            {
                float t = (float)i / n;
                // one-pole lowpass keeps the noise crunchy rather than hissy
                float white = (float)(rng.NextDouble() * 2.0 - 1.0);
                prev = Mathf.Lerp(prev, white, 0.35f);
                data[i] = prev * Mathf.Pow(1f - t, 2f) * gain;
            }
            return data;
        }

        private static AudioClip Sweep(string name, float fromHz, float toHz, float dur, float gain, Wave wave)
        {
            var d = Sweep01(fromHz, toHz, dur, wave);
            for (int i = 0; i < d.Length; i++) d[i] *= gain;
            return ClipFrom(name, d);
        }

        private static AudioClip Noise(string name, float dur, float gain)
            => ClipFrom(name, Noise01(dur, gain));

        /// <summary>Two slightly detuned sines — a soft crystalline ping with a natural beat.</summary>
        private static AudioClip Detune(string name, float hz, float spreadHz, float dur, float gain)
        {
            int n = Mathf.CeilToInt(SampleRate * dur);
            var data = new float[n];
            float p1 = 0f, p2 = 0f;
            for (int i = 0; i < n; i++)
            {
                float t = (float)i / n;
                p1 += 2f * Mathf.PI * hz / SampleRate;
                p2 += 2f * Mathf.PI * (hz + spreadHz) / SampleRate;
                data[i] = (Mathf.Sin(p1) + Mathf.Sin(p2)) * 0.5f * Mathf.Pow(1f - t, 1.6f) * gain;
            }
            return ClipFrom(name, data);
        }

        private static AudioClip Mix(string name, params float[][] parts)
        {
            int n = 0;
            foreach (var p in parts) n = Mathf.Max(n, p.Length);
            var data = new float[n];
            foreach (var p in parts)
                for (int i = 0; i < p.Length; i++) data[i] += p[i];
            for (int i = 0; i < n; i++) data[i] = Mathf.Clamp(data[i], -0.95f, 0.95f);
            return ClipFrom(name, data);
        }

        private static AudioClip Arp(string name, float[] notes, float dur, float gain, Wave wave)
        {
            int n = Mathf.CeilToInt(SampleRate * dur);
            var data = new float[n];
            float phase = 0f;
            for (int i = 0; i < n; i++)
            {
                float t = (float)i / n;
                int note = Mathf.Clamp(Mathf.FloorToInt(t * notes.Length), 0, notes.Length - 1);
                phase += 2f * Mathf.PI * notes[note] / SampleRate;
                float localT = Mathf.Repeat(t * notes.Length, 1f);
                float env = Mathf.Pow(1f - localT, 1.2f);
                data[i] = Osc(wave, phase) * env * gain;
            }
            return ClipFrom(name, data);
        }

        private static AudioClip ClipFrom(string name, float[] data)
        {
            var clip = AudioClip.Create(name, data.Length, 1, SampleRate, false);
            clip.SetData(data, 0);
            return clip;
        }
    }
}
