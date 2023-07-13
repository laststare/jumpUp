using CodeBase.Content;
using CodeBase.Game.LevelParts.Level;
using JumpUp.External;
using UniRx;
using UnityEngine;

namespace CodeBase.Game
{
    public class GameEntity : BaseDisposable
    {
        public struct Context
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

        private readonly Context _context;

        private readonly ReactiveTrigger _destroy = new ReactiveTrigger();
        private GamePlayEntity _gamePlayEntity;
        
        public GameEntity(Context context)
        {
            _context = context;
            AddUnsafe(_context.start.Subscribe(() => StartLevel(_context.levelIndex.Value)));
            AddUnsafe(_context.gameOver.Subscribe(RestartLevel));
            AddUnsafe(_context.finish.Subscribe(() => StartLevel(_context.levelIndex.Value + 1)));
            AddUnsafe(_context.countingIsOver.Subscribe(() => { _context.gameState.Value = GameState.PLAY; }));
            var gameCyclePmContext = new GameCyclePm.Context()
            {
                GameState = _context.gameState,
                OnClick = _context.onClick,
                GameOver = _context.gameOver,
                Finish = _context.finish,
                needStartTutor = _context.needStartTutor,
            };
            var gameCyclePm = new GameCyclePm(gameCyclePmContext);
            AddUnsafe(gameCyclePm);
        }

        #region levels      

        private void StartLevel(int levelIndex)
        {
            PlayerPrefs.SetInt("level", levelIndex);
            PreloadLevel(levelIndex);
        }

        private void RestartLevel() => PreloadLevel(_context.levelIndex.Value);

        private void PreloadLevel(int levelIndex)
        {
            _destroy.Notify();
            _context.levelIndex.Value = levelIndex;
            var level = new Level(_context.contentLoader.GetLevelContainer(levelIndex));
            CreateGamePlayEntity(level);  
            _context.gameState.Value = GameState.START;
        }

        #endregion

        private void CreateGamePlayEntity(Level level)
        {
            var gamePlayEntityContext = new GamePlayEntity.Context()
            {
                contentLoader = _context.contentLoader,
                gameState = _context.gameState,
                destroy = _destroy,
                levelIndex = _context.levelIndex,
                moveCoordinates = _context.moveCoordinates,
                player = _context.player,
                endlessSignTutor = _context.endlessSignTutor,
                blocksContainer = _context.blocksContainer,
                needStartTutor = _context.needStartTutor,
                winnerName = _context.winnerName,
                startRun = _context.startRun,
                playersRacePlace = _context.playersRacePlace,
                uiCanvas = _context.uiCanvas
            };

            _gamePlayEntity = new GamePlayEntity(gamePlayEntityContext, level);
            AddUnsafe(_gamePlayEntity);
        }
        
    }
}
