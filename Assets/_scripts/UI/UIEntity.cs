using _scripts.UI.Input;
using DigitalRubyShared;
using JumpUp;
using JumpUp.Content;
using JumpUp.External;
using UniRx;
using UnityEngine;

namespace _scripts.UI
{
    public class UIEntity : BaseDisposable
    {
        public struct Ctx
        {
            public IContent content;
            public ReactiveProperty<GameState> gameState;
            public GameInfoView gameInfoView;
            public ReactiveProperty<int> _levelIndex;
            public InputView inputview;
            public ReactiveEvent<Vector2> moveCoor;
            public ReactiveTrigger _onClick;
            public ReactiveProperty<Transform> player;
            public FingersJoystickScript controll;
            public ReactiveTrigger showTutor;
            public ReactiveProperty<string> winnerName;
            public ReactiveTrigger go;
            public ReactiveProperty<int> levelCounter;
            public ReactiveProperty<int> playersRacePlace;
        }

        private Ctx _ctx;

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
                _onClick = _ctx._onClick,
            };
            _ctx.inputview.SetMain(inputCtx);

            var gameInfoCtx = new GameInfoView.Ctx()
            {
                gameState = _ctx.gameState,
                _levelIndex = _ctx._levelIndex,
                showTutor = _ctx.showTutor,
                winnerName = _ctx.winnerName,
                go = _ctx.go,
                levelCounter = _ctx.levelCounter,
                playersRacePlace = _ctx.playersRacePlace
            };
            _ctx.gameInfoView.SetMain(gameInfoCtx);

        }
    }
}
