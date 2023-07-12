using System.Collections.Generic;
using CodeBase.Game.LevelParts.Camera;
using CodeBase.Game.LevelParts.Level;
using CodeBase.Game.LevelParts.Player;
using JumpUp;
using JumpUp.Content;
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
            public ReactiveEvent<Vector2> moveCoor;
            public ReactiveProperty<Transform> player;
            public Transform tutor;
            public Transform blocksContainer;
            public ReactiveTrigger showTutor;
            public ReactiveProperty<bool> needBigTutor;
            public ReactiveProperty<string> winnerName;
            public ReactiveTrigger startGame;
            public ReactiveProperty<int> playersRacePlace;
        }

        private PlayerEntity _playerEntity;
        private LevelEntity _levelEntity;
        private CameraEntity _cameraEntity;
        private readonly ReactiveEvent<GameObject> _floorPart;
        private readonly ReactiveEvent<GameObject> _roofPart;
        private readonly ReactiveEvent<float> _shake;
        private readonly ReactiveEvent<bool> _flyUp;
        private readonly ReactiveProperty<Camera> _camera;
        private readonly ReactiveProperty<List<Transform>> _players;
        private readonly ReactiveEvent<Transform> _leader;

        private readonly Ctx _ctx;
        public GamePlayEntity(Ctx ctx, Level level)
        {
            _ctx = ctx;
            _floorPart = new ReactiveEvent<GameObject>();
            _roofPart = new ReactiveEvent<GameObject>();
            _shake = new ReactiveEvent<float>();
            _flyUp = new ReactiveEvent<bool>();
            _camera = new ReactiveProperty<UnityEngine.Camera>();
            _players = new ReactiveProperty<List<Transform>>(new List<Transform>());
            _leader = new ReactiveEvent<Transform>();
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
                moveCoor = _ctx.moveCoor,
                player = _ctx.player,
                gameState = _ctx.gameState,
                Level = level,
                tutor = _ctx.tutor,
                floorPart = _floorPart,
                roofPart = _roofPart,
                shake = _shake,
                flyup = _flyUp,
                camera = _camera,
                players = _players,
                leader = _leader,
                showTutor = _ctx.showTutor,
                startGame = _ctx.startGame,
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
                needBigTutor = _ctx.needBigTutor
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
