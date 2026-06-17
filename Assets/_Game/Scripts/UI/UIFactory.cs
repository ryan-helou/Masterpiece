using System;
using UnityEngine;
using UnityEngine.UI;

namespace Velocity
{
    /// <summary>
    /// Code-only UGUI builder. Every label in the game pulls its typeface from <see cref="Font"/>,
    /// so this one property re-skins all in-game text. Everything is anchored/scaled for a
    /// 1920x1080 reference canvas.
    /// </summary>
    public static class UIFactory
    {
        private static Font _font;

        /// <summary>
        /// An elegant serif, loaded straight from the OS so the project keeps its no-bundled-assets
        /// philosophy (no TextMeshPro / no imported .ttf). The names are tried in order — Georgia
        /// resolves on macOS and Windows, with Times/Palatino as backups — and if none are installed
        /// we fall back to Unity's built-in font rather than rendering nothing.
        /// </summary>
        public static Font Font
        {
            get
            {
                if (_font == null)
                {
                    _font = UnityEngine.Font.CreateDynamicFontFromOSFont(
                        new[] { "Georgia", "Times New Roman", "Times", "Palatino", "PT Serif", "Serif" }, 32);
                    if (_font == null)
                        _font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
                }
                return _font;
            }
        }

        public static RectTransform Rect(Transform parent, string name)
        {
            var go = new GameObject(name, typeof(RectTransform));
            go.transform.SetParent(parent, false);
            return (RectTransform)go.transform;
        }

        public static void Stretch(RectTransform rt, float pad = 0f)
        {
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.offsetMin = new Vector2(pad, pad);
            rt.offsetMax = new Vector2(-pad, -pad);
        }

        public static void Place(RectTransform rt, Vector2 anchor, Vector2 pivot,
            Vector2 pos, Vector2 size)
        {
            rt.anchorMin = anchor;
            rt.anchorMax = anchor;
            rt.pivot = pivot;
            rt.anchoredPosition = pos;
            rt.sizeDelta = size;
        }

        public static Canvas Canvas(string name, int sortOrder = 0)
        {
            var go = new GameObject(name, typeof(RectTransform));
            var canvas = go.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = sortOrder;
            var scaler = go.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            scaler.matchWidthOrHeight = 0.5f;
            go.AddComponent<GraphicRaycaster>();
            return canvas;
        }

        public static Image Panel(Transform parent, Color color, string name = "Panel")
        {
            var rt = Rect(parent, name);
            Stretch(rt);
            var img = rt.gameObject.AddComponent<Image>();
            img.color = color;
            return img;
        }

        public static Image Box(Transform parent, Vector2 pos, Vector2 size, Color color,
            Vector2 anchor, Vector2 pivot, string name = "Box")
        {
            var rt = Rect(parent, name);
            Place(rt, anchor, pivot, pos, size);
            var img = rt.gameObject.AddComponent<Image>();
            img.color = color;
            return img;
        }

        public static Text Label(Transform parent, string text, int size, Color color,
            TextAnchor align = TextAnchor.MiddleCenter, FontStyle style = FontStyle.Normal)
        {
            var rt = Rect(parent, "Text");
            var t = rt.gameObject.AddComponent<Text>();
            t.font = Font;
            t.text = text;
            t.fontSize = size;
            t.color = color;
            t.alignment = align;
            t.fontStyle = style;
            t.horizontalOverflow = HorizontalWrapMode.Overflow;
            t.verticalOverflow = VerticalWrapMode.Overflow;
            t.raycastTarget = false;
            return t;
        }

        public static Text LabelAt(Transform parent, string text, int size, Color color,
            Vector2 anchor, Vector2 pivot, Vector2 pos, Vector2 boxSize,
            TextAnchor align = TextAnchor.MiddleCenter, FontStyle style = FontStyle.Normal)
        {
            var t = Label(parent, text, size, color, align, style);
            Place((RectTransform)t.transform, anchor, pivot, pos, boxSize);
            t.horizontalOverflow = HorizontalWrapMode.Wrap;
            return t;
        }

        public static Button Button(Transform parent, string label, Vector2 pos, Vector2 size,
            Action onClick, Color? bg = null, Color? text = null, int fontSize = 30,
            Vector2? anchor = null)
        {
            var a = anchor ?? new Vector2(0.5f, 0.5f);
            var rt = Rect(parent, "Button_" + label);
            Place(rt, a, new Vector2(0.5f, 0.5f), pos, size);

            var img = rt.gameObject.AddComponent<Image>();
            Color baseColor = bg ?? Palette.UiPanelSoft;
            img.color = baseColor;

            var btn = rt.gameObject.AddComponent<Button>();
            btn.targetGraphic = img;
            var cb = btn.colors;
            cb.normalColor = baseColor;
            cb.highlightedColor = Lighten(baseColor, 0.12f);
            cb.pressedColor = Darken(baseColor, 0.12f);
            cb.selectedColor = Lighten(baseColor, 0.08f);
            cb.fadeDuration = 0.08f;
            btn.colors = cb;
            if (onClick != null)
                btn.onClick.AddListener(() => { if (Sfx.Instance != null) Sfx.Instance.PlayClick(); onClick(); });

            var t = Label(rt, label, fontSize, text ?? Palette.UiText, TextAnchor.MiddleCenter, FontStyle.Bold);
            Stretch((RectTransform)t.transform);
            return btn;
        }

        /// <summary>
        /// A button whose face is a 9-sliced museum plaque sprite (brass / gilded) with an engraved
        /// label. Falls back to the flat <see cref="Button"/> if <paramref name="plate"/> is null, so
        /// a missing PNG never breaks the menu. The plate's height maps to the button height and its
        /// ornate ends are kept undistorted while the plain middle stretches to the label width.
        /// </summary>
        public static Button PlateButton(Transform parent, string label, Vector2 pos, Vector2 size,
            Action onClick, Sprite plate, Color? text = null, int fontSize = 30, Vector2? anchor = null)
        {
            if (plate == null)
                return Button(parent, label, pos, size, onClick, Palette.UiPanelSoft, text, fontSize, anchor);

            var a = anchor ?? new Vector2(0.5f, 0.5f);
            var rt = Rect(parent, "Plate_" + label);
            Place(rt, a, new Vector2(0.5f, 0.5f), pos, size);

            var img = rt.gameObject.AddComponent<Image>();
            img.sprite = plate;
            img.type = Image.Type.Sliced;
            img.fillCenter = true;
            // scale the slice so the plate's full height maps to the button height — keeps the ornate
            // ends in proportion and guarantees the 9-slice borders never overflow a short button.
            img.pixelsPerUnitMultiplier = Mathf.Max(0.01f, plate.rect.height / Mathf.Max(1f, size.y));
            img.color = Color.white;

            var btn = rt.gameObject.AddComponent<Button>();
            btn.targetGraphic = img;
            // the plate art is already at full brightness, so rest a touch dim and brighten on hover
            var cb = btn.colors;
            cb.normalColor = new Color(0.88f, 0.88f, 0.88f);
            cb.highlightedColor = Color.white;
            cb.pressedColor = new Color(0.72f, 0.72f, 0.72f);
            cb.selectedColor = new Color(0.96f, 0.96f, 0.96f);
            cb.fadeDuration = 0.08f;
            btn.colors = cb;
            if (onClick != null)
                btn.onClick.AddListener(() => { if (Sfx.Instance != null) Sfx.Instance.PlayClick(); onClick(); });

            var lbl = Label(rt, label, fontSize, text ?? Palette.UiText, TextAnchor.MiddleCenter, FontStyle.Bold);
            Stretch((RectTransform)lbl.transform);
            var sh = lbl.gameObject.AddComponent<Shadow>();   // engraved-into-metal feel
            sh.effectColor = new Color(0f, 0f, 0f, 0.5f);
            sh.effectDistance = new Vector2(1.5f, -1.5f);
            return btn;
        }

        public static Slider Slider(Transform parent, float value, Vector2 pos, Vector2 size,
            Action<float> onChange, Vector2? anchor = null)
        {
            var a = anchor ?? new Vector2(0.5f, 0.5f);
            var root = Rect(parent, "Slider");
            Place(root, a, new Vector2(0.5f, 0.5f), pos, size);
            var slider = root.gameObject.AddComponent<Slider>();

            var bg = Rect(root, "BG");
            Stretch(bg);
            bg.anchorMin = new Vector2(0, 0.5f);
            bg.anchorMax = new Vector2(1, 0.5f);
            bg.sizeDelta = new Vector2(0, 12);
            bg.anchoredPosition = Vector2.zero;
            var bgImg = bg.gameObject.AddComponent<Image>();
            bgImg.color = Palette.UiPanel;

            var fillArea = Rect(root, "FillArea");
            fillArea.anchorMin = new Vector2(0, 0.5f);
            fillArea.anchorMax = new Vector2(1, 0.5f);
            fillArea.sizeDelta = new Vector2(0, 12);
            fillArea.anchoredPosition = Vector2.zero;
            var fill = Rect(fillArea, "Fill");
            fill.anchorMin = Vector2.zero;
            fill.anchorMax = Vector2.one;
            fill.sizeDelta = Vector2.zero;
            var fillImg = fill.gameObject.AddComponent<Image>();
            fillImg.color = Palette.UiAccent;

            var handleArea = Rect(root, "HandleArea");
            Stretch(handleArea);
            var handle = Rect(handleArea, "Handle");
            handle.sizeDelta = new Vector2(22, 36);
            var handleImg = handle.gameObject.AddComponent<Image>();
            handleImg.color = Palette.UiText;

            slider.fillRect = fill;
            slider.handleRect = handle;
            slider.targetGraphic = handleImg;
            slider.direction = UnityEngine.UI.Slider.Direction.LeftToRight;
            slider.minValue = 0f;
            slider.maxValue = 1f;
            slider.value = value;
            if (onChange != null) slider.onValueChanged.AddListener(v => onChange(v));
            return slider;
        }

        public static Toggle Toggle(Transform parent, string label, bool value, Vector2 pos,
            Vector2 size, Action<bool> onChange, Vector2? anchor = null)
        {
            var a = anchor ?? new Vector2(0.5f, 0.5f);
            var root = Rect(parent, "Toggle_" + label);
            Place(root, a, new Vector2(0.5f, 0.5f), pos, size);
            var toggle = root.gameObject.AddComponent<Toggle>();

            var box = Rect(root, "Box");
            Place(box, new Vector2(0, 0.5f), new Vector2(0, 0.5f), new Vector2(18, 0), new Vector2(34, 34));
            var boxImg = box.gameObject.AddComponent<Image>();
            boxImg.color = Palette.UiPanel;

            var check = Rect(box, "Check");
            Stretch(check, 6);
            var checkImg = check.gameObject.AddComponent<Image>();
            checkImg.color = Palette.UiAccent;

            var lbl = Label(root, label, 26, Palette.UiText, TextAnchor.MiddleLeft);
            Place((RectTransform)lbl.transform, new Vector2(0, 0.5f), new Vector2(0, 0.5f),
                new Vector2(64, 0), new Vector2(size.x - 64, size.y));

            toggle.targetGraphic = boxImg;
            toggle.graphic = checkImg;
            toggle.isOn = value;
            if (onChange != null) toggle.onValueChanged.AddListener(v => onChange(v));
            return toggle;
        }

        public static RectTransform Group(Transform parent, string name, Vector2 anchor,
            Vector2 pivot, Vector2 pos, Vector2 size, float spacing, RectOffset padding = null)
        {
            var rt = Rect(parent, name);
            Place(rt, anchor, pivot, pos, size);
            var v = rt.gameObject.AddComponent<VerticalLayoutGroup>();
            v.spacing = spacing;
            v.childControlWidth = true;
            v.childControlHeight = true;
            v.childForceExpandWidth = true;
            v.childForceExpandHeight = false;
            v.padding = padding ?? new RectOffset(0, 0, 0, 0);
            return rt;
        }

        public static Color Lighten(Color c, float amt) =>
            new Color(Mathf.Clamp01(c.r + amt), Mathf.Clamp01(c.g + amt), Mathf.Clamp01(c.b + amt), c.a);

        public static Color Darken(Color c, float amt) =>
            new Color(Mathf.Clamp01(c.r - amt), Mathf.Clamp01(c.g - amt), Mathf.Clamp01(c.b - amt), c.a);
    }
}
