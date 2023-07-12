using CodeBase.UI.Input;
using DigitalRubyShared;
using JumpUp;
using JumpUp.Content;
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
            public InputView inputview;
            public ReactiveEvent<Vector2> moveCoor;
            public ReactiveTrigger _onClick;
            public FingersJoystickScript controll;
            public ReactiveTrigger countingIsOver;
            public ReactiveProperty<GameObject> endlessSignTutor;
        }

        private Ctx _ctx;
        private UIpm _pm;

        public UIEntity(Ctx ctx)
        {
            _ctx = ctx;

            var fingerCtx = new FingersJoystickScript.Ctx()
            {
                moveCoor = _ctx.moveCoor,
                gameState = _ctx.gameState
            };

            _ctx.controll.SetMain(fingerCtx);

            var inputCtx = new InputView.Ctx()
            {      
                OnClick = _ctx._onClick,
            };
            _ctx.inputview.SetMain(inputCtx);
            
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
