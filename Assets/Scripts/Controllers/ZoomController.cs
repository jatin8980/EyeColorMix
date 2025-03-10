using UnityEngine;

public class ZoomController : MonoBehaviour
{
    private CropController cropController;
    [SerializeField] private RectTransform thisRt;
    private Camera mainCamera;
    private float zoomOutMin, zoomOutMax, width, height, defaultActualCropSize = 0;
    private Vector2 preClickPos, pointerPos;
    private RectTransform parentRt;
    private bool isTwoFingers, isVerticalRotation = true;
    internal bool canZoom;

    private void Start()
    {
        mainCamera = Camera.main;
    }

    internal void SetData(float wid, float hei, CropController cropController)
    {
        width = wid;
        height = hei;
        defaultActualCropSize = cropController.actualCropSize;
        zoomOutMin = width;
        zoomOutMax = width * 2;
        parentRt = transform.parent.GetComponent<RectTransform>();
        canZoom = true;
        this.cropController = cropController;
    }

    internal void Rotate()
    {
        isVerticalRotation = !isVerticalRotation;
        ClampPos(thisRt.anchoredPosition);
    }

    private void Update()
    {
        if (!canZoom) return;

        if (Input.touchCount == 0)
        {
#if !UNITY_EDITOR
            return;
#endif
        }

        if (Input.GetMouseButtonDown(0))
        {
            preClickPos = GetDragPos(Input.mousePosition) - thisRt.anchoredPosition;
        }

        if (Input.touchCount == 2)
        {
            isTwoFingers = true;
            var touchZero = Input.GetTouch(0);
            var touchOne = Input.GetTouch(1);

            var touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
            var touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

            var prevMagnitude = (touchZeroPrevPos - touchOnePrevPos).magnitude;
            var currentMagnitude = (touchZero.position - touchOne.position).magnitude;

            var difference = currentMagnitude - prevMagnitude;
            if(difference * 0.001f!=0)
            {
                Zoom(difference * 0.001f);
            }
            Vector2 touchZeroDeltaPos = GetDragPos(touchZero.position) - GetDragPos(touchZeroPrevPos);
            Vector2 touchOneDeltaPos = GetDragPos(touchOne.position) - GetDragPos(touchOnePrevPos);
            ClampPos(thisRt.anchoredPosition + ((touchZeroDeltaPos + touchOneDeltaPos) / 2f));
            return;
        }

        if (isTwoFingers)
        {
            preClickPos = GetDragPos(Input.mousePosition) - thisRt.anchoredPosition;
            isTwoFingers = false;
        }

        if (Input.GetMouseButton(0))
        {
            ClampPos(GetDragPos(Input.mousePosition) - preClickPos);
        }

#if UNITY_EDITOR
        Zoom(Input.GetAxis("Mouse ScrollWheel"));
        ClampPos(thisRt.anchoredPosition);
#endif
    }

    private void Zoom(float increment)
    {
        var factor = Mathf.Clamp(thisRt.sizeDelta.x + (thisRt.sizeDelta.x * increment), zoomOutMin, zoomOutMax);
        thisRt.sizeDelta = new Vector2(factor, Mathf.FloorToInt(factor * (height / width)));
        cropController.actualCropSize = Mathf.FloorToInt(defaultActualCropSize * (width / thisRt.sizeDelta.x));
        cropController.CalculateMultiplier();
    }

    private void ClampPos(Vector3 pos)
    {
        var x = Mathf.Abs(thisRt.sizeDelta.x - parentRt.rect.width) / 2;
        var y = Mathf.Abs(thisRt.sizeDelta.y - parentRt.rect.height) / 2;
        if (isVerticalRotation)
        {
            pos.x = Mathf.Clamp(pos.x, -x, x);
            pos.y = Mathf.Clamp(pos.y, -y, y);
        }
        else
        {
            pos.x = Mathf.Clamp(pos.x, -y, y);
            pos.y = Mathf.Clamp(pos.y, -x, x);
        }
        thisRt.anchoredPosition = pos;
    }

    private Vector2 GetDragPos(Vector2 mousePos)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(parentRt, mousePos,
            mainCamera, out pointerPos);
        return pointerPos;
    }
}