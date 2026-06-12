using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;

namespace Velocity
{
    /// <summary>
    /// The single component that lives in the scene. On Awake it constructs the entire game in
    /// code — runtime config, camera, EventSystem, input, UI and the GameManager — so the scene
    /// needs nothing else wired up. Just drop this on one GameObject and press Play.
    /// </summary>
    public class GameBootstrap : MonoBehaviour
    {
        [Tooltip("Orthographic half-height of the camera in world units.")]
        public float cameraSize = 6.7f;

        [Tooltip("Physics tick rate. 120 gives crisp, consistent platforming.")]
        public int physicsHz = 120;

        [Tooltip("Screen pixels per game pixel for the pixelated look. 1 = off, 3 = semi-pixelated.")]
        public int pixelSize = 3;

        private static bool _booted;

        private void Awake()
        {
            if (_booted) { Destroy(gameObject); return; }
            _booted = true;
            DontDestroyOnLoad(gameObject);

            ConfigureRuntime();
            Settings.Load();
            Palette.ApplyTheme(Settings.ColorTheme);
            DisplaySettings.Apply();
            Sfx.Bootstrap();
            FX.Bootstrap();
            Music.Bootstrap();

            var cam = BuildCamera();
            BuildEventSystem();

            var input = new GameInput();

            var uiGo = new GameObject("UI");
            uiGo.transform.SetParent(transform, false);
            var ui = uiGo.AddComponent<UIManager>();
            var levelHolder = new GameObject("Level").transform;

            var gmGo = new GameObject("GameManager");
            var gm = gmGo.AddComponent<GameManager>();

            // order matters: GameManager.Instance must exist before UI buttons can fire.
            gm.Setup(input, ui, cam, levelHolder);
            ui.Init(gm, input);
            gm.Boot();
        }

        private void ConfigureRuntime()
        {
            Time.fixedDeltaTime = 1f / Mathf.Max(50, physicsHz);
            Application.runInBackground = true;
            Application.targetFrameRate = -1;    // let the platform/vsync decide
            Physics2D.queriesStartInColliders = false;
        }

        private CameraController BuildCamera()
        {
            var go = new GameObject("Main Camera");
            go.tag = "MainCamera";
            go.transform.position = new Vector3(0, 0, -10);

            var cam = go.AddComponent<Camera>();
            cam.orthographic = true;
            cam.orthographicSize = cameraSize;
            cam.clearFlags = CameraClearFlags.SolidColor;
            cam.backgroundColor = Palette.Background;
            cam.nearClipPlane = 0.1f;
            cam.farClipPlane = 100f;

            go.AddComponent<AudioListener>();
            return go.AddComponent<CameraController>();
        }

        private void BuildEventSystem()
        {
            if (FindAnyObjectByType<EventSystem>() != null) return;
            var go = new GameObject("EventSystem");
            go.transform.SetParent(transform, false);
            go.AddComponent<EventSystem>();
            var module = go.AddComponent<InputSystemUIInputModule>();
            module.AssignDefaultActions();
        }
    }
}
