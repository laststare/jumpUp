using System.Collections.Generic;
using _scripts.Content;
using JumpUp;
using JumpUp.Content;
using JumpUp.External;
using UniRx;
using UnityEngine;
using UnityEngine.AI;

namespace _scripts.Game.LevelParts.ioPlayer
{
    public class IoPLayerEntity : BaseDisposable
    {
        public struct Ctx
        {
            public IReactiveProperty<GameState> gameState;
            public IoPLayerView view;
            public PlayersNameView _nameView;
            public LevelContainer.IoPlayer _ioPlayer;
            public ReactiveEvent<GameObject> floorPart;
            public ReactiveEvent<GameObject> roofPart;
            public IReactiveProperty<int> levelIndex;
            public Transform otherCanvas;
            public ReactiveProperty<UnityEngine.Camera> camera;
            public string ioName;
            public Material skinMat;
            public ReactiveProperty<List<Transform>> players;
            public ReactiveEvent<Transform> leader;
            public ReactiveProperty<string> winnerName;
        }


        private Ctx _ctx;
        private IoPLayerView _view;
        private PlayersNameView _nameView;
        private IoPLayerPm _pm;

        private readonly ReactiveProperty<Transform> _ioplayer;
        private ReactiveProperty<Transform> _ioplayerBody;
        private readonly ReactiveProperty<Transform> _rayPlace;
        private ReactiveProperty<Vector3> moveDirection;
        private readonly ReactiveProperty<Transform> _smallJumpSearcher;
        private readonly ReactiveProperty<LayerMask> _mask;
        private readonly ReactiveProperty<LayerMask> _maskUpper;
        private readonly ReactiveTrigger _die;
        private readonly ReactiveTrigger _finish;
        private readonly ReactiveTrigger _startGame;
        private readonly ReactiveTrigger _grounded;
        private readonly ReactiveTrigger _makeSmallJump;
        private readonly ReactiveProperty<NavMeshAgent> _agent;
        private readonly ReactiveProperty<Transform> _name;

        public IoPLayerEntity(Ctx ctx)
        {
            _ctx = ctx;
  
            _ioplayerBody = new ReactiveProperty<Transform>();
            moveDirection = new ReactiveProperty<Vector3>();
            _die = new ReactiveTrigger();
            _finish = new ReactiveTrigger();
            _startGame = new ReactiveTrigger();
            _smallJumpSearcher = new ReactiveProperty<Transform>();
            _maskUpper = new ReactiveProperty<LayerMask>();
            _makeSmallJump = new ReactiveTrigger();
            _ioplayer = new ReactiveProperty<Transform>();
            _rayPlace = new ReactiveProperty<Transform>();
            _mask = new ReactiveProperty<LayerMask>();
            _agent = new ReactiveProperty<NavMeshAgent>();
            _grounded = new ReactiveTrigger();
            _name = new ReactiveProperty<Transform>();

           _view = Object.Instantiate(_ctx.view, _ctx._ioPlayer.position, _ctx._ioPlayer.rotation);
            var ioPlayerPmCtx = new IoPLayerPm.Ctx()
            {
                gameState = _ctx.gameState,
                _agent = _agent,
                ioplayer = _ioplayer,
                rayPlace = _rayPlace,
                mask = _mask,
                roofPart = _ctx.roofPart,
                floorPart = _ctx.floorPart,
                startGame = _startGame,
                grounded = _grounded,
                smallJumpSearcher = _smallJumpSearcher,
                finish = _finish,
                _nameView = _name,
                die = _die,
                camera = _ctx.camera,
                ioName = _ctx.ioName,
                maskUpper = _maskUpper,
                winnerName = _ctx.winnerName,
                MakeSmallJump = _makeSmallJump

            };
            _pm = new IoPLayerPm(ioPlayerPmCtx);
            AddUnsafe(_pm);

            var ioPlayerViewCtx = new IoPLayerView.Ctx()
            {
                _agent = _agent,
                ioplayer = _ioplayer,
                rayPlace = _rayPlace,
                mask = _mask,
                startGame = _startGame,
                grounded = _grounded,
                smallJumpSearcher = _smallJumpSearcher,
                finish = _finish,
                levelIndex = _ctx.levelIndex,
                die = _die,
                _name = _name,
                skinMat = _ctx.skinMat,
                leader = _ctx.leader,
                players = _ctx.players,
                maskUpper = _maskUpper,
                MakeSmallJump = _makeSmallJump
            };
            _view.SetMain(ioPlayerViewCtx);
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
