using UnityEngine;

namespace Velocity
{
    /// <summary>Glide power-up pickup (the feather): grants ~15s of floaty low-gravity glides
    /// and changes the character's look while held.</summary>
    public class Feather : MonoBehaviour
    {
        private bool _taken;

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (_taken) return;
            var pc = other.GetComponentInParent<PlayerController>();
            if (pc == null) return;

            _taken = true;
            pc.GrantPower(PowerUp.Glide);
            gameObject.SetActive(false);
        }
    }

    /// <summary>Generic power-up pickup — used for the High-Jump power-up ('u' tile).</summary>
    public class PowerUpItem : MonoBehaviour
    {
        public PowerUp kind = PowerUp.HighJump;
        private bool _taken;

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (_taken) return;
            var pc = other.GetComponentInParent<PlayerController>();
            if (pc == null) return;

            _taken = true;
            pc.GrantPower(kind);
            gameObject.SetActive(false);
        }
    }
}
