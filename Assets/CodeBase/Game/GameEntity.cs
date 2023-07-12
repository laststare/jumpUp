using CodeBase.Game.LevelParts.Level;
using JumpUp;
using JumpUp.Content;
using JumpUp.External;
using UniRx;
using UnityEngine;

namespace CodeBase.Game
{
    public class GameEntity : BaseDisposable
    {
        public struct Ctx
        {
            public IContent contentLoader;
            public IReactiveProperty<int> levelIndex;
            public IReactiveProperty<GameState> gameState;
            public ReactiveTrigger Start;
            public ReactiveTrigger GameOver;
            public ReactiveTrigger Finish;
            public ReactiveEvent<Vector2> moveCoor;
            public ReactiveProperty<Transform> player;
            public ReactiveTrigger _onClick;
            public Transform tutor;
            public DigitalRubyShared.FingersJoystickScript controll;
            public Transform blocksContainer;
            public ReactiveTrigger showTutor;
            public ReactiveProperty<bool> needBigTutor;
            public ReactiveProperty<string> winnerName;
            public ReactiveTrigger startGame;
            public ReactiveProperty<int> playersRacePlace;
        }

        private readonly Ctx _ctx;
        
        private readonly ReactiveTrigger _destroy;
        private GamePlayEntity _gamePlayEntity;
        private GameInput _gameInput;
        private IReadOnlyReactiveTrigger _onStart;
        private IReadOnlyReactiveTrigger _onGameOver;
        private IReadOnlyReactiveTrigger _onFinish;


        public GameEntity(Ctx ctx)
        {
            _ctx = ctx;
            _destroy = new ReactiveTrigger();
            _onStart = _ctx.Start;
            _onGameOver = _ctx.GameOver;
            _onFinish = _ctx.Finish;

            AddUnsafe(_onStart.Subscribe(() => StartLevel(_ctx.levelIndex.Value)));
            AddUnsafe(_onGameOver.Subscribe(RestartLevel));
            AddUnsafe(_onFinish.Subscribe(() => StartLevel(_ctx.levelIndex.Value + 1)));
            var gamePmCtx = new GamePm.Ctx()
            {
                GameState = _ctx.gameState,
                OnClick = _ctx._onClick,
                GameOver = _ctx.GameOver,
                Finish = _ctx.Finish,
                NeedBigTutor = _ctx.needBigTutor,
            };
            var gamePm = new GamePm(gamePmCtx);
            AddUnsafe(gamePm);
        }

        #region levels      

        private void StartLevel(int levelIndex)
        {
            PlayerPrefs.SetInt("level", levelIndex);
            PreloadLevel(levelIndex);
        }

        private void RestartLevel() => PreloadLevel(_ctx.levelIndex.Value);

        private void PreloadLevel(int levelIndex)
        {
            _destroy.Notify();
            _ctx.levelIndex.Value = levelIndex;
            var level = new Level(_ctx.contentLoader.GetLevelContainer(levelIndex));
            CreateGamePlayEntity(level);  
            _ctx.gameState.Value = GameState.START;
        }

        #endregion

        private void CreateGamePlayEntity(Level level)
        {
            var gamePlayEntityCtx = new GamePlayEntity.Ctx()
            {
                contentLoader = _ctx.contentLoader,
                gameState = _ctx.gameState,
                destroy = _destroy,
                levelIndex = _ctx.levelIndex,
                moveCoor = _ctx.moveCoor,
                player = _ctx.player,
                tutor = _ctx.tutor,
                blocksContainer = _ctx.blocksContainer,
                showTutor = _ctx.showTutor,
                needBigTutor = _ctx.needBigTutor,
                winnerName = _ctx.winnerName,
                startGame = _ctx.startGame,
                playersRacePlace = _ctx.playersRacePlace
            };

            _gamePlayEntity = new GamePlayEntity(gamePlayEntityCtx, level);
            AddUnsafe(_gamePlayEntity);
        }
        
    }
}
