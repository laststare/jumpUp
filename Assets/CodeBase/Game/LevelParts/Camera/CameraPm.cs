using Cinemachine;
using CodeBase.Content;
using Cysharp.Threading.Tasks;
using JumpUp.External;
using UniRx;
using UnityEngine;

namespace CodeBase.Game.LevelParts.Camera
{
    public class CameraPm : BaseDisposable
    {
        public struct Ctx
        {
            public IContent content;
            public IReadOnlyReactiveProperty<Transform> player;
            public IReactiveProperty<GameState> gameState;
            public ReactiveProperty<CinemachineVirtualCamera> vcam;
            public ReactiveProperty<Transform> _startTutorSphere;
            public ReactiveProperty<bool> needStartTutor;
            public Transform uiCanvas;
        }

        private Ctx _ctx;
        private Transform _cam;
        private CinemachineVirtualCamera _vcam;
        private Transform _startTutorSphere;

        public CameraPm(Ctx ctx)
        {
            _ctx = ctx;
            AddUnsafe(_ctx.gameState.Subscribe(GameStateReceiver));
            AddUnsafe(_ctx.vcam.Subscribe(GetCamera));
            AddUnsafe(_ctx._startTutorSphere.Subscribe(GetStartTutorSphere));
        }

        private void GetStartTutorSphere(Transform startTutorSphere) => _startTutorSphere = startTutorSphere;
        
        private void GetCamera(CinemachineVirtualCamera vcam) => _vcam = vcam;

        
        private async void GoBack()
        {
            float time = 0;
            while (time < 1)
            {
               if (_vcam.m_Lens.FieldOfView < 50) 
                    _vcam.m_Lens.FieldOfView += Time.deltaTime * 30;
               time += Time.deltaTime;
                    await UniTask.Yield();
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
                await UniTask.Yield();
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
                await UniTask.Yield();
            }
            _vcam.m_Lens.FieldOfView = 30;
        }


        private async void StartTutorFly()
        {
            var startTutorText = Object.Instantiate(_ctx.content.GetStartTutorText(), _ctx.uiCanvas);
            GoFar();
            _startTutorSphere.parent = null;
            _startTutorSphere.position = _ctx.player.Value.position;
            _startTutorSphere.eulerAngles = new Vector3(0, 45, 0);
            _startTutorSphere.gameObject.SetActive(true);
            _vcam.Follow = _startTutorSphere;
            _vcam.LookAt = _startTutorSphere;
            await UniTask.Delay(8000);
            _vcam.Follow = _ctx.player.Value;
            _vcam.LookAt = _ctx.player.Value;
            GoCloser();
            Object.Destroy(_startTutorSphere.gameObject);
            _ctx.gameState.Value = GameState.COUNTER;
            PlayerPrefs.SetInt("startTutor", 1);
            _ctx.needStartTutor.Value = false;
            Object.Destroy(startTutorText);
        }

        private void GameStateReceiver(GameState state)
        {
            switch (state)
            {
                case GameState.COUNTER:
                    GoBack();
                    break;
                case GameState.STARTTUTOR:
                    StartTutorFly();
                    break;
            }
        }

    }
}
