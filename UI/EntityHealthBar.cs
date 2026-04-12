using UnityEngine;
using UnityEngine.UI;

public class EntityHealthBar : MonoBehaviour
{
    [SerializeField]
    private Entity target;
    [SerializeField]
    private Vector3 worldOffset = new Vector3(0f, 1.2f, 0f);
    [SerializeField]
    private Slider slider;

    public static EntityHealthBar Create(Entity targetEntity)
    {
        var go = new GameObject($"{targetEntity.name} Health Bar", typeof(RectTransform), typeof(Canvas));
        var canvas = go.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.WorldSpace;
        canvas.overrideSorting = true;
        canvas.sortingOrder = 80;

        var rect = (RectTransform)go.transform;
        rect.sizeDelta = new Vector2(120f, 12f);
        rect.localScale = Vector3.one * 0.01f;

        var healthBar = go.AddComponent<EntityHealthBar>();
        healthBar.target = targetEntity;
        healthBar.slider = CreateSlider(rect);
        healthBar.Refresh();
        return healthBar;
    }

    private void LateUpdate()
    {
        if (target == null || target.IsDead)
        {
            Destroy(gameObject);
            return;
        }

        transform.position = target.transform.position + worldOffset;
        transform.rotation = Quaternion.identity;
        Refresh();
    }

    private void Refresh()
    {
        if (slider == null || target == null)
            return;

        slider.maxValue = target.MaxHp;
        slider.value = target.Hp;
    }

    private static Slider CreateSlider(RectTransform parent)
    {
        var sliderGo = new GameObject("Slider", typeof(RectTransform), typeof(Slider));
        sliderGo.transform.SetParent(parent, false);
        Stretch((RectTransform)sliderGo.transform, Vector2.zero, Vector2.zero);

        var background = CreateImage((RectTransform)sliderGo.transform, "Background", new Color(0f, 0f, 0f, 0.75f));
        Stretch(background.rectTransform, Vector2.zero, Vector2.zero);

        var fillAreaGo = new GameObject("Fill Area", typeof(RectTransform));
        fillAreaGo.transform.SetParent(sliderGo.transform, false);
        Stretch((RectTransform)fillAreaGo.transform, new Vector2(1f, 1f), new Vector2(-1f, -1f));

        var fill = CreateImage((RectTransform)fillAreaGo.transform, "Fill", new Color(0.9f, 0.05f, 0.05f, 0.95f));
        Stretch(fill.rectTransform, Vector2.zero, Vector2.zero);

        var slider = sliderGo.GetComponent<Slider>();
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
