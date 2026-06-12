using System.Collections.Generic;
using UnityEngine;

namespace Velocity
{
    /// <summary>
    /// Subtle multi-layer parallax starfield built from scattered dim dots. Each layer tracks the
    /// camera at a fraction of its movement to fake depth. Rebuilt per level (sized to its bounds).
    /// </summary>
    public class Backdrop : MonoBehaviour
    {
        private struct Layer { public Transform root; public float factor; }
        private readonly List<Layer> _layers = new List<Layer>();
        private Transform _cam;

        public void Setup(Transform cameraTransform, Bounds bounds)
        {
            _cam = cameraTransform;
            var area = bounds;
            area.Expand(new Vector3(24f, 16f, 0f));

            BuildLayer(0.55f, 46, 0.16f, Palette.BackdropFar, -30, area);
            BuildLayer(0.78f, 30, 0.24f, Palette.BackdropNear, -20, area);
        }

        private void BuildLayer(float factor, int count, float size, Color color, int order, Bounds area)
        {
            var root = new GameObject("BackLayer").transform;
            root.SetParent(transform, false);

            for (int i = 0; i < count; i++)
            {
                var go = new GameObject("dot");
                go.transform.SetParent(root, false);
                go.transform.localPosition = new Vector3(
                    Random.Range(area.min.x, area.max.x),
                    Random.Range(area.min.y, area.max.y), 0f);
                go.transform.localScale = Vector3.one * (size * Random.Range(0.6f, 1.4f));
                var sr = go.AddComponent<SpriteRenderer>();
                sr.sprite = TextureFactory.Circle(8, 24);
                sr.color = color;
                sr.sortingOrder = order;
            }

            _layers.Add(new Layer { root = root, factor = factor });
        }

        private void LateUpdate()
        {
            if (_cam == null) return;
            foreach (var l in _layers)
            {
                var p = _cam.position * (1f - l.factor);
                l.root.position = new Vector3(p.x, p.y, 0f);
            }
        }
    }
}
