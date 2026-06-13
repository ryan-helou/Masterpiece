using UnityEngine;

namespace Velocity
{
    /// <summary>Best coin count per level (persisted).</summary>
    public static class Coins
    {
        private static string Key(string levelId) => "vel.coins." + levelId;
        public static int Best(string levelId) => PlayerPrefs.GetInt(Key(levelId), 0);
        public static void Submit(string levelId, int n)
        {
            if (n > Best(levelId)) { PlayerPrefs.SetInt(Key(levelId), n); PlayerPrefs.Save(); }
        }
        public static void Clear(string levelId) => PlayerPrefs.DeleteKey(Key(levelId));
    }

    /// <summary>An optional coin. Grabbing it bumps the level's coin count (doesn't affect time).</summary>
    public class Collectible : MonoBehaviour
    {
        private bool _taken;

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (_taken) return;
            if (other.GetComponentInParent<PlayerController>() == null) return;
            _taken = true;

            if (GameManager.Instance != null) GameManager.Instance.OnCoinGrabbed();
            if (Sfx.Instance != null) Sfx.Instance.PlayCoin();
            if (FX.Instance != null)
            {
                FX.Instance.Burst(transform.position, Palette.Coin, 10, 6f, 0.15f, 0.5f, -8f, 2.5f);
                FX.Instance.Burst(transform.position, Color.white, 5, 4f, 0.1f, 0.45f, -6f, 2.5f);
            }
            gameObject.SetActive(false);
        }
    }
}
