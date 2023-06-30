using System.Threading.Tasks;
using Cinemachine;
using JumpUp.External;
using UniRx;
using UnityEngine;

namespace _scripts.Game.LevelParts.Camera
{
    public class CameraView : MonoBehaviour
    {

        public struct Ctx
        {
            public ReactiveProperty<Transform> camTr;
            public IReadOnlyReactiveProperty<Transform> player;
            public ReactiveEvent<float> shake;
            public IReadOnlyReactiveEvent<bool> flyup;
            public ReactiveProperty<UnityEngine.Camera> camera;
            public ReactiveProperty<CinemachineVirtualCamera> vcam;
            public ReactiveProperty<Transform> _bigTutorSphere;
        }

        private Ctx _ctx;
        [SerializeField]
        private GameObject bigTutorSphere;

        [SerializeField]
        private CinemachineVirtualCamera vcam;
        public void SetMain(Ctx ctx)
        {
            _ctx = ctx;
            _ctx.player.Subscribe(SetPlayer).AddTo(this);   
            _ctx.shake.SubscribeWithSkip(Shake).AddTo(this);
            _ctx.flyup.SubscribeWithSkip(FovChanger).AddTo(this);
            _ctx.camera.Value = GetComponent<UnityEngine.Camera>() ;
            _ctx.vcam.Value = vcam;
            _ctx._bigTutorSphere.Value = bigTutorSphere.transform;
        }

        private void SetPlayer(Transform player)
        {
            vcam.Follow = player;
            vcam.LookAt = player;
        }

        private async void Shake(float intensity)
        {
            vcam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_AmplitudeGain = intensity;
            await Task.Delay(250);
            vcam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_AmplitudeGain = 0;
        }

        private async void FovChanger(bool fly)
        {
            if (fly)
            {
                while (vcam.m_Lens.FieldOfView < 60)
                {
                    vcam.m_Lens.FieldOfView += Time.deltaTime * 20;
                    await Task.Yield();
                }
                vcam.m_Lens.FieldOfView = 60;
            }
            else 
            {
                while (vcam.m_Lens.FieldOfView > 50)
                {
                    vcam.m_Lens.FieldOfView -= Time.deltaTime * 20;
                    await Task.Yield();
                }
                vcam.m_Lens.FieldOfView = 50;
            }
        }
        
     
    }
}
