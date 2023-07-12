using CodeBase.Content;
using CodeBase.UI;
using CodeBase.UI.Input;
using JumpUp.Content;
using MoreMountains.NiceVibrations;
using UnityEngine;

namespace CodeBase
{
    public class EntryPoint : MonoBehaviour
    {
        [SerializeField] protected PrefabsInfo prefabs;
        [SerializeField] protected GameInfoView gameInfoView;
        [SerializeField] protected InputView inputview;
        [SerializeField] protected Transform tutor;
        [SerializeField] protected DigitalRubyShared.FingersJoystickScript controll;
        [SerializeField] protected Transform otherTransform;
        [SerializeField] protected Transform otherCanvas;


        private Root _root;


        private void Start()
        {
            
            MMVibrationManager.iOSInitializeHaptics();
            Application.targetFrameRate = 60;
            _root = Root.CreateRoot(new Root.Ctx
            {
                prefabs = prefabs,
                gameInfoView = gameInfoView,
                inputview = inputview,
                tutor = tutor,
                controll = controll,
                otherTransform = otherTransform,
                otherCanvas = otherCanvas
            }) ;
        }
        

        private void OnDestroy()
        {
            _root.Dispose();
        }
    }
}
