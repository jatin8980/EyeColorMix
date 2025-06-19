using System;
using System.Collections;
using System.Collections.Generic;
#if UNITY_IOS
using Unity.Advertisement.IosSupport;
#endif
using UnityEngine;
using UnityEngine.Advertisements;

public class ATT_Controller : MonoBehaviour
{
    [SerializeField] private AdsConsentController adsConsent;
    public event Action sentTrackingAuthorizationRequest;

    private void Start()
    {
        adsConsent.GatherConsent(s =>
        {
            if (s != null)
            {
                //fail to gather consent
                //reason:=> s
            }

            if (adsConsent.CanRequestAds)
            {
                //consent granted, show ads
            }

            Invoke(nameof(RequestAuthorizationTracking), 0.08f);
        });
    }

    public void RequestAuthorizationTracking()
    {
#if UNITY_IOS
        var status = ATTrackingStatusBinding.GetAuthorizationTrackingStatus();
        if (status == ATTrackingStatusBinding.AuthorizationTrackingStatus.NOT_DETERMINED)
        {
            ATTrackingStatusBinding.RequestAuthorizationTracking();

            sentTrackingAuthorizationRequest?.Invoke();
            StartCoroutine(Load_Next_Scene());
        }
        else
        {
            CheckTrackingResponse();
            //GameManager.Inst.Show_Screen(GameManager.Screens.Launch);
        }
#else
        //GameManager.Inst.Show_Screen(GameManager.Screens.Launch);
#endif
    }

    private void CheckTrackingResponse()
    {
#if UNITY_IOS
        switch (ATTrackingStatusBinding.GetAuthorizationTrackingStatus())
        {
            case ATTrackingStatusBinding.AuthorizationTrackingStatus.NOT_DETERMINED:
            case ATTrackingStatusBinding.AuthorizationTrackingStatus.RESTRICTED:
            case ATTrackingStatusBinding.AuthorizationTrackingStatus.DENIED:
            {
                MetaData gdprMetaData = new MetaData("gdpr");
                gdprMetaData.Set("consent", "false");
                Advertisement.SetMetaData(gdprMetaData);
            } 
                break;
            case ATTrackingStatusBinding.AuthorizationTrackingStatus.AUTHORIZED:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
#endif
    }

    IEnumerator Load_Next_Scene()
    {
#if UNITY_IOS
        var status = ATTrackingStatusBinding.GetAuthorizationTrackingStatus();

        if (status == ATTrackingStatusBinding.AuthorizationTrackingStatus.NOT_DETERMINED)
        {
            status = ATTrackingStatusBinding.GetAuthorizationTrackingStatus();
            yield return null;
        }
        CheckTrackingResponse();
#endif
        //GameManager.Inst.Show_Screen(GameManager.Screens.Launch);
        yield return null;
    }
}
