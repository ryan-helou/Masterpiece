using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Velocity
{
    /// <summary>
    /// All player controls, defined entirely in code (no .inputactions asset needed) so the
    /// project is self-contained. Supports keyboard + gamepad, live rebinding, and persistence
    /// of overrides to PlayerPrefs.
    /// </summary>
    public class GameInput
    {
        public InputActionAsset Asset { get; private set; }
        public InputAction Move, Jump, Dash, Restart, Pause;

        private const string PrefsKey = "vel.bindings";

        /// <summary>A single user-rebindable control row shown in the Settings menu.</summary>
        public class RebindEntry
        {
            public readonly string Group;
            public readonly string Label;
            public readonly InputAction Action;
            public readonly int BindingIndex;

            public RebindEntry(string group, string label, InputAction action, int bindingIndex)
            {
                Group = group; Label = label; Action = action; BindingIndex = bindingIndex;
            }

            public string DisplayString()
            {
                return Action.GetBindingDisplayString(BindingIndex,
                    InputBinding.DisplayStringOptions.DontUseShortDisplayNames);
            }
        }

        public readonly List<RebindEntry> Rebindable = new();

        public GameInput()
        {
            Asset = ScriptableObject.CreateInstance<InputActionAsset>();
            Asset.name = "VelocityControls";
            var map = Asset.AddActionMap("Gameplay");

            Move = map.AddAction("Move", InputActionType.Value);
            // First composite = WASD (indices 1..4 = Up,Down,Left,Right). These are the keys
            // we expose for rebinding.
            Move.AddCompositeBinding("2DVector")
                .With("Up", "<Keyboard>/w")
                .With("Down", "<Keyboard>/s")
                .With("Left", "<Keyboard>/a")
                .With("Right", "<Keyboard>/d");
            // Second composite = arrow keys (fixed alternates).
            Move.AddCompositeBinding("2DVector")
                .With("Up", "<Keyboard>/upArrow")
                .With("Down", "<Keyboard>/downArrow")
                .With("Left", "<Keyboard>/leftArrow")
                .With("Right", "<Keyboard>/rightArrow");
            Move.AddBinding("<Gamepad>/leftStick");
            Move.AddBinding("<Gamepad>/dpad");

            Jump = map.AddAction("Jump", InputActionType.Button);
            Jump.AddBinding("<Keyboard>/space");
            Jump.AddBinding("<Gamepad>/buttonSouth");

            Dash = map.AddAction("Dash", InputActionType.Button);
            Dash.AddBinding("<Keyboard>/leftShift");
            Dash.AddBinding("<Gamepad>/buttonWest");

            Restart = map.AddAction("Restart", InputActionType.Button);
            Restart.AddBinding("<Keyboard>/r");
            Restart.AddBinding("<Gamepad>/buttonNorth");

            Pause = map.AddAction("Pause", InputActionType.Button);
            Pause.AddBinding("<Keyboard>/escape");
            Pause.AddBinding("<Gamepad>/start");

            // Curated, user-facing rebind list (keyboard bindings only).
            Rebindable.Add(new RebindEntry("Movement", "Move Up",    Move, 1));
            Rebindable.Add(new RebindEntry("Movement", "Move Down",  Move, 2));
            Rebindable.Add(new RebindEntry("Movement", "Move Left",  Move, 3));
            Rebindable.Add(new RebindEntry("Movement", "Move Right", Move, 4));
            Rebindable.Add(new RebindEntry("Actions",  "Jump",    Jump, 0));
            Rebindable.Add(new RebindEntry("Actions",  "Dash",    Dash, 0));
            Rebindable.Add(new RebindEntry("Actions",  "Restart", Restart, 0));
            Rebindable.Add(new RebindEntry("Actions",  "Pause",   Pause, 0));

            LoadOverrides();
            Asset.Enable();
        }

        // ---- convenience accessors used by the controller ------------------
        public float MoveX => Mathf.Clamp(Move.ReadValue<Vector2>().x, -1f, 1f);
        public float MoveY => Move.ReadValue<Vector2>().y;
        public bool JumpPressed  => Jump.WasPressedThisFrame();
        public bool JumpHeld     => Jump.IsPressed();
        public bool JumpReleased => Jump.WasReleasedThisFrame();
        public bool DashPressed  => Dash.WasPressedThisFrame();
        public bool RestartPressed => Restart.WasPressedThisFrame();
        public bool PausePressed   => Pause.WasPressedThisFrame();

        public void Enable()  => Asset.Enable();
        public void Disable() => Asset.Disable();

        // ---- rebinding -----------------------------------------------------
        private InputActionRebindingExtensions.RebindingOperation _active;

        public void StartRebind(RebindEntry entry, Action onFinished)
        {
            _active?.Dispose();
            Asset.Disable();

            _active = entry.Action.PerformInteractiveRebinding(entry.BindingIndex)
                .WithControlsExcluding("<Mouse>")
                .WithControlsExcluding("<Pointer>/position")
                .WithCancelingThrough("<Keyboard>/escape")
                .OnComplete(op =>
                {
                    op.Dispose();
                    _active = null;
                    Asset.Enable();
                    SaveOverrides();
                    onFinished?.Invoke();
                })
                .OnCancel(op =>
                {
                    op.Dispose();
                    _active = null;
                    Asset.Enable();
                    onFinished?.Invoke();
                })
                .Start();
        }

        public void CancelRebind()
        {
            _active?.Cancel();
            _active?.Dispose();
            _active = null;
            Asset.Enable();
        }

        // ---- persistence ---------------------------------------------------
        public void SaveOverrides()
        {
            PlayerPrefs.SetString(PrefsKey, Asset.SaveBindingOverridesAsJson());
            PlayerPrefs.Save();
        }

        public void LoadOverrides()
        {
            var json = PlayerPrefs.GetString(PrefsKey, "");
            if (!string.IsNullOrEmpty(json))
                Asset.LoadBindingOverridesFromJson(json);
        }

        public void ResetToDefaults()
        {
            Asset.RemoveAllBindingOverrides();
            PlayerPrefs.DeleteKey(PrefsKey);
            PlayerPrefs.Save();
        }
    }
}
