using CodeBase.Content;
using CodeBase.Control;
using CodeBase.Game;
using CodeBase.UI;
using CodeBase.UI.Input;
using JumpUp.External;
using UniRx;
using UnityEngine;

namespace CodeBase
{
    public class RootEntity : BaseDisposable
    {
        public struct Context
        {
            public PrefabsInfo prefabs;
            public DigitalRubyShared.FingersJoystickScript controll;
            public Transform blocksContainer;
            public Transform uiCanvas;
        }

        private readonly Context _context;
        private GameEntity _gameEntity;
        private UIEntity _uIEntity;
        private ControlEntity _controlEntity;
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
        

        public RootEntity(Context context)
        {
            _context = context;
            _levelIndex = new ReactiveProperty<int>(PlayerPrefs.GetInt("level"));
            _levelCounter = new ReactiveProperty<int>(PlayerPrefs.GetInt("levelCounter"));
            _needStartTutor = new ReactiveProperty<bool>(PlayerPrefs.GetInt("startTutor") == 0);
            Init();
        }

        private void Init()
        {
            CreateContentLoader();
            CreateControlEntity();
            CreateUIEntity();
            CreateGameEntity();
            _gameOver.Notify();  
        }

        private void CreateContentLoader()
        {
            var contentLoaderContext = new ContentLoader.Context
            {
                Prefabs = _context.prefabs
            };
            _contentLoader = new ContentLoader(contentLoaderContext);
            AddUnsafe(_contentLoader);
        }
        
        private void CreateControlEntity()
        {
            var controlEntityContext = new ControlEntity.Context()
            {
                moveCoordinates = _moveCoordinates,
                onClick = _onClick,
                controll = _context.controll,
                gameState = _gameState,
                uiCanvas = _context.uiCanvas,
                content = _contentLoader
            };
            _controlEntity = new ControlEntity(controlEntityContext);
            AddUnsafe(_controlEntity);
        }

        private void CreateGameEntity()
        {
            var gameEntityContext = new GameEntity.Context
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
                blocksContainer = _context.blocksContainer,
                needStartTutor = _needStartTutor,
                winnerName = _winnerName,
                playersRacePlace = _playersRacePlace,
                uiCanvas = _context.uiCanvas,
                countingIsOver = _countingIsOver,
                startRun = _startRun
            };
            _gameEntity = new GameEntity(gameEntityContext);
            AddUnsafe(_gameEntity);
        }

        private void CreateUIEntity()
        {
            var uiEntityContext = new UIEntity.Context()
            {
                gameState = _gameState,
                content = _contentLoader,
                uiCanvas = _context.uiCanvas,
                countingIsOver = _countingIsOver,
                endlessSignTutor = _endlessSignTutor
            };
            _uIEntity = new UIEntity(uiEntityContext);
            AddUnsafe(_uIEntity);
        }

    }
}
