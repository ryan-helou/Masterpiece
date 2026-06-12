using UnityEngine;

namespace Velocity
{
    /// <summary>
    /// Tiny procedural sound engine. Generates blip/sweep AudioClips at runtime so we ship with
    /// no audio assets. One persistent AudioSource plays one-shots. Volume is driven by Settings.
    /// </summary>
    public class Sfx : MonoBehaviour
    {
        public static Sfx Instance { get; private set; }

        private AudioSource _source;
        private const int SampleRate = 44100;

        private AudioClip _jump, _dash, _land, _death, _finish, _click, _record;
        private AudioClip _coin, _stomp, _walljump, _powerup, _dashReady;
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
            _source = gameObject.AddComponent<AudioSource>();
            _source.playOnAwake = false;
            _source.spatialBlend = 0f;

            _jump   = Sweep("jump", 520f, 880f, 0.12f, 0.35f, Wave.Square);
            _dash   = Sweep("dash", 240f, 70f, 0.18f, 0.4f, Wave.Saw);
            _land   = Sweep("land", 200f, 120f, 0.08f, 0.3f, Wave.Square);
            _death  = Sweep("death", 400f, 60f, 0.45f, 0.5f, Wave.Saw);
            _finish = Arp("finish", new[] { 523.25f, 659.25f, 783.99f, 1046.5f }, 0.40f, 0.45f);
            _click  = Sweep("click", 660f, 660f, 0.05f, 0.25f, Wave.Square);
            _record = Arp("record", new[] { 783.99f, 1046.5f, 1318.5f }, 0.45f, 0.5f);
            _coin   = Arp("coin", new[] { 987.77f, 1318.5f }, 0.14f, 0.4f);
            _stomp  = Sweep("stomp", 200f, 80f, 0.1f, 0.55f, Wave.Square);
            _walljump = Sweep("walljump", 320f, 660f, 0.1f, 0.32f, Wave.Saw);
            _powerup = Arp("powerup", new[] { 659.25f, 880f, 1174.66f, 1567.98f }, 0.32f, 0.42f); // rising sparkle
            _dashReady = Sweep("dashready", 1320f, 1760f, 0.05f, 0.2f, Wave.Sine);                 // short ready tick

            _combo = new AudioClip[8];
            for (int i = 0; i < _combo.Length; i++)
            {
                float f = 523.25f * Mathf.Pow(2f, i / 12f);     // rises a semitone per combo
                _combo[i] = Sweep("combo" + i, f, f * 1.5f, 0.09f, 0.32f, Wave.Square);
            }
        }

        public void PlayJump()   => Play(_jump);
        public void PlayDash()   => Play(_dash);
        public void PlayLand()   => Play(_land);
        public void PlayDeath()  => Play(_death);
        public void PlayFinish() => Play(_finish);
        public void PlayClick()  => Play(_click);
        public void PlayRecord() => Play(_record);
        public void PlayCoin()   => Play(_coin);
        public void PlayStomp()  => Play(_stomp);
        public void PlayWallJump() => Play(_walljump);
        public void PlayPowerUp() => Play(_powerup);
        public void PlayDashReady() => Play(_dashReady);
        public void PlayCombo(int n) { if (_combo != null) Play(_combo[Mathf.Clamp(n, 0, _combo.Length - 1)]); }

        private void Play(AudioClip clip)
        {
            if (clip == null) return;
            _source.PlayOneShot(clip, Settings.SfxVolume * Settings.MasterVolume);
        }

        private enum Wave { Sine, Square, Saw }

        private static float Osc(Wave wave, float phase)
        {
            switch (wave)
            {
                case Wave.Square: return Mathf.Sin(phase) >= 0 ? 1f : -1f;
                case Wave.Saw:    return Mathf.Repeat(phase / (2f * Mathf.PI), 1f) * 2f - 1f;
                default:          return Mathf.Sin(phase);
            }
        }

        private static AudioClip Sweep(string name, float fromHz, float toHz, float dur, float gain, Wave wave)
        {
            int n = Mathf.CeilToInt(SampleRate * dur);
            var data = new float[n];
            float phase = 0f;
            for (int i = 0; i < n; i++)
            {
                float t = (float)i / n;
                float hz = Mathf.Lerp(fromHz, toHz, t);
                phase += 2f * Mathf.PI * hz / SampleRate;
                float env = Mathf.Pow(1f - t, 1.4f);            // quick decay
                data[i] = Osc(wave, phase) * env * gain;
            }
            return ClipFrom(name, data);
        }

        private static AudioClip Arp(string name, float[] notes, float dur, float gain)
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
                data[i] = Osc(Wave.Square, phase) * env * gain;
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
