using UnityEngine;
using UnityEngine.UI;

namespace Velocity
{
    /// <summary>
    /// Gives the whole game a semi-pixelated look. The world camera renders into a low-resolution,
    /// point-filtered RenderTexture; a small dedicated "display" camera then draws that buffer back
    /// up to fill the screen (via a Screen-Space-Camera canvas on the UI layer). The game's own UI
    /// is a separate ScreenSpace-Overlay canvas sorted on top, so the HUD/menus stay crisp.
    ///
    /// Keeping a real camera pointed at the display is what prevents Unity's "No cameras rendering"
    /// message once the world camera has been redirected to the texture. pixelSize &lt;= 1 disables it.
    /// </summary>
    public class PixelRender : MonoBehaviour
    {
        private const int UiLayer = 5;   // built-in "UI" layer

        [Tooltip("Screen pixels per game pixel. 1 = off, 3 = a clean 'semi-pixelated' chunk.")]
        public int pixelSize = 3;

        private Camera _cam;
        private RenderTexture _rt;
        private RawImage _raw;
        private int _lastW, _lastH, _lastSize;

        public void Init(Camera cam, int size)
        {
            _cam = cam;
            pixelSize = Mathf.Max(1, size);
            if (pixelSize <= 1) return;   // pixelation off — world camera renders straight to display

            // dedicated camera that owns the actual display and draws ONLY the pixel-output canvas
            var dcGo = new GameObject("DisplayCam");
            dcGo.transform.SetParent(transform, false);
            var displayCam = dcGo.AddComponent<Camera>();
            displayCam.orthographic = true;
            displayCam.clearFlags = CameraClearFlags.SolidColor;
            displayCam.backgroundColor = Color.black;
            displayCam.cullingMask = 1 << UiLayer;   // never re-renders the world, only the canvas
            displayCam.depth = -100;

            // the pixel output: a full-screen RawImage on a Screen-Space-Camera canvas the DisplayCam renders
            var canvasGo = new GameObject("PixelCanvas") { layer = UiLayer };
            canvasGo.transform.SetParent(transform, false);
            var canvas = canvasGo.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceCamera;
            canvas.worldCamera = displayCam;
            canvas.planeDistance = 1f;

            var rawGo = new GameObject("PixelOutput") { layer = UiLayer };
            rawGo.transform.SetParent(canvasGo.transform, false);
            _raw = rawGo.AddComponent<RawImage>();
            _raw.raycastTarget = false;
            var rt = (RectTransform)_raw.transform;
            rt.anchorMin = Vector2.zero; rt.anchorMax = Vector2.one;
            rt.offsetMin = Vector2.zero; rt.offsetMax = Vector2.zero;

            Rebuild();
        }

        private void Rebuild()
        {
            _lastW = Mathf.Max(1, Screen.width);
            _lastH = Mathf.Max(1, Screen.height);
            _lastSize = Mathf.Max(1, pixelSize);

            int w = Mathf.Max(1, _lastW / _lastSize);
            int h = Mathf.Max(1, _lastH / _lastSize);

            if (_rt != null) { _cam.targetTexture = null; _rt.Release(); Destroy(_rt); }

            _rt = new RenderTexture(w, h, 24, RenderTextureFormat.Default)
            {
                filterMode = FilterMode.Point,
                antiAliasing = 1,
                name = "PixelRT"
            };
            _rt.Create();
            _cam.targetTexture = _rt;
            _raw.texture = _rt;
        }

        private void Update()
        {
            if (_raw == null) return;     // pixelation disabled
            if (Screen.width != _lastW || Screen.height != _lastH || pixelSize != _lastSize)
                Rebuild();
        }

        private void OnDestroy()
        {
            if (_cam != null) _cam.targetTexture = null;
            if (_rt != null) { _rt.Release(); Destroy(_rt); }
        }
    }
}
