using JumpUp.External;

namespace CodeBase.Game.LevelParts.Player
{
    public class HitTriggerEntity : BaseDisposable
    {
        public struct Context
        {
           public HitTriggerView hitTriggerView;
           public ReactiveTrigger hit;
        }

        private Context _context;

        public HitTriggerEntity(Context context)
        {
            _context = context;
            var hitTriggerViewContext = new HitTriggerView.Context()
            {
                hit = _context.hit
            };
            _context.hitTriggerView.Init(hitTriggerViewContext);
        }
    }
}
