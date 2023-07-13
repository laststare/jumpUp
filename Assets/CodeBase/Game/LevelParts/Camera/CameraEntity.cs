using Cinemachine;
using CodeBase.Content;
using JumpUp.External;
using UniRx;
using UnityEngine;

namespace CodeBase.Game.LevelParts.Camera
{
    public class CameraEntity : BaseDisposable
    {
        public struct Context
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

        private Context _context;
        private readonly CameraView _cameraview;
        private CameraPm _pm;
        private readonly ReactiveProperty<CinemachineVirtualCamera> _vcam;
        private readonly ReactiveProperty<Transform> _startTutorSphere;
        public CameraEntity(Context context)
        {
            _context = context;
            _cameraview = Object.Instantiate(context.content.GetCamera().GetComponent<CameraView>());
            _vcam = new ReactiveProperty<CinemachineVirtualCamera>();
            _startTutorSphere = new ReactiveProperty<Transform>();
            var cameraPmContext = new CameraPm.Context()
            {
                player = _context.player,
                vcam = _vcam,
                gameState = _context.gameState,
                _startTutorSphere = _startTutorSphere,
                needStartTutor = _context.needStartTutor,
                uiCanvas = _context.uiCanvas,
                content = _context.content
            };
            _pm = new CameraPm(cameraPmContext);
            AddUnsafe(_pm);

            _cameraview.Init(new CameraView.Context
            {
                player = _context.player,
                shake = _context.shake,
                flyup = _context.flyup,
                camera = _context.camera,
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
