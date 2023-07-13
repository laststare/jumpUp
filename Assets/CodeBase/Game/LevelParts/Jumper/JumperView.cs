using CodeBase.Content;
using CodeBase.Game.interfaces;
using Cysharp.Threading.Tasks;
using MoreMountains.NiceVibrations;
using UnityEngine;

namespace CodeBase.Game.LevelParts.Jumper
{
    public class JumperView : MonoBehaviour, ITrigger
    {
        public struct Context
        {
            public LevelContainer.Jumper jumper;
            public Transform blocksContainer;
            public GameObject emptyCell;
        }

        private Context _context;
        public JumperType type;
        private Animator _anim;
        [SerializeField]
        private GameObject puffFx;

        private static readonly int Jump = Animator.StringToHash("jump");

        public void Init(Context context)
        {
            _context = context;
            if (type == JumperType.medium) _anim = GetComponent<Animator>();
            else transform.position = _context.jumper.position;
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
                    Instantiate(_context.emptyCell, transform.position, Quaternion.identity, _context.blocksContainer);
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
