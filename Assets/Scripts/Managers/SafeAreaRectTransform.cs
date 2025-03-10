using UnityEngine;

public class SafeAreaRectTransform : MonoBehaviour
{
    private RectTransform _panel;
    private Rect _lastSafeArea = new(0, 0, 0, 0);

    private void Awake()
    {
        _panel = GetComponent<RectTransform>();
        Refresh();
    }

    private void Update()
    {
        Refresh();
    }

    private void Refresh()
    {
        var safeArea = GetSafeArea();

        if (safeArea != _lastSafeArea)
            ApplySafeArea(safeArea);
    }

    private static Rect GetSafeArea()
    {
        return Screen.safeArea;
    }

    private void ApplySafeArea(Rect r)
    {
        _lastSafeArea = r;

        var anchorMin = r.position;
        var anchorMax = r.position + r.size;
        anchorMin.x /= Screen.width;
        anchorMin.y /= Screen.height;
        anchorMax.x /= Screen.width;
        anchorMax.y /= Screen.height;
        _panel.anchorMin = anchorMin;
        _panel.anchorMax = anchorMax;
    }
}