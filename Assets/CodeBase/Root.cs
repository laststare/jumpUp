using CodeBase.Content;
using CodeBase.UI.Input;
using JumpUp.External;
using UnityEngine;

namespace CodeBase
{
    public class Root : BaseDisposable
    {
        public struct Context
        {
            public PrefabsInfo prefabs;
            public DigitalRubyShared.FingersJoystickScript controll;
            public Transform blocksContainer;
            public Transform uiCanvas;
        }

        private readonly Context _context;
        private RootEntity _rootEntity;
        private static Root _instance;
        private static bool RootExists  => _instance != null;

        private Root(Context context)
        {
            _context = context;
            CreateRootEntity();
        }

        public static Root CreateRoot(Context context)
        {
            if (RootExists)
            {
                return null;
            }
            _instance = new Root(context);
            return _instance;
        }

        private void CreateRootEntity()
        {
            var context = new RootEntity.Context
            {
                prefabs = _context.prefabs,
                controll = _context.controll,
                blocksContainer = _context.blocksContainer,
                uiCanvas = _context.uiCanvas
            };
            _rootEntity = new RootEntity(context);
            AddUnsafe(_rootEntity);
        }

        protected override void OnDispose() => _instance = null;
    

    }
}
