using UnityEngine;

[System.Serializable]
public class ParallaxBackgroundLayer
{
    [SerializeField]
    private Transform bg;

    [SerializeField]
    private float parallaxMultiplier = 0.5f;

    private Vector2 _startPos;
    private float _bgWidth;
    [SerializeField]
    private float bgOffsetX = 10f;

    public void Init()
    {
        if (!bg) return;

        _bgWidth = bg.GetComponentInChildren<SpriteRenderer>().bounds.size.x;
        _startPos = bg.position;
    }

    public void UpdateLayer(float deltaX, float cameraX, float cameraHalfWidth)
    {
        // 1.视差位置更新
        bg.position += Vector3.right * (deltaX * parallaxMultiplier);

        // 2.背景循环更新
        BackgroundLoop(cameraX, cameraHalfWidth);
    }

    private void BackgroundLoop(float cameraX, float cameraHalfWidth)
    {
        var bgHalfWidth = _bgWidth * .5f;

        var bgLeftEdge = bg.position.x - bgHalfWidth;
        var bgRightEdge = bg.position.x + bgHalfWidth;

        var cameraLeftEdge = cameraX - cameraHalfWidth;
        var cameraRightEdge = cameraX + cameraHalfWidth;
        
        if (bgRightEdge - bgOffsetX < cameraLeftEdge) // 背景在屏幕左侧完全离开
            bg.position += Vector3.right * _bgWidth;
        else if (bgLeftEdge + bgOffsetX > cameraRightEdge) // 背景在屏幕右侧完全离开
            bg.position += Vector3.right * (-_bgWidth);
    }
}