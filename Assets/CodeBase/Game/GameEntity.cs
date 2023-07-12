using CodeBase.Content;
using CodeBase.Game.LevelParts.Level;
using JumpUp;
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
            public ReactiveTrigger start;
            public ReactiveTrigger gameOver;
            public ReactiveTrigger finish;
            public ReactiveEvent<Vector2> moveCoordinates;
            public ReactiveProperty<Transform> player;
            public ReactiveTrigger onClick;
            public ReactiveProperty<GameObject> endlessSignTutor;
            public Transform blocksContainer;
            public ReactiveProperty<bool> needStartTutor;
            public ReactiveProperty<string> winnerName;
            public ReactiveProperty<int> playersRacePlace;
            public Transform uiCanvas;
            public ReactiveTrigger countingIsOver;
            public ReactiveTrigger startRun;
        }

        private readonly Ctx _ctx;

        private readonly ReactiveTrigger _destroy = new ReactiveTrigger();
        private GamePlayEntity _gamePlayEntity;
        private GameInput _gameInput;
        
        public GameEntity(Ctx ctx)
        {
            _ctx = ctx;
            AddUnsafe(_ctx.start.Subscribe(() => StartLevel(_ctx.levelIndex.Value)));
            AddUnsafe(_ctx.gameOver.Subscribe(RestartLevel));
            AddUnsafe(_ctx.finish.Subscribe(() => StartLevel(_ctx.levelIndex.Value + 1)));
            AddUnsafe(_ctx.countingIsOver.Subscribe(() => { _ctx.gameState.Value = GameState.PLAY; }));
            var gameCyclePmCtx = new GameCyclePm.Ctx()
            {
                GameState = _ctx.gameState,
                OnClick = _ctx.onClick,
                GameOver = _ctx.gameOver,
                Finish = _ctx.finish,
                needStartTutor = _ctx.needStartTutor,
            };
            var gameCyclePm = new GameCyclePm(gameCyclePmCtx);
            AddUnsafe(gameCyclePm);
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
                moveCoordinates = _ctx.moveCoordinates,
                player = _ctx.player,
                endlessSignTutor = _ctx.endlessSignTutor,
                blocksContainer = _ctx.blocksContainer,
                needStartTutor = _ctx.needStartTutor,
                winnerName = _ctx.winnerName,
                startRun = _ctx.startRun,
                playersRacePlace = _ctx.playersRacePlace,
                uiCanvas = _ctx.uiCanvas
            };

            _gamePlayEntity = new GamePlayEntity(gamePlayEntityCtx, level);
            AddUnsafe(_gamePlayEntity);
        }
        
    }
}
