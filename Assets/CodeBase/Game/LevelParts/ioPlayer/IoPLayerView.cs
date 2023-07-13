using System.Collections.Generic;
using System.Linq;
using CodeBase.Game.interfaces;
using CodeBase.Game.LevelParts.Jumper;
using Cysharp.Threading.Tasks;
using JumpUp.External;
using MoreMountains.NiceVibrations;
using UniRx;
using UnityEngine;
using UnityEngine.AI;

namespace CodeBase.Game.LevelParts.ioPlayer
{
    public class IoPLayerView : Human, IJumper, IFinish, IBatTarget
    {
        public struct Context
        {
            public ReactiveProperty<NavMeshAgent> _agent;
            public ReactiveProperty<Transform> ioplayer;
            public ReactiveProperty<Transform> rayPlace;
            public ReactiveProperty<LayerMask> mask;
            public IReadOnlyReactiveTrigger startRun;
            public ReactiveTrigger grounded;
            public IReadOnlyReactiveProperty<Transform> smallJumpSearcher;
            public ReactiveTrigger finish;
            public IReactiveProperty<int> levelIndex;
            public ReactiveTrigger die;
            public ReactiveProperty<Transform> _name;
            public Material skinMat;
            public ReactiveEvent<Transform> leader;
            public ReactiveProperty<List<Transform>> players;
            public ReactiveProperty<LayerMask> maskUpper;
            public ReactiveTrigger MakeSmallJump;
        }

        private Context _context;
        public  ioPlayerType type;
        private NavMeshAgent _agent;
        private bool _batHited, _isAlive;
        private ReactiveProperty<bool> _grounded;
        private Vector3 _moveDirection;
        private CapsuleCollider _col;
        private static readonly int Move = Animator.StringToHash("move");
        private static readonly int Grounded = Animator.StringToHash("grounded");
        private static readonly int Fly = Animator.StringToHash("fly");
        private static readonly int Go = Animator.StringToHash("go");
        private static readonly int Rocket = Animator.StringToHash("rocket");
        private static readonly int Jump = Animator.StringToHash("smallJump");
        private static readonly int Dance = Animator.StringToHash("dance");

        public void Init(Context context)
        {
            _context = context;
            AtStart();
            smallJumpSpeed = 4;
            _agent = GetComponent<NavMeshAgent>();
            _col = GetComponent<CapsuleCollider>();
            _context._agent.Value = _agent;
            _agent.avoidancePriority = Random.RandomRange(10, 90);
            highJumpSpeed = 10.55f;
            _context.mask.Value = mask;
            _context.maskUpper.Value = maskUpper;
            _context.rayPlace.Value = rayPlace;
            _context.ioplayer.Value = transform;
            _context._name.Value = nameCanvas;

            _context.startRun.Subscribe(StartRun).AddTo(this);
            _grounded = new ReactiveProperty<bool>();
            _grounded.ObserveEveryValueChanged(x => x.Value).Subscribe(x => CheckGround(x)).AddTo(this);
           
            moveBlocker = true;
            type = ioPlayerType.best;
            skin.material = _context.skinMat;
            _context.leader.SubscribeWithSkip(x => CheckLeader(x));
            _context.players.Value.Add(transform);
            _context.MakeSmallJump.Subscribe(() => DoJump(JumperType.light));
            
        }

        private async void PlayersMove()
        {
            try
            {
                while (_isAlive && !_batHited)
                {

                    if (myCharacterController.isGrounded) vSpeed = -1;
                    else _agent.enabled = false;
                    if (HighJump)
                    {
                        vSpeed = highJumpSpeed;
                        HighJump = false;
                    }
                    if (SmallJump)
                    {
                        vSpeed = smallJumpSpeed;
                        SmallJump = false;
                    }
                    if (rocketJump)
                    {
                        vSpeed = rocketJumpSpeed;
                        rocketJump = false;
                    }
                    if (downJump)
                    {
                        vSpeed = -5;
                        downJump = false;
                    }
                    
                    vSpeed -= gravity * Time.deltaTime;
                    _moveDirection.y = vSpeed;
                    if (!_agent.enabled) myCharacterController.Move(_moveDirection * moveSpeed * Time.deltaTime);
                    ControlAnimation();

                    if (_isAlive && transform.position.y < -10)
                    {
                        _isAlive = false;
                        _context.die.Notify();
                    }

                    _grounded.Value = myCharacterController.isGrounded;
                    
                    await UniTask.Delay(1, cancellationToken: Cancellation.Token);
                }
            }
            catch
            {
                Cancellation?.Dispose();
                Cancellation = null;
            }
        }

        private void ControlAnimation()
        {
            float m = myCharacterController.velocity.x == 0 && myCharacterController.velocity.z == 0 ? 0 : 1;
            anim.SetFloat(Move, m);
            anim.SetBool(Grounded, myCharacterController.isGrounded);
            float f = myCharacterController.velocity.y > 2 ? 0 : 1;
            anim.SetFloat(Fly, f);
        }

        private void Searcher(Transform part)
        {
            if (part == null && _agent != null && _agent.enabled) DoJump(JumperType.light);
        }

        private void CheckGround(bool isgrounded)
        {
            _col.enabled = isgrounded;
            if (isgrounded && !_batHited)
            {             
                _agent.enabled = true;
                moveBlocker = true;
                _context.grounded.Notify();
            }
        }

        private void StartRun()
        {
            anim.SetTrigger(Go);
            _isAlive = true;
            PlayersMove();
            _context.smallJumpSearcher.Subscribe(Searcher).AddTo(this);
        }

        public void DoJump(JumperType type)
        {
            if (_batHited) return;
            switch (type)
            {
                case JumperType.medium:
                    Unblock(transform.position.y + 15, true);
                    _agent.enabled = false;                 
                    HighJump = true;
                    BoostSpeed(10f, 40);
                    anim.SetBool(Rocket, false);
                    ActivateTrail(2000);                  
                    break;
                case JumperType.light:
                    Unblock(transform.position.y);
                    _agent.enabled = false;
                    _moveDirection = transform.forward;
                    SmallJump = true;
                    anim.SetTrigger(Jump);
                    break;
                case JumperType.rocket:
                    Unblock(transform.position.y + 15, true);
                    _agent.enabled = false;
                    _moveDirection = new Vector3(Random.Range(0, 2) == 0 ? 0.5f : -0.5f, 0, Random.Range(0, 2) == 0 ? 0.5f : -0.5f);
                    rocketJump = true;
                    BoostSpeed(5f, 500);
                    ShowRocketPack();
                    anim.SetBool(Rocket, true);
                    break;
                case JumperType.oldCell:
                    downJump = true;
                    break;
            }
        }

        protected override void OnUnblock()
        {
            base.OnUnblock();

            var hitColliders = Physics.OverlapSphere(transform.position, 15);
            if (hitColliders.Length > 0)
            {
                var l = Mathf.Infinity;
                Transform near = null;
                switch (type)
                {
                    case ioPlayerType.best:
                        var jumpers = hitColliders.Where(x => x.GetComponent<JumperView>());
                        if (jumpers.Count() == 0)
                            _moveDirection = new Vector3(Random.Range(0, 2) == 0 ? 0.5f : -0.5f, 0, Random.Range(0, 2) == 0 ? 0.5f : -0.5f);                      
                        
                        foreach (var e in jumpers)
                        {
                            var dist = Vector3.Distance(transform.position, e.transform.position);
                            if (dist < l)
                            {
                                near = e.transform;
                                l = dist;
                            }
                        }
                        if (near != null)
                        {
                            var relativePos = near.position - transform.position;
                            var rotation = Quaternion.LookRotation(relativePos, Vector3.up);
                            transform.rotation = rotation;
                            _moveDirection = transform.forward * 2;
                        }
                        else
                            _moveDirection = new Vector3(Random.Range(0, 2) == 0 ? 0.5f : -0.5f, 0, Random.Range(0, 2) == 0 ? 0.5f : -0.5f);
                        break;

                    case ioPlayerType.smarter:
                        var cells = hitColliders.Where(x => x.gameObject.layer == 8);
                        if (cells.Count() == 0)
                            _moveDirection = new Vector3(Random.Range(0, 2) == 0 ? 0.5f : -0.5f, 0, Random.Range(0, 2) == 0 ? 0.5f : -0.5f);
                        foreach (var e in cells)
                        {
                            var dist = Vector3.Distance(transform.position, e.transform.position);
                            if (dist < l)
                            {
                                near = e.transform;
                                l = dist;
                            }
                        }
                        if (near != null)
                        {
                            var relativePos = near.position - transform.position;
                            var rotation = Quaternion.LookRotation(relativePos, Vector3.up);
                            transform.rotation = rotation;
                            _moveDirection = transform.forward * l;
                        }
                        else
                            _moveDirection = new Vector3(Random.Range(0, 2) == 0 ? 0.5f : -0.5f, 0, Random.Range(0, 2) == 0 ? 0.5f : -0.5f);
                        break;

                    default:
                        _moveDirection = new Vector3(Random.Range(0, 2) == 0 ? 0.5f : -0.5f, 0, Random.Range(0, 2) == 0 ? 0.5f : -0.5f);
                        break;
                }
               
            }
            else  
                _moveDirection = new Vector3(Random.Range(0, 2) == 0 ? 0.5f : -0.5f, 0, Random.Range(0, 2) == 0 ? 0.5f : -0.5f);
        }

        public void DoFinish()
        {
            _context.finish.Notify();
            _agent.enabled = false;
            myCharacterController.enabled = false;
            anim.SetTrigger(Dance);
        }

        public void HitByBat(Transform hitter)
        {
            if (_batHited) return;
            MMVibrationManager.Haptic(HapticTypes.MediumImpact);
            _batHited = true;
            Destroy(_agent);
            GetComponent<Collider>().enabled = false;
            var rigi = GetComponent<Rigidbody>();
            rigi.isKinematic = false;
            rigi.useGravity = true;
            rigi.AddForce((hitter.forward + new Vector3(0,0.2f,0)) * 1200, ForceMode.Impulse);
        }

        private void OnDisable() => _isAlive = false;
    }
}
