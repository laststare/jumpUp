using System.Threading.Tasks;
using CodeBase.Content;
using CodeBase.Game.interfaces;
using Cysharp.Threading.Tasks;
using JumpUp;
using MoreMountains.NiceVibrations;
using UnityEngine;

namespace CodeBase.Game.LevelParts.Jumper
{
    public class JumperView : MonoBehaviour, ITrigger
    {
        public struct Ctx
        {
            public LevelContainer.Jumper jumper;
            public Transform blocksContainer;
            public GameObject emptyCell;
        }

        private Ctx _ctx;
        public JumperType type;
        private Animator _anim;
        [SerializeField]
        private GameObject puffFx;

        private static readonly int Jump = Animator.StringToHash("jump");

        public void SetMain(Ctx ctx)
        {
            _ctx = ctx;
            if (type == JumperType.medium) _anim = GetComponent<Animator>();
            else transform.position = _ctx.jumper.position;
        }

        private void OnTriggerEnter(Collider other) => Entering(other);
        

        public void Entering(Collider other)
        {
            if (other.GetComponent<IJumper>() is not { } jump) return;
            switch (type)
            {
                case JumperType.medium:
                    jump.DoJump(type);
                    _anim.SetTrigger(Jump);
                    SetFX();
                    break;
                case JumperType.rocket:
                    jump.DoJump(type);
                    MMVibrationManager.Haptic(HapticTypes.MediumImpact);
                    gameObject.SetActive(false);
                    break;
                case JumperType.bat:
                    var batHolder = other.GetComponent<IBatHolder>();
                    if (batHolder != null)
                    {
                        batHolder.GetBat();
                        MMVibrationManager.Haptic(HapticTypes.MediumImpact);
                        gameObject.SetActive(false);
                    }
                    break;
                case JumperType.oldCell:
                    jump.DoJump(type);
                    Instantiate(_ctx.emptyCell, transform.position, Quaternion.identity, _ctx.blocksContainer);
                    gameObject.SetActive(false);
                    break;
            }
        }

        private async void SetFX()
        {
            puffFx.SetActive(true);
            await UniTask.Delay(1000);
            if(puffFx == null)return;
            puffFx.SetActive(false);
        }

    }
}
