using JumpUp.External;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;


namespace JumpUp
{
    public class GameInput : BaseDisposable
    {
        public struct Ctx
        {
          public  ReactiveTrigger _onClick;
        }

        private Ctx _ctx;

        public GameInput(Ctx Ctx)
        {
            _ctx = Ctx;
            AddUnsafe(Observable.EveryUpdate().Where(x => Input.GetMouseButtonDown(0)).Subscribe(x => { _ctx._onClick.Notify(); }));
            AddUnsafe(Observable.EveryUpdate().Where(x => Input.GetMouseButtonDown(0)).Subscribe(x => { _ctx._onClick.Notify(); }));
        }
        
            
    }
}
