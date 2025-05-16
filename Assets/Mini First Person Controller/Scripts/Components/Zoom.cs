using UnityEngine;

[ExecuteInEditMode]
public class Zoom : MonoBehaviour
{
    private Camera zoomCamera; // camera yerine zoomCamera dedik

    public float defaultFOV = 60;
    public float maxZoomFOV = 15;
    [Range(0, 1)]
    public float currentZoom;
    public float sensitivity = 1;

    void Awake()
    {
        // Kamerayı bu gameObject'ten al
        zoomCamera = GetComponent<Camera>();

        // Null değilse FOV değerini al
        if (zoomCamera != null)
        {
            defaultFOV = zoomCamera.fieldOfView;
        }
        else
        {
            Debug.LogError("Zoom.cs üzerindeki kamera bulunamadı! Bu script, bir Camera nesnesine eklenmeli.");
        }
    }
    

    void Update()
    {
        if (zoomCamera == null) return;

        currentZoom += Input.mouseScrollDelta.y * sensitivity * 0.05f;
        currentZoom = Mathf.Clamp01(currentZoom);
        zoomCamera.fieldOfView = Mathf.Lerp(defaultFOV, maxZoomFOV, currentZoom);
    }
}
