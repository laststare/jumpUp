using JumpUp;
using JumpUp.External;
using UniRx;

namespace CodeBase.Game
{
    public class GamePm : BaseDisposable
    {
        public struct Ctx
        {
            public IReactiveProperty<GameState> GameState;
            public IReadOnlyReactiveTrigger OnClick;
            public ReactiveTrigger GameOver;
            public ReactiveTrigger Finish;
            public ReactiveProperty<bool> NeedBigTutor;
        }


        private readonly Ctx  _ctx;
        private GameState  _currenGameState;

        public GamePm(Ctx ctx)
        {
            _ctx = ctx;
            AddUnsafe(ctx.GameState.Subscribe(x => _currenGameState = x));
            AddUnsafe(_ctx.OnClick.Subscribe(GetClick));
        }


        private void GetClick()
        {
            switch (_currenGameState)
            {
                case GameState.NONE:
                    break;
                case GameState.START:
                        _ctx.GameState.Value = _ctx.NeedBigTutor.Value? GameState.BIGTUTOR : GameState.COUNTER;
                    break;
                case GameState.PLAY:
                    break;
                case GameState.FINISH:
                       _ctx.Finish.Notify();
                    break;
                case GameState.GAMEOVER:
                      _ctx.GameOver.Notify();
                    break;
                case GameState.TUTOR:
                    break;
            }
        }

    }
}
