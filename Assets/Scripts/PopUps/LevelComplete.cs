using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelComplete : MonoBehaviour
{
    [SerializeField]
    private RectTransform pupilRewardRT, patternRewardRT, effectRewardRT, highlightRewardRT,
        pupilRewardParent, patternRewardParent, effectRewardParent, highlightRewardParent,
        reward1RT, reward2RT, reward3RT, reward4RT, coinPrefabRT, giftOutPosRT, pathMiddlePointRt, glowsParentRT, doubleRewardParentRt;
    [SerializeField] private Image titleImage;
    [SerializeField] private GameObject rewardsOb;
    [SerializeField] private TextMeshProUGUI rewardAmountText, rewardsTitleText, nextBtnText;
    [SerializeField] private Text giftSliderText;
    [SerializeField] private Slider giftSlider;
    [SerializeField] private Animation giftAnim;
    private int nextRewardLevel = int.MaxValue, rewardAmount, previousRewardLevel = 1, currentLevel;
    private bool isZoomIn;

    private void Start()
    {
        AdsManager.Inst.RequestAndLoadInterstitialAd();
        SoundManager.Inst.Play("LevelComplete");
        GameManager.Inst.SetCoinParentAbovePopUp(true);

        rewardAmount = 25 + ((GeneralDataManager.Level - 2) * 10);
        if (rewardAmount > 195)
        {
            rewardAmount = 195 + (Random.Range(1, 8) * 5);
        }
        GeneralDataManager.Coins += rewardAmount;
        rewardAmountText.text = "+" + rewardAmount.ToString();

        rewardsOb.SetActive(false);
        rewardsOb.GetComponent<Image>().DOFade(0, 0);
        giftAnim.enabled = false;
        rewardsTitleText.color = new(rewardsTitleText.color.r, rewardsTitleText.color.g, rewardsTitleText.color.b, 0);
        currentLevel = GeneralDataManager.Level;
        for (int i = 0; i < GeneralDataManager.Inst.levelNeededToUnlockPupil.Length; i++)
        {
            if (currentLevel <= GeneralDataManager.Inst.levelNeededToUnlockPupil[i])
            {
                nextRewardLevel = GeneralDataManager.Inst.levelNeededToUnlockPupil[i];
                break;
            }
        }

        for (int i = 0; i < GeneralDataManager.Inst.levelNeededToUnlockPattern.Length; i++)
        {
            if (currentLevel <= GeneralDataManager.Inst.levelNeededToUnlockPattern[i])
            {
                if (GeneralDataManager.Inst.levelNeededToUnlockPattern[i] < nextRewardLevel)
                    nextRewardLevel = GeneralDataManager.Inst.levelNeededToUnlockPattern[i];
                break;
            }
        }

        for (int i = 0; i < GeneralDataManager.Inst.levelNeededToUnlockEffect.Length; i++)
        {
            if (currentLevel <= GeneralDataManager.Inst.levelNeededToUnlockEffect[i])
            {
                if (GeneralDataManager.Inst.levelNeededToUnlockEffect[i] < nextRewardLevel)
                    nextRewardLevel = GeneralDataManager.Inst.levelNeededToUnlockEffect[i];
                break;
            }
        }

        for (int i = 0; i < GeneralDataManager.Inst.levelNeededToUnlockHighlight.Length; i++)
        {
            if (currentLevel <= GeneralDataManager.Inst.levelNeededToUnlockHighlight[i])
            {
                if (GeneralDataManager.Inst.levelNeededToUnlockHighlight[i] < nextRewardLevel)
                    nextRewardLevel = GeneralDataManager.Inst.levelNeededToUnlockHighlight[i];
                break;
            }
        }


        if (nextRewardLevel < int.MaxValue)
        {
            for (int i = 0; i < GeneralDataManager.Inst.levelNeededToUnlockPupil.Length; i++)
            {
                if (currentLevel > GeneralDataManager.Inst.levelNeededToUnlockPupil[i])
                {
                    if (GeneralDataManager.Inst.levelNeededToUnlockPupil[i] > previousRewardLevel)
                        previousRewardLevel = GeneralDataManager.Inst.levelNeededToUnlockPupil[i];
                }
                else
                {
                    break;
                }
            }

            for (int i = 0; i < GeneralDataManager.Inst.levelNeededToUnlockPattern.Length; i++)
            {
                if (currentLevel > GeneralDataManager.Inst.levelNeededToUnlockPattern[i])
                {
                    if (GeneralDataManager.Inst.levelNeededToUnlockPattern[i] > previousRewardLevel)
                        previousRewardLevel = GeneralDataManager.Inst.levelNeededToUnlockPattern[i];
                }
                else
                {
                    break;
                }
            }

            for (int i = 0; i < GeneralDataManager.Inst.levelNeededToUnlockEffect.Length; i++)
            {
                if (currentLevel > GeneralDataManager.Inst.levelNeededToUnlockEffect[i])
                {
                    if (GeneralDataManager.Inst.levelNeededToUnlockEffect[i] > previousRewardLevel)
                        previousRewardLevel = GeneralDataManager.Inst.levelNeededToUnlockEffect[i];
                }
                else
                {
                    break;
                }
            }

            for (int i = 0; i < GeneralDataManager.Inst.levelNeededToUnlockHighlight.Length; i++)
            {
                if (currentLevel > GeneralDataManager.Inst.levelNeededToUnlockHighlight[i])
                {
                    if (GeneralDataManager.Inst.levelNeededToUnlockHighlight[i] > previousRewardLevel)
                        previousRewardLevel = GeneralDataManager.Inst.levelNeededToUnlockHighlight[i];
                }
                else
                {
                    break;
                }
            }

            int target = nextRewardLevel - previousRewardLevel;
            int current = currentLevel - previousRewardLevel;
            giftSlider.value = (float)(current - 1) / target;
            giftSliderText.text = ((int)(giftSlider.value * 100)) + "%";
            giftSlider.gameObject.SetActive(true);
        }
        else
        {
            giftSlider.gameObject.SetActive(false);
        }
    }

    //used in aniamtion
    private void GiftSliderFill()
    {
        int target = nextRewardLevel - previousRewardLevel;
        int current = currentLevel - previousRewardLevel;
        giftSlider.DOValue((float)current / target, 1f).SetEase(Ease.Linear).OnUpdate(() =>
        {
            giftSliderText.text = ((int)(giftSlider.value * 100)) + "%";
        });
    }

    private void CollectCoins()
    {
        GameManager.Inst.StartCoroutine(CollectCoinsCoroutine());
    }

    private IEnumerator CollectCoinsCoroutine()
    {
        Vector3 targetPos = GameManager.Inst.GetCoinTarget().position;
        coinPrefabRT.sizeDelta = rewardAmountText.transform.GetChild(0).GetComponent<RectTransform>().sizeDelta;
        float delay = 0.1f;
        for (int i = 0; i < 10; i++)
        {
            RectTransform rt = Instantiate(coinPrefabRT, GameManager.Inst.coinCollectParentTR);
            rt.position = rewardAmountText.transform.GetChild(0).position;
            rt.DOSizeDelta(new(75, 75), 1f).SetDelay(delay);
            rt.DOMove(targetPos, 1f).SetDelay(delay);
            Destroy(rt.gameObject, 1 + delay);
            delay += 0.1f;
        }
        yield return new WaitForSeconds(1f + delay);
        GameManager.Inst.SetCoinsText(GeneralDataManager.Coins.ToString());
        SoundManager.Inst.Play("CoinCollect");
    }

    private IEnumerator ShowRewards()
    {
        if (isZoomIn)
        {
            isZoomIn = false;
            GameManager.Inst.homeScreen.characterTR.DOKill();
            GameManager.Inst.homeScreen.characterTR.DOScale(GameManager.Inst.homeScreen.CharacterScale(), 0.7f);
            GameManager.Inst.homeScreen.characterTR.DOLocalMove(new Vector3(0, GameManager.Inst.homeScreen.CharacterScale() - 1f, 0), 0.7f);
        }
        List<bool> rewardTypes = new() { false, false, false, false };

        if (GameManager.Inst.homeScreen.leftEyeParent.childCount > 1)
        {
            Destroy(GameManager.Inst.homeScreen.leftEyeParent.GetChild(1).gameObject);
            Destroy(GameManager.Inst.homeScreen.rightEyeParent.GetChild(1).gameObject);
        }
        GetComponent<Animation>().enabled = false;
        GameManager.Inst.SetCoinParentAbovePopUp(false);
        titleImage.DOFade(0, 0.3f);
        giftSlider.transform.DOScale(0, 0.3f);
        rewardAmountText.transform.parent.DOScale(0, 0.3f);
        doubleRewardParentRt.DOScale(0, 0.3f);
        rewardsOb.SetActive(true);
        nextBtnText.transform.parent.gameObject.SetActive(false);
        nextBtnText.color = new Color(nextBtnText.color.r, nextBtnText.color.g, nextBtnText.color.b, 0);
        nextBtnText.text = "Great";
        rewardsTitleText.DOFade(1, 0.3f);
        rewardsOb.GetComponent<Image>().DOFade(0.7f, 0.3f);
        giftAnim.transform.parent.localScale = new(0.25f, 0.25f, 0.25f);
        giftSlider.transform.GetChild(1).gameObject.SetActive(false);
        giftAnim.transform.parent.position = giftSlider.transform.GetChild(1).position;
        giftAnim.transform.parent.DOScale(1, 1f);
        giftAnim.transform.parent.GetComponent<RectTransform>().DOAnchorPos(new(0, 712), 1f);

        yield return new WaitForSeconds(1f);
        giftAnim.enabled = true;
        giftAnim.Play();
        yield return new WaitForSeconds(1f);
        int rewardCount = 0;
        for (int i = 0; i < GeneralDataManager.Inst.levelNeededToUnlockPupil.Length; i++)
        {
            if (GeneralDataManager.Inst.levelNeededToUnlockPupil[i] == currentLevel)
            {
                rewardTypes[0] = true;
                InstantiateReward(pupilRewardRT, pupilRewardParent, rewardCount, GameManager.Inst.gamePlayUi.pupilIcons[GeneralDataManager.Inst.orderToShowPupils[i]]);
                rewardCount++;
            }
        }
        if (rewardCount > 1)
        {
            pupilRewardParent.GetChild(pupilRewardParent.childCount - 1).GetChild(0).GetComponent<TextMeshProUGUI>().text = "Pupils";
        }


        rewardCount = 0;
        for (int i = 0; i < GeneralDataManager.Inst.levelNeededToUnlockPattern.Length; i++)
        {
            if (GeneralDataManager.Inst.levelNeededToUnlockPattern[i] == currentLevel)
            {
                rewardTypes[1] = true;
                InstantiateReward(patternRewardRT, patternRewardParent, rewardCount, Resources.Load<Sprite>("Sprites/PatternThumbs/" + GeneralDataManager.Inst.orderToShowPattern[i]));
                rewardCount++;
            }
        }
        if (rewardCount > 1)
        {
            patternRewardParent.GetChild(patternRewardParent.childCount - 1).GetChild(0).GetComponent<TextMeshProUGUI>().text = "Patterns";
        }

        rewardCount = 0;
        for (int i = 0; i < GeneralDataManager.Inst.levelNeededToUnlockEffect.Length; i++)
        {
            if (GeneralDataManager.Inst.levelNeededToUnlockEffect[i] == currentLevel)
            {
                rewardTypes[2] = true;
                InstantiateReward(effectRewardRT, effectRewardParent, rewardCount, Resources.Load<Sprite>("Sprites/EffectThumbs/" + GeneralDataManager.Inst.orderToShowEffects[i]));
                rewardCount++;
            }
        }
        if (rewardCount > 1)
        {
            effectRewardParent.GetChild(effectRewardParent.childCount - 1).GetChild(0).GetComponent<TextMeshProUGUI>().text = "Effects";
        }

        rewardCount = 0;
        for (int i = 0; i < GeneralDataManager.Inst.levelNeededToUnlockHighlight.Length; i++)
        {
            if (GeneralDataManager.Inst.levelNeededToUnlockHighlight[i] == currentLevel)
            {
                rewardTypes[3] = true;
                InstantiateReward(highlightRewardRT, highlightRewardParent, rewardCount, GameManager.Inst.gamePlayUi.highlightIcons[GeneralDataManager.Inst.orderToShowHighlights[i]]);
                rewardCount++;

            }
        }
        if (rewardCount > 1)
        {
            highlightRewardParent.GetChild(highlightRewardParent.childCount - 1).GetChild(0).GetComponent<TextMeshProUGUI>().text = "Highlights";
        }


        RectTransform rewardPosRT = reward1RT;

        switch (rewardTypes.FindAll(val => val == true).Count())
        {
            case 2:
                rewardPosRT = reward2RT;
                break;
            case 3:
                rewardPosRT = reward3RT;
                break;
            case 4:
                rewardPosRT = reward4RT;
                break;
        }
        int childIndex = 0;
        pupilRewardParent.position = patternRewardParent.position =
            effectRewardParent.position = highlightRewardParent.position = giftOutPosRT.position;
        pupilRewardParent.localScale = patternRewardParent.localScale =
            effectRewardParent.localScale = highlightRewardParent.localScale = Vector3.zero;
        Vector3[] path = new Vector3[2];
        path[0] = pathMiddlePointRt.position;
        for (int i = 0; i < rewardTypes.Count; i++)
        {
            if (rewardTypes[i])
            {
                path[1] = rewardPosRT.GetChild(childIndex).position;
                switch (i)
                {
                    case 0:
                        pupilRewardParent.DOScale(1, 1f).SetEase(Ease.OutBack).SetDelay(childIndex * 0.2f);
                        pupilRewardParent.DOPath(path, 1f, PathType.CatmullRom).SetEase(Ease.OutCirc).SetDelay(childIndex * 0.2f).OnComplete(() =>
                        {
                            glowsParentRT.GetChild(0).position = pupilRewardParent.position;
                            glowsParentRT.GetChild(0).GetComponent<Image>().DOFade(0.78f, 0.3f);
                        });
                        break;
                    case 1:
                        patternRewardParent.DOScale(1, 1f).SetEase(Ease.OutBack).SetDelay(childIndex * 0.2f);
                        patternRewardParent.DOPath(path, 1f, PathType.CatmullRom).SetEase(Ease.OutCirc).SetDelay(childIndex * 0.2f).OnComplete(() =>
                        {
                            glowsParentRT.GetChild(1).position = patternRewardParent.position;
                            glowsParentRT.GetChild(1).GetComponent<Image>().DOFade(0.78f, 0.3f);
                        });
                        break;
                    case 2:
                        effectRewardParent.DOScale(1, 1f).SetEase(Ease.OutBack).SetDelay(childIndex * 0.2f);
                        effectRewardParent.DOPath(path, 1f, PathType.CatmullRom).SetEase(Ease.OutCirc).SetDelay(childIndex * 0.2f).OnComplete(() =>
                        {
                            glowsParentRT.GetChild(2).position = effectRewardParent.position;
                            glowsParentRT.GetChild(2).GetComponent<Image>().DOFade(0.78f, 0.3f);
                        });
                        break;
                    case 3:
                        highlightRewardParent.DOScale(1, 1f).SetEase(Ease.OutBack).SetDelay(childIndex * 0.2f);
                        highlightRewardParent.DOPath(path, 1f, PathType.CatmullRom).SetEase(Ease.OutCirc).SetDelay(childIndex * 0.2f).OnComplete(() =>
                        {
                            glowsParentRT.GetChild(3).position = highlightRewardParent.position;
                            glowsParentRT.GetChild(3).GetComponent<Image>().DOFade(0.78f, 0.3f);
                        });
                        break;
                }
                childIndex++;
            }
        }

        yield return new WaitForSeconds(((rewardPosRT.childCount - 1) * 0.2f) + 1f);
        nextBtnText.transform.parent.gameObject.SetActive(true);
        nextBtnText.DOFade(1, 0.2f);
    }

    private void InstantiateReward(RectTransform prefabRT, RectTransform parent, int rewardCount, Sprite sprite)
    {
        RectTransform rt = Instantiate(prefabRT, parent);
        rt.SetAsFirstSibling();
        rt.anchoredPosition = new Vector2(rewardCount * 20, rewardCount * 10);
        rt.gameObject.SetActive(true);
        parent.gameObject.SetActive(true);
        if (rt.GetChild(0).childCount == 0)
            rt.GetChild(0).GetComponent<Image>().sprite = sprite;
        else
            rt.GetChild(0).GetChild(0).GetComponent<Image>().sprite = sprite;
    }

    internal void DoubleRewardCallback()
    {
        GeneralDataManager.Coins += rewardAmount;
        rewardAmountText.text = "+" + (rewardAmount * 2).ToString();
        CollectCoins();
        doubleRewardParentRt.gameObject.SetActive(false);
    }

    public void On_Next_Btn_Click()
    {
        if (nextRewardLevel - currentLevel == 0 && !rewardsOb.activeSelf)
        {
            StartCoroutine(ShowRewards());
        }
        else
        {
            GameManager.Inst.HidePopUp(gameObject);
            GameManager.Inst.Show_Screen(GameManager.Screens.Home);
            GameManager.Inst.homeScreen.Invoke(nameof(GameManager.Inst.homeScreen.ChangeCharacter), 0.5f);

            if (GameManager.Inst.homeScreen.leftEyeParent.childCount > 1)
            {
                Destroy(GameManager.Inst.homeScreen.leftEyeParent.GetChild(1).gameObject);
                Destroy(GameManager.Inst.homeScreen.rightEyeParent.GetChild(1).gameObject);
            }
            if (isZoomIn)
            {
                isZoomIn = false;
                GameManager.Inst.homeScreen.characterTR.DOKill();
                GameManager.Inst.homeScreen.characterTR.DOScale(GameManager.Inst.homeScreen.CharacterScale(), 0.7f);
                GameManager.Inst.homeScreen.characterTR.DOLocalMove(new Vector3(0, GameManager.Inst.homeScreen.CharacterScale() - 1f, 0), 0.7f);
            }
            GameManager.Inst.SetCoinParentAbovePopUp(false);
            AdsManager.Inst.ShowInterstitialAd("LevelComplete");
            System.GC.Collect();
            Resources.UnloadUnusedAssets();
        }
    }

    public void On_Zoom_Btn_Click()
    {
        if (isZoomIn)
        {
            isZoomIn = false;
            GameManager.Inst.homeScreen.characterTR.DOKill();
            GameManager.Inst.homeScreen.characterTR.DOScale(GameManager.Inst.homeScreen.CharacterScale(), 0.7f);
            GameManager.Inst.homeScreen.characterTR.DOLocalMove(new Vector3(0, GameManager.Inst.homeScreen.CharacterScale() - 1f, 0), 0.7f);
        }
        else
        {
            isZoomIn = true;
            GameManager.Inst.homeScreen.characterTR.DOKill();
            GameManager.Inst.homeScreen.characterTR.DOScale(1.8f, 0.7f);
            GameManager.Inst.homeScreen.characterTR.DOLocalMove(new(0, -1.75f, 0), 0.7f);
        }
    }

    public void On_Double_Reward_Btn_Click()
    {
        AdsManager.Inst.RequestAndLoadRewardedAd("LevelCompleteDoubleReward");
    }
}