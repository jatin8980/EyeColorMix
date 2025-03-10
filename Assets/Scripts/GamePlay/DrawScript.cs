using DG.Tweening;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DrawScript : MonoBehaviour
{
    [SerializeField] private Material blitMaterial, lineBlitMaterial, fillWithColorMat, replaceColorMat;
    [SerializeField] private RenderTexture renderTexture;
    [SerializeField] private RectTransform bottomLeftRef, borderRT, colliderPointsParentRT, pencilColliderRT;
    internal List<Color32> colorsToShowInGallery = new() { Color.white, Color.white, Color.white, Color.white };
    internal List<RenderTexture> renderTextures = new();
    internal Color32 alreadyDrawColor = new(255, 255, 255, 0);
    private Color32 toReplaceColor;
    private float diameter = 0.1f, vibrateTime;
    private RenderTexture bufferTexture;
    private Vector2 lastMPos;
    private bool isDownForDrawing, isPlayingBrushSound, turnOffBrushInvoked;
    private RawImage currentImage;

    private void OnEnable()
    {
        if (renderTextures.Count == 0)
        {
            fillWithColorMat.SetColor("_FillColor", alreadyDrawColor);
            bufferTexture = RenderTexture.GetTemporary(renderTexture.width, renderTexture.height);
            Graphics.Blit(bufferTexture, renderTexture, fillWithColorMat);
            foreach (Transform tr in transform.parent)
            {
                tr.localScale = Vector3.one * GameManager.Inst.gamePlayUi.smallScale;
            }
            MakeCollisionPoints();
        }
        renderTextures.Clear();
        borderRT.gameObject.SetActive(true);
        for (int i = 0; i < 4; i++)
        {
            RenderTexture renderTex = new RenderTexture(renderTexture);
            renderTextures.Add(renderTex);
            transform.GetChild(i).GetChild(0).GetComponent<RawImage>().texture = renderTex;
            transform.GetChild(i).gameObject.SetActive(false);
            Graphics.Blit(renderTexture, renderTex);
        }
        GameManager.Inst.gamePlayUi.NextBtnActiveSelf(false);
        pencilColliderRT.gameObject.SetActive(false);
        GameController.Inst.currentCircleIndex = 0;
        On_Circle_Change();
        toReplaceColor = alreadyDrawColor;
        vibrateTime = 0;
    }

    private void UpdateTexture(Material blitMaterial)
    {
        Graphics.Blit(renderTextures[GameController.Inst.currentCircleIndex], bufferTexture, blitMaterial);
        Graphics.Blit(bufferTexture, renderTextures[GameController.Inst.currentCircleIndex]);
    }

    private void MakeCollisionPoints()
    {
        Transform collisionPointPrefabTr = Resources.Load<Transform>("Prefabs/Gameplay/ColliderPointForColorPercentage");
        Vector3[] worldCorners = new Vector3[4];
        GetComponent<RectTransform>().GetWorldCorners(worldCorners);
        float spacing = colliderPointsParentRT.localScale.x * 0.2500f;
        int loops = Mathf.CeilToInt(Mathf.Abs(worldCorners[0].x - worldCorners[3].x) / spacing);
        for (int y = 0; y <= loops; y++)
        {
            for (int x = 0; x <= loops; x++)
            {
                Transform tr = Instantiate(collisionPointPrefabTr, colliderPointsParentRT);
                tr.position = new Vector3(worldCorners[0].x + (x * spacing), worldCorners[0].y + (y * spacing), 90);
            }
        }
    }

    private void Stamp(Vector2 p)
    {
        blitMaterial.SetVector("_TexturePos", new Vector4(p.x, p.y, 0, 0));
        UpdateTexture(blitMaterial);
    }

    private void Stamp(Vector2 p, Vector2 p2)
    {
        lineBlitMaterial.SetVector("_TexturePos", new Vector4(p.x, p.y, 0, 0));
        lineBlitMaterial.SetVector("_TexturePos2", new Vector4(p2.x, p2.y, 0, 0));
        UpdateTexture(lineBlitMaterial);
    }

    private void Update()
    {
        vibrateTime += Time.deltaTime;
        if (Input.GetMouseButtonDown(0) && !GameManager.Inst.gamePlayUi.colorSelector.isFingerOnColorChooseImage && isDownForDrawing)
        {
            DrawStart();
            return;
        }

        if (Input.GetMouseButton(0) && !GameManager.Inst.gamePlayUi.colorSelector.isFingerOnColorChooseImage && isDownForDrawing)
        {
            GameManager.Inst.gamePlayUi.pencilController.targetPos =
                Camera.main.ScreenToWorldPoint(new(Input.mousePosition.x, Input.mousePosition.y));

            GameManager.Inst.gamePlayUi.pencilController.targetPos.z = 90;


            bottomLeftRef.position = pencilColliderRT.position = GameManager.Inst.gamePlayUi.pencilController.smoothTargetPos;
            if (!GetPixelByMousePosition(out Vector2 texturePos)) return;

            if (texturePos == lastMPos)
            {
                if (!turnOffBrushInvoked)
                {
                    turnOffBrushInvoked = true;
                    Invoke(nameof(StopBrushSound), 0.15f);
                }
                return;
            }
            else
            {
                turnOffBrushInvoked = false;
                CancelInvoke(nameof(StopBrushSound));
                PlayBrushSound();
            }

            if (isDownForDrawing)
            {
                var distance = Vector2.Distance(texturePos, lastMPos);
                var f = diameter * 0.15f;
                if (distance >= f)
                {
                    Stamp(lastMPos, texturePos);
                }
                else
                {
                    Stamp(new(texturePos.x, texturePos.y));
                }
            }
            lastMPos = texturePos;
        }

        if (Input.GetMouseButtonUp(0))
        {
            isDownForDrawing = false;
            if (GameManager.Inst.gamePlayUi.colorSelector.isColorChoosed)
            {
                turnOffBrushInvoked = false;
                CancelInvoke(nameof(StopBrushSound));
                StopBrushSound();
                GameManager.Inst.gamePlayUi.pencilController.MovePencilToDefaultPos();
                if (borderRT.gameObject.activeSelf && (float)GameController.Inst.collidedCount / GameController.Inst.totalCollidersToCount > 0.85f)
                {
                    GameManager.Inst.canShowClickParticle = false;
                    FillRemainingColorWithSelectedColor();
                    if (GameController.Inst.currentCircleIndex < 3)
                        GameController.Inst.stretchController.AssignRenderTextureUvsToMeshRenderer(renderTextures[GameController.Inst.currentCircleIndex], GameController.Inst.currentCircleIndex);
                    SoundManager.Inst.Play("Step");
                    On_CircleFillComplete();
                    if (UserTutorialController.Inst != null && GeneralDataManager.TutorialStep == 2)
                    {
                        GameManager.Inst.HidePopUp(UserTutorialController.Inst.gameObject);
                        GeneralDataManager.TutorialStep++;// to 3
                        GameManager.Inst.Show_Popup(GameManager.Popups.TutorialPopUp);
                    }
                }
            }
            else
            {
                GameManager.Inst.gamePlayUi.pencilController.targetPos = GameManager.Inst.gamePlayUi.colorSelector.transform.position;
                GameManager.Inst.gamePlayUi.pencilController.MovePencilToTargetPos();
            }

        }
    }

    private bool GetPixelByMousePosition(out Vector2 texturePos)
    {
        Vector2 localCursor;
        var iRect = currentImage.GetComponent<RectTransform>();
        localCursor = bottomLeftRef.anchoredPosition;
        texturePos = localCursor / iRect.rect.size;
        return true;
    }

    private void FillRemainingColorWithSelectedColor()
    {
        replaceColorMat.SetTexture("_MainTex", renderTextures[GameController.Inst.currentCircleIndex]);
        replaceColorMat.SetColor("_ToReplaceColor", toReplaceColor);
        replaceColorMat.SetColor("_NewColor", GameManager.Inst.gamePlayUi.colorSelector.GetSelectedColor());
        replaceColorMat.SetFloat("_Tolerance", 0.01f);
        RenderTexture bufferTex = new(renderTextures[GameController.Inst.currentCircleIndex]);
        Graphics.Blit(renderTextures[GameController.Inst.currentCircleIndex], bufferTex, replaceColorMat);
        Graphics.Blit(bufferTex, renderTextures[GameController.Inst.currentCircleIndex]);
    }

    private void SetCurrentColorToShowInGalleryList()
    {
        Color cl = GameManager.Inst.gamePlayUi.colorSelector.GetSelectedColor();
        if (cl.CompareRGB(alreadyDrawColor))
            return;

        colorsToShowInGallery[GameController.Inst.currentCircleIndex] = GameManager.Inst.gamePlayUi.colorSelector.GetSelectedColor();
    }

    private void PlayBrushSound()
    {
        if (!isPlayingBrushSound)
        {
            SoundManager.Inst.Play("Brush", true);
            isPlayingBrushSound = true;
        }
    }

    private void StopBrushSound()
    {
        if (isPlayingBrushSound)
        {
            SoundManager.Inst.Stop("Brush");
            isPlayingBrushSound = false;
        }
    }

    private void DrawStart()
    {
        if (GameManager.Inst.gamePlayUi.colorSelector.isColorChoosed && borderRT.gameObject.activeSelf)
            pencilColliderRT.gameObject.SetActive(true);

        Vector3 mouseWorldPos =
            Camera.main.ScreenToWorldPoint(new(Input.mousePosition.x, Input.mousePosition.y));
        mouseWorldPos.z = 90;

        GameManager.Inst.gamePlayUi.pencilController.smoothTargetPos =
            GameManager.Inst.gamePlayUi.pencilController.targetPos =
            bottomLeftRef.position = mouseWorldPos;

        if (!GetPixelByMousePosition(out Vector2 texturePos)) return;
        lastMPos = texturePos;
        if (isDownForDrawing)
        {
            blitMaterial.SetColor("_LineColor", GameManager.Inst.gamePlayUi.colorSelector.GetSelectedColor());
            lineBlitMaterial.SetColor("_LineColor", GameManager.Inst.gamePlayUi.colorSelector.GetSelectedColor());
            Stamp(new(texturePos.x, texturePos.y));
            SetCurrentColorToShowInGalleryList();
        }
    }

    private void On_CircleFillComplete()
    {
        if (GameController.Inst.currentCircleIndex == 3)
        {
            GameManager.Inst.gamePlayUi.NextBtnActiveSelf(true);
            borderRT.gameObject.SetActive(false);
            pencilColliderRT.gameObject.SetActive(false);
            if (UserTutorialController.Inst != null)
            {
                GameManager.Inst.HidePopUp(UserTutorialController.Inst.gameObject);
                GeneralDataManager.TutorialStep++;// to 4
                GameManager.Inst.Show_Popup(GameManager.Popups.TutorialPopUp);
            }
        }
        else
        {
            GameController.Inst.currentCircleIndex++;
            On_Circle_Change();
        }
    }

    internal void Vibrate()
    {
        if (vibrateTime > 0.15f)
        {
            SoundManager.Inst.LightVibrate();
            vibrateTime = 0;
        }
    }

    internal void On_Circle_Change(bool isFromUndo = false)
    {
        switch (GameController.Inst.currentCircleIndex)
        {
            case 0:
                borderRT.DOSizeDelta(new(864, 864), 0.5f).SetEase(Ease.OutBack);
                break;
            case 1:
                borderRT.DOSizeDelta(new(648, 648), 0.5f).SetEase(Ease.OutBack);
                break;
            case 2:
                borderRT.DOSizeDelta(new(432, 432), 0.5f).SetEase(Ease.OutBack);
                break;
            case 3:
                borderRT.DOSizeDelta(new(216, 216), 0.5f).SetEase(Ease.OutBack);
                break;
        }
        GameManager.Inst.gamePlayUi.UndoBtnSetActive(GameController.Inst.currentCircleIndex > 0 && UserTutorialController.Inst == null);
        borderRT.gameObject.SetActive(true);

        currentImage = transform.GetChild(GameController.Inst.currentCircleIndex).GetChild(0).GetComponent<RawImage>();
        RectTransform currentImageRT = currentImage.GetComponent<RectTransform>();
        bottomLeftRef.SetParent(currentImage.transform);
        currentImage.transform.parent.gameObject.SetActive(true);
        diameter = Mathf.Max(864 / currentImageRT.rect.width * 0.1f, 0.1f);
        blitMaterial.SetFloat("_LineWidth", diameter / 2f);
        lineBlitMaterial.SetFloat("_LineWidth", diameter / 2f);

        Vector3[] worldPos = new Vector3[4];
        currentImageRT.GetWorldCorners(worldPos);
        GameController.Inst.collidedCount = 0;
        GameController.Inst.totalCollidersToCount = 0;
        float distanceLimit = 3.979167f * (currentImageRT.rect.width / 764) * GameManager.Inst.gamePlayUi.smallScale / 2f; //3.979167f is distance for 864 width.
        foreach (Transform tr in colliderPointsParentRT)
        {
            if (Vector3.Distance(transform.position, tr.position) <= distanceLimit)
            {
                tr.gameObject.SetActive(true);
                GameController.Inst.totalCollidersToCount++;
            }
            else
            {
                tr.gameObject.SetActive(false);
            }
        }

        if (isFromUndo)
        {
            toReplaceColor = colorsToShowInGallery[GameController.Inst.currentCircleIndex];
            transform.GetChild(GameController.Inst.currentCircleIndex + 1).GetChild(0).transform.parent.gameObject.SetActive(false);
            Graphics.Blit(renderTexture, renderTextures[GameController.Inst.currentCircleIndex + 1]);

            fillWithColorMat.SetColor("_FillColor", toReplaceColor);
            Graphics.Blit(renderTexture, renderTextures[GameController.Inst.currentCircleIndex], fillWithColorMat);
        }
        else
        {
            toReplaceColor = alreadyDrawColor;
        }
    }

    public void On_Pointer_Enter()
    {
        if ((!(Input.GetMouseButtonDown(0) || Input.GetMouseButton(0))) ||
           GameManager.Inst.gamePlayUi.colorSelector.isFingerOnColorChooseImage)
        {
            return;
        }
        isDownForDrawing = true;
        GameManager.Inst.gamePlayUi.pencilController.MovePencilToTargetPos();
        DrawStart();
    }

    public void On_Pointer_Exit()
    {
        if (GameManager.Inst.gamePlayUi.colorSelector.isFingerOnColorChooseImage || !Input.GetMouseButton(0))
            return;
        isDownForDrawing = false;
        turnOffBrushInvoked = false;
        CancelInvoke(nameof(StopBrushSound));
        StopBrushSound();
        GameManager.Inst.gamePlayUi.pencilController.MovePencilToDefaultPos();
    }
}