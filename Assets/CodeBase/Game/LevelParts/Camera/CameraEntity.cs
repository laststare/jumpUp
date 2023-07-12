using Cinemachine;
using CodeBase.Content;
using JumpUp;
using JumpUp.External;
using UniRx;
using UnityEngine;

namespace CodeBase.Game.LevelParts.Camera
{
    public class CameraEntity : BaseDisposable
    {
        public struct Ctx
        {
            public IContent content;
            public ReactiveProperty<Transform> player;
            public IReactiveProperty<GameState> gameState;
            public ReactiveEvent<float> shake;
            public ReactiveEvent<bool> flyup;
            public ReactiveProperty<UnityEngine.Camera> camera;
            public ReactiveProperty<bool> needStartTutor;
            public Transform uiCanvas;
        }

        private Ctx _ctx;
        private readonly CameraView _cameraview;
        private CameraPm _pm;
        private readonly ReactiveProperty<CinemachineVirtualCamera> _vcam;
        private readonly ReactiveProperty<Transform> _startTutorSphere;
        public CameraEntity(Ctx ctx)
        {
            _ctx = ctx;
            _cameraview = Object.Instantiate(ctx.content.GetCamera().GetComponent<CameraView>());
            _vcam = new ReactiveProperty<CinemachineVirtualCamera>();
            _startTutorSphere = new ReactiveProperty<Transform>();
            var cameraPmCtx = new CameraPm.Ctx()
            {
                player = _ctx.player,
                vcam = _vcam,
                gameState = _ctx.gameState,
                _startTutorSphere = _startTutorSphere,
                needStartTutor = _ctx.needStartTutor,
                uiCanvas = _ctx.uiCanvas,
                content = _ctx.content
            };
            _pm = new CameraPm(cameraPmCtx);
            AddUnsafe(_pm);

            _cameraview.SetMain(new CameraView.Ctx
            {
                player = _ctx.player,
                shake = _ctx.shake,
                flyup = _ctx.flyup,
                camera = _ctx.camera,
                vcam = _vcam,
                _startTutorSphere = _startTutorSphere
            });

        }

        protected override void OnDispose()
        {
            base.OnDispose();

            if (_cameraview != null)
            {
                Object.Destroy(_cameraview.gameObject);
            }
        }
    }
}
