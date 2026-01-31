using UnityEngine;

public class CameraZoomController : MonoBehaviour
{
    [Header("Settings")]
    [Tooltip("The size/FOV you want the camera to reach.")]
    public float targetZoom = 5f;

    [Tooltip("How fast the zoom happens. Higher = Faster.")]
    public float zoomSpeed = 2f;

    private Camera cam;

    void Awake()
    {
        cam = GetComponent<Camera>();
        cam.orthographicSize = targetZoom;
       
    }

    void Update()
    {
        cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, targetZoom, zoomSpeed * Time.deltaTime);
    }

    // Call this function from your GameManager or triggers to change zoom
    public void SetZoom(float newZoomValue)
    {
        targetZoom = newZoomValue;
    }
}