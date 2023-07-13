using JumpUp.External;
using UniRx;

namespace CodeBase.Game
{
    public class GameCyclePm : BaseDisposable
    {
        public struct Context
        {
            public IReactiveProperty<GameState> GameState;
            public IReadOnlyReactiveTrigger OnClick;
            public ReactiveTrigger GameOver;
            public ReactiveTrigger Finish;
            public ReactiveProperty<bool> needStartTutor;
        }


        private readonly Context  _context;
        private GameState  _currenGameState;

        public GameCyclePm(Context context)
        {
            _context = context;
            AddUnsafe(context.GameState.Subscribe(x => _currenGameState = x));
            AddUnsafe(_context.OnClick.Subscribe(GetClick));
        }


        private void GetClick()
        {
            switch (_currenGameState)
            {
                case GameState.START:
                        _context.GameState.Value = _context.needStartTutor.Value? GameState.STARTTUTOR : GameState.COUNTER;
                    break;
                case GameState.FINISH:
                       _context.Finish.Notify();
                    break;
                case GameState.GAMEOVER:
                      _context.GameOver.Notify();
                    break;
            }
        }

    }
}
