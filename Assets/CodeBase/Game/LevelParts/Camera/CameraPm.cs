using System.Threading.Tasks;
using Cinemachine;
using JumpUp;
using JumpUp.External;
using UniRx;
using UnityEngine;

namespace CodeBase.Game.LevelParts.Camera
{
    public class CameraPm : BaseDisposable
    {
        public struct Ctx
        {
            public IReadOnlyReactiveProperty<Transform> player;
            public IReactiveProperty<GameState> gameState;
            public ReactiveProperty<CinemachineVirtualCamera> vcam;
            public ReactiveProperty<Transform> _bigTutorSphere;
            public ReactiveProperty<bool> needBigTutor;
        }

        private Ctx _ctx;
        private Transform _cam;
        private Transform _player;
        private CinemachineVirtualCamera _vcam;
        private Transform _bigTutorSphere;

        public CameraPm(Ctx ctx)
        {
            _ctx = ctx;
            AddUnsafe(_ctx.player.Subscribe(SetPlayer));
            AddUnsafe(_ctx.gameState.Subscribe(GameStateReceiver));
            AddUnsafe(_ctx.vcam.Subscribe(GetCamera));
            AddUnsafe(_ctx._bigTutorSphere.Subscribe(GetBigTutorSphere));
        }

        private void GetBigTutorSphere(Transform bigTutorSphere)
        {
            _bigTutorSphere = bigTutorSphere;
        }
             

        private void GetCamera(CinemachineVirtualCamera vcam)
        {
            _vcam = vcam;
        }

        private void SetPlayer(Transform playerTransform) => _player = playerTransform;
        

        private async void GoBack()
        {
            float time = 0;
            while (time < 1)
            {
               if (_vcam.m_Lens.FieldOfView < 50) 
                    _vcam.m_Lens.FieldOfView += Time.deltaTime * 30;
               time += Time.deltaTime;
                    await Task.Yield();
            }
            _vcam.m_Lens.FieldOfView = 50;
        }


        private async void GoFar()
        {
            float time = 0;
            while (time < 1)
            {
                if (_vcam.m_Lens.FieldOfView < 70)
                    _vcam.m_Lens.FieldOfView += Time.deltaTime * 40;
                time += Time.deltaTime;
                await Task.Yield();
            }
            _vcam.m_Lens.FieldOfView = 70;
        }

        private async void GoCloser()
        {
            float time = 0;
            while (time < 1)
            {
                if (_vcam.m_Lens.FieldOfView > 30)
                    _vcam.m_Lens.FieldOfView -= Time.deltaTime * 30;
                time += Time.deltaTime;
                await Task.Yield();
            }
            _vcam.m_Lens.FieldOfView = 30;
        }


        private async void BigTutorFly()
        {
            GoFar();
            _bigTutorSphere.parent = null;
            _bigTutorSphere.position = _ctx.player.Value.position;
            _bigTutorSphere.eulerAngles = new Vector3(0, 45, 0);
            _bigTutorSphere.gameObject.SetActive(true);
            _vcam.Follow = _bigTutorSphere;
            _vcam.LookAt = _bigTutorSphere;
            await Task.Delay(8000);
            _vcam.Follow = _ctx.player.Value;
            _vcam.LookAt = _ctx.player.Value;
            GoCloser();
            Object.Destroy(_bigTutorSphere.gameObject);
            _ctx.gameState.Value = GameState.COUNTER;
            PlayerPrefs.SetInt("bigTutor", 1);
            _ctx.needBigTutor.Value = false;
        }

        private void GameStateReceiver(GameState state)
        {
            switch (state)
            {
                case GameState.COUNTER:
                    GoBack();
                    break;
                case GameState.BIGTUTOR:
                    BigTutorFly();
                    break;
            }
        }

    }
}
