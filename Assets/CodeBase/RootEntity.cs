using CodeBase.Content;
using CodeBase.Game;
using CodeBase.UI;
using CodeBase.UI.Input;
using JumpUp;
using JumpUp.Content;
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
            public GameInfoView gameInfoView;
            public InputView inputview;
            public Transform tutor;
            public DigitalRubyShared.FingersJoystickScript controll;
            public Transform blocksContainer;
        }

        private readonly Ctx _ctx;
        private GameEntity _gameEntity;
        private UIEntity _uIEntity;
        private IContent _contentLoader;

        private readonly ReactiveProperty<GameState> _gameState;
        private readonly ReactiveProperty<int> _levelIndex;
        private readonly ReactiveProperty<int> _levelCounter;
        private readonly ReactiveEvent<Vector2> _moveCoordinates;
        private readonly ReactiveTrigger _start;
        private readonly ReactiveTrigger _gameOver;
        private readonly ReactiveTrigger _finish;
        private readonly ReactiveProperty<Transform> _player;
        private readonly ReactiveTrigger _onClick;
        private readonly ReactiveTrigger _showTutor; 
        private readonly ReactiveProperty<bool> _needBigTutor;
        private readonly ReactiveProperty<string> _winnerName; 
        private readonly ReactiveTrigger _startGame;

        private readonly ReactiveProperty<int> _playersRacePlace;


        public RootEntity(Ctx ctx)
        {
            _ctx = ctx;
            _moveCoordinates = new ReactiveEvent<Vector2>();
            _start =  new ReactiveTrigger();
            _gameOver = new ReactiveTrigger();
            _finish = new ReactiveTrigger();
            _gameState = new ReactiveProperty<GameState>();
            var level = PlayerPrefs.GetInt("level");
            _levelIndex = new ReactiveProperty<int>(level);
            var levelCounter = PlayerPrefs.GetInt("levelCounter");
            _levelCounter = new ReactiveProperty<int>(levelCounter);
            _player = new ReactiveProperty<Transform>();
            _onClick = new ReactiveTrigger();
            _showTutor = new ReactiveTrigger();
            _winnerName = new ReactiveProperty<string>();
            _needBigTutor = new ReactiveProperty<bool>(PlayerPrefs.GetInt("bigTutor") == 0);
            _playersRacePlace = new ReactiveProperty<int>();
            _startGame = new ReactiveTrigger();
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
                Start = _start,
                GameOver = _gameOver,
                Finish = _finish,
                moveCoor = _moveCoordinates,
                player = _player,
                _onClick = _onClick,
                tutor = _ctx.tutor,
                controll = _ctx.controll,
                blocksContainer = _ctx.blocksContainer,
                showTutor = _showTutor,
                needBigTutor = _needBigTutor,
                winnerName = _winnerName,
                startGame = _startGame,
                playersRacePlace = _playersRacePlace
            };
            _gameEntity = new GameEntity(gameEntityCtx);
            AddUnsafe(_gameEntity);
        }

        private void CreateUIEntity()
        {
            var uiEntityCtx = new UIEntity.Ctx()
            {
                gameState = _gameState,
                _levelIndex = _levelIndex,
                inputview = _ctx.inputview,
                moveCoor = _moveCoordinates,
                _onClick = _onClick,
                gameInfoView = _ctx.gameInfoView,
                controll = _ctx.controll,
                showTutor = _showTutor,
                winnerName = _winnerName,
                startGame = _startGame,
                levelCounter = _levelCounter,
                playersRacePlace = _playersRacePlace
            };
            _uIEntity = new UIEntity(uiEntityCtx);
            AddUnsafe(_uIEntity);
        }
        
    }
}
