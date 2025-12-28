using UnityEngine;

public class ParallaxBackground : MonoBehaviour
{
    [SerializeField]
    private ParallaxBackgroundLayer[] layers;

    private Camera _mainCamera;
    private float _lastCameraX;

    private void Awake()
    {
        _mainCamera = Camera.main;

        foreach (var layer in layers)
        {
            layer.Init();
        }
    }

    private void LateUpdate()
    {
        if (!_mainCamera) return;

        var cameraX = _mainCamera.transform.position.x;
        var cameraHalfWidth = _mainCamera.orthographicSize * _mainCamera.aspect;

        var deltaX = cameraX - _lastCameraX;
        
        _lastCameraX = cameraX;
        
        foreach (var layer in layers)
            layer.UpdateLayer(deltaX, cameraX,cameraHalfWidth);
    }
}