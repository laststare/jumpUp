using CodeBase.Content;
using CodeBase.Game;
using CodeBase.UI;
using CodeBase.UI.Input;
using JumpUp;
using JumpUp.External;
using UniRx;
using UnityEngine;

namespace CodeBase
{
    public class RootEntity : BaseDisposable
    {
        public struct Ctx
        {
            public PrefabsInfo prefabs;
            public InputView inputview;
            public DigitalRubyShared.FingersJoystickScript controll;
            public Transform blocksContainer;
            public Transform uiCanvas;
        }

        private readonly Ctx _ctx;
        private GameEntity _gameEntity;
        private UIEntity _uIEntity;
        private IContent _contentLoader;

        private readonly ReactiveProperty<GameState> _gameState = new ReactiveProperty<GameState>();
        private readonly ReactiveProperty<int> _levelIndex;
        private readonly ReactiveProperty<int> _levelCounter;
        private readonly ReactiveEvent<Vector2> _moveCoordinates = new ReactiveEvent<Vector2>();
        private readonly ReactiveTrigger _start =  new ReactiveTrigger();
        private readonly ReactiveTrigger _gameOver = new ReactiveTrigger();
        private readonly ReactiveTrigger _finish = new ReactiveTrigger();
        private readonly ReactiveProperty<Transform> _player = new ReactiveProperty<Transform>();
        private readonly ReactiveTrigger _onClick = new ReactiveTrigger();
        private readonly ReactiveProperty<bool> _needStartTutor;
        private readonly ReactiveProperty<string> _winnerName = new ReactiveProperty<string>();
        private readonly ReactiveTrigger _startRun = new ReactiveTrigger();
        private readonly ReactiveTrigger _countingIsOver = new ReactiveTrigger();
        private readonly ReactiveProperty<int> _playersRacePlace = new ReactiveProperty<int>();
        private readonly ReactiveProperty<GameObject> _endlessSignTutor = new ReactiveProperty<GameObject>();



        public RootEntity(Ctx ctx)
        {
            _ctx = ctx;
            var level = PlayerPrefs.GetInt("level");
            _levelIndex = new ReactiveProperty<int>(level);
            var levelCounter = PlayerPrefs.GetInt("levelCounter");
            _levelCounter = new ReactiveProperty<int>(levelCounter);
            _needStartTutor = new ReactiveProperty<bool>(PlayerPrefs.GetInt("startTutor") == 0);
            Init();
        }

        private void Init()
        {
            CreateContentLoader();
            CreateUIEntity();
            CreateGameEntity();
            _gameOver.Notify();  
        }

        private void CreateContentLoader()
        {
            var contentLoaderCtx = new ContentLoader.Ctx
            {
                Prefabs = _ctx.prefabs
            };
            _contentLoader = new ContentLoader(contentLoaderCtx);
            AddUnsafe(_contentLoader);
        }

        private void CreateGameEntity()
        {
            var gameEntityCtx = new GameEntity.Ctx
            {
                contentLoader = _contentLoader,
                levelIndex = _levelIndex,
                gameState = _gameState,
                start = _start,
                gameOver = _gameOver,
                finish = _finish,
                moveCoordinates = _moveCoordinates,
                player = _player,
                onClick = _onClick,
                endlessSignTutor = _endlessSignTutor,
                blocksContainer = _ctx.blocksContainer,
                needStartTutor = _needStartTutor,
                winnerName = _winnerName,
                playersRacePlace = _playersRacePlace,
                uiCanvas = _ctx.uiCanvas,
                countingIsOver = _countingIsOver,
                startRun = _startRun
            };
            _gameEntity = new GameEntity(gameEntityCtx);
            AddUnsafe(_gameEntity);
        }

        private void CreateUIEntity()
        {
            var uiEntityCtx = new UIEntity.Ctx()
            {
                gameState = _gameState,
                inputview = _ctx.inputview,
                moveCoordinates = _moveCoordinates,
                _onClick = _onClick,
                controll = _ctx.controll,
                content = _contentLoader,
                uiCanvas = _ctx.uiCanvas,
                countingIsOver = _countingIsOver,
                endlessSignTutor = _endlessSignTutor
            };
            _uIEntity = new UIEntity(uiEntityCtx);
            AddUnsafe(_uIEntity);
        }
        
    }
}
