using System.Collections.Generic;
using CodeBase.Content;
using CodeBase.Game.LevelParts.Camera;
using CodeBase.Game.LevelParts.Level;
using CodeBase.Game.LevelParts.Player;
using JumpUp.External;
using UniRx;
using UnityEngine;

namespace CodeBase.Game {
    public class GamePlayEntity : BaseDisposable
    {
        public struct Context
        {
            public IContent contentLoader;
            public IReactiveProperty<GameState> gameState; 
            public ReactiveTrigger destroy;
            public IReactiveProperty<int> levelIndex;
            public ReactiveEvent<Vector2> moveCoordinates;
            public ReactiveProperty<Transform> player;
            public ReactiveProperty<GameObject> endlessSignTutor;
            public Transform blocksContainer;
            public ReactiveProperty<bool> needStartTutor;
            public ReactiveProperty<string> winnerName;
            public ReactiveTrigger startRun;
            public ReactiveProperty<int> playersRacePlace;
            public Transform uiCanvas;
        }

        private PlayerEntity _playerEntity;
        private LevelEntity _levelEntity;
        private CameraEntity _cameraEntity;
        private readonly ReactiveEvent<GameObject> _floorPart = new ReactiveEvent<GameObject>();
        private readonly ReactiveEvent<GameObject> _roofPart = new ReactiveEvent<GameObject>();
        private readonly ReactiveEvent<float> _shake = new ReactiveEvent<float>();
        private readonly ReactiveEvent<bool> _flyUp = new ReactiveEvent<bool>();
        private readonly ReactiveProperty<Camera> _camera = new ReactiveProperty<Camera>();
        private readonly ReactiveProperty<List<Transform>> _players = new ReactiveProperty<List<Transform>>(new List<Transform>());
        private readonly ReactiveEvent<Transform> _leader = new ReactiveEvent<Transform>();

        private readonly Context _context;
        public GamePlayEntity(Context context, Level level)
        {
            _context = context;
            AddUnsafe(_context.destroy.Subscribe(DestroyEverything));
            CreatePlayerEntity(level);
            CreateLevelEntity(level);
            CreateCameraEntity();
        }

        #region Create Entities
      

        private void CreateLevelEntity(Level level)
        {
            var levelEntityContext = new LevelEntity.Context
            {
                content = _context.contentLoader,
                gameState = _context.gameState,
                Level = level,
                player = _context.player,
                floorPart = _floorPart,
                roofPart = _roofPart,
                blocksContainer = _context.blocksContainer,
                destroy = _context.destroy,
                levelIndex = _context.levelIndex,
                camera = _camera,
                players = _players,
                leader = _leader,
                winnerName = _context.winnerName,
                playersRacePlace = _context.playersRacePlace
            };
            _levelEntity = new LevelEntity(levelEntityContext);
            AddUnsafe(_levelEntity);
        }

        private void CreatePlayerEntity(Level level)
        {
            var playerEntityContext = new PlayerEntity.Context
            {
                content = _context.contentLoader,
                moveCoordinates = _context.moveCoordinates,
                player = _context.player,
                gameState = _context.gameState,
                Level = level,
                endlessSignTutor = _context.endlessSignTutor,
                floorPart = _floorPart,
                roofPart = _roofPart,
                shake = _shake,
                flyup = _flyUp,
                camera = _camera,
                players = _players,
                leader = _leader,
                startRun = _context.startRun,
            };
            _playerEntity = new PlayerEntity(playerEntityContext);
            AddUnsafe(_playerEntity);
        }

        private void CreateCameraEntity()
        {
            var cameraEntityContext = new CameraEntity.Context
            {
                content = _context.contentLoader,
                player = _context.player,
                gameState = _context.gameState,
                shake = _shake,
                flyup = _flyUp,
                camera = _camera,
                needStartTutor = _context.needStartTutor,
                uiCanvas = _context.uiCanvas
            };
            _cameraEntity = new CameraEntity(cameraEntityContext);
            AddUnsafe(_cameraEntity);
        }

        #endregion

        #region Destroying Entities

        private void DestroyEverything()
        {
            DestroyPlayerEntity();
            DestroyLevelEntity();
            DestroyCameraEntity();
        }

        private void DestroyPlayerEntity()
        {
            _playerEntity?.Dispose();
            _playerEntity = null;
        }
        private void DestroyLevelEntity()
        {
            _levelEntity?.Dispose();
            _levelEntity = null;
        }
        private void DestroyCameraEntity()
        {
            _cameraEntity?.Dispose();
            _cameraEntity = null;
        }
        #endregion

    }
}
