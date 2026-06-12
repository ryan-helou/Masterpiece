using System;
using System.Collections.Generic;
using UnityEngine;

namespace Velocity
{
    /// <summary>
    /// Local leaderboard: keeps the best N times per level in PlayerPrefs (as JSON).
    /// "Leaderboard" here means your personal best-times ladder per level — fully offline.
    /// </summary>
    public static class Leaderboard
    {
        private const int MaxEntries = 10;

        [Serializable]
        private class TimeList { public List<double> times = new(); }

        private static string Key(string levelId) => "vel.lb." + levelId;

        public static List<double> GetTimes(string levelId)
        {
            var json = PlayerPrefs.GetString(Key(levelId), "");
            if (string.IsNullOrEmpty(json)) return new List<double>();
            var data = JsonUtility.FromJson<TimeList>(json);
            return data?.times ?? new List<double>();
        }

        public static bool TryGetBest(string levelId, out double best)
        {
            var t = GetTimes(levelId);
            if (t.Count > 0) { best = t[0]; return true; }
            best = 0;
            return false;
        }

        public static bool HasCompleted(string levelId) => GetTimes(levelId).Count > 0;

        /// <summary>
        /// Records a finished run. Returns the 1-based rank if it made the top N (else 0).
        /// <paramref name="isRecord"/> is true when it becomes the new #1.
        /// </summary>
        public static int Submit(string levelId, double seconds, out bool isRecord)
        {
            var times = GetTimes(levelId);
            bool hadBest = times.Count > 0;
            double prevBest = hadBest ? times[0] : double.MaxValue;

            times.Add(seconds);
            times.Sort();
            if (times.Count > MaxEntries)
                times.RemoveRange(MaxEntries, times.Count - MaxEntries);

            Save(levelId, times);

            isRecord = !hadBest || seconds < prevBest;
            int rank = times.IndexOf(seconds) + 1;     // 0 if trimmed out of the board
            return rank;
        }

        /// <summary>Sum of best times across the given levels + how many are completed.</summary>
        public static (double total, int completed, int count) WorldSummary(IEnumerable<string> levelIds)
        {
            double total = 0;
            int completed = 0, count = 0;
            foreach (var id in levelIds)
            {
                count++;
                if (TryGetBest(id, out var best)) { total += best; completed++; }
            }
            return (total, completed, count);
        }

        public static void Clear(string levelId)
        {
            PlayerPrefs.DeleteKey(Key(levelId));
            PlayerPrefs.Save();
        }

        private static void Save(string levelId, List<double> times)
        {
            var data = new TimeList { times = times };
            PlayerPrefs.SetString(Key(levelId), JsonUtility.ToJson(data));
            PlayerPrefs.Save();
        }
    }
}
