using UnityEngine;

namespace Velocity
{
    /// <summary>
    /// Procedurally-generated ambient background loop (no audio assets). A low bass drone tuned so
    /// it loops seamlessly, plus a soft pentatonic arpeggio. Volume follows Music + Master settings.
    /// </summary>
    public class Music : MonoBehaviour
    {
        public static Music Instance { get; private set; }

        private AudioSource _src;
        private const int SampleRate = 44100;

        public static void Bootstrap()
        {
            if (Instance != null) return;
            var go = new GameObject("Music");
            DontDestroyOnLoad(go);
            Instance = go.AddComponent<Music>();
        }

        private void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
            _src = gameObject.AddComponent<AudioSource>();
            _src.loop = true;
            _src.playOnAwake = false;
            _src.spatialBlend = 0f;
            _src.clip = BuildLoop();
            _src.Play();
        }

        private void Update()
        {
            if (_src != null) _src.volume = Settings.MusicVolume * Settings.MasterVolume;
        }

        private AudioClip BuildLoop()
        {
            const float dur = 8f;                       // 8s; 55Hz * 8 = 440 cycles -> seamless bass
            int n = (int)(SampleRate * dur);
            var data = new float[n];

            // A-minor pentatonic, melody octave
            float[] scale = { 220f, 261.63f, 293.66f, 329.63f, 392f, 440f };
            int[] pattern = { 0, 2, 4, 2, 1, 3, 5, 3, 0, 2, 4, 5, 4, 2, 1, 0 };
            const float step = 0.5f;

            for (int i = 0; i < n; i++)
            {
                float t = (float)i / SampleRate;

                // bass drone (seamless: integer cycles over the loop)
                float bass = Mathf.Sin(2f * Mathf.PI * 55f * t) * 0.10f
                           + Mathf.Sin(2f * Mathf.PI * 110f * t) * 0.035f;

                // plucked arpeggio
                int s = Mathf.Clamp((int)(t / step), 0, pattern.Length - 1);
                float localT = t - s * step;
                float freq = scale[pattern[s]];
                float attack = Mathf.Clamp01(localT / 0.01f);
                float decay = Mathf.Exp(-localT * 5f);
                float env = attack * decay;
                float arp = Mathf.Sin(2f * Mathf.PI * freq * t) * env * 0.085f;

                data[i] = Mathf.Clamp(bass * 0.6f + arp, -0.9f, 0.9f);
            }

            var clip = AudioClip.Create("AmbientLoop", n, 1, SampleRate, false);
            clip.SetData(data, 0);
            return clip;
        }
    }
}
