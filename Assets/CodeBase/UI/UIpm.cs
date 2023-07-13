using CodeBase.Content;
using Cysharp.Threading.Tasks;
using JumpUp.External;
using UniRx;
using UnityEngine;

namespace CodeBase.UI
{
    public class UIpm : BaseDisposable
    {
        public struct Context
        {
            public IContent content;
            public Transform uiCanvas;
            public IReactiveProperty<GameState> gameState;
            public ReactiveTrigger countingIsOver;
            public ReactiveProperty<GameObject> endlessSignTutor;
        }
        
        private readonly Context _context;
        private GameInfoView _gameInfoView;
        private StartCounterView _startCounterView;
        private WinTextView _winTextView;
        private GameObject _endlessSignTutor;
        private readonly ReactiveTrigger _showEndlessTutor = new ReactiveTrigger();


        public UIpm(Context context)
        {
            _context = context;
            CreateGameInfoView();
            CreateStartCounterView();
            CreateWinTextView();
            AddUnsafe(_showEndlessTutor.Subscribe(CreateEndlessSignTutor));
        }

        private void CreateGameInfoView()
        {
            _gameInfoView = Object.Instantiate(_context.content.GetGameInfoView(), _context.uiCanvas);
            _gameInfoView.Init(new GameInfoView.Context()
            {
                gameState = _context.gameState
            });
        }

        private void CreateStartCounterView()
        {
            _startCounterView = Object.Instantiate(_context.content.GetStartCounterView(), _context.uiCanvas);
            _startCounterView.Init(new StartCounterView.Context()
            {
                showEndlessTutor = _showEndlessTutor,
                countingIsOver = _context.countingIsOver,
                gameState = _context.gameState
            });
        }

        private void CreateEndlessSignTutor()
        {
            _context.endlessSignTutor.Value = Object.Instantiate(_context.content.GetEndlessSignTutor(), _context.uiCanvas);
        }

        private void CreateWinTextView()
        {
            _winTextView = Object.Instantiate(_context.content.GetWinTextView(), _context.uiCanvas);
            _winTextView.Init(new WinTextView.Context()
            {
                gameState = _context.gameState
            });
        }




    }
}