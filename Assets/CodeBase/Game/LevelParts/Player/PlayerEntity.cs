using System.Collections.Generic;
using CodeBase.Content;
using JumpUp.External;
using UniRx;
using UnityEngine;

namespace CodeBase.Game.LevelParts.Player
{
    public class PlayerEntity : BaseDisposable
    {
        public struct Context
        {
            public IContent content;
            public ReactiveEvent<Vector2> moveCoordinates;
            public ReactiveProperty<Transform> player;
            public IReactiveProperty<GameState> gameState;
            public Level.Level Level;
            public ReactiveProperty<GameObject> endlessSignTutor;
            public ReactiveEvent<GameObject> floorPart;
            public ReactiveEvent<GameObject> roofPart;
            public ReactiveEvent<float> shake;
            public ReactiveEvent<bool> flyup;
            public ReactiveProperty<UnityEngine.Camera> camera;
            public ReactiveProperty<List<Transform>> players;
            public ReactiveEvent<Transform> leader;
            public ReactiveTrigger startRun;
        }

        private Context _context;
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
        public PlayerEntity(Context context)
        {
            _context = context;
            _playerBody = new ReactiveProperty<Transform>();
            _rayPlace = new ReactiveProperty<Transform>();
            _moveDirection = new ReactiveProperty<Vector3>();
            _die = new ReactiveTrigger();
            _finish = new ReactiveTrigger();

            _smallJumpSearcher = new ReactiveProperty<Transform>();
            _mask = new ReactiveProperty<LayerMask>();
            _maskUpper = new ReactiveProperty<LayerMask>();
            _name = new ReactiveProperty<Transform>();

            var playerPmContext = new PlayerPm.Context()
            {
                content = _context.content,
                Level = _context.Level,
                player = _context.player,
                playerBody = _playerBody,
                moveDirection = _moveDirection,
                gameState = _context.gameState,
                moveCoordinates = _context.moveCoordinates,
                endlessSignTutor = _context.endlessSignTutor,
                die = _die,
                startRun = _context.startRun,
                floorPart = _context.floorPart,
                roofPart =_context.roofPart,
                smallJumpSearcher = _smallJumpSearcher,
                rayPlace = _rayPlace,
                mask = _mask,
                finish = _finish,
                camera = _context.camera,
                _name = _name,
                maskUpper = _maskUpper,
            };
            _pm = new PlayerPm(playerPmContext);
            AddUnsafe(_pm);


            _view = Object.Instantiate(_context.content.GetPlayerView()).GetComponent<PlayerView>();
            
            _view.Init(new PlayerView.Context
            {
                player = _context.player,
                playerBody = _playerBody,
                moveDirection = _moveDirection,
                die = _die,
                startRun = _context.startRun,
                smallJumpSearcher = _smallJumpSearcher,
                rayPlace = _rayPlace,
                mask = _mask,
                shake = _context.shake,
                finish = _finish,
                flyup = _context.flyup,
                _name = _name,
                leader = _context.leader,
                players = _context.players,
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

