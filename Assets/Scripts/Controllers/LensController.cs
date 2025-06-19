using DG.Tweening;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class LensController : MonoBehaviour
{
    private float clampRadius = 1f;
    [SerializeField] private RectTransform lensTarget, leftLensCapTR, rightLensCapTR;
    public RectTransform leftLensParent, rightLensParent;
    internal Transform currentLens;
    private Vector3 targetPos;
    private bool isRightLens, isInsideRadius;
    [SerializeField] private Image boxImage;
    Vector2Int defaultLeftLidPos;

    private void Start()
    {
        lensTarget.anchoredPosition = new Vector2(lensTarget.anchoredPosition.x, lensTarget.anchoredPosition.y - GameManager.Inst.GetDifferenceFromBottom());
    }
    private void OnEnable()
    {
        GameManager.Inst.homeScreen.characterTR.localPosition = GeneralDataManager.Inst.characterDetails[GameManager.Inst.homeScreen.characterIndex].lensDragLeftPos;
        GameManager.Inst.homeScreen.characterTR.localScale = new(3.59f, 3.59f, 3.59f);
        SetActiveTarget(false);
        RefreshForAd();
        ResetCaps();
    }

    private void Update()
    {
        if (currentLens != null)
        {
            if (Input.GetMouseButtonDown(0))
            {
                targetPos = GameManager.Inst.mainCamera.ScreenToWorldPoint(Input.mousePosition);
                targetPos.z = 90;
                StopAllCoroutines();
                SetCurrentLensAndKeepChildAsItIs(targetPos);
                StartCoroutine(MoveLocalZero(0.7f));
                currentLens.GetComponent<Animation>().Stop();
                GameManager.Inst.homeScreen.PlayBlinkAnim(false);
                currentLens.localScale = Vector3.one;
            }
            else if (Input.GetMouseButton(0))
            {
                targetPos = GameManager.Inst.mainCamera.ScreenToWorldPoint(Input.mousePosition);
                targetPos.z = 90;

                if (Vector3.Distance(targetPos, lensTarget.position) <= clampRadius)
                {
                    if (!isInsideRadius)
                    {
                        isInsideRadius = true;
                        currentLens.DOKill();
                        currentLens.DOMove(lensTarget.position, 0.2f);
                        lensTarget.localScale = new(0.95f, 0.95f, 0.95f);
                        lensTarget.GetComponent<RotateZ>().enabled = false;
                        currentLens.GetChild(0).localScale = new(0.151f, 0.151f, 0.151f);

                    }
                }
                else if (isInsideRadius)
                {
                    currentLens.GetChild(0).localScale = new(0.17f, 0.17f, 0.17f);
                    isInsideRadius = false;
                    currentLens.DOKill();
                    lensTarget.localScale = Vector3.one;
                    lensTarget.GetComponent<RotateZ>().enabled = true;
                    StopAllCoroutines();
                    SetCurrentLensAndKeepChildAsItIs(targetPos);
                    StartCoroutine(MoveLocalZero(0.7f));
                }
                else
                {
                    currentLens.position = targetPos;
                }
            }
            else if (Input.GetMouseButtonUp(0))
            {
                if (isInsideRadius)
                {
                    SoundManager.Inst.Play("EyePlace");
                    SoundManager.Inst.LightVibrate();
                    if (!isRightLens)
                    {
                        Transform tr = currentLens.GetChild(0);
                        tr.SetParent(GameManager.Inst.homeScreen.leftEyeParent);
                        tr.localScale = Vector3.one * 0.0002325472f;
                        tr.SetAsLastSibling();
                        tr.localPosition = Vector3.zero;

                        SetActiveTarget(false);
                        currentLens = null;
                        if (UserTutorialController.Inst != null)
                        {
                            GeneralDataManager.TutorialStep++;// to 10
                            GameManager.Inst.HidePopUp(UserTutorialController.Inst.gameObject);
                        }

                        GameManager.Inst.homeScreen.characterTR.DOLocalMove(GeneralDataManager.Inst.characterDetails[GameManager.Inst.homeScreen.characterIndex].lensDragRightPos, 0.8f).OnComplete(() =>
                        {
                            isRightLens = true;
                            currentLens = rightLensParent;
                            SetActiveTarget(true);

                            if (GeneralDataManager.TutorialStep == 10)
                            {
                                GameManager.Inst.Show_Popup(GameManager.Popups.TutorialPopUp);
                                UserTutorialController.Inst.StartCoroutine(UserTutorialController.Inst.SetHandAnimForLens(rightLensParent.parent, GetTargetPos()));
                            }
                            if (!Input.GetMouseButton(0))
                            {
                                rightLensParent.GetComponent<Animation>().Play();
                                GameManager.Inst.homeScreen.PlayBlinkAnim(true);
                            }
                        });
                    }
                    else
                    {
                        Transform tr = currentLens.GetChild(0);
                        tr.SetParent(GameManager.Inst.homeScreen.rightEyeParent);
                        tr.localScale = Vector3.one * 0.0002325472f;
                        tr.SetAsLastSibling();
                        SetActiveTarget(false);
                        tr.localPosition = Vector3.zero;
                        currentLens = null;
                        GameManager.Inst.homeScreen.characterTR.DOKill();
                        GameManager.Inst.homeScreen.characterTR.DOScale(GameManager.Inst.homeScreen.CharacterScale(), 0.7f);
                        GameManager.Inst.homeScreen.characterTR.DOLocalMove(new Vector3(0, GameManager.Inst.homeScreen.CharacterScale() - 1f, 0), 0.7f);
                        GameManager.Inst.homeScreen.PlayOrStopCharacterAnim(true);
                        Invoke(nameof(LevelComplete), 0.71f);
                        gameObject.SetActive(false);
                        GameManager.Inst.gamePlayUi.Invoke(nameof(GameManager.Inst.gamePlayUi.SaveCurrentLevelToGallery), 0.2f);
                        if (UserTutorialController.Inst != null)
                        {
                            GeneralDataManager.TutorialStep++;// to 11
                            GameManager.Inst.HidePopUp(UserTutorialController.Inst.gameObject);
                        }
                    }
                }
                else
                {
                    currentLens.DOKill();
                    StopAllCoroutines();
                    SetCurrentLensAndKeepChildAsItIs(currentLens.parent.position);
                    currentLens.GetComponent<Animation>().Play();
                    GameManager.Inst.homeScreen.PlayBlinkAnim(true);
                    StartCoroutine(MoveLocalZero(0.7f));
                }
            }
        }
    }

    private void LevelComplete()
    {
        GeneralDataManager.Level++;
        if (GameManager.Inst.homeScreen.characterIndex == GameManager.Inst.homeScreen.totalCharactersCount - 1)
            GameManager.Inst.homeScreen.characterIndex = 0;
        else
            GameManager.Inst.homeScreen.characterIndex++;

        GameManager.Inst.Show_Popup(GameManager.Popups.LevelCompletePopUp);
        GameManager.Inst.SetActiveCoins(true);
    }

    private void SetCurrentLensAndKeepChildAsItIs(Vector3 position)
    {
        Vector3 currentEyePos = currentLens.GetChild(0).position;
        currentLens.position = position;
        currentLens.GetChild(0).position = currentEyePos;
    }

    private IEnumerator MoveLocalZero(float duration)
    {
        Transform eyeTr = currentLens.GetChild(0);
        float time = 0;
        while (time < duration)
        {
            time += Time.deltaTime;
            eyeTr.localPosition = Vector3.Lerp(eyeTr.localPosition, Vector3.zero, time / duration);
            yield return null;
        }
        eyeTr.localPosition = Vector3.zero;
    }


    internal void SetLens(int index)
    {
        Sprite[] sprites = Resources.LoadAll<Sprite>("Sprites/Lens/Cases/" + index);
        boxImage.sprite = sprites.First(sprite => sprite.name == "box");
        if (sprites.Length == 3)
        {
            leftLensCapTR.GetComponent<Image>().sprite = sprites.First(sprite => sprite.name == "lidLeft");
            rightLensCapTR.GetComponent<Image>().sprite = sprites.First(sprite => sprite.name == "lidRight");
            rightLensCapTR.localScale = Vector3.one;
        }
        else
        {
            leftLensCapTR.GetComponent<Image>().sprite = rightLensCapTR.GetComponent<Image>().sprite = sprites.First(sprite => sprite.name.Contains("lid"));
            rightLensCapTR.localScale = new Vector3(-1, 1, 1);
        }
        float size = 250;

        switch (index)
        {
            case 0:
            case 4:
                size = 248;
                defaultLeftLidPos = new Vector2Int(177, -28);
                break;
            case 1:
                size = 260;
                defaultLeftLidPos = new Vector2Int(175, -31);
                break;
            case 2:
                size = 274;
                defaultLeftLidPos = new Vector2Int(187, -40);
                break;
            case 3:
                size = 270;
                defaultLeftLidPos = new Vector2Int(177, -17);
                break;
            case 5:
            case 19:
                size = 270;
                defaultLeftLidPos = new Vector2Int(177, -28);
                break;
            case 6:
            case 7:
            case 8:
            case 9:
                size = 238;
                defaultLeftLidPos = new Vector2Int(177, -28);
                break;
            case 10:
                size = 275;
                defaultLeftLidPos = new Vector2Int(177, -28);
                break;
            case 11:
                size = 200;
                defaultLeftLidPos = new Vector2Int(177, -28);
                break;
            case 12:
            case 13:
                size = 240;
                defaultLeftLidPos = new Vector2Int(177, -28);
                break;
            case 15:
                size = 285;
                defaultLeftLidPos = new Vector2Int(175, -28);
                break;
            case 14:
                size = 252;
                defaultLeftLidPos = new Vector2Int(175, -26);
                break;
            case 16:
            case 17:
                size = 243;
                defaultLeftLidPos = new Vector2Int(177, -28);
                break;
            case 18:
                size = 267;
                defaultLeftLidPos = new Vector2Int(177, -28);
                break;
            case 20:
                size = 230;
                defaultLeftLidPos = new Vector2Int(177, -28);
                break;
            case 21:
                size = 265;
                defaultLeftLidPos = new Vector2Int(172, -24);
                break;
            case 22:
                size = 245;
                defaultLeftLidPos = new Vector2Int(177, -28);
                break;
            case 23:
                size = 257;
                defaultLeftLidPos = new Vector2Int(177, -30);
                break;
            case 24:
                size = 256;
                defaultLeftLidPos = new Vector2Int(177, -28);
                break;
        }
        leftLensCapTR.sizeDelta = rightLensCapTR.sizeDelta = new Vector2(index == 23 ? 276 : size, size);
        leftLensCapTR.anchoredPosition = leftLensParent.parent.GetComponent<RectTransform>().anchoredPosition = new Vector2(177, -28);
        rightLensCapTR.anchoredPosition = rightLensParent.parent.GetComponent<RectTransform>().anchoredPosition = new Vector2(-177, -28);
    }

    internal void RotateRightCap()
    {
        rightLensCapTR.DOLocalRotate(new Vector3(0, 0, 360), 0.5f, RotateMode.FastBeyond360).OnComplete(() =>
        {
            rightLensCapTR.DOAnchorPos(new((-defaultLeftLidPos.x) + rightLensCapTR.sizeDelta.x * 0.85f, defaultLeftLidPos.y + rightLensCapTR.sizeDelta.x * 0.85f), 0.5f).OnComplete(() =>
            {
                rightLensCapTR.SetAsFirstSibling();
                rightLensCapTR.GetComponent<Image>().DOFade(0, 0.5f);
                rightLensCapTR.DOAnchorPos(new Vector2(-defaultLeftLidPos.x, defaultLeftLidPos.y), 0.5f).OnComplete(() =>
                {
                    Transform rightLens = rightLensParent.GetChild(0);
                    rightLensCapTR.gameObject.SetActive(false);
                    rightLens.DOScale(rightLens.localScale * 1.05f, 0.2f);
                    rightLens.DOLocalMove(rightLens.localPosition * 1.05f, 0.2f).OnComplete(() =>
                    {
                        rightLens.DOScale(0.17f, 0.7f);
                        rightLens.DOLocalMove(Vector3.zero, 0.7f);
                    });
                });
            });
        });
        targetPos = rightLensCapTR.TransformPoint(Vector3.zero);
    }

    internal void SetActiveTarget(bool active)
    {
        lensTarget.gameObject.SetActive(active);
        if (active)
        {
            lensTarget.localScale = Vector3.one;
            lensTarget.GetComponent<RotateZ>().enabled = true;
        }
    }

    internal Vector3 GetTargetPos() => lensTarget.position;


    internal void RotateLeftCap()
    {
        isRightLens = false;
        leftLensCapTR.DOLocalRotate(new Vector3(0, 0, 360), 0.5f, RotateMode.FastBeyond360).OnComplete(() =>
        {
            leftLensCapTR.DOAnchorPos(new(defaultLeftLidPos.x - leftLensCapTR.sizeDelta.x * 0.85f, defaultLeftLidPos.y + leftLensCapTR.sizeDelta.x * 0.85f), 0.5f).OnComplete(() =>
            {
                leftLensCapTR.SetAsFirstSibling();
                leftLensCapTR.GetComponent<Image>().DOFade(0, 0.5f);
                leftLensCapTR.DOAnchorPos(defaultLeftLidPos, 0.5f).OnComplete(() =>
                {
                    Transform leftLens = leftLensParent.GetChild(0);
                    leftLensCapTR.gameObject.SetActive(false);
                    leftLens.DOScale(leftLens.localScale * 1.05f, 0.2f);
                    leftLens.DOLocalMove(leftLens.localPosition * 1.05f, 0.2f).OnComplete(() =>
                    {
                        leftLens.DOScale(0.17f, 0.7f);
                        leftLens.DOLocalMove(Vector3.zero, 0.7f).OnComplete(() =>
                        {
                            currentLens = leftLensParent;
                            SetActiveTarget(true);
                            if (!Input.GetMouseButton(0))
                            {
                                leftLensParent.GetComponent<Animation>().Play();
                                GameManager.Inst.homeScreen.PlayBlinkAnim(true);
                            }
                            if (GeneralDataManager.TutorialStep == 9)
                            {
                                GameManager.Inst.Show_Popup(GameManager.Popups.TutorialPopUp);
                                UserTutorialController.Inst.StartCoroutine(UserTutorialController.Inst.SetHandAnimForLens(leftLensParent.parent, lensTarget.position));
                            }
                        });
                    });
                    GameManager.Inst.homeScreen.characterTR.gameObject.SetActive(true);
                    GameManager.Inst.homeScreen.DoFadeCharacter(1f, 0.5f);
                    GameManager.Inst.homeScreen.PlayOrStopCharacterAnim(false);
                });
            });
        });
        targetPos = leftLensCapTR.TransformPoint(Vector3.zero);
    }

    internal void ResetCaps()
    {
        leftLensCapTR.anchoredPosition = defaultLeftLidPos;
        rightLensCapTR.anchoredPosition = new Vector2(-defaultLeftLidPos.x, defaultLeftLidPos.y);
        rightLensParent.anchoredPosition = leftLensParent.anchoredPosition = Vector2.zero;
        rightLensCapTR.localEulerAngles = leftLensCapTR.localEulerAngles = Vector3.zero;
        leftLensCapTR.GetComponent<Image>().color = rightLensCapTR.GetComponent<Image>().color = Color.white;
        leftLensCapTR.gameObject.SetActive(true);
        rightLensCapTR.gameObject.SetActive(true);
        leftLensParent.parent.SetAsLastSibling();
        leftLensCapTR.SetAsLastSibling();
        rightLensParent.parent.SetAsLastSibling();
        rightLensCapTR.SetAsLastSibling();
        boxImage.transform.SetAsFirstSibling();
    }

    internal void RefreshForAd()
    {
        RectTransform rt = GetComponent<RectTransform>();
        rt.DOKill();
        if (AdsManager.Inst.isBannerLoaded)
        {
            rt.anchoredPosition = new Vector2(rt.anchoredPosition.x, 211 + GameManager.Inst.bannerHeight + 20);
        }
        else
        {
            rt.anchoredPosition = new Vector2(rt.anchoredPosition.x, 211);
        }
    }
}
