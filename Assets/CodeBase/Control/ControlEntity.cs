using CodeBase.Content;
using CodeBase.UI;
using CodeBase.UI.Input;
using DigitalRubyShared;
using JumpUp.External;
using UniRx;
using UnityEngine;

namespace CodeBase.Control
{
    public class ControlEntity : BaseDisposable
    {
        public struct Ctx
        {
            public ReactiveProperty<GameState> gameState;
            public ReactiveEvent<Vector2> moveCoordinates;
            public ReactiveTrigger onClick;
            public FingersJoystickScript controll;
            public Transform uiCanvas;
            public IContent content;
        }
        
        private Ctx _ctx;
        private ClickInputView _clickInputView;


        public ControlEntity(Ctx ctx)
        {
            _ctx = ctx;
            CreateViews();
        }

        private void CreateViews()
        {
            var fingerCtx = new FingersJoystickScript.Ctx()
            {
                moveCoordinates = _ctx.moveCoordinates,
                gameState = _ctx.gameState
            };
            _ctx.controll.SetMain(fingerCtx);
            
            _clickInputView = Object.Instantiate(_ctx.content.GetClickInputView(), _ctx.uiCanvas);
            _clickInputView .SetMain(new ClickInputView.Ctx()
            {      
                onClick = _ctx.onClick,
            });
        }
    }
}