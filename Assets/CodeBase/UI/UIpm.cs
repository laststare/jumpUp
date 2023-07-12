using CodeBase.Content;
using Cysharp.Threading.Tasks;
using JumpUp.External;
using UniRx;
using UnityEngine;

namespace CodeBase.UI
{
    public class UIpm : BaseDisposable
    {
        public struct Ctx
        {
            public IContent content;
            public Transform uiCanvas;
            public IReactiveProperty<GameState> gameState;
            public ReactiveTrigger countingIsOver;
            public ReactiveProperty<GameObject> endlessSignTutor;
        }
        
        private readonly Ctx _ctx;
        private GameInfoView _gameInfoView;
        private StartCounterView _startCounterView;
        private WinTextView _winTextView;
        private GameObject _endlessSignTutor;
        private readonly ReactiveTrigger _showEndlessTutor = new ReactiveTrigger();


        public UIpm(Ctx ctx)
        {
            _ctx = ctx;
            CreateGameInfoView();
            CreateStartCounterView();
            CreateWinTextView();
            AddUnsafe(_showEndlessTutor.Subscribe(CreateEndlessSignTutor));
        }

        private void CreateGameInfoView()
        {
            _gameInfoView = Object.Instantiate(_ctx.content.GetGameInfoView(), _ctx.uiCanvas);
            _gameInfoView.SetMain(new GameInfoView.Ctx()
            {
                gameState = _ctx.gameState
            });
        }

        private void CreateStartCounterView()
        {
            _startCounterView = Object.Instantiate(_ctx.content.GetStartCounterView(), _ctx.uiCanvas);
            _startCounterView.SetMain(new StartCounterView.Ctx()
            {
                showEndlessTutor = _showEndlessTutor,
                countingIsOver = _ctx.countingIsOver,
                gameState = _ctx.gameState
            });
        }

        private void CreateEndlessSignTutor()
        {
            _ctx.endlessSignTutor.Value = Object.Instantiate(_ctx.content.GetEndlessSignTutor(), _ctx.uiCanvas);
        }

        private void CreateWinTextView()
        {
            _winTextView = Object.Instantiate(_ctx.content.GetWinTextView(), _ctx.uiCanvas);
            _winTextView.SetMain(new WinTextView.Ctx()
            {
                gameState = _ctx.gameState
            });
        }




    }
}