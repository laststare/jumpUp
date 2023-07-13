using CodeBase.Content;
using CodeBase.UI.Input;
using DigitalRubyShared;
using JumpUp.External;
using UniRx;
using UnityEngine;

namespace CodeBase.UI
{
    public class UIEntity : BaseDisposable
    {
        public struct Context
        {
            public Transform uiCanvas;
            public IContent content;
            public ReactiveProperty<GameState> gameState;
            public ReactiveTrigger countingIsOver;
            public ReactiveProperty<GameObject> endlessSignTutor;
        }

        private Context _context;
        private UIpm _pm;

        public UIEntity(Context context)
        {
            _context = context;

            CreatePm();
        }

        private void CreatePm()
        {
            var uiPmContext = new UIpm.Context()
            {
                content = _context.content,
                uiCanvas = _context.uiCanvas,
                gameState = _context.gameState,
                countingIsOver = _context.countingIsOver,
                endlessSignTutor = _context.endlessSignTutor
            };
            _pm = new UIpm(uiPmContext);
            AddUnsafe(_pm);
        }
    }
}
