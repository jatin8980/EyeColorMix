using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PencilController : MonoBehaviour
{
    public RectTransform pencilRT;
    internal Vector3 targetPos, smoothTargetPos;
    private Coroutine moveCoroutine;
    private List<Vector3> defaultPoses = new();
    private bool isMovingDone;
    private RectTransform thisRT;
    private void Awake()
    {
        thisRT = GetComponent<RectTransform>();
    }

    private void Update()
    {
        if (smoothTargetPos != targetPos)
        {
            smoothTargetPos = Vector3.MoveTowards(smoothTargetPos, targetPos, 15f * Time.deltaTime);
        }
        if (isMovingDone)
        {
            pencilRT.position = smoothTargetPos;
            SetRotation();
        }
    }

    private void SetRotation()
    {
        Vector3 direction = transform.position - pencilRT.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        pencilRT.rotation = Quaternion.Euler(0f, 0f, angle + 90);
    }

    private IEnumerator MoveDefaultCoroutine()
    {
        while (pencilRT.position != defaultPoses[GameController.Inst.currentCircleIndex])
        {
            pencilRT.position = Vector3.MoveTowards(pencilRT.position, defaultPoses[GameController.Inst.currentCircleIndex], 20f * Time.deltaTime);
            SetRotation();
            yield return null;
        }
    }

    private IEnumerator MoveToTargetCoroutine(float duration)
    {
        float time = 0;
        thisRT.DOKill();
        Vector3 startPos = pencilRT.position;
        while (time < duration)
        {
            time += Time.deltaTime;
            thisRT.anchoredPosition = new Vector2(Mathf.Lerp(thisRT.anchoredPosition.x, -152, time / duration), Mathf.Lerp(thisRT.anchoredPosition.y, 190, time / duration));
            pencilRT.position = Vector3.Lerp(startPos, smoothTargetPos, time / duration);
            SetRotation();
            yield return null;
        }
        isMovingDone = true;
    }

    internal void SetDefaultPoses()
    {
        Vector3[] worldCorners = new Vector3[4];
        for (int i = 0; i < 4; i++)
        {
            GameManager.Inst.gamePlayUi.drawController.transform.GetChild(i).GetComponent<RectTransform>().GetWorldCorners(worldCorners);
            defaultPoses.Add(Vector3.Lerp(GameManager.Inst.gamePlayUi.drawController.transform.GetChild(i).position, worldCorners[1], 0.55f));
        }


        pencilRT.position = targetPos = smoothTargetPos = defaultPoses[GameController.Inst.currentCircleIndex];
        SetRotation();
    }

    internal void MovePencilToDefaultPos()
    {
        if (moveCoroutine != null)
            StopCoroutine(moveCoroutine);
        thisRT.DOKill();
        thisRT.DOAnchorPos(new Vector2(-72, 527), 0.2f);
        moveCoroutine = StartCoroutine(MoveDefaultCoroutine());
        isMovingDone = false;
    }

    internal void MovePencilToTargetPos()
    {
        if (moveCoroutine != null)
            StopCoroutine(moveCoroutine);
        moveCoroutine = StartCoroutine(MoveToTargetCoroutine(0.2f));
        isMovingDone = false;
    }

    internal void SetTheme(int penIndex)
    {
        pencilRT.GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/Pens/PenThemes/" + penIndex);
    }
}