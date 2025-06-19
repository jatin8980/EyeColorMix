using DG.Tweening;
using Mosframe;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class EffectsController : MonoBehaviour
{
    [SerializeField] private RectTransform contentRT;
    internal static EffectsController Inst;
    internal int selectedEffectIndex = -1;
    internal List<EffectItem> effectItems = new();
    internal List<Sprite> effectIcons;
    [SerializeField] private List<Material> effectMaterials;
    [SerializeField] private Slider effectSlider;
    private UnityAction<float> onSliderValueChange;
    [SerializeField] private AnimationCurve outQuartEaseCurve, zoomOutInTwirlCurve, zoomEffectForGalaxyCurve;

    private void Awake()
    {
        Inst = this;
        RectTransform sliderParentRT = effectSlider.transform.parent.GetComponent<RectTransform>();
        if (sliderParentRT.rect.width > 900)
        {
            float diff = sliderParentRT.rect.width - 900;
            sliderParentRT.offsetMax = new Vector2(-diff / 2f, sliderParentRT.offsetMax.y);
            sliderParentRT.offsetMin = new Vector2(diff / 2f, sliderParentRT.offsetMin.y);
        }
    }

    private void Start()
    {
        effectIcons = Resources.LoadAll<Sprite>("Sprites/EffectThumbs/").OrderBy(item => Convert.ToInt32(item.name)).ToList();
    }

    internal void OnEnable()
    {
        RectTransform rt = GetComponent<RectTransform>();
        rt.anchoredPosition = new Vector2(rt.anchoredPosition.x, -20);
        if (effectItems.Count == 0)
            return;

        if (GeneralDataManager.IsChoiceSave)
            selectedEffectIndex = GeneralDataManager.SelectedEffectIndex;
        else
            selectedEffectIndex = GeneralDataManager.Inst.orderToShowEffects[0];

        if (selectedEffectIndex == -1)
            selectedEffectIndex = GeneralDataManager.Inst.orderToShowEffects[0];
        float y = AdsManager.Inst.isBannerLoaded ? 537f + GameManager.Inst.bannerHeight : 537f;
        rt.DOAnchorPosY(y, 0.3f);
        GameManager.Inst.gamePlayUi.nextBtnParentRT.anchoredPosition = new Vector2(0, y + 100);
        effectSlider.value = 0;
        ApplyEffect(selectedEffectIndex);


        float viewPortSize = contentRT.parent.GetComponent<RectTransform>().rect.width;
        float width = effectItems[0].GetComponent<RectTransform>().rect.width;
        contentRT.anchoredPosition = new Vector2(-Mathf.Min(contentRT.rect.width - viewPortSize, (GeneralDataManager.Inst.orderToShowEffects.IndexOf(selectedEffectIndex) + 1) * width), contentRT.anchoredPosition.y);

        contentRT.parent.parent.GetComponent<DynamicScrollView>().refresh();
    }

    internal void RefreshAtIndex(int index)
    {
        foreach (EffectItem effectItem in effectItems)
        {
            if (effectItem.effectIndex == index)
            {
                effectItem.SetThis();
                break;
            }
        }
    }

    internal void RefreshForAd()
    {
        RectTransform rt = GetComponent<RectTransform>();
        rt.DOKill();
        float y = AdsManager.Inst.isBannerLoaded ? 537f + GameManager.Inst.bannerHeight : 537f;
        rt.DOAnchorPosY(y, 0.2f);
        GameManager.Inst.gamePlayUi.nextBtnParentRT.anchoredPosition = new Vector2(0, y + 100);
    }

    internal void ApplyEffect(int effectIndex)
    {
        if (GeneralDataManager.IsChoiceSave)
            GeneralDataManager.SelectedEffectIndex = effectIndex;
        else
            GeneralDataManager.SelectedEffectIndex = 0;

        effectSlider.interactable = true;
        effectSlider.DOKill();
        effectSlider.DOValue(0, effectSlider.value > 0 ? 0.5f : 0).OnComplete(() =>
        {
            GameManager.Inst.gamePlayUi.PlayItemAppliedEffect();
            GameManager.Inst.gamePlayUi.combinedCirlcesImg.material = null;
            if (effectIndex == -1)
            {
                effectSlider.interactable = false;
                return;
            }
            switch (effectIndex)
            {
                case 0:
                    GameManager.Inst.gamePlayUi.combinedCirlcesImg.material = effectMaterials[effectIndex];
                    onSliderValueChange = new((float val) =>
                    {
                        effectMaterials[effectIndex].SetFloat("_BlurSize", val * 0.00015f);
                    });
                    break;
                case 1:
                    GameManager.Inst.gamePlayUi.combinedCirlcesImg.material = effectMaterials[effectIndex];
                    onSliderValueChange = new((float val) =>
                    {
                        float t = outQuartEaseCurve.Evaluate(val / 100);
                        effectMaterials[effectIndex].SetFloat("_PixelControl", t);
                    });
                    break;
                case 2:
                    GameManager.Inst.gamePlayUi.combinedCirlcesImg.material = effectMaterials[effectIndex];
                    onSliderValueChange = new((float val) =>
                    {
                        effectMaterials[effectIndex].SetFloat("_MoveAmount", val * -0.01125f);
                    });
                    break;
                case 3:
                    GameManager.Inst.gamePlayUi.combinedCirlcesImg.material = effectMaterials[effectIndex];
                    onSliderValueChange = new((float val) =>
                    {
                        effectMaterials[effectIndex].SetFloat("_NoiseStrength", val * 0.0025f);
                    });
                    break;
                case 4:
                    GameManager.Inst.gamePlayUi.combinedCirlcesImg.material = effectMaterials[effectIndex];
                    onSliderValueChange = new((float val) =>
                    {
                        effectMaterials[effectIndex].SetFloat("_SuckAmount", val * -0.005f);
                    });
                    break;
                case 5:
                    GameManager.Inst.gamePlayUi.combinedCirlcesImg.material = effectMaterials[effectIndex];
                    onSliderValueChange = new((float val) =>
                    {
                        effectMaterials[effectIndex].SetFloat("_SpikeIntensity", val * 0.0045f);
                        effectMaterials[effectIndex].SetFloat("_StretchAmount", val * -0.0045f);
                    });
                    break;
                case 6:
                    GameManager.Inst.gamePlayUi.combinedCirlcesImg.material = effectMaterials[effectIndex];
                    onSliderValueChange = new((float val) =>
                    {
                        float maxAmount = 0.4f;
                        effectMaterials[effectIndex].SetFloat("_MaxRotation", val * 1.5f);
                        effectMaterials[effectIndex].SetFloat("_SuckAmount", zoomOutInTwirlCurve.Evaluate(val / 100) / (1 / maxAmount));
                    });
                    break;
                case 7:
                    GameManager.Inst.gamePlayUi.combinedCirlcesImg.material = effectMaterials[effectIndex];
                    onSliderValueChange = new((float val) =>
                    {
                        float effect = val * 0.002f;
                        effectMaterials[effectIndex].SetFloat("intensity", effect);
                        effectMaterials[effectIndex].SetVector("_Offset", new Vector4(-effect / 2f, -effect / 2f, 0, 0));
                    });

                    break;
                case 8:
                    GameManager.Inst.gamePlayUi.combinedCirlcesImg.material = effectMaterials[effectIndex];
                    onSliderValueChange = new((float val) =>
                    {
                        float maxAmount = 0.4f;
                        effectMaterials[effectIndex].SetFloat("_MaxRotation", val * 3.6f);
                        effectMaterials[effectIndex].SetFloat("_SuckAmount", zoomEffectForGalaxyCurve.Evaluate(val / 100) / (1 / maxAmount));
                    });
                    break;
                case 9:
                    GameManager.Inst.gamePlayUi.combinedCirlcesImg.material = effectMaterials[effectIndex];
                    onSliderValueChange = new((float val) =>
                    {
                        float effect = val * 0.0011f;
                        effectMaterials[effectIndex].SetFloat("intensity", effect);
                        effectMaterials[effectIndex].SetVector("_Offset", new Vector4(-effect / 2f, -effect / 2f, 0, 0));
                    });
                    break;
                case 10:
                    GameManager.Inst.gamePlayUi.combinedCirlcesImg.material = effectMaterials[effectIndex];
                    onSliderValueChange = new((float val) =>
                    {
                        float effect = val * 0.0007f;
                        effectMaterials[effectIndex].SetFloat("intensity", effect);
                        effectMaterials[effectIndex].SetVector("_Offset", new Vector4(-effect / 2f, -effect / 2f, 0, 0));
                    });
                    break;
            }
            if (GeneralDataManager.TutorialStep == 11)
            {
                effectSlider.DOKill();
                Transform handleRt = effectSlider.transform.GetChild(1).GetChild(0);
                UserTutorialController.Inst.handRT.position = handleRt.position;
                UserTutorialController.Inst.SetActiveHand(true);
                effectSlider.DOValue(100, 3).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.Linear).OnUpdate(() =>
                {
                    UserTutorialController.Inst.handRT.position = handleRt.position;
                });
            }
            else
            {
                effectSlider.DOValue(50, 0.5f);
            }
        });
    }

    public void On_Slider_Value_Change()
    {
        onSliderValueChange?.Invoke(effectSlider.value);
        if (effectSlider.value % 10 == 0)
        {
            SoundManager.Inst.LightVibrate();
        }
    }

    public void OnMouseDownOnSlider()
    {
        if (UserTutorialController.Inst != null)
        {
            effectSlider.DOKill();
            GameManager.Inst.HidePopUp(UserTutorialController.Inst.gameObject);
            GeneralDataManager.TutorialStep++;// to 12
            GameManager.Inst.gamePlayUi.NextBtnActiveSelf(true);
        }
    }
}