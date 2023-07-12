using CodeBase.Content;
using CodeBase.UI;
using CodeBase.UI.Input;
using MoreMountains.NiceVibrations;
using UnityEngine;

namespace CodeBase
{
    public class EntryPoint : MonoBehaviour
    {
        [SerializeField] protected PrefabsInfo prefabs;
        [SerializeField] protected InputView inputview;
        [SerializeField] protected DigitalRubyShared.FingersJoystickScript controll;
        [SerializeField] protected Transform blocksContainer;
        [SerializeField] protected Transform uiCanvas;


        private Root _root;


        private void Start()
        {
            
            MMVibrationManager.iOSInitializeHaptics();
            Application.targetFrameRate = 60;
            _root = Root.CreateRoot(new Root.Ctx
            {
                prefabs = prefabs,
                inputview = inputview,
                controll = controll,
                blocksContainer = blocksContainer,
                uiCanvas = uiCanvas
            }) ;
        }
        

        private void OnDestroy()
        {
            _root.Dispose();
        }
    }
}
