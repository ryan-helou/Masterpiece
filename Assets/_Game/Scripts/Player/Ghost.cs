using System;
using System.Collections.Generic;
using UnityEngine;

namespace Velocity
{
    /// <summary>Records a run as fixed-rate position samples.</summary>
    public class GhostRecorder
    {
        public const float Interval = 1f / 30f;
        private const int MaxSamples = 30 * 120;   // cap at 2 minutes

        public readonly List<Vector2> Samples = new List<Vector2>();

        public void Reset() => Samples.Clear();

        public void Record(double elapsed, Vector2 pos)
        {
            int target = (int)(elapsed / Interval) + 1;
            if (target > MaxSamples) target = MaxSamples;
            while (Samples.Count < target) Samples.Add(pos);
        }
    }

    /// <summary>Persists the best run's samples per level (PlayerPrefs, base64-packed floats).</summary>
    public static class GhostStore
    {
        private static string Key(string levelId) => "vel.ghost." + levelId;

        public static void Save(string levelId, List<Vector2> samples)
        {
            if (samples == null || samples.Count == 0) return;
            var floats = new float[samples.Count * 2];
            for (int i = 0; i < samples.Count; i++)
            {
                floats[2 * i] = samples[i].x;
                floats[2 * i + 1] = samples[i].y;
            }
            var bytes = new byte[floats.Length * 4];
            Buffer.BlockCopy(floats, 0, bytes, 0, bytes.Length);
            PlayerPrefs.SetString(Key(levelId), Convert.ToBase64String(bytes));
            PlayerPrefs.Save();
        }

        public static List<Vector2> Load(string levelId)
        {
            var s = PlayerPrefs.GetString(Key(levelId), "");
            if (string.IsNullOrEmpty(s)) return null;
            try
            {
                var bytes = Convert.FromBase64String(s);
                int fcount = bytes.Length / 4;
                var floats = new float[fcount];
                Buffer.BlockCopy(bytes, 0, floats, 0, bytes.Length);
                var list = new List<Vector2>(fcount / 2);
                for (int i = 0; i + 1 < fcount; i += 2)
                    list.Add(new Vector2(floats[i], floats[i + 1]));
                return list;
            }
            catch
            {
                return null;
            }
        }

        public static void Clear(string levelId)
        {
            PlayerPrefs.DeleteKey(Key(levelId));
            PlayerPrefs.Save();
        }
    }

    /// <summary>A translucent racer that replays recorded samples synced to the run time.</summary>
    public class GhostRunner : MonoBehaviour
    {
        private List<Vector2> _samples;
        private SpriteRenderer _sr;

        public void Setup(List<Vector2> samples)
        {
            _samples = samples;

            var v = new GameObject("GhostVisual");
            v.transform.SetParent(transform, false);
            _sr = v.AddComponent<SpriteRenderer>();
            _sr.sprite = TextureFactory.RoundedBox(16, 22, 7, 20);
            var c = Palette.Player;
            c.a = 0.28f;
            _sr.color = c;
            _sr.sortingOrder = 8;   // behind the live player (10)

            ResetToStart();
        }

        public void ResetToStart()
        {
            if (_samples != null && _samples.Count > 0)
                transform.position = _samples[0];
        }

        /// <summary>The time the ghost was at horizontal position x (for live +/- deltas). -1 if no data.</summary>
        public float TimeAtX(float x)
        {
            if (_samples == null || _samples.Count == 0) return -1f;
            for (int i = 0; i < _samples.Count; i++)
                if (_samples[i].x >= x) return i * GhostRecorder.Interval;
            return _samples.Count * GhostRecorder.Interval;   // past the end (ghost already finished)
        }

        public void SetTime(double t)
        {
            if (_samples == null || _samples.Count == 0) return;
            double fi = t / GhostRecorder.Interval;
            int i = (int)fi;
            if (i >= _samples.Count - 1)
            {
                transform.position = _samples[_samples.Count - 1];
            }
            else
            {
                float f = (float)(fi - i);
                transform.position = Vector2.Lerp(_samples[i], _samples[i + 1], f);
            }
        }
    }
}
