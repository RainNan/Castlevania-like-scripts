using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStatsPanel : MonoBehaviour
{
    [SerializeField]
    private Player player;
    [SerializeField]
    private PlayerSkillController skillController;
    [SerializeField]
    private Slider hpSlider;
    [SerializeField]
    private TextMeshProUGUI hpText;
    [SerializeField]
    private TextMeshProUGUI statsText;
    private readonly StringBuilder stringBuilder = new StringBuilder(256);

    public static PlayerStatsPanel CreateRuntime(Player targetPlayer, PlayerSkillController targetSkills)
    {
        var canvas = CreateHudCanvas();
        var panelGo = new GameObject("Player Stats Panel", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        panelGo.transform.SetParent(canvas.transform, false);

        var rect = (RectTransform)panelGo.transform;
        rect.anchorMin = new Vector2(0f, 1f);
        rect.anchorMax = new Vector2(0f, 1f);
        rect.pivot = new Vector2(0f, 1f);
        rect.anchoredPosition = new Vector2(18f, -18f);
        rect.sizeDelta = new Vector2(440f, 220f);

        var image = panelGo.GetComponent<Image>();
        image.color = new Color(0.04f, 0.05f, 0.06f, 0.86f);

        var panel = panelGo.AddComponent<PlayerStatsPanel>();
        panel.player = targetPlayer;
        panel.skillController = targetSkills;
        panel.BuildDefaultContent(rect);
        panel.Refresh();
        return panel;
    }

    private void Start()
    {
        ResolveReferences();
        var rect = transform as RectTransform;
        if (hpSlider == null && rect != null)
            BuildDefaultContent(rect);

        Refresh();
    }

    private void Update()
    {
        ResolveReferences();
        Refresh();
    }

    private void ResolveReferences()
    {
        if (player == null)
            player = FindObjectOfType<Player>();

        if (skillController == null && player != null)
            skillController = player.GetComponent<PlayerSkillController>();
    }

    private void Refresh()
    {
        if (player == null || hpSlider == null)
            return;

        hpSlider.maxValue = player.MaxHp;
        hpSlider.value = player.Hp;

        if (hpText != null)
            hpText.text = $"HP {Mathf.CeilToInt(player.Hp)} / {Mathf.CeilToInt(player.MaxHp)}";

        if (statsText != null)
        {
            var velocity = player.rb != null ? player.rb.velocity : Vector2.zero;
            stringBuilder.Clear();
            stringBuilder.AppendLine($"ATK      {player.Atk:0}");
            stringBuilder.AppendLine($"VEL      {velocity.x:0.00}, {velocity.y:0.00}");
            statsText.text = stringBuilder.ToString().TrimEnd();
        }
    }

    private void BuildDefaultContent(RectTransform parent)
    {
        CreateText(parent, "Title", "PLAYER", 30f, FontStyles.Bold, new Vector2(22f, -16f), new Vector2(396f, 40f));
        hpSlider = CreateSlider(parent, "HP Slider", new Vector2(22f, -66f), new Vector2(396f, 26f));
        hpText = CreateText(parent, "HP Text", "HP", 24f, FontStyles.Bold, new Vector2(22f, -102f), new Vector2(396f, 32f));
        statsText = CreateText(parent, "Stats Text", "", 24f, FontStyles.Normal, new Vector2(22f, -142f), new Vector2(396f, 64f));
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

    private static TextMeshProUGUI CreateText(RectTransform parent, string name, string textValue, float fontSize, FontStyles style, Vector2 position, Vector2 size)
    {
        var go = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(TextMeshProUGUI));
        go.transform.SetParent(parent, false);

        var rect = (RectTransform)go.transform;
        rect.anchorMin = new Vector2(0f, 1f);
        rect.anchorMax = new Vector2(0f, 1f);
        rect.pivot = new Vector2(0f, 1f);
        rect.anchoredPosition = position;
        rect.sizeDelta = size;

        var text = go.GetComponent<TextMeshProUGUI>();
        text.text = textValue;
        text.fontSize = fontSize;
        text.fontStyle = style;
        text.color = Color.white;
        text.alignment = TextAlignmentOptions.Left;
        text.enableWordWrapping = false;
        text.overflowMode = TextOverflowModes.Overflow;
        return text;
    }

    private static Slider CreateSlider(RectTransform parent, string name, Vector2 position, Vector2 size)
    {
        var go = new GameObject(name, typeof(RectTransform), typeof(Slider));
        go.transform.SetParent(parent, false);

        var rect = (RectTransform)go.transform;
        rect.anchorMin = new Vector2(0f, 1f);
        rect.anchorMax = new Vector2(0f, 1f);
        rect.pivot = new Vector2(0f, 1f);
        rect.anchoredPosition = position;
        rect.sizeDelta = size;

        var background = CreateImage(rect, "Background", new Color(0f, 0f, 0f, 0.65f));
        Stretch(background.rectTransform, Vector2.zero, Vector2.zero);

        var fillArea = new GameObject("Fill Area", typeof(RectTransform));
        fillArea.transform.SetParent(rect, false);
        Stretch((RectTransform)fillArea.transform, new Vector2(2f, 2f), new Vector2(-2f, -2f));

        var fill = CreateImage((RectTransform)fillArea.transform, "Fill", new Color(0.88f, 0.08f, 0.08f, 0.95f));
        Stretch(fill.rectTransform, Vector2.zero, Vector2.zero);

        var slider = go.GetComponent<Slider>();
        slider.transition = Selectable.Transition.None;
        slider.interactable = false;
        slider.minValue = 0f;
        slider.maxValue = 1f;
        slider.value = 1f;
        slider.fillRect = fill.rectTransform;
        slider.targetGraphic = fill;
        return slider;
    }

    private static Image CreateImage(RectTransform parent, string name, Color color)
    {
        var go = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        go.transform.SetParent(parent, false);
        var image = go.GetComponent<Image>();
        image.color = color;
        return image;
    }

    private static void Stretch(RectTransform rect, Vector2 offsetMin, Vector2 offsetMax)
    {
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = offsetMin;
        rect.offsetMax = offsetMax;
    }

}
