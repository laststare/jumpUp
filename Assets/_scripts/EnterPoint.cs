using System.Collections;
using System.Collections.Generic;
using _scripts.UI;
using _scripts.UI.Input;
using UnityEngine;
using JumpUp.Content;
using MoreMountains.NiceVibrations;
using Facebook.Unity;
using UnityEngine.UI;

namespace JumpUp
{
    public class EnterPoint : MonoBehaviour
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

            if (!FB.IsInitialized)
                FB.Init(InitCallback, OnHideUnity);
            else
                FB.ActivateApp();
            
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



        private void InitCallback()
        {
            if (FB.IsInitialized)
                FB.ActivateApp();              
            else
              Debug.Log("Failed to Initialize the Facebook SDK");
            
        }

        private void OnHideUnity(bool isGameShown)
        {

        }

        private void OnDestroy()
        {
            _root.Dispose();
        }
    }
}
