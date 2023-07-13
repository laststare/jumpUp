using JumpUp.External;
using UniRx;
using UnityEngine;

namespace CodeBase.Game.LevelParts.Finish
{
    public class FinishEntity : BaseDisposable
    {
        public struct Context
        {
            public ReactiveProperty<Transform> player;
            public FinishView view;
            public IReactiveProperty<GameState> gameState;
        }


        private readonly Context _context;
        private readonly FinishView _view;

        public FinishEntity(Context context, Vector3 finishPlace)
        {
            _context = context;
            _view = Object.Instantiate(_context.view);
            _view.transform.position = finishPlace;
            var finishViewContext = new FinishView.Context()
            {
                player = _context.player,
                gameState = _context.gameState,
            };
            _view.Init(finishViewContext);
        }


        protected override void OnDispose()
        {
            base.OnDispose();
            if (_view != null)
            {
                Object.Destroy(_view.gameObject);
            }
        }

    }
}
