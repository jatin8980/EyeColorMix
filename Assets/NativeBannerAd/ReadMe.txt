//ReadMe Native Banner Ad



#region Native Banner Ad

        internal bool nativeAdLoaded = false;
        private NativeAd nativeAd;
        NativeBannerController nativeBanner;
        public GameObject[] nativeBannerPrefabs;

        string[] android_NativeID = { "ca-app-pub-3940256099942544/2247696110", "ca-app-pub-3940256099942544/2247696110", "ca-app-pub-3940256099942544/2247696110" },
                iPhone_NativeID = { "ca-app-pub-3940256099942544/3986624511", "ca-app-pub-3940256099942544/3986624511", "ca-app-pub-3940256099942544/3986624511" };
        string _nativeAdID = string.Empty;
        int bannerType = 0;

        public void RequestNativeAd()
        {
            nativeAdLoaded = false;
            Destroy_NativeBanner_Ad();

            bannerType = Random.Range(0, nativeBannerPrefabs.Length);

            if (useTestId)
            {
#if UNITY_ANDROID
                _nativeAdID = "ca-app-pub-3940256099942544/2247696110";
#elif UNITY_IPHONE
            _nativeAdID = "ca-app-pub-3940256099942544/3986624511";
#endif
            }

#if UNITY_ANDROID
            _nativeAdID = android_NativeID[bannerType];
#elif UNITY_IPHONE
        _nativeAdID = iPhone_NativeID[bannerType];
#else
        _nativeAdID = "unexpected_platform";
#endif

            AdLoader adLoader = new AdLoader.Builder(_nativeAdID).ForNativeAd().Build();

            adLoader.OnNativeAdLoaded += this.HandleNativeAdLoaded;
            adLoader.OnAdFailedToLoad += this.HandleNativeAdFailedToLoad;

            adLoader.LoadAd(new AdRequest());
        }

        void HandleNativeAdLoaded(object sender, NativeAdEventArgs args)
        {
            nativeAdLoaded = true;

            this.nativeAd = args.nativeAd;

            nativeBanner = Instantiate(nativeBannerPrefabs[bannerType], GameManager.Inst.popUpParent.parent).GetComponent<NativeBannerController>();
            nativeBanner.Refresh_Details(nativeAd);

//This is for when ad load and at same time screen was open then refresh content
            //DEMO
FindAnyObjectByType<Tutorial>()?.Refresh_Bottom_Padding();
        }

        public void Apply_Bottom_Padding_ForNative_Banner(RectTransform rt, int bottomPadding)
        {
            if (nativeBanner != null && nativeAdLoaded)
                rt.offsetMin = new Vector2(rt.offsetMin.x, rt.offsetMin.y + bottomPadding);
        }

        void Destroy_NativeBanner_Ad()
        {
            if (nativeBanner?.gameObject != null)
                Destroy(nativeBanner.gameObject);
            nativeAdLoaded = false;
            nativeAd?.Destroy();
        }

        void HandleNativeAdFailedToLoad(object sender, AdFailedToLoadEventArgs args)
        {
            nativeAdLoaded = false;
            Debug.Log("Native ad failed to load: " + args);
        }

        #endregion