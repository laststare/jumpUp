using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using JumpUp.External;
using UniRx;
using UnityEngine;

namespace _scripts.Game.Ads
{
    public class AdsPm : BaseDisposable
    {
        public struct Ctx
        {
            public IReadOnlyReactiveTrigger Finish;
            public IReactiveProperty<int> levelIndex;
            public ReactiveTrigger ReloadAds;
            public int levelCount;
            public ReactiveProperty<int> levelCounter;
            public ReactiveProperty<bool> _interShowTime;
        }

        private Ctx _ctx;
        private const string MaxSdkKey = "6AQkyPv9b4u7yTtMH9PT40gXg00uJOTsmBOf7hDxa_-FnNZvt_qTLnJAiKeb5-2_T8GsI_dGQKKKrtwZTlCzAR";
        private const string bannerAdUnitId = "bf069117fbbc46c3"; // banner
        private const string adUnitId = "38dd13afec4de57e"; // interstitial
        private int retryAttempt;
        private bool startInterstitial, firstime = true, interShowing, interClicked, videoShowing, videoClicked, interShowTime;
        private CompositeDisposable disposables;
        private IYandexAppMetrica appmetrica;
        private CancellationTokenSource cancellation;
        public AdsPm(Ctx Ctx)
        {
            _ctx = Ctx;
            appmetrica = AppMetrica.Instance;
            MaxSdkCallbacks.OnSdkInitializedEvent += sdkConfiguration =>
            {
     
            };

    
        }

        private async void Init()
        {
            Debug.Log("MAX SDK Initialized");
            await Task.Delay(2000);
            InitializeBannerAds();
            MaxSdk.ShowBanner(bannerAdUnitId);
            await Task.Delay(3000);
            InitializeInterstitialAds();
        }



        private void TryReloadAds()
        {
            if (interShowTime || firstime)
            {
                Reload();
                firstime = false;
            }
        }

        private void CheckLevel(int l)
        {
            if (l == 2 && !startInterstitial)
            {
                SendEventVideoAdsAvailable("success", "interstitial", "ad_on_win");
                MaxSdk.ShowInterstitial(adUnitId);
                startInterstitial = true;
                AddUnsafe(_ctx.ReloadAds.Subscribe(TryReloadAds));
                AddUnsafe(_ctx.Finish.Subscribe(TryShowInterstitial));
                disposables.Dispose();
            }
        }

        private async void Reload()
        {
            cancellation = new CancellationTokenSource();
            try
            {
                interShowTime = false;
                await Task.Delay(40000, cancellationToken: cancellation.Token);
                interShowTime = true;
                _ctx._interShowTime.Value = true;
            }
            finally
            {
                cancellation?.Dispose();
                cancellation = null;
            }
        }

        private void TryShowInterstitial()
        {
            if (!interShowTime) return;
            if (MaxSdk.IsInterstitialReady(adUnitId))
            {
                SendEventVideoAdsAvailable("success", "interstitial", "ad_on_win");
                MaxSdk.ShowInterstitial(adUnitId);
            }
            else SendEventVideoAdsAvailable("not_available", "interstitial", "ad_on_win");
        }

        #region banner
        public void InitializeBannerAds()
        {

            //MaxSdkCallbacks.OnBannerAdClickedEvent += BannerAdClickedEvent;
            MaxSdkCallbacks.OnBannerAdLoadedEvent += BannerAdLoadedEvent;
            // Adaptive banners are sized based on device width for positions that stretch full width (TopCenter and BottomCenter).
            // You may use the utility method `MaxSdkUtils.GetAdaptiveBannerHeight()` to help with view sizing adjustments
            MaxSdk.CreateBanner(bannerAdUnitId, MaxSdkBase.BannerPosition.BottomCenter);
            MaxSdk.SetBannerExtraParameter(bannerAdUnitId, "adaptive_banner", "true");
            // Set background or background color for banners to be fully functional
            MaxSdk.SetBannerBackgroundColor(bannerAdUnitId, Color.white);
        }

        private void BannerAdLoadedEvent(string bannerid)
        {
            if (interShowing || videoShowing) return;
            Dictionary<string, object> eventParameters = new Dictionary<string, object>();
            int _level = CalcLevel(_ctx.levelIndex.Value);
            eventParameters["ad_type"] = "banner";
            eventParameters["result"] = "watched";
            eventParameters["placement"] = "ad_in_level";
            eventParameters["connection"] = GetConnection();
            eventParameters["level_number"] = _level;
            eventParameters["level_name"] = _level;
            eventParameters["level_count"] = _ctx.levelCounter.Value;
            eventParameters["level_diff"] = "standart";
            eventParameters["level_loop"] = CalcLoop(_ctx.levelIndex.Value);
            appmetrica.ReportEvent("video_ads_watch", eventParameters);
            eventParameters.Clear();
            Debug.Log("banner send");
        }
        #endregion

        #region Interstitial
        public void InitializeInterstitialAds()
        {
            // Attach callback
            MaxSdkCallbacks.OnInterstitialLoadedEvent += OnInterstitialLoadedEvent;
            MaxSdkCallbacks.OnInterstitialLoadFailedEvent += OnInterstitialFailedEvent;
            MaxSdkCallbacks.OnInterstitialAdFailedToDisplayEvent += InterstitialFailedToDisplayEvent;
            MaxSdkCallbacks.OnInterstitialHiddenEvent += OnInterstitialDismissedEvent;
            MaxSdkCallbacks.OnInterstitialClickedEvent += InterstitialClickedEvent;
            MaxSdkCallbacks.OnInterstitialDisplayedEvent += InterstitialDisplayedEvent;
            // Load the first interstitial
            LoadInterstitial();
        }

        private void LoadInterstitial()
        {
            MaxSdk.LoadInterstitial(adUnitId);
        }

        private void OnInterstitialLoadedEvent(string adUnitId)
        {
            // Interstitial ad is ready to be shown. MaxSdk.IsInterstitialReady(adUnitId) will now return 'true'
            // Reset retry attempt
            retryAttempt = 0;
        }

        private void OnInterstitialFailedEvent(string adUnitId, int errorCode)
        {
            // Interstitial ad failed to load 
            // We recommend retrying with exponentially higher delays up to a maximum delay (in this case 64 seconds)

            retryAttempt++;
            double retryDelay = Math.Pow(2, Math.Min(6, retryAttempt));

            // Invoke("LoadInterstitial", (float)retryDelay);
            LateLoad((float)retryDelay);
        }

        private async void LateLoad(float time)
        {
            await Task.Delay((int)time * 1000);
            LoadInterstitial();
        }

        private void InterstitialFailedToDisplayEvent(string adUnitId, int errorCode)
        {
            // Interstitial ad failed to display. We recommend loading the next ad
            _ctx._interShowTime.Value = false;
            SendEventVideoAdsStarted("fail", "interstitial", "ad_on_win");
            LoadInterstitial();
            
        }

        private void OnInterstitialDismissedEvent(string adUnitId)
        {
            // Interstitial ad is hidden. Pre-load the next ad
            if (!interClicked)
            {
                SendEvendVideoAdsWatched("canceled", "interstitial", "ad_on_win");
                interShowing = false;
            } interClicked = false;

            LoadInterstitial();
        }

        private void InterstitialClickedEvent(string adUnitId)
        {
            SendEvendVideoAdsWatched("clicked", "interstitial", "ad_on_win");
            interShowing = false;
            interClicked = true;
        }

        private void InterstitialDisplayedEvent(string adUnitId)
        {
            _ctx._interShowTime.Value = false;
            SendEventVideoAdsStarted("start", "interstitial", "ad_on_win");
            if (!interShowing)
            {
                interShowing = true;
                InterWatched();
            }
        }

        private async void InterWatched()
        {
            await Task.Delay(30000);
            if (interShowing)
            {
                SendEvendVideoAdsWatched("watched", "interstitial", "ad_on_win");
                interShowing = false;
            }
        }

        #endregion

        private void SendEvendVideoAdsWatched(string result, string type, string placenemt )
        {
            var eventParameters = new Dictionary<string, object>();
            var _level = CalcLevel(_ctx.levelIndex.Value);
            eventParameters["ad_type"] = type;
            eventParameters["placement"] = placenemt;
            eventParameters["result"] = result;
            eventParameters["connection"] = GetConnection();
            eventParameters["level_number"] = _level;
            eventParameters["level_name"] = _level;
            eventParameters["level_count"] = _ctx.levelCounter.Value-1;
            eventParameters["level_diff"] = "standart";
            eventParameters["level_loop"] = CalcLoop(_ctx.levelIndex.Value-1);
            appmetrica.ReportEvent("video_ads_watch", eventParameters);
            eventParameters.Clear();
        }

        private void SendEventVideoAdsAvailable(string result, string type, string placenemt)
        {
            var eventParameters = new Dictionary<string, object>
            {
                ["ad_type"] = type,
                ["placement"] = placenemt,
                ["result"] = result,
                ["connection"] = GetConnection()
            };
            appmetrica.ReportEvent("video_ads_available", eventParameters);
        }

        private void SendEventVideoAdsStarted(string result, string type, string placenemt)
        {
            var eventParameters = new Dictionary<string, object>
            {
                ["ad_type"] = type,
                ["placement"] = placenemt,
                ["result"] = result,
                ["connection"] = GetConnection()
            };
            appmetrica.ReportEvent("video_ads_started", eventParameters);
        }

        private bool GetConnection() =>  Application.internetReachability == NetworkReachability.NotReachable ? false : true;
        
        private int CalcLoop(int i)
        {
            var cel = i / _ctx.levelCount;
            return cel+1;
        }

        private int CalcLevel(int i)
        {
            var cel = i / _ctx.levelCount;
            var ost = i - cel * _ctx.levelCount;
            return ost + 1;
        }

    }
}
