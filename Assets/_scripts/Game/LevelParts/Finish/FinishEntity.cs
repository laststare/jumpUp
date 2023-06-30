using JumpUp;
using JumpUp.Content;
using JumpUp.External;
using JumpUp.Player;
using UniRx;
using UnityEngine;

namespace _scripts.Game.LevelParts.Finish
{
    public class FinishEntity : BaseDisposable
    {
        public struct Ctx
        {
            public IContent content;
            public ReactiveProperty<Transform> player;
            public FinishView view;
            public IReactiveProperty<GameState> gameState;
        }


        private Ctx _ctx;
        private FinishView _view;

        public FinishEntity(Ctx ctx, Vector3 finishPlace)
        {
            _ctx = ctx;
            _view = Object.Instantiate(_ctx.view);
            _view.transform.position = finishPlace;
            var finishViewCtx = new FinishView.Ctx()
            {
                player = _ctx.player,
                gameState = _ctx.gameState,
            };
            _view.SetMain(finishViewCtx);
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
