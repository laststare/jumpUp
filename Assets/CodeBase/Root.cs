using CodeBase.Content;
using CodeBase.UI;
using CodeBase.UI.Input;
using JumpUp;
using JumpUp.Content;
using JumpUp.External;
using UnityEngine;

namespace CodeBase
{
    public class Root : BaseDisposable
    {
        public struct Ctx
        {
            public PrefabsInfo prefabs;
            public GameInfoView gameInfoView;
            public InputView inputview;
            public Transform tutor;
            public DigitalRubyShared.FingersJoystickScript controll;
            public Transform otherTransform;
            public Transform otherCanvas;
        }

        private readonly Ctx  _ctx;
        private RootEntity  _rootEntity;
        private static Root _instance;
        private static bool RootExists  => _instance != null;

        private Root(Ctx ctx)
        {
            _ctx = ctx;
            CreateRootEntity();
        }

        public static Root CreateRoot(Ctx ctx)
        {
            if (RootExists)
            {
                return null;
            }
            _instance = new Root(ctx);
            return _instance;
        }

        private void CreateRootEntity()
        {
            var ctx = new RootEntity.Ctx
            {
                prefabs = _ctx.prefabs,
                gameInfoView = _ctx.gameInfoView,
                inputview = _ctx.inputview,
                tutor = _ctx.tutor,
                controll = _ctx.controll,
                otherTransform = _ctx.otherTransform,
                otherCanvas = _ctx.otherCanvas
            };
            _rootEntity = new RootEntity(ctx);
            AddUnsafe(_rootEntity);
        }

        protected override void OnDispose() => _instance = null;
    

    }
}
