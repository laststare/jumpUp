using System.Collections.Generic;
using System.Threading.Tasks;
using JumpUp;
using JumpUp.Content;
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
            public IReadOnlyReactiveEvent<Vector2> moveCoor;
            public ReactiveProperty<Vector3> moveDirection;
            public Transform tutor;
            public ReactiveTrigger die;
            public ReactiveTrigger startGame;
            public Level.Level Level;
            public ReactiveEvent<GameObject> floorPart;
            public ReactiveEvent<GameObject> roofPart;
            public ReactiveProperty<Transform> smallJumpSearcher;
            public IReadOnlyReactiveProperty<Transform> rayPlace;
            public ReactiveProperty<LayerMask> mask;
            public IReadOnlyReactiveTrigger finish;
            public ReactiveProperty<UnityEngine.Camera> camera;
            public ReactiveProperty<Transform> _name;
            public ReactiveTrigger showTutor;
            public ReactiveProperty<LayerMask> maskUpper;
        }

        private Ctx _ctx;
        private readonly float _speed = 2.5f;
        private readonly float sideStep = 0.1f;
        private Vector3 _moveDirection;
        private Transform _playerTr, _playerBody, _rayPlace;
        private Vector2 _moveCoor;
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
            AddUnsafe(_ctx.moveCoor.Subscribe(GetCoor));
            AddUnsafe(_ctx.finish.Subscribe(Finish));
            AddUnsafe(_ctx._name.Subscribe(GetName));
            AddUnsafe(_ctx.showTutor.Subscribe(ShowTutor));
        }

        private void GetPlayer(Transform player)
        {
            if (player == null) return;
            _playerTr = player;
            _playerTr.position = _ctx.Level.playersPosiiton;
            SeePlayer();
        }

        private void GetCoor(Vector2 moveCoor) =>  _moveCoor = moveCoor;
                  
        private void ShowTutor() => _ctx.tutor.gameObject.SetActive(true);
        private async void WaitTouch()
        {
            while (_moveCoor == Vector2.zero)
            {
                await Task.Yield();
            }
            _ctx.tutor.gameObject.SetActive(false);
            _ctx.startGame.Notify();
            _canDelete = true;
        }

        private void Move(Vector2 moveCoor)
        {
            RayCasting();
            if (_ctx.gameState.Value != GameState.PLAY) return;
            _moveDirection = new Vector3(moveCoor.x, 0, moveCoor.y);
            _moveDirection = _playerTr.TransformDirection(_moveDirection);
            _moveDirection *= _speed;
            _moveDirection.y -= 20 * Time.deltaTime;

            _ctx.moveDirection.Value = _moveDirection;

            var h = moveCoor.x;
            var v = moveCoor.y;
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
                    AddUnsafe(_ctx.moveCoor.Subscribe(x => Move(x)));
                    break;
                case GameState.GAMEOVER:
                    _ctx.tutor.gameObject.SetActive(false);
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
                await Task.Yield();
            }
        }

        private async void SeePlayer()
        {
            await Task.Delay(1500);
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
                    await Task.Yield();
                }
            }
            catch { }
        }
    }
}
