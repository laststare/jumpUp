using JumpUp.External;

namespace _scripts.Game.LevelParts.Player
{
    public class HitTriggerEntity : BaseDisposable
    {
        public struct Ctx
        {
           public HitTriggerView hitTriggerView;
           public ReactiveTrigger hit;
        }

        private Ctx _ctx;

        public HitTriggerEntity(Ctx ctx)
        {
            _ctx = ctx;
            var hitTriggerViewCtx = new HitTriggerView.Ctx()
            {
                hit = _ctx.hit
            };
            _ctx.hitTriggerView.SetMain(hitTriggerViewCtx);
        }
    }
}
