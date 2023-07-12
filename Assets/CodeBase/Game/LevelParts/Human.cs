using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace CodeBase.Game.LevelParts
{
    public class Human : MonoBehaviour
    {
        [HideInInspector]
        public  Transform body;
        [HideInInspector]
        public CharacterController myCharacterController;
        [HideInInspector]
        public Animator anim;
        public LayerMask mask, maskUpper;
        public Transform rayPlace, rocketPack;
        [HideInInspector]
        public float vSpeed, highJumpSpeed = 10.5f, smallJumpSpeed = 2f, rocketJumpSpeed = 11.2f, moveSpeed = 2f, gravity = 7, downJumpSpeed = 5f;
        [SerializeField]
        private GameObject trail;
        public  CancellationTokenSource Cancellation;
        public  SkinnedMeshRenderer skin;
        public  GameObject crown, bat;



        [HideInInspector]
        public bool HighJump, SmallJump, rocketJump, alive = true, moveBlocker, waitingShake, downJump;

        public Transform nameCanvas;

        protected async void BoostSpeed(float speed, int time)
        {
            moveSpeed = speed;
            await UniTask.Delay(time);
            moveSpeed = 2f;
        }

        protected void AtStart()
        {
            body = transform.GetChild(0);
            anim =  body.GetComponent<Animator>();
            myCharacterController = GetComponent<CharacterController>();
            Cancellation = new CancellationTokenSource();
        }

        protected async void ShowRocketPack()
        {
            rocketPack.gameObject.SetActive(true);
            await UniTask.Delay(2500);
            try
            {
                rocketPack.gameObject.SetActive(false);
            }
            catch { }
        }

        protected async void ActivateTrail(int time)
        {
            trail.SetActive(true);
            await UniTask.Delay(time);
            try
            {
                trail.SetActive(false);
            }
            catch { }
        }

        protected async void Unblock(float y, bool autocorrect = false)
        {
            try
            {
                moveBlocker = true;
                while (transform.position.y < y)
                {
                    await UniTask.Delay(1, cancellationToken: Cancellation.Token);
                }
                if (autocorrect) OnUnblock();
                moveBlocker = false;
            }
            catch
            {
                Cancellation?.Dispose();
                Cancellation = null;
            }
        }

        protected virtual void OnUnblock() { }
        

        protected void CheckLeader(Transform leaderTransform) => crown.SetActive(leaderTransform == transform);
    }
}
