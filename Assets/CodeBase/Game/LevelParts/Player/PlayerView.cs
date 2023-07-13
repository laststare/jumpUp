using System.Collections.Generic;
using CodeBase.Game.interfaces;
using Cysharp.Threading.Tasks;
using JumpUp.External;
using UniRx;
using UnityEngine;
using UnityEngine.AI;

namespace CodeBase.Game.LevelParts.Player
{
    public class PlayerView : Human, IFinish, IJumper, IBatHolder
    {
        public struct Context
        {
            public ReactiveProperty<Transform> playerBody;
            public IReadOnlyReactiveProperty<Vector3> moveDirection;
            public ReactiveTrigger die;
            public ReactiveTrigger startRun;
            public ReactiveProperty<Transform> player;
            public IReadOnlyReactiveProperty<Transform> smallJumpSearcher;
            public ReactiveProperty<Transform> rayPlace;
            public ReactiveProperty<LayerMask> mask;
            public ReactiveEvent<float> shake;
            public ReactiveTrigger finish;
            public ReactiveEvent<bool> flyup;
            public ReactiveProperty<Transform> _name;
            public ReactiveEvent<Transform> leader;
            public ReactiveProperty<List<Transform>> players;
            public ReactiveProperty<LayerMask> maskUpper;
        }

        private Context _context;
        private ReactiveProperty<bool> grounded;
        private NavMeshAgent agent;
        [SerializeField]
        private HitTriggerView hitTriggerView;
        private HitTriggerEntity hitTriggerEntity;
        private ReactiveTrigger hit;
        [SerializeField]
        private ParticleSystem  batFx;
        private bool _batCoolDown, _haveBat;
        private static readonly int Move = Animator.StringToHash("move");
        private static readonly int Grounded = Animator.StringToHash("grounded");
        private static readonly int Fly = Animator.StringToHash("fly");
        private static readonly int Go = Animator.StringToHash("go");
        private static readonly int Dance = Animator.StringToHash("dance");
        private static readonly int Rocket = Animator.StringToHash("rocket");
        private static readonly int Jump = Animator.StringToHash("smallJump");
        private static readonly int Hit = Animator.StringToHash("hit");

        public void Init(Context Context)
        {
            _context = Context;
            AtStart();
            smallJumpSpeed = 2f;
            agent = GetComponent<NavMeshAgent>();
            _context.mask.Value = mask;
            _context.maskUpper.Value = maskUpper;
            _context.player.Value = transform;
            _context.playerBody.Value = body;
            _context.rayPlace.Value = rayPlace;
            _context._name.Value = nameCanvas;
            _context.startRun.Subscribe(startRun).AddTo(this);
            _context.smallJumpSearcher.Subscribe(Searcher).AddTo(this);
            grounded = new ReactiveProperty<bool>();
            grounded.ObserveEveryValueChanged(x => x.Value).Subscribe(CheckGround).AddTo(this);
            _context.leader.SubscribeWithSkip(CheckLeader);
            _context.players.Value.Add(transform);

            hit = new ReactiveTrigger();

            var hitTriggerEntityContext = new HitTriggerEntity.Context()
            { 
                hitTriggerView = hitTriggerView,
                hit = hit
            };
            hitTriggerEntity = new HitTriggerEntity(hitTriggerEntityContext);

            hit.Subscribe(HitAnimPlay).AddTo(this);
    }

        private void PlayersMove(Vector3 moveDirection)
        {
            if (myCharacterController.isGrounded)
            {
                vSpeed = -1;
                if (HighJump) vSpeed = highJumpSpeed;
                if (SmallJump) vSpeed = smallJumpSpeed;
                if (rocketJump) vSpeed = rocketJumpSpeed;
                if (downJump) vSpeed = -5;

            }
            else {
                if (HighJump) vSpeed = highJumpSpeed;                  
                if (SmallJump) vSpeed = smallJumpSpeed;
                if (rocketJump) vSpeed = rocketJumpSpeed;
                if (downJump) vSpeed = -5;
                HighJump = false;
                SmallJump = false;
                rocketJump = false;
                downJump = false;
            }
            
            vSpeed -= gravity * Time.deltaTime;
            if (myCharacterController.velocity.y < -17 && !waitingShake) waitingShake = true;
            moveDirection.y = vSpeed;
            if (moveBlocker) moveDirection = new Vector3(0, moveDirection.y, 0);
            myCharacterController.Move(moveDirection * moveSpeed  * Time.deltaTime);
            ControlAnimation(moveDirection);

            if (alive && transform.position.y < -10)
            {
                alive = false;
                _context.die.Notify();
            }

            grounded.Value = myCharacterController.isGrounded;
        }

        private void ControlAnimation(Vector3 moveDirection)
        {
            float m = moveDirection.x == 0 && moveDirection.z == 0 ? 0 : _haveBat? 2 :1;
            anim.SetFloat(Move, m);
            anim.SetBool(Grounded, myCharacterController.isGrounded);
            float f = myCharacterController.velocity.y > 2 ? 0 : 1;
            anim.SetFloat(Fly, f);
        }

        private void startRun()
        {
            anim.SetTrigger(Go);
            _context.moveDirection.Subscribe(x => PlayersMove(x)).AddTo(this);
        }

        public void DoFinish()
        {
            _context.finish.Notify();
            anim.SetTrigger(Dance);
        }

        private void Searcher(Transform part)
        {
            if (part == null && myCharacterController.isGrounded) DoJump(JumperType.light);
        }

        public void DoJump(JumperType type)
        {
            switch (type)
            {
                case JumperType.medium:
                    Unblock(transform.position.y + 15);
                    HighJump = true;
                    BoostSpeed(10f, 40);
                    anim.SetBool(Rocket, false);
                    ActivateTrail(2000);
                    _context.shake.Notify(0.5f);
                    waitingShake = true;
                    _context.flyup.Notify(true);
                    break;
                case JumperType.light:
                    SmallJump = true;
                    anim.SetTrigger(Jump);
                    break;
                case JumperType.rocket:
                    Unblock(transform.position.y + 15);
                    rocketJump = true;
                    BoostSpeed(5f, 500);
                    ShowRocketPack();
                    anim.SetBool(Rocket, true);
                    _context.shake.Notify(0.5f);
                    waitingShake = true;
                    _context.flyup.Notify(true);
                    break;
                case JumperType.oldCell:
                    downJump = true;
                    break;
            }
        }

        private void CheckGround(bool isGrounded)
        {
            if (!isGrounded || !waitingShake) return;
            _context.shake.Notify(0.5f);
            waitingShake = false;
            _context.flyup.Notify(false);
        }

        private async void HitAnimPlay()
        {
            if (_batCoolDown) return;
            _batCoolDown = true;
            anim.SetTrigger(Hit);
            PlayBatFx();
            await UniTask.Delay(500);
            BatActivator(false);
            _batCoolDown = false;
            _haveBat = false;
        }

        public void GetBat() =>  BatActivator(true);
        

        private void BatActivator(bool activate)
        {
            _haveBat = true;
            bat.SetActive(activate);
            hitTriggerView.gameObject.SetActive(activate);
        }

        private async void PlayBatFx()
        {
            await UniTask.Delay(50);
            batFx.Play();
        }
    }
}
