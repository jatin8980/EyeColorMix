using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class StretchController : MonoBehaviour
{
    [SerializeField] private Transform stretchProgressPrefabTR, stretchProgressParent;
    [SerializeField] private Material copyUvsMat, combineTextureMat, blurMat;
    public MarkTRItem markPrefab;
    public Transform handTR, markTRParent;
    public List<MeshRenderer> circles;
    public List<Texture2D> circleTextures;
    internal List<MarkTRItem> markTRItems = new();
    private List<Mesh> circleMeshes = new();
    private List<float> circleRadius = new();
    private List<Vector3[]> currentVerticiesList = new();
    private Dictionary<int, Transform> stretchProgressInfos = new();
    private RenderTexture[] stretchedRenderTextures = new RenderTexture[4];
    private float defaultStretchProgressScale = 0.05f, vibrateTime = 0, stretchRandomRotate;
    private Vector3 targetForHand;
    private bool turnOffBrushInvoked, isPlayingBrushSound;

    private void Awake()
    {
        transform.GetChild(1).GetComponent<CircleMesh>().MakeCircle();
        transform.GetChild(2).GetComponent<CircleMesh>().MakeCircle();
        transform.GetChild(3).GetComponent<CircleMesh>().MakeCircle();
        transform.GetChild(4).GetComponent<CircleMesh>().MakeCircle();
        stretchRandomRotate = Random.Range(0, 360);
    }

    private void OnEnable()
    {
        GameController.Inst.beforeSRs[0].transform.parent.parent.DOKill();
        transform.DOKill();
        GameController.Inst.beforeSRs[0].transform.parent.parent.localScale = transform.localScale = new Vector3(2.25f, 2.25f, 2.25f) * GameManager.Inst.gamePlayUi.smallScale;
        turnOffBrushInvoked = false;
        vibrateTime = 0;
    }

    private void Start()
    {
        int index = -1;
        foreach (MeshRenderer meshRenderer in circles)
        {
            index++;
            if (index < 1)
                continue;
            circleMeshes.Add(meshRenderer.GetComponent<MeshFilter>().mesh);
            circleRadius.Add(meshRenderer.GetComponent<CircleMesh>().radius);
            currentVerticiesList.Add(new Vector3[circleMeshes[0].vertices.Length]);
        }

        for (int i = 1; i < currentVerticiesList[0].Length; i++)
        {
            currentVerticiesList[0][i] = Vector3.zero;
            currentVerticiesList[1][i] = Vector3.zero;
            currentVerticiesList[2][i] = Vector3.zero;
            markTRItems[i - 1].transform.localPosition = markTRItems[i - 1].realPos * 0.25f;
        }

        RefreshAllMeshNormals();

        Transform stretchProgressTR;
        for (int i = 0; i < markTRItems.Count; i++)
        {
            if (i % 7 == 0)
            {
                stretchProgressTR = Instantiate(stretchProgressPrefabTR, stretchProgressParent);
                stretchProgressTR.localPosition = markTRItems[i].realPos * 0.93f;
                stretchProgressTR.localScale = new(defaultStretchProgressScale, defaultStretchProgressScale, defaultStretchProgressScale);
                stretchProgressInfos.Add(i, stretchProgressTR);
            }
        }
        transform.position = GameController.Inst.beforeSRs[0].transform.parent.parent.position = targetForHand =
            new(GameManager.Inst.gamePlayUi.drawController.transform.position.x, GameManager.Inst.gamePlayUi.drawController.transform.position.y, 0);
        gameObject.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetMouseButton(0))
        {
            targetForHand = GameManager.Inst.mainCamera.ScreenToWorldPoint(Input.mousePosition);
            targetForHand.z = 0;
            GameManager.Inst.gamePlayUi.pencilController.targetPos = targetForHand;
            GameManager.Inst.gamePlayUi.pencilController.targetPos.z = 90;

            if (GameManager.Inst.gamePlayUi.pencilController.smoothTargetPos == GameManager.Inst.gamePlayUi.pencilController.targetPos)
            {
                if (!turnOffBrushInvoked)
                {
                    turnOffBrushInvoked = true;
                    Invoke(nameof(StopBrushSound), 0.15f);
                }
            }
            else
            {
                turnOffBrushInvoked = false;
                CancelInvoke(nameof(StopBrushSound));
                PlayBrushSound();
            }

            GameManager.Inst.gamePlayUi.pencilController.smoothTargetPos = GameManager.Inst.gamePlayUi.pencilController.targetPos;
            UpdateStepProgress();
        }

        if (Input.GetMouseButtonDown(0))
        {
            handTR.position = targetForHand;
            if (!GameManager.Inst.IsMouseOverAnyUi())
                GameManager.Inst.gamePlayUi.pencilController.MovePencilToTargetPos();
        }

        if (Input.GetMouseButtonUp(0))
        {
            int counter = 0;
            foreach (Transform t in stretchProgressInfos.Values)
            {
                if (t.localScale.x < defaultStretchProgressScale * 0.3f)
                {
                    counter++;
                }
            }
            if (counter > stretchProgressInfos.Count * 0.8 && handTR.gameObject.activeSelf)
            {
                GameManager.Inst.canShowClickParticle = false;
                StartCoroutine(StretchRemaining());
            }
            GameManager.Inst.gamePlayUi.pencilController.MovePencilToDefaultPos();
            turnOffBrushInvoked = false;
            CancelInvoke(nameof(StopBrushSound));
            StopBrushSound();
        }

        handTR.position = Vector3.MoveTowards(handTR.position, targetForHand, 10 * Time.deltaTime);
        vibrateTime += Time.deltaTime;
    }

    private void LateUpdate()
    {
        RefreshAllMeshNormals();
    }

    private void RefreshAllMeshNormals()
    {
        int index = 0;
        foreach (Mesh mesh in circleMeshes)
        {
            mesh.vertices = currentVerticiesList[index];
            mesh.RecalculateNormals();
            index++;
        }
    }

    private void UpdateStepProgress()
    {
        float totalScale = stretchProgressInfos.Count * defaultStretchProgressScale;
        float currentScale = stretchProgressInfos.Values.Select(tr => tr.localScale.x).Sum();
    }

    private void PlayBrushSound()
    {
        if (!isPlayingBrushSound && handTR.gameObject.activeSelf)
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

    internal IEnumerator StretchRemaining()
    {
        handTR.gameObject.SetActive(false);
        //GameManager.Inst.gamePlayUi.AutoStretchBtnSetActive(false);
        float speed = 1f;
        float time = 0;
        if (UserTutorialController.Inst != null)
        {
            GameManager.Inst.HidePopUp(UserTutorialController.Inst.gameObject);
            GeneralDataManager.TutorialStep++;//to 6
        }
        while (time < 1)
        {
            for (int i = 1; i < currentVerticiesList[0].Length; i++)
            {
                currentVerticiesList[0][i] = markTRItems[i - 1].realPos * Mathf.Min(circleRadius[0], (currentVerticiesList[0][i].x / markTRItems[i - 1].realPos.x) + speed * Time.deltaTime);
                currentVerticiesList[1][i] = markTRItems[i - 1].realPos * Mathf.Min(circleRadius[1], (currentVerticiesList[1][i].x / markTRItems[i - 1].realPos.x) + speed * Time.deltaTime);
                currentVerticiesList[2][i] = markTRItems[i - 1].realPos * Mathf.Min(circleRadius[2], (currentVerticiesList[2][i].x / markTRItems[i - 1].realPos.x) + speed * Time.deltaTime);

                if (stretchProgressInfos.ContainsKey(i - 1))
                {
                    stretchProgressInfos[i - 1].localScale = (defaultStretchProgressScale - (defaultStretchProgressScale * (currentVerticiesList[2][i].x / markTRItems[i - 1].realPos.x + 0.05f))) * Vector3.one;
                }
            }
            UpdateStepProgress();
            time += Time.deltaTime;
            yield return null;
        }
        if (GeneralDataManager.TutorialStep == 6)
        {
            GameManager.Inst.Show_Popup(GameManager.Popups.TutorialPopUp);
        }
        SoundManager.Inst.Play("Step");
        GameManager.Inst.gamePlayUi.NextBtnActiveSelf(true);
        turnOffBrushInvoked = false;
        CancelInvoke(nameof(StopBrushSound));
        StopBrushSound();
    }

    internal void AssignRenderTextureUvsToMeshRenderer(RenderTexture renderTexture, int index)
    {
        Texture2D stretchedTexture;
        Material blurMaterial = new(blurMat);
        blurMaterial.SetFloat("_SampleCount", 10f);
        blurMaterial.SetFloat("_BlurSize", 0.005f);

        Texture2D texture2D = GameManager.Inst.RenderTextureToTexture2D(renderTexture);
        GameController.Inst.beforeSRs[index].sprite = Sprite.Create(texture2D, new Rect(0.0f, 0.0f, renderTexture.width, renderTexture.height), new Vector2(0.5f, 0.5f));

        RenderTexture destination = new(renderTexture);
        Graphics.Blit(circleTextures[index], destination);

        copyUvsMat.SetTexture("_MainTex", renderTexture);
        copyUvsMat.SetTexture("_DestTex", destination);
        copyUvsMat.SetFloat("_DestRotation", stretchRandomRotate);

        RenderTexture bufferTex2 = new RenderTexture(renderTexture);
        RenderTexture bufferTex3 = new RenderTexture(renderTexture);

        Graphics.Blit(renderTexture, bufferTex3, blurMaterial);
        Graphics.Blit(bufferTex3, bufferTex2, copyUvsMat);
        Graphics.Blit(bufferTex2, destination);
        stretchedRenderTextures[index] = destination;
        stretchedTexture = GameManager.Inst.RenderTextureToTexture2D(destination);
        circles[index].material.mainTexture = stretchedTexture;
    }

    internal void CombineAllCircles()
    {
        List<Texture> stretchedTextures = new();
        foreach (MeshRenderer meshRenderer in circles)
        {
            stretchedTextures.Add(meshRenderer.material.mainTexture);
        }

        RenderTexture bufferTex2 = new RenderTexture(stretchedTextures[0].width, stretchedTextures[0].height, 0);
        RenderTexture stretchedCombinedTexture = new(bufferTex2);

        combineTextureMat.SetTexture("_DestTex", stretchedCombinedTexture);
        foreach (Texture stretchedTexture in stretchedTextures)
        {
            Graphics.Blit(stretchedTexture, bufferTex2, combineTextureMat);
            Graphics.Blit(bufferTex2, stretchedCombinedTexture);
        }
        GameManager.Inst.gamePlayUi.combinedCirlcesImg.sprite =
        Sprite.Create(GameManager.Inst.RenderTextureToTexture2D(stretchedCombinedTexture), new Rect(0.0f, 0.0f, stretchedTextures[0].width, stretchedTextures[0].height), new Vector2(0.5f, 0.5f));
    }

    internal void SnapToCurrentPos(MarkTRItem markTRItem)
    {
        if (vibrateTime > 0.15f && Input.GetMouseButton(0))
        {
            SoundManager.Inst.LightVibrate();
            vibrateTime = 0;
        }
        float totalDistance = Mathf.Min(Vector3.Distance(transform.position, handTR.position), transform.localScale.x);

        float currentTouchRadius = totalDistance / transform.localScale.x;

        float newDistance = Vector3.Distance(markTRItem.realPos, markTRItem.realPos * currentTouchRadius);
        if (newDistance < markTRItem.lastDistance)
        {
            markTRItem.lastDistance = newDistance;
            float per = Mathf.Min(circleRadius[2], currentTouchRadius * 1.15f);

            currentVerticiesList[2][markTRItem.vertexIndex] = markTRItem.transform.localPosition = markTRItem.realPos * per;
            if (currentTouchRadius > circleRadius[2] - 0.5f)
            {
                per = Mathf.Min(circleRadius[1], currentTouchRadius * 1.15f);
                currentVerticiesList[1][markTRItem.vertexIndex] = markTRItem.transform.localPosition = markTRItem.realPos * per;
            }
            if (currentTouchRadius > circleRadius[1] - 0.33f)
            {
                per = Mathf.Min(circleRadius[0], currentTouchRadius);
                currentVerticiesList[0][markTRItem.vertexIndex] = markTRItem.transform.localPosition = markTRItem.realPos * per;
            }

            if (stretchProgressInfos.ContainsKey(markTRItem.vertexIndex - 1))
            {
                stretchProgressInfos[markTRItem.vertexIndex - 1].localScale = (defaultStretchProgressScale - (defaultStretchProgressScale * currentTouchRadius)) * Vector3.one;
            }
        }
    }
}