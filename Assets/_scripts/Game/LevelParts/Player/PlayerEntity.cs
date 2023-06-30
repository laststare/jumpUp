using System.Collections.Generic;
using JumpUp;
using JumpUp.Content;
using JumpUp.External;
using JumpUp.Player;
using UniRx;
using UnityEngine;

namespace _scripts.Game.LevelParts.Player
{
    public class PlayerEntity : BaseDisposable
    {
        public struct Ctx
        {
            public IContent content;
            public ReactiveEvent<Vector2> moveCoor;
            public ReactiveProperty<Transform> player;
            public IReactiveProperty<GameState> gameState;
            public Level.Level Level;
            public Transform tutor;
            public ReactiveEvent<GameObject> floorPart;
            public ReactiveEvent<GameObject> roofPart;
            public ReactiveEvent<float> shake;
            public ReactiveEvent<bool> flyup;
            public ReactiveProperty<UnityEngine.Camera> camera;
            public ReactiveProperty<List<Transform>> players;
            public ReactiveEvent<Transform> leader;
            public ReactiveTrigger showTutor;
            public ReactiveTrigger startGame;
            public ReactiveTrigger ReloadAds;
        }

        private Ctx _ctx;
        private PlayerView _view;
        private PlayerPm _pm;
        private readonly ReactiveProperty<Transform> _playerBody;
        private readonly ReactiveProperty<Transform> _rayPlace;
        private readonly ReactiveProperty<Vector3> _moveDirection;
        private readonly ReactiveTrigger _die;
        private readonly ReactiveTrigger _finish;

        private readonly ReactiveProperty<Transform> _smallJumpSearcher;
        private readonly ReactiveProperty<LayerMask> _mask;
        private readonly ReactiveProperty<LayerMask> _maskUpper;
        private readonly ReactiveProperty<Transform> _name;
        public PlayerEntity(Ctx ctx)
        {
            _ctx = ctx;
            _playerBody = new ReactiveProperty<Transform>();
            _rayPlace = new ReactiveProperty<Transform>();
            _moveDirection = new ReactiveProperty<Vector3>();
            _die = new ReactiveTrigger();
            _finish = new ReactiveTrigger();

            _smallJumpSearcher = new ReactiveProperty<Transform>();
            _mask = new ReactiveProperty<LayerMask>();
            _maskUpper = new ReactiveProperty<LayerMask>();
            _name = new ReactiveProperty<Transform>();

            var playerPmCtx = new PlayerPm.Ctx()
            {
                content = _ctx.content,
                Level = _ctx.Level,
                player = _ctx.player,
                playerBody = _playerBody,
                moveDirection = _moveDirection,
                gameState = _ctx.gameState,
                moveCoor = _ctx.moveCoor,
                tutor = _ctx.tutor,
                die = _die,
                startGame = _ctx.startGame,
                floorPart = _ctx.floorPart,
                roofPart =_ctx.roofPart,
                smallJumpSearcher = _smallJumpSearcher,
                rayPlace = _rayPlace,
                mask = _mask,
                finish = _finish,
                camera = _ctx.camera,
                _name = _name,
                showTutor = _ctx.showTutor,
                maskUpper = _maskUpper,
                ReloadAds = _ctx.ReloadAds
            };
            _pm = new PlayerPm(playerPmCtx);
            AddUnsafe(_pm);


            _view = Object.Instantiate(_ctx.content.GetPlayerView()).GetComponent<PlayerView>();
            
            _view.SetMain(new PlayerView.Ctx
            {
                player = _ctx.player,
                playerBody = _playerBody,
                moveDirection = _moveDirection,
                die = _die,
                startGame = _ctx.startGame,
                smallJumpSearcher = _smallJumpSearcher,
                rayPlace = _rayPlace,
                mask = _mask,
                shake = _ctx.shake,
                finish = _finish,
                flyup = _ctx.flyup,
                _name = _name,
                leader = _ctx.leader,
                players = _ctx.players,
                maskUpper = _maskUpper
            });
    
       }

        protected override void OnDispose()
        {
            base.OnDispose();

            if (_view != null)
            {
                Object.Destroy(_view.gameObject);
            }
        }


    }
}

