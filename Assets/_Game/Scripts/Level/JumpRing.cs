using UnityEngine;

namespace Velocity
{
    /// <summary>Geometry-Dash jump ring: while you're inside it, tap jump for a boosted air-jump.</summary>
    public class JumpRing : MonoBehaviour
    {
        public float boost = 19f;

        private void OnTriggerStay2D(Collider2D other)
        {
            var pc = other.GetComponentInParent<PlayerController>();
            if (pc != null) pc.OfferRing(boost);
        }
    }
}
