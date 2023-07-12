using CodeBase.Content;
using CodeBase.UI;
using CodeBase.UI.Input;
using JumpUp;
using JumpUp.External;
using UnityEngine;

namespace CodeBase
{
    public class Root : BaseDisposable
    {
        public struct Ctx
        {
            public PrefabsInfo prefabs;
            public InputView inputview;
            public DigitalRubyShared.FingersJoystickScript controll;
            public Transform blocksContainer;
            public Transform uiCanvas;
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
                inputview = _ctx.inputview,
                controll = _ctx.controll,
                blocksContainer = _ctx.blocksContainer,
                uiCanvas = _ctx.uiCanvas
            };
            _rootEntity = new RootEntity(ctx);
            AddUnsafe(_rootEntity);
        }

        protected override void OnDispose() => _instance = null;
    

    }
}
