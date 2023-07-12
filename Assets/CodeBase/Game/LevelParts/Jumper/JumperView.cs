using System.Threading.Tasks;
using CodeBase.Content;
using CodeBase.Game.interfaces;
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
            public Transform otherTransform;
            public GameObject emptyCell;
        }

        private Ctx _ctx;
        public JumperType type;
        private Animator anim;
        [SerializeField]
        private GameObject puffFx;
        public void SetMain(Ctx ctx)
        {
            _ctx = ctx;
            if (type == JumperType.medium) anim = GetComponent<Animator>();
            else transform.position = _ctx.jumper.position;
        }

        private void OnTriggerEnter(Collider other) => Entering(other);
        

        public void Entering(Collider other)
        {
            var _jump = other.GetComponent<IJumper>();
            if (_jump != null)
            {
                switch (type)
                {
                    case JumperType.medium:
                        _jump.DoJump(type);
                        anim.SetTrigger("jump");
                        SetFX();
                        break;
                    case JumperType.rocket:
                        _jump.DoJump(type);
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
                        _jump.DoJump(type);
                        Instantiate(_ctx.emptyCell, transform.position, Quaternion.identity, _ctx.otherTransform);
                        gameObject.SetActive(false);
                        break;
                }
            }      
        }

        private async void SetFX()
        {
            puffFx.SetActive(true);
            if (await Wait(1000))
            {
                try { puffFx.SetActive(false); }
                catch { }
            }
        }

        private async Task<bool> Wait(int time)
        {
            await Task.Delay(time);
            return true;
        }
    }
}
