using System.Collections.Generic;
using CodeBase.Content;
using CodeBase.Game.LevelParts.Camera;
using CodeBase.Game.LevelParts.Level;
using CodeBase.Game.LevelParts.Player;
using JumpUp;
using JumpUp.External;
using UniRx;
using UnityEngine;

namespace CodeBase.Game {
    public class GamePlayEntity : BaseDisposable
    {
        public struct Ctx
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

        private readonly Ctx _ctx;
        public GamePlayEntity(Ctx ctx, Level level)
        {
            _ctx = ctx;
            AddUnsafe(_ctx.destroy.Subscribe(DestroyEverything));
            CreatePlayerEntity(level);
            CreateLevelEntity(level);
            CreateCameraEntity();
        }

        #region Create Entities
      

        private void CreateLevelEntity(Level level)
        {
            var levelEntityCtx = new LevelEntity.Ctx
            {
                content = _ctx.contentLoader,
                gameState = _ctx.gameState,
                Level = level,
                player = _ctx.player,
                floorPart = _floorPart,
                roofPart = _roofPart,
                blocksContainer = _ctx.blocksContainer,
                destroy = _ctx.destroy,
                levelIndex = _ctx.levelIndex,
                camera = _camera,
                players = _players,
                leader = _leader,
                winnerName = _ctx.winnerName,
                playersRacePlace = _ctx.playersRacePlace
            };
            _levelEntity = new LevelEntity(levelEntityCtx);
            AddUnsafe(_levelEntity);
        }

        private void CreatePlayerEntity(Level level)
        {
            var playerEntityCtx = new PlayerEntity.Ctx
            {
                content = _ctx.contentLoader,
                moveCoordinates = _ctx.moveCoordinates,
                player = _ctx.player,
                gameState = _ctx.gameState,
                Level = level,
                endlessSignTutor = _ctx.endlessSignTutor,
                floorPart = _floorPart,
                roofPart = _roofPart,
                shake = _shake,
                flyup = _flyUp,
                camera = _camera,
                players = _players,
                leader = _leader,
                startRun = _ctx.startRun,
            };
            _playerEntity = new PlayerEntity(playerEntityCtx);
            AddUnsafe(_playerEntity);
        }

        private void CreateCameraEntity()
        {
            var cameraEntityCtx = new CameraEntity.Ctx
            {
                content = _ctx.contentLoader,
                player = _ctx.player,
                gameState = _ctx.gameState,
                shake = _shake,
                flyup = _flyUp,
                camera = _camera,
                needStartTutor = _ctx.needStartTutor,
                uiCanvas = _ctx.uiCanvas
            };
            _cameraEntity = new CameraEntity(cameraEntityCtx);
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
