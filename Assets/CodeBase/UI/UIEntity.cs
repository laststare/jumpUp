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
        public struct Ctx
        {
            public Transform uiCanvas;
            public IContent content;
            public ReactiveProperty<GameState> gameState;
            public ReactiveTrigger countingIsOver;
            public ReactiveProperty<GameObject> endlessSignTutor;
        }

        private Ctx _ctx;
        private UIpm _pm;

        public UIEntity(Ctx ctx)
        {
            _ctx = ctx;

            CreatePm();
        }

        private void CreatePm()
        {
            var uiPmCtx = new UIpm.Ctx()
            {
                content = _ctx.content,
                uiCanvas = _ctx.uiCanvas,
                gameState = _ctx.gameState,
                countingIsOver = _ctx.countingIsOver,
                endlessSignTutor = _ctx.endlessSignTutor
            };
            _pm = new UIpm(uiPmCtx);
            AddUnsafe(_pm);
        }
    }
}
