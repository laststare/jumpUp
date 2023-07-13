using System.Collections.Generic;
using System.Threading;
using CodeBase.Game.LevelParts.Finish;
using CodeBase.Game.LevelParts.Jumper;
using Cysharp.Threading.Tasks;
using JumpUp.External;
using UniRx;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

namespace CodeBase.Game.LevelParts.ioPlayer
{
    public class IoPLayerPm : BaseDisposable
    {
        public struct Context
        {
            public ReactiveProperty<Transform> smallJumpSearcher;
            public IReactiveProperty<GameState> gameState;
            public ReactiveProperty<Transform> ioplayer;
            public ReactiveProperty<Transform> rayPlace;
            public ReactiveProperty<LayerMask> mask;
            public ReactiveProperty<NavMeshAgent> _agent;
            public ReactiveEvent<GameObject> floorPart;
            public ReactiveEvent<GameObject> roofPart;
            public ReactiveTrigger startRun;
            public ReactiveTrigger grounded;
            public IReadOnlyReactiveTrigger finish;
            public ReactiveProperty<Transform> _nameView;
            public ReactiveTrigger die;
            public ReactiveProperty<UnityEngine.Camera> camera;
            public string ioName;
            public ReactiveProperty<LayerMask> maskUpper;
            public ReactiveProperty<string> winnerName;
        }


        private Context _context;
        private float _speed = 2.5f;
        private readonly float _sideStep = 0.1f;
        private Transform _playerTr, _playerBody, _rayPlace;
        private RaycastHit _hit, _hitFront, _hitUp;
        private GameObject _tmpObj;
        private LayerMask _mask, _maskUpper;

        private List<Vector3> _sides;
        private NavMeshAgent _agent;
        private readonly CancellationTokenSource cancellation;
        private Transform _name;
        private Text _nameTx;
        private bool _isAlive;
        private Vector3 _nextpoint;


        public IoPLayerPm(Context context)
        {
            _context = context;
            cancellation = new CancellationTokenSource();
            AddUnsafe(_context.gameState.Subscribe(GameStateReciever));
            AddUnsafe(_context.grounded.Subscribe(Grounded));
            AddUnsafe(_context.finish.Subscribe(Finish));
            AddUnsafe(_context.ioplayer.Subscribe(GetPlayer));
            AddUnsafe(_context._nameView.Subscribe(GetName));
            AddUnsafe(_context.die.Subscribe(Die));
            AddUnsafe(_context.camera.Subscribe(GetCamera));
        }

        private async void Raycasting()
        {
            while (_isAlive)
                {
                    _sides = new List<Vector3>();
                    _sides.Add(new Vector3(_playerTr.position.x + _sideStep, _playerTr.position.y, _playerTr.position.z));
                    _sides.Add(new Vector3(_playerTr.position.x - _sideStep, _playerTr.position.y, _playerTr.position.z));
                    _sides.Add(new Vector3(_playerTr.position.x, _playerTr.position.y, _playerTr.position.z + _sideStep));
                    _sides.Add(new Vector3(_playerTr.position.x, _playerTr.position.y, _playerTr.position.z - _sideStep));

                    foreach (var side in _sides)
                    {
                        if (Physics.Raycast(side, Vector3.down, out _hit, 1, _mask))
                        {
                            if (_hit.transform.gameObject.layer == 8 && _hit.transform.gameObject != _tmpObj)
                            {
                                _tmpObj = _hit.transform.gameObject;
                                _context.floorPart.Notify(_tmpObj);
                            }
                        }
                    }


                    if (Physics.Raycast(_playerTr.position, Vector3.up, out _hitUp, 5))
                    {
                    //Debug.LogError(hitUp.transform.gameObject);
                    if (_hitUp.transform.gameObject.layer == 8) _context.roofPart.Notify(_hitUp.transform.gameObject);
                    }
                    Debug.DrawRay(_playerTr.position, Vector3.up, Color.red);

                    // looking for holes
                    if (Physics.Raycast(_rayPlace.position, Vector3.down, out _hitFront, 2))
                    {
                        _context.smallJumpSearcher.Value = _hitFront.transform;
                    }
                    else _context.smallJumpSearcher.Value = null;
                        //_context.MakeSmallJump.Notify();
                    Debug.DrawRay(_rayPlace.position, Vector3.down, Color.red);

                //Debug.LogError($"{_context.ioplayer.Value.gameObject.name} {nextpoint}");

                  await UniTask.Yield();
                }
        }

        private void LookForJumper()
        {
            var hits = Physics.OverlapSphere(_playerTr.position, 35);
            var colliders = new List<Transform>();
            foreach (var c in hits)
            {
                if (c.GetComponent<JumperView>() && c.transform.position.y < _playerTr.position.y + 5 && c.transform.position.y > _playerTr.position.y - 5
                    && c.GetComponent<JumperView>().type == JumperType.medium)
                    colliders.Add(c.transform);

                if (c.GetComponent<FinishView>() && c.transform.position.y < _playerTr.position.y + 5)
                {
                    _agent.SetDestination(c.transform.position);
                    colliders.Clear();
                    break;
                }
            }

            Transform near = null;
            if (colliders.Count <= 0) return;
            var l = Mathf.Infinity;
            foreach (var e in colliders)
            {
                var dist = Vector3.Distance(_playerTr.position, e.transform.position);
                if (dist < l) near = e.transform;
            }
            if (_agent.isOnNavMesh)
                _agent.SetDestination(near.position);
            else _agent.enabled = false;
        }

        private void Grounded()
        {
            LookForJumper();
        }

        private void Finish() 
        {
            _context.winnerName.Value = _context.ioName;
            _context.gameState.Value = GameState.GAMEOVER;    
        }

        private void GameStateReciever(GameState state)
        {
            switch (state)
            {
                case GameState.PLAY:
                    _isAlive = true;
                    Raycasting();
                    _context.startRun.Notify();
                    LookForJumper();
                    var i = Random.Range(0, 100);
                    _context.ioplayer.Value.gameObject.name = $"io player {i}";
                    break;
                case GameState.FINISH:
                    cancellation?.Cancel();
                    break;
                case GameState.GAMEOVER:
                    cancellation?.Cancel();
                    break;
            }
        }

        private void GetPlayer(Transform player)
        {
            _playerTr = player;
            _agent = _context._agent.Value;
            _rayPlace = _context.rayPlace.Value;
            _mask = _context.mask.Value;
            _maskUpper = _context.maskUpper.Value;
        }

        private void GetName(Transform view)
        {
            if (view == null) return;
            _name = view;
            _nameTx = _name.GetChild(0).GetComponent<Text>();
            _nameTx.text = _context.ioName;
        }

        private async void GetCamera(UnityEngine.Camera cam)
        {
            if (cam == null) return;
            _name.gameObject.GetComponent<Canvas>().worldCamera = cam;
            try
            {
                while ((_context.gameState.Value != GameState.FINISH || _context.gameState.Value != GameState.GAMEOVER) || _playerTr.gameObject.activeSelf)
                {
                    _name.LookAt(UnityEngine.Camera.main.transform.position);
                    await UniTask.Yield();
                }
            }
            catch { }
        }
        
        private void Die()
        {
            _playerTr.gameObject.SetActive(false);
            _name.gameObject.SetActive(false);       
        }

        protected override void OnDispose()
        {
            base.OnDispose();
            _isAlive = false;
        }

    }
}
