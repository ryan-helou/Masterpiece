using UnityEngine;

namespace Velocity
{
    /// <summary>Finish trigger. Touching it completes the level — UNLESS a boss still lives
    /// (<see cref="ScreamBoss.Alive"/> > 0), in which case the exit stays sealed and dimmed until
    /// the boss is defeated, then flashes open. (No boss in the level → behaves as a plain goal.)</summary>
    public class Goal : MonoBehaviour
    {
        private bool _used;
        private bool _sealed;
        private SpriteRenderer[] _vis;
        private Color[] _base;

        private void Start()
        {
            _vis = GetComponentsInChildren<SpriteRenderer>();
            _base = new Color[_vis.Length];
            for (int i = 0; i < _vis.Length; i++) _base[i] = _vis[i].color;
            _sealed = ScreamBoss.Alive > 0;
            Apply();
        }

        private void Update()
        {
            bool nowSealed = ScreamBoss.Alive > 0;
            if (nowSealed == _sealed) return;
            _sealed = nowSealed;
            Apply();
            if (!_sealed)   // the boss just died — the way opens
            {
                if (Sfx.Instance != null) Sfx.Instance.PlayRing();
                if (GameManager.Instance != null) GameManager.Instance.Shake(0.3f);
                if (FX.Instance != null) FX.Instance.Burst(transform.position, Palette.Goal, 22, 7f, 0.2f, 0.7f, -4f, 1.5f);
            }
        }

        private void Apply()
        {
            if (_vis == null) return;
            for (int i = 0; i < _vis.Length; i++)
            {
                var c = _base[i];
                if (_sealed) c.a *= 0.22f;
                _vis[i].color = c;
            }
        }

        private void OnTriggerEnter2D(Collider2D other) => TryFinish(other);
        private void OnTriggerStay2D(Collider2D other) => TryFinish(other);   // also fires if you wait inside it for the seal to lift

        private void TryFinish(Collider2D other)
        {
            if (_used || _sealed) return;
            if (other.GetComponentInParent<PlayerController>() == null) return;
            _used = true;
            if (GameManager.Instance != null) GameManager.Instance.OnReachGoal();
        }
    }
}
