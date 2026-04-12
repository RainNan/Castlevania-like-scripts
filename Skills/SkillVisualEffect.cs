using UnityEngine;

public class SkillVisualEffect : MonoBehaviour
{
    private static Sprite circleSprite;

    private SpriteRenderer spriteRenderer;
    private float duration = 0.25f;
    private float timer;
    private Vector3 startScale;
    private Color startColor;

    public static void Spawn(Vector3 position, float radius, Color color, float lifeTime = 0.25f)
    {
        var go = new GameObject("Skill Visual Effect");
        go.transform.position = new Vector3(position.x, position.y, position.z - 0.05f);
        go.transform.localScale = Vector3.one * Mathf.Max(0.05f, radius * 2f);

        var effect = go.AddComponent<SkillVisualEffect>();
        effect.duration = Mathf.Max(0.05f, lifeTime);
        effect.spriteRenderer = go.AddComponent<SpriteRenderer>();
        effect.spriteRenderer.sprite = GetCircleSprite();
        effect.spriteRenderer.color = color;
        effect.spriteRenderer.sortingOrder = 20;
    }

    private void Awake()
    {
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        startScale = transform.localScale;
        startColor = spriteRenderer != null ? spriteRenderer.color : Color.white;
    }

    private void Update()
    {
        timer += Time.deltaTime;
        var t = Mathf.Clamp01(timer / duration);
        transform.localScale = Vector3.Lerp(startScale, startScale * 1.35f, t);

        if (spriteRenderer != null)
        {
            var color = startColor;
            color.a = Mathf.Lerp(startColor.a, 0f, t);
            spriteRenderer.color = color;
        }

        if (timer >= duration)
            Destroy(gameObject);
    }

    private static Sprite GetCircleSprite()
    {
        if (circleSprite != null)
            return circleSprite;

        const int size = 64;
        var texture = new Texture2D(size, size, TextureFormat.RGBA32, false)
        {
            filterMode = FilterMode.Bilinear,
            wrapMode = TextureWrapMode.Clamp
        };

        var center = new Vector2((size - 1) * 0.5f, (size - 1) * 0.5f);
        var radius = size * 0.48f;
        for (var y = 0; y < size; y++)
        {
            for (var x = 0; x < size; x++)
            {
                var distance = Vector2.Distance(new Vector2(x, y), center);
                var alpha = distance <= radius ? Mathf.Clamp01(1f - distance / radius) : 0f;
                texture.SetPixel(x, y, new Color(1f, 1f, 1f, alpha));
            }
        }

        texture.Apply();
        circleSprite = Sprite.Create(texture, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), size);
        circleSprite.name = "Runtime Skill Circle";
        return circleSprite;
    }
}
