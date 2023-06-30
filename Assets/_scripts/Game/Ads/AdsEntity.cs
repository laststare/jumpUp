using JumpUp.Content;
using JumpUp.External;
using UniRx;

namespace _scripts.Game.Ads
{
    public class AdsEntity : BaseDisposable
    {
        public struct Ctx
        {
            public IContent content;
            public ReactiveTrigger Finish;
            public IReactiveProperty<int> levelIndex;
            public ReactiveTrigger ReloadAds;
            public ReactiveProperty<int> levelCounter;
            public ReactiveProperty<bool> interShowTime;
        }

        private Ctx _ctx;
        private AdsPm pm;
        public AdsEntity(Ctx Ctx)
        {
            _ctx = Ctx;

            var adsPmCtx = new AdsPm.Ctx()
            {
                levelCount = _ctx.content.GetLevelCount(),
                Finish = _ctx.Finish,
                levelIndex = _ctx.levelIndex,
                ReloadAds = _ctx.ReloadAds,
                levelCounter = _ctx.levelCounter,
                _interShowTime = _ctx.interShowTime
            };
            pm = new AdsPm(adsPmCtx);
            AddUnsafe(pm);
        }
    }
}
