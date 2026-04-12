using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkillCooldownBar : MonoBehaviour
{
    private const int VisibleSkillCount = 3;

    [SerializeField]
    private PlayerSkillController skillController;

    private readonly List<SlotView> slotViews = new List<SlotView>(VisibleSkillCount);

    public static SkillCooldownBar CreateRuntime(PlayerSkillController targetSkills)
    {
        var canvas = CreateHudCanvas();
        var barGo = new GameObject("Skill Cooldown Bar", typeof(RectTransform));
        barGo.transform.SetParent(canvas.transform, false);

        var rect = (RectTransform)barGo.transform;
        rect.anchorMin = new Vector2(0.5f, 0f);
        rect.anchorMax = new Vector2(0.5f, 0f);
        rect.pivot = new Vector2(0.5f, 0f);
        rect.anchoredPosition = new Vector2(0f, 28f);
        rect.sizeDelta = new Vector2(330f, 108f);

        var bar = barGo.AddComponent<SkillCooldownBar>();
        bar.skillController = targetSkills;
        bar.BuildSlots(rect);
        bar.Refresh();
        return bar;
    }

    private void Start()
    {
        ResolveReferences();
        if (slotViews.Count == 0 && transform is RectTransform rect)
            BuildSlots(rect);

        Refresh();
    }

    private void Update()
    {
        ResolveReferences();
        Refresh();
    }

    private void ResolveReferences()
    {
        if (skillController != null)
            return;

        var player = FindObjectOfType<Player>();
        if (player != null)
            skillController = player.GetComponent<PlayerSkillController>();
    }

    private void BuildSlots(RectTransform parent)
    {
        slotViews.Clear();

        const float slotSize = 86f;
        const float gap = 18f;
        var startX = -slotSize - gap;

        for (var i = 0; i < VisibleSkillCount; i++)
        {
            var slot = CreateSlot(parent, i, new Vector2(startX + i * (slotSize + gap), 0f), slotSize);
            slotViews.Add(slot);
        }
    }

    private void Refresh()
    {
        if (skillController == null)
            return;

        var skills = skillController.Skills;
        for (var i = 0; i < slotViews.Count; i++)
        {
            if (i >= skills.Count || skills[i] == null)
            {
                slotViews[i].Root.SetActive(false);
                continue;
            }

            slotViews[i].Root.SetActive(true);
            RefreshSlot(slotViews[i], skills[i]);
        }
    }

    private static void RefreshSlot(SlotView slot, PlayerSkillController.SkillSlot skill)
    {
        slot.KeyText.text = BindingLabel(skill.binding);
        slot.NameText.text = skill.displayName;
        slot.Icon.color = new Color(skill.effectColor.r, skill.effectColor.g, skill.effectColor.b, 0.9f);

        if (skill.cooldown <= 0f || skill.cooldownRemaining <= 0f)
        {
            slot.CooldownMask.anchorMin = new Vector2(0f, 1f);
            slot.CooldownText.text = string.Empty;
            slot.CooldownText.gameObject.SetActive(false);
            return;
        }

        var ratio = Mathf.Clamp01(skill.cooldownRemaining / skill.cooldown);
        slot.CooldownMask.anchorMin = new Vector2(0f, 1f - ratio);
        slot.CooldownMask.anchorMax = Vector2.one;
        slot.CooldownMask.offsetMin = Vector2.zero;
        slot.CooldownMask.offsetMax = Vector2.zero;
        slot.CooldownText.gameObject.SetActive(true);
        slot.CooldownText.text = skill.cooldownRemaining >= 1f
            ? Mathf.CeilToInt(skill.cooldownRemaining).ToString()
            : skill.cooldownRemaining.ToString("0.0");
    }

    private static SlotView CreateSlot(RectTransform parent, int index, Vector2 position, float size)
    {
        var root = new GameObject($"Skill Slot {index + 1}", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        root.transform.SetParent(parent, false);

        var rect = (RectTransform)root.transform;
        rect.anchorMin = new Vector2(0.5f, 0f);
        rect.anchorMax = new Vector2(0.5f, 0f);
        rect.pivot = new Vector2(0.5f, 0f);
        rect.anchoredPosition = position;
        rect.sizeDelta = new Vector2(size, size);

        var background = root.GetComponent<Image>();
        background.color = new Color(0.03f, 0.04f, 0.04f, 0.88f);

        var icon = CreateImage(rect, "Icon", new Color(0.8f, 0.8f, 0.8f, 0.9f));
        Stretch(icon.rectTransform, new Vector2(7f, 17f), new Vector2(-7f, -15f));

        var mask = CreateImage(rect, "Cooldown Mask", new Color(0f, 0f, 0f, 0.68f));
        Stretch(mask.rectTransform, Vector2.zero, Vector2.zero);

        var keyText = CreateText(rect, "Key", "", 17f, FontStyles.Bold, new Vector2(0f, -4f), new Vector2(size, 24f));
        keyText.alignment = TextAlignmentOptions.Top;

        var cooldownText = CreateText(rect, "Cooldown", "", 23f, FontStyles.Bold, new Vector2(0f, -26f), new Vector2(size, 40f));
        cooldownText.alignment = TextAlignmentOptions.Center;

        var nameText = CreateText(rect, "Name", "", 11f, FontStyles.Normal, new Vector2(0f, -68f), new Vector2(size, 20f));
        nameText.alignment = TextAlignmentOptions.Center;
        nameText.enableWordWrapping = false;

        return new SlotView
        {
            Root = root,
            Icon = icon,
            CooldownMask = mask.rectTransform,
            CooldownText = cooldownText,
            KeyText = keyText,
            NameText = nameText
        };
    }

    private static Canvas CreateHudCanvas()
    {
        var existing = GameObject.Find("RPG Demo HUD");
        if (existing != null && existing.TryGetComponent<Canvas>(out var existingCanvas))
            return existingCanvas;

        var canvasGo = new GameObject("RPG Demo HUD", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
        var canvas = canvasGo.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 100;

        var scaler = canvasGo.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920f, 1080f);
        scaler.matchWidthOrHeight = 0.5f;

        return canvas;
    }

    private static Image CreateImage(RectTransform parent, string name, Color color)
    {
        var go = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        go.transform.SetParent(parent, false);
        var image = go.GetComponent<Image>();
        image.color = color;
        return image;
    }

    private static TextMeshProUGUI CreateText(RectTransform parent, string name, string textValue, float fontSize, FontStyles style, Vector2 position, Vector2 size)
    {
        var go = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(TextMeshProUGUI));
        go.transform.SetParent(parent, false);

        var rect = (RectTransform)go.transform;
        rect.anchorMin = new Vector2(0.5f, 1f);
        rect.anchorMax = new Vector2(0.5f, 1f);
        rect.pivot = new Vector2(0.5f, 1f);
        rect.anchoredPosition = position;
        rect.sizeDelta = size;

        var text = go.GetComponent<TextMeshProUGUI>();
        text.text = textValue;
        text.fontSize = fontSize;
        text.fontStyle = style;
        text.color = Color.white;
        text.alignment = TextAlignmentOptions.Center;
        text.enableWordWrapping = true;
        return text;
    }

    private static void Stretch(RectTransform rect, Vector2 offsetMin, Vector2 offsetMax)
    {
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = offsetMin;
        rect.offsetMax = offsetMax;
    }

    private static string BindingLabel(string binding)
    {
        if (string.IsNullOrWhiteSpace(binding))
            return "-";

        var slashIndex = binding.LastIndexOf('/');
        var label = slashIndex >= 0 ? binding.Substring(slashIndex + 1) : binding;
        return label.Replace("leftButton", "LMB").ToUpperInvariant();
    }

    private struct SlotView
    {
        public GameObject Root;
        public Image Icon;
        public RectTransform CooldownMask;
        public TextMeshProUGUI CooldownText;
        public TextMeshProUGUI KeyText;
        public TextMeshProUGUI NameText;
    }
}
