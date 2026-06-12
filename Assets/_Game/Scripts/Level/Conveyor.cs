using UnityEngine;

namespace Velocity
{
    /// <summary>
    /// A belt that nudges whatever stands on it. Holds a horizontal velocity that the
    /// PlayerController reads while grounded on it (see ProbeGround).
    /// </summary>
    public class Conveyor : MonoBehaviour
    {
        public Vector2 Velocity = Vector2.zero;

        private float _scroll;
        private SpriteRenderer _sr;

        private void Awake()
        {
            _sr = GetComponent<SpriteRenderer>();
        }

        private void Update()
        {
            // subtle brightness pulse moving along the belt direction for a sense of motion
            if (_sr == null) return;
            _scroll += Velocity.x * Time.deltaTime * 0.4f;
            float pulse = 0.5f + 0.5f * Mathf.Sin(_scroll);
            _sr.color = Color.Lerp(Palette.Moving, Palette.GroundCap, pulse * 0.5f);
        }
    }
}
