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
        public struct Context
        {
            public ReactiveProperty<GameState> gameState;
            public ReactiveEvent<Vector2> moveCoordinates;
            public ReactiveTrigger onClick;
            public FingersJoystickScript controll;
            public Transform uiCanvas;
            public IContent content;
        }
        
        private Context _context;
        private ClickInputView _clickInputView;


        public ControlEntity(Context context)
        {
            _context = context;
            CreateViews();
        }

        private void CreateViews()
        {
            var fingerContext = new FingersJoystickScript.Context()
            {
                moveCoordinates = _context.moveCoordinates,
                gameState = _context.gameState
            };
            _context.controll.Init(fingerContext);
            
            _clickInputView = Object.Instantiate(_context.content.GetClickInputView(), _context.uiCanvas);
            _clickInputView .Init(new ClickInputView.Context()
            {      
                onClick = _context.onClick,
            });
        }
    }
}