using UnityEngine;

namespace Velocity
{
    /// <summary>Constant rotation, used for saw blades / spinning coins.</summary>
    public class Spinner : MonoBehaviour
    {
        public float degreesPerSecond = 360f;

        private void Update()
        {
            transform.Rotate(0f, 0f, degreesPerSecond * Time.deltaTime);
        }
    }
}
