using JumpUp.External;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.Video;
using JumpUp;
using System;

namespace JumpUp
{
    public class GamePm : BaseDisposable
    {
        public struct Ctx
        {
            public IReactiveProperty<GameState> _gameState;
            public IReadOnlyReactiveTrigger _onClick;
            public ReactiveTrigger GameOver;
            public ReactiveTrigger Finish;
            public ReactiveProperty<bool> needBigTutor;
            public ReactiveProperty<bool> interShowTime;
        }


        private readonly                    Ctx  _ctx;
        private                             GameState   currenGameState;

        public GamePm(Ctx Ctx)
        {
            _ctx = Ctx;
            AddUnsafe(Ctx._gameState.Subscribe(x => currenGameState = x));
            AddUnsafe(_ctx._onClick.Subscribe(GetClick));
        }


        private void GetClick()
        {
            switch (currenGameState)
            {
                case GameState.NONE:
                    break;
                case GameState.START:
                    if (!_ctx.interShowTime.Value)
                        _ctx._gameState.Value = _ctx.needBigTutor.Value == true? GameState.BIGTUTOR : GameState.COUNTER;
                    break;
                case GameState.PLAY:
                    break;
                case GameState.FINISH:
                       _ctx.Finish.Notify();
                    break;
                case GameState.GAMEOVER:
                      _ctx.GameOver.Notify();
                    break;
                case GameState.TUTOR:

                    break;
            }
        }

    }
}
