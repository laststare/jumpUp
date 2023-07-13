using System.Collections.Generic;
using CodeBase.Content;
using JumpUp.External;
using UniRx;
using UnityEngine;
using UnityEngine.AI;

namespace CodeBase.Game.LevelParts.ioPlayer
{
    public class IoPLayerEntity : BaseDisposable
    {
        public struct Context
        {
            public IReactiveProperty<GameState> gameState;
            public IoPLayerView view;
            public LevelContainer.IoPlayer _ioPlayer;
            public ReactiveEvent<GameObject> floorPart;
            public ReactiveEvent<GameObject> roofPart;
            public IReactiveProperty<int> levelIndex;
            public ReactiveProperty<UnityEngine.Camera> camera;
            public string ioName;
            public Material skinMat;
            public ReactiveProperty<List<Transform>> players;
            public ReactiveEvent<Transform> leader;
            public ReactiveProperty<string> winnerName;
        }
        
        private Context _context;
        private IoPLayerView _view;
        private PlayersNameView _nameView;
        private IoPLayerPm _pm;

        private readonly ReactiveProperty<Transform> _ioplayer;
        private readonly ReactiveProperty<Transform> _rayPlace;
        private readonly ReactiveProperty<Transform> _smallJumpSearcher;
        private readonly ReactiveProperty<LayerMask> _mask;
        private readonly ReactiveProperty<LayerMask> _maskUpper;
        private readonly ReactiveTrigger _die;
        private readonly ReactiveTrigger _finish;
        private readonly ReactiveTrigger _startRun;
        private readonly ReactiveTrigger _grounded;
        private readonly ReactiveTrigger _makeSmallJump;
        private readonly ReactiveProperty<NavMeshAgent> _agent;
        private readonly ReactiveProperty<Transform> _name;

        public IoPLayerEntity(Context context)
        {
            _context = context;
  
            _die = new ReactiveTrigger();
            _finish = new ReactiveTrigger();
            _startRun = new ReactiveTrigger();
            _smallJumpSearcher = new ReactiveProperty<Transform>();
            _maskUpper = new ReactiveProperty<LayerMask>();
            _makeSmallJump = new ReactiveTrigger();
            _ioplayer = new ReactiveProperty<Transform>();
            _rayPlace = new ReactiveProperty<Transform>();
            _mask = new ReactiveProperty<LayerMask>();
            _agent = new ReactiveProperty<NavMeshAgent>();
            _grounded = new ReactiveTrigger();
            _name = new ReactiveProperty<Transform>();

           _view = Object.Instantiate(_context.view, _context._ioPlayer.position, _context._ioPlayer.rotation);
            var ioPlayerPmContext = new IoPLayerPm.Context()
            {
                gameState = _context.gameState,
                _agent = _agent,
                ioplayer = _ioplayer,
                rayPlace = _rayPlace,
                mask = _mask,
                roofPart = _context.roofPart,
                floorPart = _context.floorPart,
                startRun = _startRun,
                grounded = _grounded,
                smallJumpSearcher = _smallJumpSearcher,
                finish = _finish,
                _nameView = _name,
                die = _die,
                camera = _context.camera,
                ioName = _context.ioName,
                maskUpper = _maskUpper,
                winnerName = _context.winnerName,

            };
            _pm = new IoPLayerPm(ioPlayerPmContext);
            AddUnsafe(_pm);

            var ioPlayerViewContext = new IoPLayerView.Context()
            {
                _agent = _agent,
                ioplayer = _ioplayer,
                rayPlace = _rayPlace,
                mask = _mask,
                startRun = _startRun,
                grounded = _grounded,
                smallJumpSearcher = _smallJumpSearcher,
                finish = _finish,
                levelIndex = _context.levelIndex,
                die = _die,
                _name = _name,
                skinMat = _context.skinMat,
                leader = _context.leader,
                players = _context.players,
                maskUpper = _maskUpper,
                MakeSmallJump = _makeSmallJump
            };
            _view.Init(ioPlayerViewContext);
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
