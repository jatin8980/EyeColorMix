using System;
using UnityEngine;
using GoogleMobileAds.Ump.Api;

public class AdsConsentController : MonoBehaviour
{
    public bool CanRequestAds => ConsentInformation.CanRequestAds();

    public void GatherConsent(Action<string> onComplete)
    {
        var reqParameter = new ConsentRequestParameters()
        {
           /* ConsentDebugSettings = new ConsentDebugSettings()
            {
                DebugGeography = DebugGeography.EEA,
                TestDeviceHashedIds = new List<string>()
                {
                    "86A96FBEE1F62547055A699FAC84E1DE"
                }
            }*/
        };

        ConsentInformation.Update(reqParameter, (error =>
        {
            if (error != null)
            {
                onComplete(error.Message);
                return;
            }

            ConsentForm.LoadAndShowConsentFormIfRequired(formError => { onComplete?.Invoke(formError?.Message); });
        }));
    }
}