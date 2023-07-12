using System.Collections.Generic;
using CodeBase.Content;
using Cysharp.Threading.Tasks;
using JumpUp.External;
using UniRx;
using UnityEngine;

namespace CodeBase.Game.LevelParts.Player
{
    public class PlayerPm : BaseDisposable
    {
        public struct Ctx
        {
            public IContent content;
            public ReactiveProperty<Transform> player;
            public ReactiveProperty<Transform> playerBody;
            public IReactiveProperty<GameState> gameState;
            public IReadOnlyReactiveEvent<Vector2> moveCoordinates;
            public ReactiveProperty<Vector3> moveDirection;
            public ReactiveProperty<GameObject> endlessSignTutor;
            public ReactiveTrigger die;
            public ReactiveTrigger startRun;
            public Level.Level Level;
            public ReactiveEvent<GameObject> floorPart;
            public ReactiveEvent<GameObject> roofPart;
            public ReactiveProperty<Transform> smallJumpSearcher;
            public IReadOnlyReactiveProperty<Transform> rayPlace;
            public ReactiveProperty<LayerMask> mask;
            public IReadOnlyReactiveTrigger finish;
            public ReactiveProperty<UnityEngine.Camera> camera;
            public ReactiveProperty<Transform> _name;
            public ReactiveProperty<LayerMask> maskUpper;
        }

        private Ctx _ctx;
        private readonly float _speed = 2.5f;
        private readonly float sideStep = 0.1f;
        private Vector3 _moveDirection;
        private Transform _playerTr, _playerBody, _rayPlace;
        private Vector2 _moveCoordinates;
        private RaycastHit _hit, _hitFront;
        private GameObject _tmpObj;
        private LayerMask _mask, _maskUpper;
        private bool _canDelete;
        private List<Vector3> _sides;
        private Transform _name;
        public PlayerPm(Ctx ctx)
        {
            _ctx = ctx;
            AddUnsafe(_ctx.player.Subscribe(GetPlayer));
            AddUnsafe(_ctx.gameState.Subscribe(GameStateReceiver));
            AddUnsafe(_ctx.die.Subscribe(Die));
            AddUnsafe(_ctx.moveCoordinates.Subscribe(GetCoordinates));
            AddUnsafe(_ctx.finish.Subscribe(Finish));
            AddUnsafe(_ctx._name.Subscribe(GetName));
        }

        private void GetPlayer(Transform player)
        {
            if (player == null) return;
            _playerTr = player;
            _playerTr.position = _ctx.Level.playersPosiiton;
            SeePlayer();
        }

        private void GetCoordinates(Vector2 moveCoordinates) =>  _moveCoordinates = moveCoordinates;
                  
        private async void WaitTouch()
        {
            while (_moveCoordinates == Vector2.zero)
            {
                await UniTask.Yield();
            }
            Object.Destroy(_ctx.endlessSignTutor.Value);
            _ctx.startRun.Notify();
            _canDelete = true;
        }

        private void Move(Vector2 moveCoordinates)
        {
            RayCasting();
            if (_ctx.gameState.Value != GameState.PLAY) return;
            _moveDirection = new Vector3(moveCoordinates.x, 0, moveCoordinates.y);
            _moveDirection = _playerTr.TransformDirection(_moveDirection);
            _moveDirection *= _speed;
            _moveDirection.y -= 20 * Time.deltaTime;

            _ctx.moveDirection.Value = _moveDirection;

            var h = moveCoordinates.x;
            var v = moveCoordinates.y;
            if (h != 0 && v != 0)
            {
                var angle1 = Mathf.LerpAngle(_playerBody.localEulerAngles.y, Mathf.Atan2(h, v) * 180 / Mathf.PI, Time.deltaTime * 50);
                _playerBody.localEulerAngles = new Vector3(0, angle1, 0);
            }
           
        }

        private void RayCasting()
        {
            _sides = new List<Vector3>();
            _sides.Add(new Vector3(_playerTr.position.x + sideStep, _playerTr.position.y, _playerTr.position.z));
            _sides.Add(new Vector3(_playerTr.position.x - sideStep, _playerTr.position.y, _playerTr.position.z));
            _sides.Add(new Vector3(_playerTr.position.x, _playerTr.position.y, _playerTr.position.z + sideStep));
            _sides.Add(new Vector3(_playerTr.position.x, _playerTr.position.y, _playerTr.position.z - sideStep));

            foreach (var side in _sides)
            {
                if (Physics.Raycast(side, Vector3.down, out _hit, 1, _mask))
                {
                    if (_hit.transform.gameObject.layer == 8 && _hit.transform != _playerTr && _hit.transform.gameObject != _tmpObj)
                    {
                        _tmpObj = _hit.transform.gameObject;
                        if (_canDelete) _ctx.floorPart.Notify(_tmpObj);
                    }
                }
                Debug.DrawRay(side, Vector3.down, Color.red);
            }


            if (Physics.Raycast(_playerTr.position, Vector3.up, out _hit, 2, _maskUpper))
            {
                _ctx.roofPart.Notify(_hit.transform.gameObject);
            }

            ////looking for holes
            if (Physics.Raycast(_rayPlace.position, Vector3.down, out _hitFront, 2))
            {
                _ctx.smallJumpSearcher.Value = _hitFront.transform;
            }
            else _ctx.smallJumpSearcher.Value = null;
        }
             
        private void Die() => _ctx.gameState.Value = GameState.GAMEOVER;

        private void Finish() => _ctx.gameState.Value = GameState.FINISH;

        private void GameStateReceiver(GameState state)
        {
            switch (state)
            {
                case GameState.PLAY:

                    _playerBody = _ctx.playerBody.Value;
                    _rayPlace = _ctx.rayPlace.Value;
                    _mask = _ctx.mask.Value;
                    _maskUpper = _ctx.maskUpper.Value;
                    WaitTouch();
                    AddUnsafe(_ctx.moveCoordinates.Subscribe(x => Move(x)));
                    break;
                case GameState.GAMEOVER:
                    Object.Destroy(_ctx.endlessSignTutor.Value);
                    MoveWithOutControl();
                    break;
            }
        }

        private async void MoveWithOutControl()
        {
            while (_ctx.gameState.Value == GameState.GAMEOVER)
            {
                _moveDirection =   Vector3.zero;
                _moveDirection = _playerTr.TransformDirection(_moveDirection);
                _moveDirection *= _speed;
                _moveDirection.y -= 20 * Time.deltaTime;
                
                _ctx.moveDirection.Value = _moveDirection;
                await UniTask.Yield();
            }
        }

        private async void SeePlayer()
        {
            await UniTask.Delay(1500);
            _playerTr.gameObject.SetActive(true);
            AddUnsafe(_ctx.camera.Subscribe(x => GetCamera(x)));
        }

        private void GetName(Transform view) => _name = view;
        

        private async void GetCamera(UnityEngine.Camera cam)
        {
            if (cam == null) return;
            _name.gameObject.GetComponent<Canvas>().worldCamera = cam;
            try
            {
                while (_playerTr.gameObject.activeSelf)
                {
                    _name.LookAt(UnityEngine.Camera.main.transform.position);
                    await UniTask.Yield();
                }
            }
            catch { }
        }
    }
}
